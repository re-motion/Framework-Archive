using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;
using System.Globalization;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Collections;

namespace Rubicon.Web.ExecutionEngine
{

public interface IWxePage: IPage
{
  NameObjectCollection Variables { get; }
  WxePageStep CurrentStep { get; }
  WxeFunction CurrentFunction { get; }
  NameValueCollection GetPostBackCollection ();
}

/// <summary>
///   Base class for pages that can be called by <see cref="WxePageStep"/>.
/// </summary>
/// <remarks>
///   If you cannot derive your pages from this class (e.g., because you need to derive from another class), you may
///   implement <see cref="IWxePage"/> yourself by using the <see langword="static"/> methods provided by <c>WxePage</c>.
/// </remarks>
public class WxePage: Page, IWxePage
{
  public static void OnLoad (IWxePage @this, HttpContext context, ref HtmlForm form, out WxePageStep currentStep, out WxeFunction currentFunction, EventArgs e)
  {
    WxeHandler wxeHandler = context.Handler as WxeHandler;
    currentStep = (wxeHandler == null) ? null : wxeHandler.CurrentFunction.ExecutingStep as WxePageStep;

    WxeStep step = currentStep;
    do {
      currentFunction = step as WxeFunction;
      if (currentFunction != null)
        break;
      step = step.ParentStep;
    } while (step != null);
      
    if (form == null)
      throw new HttpException (@this.GetType().FullName + " does not initialize field 'Form'.");
    form = WxeForm.Replace (form);
  }

  public static NameValueCollection DeterminePostBackMode (HttpContext context)
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
  
  protected HtmlForm Form;
  private WxePageStep _currentStep;
  private WxeFunction _currentFunction;

  protected override void OnInit (EventArgs e)
  {
  }

  protected override void OnLoad (EventArgs e)
  {
    WxePage.OnLoad (this, Context, ref Form, out _currentStep, out _currentFunction, e);
    base.OnLoad (e);
  }

  protected override NameValueCollection DeterminePostBackMode()
  {
    return WxePage.DeterminePostBackMode (Context);
  }

  public NameValueCollection GetPostBackCollection ()
  {
    return WxePage.DeterminePostBackMode (Context);
  }

  public WxePageStep CurrentStep
  {
    get { return _currentStep; }
  }
  
  public WxeFunction CurrentFunction
  {
    get { return _currentFunction; }
  }

  public NameObjectCollection Variables 
  {
    get { return (_currentStep == null) ? null : _currentStep.Variables; }
  }
}

}
