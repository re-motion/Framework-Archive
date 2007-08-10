using System;
using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Rubicon.CodeGeneration;
using Rubicon.CodeGeneration.DPExtensions;
using Rubicon.Mixins.Definitions;
using Rubicon.Utilities;

namespace Rubicon.Mixins.CodeGeneration.DynamicProxy
{
  internal class BaseCallMethodGenerator
  {
    private CustomMethodEmitter _methodEmitter;
    private readonly MixinTypeGenerator[] _mixinTypeGenerators;
    private TargetClassDefinition _targetClassConfiguration;
    private FieldReference _depthField;
    private FieldReference _thisField;
    private TypeGenerator _surroundingType;

    public BaseCallMethodGenerator (CustomMethodEmitter methodEmitter, BaseCallProxyGenerator baseCallProxyGenerator,
        MixinTypeGenerator[] mixinTypeGenerators)
    {
      ArgumentUtility.CheckNotNull ("methodEmitter", methodEmitter);
      ArgumentUtility.CheckNotNull ("baseCallProxyGenerator", baseCallProxyGenerator);
      ArgumentUtility.CheckNotNull ("mixinTypeGenerators", mixinTypeGenerators);

      _methodEmitter = methodEmitter;
      _mixinTypeGenerators = mixinTypeGenerators;
      _thisField = baseCallProxyGenerator.ThisField;
      _depthField = baseCallProxyGenerator.DepthField;
      _surroundingType = baseCallProxyGenerator.SurroundingType;
      _targetClassConfiguration = _surroundingType.Configuration;
    }

    public void AddBaseCallToNextInChain (MethodDefinition methodDefinitionOnTarget)
    {
      Assertion.IsTrue(methodDefinitionOnTarget.DeclaringClass == _targetClassConfiguration);

      for (int potentialDepth = 0; potentialDepth < _targetClassConfiguration.Mixins.Count; ++potentialDepth)
      {
        MethodDefinition nextInChain = GetNextInBaseChain (methodDefinitionOnTarget, potentialDepth);
        AddBaseCallToTargetIfDepthMatches (nextInChain, potentialDepth);
      }
      AddBaseCallToTarget (methodDefinitionOnTarget);
    }

    private MethodDefinition GetNextInBaseChain (MethodDefinition methodDefinitionOnTarget, int potentialDepth)
    {
      Assertion.IsTrue (methodDefinitionOnTarget.DeclaringClass == _targetClassConfiguration);

      for (int i = potentialDepth; i < _targetClassConfiguration.Mixins.Count; ++i)
        if (methodDefinitionOnTarget.Overrides.ContainsKey (_targetClassConfiguration.Mixins[i].Type))
          return methodDefinitionOnTarget.Overrides[_targetClassConfiguration.Mixins[i].Type];
      return methodDefinitionOnTarget;
    }

    private void AddBaseCallToTargetIfDepthMatches (MethodDefinition target, int requestedDepth)
    {
      _methodEmitter.AddStatement (
          new IfStatement (
              new SameConditionExpression (_depthField.ToExpression (), new ConstReference (requestedDepth).ToExpression ()),
          CreateBaseCallStatement (target, _methodEmitter.ArgumentReferences)));
    }

    public void AddBaseCallToTarget (MethodDefinition target)
    {
      Statement baseCallStatement = CreateBaseCallStatement (target, _methodEmitter.ArgumentReferences);
      _methodEmitter.AddStatement (baseCallStatement);
    }

    private Statement CreateBaseCallStatement (MethodDefinition target, ArgumentReference[] args)
    {
      Expression[] argExpressions = Array.ConvertAll<ArgumentReference, Expression> (args, delegate (ArgumentReference a) { return a.ToExpression (); });
      if (target.DeclaringClass == _targetClassConfiguration)
      {
        MethodInfo baseCallMethod = _surroundingType.GetBaseCallMethod (target.MethodInfo);
        return new ReturnStatement (new VirtualMethodInvocationExpression (_thisField, baseCallMethod, argExpressions));
      }
      else
      {
        MixinDefinition mixin = (MixinDefinition) target.DeclaringClass;
        MethodInfo baseCallMethod = GetMixinMethodToCall (mixin.MixinIndex, target);
        Reference mixinReference = GetMixinReference (mixin, baseCallMethod.DeclaringType);

        return new ReturnStatement (new VirtualMethodInvocationExpression (mixinReference, baseCallMethod, argExpressions));
      }
    }

    private MethodInfo GetMixinMethodToCall (int mixinIndex, MethodDefinition mixinMethod)
    {
      if (mixinMethod.MethodInfo.IsPublic)
        return mixinMethod.MethodInfo;
      else
      {
        Assertion.IsNotNull (_mixinTypeGenerators[mixinIndex]);
        return _mixinTypeGenerators[mixinIndex].GetPublicMethodWrapper (mixinMethod);
      }
    }

    private Reference GetMixinReference (MixinDefinition mixin, Type concreteMixinType)
    {
      Reference extensionsReference = new IndirectFieldReference (_thisField, _surroundingType.ExtensionsField);
      Expression mixinExpression = new CastClassExpression (concreteMixinType, 
        new LoadArrayElementExpression (mixin.MixinIndex, extensionsReference, typeof (object)));
      return new ExpressionReference (concreteMixinType, mixinExpression, _methodEmitter);
    }
  }
}
