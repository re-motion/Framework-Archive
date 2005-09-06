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

/// <summary> This interface represents a page that can be used in a <see cref="WxePageStep"/>. </summary>
/// <include file='doc\include\ExecutionEngine\IWxePage.xml' path='IWxePage/Class/*' />
public interface IWxePage: IPage, IWxeTemplateControl
{
  /// <summary> Gets the post back data for the page. </summary>
  /// <remarks> Application developers should only rely on this collection for accessing the post back data. </remarks>
  NameValueCollection GetPostBackCollection ();

  /// <summary> End this page step and continue with the WXE function. </summary>
  void ExecuteNextStep ();

  /// <summary> Executes a WXE function in another window or frame. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='IWxePage/ExecuteFunction/param[@name="function" or @name="target" or @name="sender" or @name="returningPostback"]' />
  void ExecuteFunction (WxeFunction function, string target, Control sender, bool returningPostback);
  
  /// <summary> Executes a WXE function in another window or frame. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='IWxePage/ExecuteFunction/param[@name="function" or @name="target" or @name="features" or @name="sender" or @name="returningPostback"]' />
  void ExecuteFunction (WxeFunction function, string target, string features, Control sender, bool returningPostback);

  /// <summary> Executes a WXE function in the current window. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='IWxePage/ExecuteFunction/param[@name="function"]' />
  void ExecuteFunction (WxeFunction function);

  /// <summary>
  ///   Executes a WXE function in the current window without triggering the current post back event on returning.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='IWxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender"]' />
  void ExecuteFunctionNoRepost (WxeFunction function, Control sender);
  
  /// <summary>
  ///   Executes a WXE function in the current window without triggering the current post back event on returning.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='IWxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="usesEventTarget"]' />
  void ExecuteFunctionNoRepost (WxeFunction function, Control sender, bool usesEventTarget);

  /// <summary> Gets a flag describing whether this post back has been triggered by returning from a WXE function. </summary>
  bool IsReturningPostBack { get; }

  /// <summary> Gets the WXE function that has been executed in the current page. </summary>
  WxeFunction ReturningFunction { get; }

  /// <summary>
  ///   Gets or sets a flag that determines whether to display a confirmation dialog before aborting the session. 
  ///  </summary>
  /// <value> <see langowrd="true"/> to display the confirmation dialog. </value>
  bool IsAbortConfirmationEnabled { get; }

  /// <summary>
  ///   Gets or sets a flag that determines whether abort the session upon closing the window. 
  ///  </summary>
  /// <value> <see langowrd="true"/> to abort the session upon navigtion away from the page. </value>
  bool IsAbortEnabled { get; }

  /// <summary> Gets or sets the <see cref="HtmlForm"/> of the ASP.NET page. </summary>
  [EditorBrowsable (EditorBrowsableState.Never)]
  HtmlForm HtmlForm { get; set; }
}

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

    _wxeForm.LoadPostData +=new EventHandler(Form_LoadPostData);
    _page.PreRender +=new EventHandler(Page_PreRender);
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

/// <summary>
///   <b>WxePage</b> is the default implementation of the <see cref="IWxePage"/> interface. Use this type
///   a base class for pages that can be called by <see cref="WxePageStep"/>.
/// </summary>
/// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/Class/*' />
/// <seealso cref="IWxePage"/>
/// <seealso cref="ISmartNavigablePage"/>
public class WxePage: Page, IWxePage, ISmartNavigablePage
{
  #region IWxePage Impleplementation

  /// <summary> End this page step and continue with the WXE function. </summary>
  public void ExecuteNextStep ()
  {
    _wxeInfo.ExecuteNextStep();
  }

  /// <summary> Executes a WXE function in another window or frame. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='IWxePage/ExecuteFunction/param[@name="function" or @name="target" or @name="sender" or @name="returningPostback"]' />
  public void ExecuteFunction (WxeFunction function, string target, Control sender, bool returningPostback)
  {
    _wxeInfo.ExecuteFunction (function, target, sender, returningPostback);
  }

