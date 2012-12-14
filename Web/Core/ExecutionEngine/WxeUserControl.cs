using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Rubicon.Collections;

namespace Rubicon.Web.ExecutionEngine
{

public class WxeUserControl: UserControl, IWxeTemplateControl
{
  WxeTemplateControlInfo _wxeInfo = new WxeTemplateControlInfo();

  protected override void OnLoad (EventArgs e)
  {
    _wxeInfo.OnInit (this, Context);
    base.OnLoad (e);
  }

  public IWxePageStep CurrentStep
  {
    get { return (IWxePageStep) _wxeInfo.CurrentStep; }
  }
  
  public WxeFunction CurrentFunction
  {
    get { return _wxeInfo.CurrentFunction; }
  }

  public NameObjectCollection Variables 
  {
    get { return _wxeInfo.Variables; }
  }

  public new IWxePage Page
  {
    get { return (IWxePage) base.Page; }
  }
}

}