using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Reflection;
using Rubicon.Collections;
using Rubicon.Globalization;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;
using Rubicon.Web.Configuration;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UI.Globalization;
using Rubicon.Web.Utilities;

namespace Rubicon.Web.ExecutionEngine
{

public class WxePageInfo: WxeTemplateControlInfo, IDisposable
{
  /// <summary> A list of resources. </summary>
  /// <remarks> 
  ///   Resources will be accessed using 
  ///   <see cref="M:Rubicon.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
  ///   See the documentation of <b>GetString</b> for further details.
  /// </remarks>
  [ResourceIdentifiers]
  [MultiLingualResources ("Rubicon.Web.Globalization.WxePageInfo")]
  protected enum ResourceIdentifier
  {
    AbortMessage
  }

  public static readonly string ReturningTokenID = "wxeReturningToken";
  public static readonly string PageTokenID = "wxePageToken";
  public static readonly string PostBackSequenceNumberID = "wxePostBackSequenceNumber";
  private const string c_smartScrollingID = "smartScrolling";
  private const string c_smartFocusID = "smartFocus";
  private const string c_script = "ExecutionEngine.js";
  private const string c_smartNavigationScript = "SmartNavigation.js";

  private IWxePage _page;
  private WxeForm _wxeForm;
  private bool _postbackCollectionInitialized = false;
  private NameValueCollection _postbackCollection = null;
  /// <summary> The <see cref="WxeFunctionState"/> designated by <b>WxeForm.ReturningToken</b>. </summary>
  private WxeFunctionState _returningFunctionState = null;
  private bool _isSmartNavigationDataDisacarded = false;
  private string _smartFocusID = null;

  private bool _executeNextStep = false;
  private HttpResponse _response; // used for cleanup in Dispose

  private bool _isPreRendering = false;
  private AutoInitHashtable _clientSideEventHandlers = new AutoInitHashtable (typeof (NameValueCollection));

  /// <summary> Initializes a new instance of the <b>WxePageInfo</b> type. </summary>
  /// <param name="page"> 
  ///   The <see cref="IWxePage"/> containing this <b>WxePageInfo</b> object. 
  ///   The page must be derived from <see cref="System.Web.UI.Page">System.Web.UI.Page</see>.
  /// </param>
  public WxePageInfo (IWxePage page)
  {
    ArgumentUtility.CheckNotNullAndType ("page", page, typeof (Page));
    _page = page;
  }

  public void Initialize (HttpContext context)
  {
    ArgumentUtility.CheckNotNull ("context", context);
    base.Initialize (_page, context);

    // TODO: .net 2.0
    //_httpContext = context;
    //_httpContext.Handler = _page;

    if (! ControlHelper.IsDesignMode (_page, context))
    {
      _wxeForm = WxeForm.Replace (_page.HtmlForm);
      _page.HtmlForm = _wxeForm;
    }

    if (_page.CurrentStep != null)
      _page.RegisterHiddenField (WxePageInfo.PageTokenID, CurrentStep.PageToken);

    _wxeForm.LoadPostData += new EventHandler(Form_LoadPostData);
    _page.PreRender += new EventHandler(Page_PreRender);
    // TODO: .net 2.0
    // _page.Unload += new EventHandler(Page_Unload);
  }

  // TODO: .net 2.0
  //void Page_Unload(object sender, EventArgs e)
  //{
  //  _httpContext.Handler = WxeHandler;
  //}

  private void Form_LoadPostData (object sender, EventArgs e)
  {
    NameValueCollection postBackCollection = _page.GetPostBackCollection();
    if (postBackCollection == null)
      throw new InvalidOperationException ("The IWxePage has no PostBackCollection even though this is a post back.");
    HandleLoadPostData (postBackCollection);
  }

