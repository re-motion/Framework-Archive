// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections;
using Remotion.Reflection.TypeDiscovery;

namespace Remotion.Implementation
{
  public class ConcreteImplementationResolver
  {
    public static Type ResolveType (string typeNameTemplate)
    {
      return ContextAwareTypeDiscoveryUtility.GetType (GetTypeName (typeNameTemplate), true);
    }

    // TODO: Remove
    public static object InstantiateType (string typeNameTemplate, params IEnumerable[] args)
    {
      return Activator.CreateInstance (ResolveType(typeNameTemplate), args);
    }

    // TODO: Remove
    public static object InstantiateType (Type type, params object[] args)
    {
      return Activator.CreateInstance (type, args);
    }

    public static string GetTypeName (string typeNameTemplate)
    {
      string versioned = typeNameTemplate.Replace ("<version>", FrameworkVersion.Value.ToString ());
      return versioned.Replace ("<publicKeyToken>", GetPublicKeyTokenString ());
    }
    
    private static string GetPublicKeyTokenString ()
    {
      byte[] bytes = typeof (ConcreteImplementationAttribute).Assembly.GetName ().GetPublicKeyToken ();
      return string.Format ("{0:x2}{1:x2}{2:x2}{3:x2}{4:x2}{5:x2}{6:x2}{7:x2}",
          bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], bytes[5], bytes[6], bytes[7]);
    }

  }
}