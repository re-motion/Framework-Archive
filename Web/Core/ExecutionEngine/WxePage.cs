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

namespace Rubicon.Web.ExecutionEngine
{

public interface IWxePage: IPage, IWxeTemplateControl
{
  NameValueCollection GetPostBackCollection ();
}

public class WxePageInfo: WxeTemplateControlInfo
{
  public void OnInit (IWxePage page, HttpContext context, ref HtmlForm form)
  {
    OnInit (page, context);

    if (! ControlHelper.IsDesignMode (page, context))
    {
      if (form == null)
        throw new HttpException (page.GetType().FullName + " does not initialize field 'Form'.");
      form = WxeForm.Replace (form);
    }
  }

  private bool _postbackCollectionInitialized = false;
  private NameValueCollection _postbackCollection = null;

  public NameValueCollection DeterminePostBackMode (HttpContext context)
  {
    if (! _postbackCollectionInitialized)
    {
      if (! WxeContext.Current.IsPostBack)
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
  private WxePageInfo _wxeInfo = new WxePageInfo(); 

  protected HtmlForm Form;

  protected override void OnInit (EventArgs e)
  {
    _wxeInfo.OnInit (this, Context, ref Form);
    base.OnInit (e);
  }

  protected override NameValueCollection DeterminePostBackMode()
  {
    return _wxeInfo.DeterminePostBackMode (Context);
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

  private bool _executeNextStep = false;
  private HttpResponse _response; // used for cleanup in Dispose

  public void ExecuteNextStep ()
  {
    _executeNextStep = true;
    _response = Response;
    this.Visible = false; // suppress prerender and render events
  }

  public override void Dispose()
  {
    base.Dispose ();
    if (_executeNextStep)
    {
      _response.Clear(); // throw away page trace output
      throw new WxeExecuteNextStepException();
    }
  }

  protected bool IsReturningPostBack
  {
    get { return WxeContext.Current.IsReturningPostBack; }
  }

  public void ExecuteFunction (WxeFunction function, string target, Control sender)
  {
    ArrayList pages = (ArrayList) Session["WxePages"];
    WxePageSession pageSession = new WxePageSession (function, 20);
    pages.Add (pageSession);
    string href = Request.Path + "?WxePageToken=" + pageSession.PageToken;
    string script = string.Format (@"window.open(""{0}"", ""{1}"");", href, target);

    // string eventtarget = GetPostBackCollection()["__EVENTTARGET"];
    // string eventargument = GetPostBackCollection()["__EVENTARGUMENT"];
    // subFunction.ReturnUrl = "javascript:window.opener.__doPostBack(\"" + eventtarget + "\",\"" + eventargument + "\"); window.close();";
    function.ReturnUrl = "javascript:window.opener.doSubmit(\"" + sender.ClientID + "\", \"" + pageSession.PageToken + "\"); window.close();";

    PageUtility.RegisterStartupScriptBlock (this, "WxeExecuteFunction", script);
  }

  public void ExecuteFunction (WxeFunction function)
  {
    CurrentStep.ExecuteFunction (this, function);
  }

  public WxeFunction ReturningFunction
  {
    get { return WxeContext.Current.ReturningFunction; }
  }
}

}
