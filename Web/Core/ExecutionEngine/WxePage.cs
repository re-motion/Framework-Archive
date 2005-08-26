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

public interface IWxePage: ISmartNavigablePage, IWxeTemplateControl
{
  NameValueCollection GetPostBackCollection ();
  void ExecuteNextStep ();

  /// <summary>
  ///   Executes a WXE function in another window or frame.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionExternal/param[@name!="features"]' />
  void ExecuteFunction (WxeFunction function, string target, Control sender, bool returningPostback);
  /// <summary>
  ///   Executes a WXE function in another window or frame.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\WxePage.xml' path='WxePage/ExecuteFunctionExternal/*' />
  void ExecuteFunction (WxeFunction function, string target, string features, Control sender, bool returningPostback);

  /// <summary>
  ///   Executes a WXE function in the current window.
  /// </summary>
  void ExecuteFunction (WxeFunction function);

  /// <summary>
  ///   Executes a WXE function in the current window without triggering the current post back event on returning.
  /// </summary>
  void ExecuteFunctionNoRepost (WxeFunction function, Control sender);
  /// <summary>
  ///   Executes a WXE function in the current window without triggering the current post back event on returning.
  /// </summary>
  void ExecuteFunctionNoRepost (WxeFunction function, Control sender, bool usesEventTarget);
  bool IsReturningPostBack { get; }
  WxeFunction ReturningFunction { get; }

  [EditorBrowsable (EditorBrowsableState.Never)]
  HtmlForm HtmlForm { get; set; }

  /// <summary>
  ///   Gets or sets the flag that determines whether to display a confirmation dialog before aborting the session. 
  ///  </summary>
  /// <value> <see langowrd="true"/> to display the confirmation dialog. </value>
  bool IsAbortConfirmationEnabled { get; }

  /// <summary>
  ///   Gets or sets the flag that determines whether abort the session upon closing the window. 
  ///  </summary>
  /// <value> <see langowrd="true"/> to abort the session upon navigtion away from the page. </value>
  bool IsAbortEnabled { get; }
}

public class WxePageInfo: WxeTemplateControlInfo, IDisposable
{
  /// <summary> A list of resources. </summary>
  /// <remarks> 
  ///   Resources will be accessed using 
  ///   <see cref="M:IResourceManager.GetString (Enum)">IResourceManager.GetString (Enum)</see>. 
  /// </remarks>
  [ResourceIdentifiers]
  [MultiLingualResources ("Rubicon.Web.Globalization.WxePageInfo")]
  protected enum ResourceIdentifier
  {
    AbortMessage
  }

  private const string c_script = "ExecutionEngine.js";
  private const string c_smartNavigationScript = "SmartNavigation.js";
  public const string PageTokenID = "wxePageToken";
  private const string c_smartScrollingID = "smartScrolling";
  private const string c_smartFocusID = "smartFocus";

  private IWxePage _page;
  private WxeForm _form;
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
    base.Initialize (_page, context);

    if (! ControlHelper.IsDesignMode (_page, context))
    {
      //  if (_page.HtmlForm == null)
      //    throw new HttpException (_page.GetType().FullName + " does not initialize field 'Form'.");
      _form = WxeForm.Replace (_page.HtmlForm);
      _page.HtmlForm = _form;
    }

    if (_page.CurrentStep != null)
      _page.RegisterHiddenField (WxePageInfo.PageTokenID, _page.CurrentStep.PageToken);