  /// <summary> Executes a WXE function in another window or frame. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='IWxePage/ExecuteFunction/param[@name="function" or @name="target" or @name="features" or @name="sender" or @name="returningPostback"]' />
  public void ExecuteFunction (WxeFunction function, string target, string features, Control sender, bool returningPostback)
  {
    _wxeInfo.ExecuteFunction (function, target, features, sender, returningPostback);
  }

  /// <summary> Executes a WXE function in another window or frame. </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='IWxePage/ExecuteFunction/param[@name="function"]' />
  public void ExecuteFunction (WxeFunction function)
  {
    _wxeInfo.ExecuteFunction (function);
  }

  /// <summary>
  ///   Executes the specified WXE function, then returns to this page without causing the current event again.
  /// </summary>
  /// <remarks>
  ///   This overload tries to determine automatically whether the current event was caused by the __EVENTTARGET field.
  /// </remarks>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='IWxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender"]' />
  public void ExecuteFunctionNoRepost (WxeFunction function, Control sender)
  {
    _wxeInfo.ExecuteFunctionNoRepost (function, sender);
  }

  /// <summary>
  ///   Executes the specified WXE function, then returns to this page without causing the current event again.
  /// </summary>
  /// <remarks>
  ///   This overload allows you to specify whether the current event was caused by the __EVENTTARGET field.
  ///   When in doubt, use <see cref="M:Rubicon.Web.ExecutionEngine.WxePage.ExecuteFunctionNoRepost(Rubicon.Web.ExecutionEngine.WxeFunction,System.Web.UI.Control)">ExecuteFunctionNoRepost(WxeFunction,Control)</see>.
  /// </remarks>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='IWxePage/ExecuteFunctionNoRepost/param[@name="function" or @name="sender" or @name="usesEventTarget"]' />
  public void ExecuteFunctionNoRepost (WxeFunction function, Control sender, bool usesEventTarget)
  {
    _wxeInfo.ExecuteFunctionNoRepost (function, sender, usesEventTarget);
  }

  /// <summary> Gets a flag describing whether this post back has been triggered by returning from a WXE function. </summary>
  [Browsable (false)]
  public bool IsReturningPostBack
  {
    get { return _wxeInfo.IsReturningPostBack; }
  }

  /// <summary> Gets the WXE function that has been executed in the current page. </summary>
  [Browsable (false)]
  public WxeFunction ReturningFunction
  {
    get { return _wxeInfo.ReturningFunction; }
  }

  #endregion

  private WxePageInfo _wxeInfo;
  private ValidatableControlInitializer _validatableControlInitializer;
  private PostLoadInvoker _postLoadInvoker;
  private bool disposed;
  private NaBooleanEnum _enableAbortConfirmation = NaBooleanEnum.Undefined;
  private NaBooleanEnum _enableAbort = NaBooleanEnum.Undefined;
  private NaBooleanEnum _enableSmartScrolling = NaBooleanEnum.Undefined;
  private NaBooleanEnum _enableSmartFocusing = NaBooleanEnum.Undefined;

  // protected HtmlForm Form; - won't work in VS 2005

  public WxePage ()
  {
    _wxeInfo = new WxePageInfo (this);
    _validatableControlInitializer = new ValidatableControlInitializer (this);
    _postLoadInvoker = new PostLoadInvoker (this);
    disposed = false;
  }

  protected override NameValueCollection DeterminePostBackMode()
  {
    NameValueCollection result = _wxeInfo.EnsurePostBackModeDetermined (Context);
    _wxeInfo.Initialize (Context);
    OnPreInit();
    OnBeforeInit();
    return result;
  }

  /// <summary> Called before the initialization phase of the page. </summary>
  /// <remarks> 
  ///   In ASP.NET 1.1 this method is called by <b>DeterminePostBackMode</b>. Therefor you should not use
  ///   the postback collection during pre init.
  /// </remarks>
  protected virtual void OnPreInit ()
  {
  }

  [Obsolete ("Use OnPreInit instead.") ]
  protected virtual void OnBeforeInit()
  {
  }
  protected override void SavePageStateToPersistenceMedium (object viewState)
  {
    if (WebConfiguration.Current.ExecutionEngine.ViewStateInSession)
      CurrentStep.SavePageStateToPersistenceMedium (viewState);
    else
      base.SavePageStateToPersistenceMedium (viewState);
  }

