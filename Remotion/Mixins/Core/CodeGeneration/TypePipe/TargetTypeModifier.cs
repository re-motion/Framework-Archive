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
using System.Diagnostics;
using System.Reflection;
using Microsoft.Scripting.Ast;
using Remotion.Mixins.Context;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370: Docs.
  public class TargetTypeModifier : ITargetTypeModifier
  {
    private readonly IComplexExpressionBuilder _complexExpressionBuilder;

    private static readonly ConstructorInfo s_debuggerBrowsableAttributeCtor =
        MemberInfoFromExpressionUtility.GetConstructor (() => new DebuggerBrowsableAttribute (DebuggerBrowsableState.Never));

    private static readonly ConstructorInfo s_mixinArrayInitializerCtor =
        MemberInfoFromExpressionUtility.GetConstructor (() => new MixinArrayInitializer (null, Type.EmptyTypes));

    public TargetTypeModifier (IComplexExpressionBuilder complexExpressionBuilder)
    {
      ArgumentUtility.CheckNotNull ("complexExpressionBuilder", complexExpressionBuilder);

      _complexExpressionBuilder = complexExpressionBuilder;
    }

    public TargetTypeModifierContext CreateContext (MutableType targetType)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);

      return new TargetTypeModifierContext (targetType);
    }

    public void ImplementInterfaces (TargetTypeModifierContext context, IEnumerable<Type> interfacesToImplement)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("interfacesToImplement", interfacesToImplement);

      foreach (var ifc in interfacesToImplement)
        context.ConcreteTarget.AddInterface (ifc);
    }

    public void AddFields (TargetTypeModifierContext context, Type nextCallProxyType)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("nextCallProxyType", nextCallProxyType);

      var tt = context.ConcreteTarget;
      var privateStatic = FieldAttributes.Private | FieldAttributes.Static;
      context.ClassContextField = AddDebuggerInvisibleField (tt, "__classContext", typeof (ClassContext), privateStatic);
      context.MixinArrayInitializerField = AddDebuggerInvisibleField (tt, "__mixinArrayInitializer", typeof (MixinArrayInitializer), privateStatic);
      context.ExtensionsField = AddDebuggerInvisibleField (tt, "__extensions", typeof (object[]), FieldAttributes.Private);
      context.FirstField = AddDebuggerInvisibleField (tt, "__first", nextCallProxyType, FieldAttributes.Private);
    }

    public void AddTypeInitializations (TargetTypeModifierContext context, ClassContext classContext, IEnumerable<Type> concreteMixinTypes)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("classContext", classContext);
      ArgumentUtility.CheckNotNull ("concreteMixinTypes", concreteMixinTypes);

      var staticInitializations =
          Expression.Block (
              typeof (void),
              Expression.Assign (
                  Expression.Field (null, context.ClassContextField),
                  _complexExpressionBuilder.CreateNewClassContextExpression (classContext)),
              Expression.Assign (
                  Expression.Field (null, context.MixinArrayInitializerField),
                  Expression.New (
                      s_mixinArrayInitializerCtor,
                      Expression.Constant (classContext.Type),
                      Expression.Constant (concreteMixinTypes.ToArray()))));

      context.ConcreteTarget.AddTypeInitialization (ctx => staticInitializations);
    }

    public void AddInitializations (TargetTypeModifierContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      context.ConcreteTarget.AddInitialization (ctx => _complexExpressionBuilder.CreateInitializationExpression (ctx.This, context.ExtensionsField));
    }

    public void ImplementIInitializableMixinTarget (TargetTypeModifierContext context)
    {
      throw new NotImplementedException();
    }

    public void ImplementIMixinTarget (TargetTypeModifierContext context)
    {
      throw new NotImplementedException();
    }

    public void ImplementIntroducedInterfaces (TargetTypeModifierContext context)
    {
      throw new NotImplementedException();
    }

    public void ImplementRequiredDuckMethods (TargetTypeModifierContext context)
    {
      throw new NotImplementedException();
    }

    public void AddMixedTypeAttribute (TargetTypeModifierContext context)
    {
      throw new NotImplementedException();
    }

    public void AddDebuggerAttributes (TargetTypeModifierContext context)
    {
      throw new NotImplementedException();
    }

    public void ImplementOverrides (TargetTypeModifierContext context)
    {
      throw new NotImplementedException();
    }

    public void ImplementOverridingMethods (TargetTypeModifierContext context)
    {
      throw new NotImplementedException();
    }

    private MutableFieldInfo AddDebuggerInvisibleField (MutableType targetType, string name, Type type, FieldAttributes attributes)
    {
      var field = targetType.AddField (name, attributes, type);
      var debuggerAttribute = new CustomAttributeDeclaration (s_debuggerBrowsableAttributeCtor, new object[] { DebuggerBrowsableState.Never });
      field.AddCustomAttribute (debuggerAttribute);

      return field;
    }
  }
}