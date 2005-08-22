using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Reflection;
using Rubicon.Utilities;
using Rubicon.Collections;

namespace Rubicon.Web.ExecutionEngine
{

/// <summary>
/// Performs a single operation in a web application as part of a <see cref="WxeFunction"/>.
/// </summary>
[Serializable]
public abstract class WxeStep
{
  private WxeStep _parentStep = null;
  private bool _isAborted = false;

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

  public bool IsAborted
  {
    get { return _isAborted; }
  }
  
  /// <summary> Aborts the <b>WxeStep</b> by calling <see cref="AbortRecursive"/>. </summary>
  /// <remarks> 
  ///   A single <b>WxeStep</b> is usually not aborted. This method is usually called by aborting the 
  ///   a <see cref="WxeFunctionState"/> and subsequently the contained <see cref="WxeFunction"/>, which is derived
  ///   from <b>WxeStep</b>.
  /// </remarks>
  public void Abort()
  {
    if (! _isAborted)
    {
      AbortRecursive();
      _isAborted = true;
    }
  }

  protected virtual void AbortRecursive()
  {
  }
}

public delegate void WxeMethod ();
public delegate void WxeMethodWithContext (WxeContext context);

/// <summary> Performs a step implemented by an instance method of a <see cref="WxeFunction"/>. </summary>
/// <example>
///   An example where the 3rd step of the <see cref="WxeFunction"/> <b>MyFunction</b> is a <b>WxeMethodStep</b>
///   <code>
/// public class MyFunction: WxeFunction 
/// {
///   ...
///   // Step2
///   private void Step3
///   {
///     // Do something
///   }
///   // Step4
///   ...
/// }
///   </code>
/// </example>
[Serializable]
public class WxeMethodStep: WxeStep
{
  /// <summary> The <see cref="WxeStepList"/> containing the <b>Method</b> used for this <b>WxeMethodStep</b>. </summary>
  private WxeStepList _target;
  /// <summary> The name of the method to be invoked in this <b>WxeMethodStep</b>. </summary>
  private string _methodName;
  /// <summary> <see langword="true"/> if the method has a context, i.e. parameters. </summary>
  private bool _hasContext;
  /// <summary> The cached <see cref="WxeMethod"/> delegate used to invoke this <b>WxeMethodStep</b>. </summary>
  [NonSerialized]
  private WxeMethod _method;
  /// <summary> The cached <see cref="WxeMethodWithContext"/> delegate used to invoke this <b>WxeMethodStep</b>. </summary>
  [NonSerialized]
  private WxeMethodWithContext _methodWithContext;

  /// <summary> Initalizes a new instance of the <b>WxeMethodStep</b> type. </summary>
  /// <param name="target">
  ///   The <see cref="WxeStepList"/> containing the <b>Method</b> used for this <b>WxeMethodStep</b>. 
  ///   Must not be <see langword="null"/>.
  /// </param>
  /// <param name="method"> 
  ///   The <see cref="MethodInfo"/> of the <b>Method</b> used for this <b>MethodStep</b>. 
  ///   Must not be <see langword="null"/>. The specified method must be part of the <paramref name="target"/>'s type.
  /// </param>
  public WxeMethodStep (WxeStepList target, MethodInfo method)
  {
    ArgumentUtility.CheckNotNull ("target", target);
    ArgumentUtility.CheckNotNull ("method", method);
    if (target.GetType() != method.DeclaringType)
      throw new ArgumentException ("The DeclaringType of 'method' does not match the type of 'target'.");
    _target = target;
    _methodName = method.Name;
    _hasContext = method.GetParameters().Length > 0;
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