  /// <exception cref="WxePostBackOutOfSequenceException"> 
  ///   Thrown if a postback with an incorrect sequence number is handled. 
  /// </exception>
  protected virtual void HandleLoadPostData (NameValueCollection postBackCollection)
  {
    ArgumentUtility.CheckNotNull ("postBackCollection", postBackCollection);

    int postBackID = int.Parse (postBackCollection[WxePageInfo.PostBackSequenceNumberID]);
    if (postBackID != WxeContext.Current.PostBackID)
      throw new WxePostBackOutOfSequenceException();

    string returningToken = postBackCollection[WxePageInfo.ReturningTokenID];
    if (! StringUtility.IsNullOrEmpty (returningToken))
    {
      WxeFunctionStateCollection functionStates = WxeFunctionStateCollection.Instance;
      WxeFunctionState functionState = functionStates.GetItem (returningToken);
      if (functionState != null)
      {
        WxeContext wxeContext = WxeContext.Current;
        wxeContext.ReturningFunction = functionState.Function;
        wxeContext.IsReturningPostBack = true;
        _returningFunctionState = functionState;
      }
    }
  }

  /// <summary> Handles the <b>PreRender</b> event of the page. </summary>
  private void Page_PreRender (object sender, EventArgs e)
  {
    _isPreRendering = true;
    
    PreRenderWxe();
    PreRenderSmartNavigation();
  }

  protected void PreRenderWxe()
  {
    WxeContext wxeContext = WxeContext.Current;
    Page page = (Page) _page;
    
    page.RegisterHiddenField (WxeHandler.Parameters.WxeFunctionToken, wxeContext.FunctionToken);
    page.RegisterHiddenField (WxePageInfo.ReturningTokenID, null);
    int nextPostBackID = wxeContext.PostBackID + 1;
    page.RegisterHiddenField (WxePageInfo.PostBackSequenceNumberID, nextPostBackID.ToString());

    string key = "wxeDoSubmit";
    PageUtility.RegisterClientScriptBlock (page, key,
          "function wxeDoSubmit (button, pageToken) { \r\n"
        + "  var theForm = document." + _wxeForm.ClientID + "; \r\n"
        + "  theForm." + WxePageInfo.ReturningTokenID + ".value = pageToken; \r\n"
        + "  document.getElementById(button).click(); \r\n"
        + "}");

    key = "wxeDoPostBack";
    PageUtility.RegisterClientScriptBlock (page, key,
          "function wxeDoPostBack (control, argument, returningToken) { \r\n"
        + "  var theForm = document." + _wxeForm.ClientID + "; \r\n"
        + "  theForm." + WxePageInfo.ReturningTokenID + ".value = returningToken; \r\n"
        + "  __doPostBack (control, argument); \r\n"
        + "}");

    key = "wxeScript";
    string url = ResourceUrlResolver.GetResourceUrl (page, typeof (WxePageInfo), ResourceType.Html, c_script);
    HtmlHeadAppender.Current.RegisterJavaScriptInclude (key, url);

    RegisterWxeInitializationScript(); 
  }

  protected void PreRenderSmartNavigation()
  {
    ISmartNavigablePage smartNavigablePage = _page as ISmartNavigablePage;
    if (smartNavigablePage == null)
      return;

    NameValueCollection postBackCollection = _page.GetPostBackCollection();
    Page page = (Page) _page;

    if (smartNavigablePage.IsSmartScrollingEnabled || smartNavigablePage.IsSmartFocusingEnabled)
    {
      string key = "smartNavigationScript";
      string url = ResourceUrlResolver.GetResourceUrl (
          page, typeof (WxePageInfo), ResourceType.Html, c_smartNavigationScript);
      HtmlHeadAppender.Current.RegisterJavaScriptInclude (key, url);
    }

    if (smartNavigablePage.IsSmartScrollingEnabled)
    {
      string smartScrollingValue = null;
      if (postBackCollection != null && !_isSmartNavigationDataDisacarded)
        smartScrollingValue = postBackCollection[c_smartScrollingID];
      page.RegisterHiddenField (c_smartScrollingID, smartScrollingValue);
    }

    if (smartNavigablePage.IsSmartFocusingEnabled)
    {
      string smartFocusValue = null;
      if (postBackCollection != null && !_isSmartNavigationDataDisacarded)
        smartFocusValue = postBackCollection[c_smartFocusID];
      if (! StringUtility.IsNullOrEmpty (_smartFocusID))
        smartFocusValue = _smartFocusID;
      page.RegisterHiddenField (c_smartFocusID, smartFocusValue);
    }
  }

