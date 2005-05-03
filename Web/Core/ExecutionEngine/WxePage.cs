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
using Rubicon.Web.UI;
using Rubicon.Web.Utilities;
using Rubicon.Utilities;

namespace Rubicon.Web.ExecutionEngine
{

public interface IWxePage: IPage, IWxeTemplateControl
{
  NameValueCollection GetPostBackCollection ();
  void ExecuteNextStep ();
  void ExecuteFunction (WxeFunction function, string target, Control sender, bool returningPostback);
  void ExecuteFunction (WxeFunction function);
  void ExecuteFunctionNoRepost (WxeFunction function, Control sender);
  void ExecuteFunctionNoRepost (WxeFunction function, Control sender, bool usesEventTarget);
  bool IsReturningPostBack { get; }
  WxeFunction ReturningFunction { get; }

  [EditorBrowsable (EditorBrowsableState.Never)]
  HtmlForm HtmlForm { get; set; }
}

public class WxePageInfo: WxeTemplateControlInfo, IDisposable
{
  private IWxePage _page;
  private WxeForm _form;
  private bool _postbackCollectionInitialized = false;
  private NameValueCollection _postbackCollection = null;

  private bool _executeNextStep = false;
  private HttpResponse _response; // used for cleanup in Dispose

  public WxePageInfo (IWxePage page)
  {
    _page = page;
  }

  public void Initialize (HttpContext context)
  {
    base.OnInit (_page, context);

    if (! ControlHelper.IsDesignMode (_page, context))
    {
      //  if (_page.HtmlForm == null)
      //    throw new HttpException (_page.GetType().FullName + " does not initialize field 'Form'.");
      _form = WxeForm.Replace (_page.HtmlForm);
      _page.HtmlForm = _form;
    }

    HtmlInputHidden wxePageTokenField = new HtmlInputHidden();
    wxePageTokenField.ID = "wxePageToken";
    if (_page.CurrentStep != null)
      wxePageTokenField.Value = _page.CurrentStep.PageToken;
    _form.Controls.Add (wxePageTokenField);

    _page.Load += new EventHandler (Page_Load);
  }

  private void Page_Load (object sender, EventArgs e)
  {
    PageUtility.RegisterClientScriptBlock ((Page)_page, "wxeDoSubmit",
        "function wxeDoSubmit (button, pageToken) { \n"
        + "  var theForm = document." + _form.ClientID + "; \n"
        + "  theForm.returningToken.value = pageToken; \n"
        + "  document.getElementById(button).click(); \n"
        + "}");
    PageUtility.RegisterClientScriptBlock ((Page)_page, "wxeDoPostBack",
        "function wxeDoPostBack (control, argument, returningToken) { \n"
        + "  var theForm = document." + _form.ClientID + "; \n"
        + "  theForm.returningToken.value = returningToken; \n"
        + "  __doPostBack (control, argument); \n"
        + "}");

    NameValueCollection postBackCollection = _page.GetPostBackCollection();
    if (postBackCollection != null)
    {
      string returningToken = _form.ReturningToken;
      // string returningToken = postBackCollection["returningToken"];
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
  }

  public NameValueCollection DeterminePostBackMode (HttpContext context)
  {
    if (! _postbackCollectionInitialized)
    {
      if (WxeContext.Current == null)
      {
        _postbackCollection = null;
      }
      else if (! WxeContext.Current.IsPostBack)
      {
        _postbackCollection = null;
      }
      else if (WxeContext.Current.PostBackCollection != null)
      {
        _postbackCollection = WxeContext.Current.PostBackCollection;
      }
      else if (context.Request == null)
      {
        _postbackCollection = null;
      }
      else
      {
        NameValueCollection collection;
        if (0 == string.Compare (context.Request.HttpMethod, "POST", false, CultureInfo.InvariantCulture))
          collection = context.Request.Form;
        else
          collection = context.Request.QueryString;

        if ((collection[ControlHelper.ViewStateID] == null) && (collection[ControlHelper.PostEventSourceID] == null))
          _postbackCollection = null;
        else
          _postbackCollection = collection;
      }

      _postbackCollectionInitialized = true;
    }
    return _postbackCollection;
  }  

  public void ExecuteNextStep ()
  {
    _executeNextStep = true;
    _response = _page.Response;
    _page.Visible = false; // suppress prerender and render events
  }

  public void ExecuteFunction (WxeFunction function, string target, Control sender, bool returningPostback)
  {
    WxeFunctionState functionState = new WxeFunctionState (function);
    WxeFunctionStateCollection functionStates = WxeFunctionStateCollection.Instance;
    functionStates.Add (functionState);

    string href = _page.Request.Path + "?WxeFunctionToken=" + functionState.FunctionToken;
    string openScript = string.Format (@"window.open(""{0}"", ""{1}"");", href, target);
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
            "if (window.opener && window.opener.wxeDoPostBack && window.opener.document.getElementById(\"wxePageToken\") && window.opener.document.getElementById(\"wxePageToken\").value == \"{0}\") \n"
          + "  window.opener.wxeDoPostBack(\"{1}\", \"{2}\", \"{3}\"); \n"
          + "window.close();", 
          _page.CurrentStep.PageToken,
          eventtarget, 
          eventargument, 
          functionState.FunctionToken);
    }
    else
    {
      returnScript = string.Format (
            "if (window.opener && window.opener.wxeDoSubmit && window.opener.document.getElementById(\"wxePageToken\") && window.opener.document.getElementById(\"wxePageToken\").value == \"{0}\") \n"
          + "  window.opener.wxeDoSubmit(\"{1}\", \"{2}\"); \n"
          + "window.close();", 
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
    NameValueCollection result = _wxeInfo.DeterminePostBackMode (Context);
    _wxeInfo.Initialize (Context);
    OnBeforeInit();
    return result;
  }

  protected virtual void OnBeforeInit ()
  {
  }

  public NameValueCollection GetPostBackCollection ()
  {
    return _wxeInfo.DeterminePostBackMode (Context);
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
