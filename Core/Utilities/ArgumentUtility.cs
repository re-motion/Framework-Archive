using System;
using System.Runtime.Serialization;

namespace Rubicon
{

/// <summary>
/// This utility class provides methods for checking arguments.
/// </summary>
public sealed class ArgumentUtility
{
  public static void CheckNotNull (string argumentName, object actualValue)
  {
    if (actualValue == null)
      throw new ArgumentNullException (argumentName);
  }

  public static void CheckNotNullOrEmpty (string argumentName, string actualValue)
  {
    CheckNotNull (argumentName, actualValue);
    if (actualValue.Length == 0)
      throw new ArgumentEmptyException (argumentName);
  }

  public static void CheckNotNullOrEmpty (string argumentName, System.Array array)
  {
    CheckNotNull (argumentName, array);
    if (array.Length == 0)
      throw new ArgumentEmptyException (argumentName);
  }

  public static void CheckNotNullOrEmpty (string argumentName, System.Collections.ICollection collection)
  {
    CheckNotNull (argumentName, collection);
    if (collection.Count == 0)
      throw new ArgumentEmptyException (argumentName);
  }

  public static void ThrowEnumArgumentOutOfRangeException (string argumentName, System.Enum actualValue)
  {
    string message = string.Format ("The value of argument {0} is not a valid value of the type {1}. Actual value was {2}.",
        argumentName, actualValue.GetType(), actualValue);
    throw new ArgumentOutOfRangeException (argumentName, actualValue, message);
  }

  /// <summary>
  /// Returns the value itself if it is not a null reference and of the specified type.
  /// </summary>
  /// <exception cref="ArgumentNullException">The actual value is a null reference.</exception>
  /// <exception cref="ArgumentTypeException">The actual value is an instance of another type (which is not a subclass of the expected type).</exception>
  /// <remarks>This method should be replaced with a templated version and marked as [Obsolete] when C# 
  /// templates are available.</remarks>
  public static object CheckNotNullAndType (string argumentName, object actualValue, Type expectedType)
  {
    if (actualValue == null)
      throw new ArgumentNullException (argumentName);
    return CheckType (argumentName, actualValue, expectedType);
  }

  /// <summary>
  /// Returns the value itself if it is of the specified reference type.
  /// </summary>
  /// <exception cref="ArgumentTypeException">The actual value is an instance of another type (which is not a subclass of the expected type).</exception>
  /// <exception cref="NotSupportedException">The expected type is a value type.</exception>
  /// <remarks> For value types, use <see cref="CheckNotNullAndType"/> instead.
  ///   <para>This method should be replaced with a templated version and marked as [Obsolete] when C# 
  ///   templates are available.</para>
  /// </remarks>
  public static object CheckType (string argumentName, object actualValue, Type expectedType)
  {
    if (expectedType.IsValueType)
      throw new NotSupportedException ("Cannot use ArgumentUtility.CheckType for value types. Use CheckNotNullAndType instead.");

    if (actualValue == null)
      return null;
    
    if (! expectedType.IsAssignableFrom (actualValue.GetType()))
      throw new ArgumentTypeException (argumentName, expectedType, actualValue.GetType());
    return actualValue;
  }

	public static void CheckValidEnumValue (System.Enum enumValue, string argumentName)
	{
		if (enumValue == null)
			throw new ArgumentNullException (argumentName);
		else if (! EnumUtility.IsValidEnumValue (enumValue))
			throw new ArgumentOutOfRangeException (argumentName);
	}

	private ArgumentUtility()
	{
	}
}

/// <summary>
/// This exception is thrown if an argument has an invalid type.
/// </summary>
[Serializable]
public class ArgumentTypeException: ArgumentException
{
  public ArgumentTypeException (string argumentName, Type expectedType, Type actualType)
    : base (FormatMessage (argumentName, expectedType, actualType), argumentName)
  {
  }

  public ArgumentTypeException (SerializationInfo info, StreamingContext context)
    : base (info, context)
  {
  }

  private static string FormatMessage (string argumentName, Type expectedType, Type actualType)
  {
    return string.Format ("Argument {0} has type {2} when type {1} was expected.",
        argumentName, expectedType, actualType);
  }
}

/// <summary>
/// This exception is thrown if an argument is empty although it must have a content.
/// </summary>
[Serializable]
public class ArgumentEmptyException: ArgumentException
{
  public ArgumentEmptyException (string argumentName)
    : base (FormatMessage (argumentName))
  {
  }

  public ArgumentEmptyException (SerializationInfo info, StreamingContext context)
    : base (info, context)
  {
  }

  private static string FormatMessage (string argumentName)
  {
    return string.Format ("Argument {0} is empty.", argumentName);
  }
}

}