  protected void RegisterWxeInitializationScript()
  {
    int refreshIntervall = 0;
    string refreshPath = "null";
    string abortPath = "null";
    string abortMessage = "null";
    if (WxeHandler.IsSessionManagementEnabled)
    {
      //  Ensure the registration of "__doPostBack" on the page.
      string temp = _page.GetPostBackEventReference ((Page)_page);

      bool isAbortConfirmationEnabled = _page.IsAbortConfirmationEnabled;
      bool isAbortEnabled = _page.IsAbortEnabled;

      WxeContext wxeContext = WxeContext.Current;
      string resumePath = wxeContext.GetResumePath ();

      refreshIntervall = WxeHandler.RefreshInterval * 60000;
      refreshPath = "'" + resumePath + "&" + WxeHandler.Parameters.WxeAction + "=" + WxeHandler.Actions.Refresh + "'";
      
      if (isAbortEnabled)
        abortPath = "'" + resumePath + "&" + WxeHandler.Parameters.WxeAction + "=" + WxeHandler.Actions.Abort + "'";
      
      if (isAbortEnabled && isAbortConfirmationEnabled)
        abortMessage = "'" + GetResourceManager().GetString (ResourceIdentifier.AbortMessage) + "'";        

    }

    string smartScrollingFieldID = "null";
    string smartFocusFieldID = "null";

    ISmartNavigablePage smartNavigablePage = _page as ISmartNavigablePage;
    if (smartNavigablePage != null)
    {
      if (smartNavigablePage.IsSmartScrollingEnabled)
        smartScrollingFieldID = "'" + c_smartScrollingID + "'";
      if (smartNavigablePage.IsSmartFocusingEnabled)
        smartFocusFieldID = "'" + c_smartFocusID + "'";
    }
   
    StringBuilder initScript = new StringBuilder (500);

    initScript.Append ("var _wxe_eventHandlers = new Array(); \r\n");
    initScript.Append ("var _wxe_eventHandlersByEvent = null; \r\n");
    initScript.Append ("\r\n");

    foreach (WxePageEvents pageEvent in _clientSideEventHandlers.Keys)
    {
      NameValueCollection eventHandlers = (NameValueCollection) _clientSideEventHandlers[pageEvent];

      initScript.Append ("_wxe_eventHandlersByEvent = new Array(); \r\n");

      for (int i = 0; i < eventHandlers.Keys.Count; i++)
      {
        initScript.Append ("_wxe_eventHandlersByEvent.push ('");
        initScript.Append (eventHandlers.Get (i));
        initScript.Append ("'); \r\n");
      }
      
      initScript.Append ("_wxe_eventHandlers['");
      initScript.Append (pageEvent.ToString().ToLower());
      initScript.Append ("'] = _wxe_eventHandlersByEvent; \r\n");
      initScript.Append ("\r\n");
    }

    initScript.Append ("Wxe_Initialize ('");
    initScript.Append (_wxeForm.ClientID).Append ("', ");
    initScript.Append (refreshIntervall).Append (", ");
    initScript.Append (refreshPath).Append (", ");
    initScript.Append (abortPath).Append (", ");
    initScript.Append (abortMessage).Append (", ");
    initScript.Append (smartScrollingFieldID).Append (", ");
    initScript.Append (smartFocusFieldID).Append (", ");
    initScript.Append ("_wxe_eventHandlers); \r\n");

    initScript.Append ("\r\n");
    initScript.Append ("_wxe_eventHandlers = null; \r\n");
    initScript.Append ("_wxe_eventHandlersByEvent = null;");

    string key = "wxeInitialize";
    PageUtility.RegisterClientScriptBlock ((Page)_page, key, initScript.ToString());
  }

