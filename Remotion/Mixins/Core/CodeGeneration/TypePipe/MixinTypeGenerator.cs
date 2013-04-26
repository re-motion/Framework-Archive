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
using System.Runtime.Serialization;
using Remotion.Collections;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.Utilities;
using Remotion.TypePipe.Dlr.Ast;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.Implementation;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370
  public class MixinTypeGenerator
  {
    private static readonly MethodInfo s_getObjectDataForGeneratedTypesMethod = MemberInfoFromExpressionUtility.GetMethod (
        () => MixinSerializationHelper.GetObjectDataForGeneratedTypes (null, new StreamingContext(), null, null, false));

    private readonly ConcreteMixinTypeIdentifier _identifier;
    private readonly MutableType _type;
    private readonly IAttributeGenerator _attributeGenerator;

    private Expression _identifierField;

    public MixinTypeGenerator (ConcreteMixinTypeIdentifier identifier, MutableType type, IAttributeGenerator attributeGenerator)
    {
      ArgumentUtility.CheckNotNull ("identifier", identifier);
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("attributeGenerator", attributeGenerator);

      _identifier = identifier;
      _type = type;
      _attributeGenerator = attributeGenerator;
    }

    public void AddInterfaces ()
    {
      _type.AddInterface (typeof (ISerializable));
      _type.AddInterface (typeof (IGeneratedMixinType));
    }

    public void AddFields ()
    {
      var field = _type.AddField ("__identifier", FieldAttributes.Public | FieldAttributes.Static, typeof (ConcreteMixinTypeIdentifier));
      _identifierField = Expression.Field (null, field);
    }

    public void AddTypeInitializer ()
    {
      var serializer = new ExpressionConcreteMixinTypeIdentifierSerializer();
      _identifier.Serialize (serializer);
      var newExpression = serializer.CreateNewExpression();

      _type.AddTypeInitializer (ctx => Expression.Assign (_identifierField, newExpression));
    }

    public void ImplementGetObjectData ()
    {
      SerializationImplementer2.ImplementGetObjectDataByDelegation (
          _type,
          (ctx, baseIsISerializable) =>
          Expression.Call (
              null,
              s_getObjectDataForGeneratedTypesMethod,
              ctx.Parameters[0],
              ctx.Parameters[1],
              ctx.This,
              _identifierField,
              Expression.Constant (!baseIsISerializable)));
    }

    public void AddMixinTypeAttribute ()
    {
      _attributeGenerator.AddConcreteMixinTypeAttribute (_type, _identifier);
    }

    public void AddDebuggerAttributes ()
    {
      var debuggerDisplayString = "Derived mixin: " + _identifier;
      _attributeGenerator.AddDebuggerDisplayAttribute (_type, debuggerDisplayString, null);
    }

    public OverrideInterface GenerateOverrides (ITypeAssemblyContext context)
    {
      var overrideInterfaceGenerator = OverrideInterfaceGenerator2.CreateNestedGenerator (context, _type, "IOverriddenMethods");

      var targetReference = GetTargetReference();
      foreach (var method in _identifier.Overridden)
      {
        var methodOverride = _type.GetOrAddOverride (method);
        var methodToCall = overrideInterfaceGenerator.AddOverriddenMethod (method);

        AddCallToOverrider (methodOverride, targetReference, methodToCall);
      }

      return new OverrideInterface (overrideInterfaceGenerator.Type, overrideInterfaceGenerator.GetInterfaceMethodsForOverriddenMethods());
    }

    private Expression GetTargetReference ()
    {
      var targetProperty = MixinReflector.GetTargetProperty (_type.BaseType);
      if (targetProperty == null)
      {
        throw new NotSupportedException (
            "The code generator does not support mixins with overridden methods or non-public overriders if the mixin doesn't derive from the "
            + "generic Mixin base classes.");
      }

      return Expression.Property (new ThisExpression (_type), targetProperty);
    }

    private void AddCallToOverrider (MutableMethodInfo methodOverride, Expression targetReference, MethodInfo targetMethod)
    {
      var castedTargetReference = Expression.Convert (targetReference, targetMethod.DeclaringType);
      methodOverride.SetBody (ctx => ctx.DelegateTo (castedTargetReference, targetMethod));
    }

    public Dictionary<MethodInfo, MethodInfo> GenerateMethodWrappers ()
    {
      var wrappers = from m in _identifier.Overriders
                     where !m.IsPublic
                     select new { Method = m, Wrapper = GetPublicMethodWrapper (m) };
      return wrappers.ToDictionary (pair => pair.Method, pair => pair.Wrapper);
    }


    private readonly Cache<MethodInfo, MethodInfo> _publicMethodWrappers = new Cache<MethodInfo, MethodInfo>();
    private MethodInfo GetPublicMethodWrapper (MethodInfo methodToBeWrapped)
    {
      ArgumentUtility.CheckNotNull ("methodToBeWrapped", methodToBeWrapped);

      return _publicMethodWrappers.GetOrCreateValue (methodToBeWrapped, CreatePublicMethodWrapper);
    }

    private MethodInfo CreatePublicMethodWrapper (MethodInfo methodToBeWrapped)
    {
      var name = "__wrap__" + methodToBeWrapped.Name;
      var attributes = MethodAttributes.Public | MethodAttributes.HideBySig;
      var md = MethodDeclaration.CreateEquivalent (methodToBeWrapped);
      var wrapper = _type.AddMethod (name, attributes, md, ctx => ctx.DelegateTo (ctx.This, methodToBeWrapped));

      _attributeGenerator.AddGeneratedMethodWrapperAttribute (wrapper, methodToBeWrapped);

      return wrapper;
    }
  }
}