    _page.Load += new EventHandler (Page_Load);
    _page.PreRender +=new EventHandler(Page_PreRender);
    _page.Unload += new EventHandler(Page_Unload);
  }

  private void Page_Load (object sender, EventArgs e)
  {
    NameValueCollection postBackCollection = _page.GetPostBackCollection();
    if (postBackCollection != null)
    {
      string returningToken = _form.ReturningToken;
      // string returningToken = postBackCollection[WxeForm.ReturningTokenID];
      if (! StringUtility.IsNullOrEmpty (returningToken))
      {
        WxeFunctionStateCollection functionStates = WxeFunctionStateCollection.Instance;
        WxeFunctionState functionState = functionStates.GetItem (returningToken);
        if (functionState != null)
        {
          WxeContext.Current.ReturningFunction = functionState.Function;
          WxeContext.Current.IsReturningPostBack = true;
          _returningFunctionState = functionState;
        }
      }
    }

    _form.ReturningToken = string.Empty;    

  }

  /// <summary> Handles the <b>PreRender</b> event of the page. </summary>
  private void Page_PreRender (object sender, EventArgs e)
  {
    NameValueCollection postBackCollection = _page.GetPostBackCollection();

    _page.RegisterHiddenField (WxeHandler.Parameters.WxeFunctionToken, WxeContext.Current.FunctionToken);

    Page page = (Page) _page;
    string key = "wxeDoSubmit";
    PageUtility.RegisterClientScriptBlock (page, key,
          "function wxeDoSubmit (button, pageToken) { \r\n"
        + "  var theForm = document." + _form.ClientID + "; \r\n"
        + "  theForm." + WxeForm.ReturningTokenID + ".value = pageToken; \r\n"
        + "  document.getElementById(button).click(); \r\n"
        + "}");

    key = "wxeDoPostBack";
    PageUtility.RegisterClientScriptBlock (page, key,
          "function wxeDoPostBack (control, argument, returningToken) { \r\n"
        + "  var theForm = document." + _form.ClientID + "; \r\n"
        + "  theForm." + WxeForm.ReturningTokenID + ".value = returningToken; \r\n"
        + "  __doPostBack (control, argument); \r\n"
        + "}");

    key = "wxeScript";
    string url = ResourceUrlResolver.GetResourceUrl (page, typeof (WxePageInfo), ResourceType.Html, c_script);
    HtmlHeadAppender.Current.RegisterJavaScriptInclude (key, url);
    
    if (_page.IsSmartScrollingEnabled || _page.IsSmartFocusingEnabled)
    {
      key = "smartNavigationScript";
      url = ResourceUrlResolver.GetResourceUrl (page, typeof (WxePageInfo), ResourceType.Html, c_smartNavigationScript);
      HtmlHeadAppender.Current.RegisterJavaScriptInclude (key, url);
    }

    RegisterWxeInitializationScript(); 
    
    if (_page.IsSmartScrollingEnabled)
    {
      string smartScrollingValue = null;
      if (postBackCollection != null && !_isSmartNavigationDataDisacarded)
        smartScrollingValue = postBackCollection[c_smartScrollingID];
      _page.RegisterHiddenField (c_smartScrollingID, smartScrollingValue);
    }

    if (_page.IsSmartFocusingEnabled)
    {
      string smartFocusValue = null;
      if (postBackCollection != null && !_isSmartNavigationDataDisacarded)
        smartFocusValue = postBackCollection[c_smartFocusID];
      if (! StringUtility.IsNullOrEmpty (_smartFocusID))
        smartFocusValue = _smartFocusID;
      _page.RegisterHiddenField (c_smartFocusID, smartFocusValue);
    }
  }

  /// <summary> Handles the <b>Unload</b> event of the page. </summary>
  /// <remarks> 
  ///   Aborts the <see cref="_returningFunctionState"/> if its <see cref="WxeFunctionState.Function"/> is the root 
  ///   function.
  /// </remarks>
  private void Page_Unload(object sender, EventArgs e)
  {
    if (_returningFunctionState != null)
    {
      bool isRootFunction = _returningFunctionState.Function == _returningFunctionState.Function.RootFunction;
      if (isRootFunction)
        WxeFunctionStateCollection.Instance.Abort (_returningFunctionState);
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

      string resumePath = WxeContext.Current.GetResumePath (false);

      refreshIntervall = WxeHandler.RefreshInterval * 60000;
      refreshPath = "'" + resumePath + "&" + WxeHandler.Parameters.WxeAction + "=" + WxeHandler.Actions.Refresh + "'";
      
      if (isAbortEnabled)
        abortPath = "'" + resumePath + "&" + WxeHandler.Parameters.WxeAction + "=" + WxeHandler.Actions.Abort + "'";
      
      if (isAbortEnabled && isAbortConfirmationEnabled)
        abortMessage = "'" + GetResourceManager().GetString (ResourceIdentifier.AbortMessage) + "'";        

    }

    string smartScrollingFieldID = "null";
    string smartFocusFieldID = "null";

    if (_page.IsSmartScrollingEnabled)
      smartScrollingFieldID = "'" + c_smartScrollingID + "'";
    if (_page.IsSmartFocusingEnabled)
      smartFocusFieldID = "'" + c_smartFocusID + "'";
   
    string key = "wxeInitialize";
    PageUtility.RegisterStartupScriptBlock ((Page)_page, key,
          "Wxe_Initialize ('" + _form.ClientID + "', " 
        + refreshIntervall.ToString() + ", " + refreshPath + ", " 
        + abortPath + ", " + abortMessage + ", "
        + smartScrollingFieldID + ", " + smartFocusFieldID + ");");
  }

  public NameValueCollection EnsurePostBackModeDetermined (HttpContext context)
  {
    if (! _postbackCollectionInitialized)
    {
      _postbackCollection = DeterminePostBackMode (context);
      _postbackCollectionInitialized = true;
    }
    return _postbackCollection;
  }  

  private NameValueCollection DeterminePostBackMode (HttpContext context)
  {
    if (WxeContext.Current == null)
      return null;
    if (! WxeContext.Current.IsPostBack)
      return null;
    if (WxeContext.Current.PostBackCollection != null)
      return WxeContext.Current.PostBackCollection;
    if (context.Request == null)
      return null;

    NameValueCollection collection;
    if (StringUtility.AreEqual (context.Request.HttpMethod, "POST", false))
      collection = context.Request.Form;
    else
      collection = context.Request.QueryString;

    if ((collection[ControlHelper.ViewStateID] == null) && (collection[ControlHelper.PostEventSourceID] == null))
      return null;
    else
      return collection;
  }

  public void ExecuteNextStep ()
  {
    _executeNextStep = true;
    _response = _page.Response;
    _page.Visible = false; // suppress prerender and render events
  }

  public void ExecuteFunction (WxeFunction function, string target, Control sender, bool returningPostback)
  {
    ExecuteFunction (function, target, null, sender, returningPostback);
  }

  public void ExecuteFunction (WxeFunction function, string target, string features, Control sender, bool returningPostback)
  {
    bool enableCleanUp = !returningPostback;
    WxeFunctionState functionState = new WxeFunctionState (function, enableCleanUp);
    WxeFunctionStateCollection functionStates = WxeFunctionStateCollection.Instance;
    functionStates.Add (functionState);

    string href = WxeContext.GetResumePath (_page.Request, _page.Response, functionState.FunctionToken, true);
    string openScript;
    if (features != null)
      openScript = string.Format (@"window.open(""{0}"", ""{1}"", ""{2}"");", href, target, features);
    else
      openScript = string.Format (@"window.open(""{0}"", ""{1}"");", href, target);
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
            "if (window.opener && window.opener.wxeDoPostBack && window.opener.document.getElementById(\"{0}\") && window.opener.document.getElementById(\"{0}\").value == \"{1}\") \n"
          + "  window.opener.wxeDoPostBack(\"{2}\", \"{3}\", \"{4}\"); \n"
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
            "if (window.opener && window.opener.wxeDoSubmit && window.opener.document.getElementById(\"{0}\") && window.opener.document.getElementById(\"{0}\").value == \"{1}\") \n"
          + "  window.opener.wxeDoSubmit(\"{2}\", \"{3}\"); \n"
          + "window.close();", 
          WxePageInfo.PageTokenID,
          _page.CurrentStep.PageToken,
          sender.ClientID, 
          functionState.FunctionToken);
    }
    function.ReturnUrl = "javascript:" + returnScript;
  }

  public bool UsesEventTarget
  {
    get { return ! StringUtility.IsNullOrEmpty (_page.GetPostBackCollection()[ControlHelper.PostEventSourceID]); }
  }

  public void Dispose ()
  {
    if (_executeNextStep)
    {
      _response.Clear(); // throw away page trace output
      throw new WxeExecuteNextStepException();
    }
  }

  public WxeForm Form
  {
    get { return _form; }
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

  public HtmlForm HtmlFormDefaultImplementation
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

  /// <summary> Clears scrolling and focus information on the page. </summary>
  public void DiscardSmartNavigationData ()
  {
    _isSmartNavigationDataDisacarded = true;
  }

  /// <summary> Sets the focus to the passed control. </summary>
  /// <param name="control"> The <see cref="IFocusableControl"/> to assign the focus to. </param>
  public void SetFocus (IFocusableControl control)
  {
    ArgumentUtility.CheckNotNull ("control", control);
    if (StringUtility.IsNullOrEmpty (control.FocusID))
      return;
    SetFocus (control.FocusID);
  }

  /// <summary> Sets the focus to the passed control ID. </summary>
  /// <param name="id"> The client side ID of the control to assign the focus to. </param>
  public void SetFocus (string id)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("id", id);
    _smartFocusID = id;
  }
}