  public NameValueCollection EnsurePostBackModeDetermined (HttpContext context)
  {
    ArgumentUtility.CheckNotNull ("context", context);

    if (! _postbackCollectionInitialized)
    {
      _postbackCollection = DeterminePostBackMode (context);
      _postbackCollectionInitialized = true;
    }
    return _postbackCollection;
  }  

  private NameValueCollection DeterminePostBackMode (HttpContext httpContext)
  {
    WxeContext wxeContext = WxeContext.Current;
    if (wxeContext == null)
      return null;
    if (! wxeContext.IsPostBack)
      return null;
    if (wxeContext.PostBackCollection != null)
      return wxeContext.PostBackCollection;
    if (httpContext.Request == null)
      return null;

    NameValueCollection collection;
    if (StringUtility.AreEqual (httpContext.Request.HttpMethod, "POST", false))
      collection = httpContext.Request.Form;
    else
      collection = httpContext.Request.QueryString;

    if ((collection[ControlHelper.ViewStateID] == null) && (collection[ControlHelper.PostEventSourceID] == null))
      return null;
    else
      return collection;
  }

  /// <summary> Implements <see cref="IWxePage.ExecuteNextStep">IWxePage.ExecuteNextStep</see>. </summary>
  public void ExecuteNextStep ()
  {
    _executeNextStep = true;
    _response = _page.Response;
    _page.Visible = false; // suppress prerender and render events
  }


  /// <summary>
  ///   Implements <see cref="M:Rubicon.Web.ExecutionEngine.IWxePage.ExecuteFunction(Rubicon.Web.ExecutionEngine.WxeFunction)">IWxePage.ExecuteFunction(WxeFunction)</see>.
  /// </summary>
  public void ExecuteFunction (WxeFunction function)
  {
    ExecuteFunction (function, false, false);
  }

  /// <summary>
  ///   Implements <see cref="M:Rubicon.Web.ExecutionEngine.IWxePage.ExecuteFunction(Rubicon.Web.ExecutionEngine.WxeFunction,System.Boolean,System.Boolean)">IWxePage.ExecuteFunction(WxeFunction,Boolean,Boolean)</see>.
  /// </summary>
  public void ExecuteFunction (WxeFunction function, bool createPermaUrl, bool useParentPermaUrl)
  {
    CurrentStep.ExecuteFunction (_page, function, createPermaUrl);
  }

  /// <summary>
  ///   Implements <see cref="M:Rubicon.Web.ExecutionEngine.IWxePage.ExecuteFunctionNoRepost(Rubicon.Web.ExecutionEngine.WxeFunction,System.Web.UI.Control)">IWxePage.ExecuteFunctionNoRepost (WxeFunction,Control)</see>.
  /// </summary>
  public void ExecuteFunctionNoRepost (WxeFunction function, Control sender)
  {
    ExecuteFunctionNoRepost (function, sender, UsesEventTarget, false, false);
  }

  /// <summary>
  ///   Implements <see cref="M:Rubicon.Web.ExecutionEngine.IWxePage.ExecuteFunctionNoRepost(Rubicon.Web.ExecutionEngine.WxeFunction,System.Web.UI.Control,System.Boolean)">IWxePage.ExecuteFunctionNoRepost(WxeFunction,Control,Boolean)</see>.
  /// </summary>
  public void ExecuteFunctionNoRepost (WxeFunction function, Control sender, bool usesEventTarget)
  {
    ExecuteFunctionNoRepost (function, sender, usesEventTarget, false, false);
  }

