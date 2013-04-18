// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Linq;
using System.Reflection;
using Microsoft.Scripting.Ast;
using Remotion.FunctionalProgramming;
using Remotion.Mixins.Definitions;
using Remotion.TypePipe.Implementation;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370: docs
  // TODO 5370: tests
  public class NextCallProxyGenerator : INextCallProxyGenerator
  {
    private readonly IExpressionBuilder _expressionBuilder;
    private readonly INextCallMethodGenerator _nextCallMethodGenerator;

    public NextCallProxyGenerator (IExpressionBuilder expressionBuilder, INextCallMethodGenerator nextCallMethodGenerator)
    {
      ArgumentUtility.CheckNotNull ("expressionBuilder", expressionBuilder);
      ArgumentUtility.CheckNotNull ("nextCallMethodGenerator", nextCallMethodGenerator);

      _expressionBuilder = expressionBuilder;
      _nextCallMethodGenerator = nextCallMethodGenerator;
    }

    public INextCallProxy Create (
        TypeAssemblyContext context, TargetClassDefinition targetClassDefinition, IList<ConcreteMixinType> concreteMixinTypesWithNulls)
    {
      var concreteTarget = context.ProxyType;

      var nextCallProxy = CreateNextCallProxyType(context, concreteTarget);
      AddRequiredInterfaces (nextCallProxy, targetClassDefinition);
      // tODO 5370: Old code woul add SerializableAttribute.

      var thisField = nextCallProxy.AddField ("__this", FieldAttributes.Private, concreteTarget);
      var depthField = nextCallProxy.AddField ("__depth", FieldAttributes.Private, typeof (int));

      var constructor = AddConstructor (nextCallProxy, concreteTarget, thisField, depthField);

      var overriddenMethodToImplementationMap =
          ImplementBaseCallsForOverriddenMethodsOnTarget (targetClassDefinition, concreteMixinTypesWithNulls, nextCallProxy);
      ImplementBaseCallsForRequirements (targetClassDefinition, concreteMixinTypesWithNulls, nextCallProxy, overriddenMethodToImplementationMap);

      return new NextCallProxy (nextCallProxy, constructor, overriddenMethodToImplementationMap);
    }

    private void ImplementBaseCallsForRequirements (
        TargetClassDefinition targetClassDefinition,
        IList<ConcreteMixinType> concreteMixinTypesWithNulls,
        MutableType nextCallProxy,
        Dictionary<MethodDefinition, MethodInfo> overriddenMethodToImplementationMap)
    {
      foreach (var requiredType in targetClassDefinition.RequiredNextCallTypes)
        foreach (var requiredMethod in requiredType.Methods)
          ImplementBaseCallForRequirement (
              nextCallProxy, requiredMethod, targetClassDefinition, concreteMixinTypesWithNulls, overriddenMethodToImplementationMap);
    }

    private void ImplementBaseCallForRequirement (
        MutableType nextCallProxy,
        RequiredMethodDefinition requiredMethod,
        TargetClassDefinition targetClassDefinition,
        IList<ConcreteMixinType> concreteMixinTypesWithNulls,
        Dictionary<MethodDefinition, MethodInfo> overriddenMethodToImplementationMap)
    {
      if (requiredMethod.ImplementingMethod.DeclaringClass == targetClassDefinition)
        ImplementBaseCallForRequirementOnTarget (nextCallProxy, targetClassDefinition, concreteMixinTypesWithNulls, requiredMethod);
      else
        ImplementBaseCallForRequirementOnMixin (nextCallProxy, concreteMixinTypesWithNulls, requiredMethod, overriddenMethodToImplementationMap);
    }

    // Required base call method implemented by "this" -> either overridden or not
    // If overridden, delegate to next in chain, else simply delegate to "this" field
    private void ImplementBaseCallForRequirementOnTarget (
        MutableType nextCallProxy,
        TargetClassDefinition targetClassDefinition,
        IList<ConcreteMixinType> concreteMixinTypesWithNulls,
        RequiredMethodDefinition requiredMethod)
    {
      var methodImplementation = nextCallProxy.AddExplicitOverride (requiredMethod.InterfaceMethod, ctx => Expression.Default (ctx.ReturnType));
      // TODO 5370: refactor if-else away.
      if (requiredMethod.ImplementingMethod.Overrides.Count == 0) // this is not an overridden method, call method directly on _this
      {
        methodImplementation.SetBody (ctx => _nextCallMethodGenerator.CreateBaseCallToTarget (ctx, requiredMethod.ImplementingMethod));
      }
      else // this is an override, go to next in chain
      {
        // a base call for this might already have been implemented as an overriden method, but we explicitly implement the call chains anyway: it's
        // slightly easier and better for performance
        Assertion.IsFalse (targetClassDefinition.Methods.ContainsKey (requiredMethod.InterfaceMethod));
        methodImplementation.SetBody (
            ctx => _nextCallMethodGenerator.CreateBaseCallToNextInChain (ctx, requiredMethod.ImplementingMethod, concreteMixinTypesWithNulls));
      }
    }

    // Required base call method implemented by extension -> either as an overridde or not
    // If an overridde, delegate to next in chain, else simply delegate to the extension implementing it field
    private void ImplementBaseCallForRequirementOnMixin (
        MutableType nextCallProxy,
        IList<ConcreteMixinType> concreteMixinTypesWithNulls,
        RequiredMethodDefinition requiredMethod,
        Dictionary<MethodDefinition, MethodInfo> overriddenMethodToImplementationMap)
    {
      var methodImplementation = nextCallProxy.AddExplicitOverride (requiredMethod.InterfaceMethod, ctx => Expression.Default (ctx.ReturnType));
      // TODO 5370: refactor if-else away.
      if (requiredMethod.ImplementingMethod.Base == null) // this is not an override, call method directly on extension
      {
        methodImplementation.SetBody (ctx => _nextCallMethodGenerator.CreateBaseCallToTarget (ctx, requiredMethod.ImplementingMethod));
      }
      else // this is an override, go to next in chain
      {
        // a base call for this has already been implemented as an overriden method, but we explicitly implement the call chains anyway: it's
        // slightly easier and better for performance
        Assertion.IsTrue (overriddenMethodToImplementationMap.ContainsKey (requiredMethod.ImplementingMethod.Base));
        methodImplementation.SetBody (
            ctx => _nextCallMethodGenerator.CreateBaseCallToNextInChain (ctx, requiredMethod.ImplementingMethod.Base, concreteMixinTypesWithNulls));
      }
    }

    private static MutableType CreateNextCallProxyType (TypeAssemblyContext context, MutableType concreteTarget)
    {
      var name = concreteTarget.Name + "_NextCallProxy";
      return context.CreateType (name, concreteTarget.Namespace, TypeAttributes.NestedPublic | TypeAttributes.Sealed, typeof (object));
    }

    private static void AddRequiredInterfaces (MutableType nextCallProxy, TargetClassDefinition targetClassDefinition)
    {
      var interfaces = EnumerableUtility
          .Singleton (typeof (IGeneratedNextCallProxyType))
          .Concat (targetClassDefinition.RequiredNextCallTypes.Select (requiredType => requiredType.Type));

      foreach (var ifc in interfaces)
        nextCallProxy.AddInterface (ifc);
    }

    private static MutableConstructorInfo AddConstructor (
        MutableType nextCallProxy, MutableType concreteTarget, MutableFieldInfo thisField, MutableFieldInfo depthField)
    {
      return nextCallProxy.AddConstructor (
          MethodAttributes.Public,
          new[] { new ParameterDeclaration (concreteTarget, "this"), new ParameterDeclaration (typeof (int), "depth") },
          ctx =>
          Expression.Block (
              Expression.Assign (Expression.Field (ctx.This, thisField), ctx.Parameters[0]),
              Expression.Assign (Expression.Field (ctx.This, depthField), ctx.Parameters[1])));
    }


    private Dictionary<MethodDefinition, MethodInfo> ImplementBaseCallsForOverriddenMethodsOnTarget (
        TargetClassDefinition targetClassDefinition, IList<ConcreteMixinType> concreteMixinTypesWithNulls, MutableType nextCallProxy)
    {
      var overriddenMethodToImplementationMap = new Dictionary<MethodDefinition, MethodInfo> ();

      foreach (var method in targetClassDefinition.GetAllMethods ().Where (method => method.Overrides.Count > 0))
      {
        Assertion.IsTrue (method.DeclaringClass == targetClassDefinition);
        ImplementBaseCallForOverridenMethodOnTarget (method, nextCallProxy, concreteMixinTypesWithNulls, overriddenMethodToImplementationMap);
      }

      return overriddenMethodToImplementationMap;
    }

    private void ImplementBaseCallForOverridenMethodOnTarget (
        MethodDefinition methodDefinitionOnTarget,
        MutableType nextCallProxy,
        IList<ConcreteMixinType> concreteMixinTypesWithNulls,
        Dictionary<MethodDefinition, MethodInfo> overriddenMethodToImplementationMap)
    {
      var attributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual;
      var md = MethodDeclaration.CreateEquivalent (methodDefinitionOnTarget.MethodInfo);
      var methodOverride = nextCallProxy.AddMethod (
          methodDefinitionOnTarget.FullName,
          attributes,
          md,
          ctx => _nextCallMethodGenerator.CreateBaseCallToNextInChain (ctx, methodDefinitionOnTarget, concreteMixinTypesWithNulls));

      overriddenMethodToImplementationMap.Add (methodDefinitionOnTarget, methodOverride);

      // If the base type of the emitter (object) already has the method being overridden (ToString, Equals, etc.), mixins could use the base 
      // implementation of the method rather than coming via the next call interface. Therefore, we need to override that base method and point it
      // towards our next call above.
      Assertion.IsTrue (
          nextCallProxy.BaseType == typeof (object),
          "This code assumes that only non-generic methods could match on the base type, which holds for object.");
      // Since object has no generic methods, we can use the exact parameter types to find the equivalent method.
      var equivalentMethodOnProxyBase = nextCallProxy.BaseType.GetMethod (
          methodDefinitionOnTarget.Name,
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
          null,
          methodOverride.GetParameters ().Select (p => p.ParameterType).ToArray (),
          null);
      if (equivalentMethodOnProxyBase != null && equivalentMethodOnProxyBase.IsVirtual)
      {
        nextCallProxy.GetOrAddOverride (equivalentMethodOnProxyBase)
                     .SetBody (ctx => _expressionBuilder.CreateDelegation (ctx, ctx.This, methodOverride));
      }
    }
  }
}