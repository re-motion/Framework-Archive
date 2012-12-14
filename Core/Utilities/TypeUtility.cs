using System;

namespace Rubicon.Utilities
{

/// <summary>
/// Utility methods for handling types.
/// </summary>
public sealed class TypeUtility
{
  /// <summary>
  ///   Converts abbreviated qualified type names into standard qualified type names.
  /// </summary>
  /// <remarks>
  ///   Abbreviated type names use the format <c>assemblyname::subnamespace::type</c>. For instance, the
  ///   abbreviated type name <c>"Rubicon.Web::Utilities.ControlHelper"</c> would result in the standard
  ///   type name <c>"Rubicon.Web.Utilities.ControlHelper, Rubicon.Web"</c>.
  /// </remarks>
  /// <param name="abbreviatedTypeName"> A standard or abbreviated type name. </param>
  /// <returns> A standard type name as expected by <see cref="Type.GetType"/>. </returns>
  public static string ParseAbbreviatedTypeName (string abbreviatedTypeName)
  {
    if (abbreviatedTypeName == null)
      return null;

    int posDoubleColon = abbreviatedTypeName.IndexOf ("::");
    if (posDoubleColon < 0)
      return abbreviatedTypeName;

    string assembly = abbreviatedTypeName.Substring (0, posDoubleColon);
    string type = abbreviatedTypeName.Substring (posDoubleColon + 2);

    return assembly + "." + type + ", " + assembly;
  }

  /// <summary>
  ///   Loads a type, optionally using an abbreviated type name as defined in <see cref="ParseAbbreviatedTypeName"/>.
  /// </summary>
  public static Type GetType (string abbreviatedTypeName)
  {
    return Type.GetType (ParseAbbreviatedTypeName (abbreviatedTypeName));
  }

  /// <summary>
  ///   Loads a type, optionally using an abbreviated type name as defined in <see cref="ParseAbbreviatedTypeName"/>.
  /// </summary>
  public static Type GetType (string abbreviatedTypeName, bool throwOnError)
  {
    return Type.GetType (ParseAbbreviatedTypeName (abbreviatedTypeName), throwOnError);
  }

  /// <summary>
  ///   Loads a type, optionally using an abbreviated type name as defined in <see cref="ParseAbbreviatedTypeName"/>.
  /// </summary>
  public static Type GetType (string abbreviatedTypeName, bool throwOnError, bool ignoreCase)
  {
    return Type.GetType (ParseAbbreviatedTypeName (abbreviatedTypeName), throwOnError, ignoreCase);
  }

	private TypeUtility()
	{
	}
}

}
