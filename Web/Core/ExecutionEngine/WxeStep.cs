using System;
using System.Collections;
using System.Collections.Specialized;
using Rubicon.Utilities;
using Rubicon.Collections;

namespace Rubicon.Web.ExecutionEngine
{

/// <summary>
/// Performs a single operation in a web application as part of a <see cref="WebFunction"/>.
/// </summary>
public abstract class WxeStep
{
  private WxeStep _parentStep = null;

  public WxeStep ParentStep
  {
    get { return _parentStep; }
    set { _parentStep = value; }
  }

  public void Execute ()
  {
    Execute (WxeContext.Current);
  }

  public abstract void Execute (WxeContext context);

  public virtual WxeStep ExecutingStep 
  {
    get { return this; }
  }

  public virtual NameObjectCollection Variables
  {
    get { return (_parentStep == null) ? null : _parentStep.Variables; }
  }

  public void ExecuteNextStep ()
  {
    ExecuteNextStep (WxeContext.Current);
  }

  public virtual void ExecuteNextStep (WxeContext context)
  {
    // the default implementation assumes that the current step does not contain any child steps and therefore executes the parent's next step
    if (ParentStep != null)
      ParentStep.ExecuteNextStep (context);
  }
}

public delegate void WxeMethod (WxeContext context);

public class WxeMethodStep: WxeStep
{
  private WxeMethod _method;

  public WxeMethodStep (WxeMethod method)
  {
    ArgumentUtility.CheckNotNull ("method", method);
    _method = method;
  }

  public override void Execute (WxeContext context)
  {
    _method (context);
  }
}

}
