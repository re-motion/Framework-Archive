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
using System.Reflection;
using Microsoft.Scripting.Ast;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.Context;
using Remotion.TypePipe.Expressions;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370: docs
  // TODO 5370: tests
  public class ComplexExpressionBuilder : IComplexExpressionBuilder
  {
    private static readonly MethodInfo s_initializeMethod =
        MemberInfoFromExpressionUtility.GetMethod ((IInitializableMixinTarget o) => o.Initialize());

    public Expression CreateNewClassContextExpression (ClassContext classContext)
    {
      ArgumentUtility.CheckNotNull ("classContext", classContext);

      var serializer = new ExpressionClassContextSerializer();
      classContext.Serialize (serializer);

      return serializer.CreateNewExpression();
    }

    public Expression CreateInitializationExpression (ThisExpression @this, FieldInfo extensionsField)
    {
      ArgumentUtility.CheckNotNull ("this", @this);
      ArgumentUtility.CheckNotNull ("extensionsField", extensionsField);


      return Expression.IfThen (
          Expression.Equal (Expression.Field (@this, extensionsField), Expression.Constant (null)),
          Expression.Call (@this, s_initializeMethod));
    }
  }
}