using System;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy;
using Rubicon.CodeGeneration.DPExtensions;
using Rubicon.Collections;
using Rubicon.Utilities;
using Rubicon.CodeGeneration;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Rubicon.Mixins.Samples
{
  public delegate object MethodInvocationHandler (object instance, MethodInfo method, object[] args, Func<object[], object> baseMethod);

  public class DynamicMixinBuilder
  {
    private static readonly ConstructorInfo s_attributeConstructor = typeof (OverrideAttribute).GetConstructor (Type.EmptyTypes);
    private static readonly MethodInfo s_handlerInvokeMethod = typeof (MethodInvocationHandler).GetMethod ("Invoke");

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

      FieldReference invocationHandlerField = emitter.CreateStaticField ("InvocationHandler", typeof (MethodInvocationHandler));

      foreach (MethodInfo method in _methodsToOverride)
        AddOverrider (emitter, method, invocationHandlerField);

      Type builtType = emitter.BuildType ();
      builtType.GetField (invocationHandlerField.Reference.Name).SetValue (null, methodInvocationHandler);
      return builtType;
    }

    private void AddOverrider (CustomClassEmitter emitter, MethodInfo method, Reference invocationHandler)
    {
      CustomMethodEmitter overrider = emitter.CreateMethod (method.Name, MethodAttributes.Public | MethodAttributes.Virtual);
      overrider.CopyParametersAndReturnType (method);
      overrider.AddCustomAttribute (new CustomAttributeBuilder (s_attributeConstructor, new object[0]));

      Expression thisValue = GetThisValueExpression(emitter);
      Expression overriddenMethodValue = new MethodTokenExpression (method);
      LocalReference argsLocal = CopyArgumentsToLocalVariable (overrider);
      
      TypeReference handlerReference = new TypeReferenceWrapper (invocationHandler, typeof (MethodInvocationHandler));
      Expression[] handlerArgs = new Expression[] { thisValue, overriddenMethodValue, argsLocal.ToExpression(), NullExpression.Instance };

      Expression invocationHandlerResult = new VirtualMethodInvocationExpression (handlerReference, s_handlerInvokeMethod, handlerArgs);

      overrider.ImplementByReturning (new ConvertExpression (method.ReturnType, typeof (object), invocationHandlerResult));
    }

    private Expression GetThisValueExpression (CustomClassEmitter emitter)
    {
      PropertyInfo thisProperty = emitter.BaseType.GetProperty ("This", BindingFlags.NonPublic | BindingFlags.Instance);
      return new PropertyReference (SelfReference.Self, thisProperty).ToExpression ();
    }

    private LocalReference CopyArgumentsToLocalVariable (CustomMethodEmitter overrider)
    {
      LocalReference argsLocal = overrider.DeclareLocal (typeof (object[]));

      ArgumentReference[] argumentReferences = overrider.ArgumentReferences;
      overrider.AddStatement (new AssignStatement (argsLocal, new NewArrayExpression (argumentReferences.Length, typeof (object))));
      for (int i = 0; i < argumentReferences.Length; ++i)
      {
        Expression castArgument = new ConvertExpression (typeof (object), argumentReferences[i].Type, argumentReferences[i].ToExpression ());
        overrider.AddStatement (new AssignArrayStatement (argsLocal, i, castArgument));
      }
      return argsLocal;
    }

    public void OverrideMethod (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);

      if (method.DeclaringType != _targetType)
        throw new ArgumentException ("The declaring type of the method must be the target type.", "method");

      _methodsToOverride.Add (method);
    }

    public static Assembly ResolveAssembly (object sender, ResolveEventArgs args)
    {
      if (DynamicMixinBuilder.Scope.StrongNamedModule != null && args.Name == DynamicMixinBuilder.Scope.StrongNamedModule.Assembly.FullName)
        return DynamicMixinBuilder.Scope.StrongNamedModule.Assembly;
      else if (DynamicMixinBuilder.Scope.WeakNamedModule != null && args.Name == DynamicMixinBuilder.Scope.WeakNamedModule.Assembly.FullName)
        return DynamicMixinBuilder.Scope.WeakNamedModule.Assembly;
      else
        return null;
    }
  }
}
