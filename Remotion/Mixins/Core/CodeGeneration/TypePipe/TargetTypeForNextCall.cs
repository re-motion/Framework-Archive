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
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370
  public class TargetTypeForNextCall : ITargetTypeForNextCall
  {
    private readonly MutableType _concreteTarget;
    private readonly Dictionary<MethodInfo, MethodInfo> _baseCallMethods = new Dictionary<MethodInfo, MethodInfo>();

    public TargetTypeForNextCall (MutableType concreteTarget)
    {
      ArgumentUtility.CheckNotNull ("concreteTarget", concreteTarget);

      _concreteTarget = concreteTarget;
    }

    public Expression ExtensionsField
    {
      get
      {
        var field = _concreteTarget.AddedFields.Single (f => f.Name == "__extensions");
        return Expression.Field (new ThisExpression (_concreteTarget), field);
      }
    }

    public MethodInfo GetBaseCallMethod (MethodInfo overriddenMethod)
    {
      ArgumentUtility.CheckNotNull ("overriddenMethod", overriddenMethod);
      Assertion.IsNotNull (overriddenMethod.DeclaringType);

      if (!overriddenMethod.DeclaringType.IsAssignableFrom (_concreteTarget.BaseType))
      {
        string message = string.Format (
            "Cannot create base call method for a method defined on a different type than the base type: {0}.{1}.",
            overriddenMethod.DeclaringType.FullName,
            overriddenMethod.Name);
        throw new ArgumentException (message, "overriddenMethod");
      }

      if (!_baseCallMethods.ContainsKey (overriddenMethod))
        _baseCallMethods.Add (overriddenMethod, ImplementBaseCallMethod (overriddenMethod));

      return _baseCallMethods[overriddenMethod];
    }

    private MethodInfo ImplementBaseCallMethod (MethodInfo method)
    {
      Assertion.IsTrue (Utilities.ReflectionUtility.IsPublicOrProtected (method));

      var attributes = MethodAttributes.Public | MethodAttributes.HideBySig;
      var name = "__base__" + method.Name;
      var md = MethodDeclaration.CreateEquivalent (method);

      return _concreteTarget.AddMethod (name, attributes, md, ctx =>
      {
        var baseMethod = ctx.BaseMethod;
        if (baseMethod.IsGenericMethod)
          baseMethod = baseMethod.MakeTypePipeGenericMethod (ctx.GenericParameters.ToArray ());

        return ctx.CallBase (baseMethod);
      });
    }

    //public MethodInfo GetPublicMethodWrapper (MethodDefinition methodToBeWrapped)
    //{
    //  ArgumentUtility.CheckNotNull ("methodToBeWrapped", methodToBeWrapped);
    //  if (methodToBeWrapped.DeclaringClass != _targetClassDefinition)
    //    throw new ArgumentException ("Only methods from class " + _targetClassDefinition.FullName + " can be wrapped.");

    //  return Emitter.GetPublicMethodWrapper (methodToBeWrapped.MethodInfo);
    //}
  }
}