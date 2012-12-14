using System;
using System.Web;
using System.Web.UI;
using Rubicon.Collections;
using Rubicon.Web.UI;
using Rubicon.Web.Utilities;

namespace Rubicon.Web.ExecutionEngine
{

public interface IWxeTemplateControl: ITemplateControl
{
  NameObjectCollection Variables { get; }
  WxePageStep CurrentStep { get; }
  WxeFunction CurrentFunction { get; }
}

public class WxeTemplateControlInfo
{
  private WxePageStep _currentStep;
  private WxeFunction _currentFunction;
  private WxeHandler _wxeHandler;

    public void OnInit (IWxeTemplateControl control, HttpContext context)
  {
    if (ControlHelper.IsDesignMode (control, context))
      return;

    if (control is Page)
    {
      _wxeHandler = context.Handler as WxeHandler;
    }
    else
    {
      IWxePage wxePage = control.Page as IWxePage;
      if (wxePage == null)
        throw new InvalidOperationException(string.Format("'{0}' can only be added to a Page implementing the IWxePage interface.", control.GetType().FullName));
      _wxeHandler = wxePage.WxeHandler;
    }

    WxeHandler wxeHandler = context.Handler as WxeHandler;
    if (wxeHandler == null)
      throw new HttpException ("No current WxeHandler found.");

    _currentStep = (wxeHandler == null) ? null : wxeHandler.CurrentFunction.ExecutingStep as WxePageStep;

    WxeStep step = _currentStep;
    do {
      _currentFunction = step as WxeFunction;
      if (_currentFunction != null)
        break;
      step = step.ParentStep;
    } while (step != null);
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

    public WxeHandler WxeHandler
    {
        get { return _wxeHandler; }
    }
}

}
