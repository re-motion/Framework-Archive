using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;
using System.Globalization;
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

  public void Initialize (HttpContext context, ref HtmlForm form)
  {
    base.OnInit (_page, context);
    if (! ControlHelper.IsDesignMode (_page, context))
    {
      if (form == null)
        throw new HttpException (_page.GetType().FullName + " does not initialize field 'Form'.");
      _form = WxeForm.Replace (form);
      form = _form;
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

    NameValueCollection postBackCollection = _page.GetPostBackCollection();
    if (postBackCollection != null)
    {
      string returningToken = _form.ReturningToken;
      // string returningToken = postBackCollection["returningToken"];
      if (! StringUtility.IsNullOrEmpty (returningToken))
      {
        WxeWindowStateCollection windowStates = WxeWindowStateCollection.Instance;
        WxeWindowState windowState = windowStates.GetItem (returningToken);
        if (windowState != null)
        {
          WxeContext.Current.ReturningFunction = windowState.Function;
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

        if ((collection["__VIEWSTATE"] == null) && (collection["__EVENTTARGET"] == null))
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
    WxeWindowState windowState = new WxeWindowState (function, 20);
    WxeWindowStateCollection windowStates = WxeWindowStateCollection.Instance;
    windowStates.Add (windowState);

    string href = _page.Request.Path + "?WxeWindowToken=" + windowState.WindowToken;
    string openScript = string.Format (@"window.open(""{0}"", ""{1}"");", href, target);
    PageUtility.RegisterStartupScriptBlock ((Page)_page, "WxeExecuteFunction", openScript);

    string returnScript;
    if (! returningPostback)
    {
      returnScript = "window.close();";
    }
    else if (UsesEventTarget)
    {
      string eventtarget = _page.GetPostBackCollection()["__EVENTTARGET"];
      string eventargument = _page.GetPostBackCollection()["__EVENTARGUMENT"];
      returnScript = string.Format (
            "if (window.opener && window.opener.__doPostBack && window.opener.document.getElementById(\"wxePageToken\") && window.opener.document.getElementById(\"wxePageToken\").value == \"{0}\") \n"
          + "  window.opener.__doPostBack(\"{1}\", \"{2}\"); \n"
          + "window.close();", 
          _page.CurrentStep.PageToken,
          eventtarget, 
          eventargument);
    }
    else
    {
      returnScript = string.Format (
            "if (window.opener && window.opener.wxeDoSubmit && window.opener.document.getElementById(\"wxePageToken\") && window.opener.document.getElementById(\"wxePageToken\").value == \"{0}\") \n"
          + "  window.opener.wxeDoSubmit(\"{1}\", \"{2}\"); \n"
          + "window.close();", 
          _page.CurrentStep.PageToken,
          sender.ClientID, 
          windowState.WindowToken);
    }
    function.ReturnUrl = "javascript:" + returnScript;
  }

  public bool UsesEventTarget
  {
    get { return ! StringUtility.IsNullOrEmpty (_page.GetPostBackCollection()["__EVENTTARGET"]); }
  }

  public void Dispose ()
  {
    if (_executeNextStep)
    {
      _response.Clear(); // throw away page trace output
      throw new WxeExecuteNextStepException();
    }
  }
}

/// <summary>
///   Base class for pages that can be called by <see cref="WxePageStep"/>.
/// </summary>
/// <remarks>
///   The <see cref="HtmlForm"/> must use the ID "Form". 
///   If you cannot derive your pages from this class (e.g., because you need to derive from another class), you may
///   implement <see cref="IWxePage"/> and override <see cref="OnInit"/> and <see cref="Page.DeterminePostBackMode"/>. 
///   Use <see cref="WxePageInfo"/> to implementat all methods and properties.
/// </remarks>
public class WxePage: Page, IWxePage
{
  private WxePageInfo _wxeInfo;

  protected HtmlForm Form;

  public WxePage ()
  {
    _wxeInfo = new WxePageInfo (this);
  }

  protected override NameValueCollection DeterminePostBackMode()
  {
    NameValueCollection result = _wxeInfo.DeterminePostBackMode (Context);
    _wxeInfo.Initialize (Context, ref Form);
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
    get { return (WxeForm) Form; }
  }

  public override void Dispose()
  {
    base.Dispose ();
    _wxeInfo.Dispose();
  }
}

}
