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
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370
  public class NextCallProxy : INextCallProxy
  {
    private readonly Type _type;
    private readonly ConstructorInfo _constructor;
    private readonly Dictionary<MethodDefinition, MethodInfo> _overriddenMethodToImplementationMap;

    public NextCallProxy (Type type, ConstructorInfo constructor, Dictionary<MethodDefinition, MethodInfo> overriddenMethodToImplementationMap)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("constructor", constructor);
      ArgumentUtility.CheckNotNull ("overriddenMethodToImplementationMap", overriddenMethodToImplementationMap);

      _type = type;
      _constructor = constructor;
      _overriddenMethodToImplementationMap = overriddenMethodToImplementationMap;
    }

    public Type Type
    {
      get { return _type; }
    }

    public ConstructorInfo Constructor
    {
      get { return _constructor; }
    }

    public MethodInfo GetProxyMethodForOverriddenMethod (MethodDefinition method)
    {
      Assertion.IsTrue (_overriddenMethodToImplementationMap.ContainsKey (method),
                        "The method " + method.Name + " must be registered with the NextCallProxyGenerator.");
      return _overriddenMethodToImplementationMap[method];
    }
  }
}