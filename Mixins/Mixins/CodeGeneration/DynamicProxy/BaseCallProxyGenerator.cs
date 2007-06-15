using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Mixins.Definitions;
using System.Reflection;
using Rubicon.Utilities;
using System.Runtime.Serialization;

namespace Mixins.CodeGeneration.DynamicProxy
{
  public class BaseCallProxyGenerator
  {
    private static readonly MethodInfo _getObjectDataMethod =
        typeof (ISerializable).GetMethod ("GetObjectData", new Type[] { typeof (SerializationInfo), typeof (StreamingContext) });

    private readonly TypeGenerator _surroundingType;
    private readonly ExtendedClassEmitter _emitter;
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
        interfaces.Add (requiredType.Type);

      _emitter = new ExtendedClassEmitter (
          new NestedClassEmitter (
              surroundingTypeEmitter,
              "BaseCallProxy",
              typeof (object),
              interfaces.ToArray()));
      _emitter.AddCustomAttribute (new CustomAttributeBuilder (typeof (SerializableAttribute).GetConstructor (Type.EmptyTypes), new object[0]));

      _thisField = _emitter.InnerEmitter.CreateField ("__this", _surroundingType.TypeBuilder);
      _depthField = _emitter.InnerEmitter.CreateField ("__depth", typeof (int));

      ImplementConstructor();
      ImplementBaseCallsForOverriddenMethodsOnTarget();
      ImplementBaseCallsForRequirements();
    }

    public Type TypeBuilder
    {
      get { return _emitter.TypeBuilder; }
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

    public MethodInfo GetProxyMethodForOverriddenMethod (MethodDefinition method)
    {
      return _overriddenMethodToImplementationMap[method];
    }

    private void ImplementConstructor ()
    {
      ArgumentReference arg1 = new ArgumentReference (_surroundingType.TypeBuilder);
      ArgumentReference arg2 = new ArgumentReference (typeof (int));
      ConstructorEmitter ctor = _emitter.InnerEmitter.CreateConstructor (arg1, arg2);
      ctor.CodeBuilder.InvokeBaseConstructor();
      ctor.CodeBuilder.AddStatement (new AssignStatement (_thisField, arg1.ToExpression()));
      ctor.CodeBuilder.AddStatement (new AssignStatement (_depthField, arg2.ToExpression()));
      ctor.CodeBuilder.AddStatement (new ReturnStatement());
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
      CustomMethodEmitter methodOverride = new CustomMethodEmitter (_emitter.InnerEmitter, methodDefinitionOnTarget.FullName, attributes);
      methodOverride.CopyParametersAndReturnTypeFrom (methodDefinitionOnTarget.MethodInfo);

      BaseCallMethodGenerator methodGenerator = new BaseCallMethodGenerator (methodOverride, this);
      methodGenerator.AddBaseCallToNextInChain (methodDefinitionOnTarget);

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
      CustomMethodEmitter methodImplementation = _emitter.CreateMethodOverrideOrInterfaceImplementation (requiredMethod.InterfaceMethod);
      if (requiredMethod.ImplementingMethod.Overrides.Count == 0)
        methodImplementation.ImplementMethodByDelegation (_thisField, requiredMethod.ImplementingMethod.MethodInfo);
      else
      {
        Assertion.Assert (!_baseClassConfiguration.Methods.HasItem (requiredMethod.InterfaceMethod));
        BaseCallMethodGenerator methodGenerator = new BaseCallMethodGenerator (methodImplementation, this);
        methodGenerator.AddBaseCallToNextInChain (requiredMethod.ImplementingMethod);
      }
    }

    // Required base call method implemented by extension -> either as an overridde or not
    // If an overridde, delegate to next in chain, else simply delegate to the extension implementing it field
    private void ImplementBaseCallForRequirementOnMixin (RequiredBaseCallMethodDefinition requiredMethod)
    {
      CustomMethodEmitter methodImplementation = _emitter.CreateMethodOverrideOrInterfaceImplementation (requiredMethod.InterfaceMethod);
      if (requiredMethod.ImplementingMethod.Base == null) // this is not an override
      {
        BaseCallMethodGenerator methodGenerator = new BaseCallMethodGenerator (methodImplementation, this);
        methodGenerator.AddBaseCallToTarget (requiredMethod.ImplementingMethod);
      }
      else // this is an override
      {
        Assertion.Assert (!_baseClassConfiguration.Methods.HasItem (requiredMethod.InterfaceMethod));
        BaseCallMethodGenerator methodGenerator = new BaseCallMethodGenerator (methodImplementation, this);
        methodGenerator.AddBaseCallToNextInChain (requiredMethod.ImplementingMethod.Base);
      }
    }
  }
}