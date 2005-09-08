using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;
using System.Globalization;
using System.ComponentModel;
using System.Reflection;
using Rubicon.NullableValueTypes;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Collections;
using Rubicon.Globalization;
using Rubicon.Web.UI.Globalization;
using Rubicon.Web.UI;
using Rubicon.Web.Utilities;
using Rubicon.Web.Configuration;
using Rubicon.Utilities;

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

  public WxePageInfo (IWxePage page)
  {
    ArgumentUtility.CheckNotNullAndType ("page", page, typeof (Page));
    _page = page;
  }

  public void Initialize (HttpContext context)
  {
    ArgumentUtility.CheckNotNull ("context", context);
    base.Initialize (_page, context);

    if (! ControlHelper.IsDesignMode (_page, context))
    {
      _wxeForm = WxeForm.Replace (_page.HtmlForm);
      _page.HtmlForm = _wxeForm;
    }

    if (_page.CurrentStep != null)
      _page.RegisterHiddenField (WxePageInfo.PageTokenID, _page.CurrentStep.PageToken);

    _wxeForm.LoadPostData += new EventHandler(Form_LoadPostData);
    _page.PreRender += new EventHandler(Page_PreRender);
  }

  private void Form_LoadPostData (object sender, EventArgs e)
  {
    NameValueCollection postBackCollection = _page.GetPostBackCollection();
    if (postBackCollection == null)
      throw new InvalidOperationException ("The IWxePage has no PostBackCollection even though this is a post back.");
    HandleLoadPostData (postBackCollection);
  }

  protected virtual void HandleLoadPostData (NameValueCollection postBackCollection)
  {
    ArgumentUtility.CheckNotNull ("postBackCollection", postBackCollection);

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
    PreRenderWxe();
    PreRenderSmartNavigation();
  }

  protected void PreRenderWxe()
  {
    WxeContext wxeContext = WxeContext.Current;
    Page page = (Page) _page;
    
    page.RegisterHiddenField (WxeHandler.Parameters.WxeFunctionToken, wxeContext.FunctionToken);
    page.RegisterHiddenField (WxePageInfo.ReturningTokenID, null);

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
      string resumePath = wxeContext.GetResumePath (false);

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
   
    string key = "wxeInitialize";
    PageUtility.RegisterStartupScriptBlock ((Page)_page, key,
          "Wxe_Initialize ('" + _wxeForm.ClientID + "', " 
        + refreshIntervall.ToString() + ", " + refreshPath + ", " 
        + abortPath + ", " + abortMessage + ", "
        + smartScrollingFieldID + ", " + smartFocusFieldID + ");");
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
  ///   Implements <see cref="M:Rubicon.Web.ExecutionEngine.IWxePage.ExecuteFunction(Rubicon.Web.ExecutionEngine.WxeFunction,System.String,System.Web.UI.Control,System.Boolean)">IWxePage.ExecuteFunction(WxeFunction,String,Control,Boolean)</see>.
  /// </summary>
  public void ExecuteFunction (WxeFunction function, string target, Control sender, bool returningPostback)
  {
    ExecuteFunction (function, target, null, sender, returningPostback);
  }

  /// <summary>
  ///   Implements <see cref="M:Rubicon.Web.ExecutionEngine.IWxePage.ExecuteFunction(Rubicon.Web.ExecutionEngine.WxeFunction,System.String,System.String,System.Web.UI.Control,System.Boolean)">IWxePage.ExecuteFunction(WxeFunction,String,String,Control,Boolean)</see>.
  /// </summary>
  public void ExecuteFunction (WxeFunction function, string target, string features, Control sender, bool returningPostback)
  {
    bool enableCleanUp = !returningPostback;
    WxeFunctionState functionState = new WxeFunctionState (function, enableCleanUp);
    WxeFunctionStateCollection functionStates = WxeFunctionStateCollection.Instance;
    functionStates.Add (functionState);

    string href = WxeContext.GetResumePath (_page.Request, _page.Response, functionState.FunctionToken, true);
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
            "if (window.opener && window.opener.wxeDoPostBack && window.opener.document.getElementById('{0}') && window.opener.document.getElementById('{0}').value == '{1}') \n"
          + "  window.opener.wxeDoPostBack('{2}', '{3}', '{4}'); \n"
          + "window.close();", 
          WxePageInfo.PageTokenID,
          _page.CurrentStep.PageToken,
          eventtarget, 
          eventargument, 
          functionState.FunctionToken);
    }
    else
    {
      returnScript = string.Format (
            "if (window.opener && window.opener.wxeDoSubmit && window.opener.document.getElementById('{0}') && window.opener.document.getElementById('{0}').value == '{1}') \n"
          + "  window.opener.wxeDoSubmit('{2}', '{3}'); \n"
          + "window.close();", 
          WxePageInfo.PageTokenID,
          _page.CurrentStep.PageToken,
          sender.ClientID, 
          functionState.FunctionToken);
    }
    function.ReturnUrl = "javascript:" + returnScript;
  }

  /// <summary>
  ///   Implements <see cref="M:Rubicon.Web.ExecutionEngine.IWxePage.ExecuteFunction(Rubicon.Web.ExecutionEngine.WxeFunction)">IWxePage.ExecuteFunction(WxeFunction)</see>.
  /// </summary>
  public void ExecuteFunction (WxeFunction function)
  {
    CurrentStep.ExecuteFunction (_page, function);
  }

  /// <summary>
  ///   Implements <see cref="M:Rubicon.Web.ExecutionEngine.IWxePage.ExecuteFunctionNoRepost(Rubicon.Web.ExecutionEngine.WxeFunction,System.Web.UI.Control)">IWxePage.ExecuteFunctionNoRepost (WxeFunction,Control)</see>.
  /// </summary>
  public void ExecuteFunctionNoRepost (WxeFunction function, Control sender)
  {
    ExecuteFunctionNoRepost (function, sender, UsesEventTarget);
  }

  /// <summary>
  ///   Implements <see cref="M:Rubicon.Web.ExecutionEngine.IWxePage.ExecuteFunctionNoRepost(Rubicon.Web.ExecutionEngine.WxeFunction,System.Web.UI.Control,System.Boolean)">IWxePage.ExecuteFunctionNoRepost(WxeFunction,Control,Boolean)</see>.
  /// </summary>
  public void ExecuteFunctionNoRepost (WxeFunction function, Control sender, bool usesEventTarget)
  {
    CurrentStep.ExecuteFunctionNoRepost (_page, function, sender, usesEventTarget);
  }

  /// <summary> 
  ///   Gets a flag describing whether the post back was most likely caused by the ASP.NET post back mechanism.
  /// </summary>
  /// <value> <see langword="true"/> if the post back collection contains the <b>__EVENTTARGET</b> field. </value>
  protected bool UsesEventTarget
  {
    get { return ! StringUtility.IsNullOrEmpty (_page.GetPostBackCollection()[ControlHelper.PostEventSourceID]); }
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

  /// <summary> 
  ///   If <see cref="ExecuteNextStep"/> has been called prior to disposing the page, <b>Dispose</b> will
  ///   break execution of this page life cycle and allow the Execution Engine to continue with the next step.
  /// </summary>
  /// <remarks> 
  ///   <para>
  ///     If <see cref="ExecuteNextStep"/> has been called, <b>Dispose</b> slears the <see cref="HttpResonse"/> 
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
      if (_htmlFormField != null) // Can only be null without an exception during design mode
        return (HtmlForm) _htmlFormField.GetValue (_page);
      else
        return null;
    }
    set
    {
      EnsureHtmlFormFieldInitialized();
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
