using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;
using System.Globalization;
using System.ComponentModel;
using System.Reflection;
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

public interface IWxePage: IPage, IWxeTemplateControl
{
  NameValueCollection GetPostBackCollection ();
  void ExecuteNextStep ();

  /// <summary>
  ///   Executes a WXE function in another window or frame.
  /// </summary>
  /// <include file='doc\include\WxePage.xml' path='WxePage/ExecuteFunctionExternal/param[@name!="sender"]' />
  void ExecuteFunction (WxeFunction function, string target, Control sender, bool returningPostback);
  /// <summary>
  ///   Executes a WXE function in another window or frame.
  /// </summary>
  /// <include file='doc\include\WxePage.xml' path='WxePage/ExecuteFunctionExternal/*' />
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
  public const string PageTokenID = "wxePageToken";

  private IWxePage _page;
  private WxeForm _form;
  private bool _postbackCollectionInitialized = false;
  private NameValueCollection _postbackCollection = null;

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
        }
      }
    }

    _form.ReturningToken = string.Empty;    
  
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

    RegisterSessionManagement();
  }

  protected void RegisterSessionManagement()
  {
    if (WxeHandler.IsSessionManagementEnabled)
    {
      //  Ensure the registration of "__doPostBack" on the page.
      string temp = _page.GetPostBackEventReference ((Page)_page);

      //TODO: Optionally switch off abort warning
      bool isAbortConfirmationEnabled = true;
      bool isAbortEnabled = true;

      int refreshIntervall = WxeHandler.RefreshInterval * 60000;
      string resumePath = WxeContext.Current.GetResumePath (false);
      string refreshPath = 
          "'" + resumePath + "&" + WxeHandler.Parameters.WxeAction + "=" + WxeHandler.Actions.Refresh + "'";
      
      string abortPath;
      if (isAbortEnabled)
        abortPath = "'" + resumePath + "&" + WxeHandler.Parameters.WxeAction + "=" + WxeHandler.Actions.Abort + "'";
      else
        abortPath = "null";
      
      string abortMessage;
      if (isAbortEnabled && isAbortConfirmationEnabled)
        abortMessage = "'" + GetResourceManager().GetString (ResourceIdentifier.AbortMessage) + "'";
      else
        abortMessage = "null";

      string key = "wxeInitialize";
      PageUtility.RegisterStartupScriptBlock ((Page)_page, key,
            "Wxe_Initialize ('" + _form.ClientID + "', " 
          + refreshIntervall.ToString() + ", " + refreshPath + ", " 
          + abortPath + ", " + abortMessage + ");");
    }
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
    WxeFunctionState functionState = new WxeFunctionState (function);
    WxeFunctionStateCollection functionStates = WxeFunctionStateCollection.Instance;
    functionStates.Add (functionState);

    string href = WxeContext.GetResumePath (_page.Request, functionState.FunctionToken, true);
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

  public WxePageStep CurrentStep
  {
    get { return _wxeInfo.CurrentStep; }
  }
  
  public WxeFunction CurrentFunction
  {
    get { return _wxeInfo.CurrentFunction; }
  }

  public NameObjectCollection Variables 
  {
    get { return _wxeInfo.Variables; }
  }

  public void ExecuteNextStep ()
  {
    _wxeInfo.ExecuteNextStep();
  }

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
  ///   When in doubt, use <see cref="ExecuteFunctionNoRepost (IWxePage, WxeFunction, Control)"/>.
  /// </remarks>
  public void ExecuteFunctionNoRepost (WxeFunction function, Control sender, bool usesEventTarget)
  {
    CurrentStep.ExecuteFunctionNoRepost (this, function, sender, usesEventTarget);
  }

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
}

}
