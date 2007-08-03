using System;
using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Rubicon.Mixins.CodeGeneration.DynamicProxy.DPExtensions;
using Rubicon.Mixins.Definitions;
using Rubicon.Utilities;

namespace Rubicon.Mixins.CodeGeneration.DynamicProxy
{
  internal class BaseCallMethodGenerator
  {
    private CustomMethodEmitter _methodEmitter;
    private BaseClassDefinition _baseClassConfiguration;
    private FieldReference _depthField;
    private FieldReference _thisField;
    private TypeGenerator _surroundingType;

    public BaseCallMethodGenerator (CustomMethodEmitter methodEmitter, BaseCallProxyGenerator baseCallProxyGenerator)
    {
      ArgumentUtility.CheckNotNull ("methodEmitter", methodEmitter);
      ArgumentUtility.CheckNotNull ("baseCallProxyGenerator", baseCallProxyGenerator);

      _methodEmitter = methodEmitter;
      _thisField = baseCallProxyGenerator.ThisField;
      _depthField = baseCallProxyGenerator.DepthField;
      _surroundingType = baseCallProxyGenerator.SurroundingType;
      _baseClassConfiguration = _surroundingType.Configuration;
    }

    public void AddBaseCallToNextInChain (MethodDefinition methodDefinitionOnTarget)
    {
      Assertion.IsTrue(methodDefinitionOnTarget.DeclaringClass == _baseClassConfiguration);

      for (int potentialDepth = 0; potentialDepth < _baseClassConfiguration.Mixins.Count; ++potentialDepth)
      {
        MethodDefinition nextInChain = GetNextInBaseChain (methodDefinitionOnTarget, potentialDepth);
        AddBaseCallToTargetIfDepthMatches (nextInChain, potentialDepth);
      }
      AddBaseCallToTarget (methodDefinitionOnTarget);
    }

    private MethodDefinition GetNextInBaseChain (MethodDefinition methodDefinitionOnTarget, int potentialDepth)
    {
      Assertion.IsTrue (methodDefinitionOnTarget.DeclaringClass == _baseClassConfiguration);

      for (int i = potentialDepth; i < _baseClassConfiguration.Mixins.Count; ++i)
        if (methodDefinitionOnTarget.Overrides.ContainsKey (_baseClassConfiguration.Mixins[i].Type))
          return methodDefinitionOnTarget.Overrides[_baseClassConfiguration.Mixins[i].Type];
      return methodDefinitionOnTarget;
    }

    private void AddBaseCallToTargetIfDepthMatches (MethodDefinition target, int requestedDepth)
    {
      _methodEmitter.InnerEmitter.CodeBuilder.AddStatement (
          new IfStatement (
              new SameConditionExpression (_depthField.ToExpression (), new ConstReference (requestedDepth).ToExpression ()),
          CreateBaseCallStatement (target, _methodEmitter.InnerEmitter.Arguments)));
    }

    public void AddBaseCallToTarget (MethodDefinition target)
    {
      Statement baseCallStatement = CreateBaseCallStatement (target, _methodEmitter.InnerEmitter.Arguments);
      _methodEmitter.InnerEmitter.CodeBuilder.AddStatement (baseCallStatement);
    }

    private Statement CreateBaseCallStatement (MethodDefinition target, ArgumentReference[] args)
    {
      Expression[] argExpressions = Array.ConvertAll<ArgumentReference, Expression> (args, delegate (ArgumentReference a) { return a.ToExpression (); });
      if (target.DeclaringClass == _baseClassConfiguration)
      {
        MethodInfo baseCallMethod = _surroundingType.GetBaseCallMethodFor (target.MethodInfo);
        return new ReturnStatement (new VirtualMethodInvocationExpression (_thisField, baseCallMethod, argExpressions));
      }
      else
      {
        MixinDefinition mixin = (MixinDefinition) target.DeclaringClass;
        MethodInfo baseCallMethod = GetMixinMethodToCall (target.MethodInfo);
        Reference mixinReference = GetMixinReference (mixin, baseCallMethod.DeclaringType);

        return new ReturnStatement (new VirtualMethodInvocationExpression (mixinReference, baseCallMethod, argExpressions));
      }
    }

    private MethodInfo GetMixinMethodToCall (MethodInfo targetMethod)
    {
      if (targetMethod.IsPublic)
        return targetMethod;
      else
        throw new NotImplementedException ("Non-public overriders are not implemented: " + targetMethod.DeclaringType + "." + targetMethod.Name);
    }

    private Reference GetMixinReference (MixinDefinition mixin, Type concreteMixinType)
    {
      Reference extensionsReference = new IndirectFieldReference (_thisField, _surroundingType.ExtensionsField);
      Expression mixinExpression = new CastClassExpression (concreteMixinType, 
        new LoadArrayElementExpression (mixin.MixinIndex, extensionsReference, typeof (object)));
      return new ExpressionReference (concreteMixinType, mixinExpression, _methodEmitter.InnerEmitter);
    }
  }
}
