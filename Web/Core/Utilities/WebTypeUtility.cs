using System;
#if ! NET11
using System.Web.Compilation;
#endif
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
    string typeName = TypeUtility.ParseAbbreviatedTypeName (abbreviatedTypeName);
#if NET11
    return TypeUtility.GetType (typeName, throwOnError, ignoreCase);
#else
    return BuildManager.GetType (typeName, throwOnError, ignoreCase);
#endif
  }


	private WebTypeUtility()
	{
  }
}

}
