using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Reflection;
using Rubicon.Utilities;
using Rubicon.Collections;

namespace Rubicon.Web.ExecutionEngine
{

/// <summary> This delegate represents a parameterless method called. </summary>
/// <remarks> The method is usually an instance method of a <see cref="WxeStepList"/> or <see cref="WxeFunction"/>. </remarks>
public delegate void WxeMethod ();

/// <summary> This delegate represents a method accepting a <see cref="WxeContext"/> as a parameter. </summary>
/// <remarks> The method is usually an instance method of a <see cref="WxeStepList"/> or <see cref="WxeFunction"/>. </remarks>
public delegate void WxeMethodWithContext (WxeContext context);

/// <summary> Performs a step implemented by an instance method of a <see cref="WxeFunction"/>. </summary>
/// <include file='doc\include\ExecutionEngine\WxeMethodStep.xml' path='WxeMethodStep/Class/*' />
[Serializable]
public class WxeMethodStep: WxeStep
{
  /// <summary> The <see cref="WxeStepList"/> containing the <b>Method</b> used for this <b>WxeMethodStep</b>. </summary>
  private WxeStepList _target;
  /// <summary> The name of the method to be invoked in this <b>WxeMethodStep</b>. </summary>
  private string _methodName;
  /// <summary> <see langword="true"/> if the metho has a parameter of type <see cref="WxeContext"/>. </summary>
  private bool _hasContext;
  /// <summary> The cached <see cref="WxeMethod"/> delegate used to invoke this <b>WxeMethodStep</b>. </summary>
  [NonSerialized]
  private WxeMethod _method;
  /// <summary> The cached <see cref="WxeMethodWithContext"/> delegate used to invoke this <b>WxeMethodStep</b>. </summary>
  [NonSerialized]
  private WxeMethodWithContext _methodWithContext;

  /// <summary> Initalizes a new instance of the <b>WxeMethodStep</b> type. </summary>
  /// <include file='doc\include\ExecutionEngine\WxeMethodStep.xml' path='WxeMethodStep/Ctor/*' />
  public WxeMethodStep (WxeStepList target, MethodInfo method)
  {
    ArgumentUtility.CheckNotNull ("target", target);
    ArgumentUtility.CheckNotNull ("method", method);

    if (target.GetType() != method.DeclaringType || method.IsStatic)
      throw new WxeException ("Method step '" + method.Name + "' is no instance method of the type '" + target.GetType().FullName + "'.");
    
    ParameterInfo[] parameters = method.GetParameters();
    if (parameters.Length > 1)
      throw new WxeException ("Method step '" + method.Name + "', declared in type '" + method.DeclaringType.FullName + "', does not support more than one parameter.");
    if (parameters.Length == 1 && ! typeof (WxeContext).IsAssignableFrom (parameters[0].ParameterType))
      throw new WxeException ("Method step '" + method.Name + "', declared in type '" + method.DeclaringType.FullName + "', may only have a parameter of type WxeContext.");

    _target = target;
    _methodName = method.Name;

    _hasContext = parameters.Length == 1;
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

  /// <summary> Executes the method provided during the initizalion of this <see cref="WxeMethodStep"/>. </summary>
  /// <param name="context"> The <see cref="WxeContext"/> containing the information about the execution. </param>
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