  /// <summary>
  ///   Implements <see cref="M:Rubicon.Web.ExecutionEngine.IWxePage.ExecuteFunctionNoRepost(Rubicon.Web.ExecutionEngine.WxeFunction,System.Web.UI.Control,System.Boolean,System.Boolean)">IWxePage.ExecuteFunctionNoRepost (WxeFunction,Control,Boolean,Boolean)</see>.
  /// </summary>
  public void ExecuteFunctionNoRepost (
      WxeFunction function, Control sender, bool createPermaUrl, bool useParentPermaUrl)
  {
    ExecuteFunctionNoRepost (function, sender, UsesEventTarget, createPermaUrl, useParentPermaUrl);
  }

  /// <summary>
  ///   Implements <see cref="M:Rubicon.Web.ExecutionEngine.IWxePage.ExecuteFunctionNoRepost(Rubicon.Web.ExecutionEngine.WxeFunction,System.Web.UI.Control,System.Boolean,System.Boolean,System.Boolean)">IWxePage.ExecuteFunctionNoRepost(WxeFunction,Control,Boolean,Boolean,Boolean)</see>.
  /// </summary>
  public void ExecuteFunctionNoRepost (
      WxeFunction function, Control sender, bool usesEventTarget, bool createPermaUrl, bool useParentPermaUrl)
  {
    CurrentStep.ExecuteFunctionNoRepost (_page, function, sender, usesEventTarget, createPermaUrl);
  }

  /// <summary> 
  ///   Gets a flag describing whether the post back was most likely caused by the ASP.NET post back mechanism.
  /// </summary>
  /// <value> <see langword="true"/> if the post back collection contains the <b>__EVENTTARGET</b> field. </value>
  protected bool UsesEventTarget
  {
    get { return ! StringUtility.IsNullOrEmpty (_page.GetPostBackCollection()[ControlHelper.PostEventSourceID]); }
  }

  /// <summary>
  ///   Implements <see cref="M:Rubicon.Web.ExecutionEngine.IWxePage.ExecuteFunctionExternal(Rubicon.Web.ExecutionEngine.WxeFunction,System.String,System.Web.UI.Control,System.Boolean)">IWxePage.ExecuteFunctionExternal(WxeFunction,String,Control,Boolean)</see>.
  /// </summary>
  public void ExecuteFunctionExternal (WxeFunction function, string target, Control sender, bool returningPostback)
  {
    ExecuteFunctionExternal (function, target, null, sender, returningPostback, false, false);
  }

  /// <summary>
  ///   Implements <see cref="M:Rubicon.Web.ExecutionEngine.IWxePage.ExecuteFunctionExternal(Rubicon.Web.ExecutionEngine.WxeFunction,System.String,System.String,System.Web.UI.Control,System.Boolean)">IWxePage.ExecuteFunctionExternal(WxeFunction,String,String,Control,Boolean)</see>.
  /// </summary>
  public void ExecuteFunctionExternal (
      WxeFunction function, string target, string features, Control sender, bool returningPostback)
  {
    ExecuteFunctionExternal (function, target, features, sender, returningPostback, false, false);
  }

  /// <summary>
  ///   Implements <see cref="M:Rubicon.Web.ExecutionEngine.IWxePage.ExecuteFunctionExternal(Rubicon.Web.ExecutionEngine.WxeFunction,System.String,System.Web.UI.Control,System.Boolean,System.Boolean,System.Boolean)">IWxePage.ExecuteFunctionExternal(WxeFunction,String,Control,Boolean,Boolean,Boolean)</see>.
  /// </summary>
  public void ExecuteFunctionExternal (
      WxeFunction function, string target, Control sender, bool returningPostback, 
      bool createPermaUrl, bool useParentPermaUrl)
  {
    ExecuteFunctionExternal (function, target, null, sender, returningPostback, createPermaUrl, useParentPermaUrl);
  }

