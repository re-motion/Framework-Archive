using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Mixins.Definitions;
using System.Reflection;
using Rubicon.Utilities;

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
      ImplementBaseCallsForOverriddenMethodsOnTarget();
      ImplementBaseCallsForRequirements();
    }

    public Type TypeBuilder
    {
      get { return _nestedEmitter.TypeBuilder; }
    }

    public FieldReference ThisField
    {
      get { return _thisField; }
    }

    public FieldReference DepthField
    {
      get { return _depthField; }
    }

    public TypeGenerator SurroundingType
    {
      get { return _surroundingType; }
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

    private void ImplementBaseCallsForOverriddenMethodsOnTarget ()
    {
      foreach (MethodDefinition method in _baseClassConfiguration.Methods)
      {
        if (method.Overrides.Count > 0)
          ImplementBaseCallForOverridenMethodOnTarget (method);
      }
    }

    private void ImplementBaseCallForOverridenMethodOnTarget (MethodDefinition methodDefinitionOnTarget)
    {
      Assertion.Assert (methodDefinitionOnTarget.DeclaringClass == _baseClassConfiguration);

      MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.HideBySig;
      CustomMethodEmitter methodOverride = new CustomMethodEmitter (_nestedEmitter.InnerEmitter, methodDefinitionOnTarget.FullName, attributes);
      methodOverride.CopyParametersAndReturnTypeFrom (methodDefinitionOnTarget.MethodInfo);

      BaseCallMethodGenerator methodGenerator = new BaseCallMethodGenerator (methodOverride, this);
      methodGenerator.AddBaseCallToNextInChain (methodDefinitionOnTarget);
      // methodOverride.InnerEmitter.Generate();

      _overriddenMethodToImplementationMap.Add (methodDefinitionOnTarget, methodOverride.MethodBuilder);
    }

    private void ImplementBaseCallsForRequirements ()
    {
      foreach (RequiredBaseCallTypeDefinition requiredType in _baseClassConfiguration.RequiredBaseCallTypes)
        foreach (RequiredBaseCallMethodDefinition requiredMethod in requiredType.BaseCallMethods)
          ImplementBaseCallForRequirement (requiredMethod);
    }

    private void ImplementBaseCallForRequirement (RequiredBaseCallMethodDefinition requiredMethod)
    {
      if (requiredMethod.ImplementingMethod.DeclaringClass == _baseClassConfiguration)
        ImplementBaseCallForRequirementOnTarget (requiredMethod);
      else
        ImplementBaseCallForRequirementOnMixin (requiredMethod);
    }

    // Required base call method implemented by "this" -> either overridden or not
    // If overridden, delegate to next in chain, else simply delegate to "this" field
    private void ImplementBaseCallForRequirementOnTarget (RequiredBaseCallMethodDefinition requiredMethod)
    {
      CustomMethodEmitter methodImplementation = _nestedEmitter.CreateMethodOverrideOrInterfaceImplementation (requiredMethod.InterfaceMethod);
      if (requiredMethod.ImplementingMethod.Overrides.Count == 0)
        methodImplementation.ImplementMethodByDelegation (_thisField, requiredMethod.ImplementingMethod.MethodInfo);
      else
      {
        Assertion.Assert (!_baseClassConfiguration.Methods.HasItem (requiredMethod.InterfaceMethod));
        BaseCallMethodGenerator methodGenerator = new BaseCallMethodGenerator (methodImplementation, this);
        methodGenerator.AddBaseCallToNextInChain (requiredMethod.ImplementingMethod);
      }
    }

    // Required base call method implemented by extension -> delegate to respective extension
    private void ImplementBaseCallForRequirementOnMixin (RequiredBaseCallMethodDefinition requiredMethod)
    {
      CustomMethodEmitter methodImplementation = _nestedEmitter.CreateMethodOverrideOrInterfaceImplementation (requiredMethod.InterfaceMethod);
      BaseCallMethodGenerator methodGenerator = new BaseCallMethodGenerator (methodImplementation, this);
      methodGenerator.AddBaseCallToTarget (requiredMethod.ImplementingMethod);
      // methodImplementation.InnerEmitter.Generate ();
    }

    public void Finish()
    {
      _nestedEmitter.InnerEmitter.BuildType();
    }
  }
}