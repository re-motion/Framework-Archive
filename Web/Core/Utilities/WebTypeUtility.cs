using System;
using System.Collections;
#if ! NET11
using System.Web.Compilation;
#endif
using System.Reflection;
using Rubicon.Utilities;

namespace Rubicon.Web.Utilities
{

/// <summary> Utility methods for handling types in web projects. </summary>
public sealed class WebTypeUtility
{
  /// <summary>
  ///   Loads a type, optionally using an abbreviated type name as defined in 
  ///   <see cref="TypeUtility.ParseAbbreviatedTypeName"/>.
  /// </summary>
  public static Type GetType (string abbreviatedTypeName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("abbreviatedTypeName", abbreviatedTypeName);
    string typeName = TypeUtility.ParseAbbreviatedTypeName (abbreviatedTypeName);
#if NET11
    return TypeUtility.GetType (typeName);
#else
    return BuildManager.GetType (typeName, true);
#endif
  }

  /// <summary>
  ///   Loads a type, optionally using an abbreviated type name as defined in 
  ///   <see cref="TypeUtility.ParseAbbreviatedTypeName"/>.
  /// </summary>
  public static Type GetType (string abbreviatedTypeName, bool throwOnError)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("abbreviatedTypeName", abbreviatedTypeName);
    string typeName = TypeUtility.ParseAbbreviatedTypeName (abbreviatedTypeName);
#if NET11
    return TypeUtility.GetType (typeName, throwOnError);
#else
    return BuildManager.GetType (typeName, throwOnError);
#endif
    }

  /// <summary>
  ///   Loads a type, optionally using an abbreviated type name as defined in 
  ///   <see cref="TypeUtility.ParseAbbreviatedTypeName"/>.
  /// </summary>
  public static Type GetType (string abbreviatedTypeName, bool throwOnError, bool ignoreCase)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("abbreviatedTypeName", abbreviatedTypeName);
    string typeName = TypeUtility.ParseAbbreviatedTypeName (abbreviatedTypeName);
#if NET11
    return TypeUtility.GetType (typeName, throwOnError, ignoreCase);
#else
    return BuildManager.GetType (typeName, throwOnError, ignoreCase);
#endif
  }

  public static string GetQualifiedName (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);
#if ! NET11
    if (IsCompiledType (type))
      return type.FullName;
#endif
    return type.FullName + "," + type.Assembly.GetName().Name;
  }

#if ! NET11
  public static bool IsCompiledType (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);
    IList codeAssemblies = BuildManager.CodeAssemblies;
    if (codeAssemblies == null)
      return false;

    foreach (Assembly assembly in codeAssemblies)
    {
      if (assembly == type.Assembly)
        return true;
    }
    return false;
  }
#endif

	private WebTypeUtility()
	{
  }
}

}