  /// <summary>
  ///   Implements <see cref="M:Rubicon.Web.ExecutionEngine.IWxePage.ExecuteFunctionExternal(Rubicon.Web.ExecutionEngine.WxeFunction,System.String,System.String,System.Web.UI.Control,System.Boolean,System.Boolean,System.Boolean)">IWxePage.ExecuteFunctionExternal(WxeFunction,String,String,Control,Boolean,Boolean,Boolean)</see>.
  /// </summary>
  public void ExecuteFunctionExternal (
      WxeFunction function, string target, string features, Control sender, bool returningPostback, 
      bool createPermaUrl, bool useParentPermaUrl)
  {
    ArgumentUtility.CheckNotNull ("function", function);

    WxeContext wxeContext = WxeContext.Current;
    HttpContext httpContext = wxeContext.HttpContext;


    UrlMapping.UrlMapping mapping = UrlMapping.UrlMappingConfiguration.Current.Mappings[function.GetType()];
    string path = null;
    if (mapping != null)
      path = mapping.Resource;
    
    string queryString = string.Empty;
    if (createPermaUrl)
    {
      if (mapping == null)
      {
        string functionTypeName = function.GetType().AssemblyQualifiedName;
        int separator = functionTypeName.IndexOf (',', 0);
        separator = functionTypeName.IndexOf (',', separator + 1);
        functionTypeName = functionTypeName.Substring (0, separator);
        functionTypeName = functionTypeName.Replace (" ", "");
        queryString = PageUtility.AddUrlParameter (queryString, WxeHandler.Parameters.WxeFunctionType, functionTypeName);
      }

      NameValueCollection serializedParameters = function.SerializeParametersForQueryString ();
      foreach (string key in serializedParameters)
        queryString = PageUtility.AddUrlParameter (queryString, key, serializedParameters[key]);
    }
    else
    {
      queryString = wxeContext.QueryString;
    }

    bool enableCleanUp = !returningPostback;
    WxeFunctionState functionState = new WxeFunctionState (function, enableCleanUp);
    WxeFunctionStateCollection functionStates = WxeFunctionStateCollection.Instance;
    functionStates.Add (functionState);

    if (StringUtility.IsNullOrEmpty (path))
      path = httpContext.Request.Url.AbsolutePath;
    string href = WxeContext.GetResumePath (path, httpContext.Response, functionState.FunctionToken, queryString);

    string openScript;
    if (features != null)
      openScript = string.Format ("window.open('{0}', '{1}', '{2}');", href, target, features);
    else
      openScript = string.Format ("window.open('{0}', '{1}');", href, target);
    PageUtility.RegisterStartupScriptBlock ((Page)_page, "WxeExecuteFunction", openScript);

    string returnScript;
    if (! returningPostback)
    {
      returnScript = "window.close();";
    }
    else if (UsesEventTarget)
    {
      string eventtarget = _page.GetPostBackCollection()[ControlHelper.PostEventSourceID];
      string eventargument = _page.GetPostBackCollection()[ControlHelper.PostEventArgumentID ];
      returnScript = string.Format (
            "\r\n"
          + "if (   window.opener != null \r\n"
          + "    && ! window.opener.closed \r\n"
          + "    && window.opener.wxeDoPostBack != null \r\n"
          + "    && window.opener.document.getElementById('{0}') != null \r\n"
          + "    && window.opener.document.getElementById('{0}').value == '{1}') \r\n"
          + "{{ \r\n"
          + "  window.opener.wxeDoPostBack('{2}', '{3}', '{4}'); \r\n"
          + "}} \r\n"
          + "window.close(); \r\n",
          WxePageInfo.PageTokenID,
          _page.CurrentStep.PageToken,
          eventtarget, 
          eventargument, 
          functionState.FunctionToken);
    }
    else
    {
      if (! (sender is IPostBackEventHandler || sender is IPostBackDataHandler))
        throw new ArgumentException ("The sender must implement either IPostBackEventHandler or IPostBackDataHandler. Provide the control that raised the post back event.");

      returnScript = string.Format (
            "\r\n"
          + "if (   window.opener != null \r\n"
          + "    && ! window.opener.closed \r\n"
          + "    && window.opener.wxeDoSubmit != null \r\n"
          + "    && window.opener.document.getElementById('{0}') != null \r\n"
          + "    && window.opener.document.getElementById('{0}').value == '{1}') \r\n"
          + "{{ \r\n"
          + "  window.opener.wxeDoSubmit('{2}', '{3}'); \r\n"
          + "}} \r\n"
          + "window.close(); \r\n",
          WxePageInfo.PageTokenID,
          _page.CurrentStep.PageToken,
          sender.ClientID, 
          functionState.FunctionToken);
    }
    function.ReturnUrl = "javascript:" + returnScript;
  }
  /// <summary> Implements <see cref="IWxePage.IsReturningPostBack">IWxePage.IsReturningPostBack</see>. </summary>
  public bool IsReturningPostBack
  {
    get 
    { 
      WxeContext wxeContext = WxeContext.Current;
      return ((wxeContext == null) ? false : wxeContext.IsReturningPostBack); 
    }
  }

