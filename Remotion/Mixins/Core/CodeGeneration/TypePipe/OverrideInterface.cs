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
using Remotion.TypePipe.CodeGeneration;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370
  public class OverrideInterface
  {
    private readonly Type _type;
    private readonly Dictionary<MethodInfo, MethodInfo> _interfaceMethodsByOverriddenMethods;

    public OverrideInterface (Type type, Dictionary<MethodInfo, MethodInfo> interfaceMethodsByOverriddenMethods)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("interfaceMethodsByOverriddenMethods", interfaceMethodsByOverriddenMethods);

      _type = type;
      _interfaceMethodsByOverriddenMethods = interfaceMethodsByOverriddenMethods;
    }

    public Type Type
    {
      get { return _type; }
    }

    public Dictionary<MethodInfo, MethodInfo> InterfaceMethodsByOverriddenMethods
    {
      get { return _interfaceMethodsByOverriddenMethods; }
    }

    public OverrideInterface SubstituteForRealReflectionObjects (GeneratedTypeContext context)
    {
      var type = context.GetGeneratedType ((MutableType) _type);
      var mapping = _interfaceMethodsByOverriddenMethods.ToDictionary (
          pair => pair.Key, pair => context.GetGeneratedMethod ((MutableMethodInfo) pair.Value));

      return new OverrideInterface (type, mapping);
    }
  }
}