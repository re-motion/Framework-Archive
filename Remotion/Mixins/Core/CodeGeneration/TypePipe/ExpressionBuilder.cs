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
using System.Linq;
using System.Reflection;
using Microsoft.Scripting.Ast;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.Context;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370: docs
  // TODO 5370: tests
  public class ExpressionBuilder : IExpressionBuilder
  {
    private static readonly MethodInfo s_initializeMethod =
        MemberInfoFromExpressionUtility.GetMethod ((IInitializableMixinTarget o) => o.Initialize());

    public Expression CreateNewClassContext (ClassContext classContext)
    {
      ArgumentUtility.CheckNotNull ("classContext", classContext);

      var serializer = new ExpressionClassContextSerializer();
      classContext.Serialize (serializer);

      return serializer.CreateNewExpression();
    }

    public Expression CreateInitialization (MutableType concreteTarget, Expression extensionsField)
    {
      ArgumentUtility.CheckNotNull ("concreteTarget", concreteTarget);
      ArgumentUtility.CheckNotNull ("extensionsField", extensionsField);

      // if (__extensions == null)
      //   ((IInitializableMixinTarget) this).Initialize();

      return Expression.IfThen (
          Expression.Equal (extensionsField, Expression.Constant (null)),
          Expression.Call (new ThisExpression (concreteTarget), s_initializeMethod));
    }

    public Expression CreateDelegation (MethodBodyContextBase bodyContext, Expression instance, MethodInfo methodToCall)
    {
      ArgumentUtility.CheckNotNull ("bodyContext", bodyContext);
      ArgumentUtility.CheckNotNull ("instance", instance);
      ArgumentUtility.CheckNotNull ("methodToCall", methodToCall);

      // instance.MethodToCall(<parameters>);

      if (methodToCall.IsGenericMethodDefinition)
        methodToCall = methodToCall.MakeTypePipeGenericMethod (bodyContext.GenericParameters.ToArray());

      return Expression.Call (instance, methodToCall, bodyContext.Parameters.Cast<Expression>());
    }

    public Expression CreateInitializingDelegation (
        MethodBodyContextBase bodyContext, Expression extensionsField, Expression instance, MethodInfo methodToCall)
    {
      ArgumentUtility.CheckNotNull ("bodyContext", bodyContext);
      ArgumentUtility.CheckNotNull ("extensionsField", extensionsField);
      ArgumentUtility.CheckNotNull ("instance", instance);
      ArgumentUtility.CheckNotNull ("methodToCall", methodToCall);

      // <CreateInitialization>
      // <CreateDelegation>

      return Expression.Block (
          CreateInitialization (bodyContext.DeclaringType, extensionsField),
          CreateDelegation (bodyContext, instance, methodToCall));
    }
  }
}