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
using System.Reflection.Emit;
using Remotion.TypePipe.Implementation;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370
  public class OverrideInterfaceGenerator2
  {
    public static OverrideInterfaceGenerator2 CreateNestedGenerator (ITypeAssemblyContext context, MutableType outerType, string typeName)
    {
      ArgumentUtility.CheckNotNull ("outerType", outerType);
      ArgumentUtility.CheckNotNullOrEmpty ("typeName", typeName);

      var interfaceType = context.CreateInterface (outerType.Name + "." + typeName, outerType.Namespace);
      return new OverrideInterfaceGenerator2 (interfaceType);
    }

    private readonly IAttributeGenerator _attributeGenerator = new AttributeGenerator();
    private readonly Dictionary<MethodInfo, MethodInfo> _interfaceMethods = new Dictionary<MethodInfo, MethodInfo> ();
    private readonly MutableType _interfaceType;

    private OverrideInterfaceGenerator2 (MutableType interfaceType)
    {
      ArgumentUtility.CheckNotNull ("interfaceType", interfaceType);

      _interfaceType = interfaceType;
    }

    public MethodInfo AddOverriddenMethod (MethodInfo overriddenMethod)
    {
      ArgumentUtility.CheckNotNull ("overriddenMethod", overriddenMethod);

      var name = overriddenMethod.Name;
      var attributes = MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual;
      var md = MethodDeclaration.CreateEquivalent (overriddenMethod);
      var method = _interfaceType.AddMethod (name, attributes, md, bodyProvider: null);

      _attributeGenerator.AddOverrideInterfaceMappingAttribute (method, overriddenMethod);

      _interfaceMethods.Add (overriddenMethod, method);

      return method;
    }

    public Dictionary<MethodInfo, MethodInfo> GetInterfaceMethodsForOverriddenMethods ()
    {
      return _interfaceMethods;
    }

    public Type Type
    {
      get { return _interfaceType; }
    }
  }
}