using System;
using System.Collections;
using System.Reflection;

using Rubicon.Utilities;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Core.UnitTests.Utilities
{

/// <summary> Exposes non-public members of the <see cref="StringUtility"/> type. </summary>
public class StringUtilityMock
{
  public static void AddParseMethodToCache (Type key, MethodInfo parseMethod)
  {
    PrivateInvoke.InvokeNonPublicStaticMethod (
        typeof (StringUtility), "AddParseMethodToCache", new object[]{key, parseMethod});
  }

  public static object ParseScalarValue (Type type, string value, IFormatProvider format)
  {
    return PrivateInvoke.InvokeNonPublicStaticMethod (
        typeof (StringUtility), "ParseScalarValue", new object[]{type, value, format});
  }

  public static object ParseArrayValue (Type type, string value, IFormatProvider format)
  {
    return PrivateInvoke.InvokeNonPublicStaticMethod (
        typeof (StringUtility), "ParseArrayValue", new object[]{type, value, format});
  }

  public static MethodInfo GetParseMethodFromCache (Type key)
  {
    return (MethodInfo) PrivateInvoke.InvokeNonPublicStaticMethod (
        typeof (StringUtility), "GetParseMethodFromCache", new object[]{key});
  }

  public static bool HasTypeInCache (Type type)
  {
    return (bool) PrivateInvoke.InvokeNonPublicStaticMethod (
        typeof (StringUtility), "HasTypeInCache", new object[]{type});
  }

  public static void ClearCache()
  {
    Hashtable cache = (Hashtable) PrivateInvoke.GetNonPublicStaticField (typeof (StringUtility), "s_parseMethods");
    cache.Clear();
  }
}

}
