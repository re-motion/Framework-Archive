using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;
using System.Globalization;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Collections;
using Rubicon.Web.UI;

namespace Rubicon.Web.ExecutionEngine
{

public interface IWxePage: IPage, IWxeTemplateControl
{
  NameValueCollection GetPostBackCollection ();
}

public class WxePageInfo: WxeTemplateControlInfo
{
  public void OnLoad (IWxePage page, HttpContext context, ref HtmlForm form)
  {
    OnLoad (page, context);

    if (form == null)
      throw new HttpException (page.GetType().FullName + " does not initialize field 'Form'.");
    form = WxeForm.Replace (form);
  }

  public NameValueCollection DeterminePostBackMode (HttpContext context)
  {
    if (WxeContext.Current.PostBackCollection != null)
      return WxeContext.Current.PostBackCollection;

    if (context.Request == null)
      return null;

    NameValueCollection collection;
    if (0 == string.Compare (context.Request.HttpMethod, "POST", false, CultureInfo.InvariantCulture))
      collection = context.Request.Form;
    else
      collection = context.Request.QueryString;

    if ((collection["__VIEWSTATE"] == null) && (collection["__EVENTTARGET"] == null))
      return null;

    return collection;
  }  
}

/// <summary>
///   Base class for pages that can be called by <see cref="WxePageStep"/>.
/// </summary>
/// <remarks>
///   The <see cref="HtmlForm"/> must use the ID "Form". 
///   If you cannot derive your pages from this class (e.g., because you need to derive from another class), you may
///   implement <see cref="IWxePage"/> and override <see cref="Page.OnLoad"/> and <see cref="Page.DeterminePostBackMode"/>. 
///   Use <see cref="WxePageInfo"/> to implementat all methods and properties.
/// </remarks>
public class WxePage: Page, IWxePage
{
  private WxePageInfo _wxeInfo = new WxePageInfo(); 

  protected HtmlForm Form;

  protected override void OnLoad (EventArgs e)
  {
    _wxeInfo.OnLoad (this, Context, ref Form);
    base.OnLoad (e);
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
}

}
