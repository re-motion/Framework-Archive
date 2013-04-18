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
using System.Reflection;
using Microsoft.Scripting.Ast;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  public class ExpressionConcreteMixinTypeIdentifierSerializer : ConcreteMixinTypeIdentifierSerializerBase
  {
    private static readonly ConstructorInfo s_constructor =
        MemberInfoFromExpressionUtility.GetConstructor (() => new ConcreteMixinTypeIdentifier (null, null, null));

    private static readonly ConstructorInfo s_hashSetConstructor =
        MemberInfoFromExpressionUtility.GetConstructor (() => new HashSet<MethodInfo> (new MethodInfo[0]));

    public Expression CreateNewExpression ()
    {
      // new ConcreteMixinTypeIdentifier (
      //     MixinType,
      //     new HashSet<MethodInfo> (Overriders)
      //     new HashSet<MethodInfo> (Overridden));

      return Expression.New (
          s_constructor,
          Expression.Constant (MixinType),
          Expression.New (s_hashSetConstructor, Expression.Constant (Overriders.ToArray())),
          Expression.New (s_hashSetConstructor, Expression.Constant (Overridden.ToArray())));
    }
  }
}