using System;
using System.Web;
using System.Web.UI;
using Rubicon.Collections;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;

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

  public void OnInit (IWxeTemplateControl control, HttpContext context)
  {
    WxeHandler wxeHandler = context.Handler as WxeHandler;
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
}

}