  protected override object LoadPageStateFromPersistenceMedium()
  {
    if (WebConfiguration.Current.ExecutionEngine.ViewStateInSession)
      return CurrentStep.LoadPageStateFromPersistenceMedium ();
    else
      return base.LoadPageStateFromPersistenceMedium ();
  }

  /// <summary> Makes sure that PostLoad is called on all controls that support <see cref="ISupportsPostLoadControl"/>. </summary>
  public void EnsurePostLoadInvoked ()
  {
    _postLoadInvoker.EnsurePostLoadInvoked();
  }

  /// <summary> Makes sure that all validators are registered with their <see cref="IValidatableControl"/> controls. </summary>
  public void EnsureValidatableControlsInitialized ()
  {
    _validatableControlInitializer.EnsureValidatableControlsInitialized ();
  }

  /// <summary>
  ///   Call this method before validating when using <see cref="Rubicon.Web.UI.Controls.FormGridManager"/> 
  ///   and <see cref="M:Rubicon.ObjectBinding.Web.Controls.IBusinessObjectDataSourceControl.Validate()"/>.
  /// </summary>
  public void PrepareValidation()
  {
    EnsurePostLoadInvoked();
    EnsureValidatableControlsInitialized();
  }

  /// <summary> Gets the post back data for the page. </summary>
  public NameValueCollection GetPostBackCollection ()
  {
    return _wxeInfo.EnsurePostBackModeDetermined (Context);
  }

  /// <summary> Gets the <see cref="WxePageStep"/> that called this <see cref="WxePage"/>. </summary>
  [Browsable (false)]
  public WxePageStep CurrentStep
  {
    get { return _wxeInfo.CurrentStep; }
  }
  
  /// <summary> Gets the <see cref="WxeFunction"/> of which the <see cref="CurrentStep"/> is a part. </summary>
  /// <value> 
  ///   A <see cref="WxeFunction"/> or <see langwrpd="null"/> if the <see cref="CurrentStep"/> is not part of a
  ///   <see cref="WxeFunction"/>.
  /// </value>
  [Browsable (false)]
  public WxeFunction CurrentFunction
  {
    get { return _wxeInfo.CurrentFunction; }
  }

  /// <summary> Gets the <see cref="WxeStep.Variables"/> collection of the <see cref="CurrentStep"/>. </summary>
  /// <value> 
  ///   A <see cref="NameObjectCollection"/> or <see langword="null"/> if the step is not part of a 
  ///   <see cref="WxeFunction"/>
  /// </value>
  [Browsable (false)]
  public NameObjectCollection Variables 
  {
    get { return _wxeInfo.Variables; }
  }

  /// <summary> Gets the <see cref="WxeForm"/> of this page. </summary>
  protected WxeForm WxeForm
  {
    get { return _wxeInfo.WxeForm; }
  }

  /// <summary> Gets or sets the <b>HtmlForm</b> of this page. </summary>
  /// <remarks> Redirects the call to the <see cref="HtmlForm"/> property. </remarks>
  [EditorBrowsable (EditorBrowsableState.Never)]
  HtmlForm IWxePage.HtmlForm
  {
    get { return HtmlForm; }
    set { HtmlForm = value; }
  }

  /// <summary> Gets or sets the <b>HtmlForm</b> of this page. </summary>
  /// <remarks>
  ///   <note type="inheritinfo"> 
  ///     Override this property you do not wish to rely on automatic detection of the <see cref="HtmlForm"/>
  ///     using reflection.
  ///   </note>
  /// </remarks>
  [EditorBrowsable (EditorBrowsableState.Never)]
  protected virtual HtmlForm HtmlForm
  {
    get { return _wxeInfo.HtmlForm; }
    set { _wxeInfo.HtmlForm = value; }
  }