  /// <summary> Implements <see cref="IWxePage.ReturningFunction">IWxePage.ReturningFunction</see>. </summary>
  public WxeFunction ReturningFunction
  {
    get 
    { 
      WxeContext wxeContext = WxeContext.Current;
      return ((wxeContext == null) ? null : wxeContext.ReturningFunction); 
    }
  }

  /// <summary> Implements <see cref="IWxePage.RegisterClientSidePageEventHandler">IWxePage.RegisterClientSidePageEventHandler</see>. </summary>
  public void RegisterClientSidePageEventHandler (WxePageEvents pageEvent, string key, string function)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("key", key);
    ArgumentUtility.CheckNotNullOrEmpty ("function", function);
    if (! System.Text.RegularExpressions.Regex.IsMatch (function, @"^([a-zA-Z_][a-zA-Z0-9_]*)$"))
      throw new ArgumentException ("Invalid function name: '" + function + "'.", "function");

    if (_isPreRendering)
      throw new InvalidOperationException ("RegisterClientSidePageEventHandler must not be called after the PreRender method of the System.Web.UI.Page has been invoked.");

    NameValueCollection eventHandlers = (NameValueCollection) _clientSideEventHandlers[pageEvent];
    eventHandlers[key] = function;
  }

  /// <summary> aves the viewstate into the executing <see cref="WxePageStep"/>. </summary>
  /// <param name="viewState"> An <b>ASP.NET</b> viewstate object. </param>
  public void SavePageStateToPersistenceMedium (object viewState)
  {
    CurrentStep.SavePageStateToPersistenceMedium (viewState);
  }

  /// <summary> Returns the viewstate previously saved into the executing <see cref="WxePageStep"/>. </summary>
  /// <returns> An <b>ASP.NET</b> viewstate object. </returns>
  public object LoadPageStateFromPersistenceMedium()
  {
    return CurrentStep.LoadPageStateFromPersistenceMedium ();
  }

  /// <summary> 
  ///   If <see cref="ExecuteNextStep"/> has been called prior to disposing the page, <b>Dispose</b> will
  ///   break execution of this page life cycle and allow the Execution Engine to continue with the next step.
  /// </summary>
  /// <remarks> 
  ///   <para>
  ///     If <see cref="ExecuteNextStep"/> has been called, <b>Dispose</b> clears the <see cref="HttpResponse"/>'s
  ///     output and ends the execution of the current step by throwing a <see cref="WxeExecuteNextStepException"/>. 
  ///     This exception is handled by the Execution Engine framework.
  ///   </para>
  ///   <note>
  ///     See the remarks section of <see cref="IWxePage"/> for details on calling <b>Dispose</b>.
  ///   </note>
  /// </remarks>
  public void Dispose ()
  {
    if (_returningFunctionState != null)
    {
      bool isRootFunction = _returningFunctionState.Function == _returningFunctionState.Function.RootFunction;
      if (isRootFunction)
        WxeFunctionStateCollection.Instance.Abort (_returningFunctionState);
    }

    if (_executeNextStep)
    {
      _response.Clear(); // throw away page trace output
      throw new WxeExecuteNextStepException();
    }
  }

