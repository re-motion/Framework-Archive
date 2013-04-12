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
using Remotion.Mixins.Context;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370: Docs.
  public class TargetTypeModifier : ITargetTypeModifier
  {
    private static readonly ConstructorInfo s_debuggerBrowsableAttributeCtor =
        MemberInfoFromExpressionUtility.GetConstructor (() => new DebuggerBrowsableAttribute (DebuggerBrowsableState.Never));

    public TargetTypeModifierContext CreateContext (MutableType targetType)
    {
      throw new NotImplementedException();
    }

    public void ImplementInterfaces (TargetTypeModifierContext ctx, IEnumerable<Type> interfacesToImplement)
    {
      ArgumentUtility.CheckNotNull ("ctx", ctx);
      ArgumentUtility.CheckNotNull ("interfacesToImplement", interfacesToImplement);

      foreach (var ifc in interfacesToImplement)
        ctx.TargetType.AddInterface (ifc);
    }

    public void AddFields (TargetTypeModifierContext ctx, Type nextCallProxyType)
    {
      ArgumentUtility.CheckNotNull ("ctx", ctx);
      ArgumentUtility.CheckNotNull ("nextCallProxyType", nextCallProxyType);

      var tt = ctx.TargetType;
      var privateStatic = FieldAttributes.Private | FieldAttributes.Static;
      ctx.ClassContextField = AddDebuggerInvisibleField (tt, "__classContext", typeof (ClassContext), privateStatic);
      ctx.MixinArrayInitializerField = AddDebuggerInvisibleField (tt, "__mixinArrayInitializer", typeof (MixinArrayInitializer), privateStatic);
      ctx.ExtensionsField = AddDebuggerInvisibleField (tt, "__extensions", typeof (object[]), FieldAttributes.Private);
      ctx.FirstField = AddDebuggerInvisibleField (tt, "__first", nextCallProxyType, FieldAttributes.Private);
    }

    public void AddStaticInitializations (TargetTypeModifierContext ctx)
    {
      throw new NotImplementedException();
    }

    public void ImplementIInitializableMixinTarget (TargetTypeModifierContext ctx)
    {
      throw new NotImplementedException();
    }

    public void ImplementIMixinTarget (TargetTypeModifierContext ctx)
    {
      throw new NotImplementedException();
    }

    public void ImplementIntroducedInterfaces (TargetTypeModifierContext ctx)
    {
      throw new NotImplementedException();
    }

    public void ImplementRequiredDuckMethods (TargetTypeModifierContext ctx)
    {
      throw new NotImplementedException();
    }

    public void AddMixedTypeAttribute (TargetTypeModifierContext ctx)
    {
      throw new NotImplementedException();
    }

    public void AddDebuggerAttributes (TargetTypeModifierContext ctx)
    {
      throw new NotImplementedException();
    }

    public void ImplementOverrides (TargetTypeModifierContext ctx)
    {
      throw new NotImplementedException();
    }

    public void ImplementOverridingMethods (TargetTypeModifierContext ctx)
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