﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Scripting.Ast;
using Remotion.Mixins.Definitions;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370
  public class NextCallMethodGenerator : INextCallMethodGenerator
  {
    private readonly TargetClassDefinition _targetClassDefinition;
    private readonly ITargetTypeForNextCall _targetTypeForNextCall;
    private readonly Expression _thisField;
    private readonly Expression _depthField;
    private readonly IList<ConcreteMixinType> _concreteMixinTypesWithNulls;

    public NextCallMethodGenerator (
        TargetClassDefinition targetClassDefinition,
        ITargetTypeForNextCall targetTypeForNextCall,
        Expression thisField,
        Expression depthField,
        IList<ConcreteMixinType> concreteMixinTypesWithNulls)
    {
      _targetClassDefinition = targetClassDefinition;
      _targetTypeForNextCall = targetTypeForNextCall;
      _thisField = thisField;
      _depthField = depthField;
      _concreteMixinTypesWithNulls = concreteMixinTypesWithNulls;
    }

    public Expression CreateBaseCallToNextInChain (MethodBodyContextBase ctx, MethodDefinition methodDefinitionOnTarget)
    {
      Assertion.IsTrue (methodDefinitionOnTarget.DeclaringClass == _targetClassDefinition);

      var expressions = new List<Expression>();

      for (int potentialDepth = 0; potentialDepth < _targetClassDefinition.Mixins.Count; ++potentialDepth)
      {
        var nextInChain = GetNextInChain (methodDefinitionOnTarget, potentialDepth);
        var baseCallIfDepthMatches = AddBaseCallToTargetIfDepthMatches (nextInChain, potentialDepth, ctx.Parameters.Cast<Expression>());
        expressions.Add (baseCallIfDepthMatches);
      }
      var baseCall = CreateBaseCallToTarget (ctx, methodDefinitionOnTarget);
      expressions.Add (baseCall);

      return Expression.Block (expressions);
    }

    private MethodDefinition GetNextInChain (MethodDefinition methodDefinitionOnTarget, int potentialDepth)
    {
      Assertion.IsTrue (methodDefinitionOnTarget.DeclaringClass == _targetClassDefinition);

      for (int i = potentialDepth; i < _targetClassDefinition.Mixins.Count; ++i)
        if (methodDefinitionOnTarget.Overrides.ContainsKey (_targetClassDefinition.Mixins[i].Type))
          return methodDefinitionOnTarget.Overrides[_targetClassDefinition.Mixins[i].Type];
      return methodDefinitionOnTarget;
    }

    private Expression AddBaseCallToTargetIfDepthMatches (MethodDefinition target, int requestedDepth, IEnumerable<Expression> argExpressions)
    {
      return Expression.IfThen (
              Expression.Equal (_depthField, Expression.Constant (requestedDepth)),
              CreateBaseCallStatement (target, argExpressions));
    }

    public Expression CreateBaseCallToTarget (MethodBodyContextBase ctx, MethodDefinition target)
    {
      return CreateBaseCallStatement (target, ctx.Parameters.Cast<Expression>());
    }

    private Expression CreateBaseCallStatement (MethodDefinition target, IEnumerable<Expression> argExpressions)
    {
      if (target.DeclaringClass == _targetClassDefinition)
        return CreateBaseCallToTargetClassStatement (target, argExpressions);
      else
        return CreateBaseCallToMixinStatement (target, argExpressions);
    }

    private Expression CreateBaseCallToTargetClassStatement (MethodDefinition target, IEnumerable<Expression> argExpressions)
    {
      MethodInfo baseCallMethod = _targetTypeForNextCall.GetBaseCallMethod (target.MethodInfo);
      return Expression.Call (_thisField, baseCallMethod, argExpressions);
    }

    private Expression CreateBaseCallToMixinStatement (MethodDefinition target, IEnumerable<Expression> argExpressions)
    {
      var mixin = (MixinDefinition) target.DeclaringClass;
      var baseCallMethod = GetMixinMethodToCall (mixin.MixinIndex, target);
      var mixinReference = GetMixinReference (mixin, baseCallMethod.DeclaringType);

      return Expression.Call (mixinReference, baseCallMethod, argExpressions);
    }

    private MethodInfo GetMixinMethodToCall (int mixinIndex, MethodDefinition mixinMethod)
    {
      if (mixinMethod.MethodInfo.IsPublic)
        return mixinMethod.MethodInfo;
      else
      {
        Assertion.IsNotNull (_concreteMixinTypesWithNulls[mixinIndex]);
        return _concreteMixinTypesWithNulls[mixinIndex].GetMethodWrapper (mixinMethod.MethodInfo);
      }
    }

    private Expression GetMixinReference (MixinDefinition mixin, Type concreteMixinType)
    {
      return Expression.Convert (
          Expression.ArrayAccess (_targetTypeForNextCall.ExtensionsField, Expression.Constant (mixin.MixinIndex)),
          concreteMixinType);
    }
  }
}