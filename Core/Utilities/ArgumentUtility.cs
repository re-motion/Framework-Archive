using System;
using System.Runtime.Serialization;
using System.Collections;

namespace Rubicon.Utilities
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

  public static void CheckNotNullOrEmpty (string argumentName, ICollection collection)
  {
    CheckNotNull (argumentName, collection);
    if (collection.Count == 0)
      throw new ArgumentEmptyException (argumentName);
  }

  public static void CheckNotNullOrItemsNull (string argumentName, System.Array array)
  {
    CheckNotNull (argumentName, array);

    for (int i = 0; i < array.LongLength; ++i)
    {
      if (array.GetValue (i) == null)
        throw new ArgumentItemNullException (argumentName, i);
    }
  }

  public static void CheckNotNullOrItemsNull (string argumentName, ICollection collection)
  {
    CheckNotNull (argumentName, collection);

    int i = 0;
    foreach (object item in collection)
    {
      if (item == null)
        throw new ArgumentItemNullException (argumentName, i);
      ++i;
    }
  }
  
  public static void CheckNotNullOrEmptyOrItemsNull (string argumentName, System.Array array)
  {
    CheckNotNullOrItemsNull (argumentName, array);
    if (array.Length == 0)
      throw new ArgumentEmptyException (argumentName);
  }

  [Obsolete ("Causes StackOverflow")]
  public static void CheckNotNullOrEmptyOrItemsNull (string argumentName, ICollection collection)
  {
    CheckNotNullOrItemsNull (argumentName, collection);
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

  public static void CheckItemsType (string argumentName, ICollection collection, Type itemType)
  {
    if (collection == null)
      return;

    int index = 0;
    foreach (object item in collection)
    {
      if (item != null && ! itemType.IsAssignableFrom (item.GetType()))
        throw new ArgumentItemTypeException (argumentName, index, itemType, item.GetType());
      ++ index;
    }
  }

  public static void CheckItemsNotNullAndType (string argumentName, ICollection collection, Type itemType)
  {
    if (collection == null)
      return;

    int index = 0;
    foreach (object item in collection)
    {
      if (item == null)
        throw new ArgumentItemNullException (argumentName, index);
      if (! itemType.IsAssignableFrom (item.GetType()))
        throw new ArgumentItemTypeException (argumentName, index, itemType, item.GetType());
      ++ index;
    }
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

/// <summary>
/// This exception is thrown if a list argument contains a null reference.
/// </summary>
[Serializable]
public class ArgumentItemNullException: ArgumentException
{
  public ArgumentItemNullException (string argumentName, int index)
    : base (FormatMessage (argumentName, index))
  {
  }

  public ArgumentItemNullException (SerializationInfo info, StreamingContext context)
    : base (info, context)
  {
  }

  private static string FormatMessage (string argumentName, int index)
  {
    return string.Format ("Item {0} of argument {1} is null.", index, argumentName);
  }
}


/// <summary>
/// This exception is thrown if a list argument contains an item of the wrong type.
/// </summary>
[Serializable]
public class ArgumentItemTypeException: ArgumentException
{
  public ArgumentItemTypeException (string argumentName, int index, Type expectedType, Type actualType)
    : base (FormatMessage (argumentName, index, expectedType, actualType))
  {
  }

  public ArgumentItemTypeException (SerializationInfo info, StreamingContext context)
    : base (info, context)
  {
  }

  private static string FormatMessage (string argumentName, int index, Type expectedType, Type actualType)
  {
    return string.Format ("Item {0} of argument {1} has the type {2} instead of {3}.", index, argumentName, actualType.FullName, expectedType.FullName);
  }
}

}
