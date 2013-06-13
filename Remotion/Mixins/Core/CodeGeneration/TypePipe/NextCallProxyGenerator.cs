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
using System.Linq;
using System.Reflection;
using Remotion.FunctionalProgramming;
using Remotion.Mixins.Definitions;
using Remotion.TypePipe.Dlr.Ast;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.TypeAssembly;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370: docs
  // TODO 5370: tests
  public class NextCallProxyGenerator : INextCallProxyGenerator
  {
    public ITargetTypeForNextCall GetTargetTypeWrapper (MutableType concreteTarget)
    {
      ArgumentUtility.CheckNotNull ("concreteTarget", concreteTarget);

      return new TargetTypeForNextCall (concreteTarget);
    }

    public INextCallProxy Create (
        IProxyTypeAssemblyContext context,
        TargetClassDefinition targetClassDefinition,
        IList<IMixinInfo> mixinInfos,
        ITargetTypeForNextCall targetTypeForNextCall)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("targetClassDefinition", targetClassDefinition);
      ArgumentUtility.CheckNotNull ("mixinInfos", mixinInfos);
      ArgumentUtility.CheckNotNull ("targetTypeForNextCall", targetTypeForNextCall);

      var concreteTarget = context.ProxyType;
      var nextCallProxyType = CreateNextCallProxyType(concreteTarget, targetClassDefinition);

      var thisField = AddPublicField (nextCallProxyType, "__this", concreteTarget);
      var depthField = AddPublicField (nextCallProxyType, "__depth", typeof (int));

      var constructor = AddConstructor (nextCallProxyType, concreteTarget, thisField, depthField);

      var nextCallMethodGenerator = new NextCallMethodGenerator (
          targetClassDefinition, targetTypeForNextCall, thisField, depthField, mixinInfos);
      var nextCallProxy = new NextCallProxy (nextCallProxyType, constructor, targetClassDefinition, new ExpressionBuilder(), nextCallMethodGenerator);

      nextCallProxy.ImplementBaseCallsForOverriddenMethodsOnTarget();
      nextCallProxy.ImplementBaseCallsForRequirements();

      return nextCallProxy;
    }

    private static MutableType CreateNextCallProxyType (MutableType concreteTarget, TargetClassDefinition targetClassDefinition)
    {
      // TODO 5370: Old code would add SerializableAttribute, instead of TypeAttributes. (Does this work too?!)
      var attributes = TypeAttributes.NestedPublic | TypeAttributes.Sealed | TypeAttributes.Serializable;
      var nextCallProxy =  concreteTarget.AddNestedType ("NextCallProxy", attributes, typeof (object));

      AddRequiredInterfaces (nextCallProxy, targetClassDefinition);

      return nextCallProxy;
    }

    private static void AddRequiredInterfaces (MutableType nextCallProxy, TargetClassDefinition targetClassDefinition)
    {
      var interfaces = EnumerableUtility
          .Singleton (typeof (IGeneratedNextCallProxyType))
          .Concat (targetClassDefinition.RequiredNextCallTypes.Select (requiredType => requiredType.Type));

      foreach (var ifc in interfaces)
        nextCallProxy.AddInterface (ifc);
    }

    private static Expression AddPublicField (MutableType nextCallProxyType, string name, Type type)
    {
      var field = nextCallProxyType.AddField (name, FieldAttributes.Public, type);
      return Expression.Field (new ThisExpression (nextCallProxyType), field);
    }

    private static MutableConstructorInfo AddConstructor (
        MutableType nextCallProxy, MutableType concreteTarget, Expression thisField, Expression depthField)
    {
      return nextCallProxy.AddConstructor (
          MethodAttributes.Public,
          new[] { new ParameterDeclaration (concreteTarget, "this"), new ParameterDeclaration (typeof (int), "depth") },
          ctx =>
          Expression.Block (
              ctx.CallBaseConstructor(),
              Expression.Assign (thisField, ctx.Parameters[0]),
              Expression.Assign (depthField, ctx.Parameters[1])));
    }
  }
}