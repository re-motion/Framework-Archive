using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using Rubicon.Utilities;
using Rubicon.Collections;

namespace Rubicon.Web.ExecutionEngine
{

/// <summary>
/// Performs a single operation in a web application as part of a <see cref="WxeFunction"/>.
/// </summary>
[Serializable]
public abstract class WxeStep: IDisposable
{
  private WxeStep _parentStep;
  private bool _disposed;

  public WxeStep()
  {
    WxeStep _parentStep = null;
    bool _disposed = false;
  }

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

  public WxeFunction RootFunction
  {
    get
    {
      WxeStep s = this;
      while (s.ParentStep != null)
        s = s.ParentStep;
      return s as WxeFunction;
    }
  }

  public WxeFunction ParentFunction
  {
    get 
    {
      for (WxeStep step = ParentStep;
           step != null;
           step = step.ParentStep)
      {
        WxeFunction function = step as WxeFunction;
        if (function != null)
          return function;
      }
      return null;
    }
  }
  
  public void Dispose()
  {
    if (! _disposed)
    {
      Dispose (true);
      GC.SuppressFinalize (this);
      _disposed = true;
    }
  }

  protected virtual void Dispose (bool disposing)
  {
  }

  ~WxeStep ()
  {
    if (! _disposed)
      Dispose (false);
  }

  protected static WxeVariableReference varref (string localVariable)
  {
    return new WxeVariableReference (localVariable);
  }

  protected Exception CurrentException
  {
    get 
    {
      for (WxeStep step = this;
           step != null;
           step = step.ParentStep)
      {
        if (step is WxeCatchBlock)
          return ((WxeCatchBlock)step).Exception;
      }

      return null;
    }
  }
}

public delegate void WxeMethod ();
public delegate void WxeMethodWithContext (WxeContext context);

[Serializable]
public class WxeMethodStep: WxeStep
{
  private WxeStepList _target;
  private string _methodName;
  private bool _hasContext;
  [NonSerialized]
  private WxeMethod _method;
  [NonSerialized]
  private WxeMethodWithContext _methodWithContext;

  public WxeMethodStep (WxeStepList target, string methodName, bool hasContext)
  {
    ArgumentUtility.CheckNotNull ("target", target);
    ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
    _target = target;
    _methodName = methodName;
    _hasContext = hasContext;
  }

//  public WxeMethodStep (WxeMethod method)
//    : this ((WxeStepList) method.Target, method.Method.Name, false)
//  {
//    ArgumentUtility.CheckNotNull ("method", method);
//    _method = method;
//  }
//
//  public WxeMethodStep (WxeMethodWithContext method)
//    : this ((WxeStepList) method.Target, method.Method.Name, true)
//  {
//    ArgumentUtility.CheckNotNull ("method", method);
//    _methodWithContext = method;
//  }

  public override void Execute (WxeContext context)
  {
    if (_hasContext)
    {
      if (_methodWithContext == null)
      {
        _methodWithContext = 
          (WxeMethodWithContext) Delegate.CreateDelegate (typeof (WxeMethodWithContext), _target, _methodName, false);
      }
      _methodWithContext (context);
    }
    else
    {
      if (_method == null)
        _method = (WxeMethod) Delegate.CreateDelegate (typeof (WxeMethod), _target, _methodName, false);
      _method ();
    }
  }
}

}
