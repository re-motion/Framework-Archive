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
using System.Reflection;
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{

/// <summary> Exposes non-public members of the <see cref="StringUtility"/> type. </summary>
public class StubStringUtility
{
  public static MethodInfo GetParseMethod  (Type type, bool throwIfNotFound)
  {
    return (MethodInfo) PrivateInvoke.InvokeNonPublicStaticMethod (
        typeof (StringUtility), "GetParseMethod", new object[]{type, throwIfNotFound});
  }

  public static MethodInfo GetParseMethodWithFormatProviderFromType (Type type)
  {
    return (MethodInfo) PrivateInvoke.InvokeNonPublicStaticMethod (
        typeof (StringUtility), "GetParseMethodWithFormatProviderFromType", new object[]{type});
  }

  public static MethodInfo GetParseMethodFromType (Type type)
  {
    return (MethodInfo) PrivateInvoke.InvokeNonPublicStaticMethod (
        typeof (StringUtility), "GetParseMethodFromType", new object[]{type});
  }

  public static void ClearCache()
  {
    var cache = (InterlockedCache<Type, MethodInfo>) PrivateInvoke.GetNonPublicStaticField (typeof (StringUtility), "s_parseMethods");
    cache.Clear();
  }
}

}