  /// <summary> Disposes the page. </summary>
  /// <remarks>
  ///   <b>Dispose</b> is part of the ASP.NET page execution life cycle. It does not actually implement the 
  ///   disposeable pattern.
  ///   <note type="inheritinfo">
  ///     Do not override this method. Use <see cref="M:Rubicon.Web.ExecutionEngine.WxePage.Dispose(System.Boolean)">Dispose(Boolean)</see>
  ///     instead.
  ///   </note>
  /// </remarks>
  public override void Dispose()
  {
    base.Dispose ();
    if (! disposed)
    {
      Dispose (true);
      disposed = true;
      _wxeInfo.Dispose();
    }
  }

  /// <summary> Disposes the page. </summary>
  protected virtual void Dispose (bool disposing)
  {
  }

  /// <summary> 
  ///   Gets or sets the flag that determines whether to display a confirmation dialog before aborting the session. 
  /// </summary>
  /// <value> 
  ///   <see cref="NaBooleanEnum.True"/> to display a confirmation dialog. 
  ///   Defaults to <see cref="NaBooleanEnum.Undefined"/>, which is interpreted as <see cref="NaBooleanEnum.False"/>.
  /// </value>
  /// <remarks>
  ///   Use <see cref="IsAbortConfirmationEnabled"/> to evaluate this property.
  /// </remarks>
  [Description("The flag that determines whether to display a confirmation dialog before aborting the session. Undefined is interpreted as false.")]
  [Category ("Behavior")]
  [DefaultValue (NaBooleanEnum.Undefined)]
  public virtual NaBooleanEnum EnableAbortConfirmation
  {
    get { return _enableAbortConfirmation; }
    set { _enableAbortConfirmation = value; }
  }

  /// <summary> Gets the evaluated value for the <see cref="EnableAbortConfirmation"/> property. </summary>
  /// <value> <see langowrd="true"/> if <see cref="EnableAbortConfirmation"/> is <see cref="NaBooleanEnum.True"/>. </value>
  protected virtual bool IsAbortConfirmationEnabled
  {
    get { return _enableAbortConfirmation == NaBooleanEnum.True; }
  }

  /// <summary> Implementation of <see cref="IWxePage.IsAbortConfirmationEnabled"/>. </summary>
  /// <value> The value returned by <see cref="IsAbortConfirmationEnabled"/>. </value>
  bool IWxePage.IsAbortConfirmationEnabled
  {
    get { return IsAbortConfirmationEnabled; }
  }

  /// <summary> Gets or sets the flag that determines whether to abort the session upon closing the window. </summary>
  /// <value> 
  ///   <see cref="NaBooleanEnum.True"/> to abort the session. 
  ///   Defaults to <see cref="NaBooleanEnum.Undefined"/>, which is interpreted as <see cref="NaBooleanEnum.True"/>.
  /// </value>
  /// <remarks>
  ///   Use <see cref="IsAbortEnabled"/> to evaluate this property.
  /// </remarks>
  [Description("The flag that determines whether to abort the session when the window is closed. Undefined is interpreted as true.")]
  [Category ("Behavior")]
  [DefaultValue (NaBooleanEnum.Undefined)]
  public virtual NaBooleanEnum EnableAbort
  {
    get { return _enableAbort; }
    set { _enableAbort = value; }
  }

  /// <summary> Gets the evaluated value for the <see cref="EnableAbort"/> property. </summary>
  /// <value>
  ///   <see langowrd="false"/> if <see cref="EnableAbort"/> is <see cref="NaBooleanEnum.False"/>. 
  /// </value>
  protected virtual bool IsAbortEnabled
  {
    get { return _enableAbort != NaBooleanEnum.False; }
  }

  /// <summary> Implementation of <see cref="IWxePage.IsAbortEnabled"/>. </summary>
  /// <value> The value returned by <see cref="IsAbortEnabled"/>. </value>
  bool IWxePage.IsAbortEnabled
  {
    get { return IsAbortEnabled; }
  }