  public WxeForm WxeForm
  {
    get { return _wxeForm; }
  }

  private FieldInfo _htmlFormField = null;
  private bool _htmlFormFieldInitialized = false;

  private void EnsureHtmlFormFieldInitialized()
  {
    if (! _htmlFormFieldInitialized)
    {
      bool isDesignMode = ControlHelper.IsDesignMode (_page);

      // TODO: .net 2.0
      //Control page = (Page)_page;
      //if (((Page)_page).Master != null)
      //  page = ((Page)_page).Master;

      MemberInfo[] fields = _page.GetType().FindMembers (
            MemberTypes.Field, 
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, 
            new MemberFilter (FindHtmlFormControlFilter), null);
      if (fields.Length < 1 && ! isDesignMode)
        throw new ApplicationException ("Page class " + _page.GetType().FullName + " has no field of type HtmlForm. Please add a field or override property IWxePage.HtmlForm.");
      else if (fields.Length > 1)
        throw new ApplicationException ("Page class " + _page.GetType().FullName + " has more than one field of type HtmlForm. Please remove excessive fields or override property IWxePage.HtmlForm.");
      if (fields.Length > 0) // Can only be null without an exception during design mode
      {
        _htmlFormField = (FieldInfo) fields[0];
        _htmlFormFieldInitialized = true;
      }
    }
  }

  private bool FindHtmlFormControlFilter (MemberInfo member, object filterCriteria)
  {
    return (member is FieldInfo && ((FieldInfo)member).FieldType == typeof (HtmlForm));
  }

  /// <summary> 
  ///   Implements <see cref="IWxePage.HtmlForm">IWxePage.HtmlForm</see>.
  /// </summary>
  public HtmlForm HtmlForm
  {
    get
    {
      EnsureHtmlFormFieldInitialized();

      // TODO: .net 2.0
      //Control page = (Page)_page;
      //if (((Page)_page).Master != null)
      //  page = ((Page)_page).Master;

      if (_htmlFormField != null) // Can only be null without an exception during design mode
        return (HtmlForm) _htmlFormField.GetValue (_page);
      else
        return null;
    }
    set
    {
      EnsureHtmlFormFieldInitialized();

      // TODO: .net 2.0
      //Control page = (Page)_page;
      //if (((Page)_page).Master != null)
      //  page = ((Page)_page).Master;

      if (_htmlFormField != null) // Can only be null without an exception during design mode
        _htmlFormField.SetValue (_page, value);
    }
  }

  /// <summary> Find the <see cref="IResourceManager"/> for this WxePageInfo. </summary>
  protected virtual IResourceManager GetResourceManager()
  {
    return GetResourceManager (typeof (ResourceIdentifier));
  }

  /// <summary>
  ///   Implements <see cref="M:Rubicon.Web.UI.ISmartNavigablePage.DiscardSmartNavigationData()">ISmartNavigablePage.DiscardSmartNavigationData()</see>.
  /// </summary>
  public void DiscardSmartNavigationData ()
  {
    _isSmartNavigationDataDisacarded = true;
  }

  /// <summary>
  ///   Implements <see cref="M:Rubicon.Web.UI.ISmartNavigablePage.SetFocus(Rubicon.Web.UI.Controls.IFocusableControl)">ISmartNavigablePage.SetFocus(IFocusableControl)</see>.
  /// </summary>
  public void SetFocus (IFocusableControl control)
  {
    ArgumentUtility.CheckNotNull ("control", control);
    if (StringUtility.IsNullOrEmpty (control.FocusID))
      return;
    SetFocus (control.FocusID);
  }

  /// <summary>
  ///   Implements <see cref="M:Rubicon.Web.UI.ISmartNavigablePage.SetFocus(System.String)">ISmartNavigablePage.SetFocus(String)</see>.
  /// </summary>
  public void SetFocus (string id)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("id", id);
    _smartFocusID = id;
  }
}

}
