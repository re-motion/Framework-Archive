using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Mixins.Definitions;
using System.Reflection;
using Rubicon.Utilities;

using LoadArrayElementExpression = Mixins.CodeGeneration.DynamicProxy.DPExtensions.LoadArrayElementExpression;
using Mixins.CodeGeneration.DynamicProxy.DPExtensions;

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
      ImplementBaseCallsForOverrides();
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

    private void ImplementBaseCallsForOverrides ()
    {
      foreach (MethodDefinition method in _baseClassConfiguration.Methods)
      {
        if (method.Overrides.Count > 0)
          ImplementBaseCallForOverride (method);
      }
    }

    // Implements a base or interface method
    private void ImplementBaseCallForOverride (MethodDefinition methodDefinitionOnTarget)
    {
      Assertion.Assert (methodDefinitionOnTarget.DeclaringClass == _baseClassConfiguration);

      MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.HideBySig;
      CustomMethodEmitter methodOverride = new CustomMethodEmitter (_nestedEmitter.InnerEmitter, methodDefinitionOnTarget.FullName, attributes);
      methodOverride.CopyParametersAndReturnTypeFrom (methodDefinitionOnTarget.MethodInfo);
      AddBaseCallToNextInChain(methodOverride, methodDefinitionOnTarget);
      // methodOverride.InnerEmitter.Generate();

      _overriddenMethodToImplementationMap.Add (methodDefinitionOnTarget, methodOverride.MethodBuilder);
    }

    private void AddBaseCallToNextInChain(CustomMethodEmitter methodOverride, MethodDefinition methodDefinitionOnTarget)
    {
      Assertion.Assert (methodDefinitionOnTarget.DeclaringClass == _baseClassConfiguration);

      for (int potentialDepth = 0; potentialDepth < _baseClassConfiguration.Mixins.Count; ++potentialDepth)
      {
        MethodDefinition nextInChain = GetNextInBaseChain (methodDefinitionOnTarget, potentialDepth);
        AddBaseCallToTargetIfDepthMatches (methodOverride, nextInChain, potentialDepth);
      }
      AddUnconditionalBaseCallToTarget (methodOverride, methodDefinitionOnTarget);
    }

    private MethodDefinition GetNextInBaseChain (MethodDefinition methodDefinitionOnTarget, int potentialDepth)
    {
      Assertion.Assert (methodDefinitionOnTarget.DeclaringClass == _baseClassConfiguration);

      for (int i = potentialDepth; i < _baseClassConfiguration.Mixins.Count; ++i)
        if (methodDefinitionOnTarget.Overrides.HasItem (_baseClassConfiguration.Mixins[i].Type))
          return methodDefinitionOnTarget.Overrides[_baseClassConfiguration.Mixins[i].Type];
      return methodDefinitionOnTarget;
    }

    private void AddBaseCallToTargetIfDepthMatches (CustomMethodEmitter methodEmitter, MethodDefinition target, int requestedDepth)
    {
      methodEmitter.InnerEmitter.CodeBuilder.AddStatement (
          new IfStatement (
              new SameConditionExpression (_depthField.ToExpression(), new ConstReference (requestedDepth).ToExpression()),
          CreateBaseCallStatements (methodEmitter, target, methodEmitter.InnerEmitter.Arguments)));
    }

    private void AddUnconditionalBaseCallToTarget (CustomMethodEmitter methodEmitter, MethodDefinition target)
    {
      foreach (Statement statement in CreateBaseCallStatements (methodEmitter, target, methodEmitter.InnerEmitter.Arguments))
        methodEmitter.InnerEmitter.CodeBuilder.AddStatement (statement);
    }

    private Statement[] CreateBaseCallStatements (CustomMethodEmitter methodEmitter, MethodDefinition target, ArgumentReference[] args)
    {
      Expression[] argExpressions = Array.ConvertAll<ArgumentReference, Expression> (args, delegate (ArgumentReference a) { return a.ToExpression(); });
      if (target.DeclaringClass == _baseClassConfiguration)
      {
        MethodInfo baseCallMethod = _surroundingType.GetBaseCallMethodFor (target.MethodInfo);
        return new Statement[] {
          new ReturnStatement (new VirtualMethodInvocationExpression (_thisField, baseCallMethod, argExpressions))
        };
      }
      else
      {
        MixinDefinition mixin = (MixinDefinition) target.DeclaringClass;
        Reference mixinReference = GetMixinReference(methodEmitter, mixin);

        return new Statement[] {
            new ReturnStatement (new VirtualMethodInvocationExpression (mixinReference, target.MethodInfo, argExpressions))
        };
      }
    }

    private Reference GetMixinReference(CustomMethodEmitter methodEmitter, MixinDefinition mixin)
    {
      Reference extensionsReference = new IndirectFieldReference(_thisField, _surroundingType.ExtensionsField);
      Expression mixinExpression = new CastClassExpression (mixin.Type,
                                                            new LoadArrayElementExpression (mixin.MixinIndex, extensionsReference, typeof (object)));
      return new ExpressionReference (mixin.Type, mixinExpression, methodEmitter.InnerEmitter);
    }

    private void ImplementRequiredBaseCallTypes ()
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
        AddBaseCallToNextInChain (methodImplementation, requiredMethod.ImplementingMethod);
      }
    }

    // Required abse call method implemented by extension -> delegate to respective extension
    private void ImplementBaseCallForRequirementOnMixin (RequiredBaseCallMethodDefinition requiredMethod)
    {
      MixinDefinition mixin = (MixinDefinition) requiredMethod.ImplementingMethod.DeclaringClass;
      
      CustomMethodEmitter methodImplementation = _nestedEmitter.CreateMethodOverrideOrInterfaceImplementation (requiredMethod.InterfaceMethod);

      Reference mixinReference = GetMixinReference (methodImplementation, mixin);
      methodImplementation.ImplementMethodByDelegation (mixinReference, requiredMethod.ImplementingMethod.MethodInfo);
      // methodImplementation.InnerEmitter.Generate ();
    }

    public void Finish()
    {
      _nestedEmitter.InnerEmitter.BuildType();
    }
  }
}