  /// <summary> Gets or sets the flag that determines whether to use smart scrolling. </summary>
  /// <value> 
  ///   <see cref="NaBooleanEnum.True"/> to use smart scrolling. 
  ///   Defaults to <see cref="NaBooleanEnum.Undefined"/>, which is interpreted as <see cref="NaBooleanEnum.True"/>.
  /// </value>
  /// <remarks>
  ///   Use <see cref="IsSmartScrollingEnabled"/> to evaluate this property.
  /// </remarks>
  [Description("The flag that determines whether to use smart scrolling. Undefined is interpreted as true.")]
  [Category ("Behavior")]
  [DefaultValue (NaBooleanEnum.Undefined)]
  public virtual NaBooleanEnum EnableSmartScrolling
  {
    get { return _enableSmartScrolling; }
    set { _enableSmartScrolling = value; }
  }

  /// <summary> Gets the evaluated value for the <see cref="EnableSmartScrolling"/> property. </summary>
  /// <value> 
  ///   <see langowrd="false"/> if <see cref="EnableSmartScrolling"/> is <see cref="NaBooleanEnum.False"/>
  ///   or the <see cref="SmartNavigationConfiguration.EnableScrolling"/> configuration setting is 
  ///   <see langword="false"/>.
  /// </value>
  protected virtual bool IsSmartScrollingEnabled
  {
    get
    {
      if (! WebConfiguration.Current.SmartNavigation.EnableScrolling)
        return false;
      return _enableSmartScrolling != NaBooleanEnum.False; 
    }
  }

  /// <summary> Implementation of <see cref="ISmartNavigablePage.IsSmartScrollingEnabled"/>. </summary>
  /// <value> The value returned by <see cref="IsSmartScrollingEnabled"/>. </value>
  bool ISmartNavigablePage.IsSmartScrollingEnabled
  {
    get { return IsSmartScrollingEnabled; }
  }

  /// <summary> Gets or sets the flag that determines whether to use smart navigation. </summary>
  /// <value> 
  ///   <see cref="NaBooleanEnum.True"/> to use smart navigation. 
  ///   Defaults to <see cref="NaBooleanEnum.Undefined"/>, which is interpreted as <see cref="NaBooleanEnum.True"/>.
  /// </value>
  /// <remarks>
  ///   Use <see cref="IsSmartFocusingEnabled"/> to evaluate this property.
  /// </remarks>
  [Description("The flag that determines whether to use smart navigation. Undefined is interpreted as true.")]
  [Category ("Behavior")]
  [DefaultValue (NaBooleanEnum.Undefined)]
  public virtual NaBooleanEnum EnableSmartFocusing
  {
    get { return _enableSmartFocusing; }
    set { _enableSmartFocusing = value; }
  }

  /// <summary> Gets the evaluated value for the <see cref="EnableSmartFocusing"/> property. </summary>
  /// <value> 
  ///   <see langowrd="false"/> if <see cref="EnableSmartFocusing"/> is <see cref="NaBooleanEnum.False"/>
  ///   or the <see cref="SmartNavigationConfiguration.EnableFocusing"/> configuration setting is 
  ///   <see langword="false"/>.
  /// </value>
  protected virtual bool IsSmartFocusingEnabled
  {
    get
    {
      if (! WebConfiguration.Current.SmartNavigation.EnableFocusing)
        return false;
      return _enableSmartFocusing != NaBooleanEnum.False; 
    }
  }

  /// <summary> Implementation of <see cref="ISmartNavigablePage.IsSmartFocusingEnabled"/>. </summary>
  /// <value> The value returned by <see cref="IsSmartFocusingEnabled"/>. </value>
  bool ISmartNavigablePage.IsSmartFocusingEnabled
  {
    get { return IsSmartFocusingEnabled; }
  }

  /// <summary> Clears scrolling and focus information on the page. </summary>
  public void DiscardSmartNavigationData ()
  {
    _wxeInfo.DiscardSmartNavigationData();
  }

  /// <summary> Sets the focus to the passed control. </summary>
  /// <param name="control"> The <see cref="IFocusableControl"/> to assign the focus to. </param>
  public void SetFocus (IFocusableControl control)
  {
    _wxeInfo.SetFocus (control);
  }

  /// <summary> Sets the focus to the passed control ID. </summary>
  /// <param name="id"> The client side ID of the control to assign the focus to. </param>
  public void SetFocus (string id)
  {
    _wxeInfo.SetFocus (id);
  }
}

}
