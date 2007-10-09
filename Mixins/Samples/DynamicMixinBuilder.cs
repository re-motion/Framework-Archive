using System;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy;
using Rubicon.Collections;
using Rubicon.Utilities;
using Rubicon.CodeGeneration;

namespace Rubicon.Mixins.Samples
{
  public delegate object MethodInvocationHandler (object instance, MethodInfo method, object[] args, Func<object[], object> baseMethod);

  public class DynamicMixinBuilder
  {
    private static readonly ConstructorInfo s_attributeConstructor = typeof (OverrideAttribute).GetConstructor (Type.EmptyTypes);

    public static ModuleScope Scope = new ModuleScope (true);

    private readonly Type _targetType;
    private readonly Set<MethodInfo> _methodsToOverride = new Set<MethodInfo> ();

    public DynamicMixinBuilder (Type targetType)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      _targetType = targetType;
    }

    public Type BuildMixinType (MethodInvocationHandler methodInvocationHandler)
    {
      ArgumentUtility.CheckNotNull ("methodInvocationHandler", methodInvocationHandler);

      Type mixinBase = typeof (Mixin<object, object>);
      CustomClassEmitter emitter = new CustomClassEmitter (Scope, "DynamicMixinFor_" + _targetType.Name, mixinBase);

      foreach (MethodInfo method in _methodsToOverride)
        AddOverrider (emitter, method);

      return emitter.BuildType ();
    }

    private void AddOverrider (CustomClassEmitter emitter, MethodInfo method)
    {
      CustomMethodEmitter overrider = emitter.CreateMethod (method.Name, MethodAttributes.Public | MethodAttributes.Virtual);
      overrider.CopyParametersAndReturnType (method);
      overrider.AddCustomAttribute (new CustomAttributeBuilder (s_attributeConstructor, new object[0]));

      overrider.ImplementByReturningDefault ();
    }

    public void OverrideMethod (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);

      if (method.DeclaringType != _targetType)
        throw new ArgumentException ("The declaring type of the method must be the target type.", "method");

      _methodsToOverride.Add (method);
    }
  }
}
