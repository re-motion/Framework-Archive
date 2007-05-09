using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Mixins.Definitions;
using System.Reflection;
using Rubicon.Utilities;

using LoadArrayElementExpression = Mixins.CodeGeneration.DynamicProxy.DPExtensions.LoadArrayElementExpression;

namespace Mixins.CodeGeneration.DynamicProxy
{
  public class BaseCallProxyGenerator
  {
    private readonly TypeGenerator _surroundingType;
    private readonly ExtendedClassEmitter _nestedEmitter;
    private readonly BaseClassDefinition _baseClassConfiguration;
    private FieldReference _depthField;
    private FieldReference _thisField;
    private Dictionary<MethodDefinition, MethodInfo> _overriddenMethodToImplementationMap = new Dictionary<MethodDefinition, MethodInfo>();

    public BaseCallProxyGenerator (TypeGenerator surroundingType, ClassEmitter surroundingTypeEmitter)
    {
      _surroundingType = surroundingType;
      _baseClassConfiguration = surroundingType.Configuration;

      List<Type> interfaces = new List<Type> ();
      foreach (RequiredBaseCallTypeDefinition requiredType in _baseClassConfiguration.RequiredBaseCallTypes)
      {
        interfaces.Add (requiredType.Type);
      }

      _nestedEmitter = new ExtendedClassEmitter (new NestedClassEmitter (surroundingTypeEmitter, "BaseCallProxy", typeof (object),
          interfaces.ToArray ()));
      _nestedEmitter.AddCustomAttribute (new CustomAttributeBuilder (typeof (SerializableAttribute).GetConstructor(Type.EmptyTypes), new object[0]));

      _thisField = _nestedEmitter.InnerEmitter.CreateField ("_this", _surroundingType.TypeBuilder);
      _depthField = _nestedEmitter.InnerEmitter.CreateField ("_depth", typeof (int));

      GenerateConstructor();
      ImplementOverriddenMethods();
      ImplementRequiredBaseCallTypes();
    }

    public Type TypeBuilder
    {
      get { return _nestedEmitter.TypeBuilder; }
    }

    public MethodInfo GetProxyMethodForOverriddenMethod(MethodDefinition method)
    {
      return _overriddenMethodToImplementationMap[method];
    }

    private void GenerateConstructor ()
    {
      ArgumentReference arg1 = new ArgumentReference (_surroundingType.TypeBuilder);
      ArgumentReference arg2 = new ArgumentReference (typeof (int));
      ConstructorEmitter ctor = _nestedEmitter.InnerEmitter.CreateConstructor (arg1, arg2);
      ctor.CodeBuilder.InvokeBaseConstructor();
      ctor.CodeBuilder.AddStatement (new AssignStatement (_thisField, arg1.ToExpression ()));
      ctor.CodeBuilder.AddStatement (new AssignStatement (_depthField, arg2.ToExpression ()));
      ctor.CodeBuilder.AddStatement (new ReturnStatement ());
      // ctor.Generate();
    }

    private void ImplementOverriddenMethods ()
    {
      foreach (MethodDefinition method in _baseClassConfiguration.Methods)
      {
        if (method.Overrides.Count > 0)
          ImplementOverriddenMethod (method);
      }
    }

    // Implements a base or interface method
    private void ImplementOverriddenMethod (MethodDefinition methodDefinition)
    {
      MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.HideBySig;
      CustomMethodEmitter methodOverride = new CustomMethodEmitter (_nestedEmitter.InnerEmitter, methodDefinition.FullName, attributes);
      methodOverride.CopyParametersAndReturnTypeFrom (methodDefinition.MethodInfo, _nestedEmitter.InnerEmitter);
      AddStatementsToDelegateToNextInChain(methodOverride);
      // methodOverride.InnerEmitter.Generate();

      _overriddenMethodToImplementationMap.Add (methodDefinition, methodOverride.MethodBuilder);
    }

    private static void AddStatementsToDelegateToNextInChain(CustomMethodEmitter methodOverride)
    {
      methodOverride.ImplementMethodByThrowing (typeof (NotImplementedException), "Not implemented.");
    }

    private void ImplementRequiredBaseCallTypes ()
    {
      foreach (RequiredBaseCallTypeDefinition requiredType in _baseClassConfiguration.RequiredBaseCallTypes)
        foreach (RequiredBaseCallMethodDefinition requiredMethod in requiredType.BaseCallMethods)
          ImplementRequiredBaseCallMethod (requiredMethod);
    }

    private void ImplementRequiredBaseCallMethod (RequiredBaseCallMethodDefinition requiredMethod)
    {
      if (requiredMethod.ImplementingMethod.DeclaringClass == _baseClassConfiguration)
        ImplementRequiredBaseCallMethodOnThis (requiredMethod);
      else
        ImplementRequiredBaseCallMethodOnExtension (requiredMethod);
    }

    // Required base call method implemented by "this" -> either overridden or not
    // If overridden, delegate to next in chain, else simply delegate to "this" field
    private void ImplementRequiredBaseCallMethodOnThis (RequiredBaseCallMethodDefinition requiredMethod)
    {
      CustomMethodEmitter methodImplementation = _nestedEmitter.CreateMethodOverrideOrInterfaceImplementation (requiredMethod.InterfaceMethod);
      if (requiredMethod.ImplementingMethod.Overrides.Count == 0)
        methodImplementation.ImplementMethodByDelegation (_thisField, requiredMethod.ImplementingMethod.MethodInfo);
      else
      {
        Assertion.Assert (!_baseClassConfiguration.Methods.HasItem (requiredMethod.InterfaceMethod));
        AddStatementsToDelegateToNextInChain (methodImplementation);
      }
    }

    // Required abse call method implemented by extension -> delegate to respective extension
    private void ImplementRequiredBaseCallMethodOnExtension (RequiredBaseCallMethodDefinition requiredMethod)
    {
      MixinDefinition mixin = (MixinDefinition) requiredMethod.ImplementingMethod.DeclaringClass;
      
      CustomMethodEmitter methodImplementation = _nestedEmitter.CreateMethodOverrideOrInterfaceImplementation (requiredMethod.InterfaceMethod);
      methodImplementation.ImplementMethodByThrowing (typeof (NotImplementedException), "Not implemented.");

      /*LocalReference outerExtensionsLocal = methodImplementation.InnerEmitter.CodeBuilder.DeclareLocal (typeof(object[]));
      methodImplementation.InnerEmitter.CodeBuilder.AddStatement(new AssignStatement(outerExtensionsLocal,
          new ReferenceExpression(new IndirectFieldReference(_thisField, _surroundingType.ExtensionsField))));

      LocalReference mixinLocal = methodImplementation.InnerEmitter.CodeBuilder.DeclareLocal (mixin.Type);
      methodImplementation.InnerEmitter.CodeBuilder.AddStatement (new AssignStatement (mixinLocal,
        new CastClassExpression (mixin.Type, new LoadArrayElementExpression (mixin.MixinIndex, outerExtensionsLocal, typeof (object)))));
          
      _nestedEmitter.ImplementMethodByDelegation (methodImplementation.InnerEmitter, mixinLocal, requiredMethod.ImplementingMethod.MethodInfo);*/
      // methodImplementation.InnerEmitter.Generate ();
    }

    public void Finish()
    {
      _nestedEmitter.InnerEmitter.BuildType();
    }
  }
}