/// <summary>
///   Base class for pages that can be called by <see cref="WxePageStep"/>.
/// </summary>
/// <remarks>
///   The <see cref="HtmlForm"/> must use the ID "Form". 
///   If you cannot derive your pages from this class (e.g., because you need to derive from another class), you may
///   implement <see cref="IWxePage"/> and override <see cref="DeterminePostBackMode"/> and <see cref="Dispose"/>. 
///   Use <see cref="WxePageInfo"/> to implementat all methods and properties.
/// </remarks>
public class WxePage: Page, IWxePage
{
  private WxePageInfo _wxeInfo;
  private ValidatableControlInitializer _validatableControlInitializer;
  private PostLoadInvoker _postLoadInvoker;
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
  }

  protected override NameValueCollection DeterminePostBackMode()
  {
    NameValueCollection result = _wxeInfo.EnsurePostBackModeDetermined (Context);
    _wxeInfo.Initialize (Context);
    OnBeforeInit();
    return result;
  }

  protected virtual void OnBeforeInit ()
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

  public NameValueCollection GetPostBackCollection ()
  {
    return _wxeInfo.EnsurePostBackModeDetermined (Context);
  }

  [Browsable (false)]
  public WxePageStep CurrentStep
  {
    get { return _wxeInfo.CurrentStep; }
  }
  
  [Browsable (false)]
  public WxeFunction CurrentFunction
  {
    get { return _wxeInfo.CurrentFunction; }
  }

  [Browsable (false)]
  public NameObjectCollection Variables 
  {
    get { return _wxeInfo.Variables; }
  }

  public void ExecuteNextStep ()
  {
    _wxeInfo.ExecuteNextStep();
  }

  [Browsable (false)]
  public bool IsReturningPostBack
  {
    get { return ((WxeContext.Current == null) ? false : WxeContext.Current.IsReturningPostBack); }
  }

  public void ExecuteFunction (WxeFunction function, string target, Control sender, bool returningPostback)
  {
    _wxeInfo.ExecuteFunction (function, target, sender, returningPostback);
  }

  public void ExecuteFunction (WxeFunction function, string target, string features, Control sender, bool returningPostback)
  {
    _wxeInfo.ExecuteFunction (function, target, features, sender, returningPostback);
  }

  public void ExecuteFunction (WxeFunction function)
  {
    CurrentStep.ExecuteFunction (this, function);
  }

  /// <summary>
  ///   Executes the specified WXE function, then returns to this page without causing the current event again.
  /// </summary>
  /// <remarks>
  ///   This overload tries to determine automatically whether the current event was caused by the __EVENTTARGET field.
  /// </remarks>
  public void ExecuteFunctionNoRepost (WxeFunction function, Control sender)
  {
    ExecuteFunctionNoRepost (function, sender, _wxeInfo.UsesEventTarget);
  }

  /// <summary>
  ///   Executes the specified WXE function, then returns to this page without causing the current event again.
  /// </summary>
  /// <remarks>
  ///   This overload allows you to specify whether the current event was caused by the __EVENTTARGET field.
  ///   When in doubt, use <see cref="M:ExecuteFunctionNoRepost (IWxePage, WxeFunction, Control)"/>.
  /// </remarks>
  public void ExecuteFunctionNoRepost (WxeFunction function, Control sender, bool usesEventTarget)
  {
    CurrentStep.ExecuteFunctionNoRepost (this, function, sender, usesEventTarget);
  }

  [Browsable (false)]
  public WxeFunction ReturningFunction
  {
    get { return ((WxeContext.Current == null) ? null : WxeContext.Current.ReturningFunction); }
  }

  protected WxeForm WxeForm
  {
    get { return (WxeForm) _wxeInfo.Form; }
  }

  public override void Dispose()
  {
    base.Dispose ();
    _wxeInfo.Dispose();
  }

  [EditorBrowsable (EditorBrowsableState.Never)]
  [Browsable (false)]
  public virtual HtmlForm HtmlForm
  {
    get { return _wxeInfo.HtmlFormDefaultImplementation; }
    set { _wxeInfo.HtmlFormDefaultImplementation = value; }
  }

  /// <summary>
  ///   Makes sure that PostLoad is called on all controls that support <see cref="ISupportsPostLoadControl"/>
  /// </summary>
  public void EnsurePostLoadInvoked ()
  {
    _postLoadInvoker.EnsurePostLoadInvoked();
  }

  /// <summary>
  ///   Makes sure that all validators are registered with their <see cref="IValidatableControl"/> controls.
  /// </summary>
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
