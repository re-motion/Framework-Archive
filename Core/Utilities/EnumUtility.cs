using System;

namespace Rubicon.Utilities
{

/// <summary>
/// This utility class provides methods for dealing with enumeration values.
/// </summary>
public sealed class EnumUtility
{
	/// <summary>
	/// Checks whether the specified value is one of the values that the enumeration type defines.
	/// </summary>
	public static bool IsValidEnumValue (System.Enum enumValue)
	{
		if (enumValue == null)
			throw new ArgumentNullException ("enumValue");
		string stringRepresentation = enumValue.ToString();
		if (stringRepresentation == null || stringRepresentation.Length == 0)
			return false;
		char firstChar = stringRepresentation[0];
		return ! (firstChar == '-' || char.IsDigit (firstChar));
	}

	private EnumUtility()
	{
	}
}

}
