using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Rubicon.Utilities
{

  /// <summary>
  /// This utility class provides methods for checking arguments.
  /// </summary>
  public static class ArgumentUtility
  {
    public static T CheckNotNull<T> (string argumentName, T actualValue)
    {
      if (actualValue == null)
        throw new ArgumentNullException (argumentName);

      return actualValue;
    }

    public static string CheckNotNullOrEmpty (string argumentName, string actualValue)
    {
      CheckNotNull (argumentName, actualValue);
      if (actualValue.Length == 0)
        throw new ArgumentEmptyException (argumentName);

      return actualValue;
    }

    public static T CheckNotNullOrEmpty<T> (string argumentName, T collection)
      where T: ICollection
    {
      CheckNotNull (argumentName, collection);
      if (collection.Count == 0)
        throw new ArgumentEmptyException (argumentName);

      return collection;
    }

    public static T CheckNotNullOrItemsNull<T> (string argumentName, T collection)
       where T: ICollection
     {
      CheckNotNull (argumentName, collection);

      int i = 0;
      foreach (object item in collection)
      {
        if (item == null)
          throw new ArgumentItemNullException (argumentName, i);
        ++i;
      }

      return collection;
    }

    public static T CheckNotNullOrEmptyOrItemsNull<T> (string argumentName, T collection)
       where T : ICollection
    {
      CheckNotNullOrItemsNull (argumentName, collection);
      if (collection.Count == 0)
        throw new ArgumentEmptyException (argumentName);
      
      return collection;
    }

    public static void ThrowEnumArgumentOutOfRangeException (string argumentName, System.Enum actualValue)
    {
      string message = string.Format ("The value of argument {0} is not a valid value of the type {1}. Actual value was {2}.",
          argumentName, actualValue.GetType (), actualValue);
      throw new ArgumentOutOfRangeException (argumentName, actualValue, message);
    }

		[Obsolete (@"Use CheckNotNullAndType<ExpectedType> instead. Example: "
							 + @"Dog d = (Dog) ArgumentUtility.CheckNotNullAndType (""animal"", animal, typeof(Dog));  "
							 + @"-->  Dog d = ArgumentUtility.CheckNotNullAndType<Dog> (""animal"", animal);" )]
		public static object CheckNotNullAndType (string argumentName, object actualValue, Type expectedType)
		{
			if (actualValue == null)
				throw new ArgumentNullException (argumentName);
			return CheckType (argumentName, actualValue, expectedType);
		}

		/// <summary>Returns the value itself if it is not <see langword="null"/> and of the specified value type.</summary>
		/// <typeparam name="TExpected"> The type that <paramref name="actualValue"/> must have. </typeparam>
		/// <exception cref="ArgumentNullException">The <paramref name="actualValue"/> is a <see langword="null"/>.</exception>
		/// <exception cref="ArgumentTypeException">The <paramref name="actualValue"/> is an instance of another type (which is not a subclass of <typeparamref name="TExpected"/>).</exception>
		public static TExpected CheckNotNullAndType<TExpected> (string argumentName, object actualValue)
			where TExpected: class
		{
			if (actualValue == null)
				throw new ArgumentNullException (argumentName);
			return CheckType<TExpected> (argumentName, actualValue);
		}

		/// <summary>Returns the value itself if it is not <see langword="null"/> and of the specified value type.</summary>
		/// <typeparam name="TExpected"> The type that <paramref name="actualValue"/> must have. </typeparam>
		/// <exception cref="ArgumentNullException">The <paramref name="actualValue"/> is a <see langword="null"/>.</exception>
		/// <exception cref="ArgumentTypeException">The <paramref name="actualValue"/> is an instance of another type.</exception>
		public static TExpected CheckNotNullAndValueType<TExpected> (string argumentName, object actualValue)
			where TExpected: struct
		{
			if (actualValue == null)
				throw new ArgumentNullException (argumentName);

      if (! (actualValue is TExpected))
				throw new ArgumentTypeException (argumentName, typeof (TExpected), actualValue.GetType ());
			return (TExpected) actualValue;
    }

		[Obsolete (@"Use CheckType<ExpectedType> instead. Example: "
							 + @"Dog d = (Dog) ArgumentUtility.CheckType (""animal"", animal, typeof(Dog));  "
							 + @"-->  Dog d = ArgumentUtility.CheckType<Dog> (""animal"", animal);" )]
		public static object CheckType (string argumentName, object actualValue, Type expectedType)
		{
			if (expectedType.IsValueType)
				throw new NotSupportedException ("Cannot use ArgumentUtility.CheckType for value types. Use CheckNotNullAndType instead.");

			if (actualValue == null)
				return null;

			if (!expectedType.IsInstanceOfType (actualValue))
				throw new ArgumentTypeException (argumentName, expectedType, actualValue.GetType ());
			return actualValue;
		}

		/// <summary>Returns the value itself if it is of the specified reference type.</summary>
		/// <typeparam name="TExpected"> The type that <paramref name="actualValue"/> must have. </typeparam>
    /// <exception cref="ArgumentTypeException">The <paramref name="actualValue"/> is an instance of another type (which is not a subclass of the <paramref name="expectedType"/>).</exception>
		/// <remarks>For value types, use <see cref="CheckValueType{T}"/> instead.</remarks>
		public static TExpected CheckType<TExpected> (string argumentName, object actualValue)
			where TExpected: class
		{
			if (actualValue == null)
				return default (TExpected); 

			TExpected result = actualValue as TExpected;
			if (result == null)
				throw new ArgumentTypeException (argumentName, typeof (TExpected), actualValue.GetType ());
			return result;
		}

		/// <summary>Returns the value itself if it is of the specified value type.</summary>
		/// <typeparam name="TExpected"> The type that <paramref name="actualValue"/> must have. </typeparam>
		/// <exception cref="ArgumentTypeException">The <paramref name="actualValue"/> is an instance of another type.</exception>
		/// <remarks>For reference types, use <see cref="CheckType{T}"/> instead.</remarks>
		public static TExpected? CheckValueType<TExpected> (string argumentName, object actualValue)
			where TExpected: struct
		{
			if (actualValue == null)
				return default (TExpected?); 

      if (! (actualValue is TExpected))
				throw new ArgumentTypeException (argumentName, typeof (TExpected), actualValue.GetType ());
			return (TExpected?) actualValue;
		}

		/// <summary>Checks whether <paramref name="actualType"/> is not <see langword="null"/> and can be assigned to <paramref name="expectedType"/>.</summary>
    /// <exception cref="ArgumentNullException">The <paramref name="actualType"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentTypeException">The <paramref name="actualType"/> cannot be assigned to <paramref name="expectedType"/>.</exception>
    public static Type CheckNotNullAndTypeIsAssignableFrom (string argumentName, Type actualType, Type expectedType)
    {
      if (actualType == null)
        throw new ArgumentNullException (argumentName);
      return CheckTypeIsAssignableFrom (argumentName, actualType, expectedType);
    }

    /// <summary>Checks whether <paramref name="actualType"/> can be assigned to <paramref name="expectedType"/>.</summary>
    /// <exception cref="ArgumentTypeException">The <paramref name="actualType"/> cannot be assigned to <paramref name="expectedType"/>.</exception>
    public static Type CheckTypeIsAssignableFrom (string argumentName, Type actualType, Type expectedType)
    {
      ArgumentUtility.CheckNotNull ("expectedType", expectedType);
      if (actualType != null)
      {
        if (!expectedType.IsAssignableFrom (actualType))
        {
          throw new ArgumentTypeException (
              string.Format ("Argument {0} is a {2}, which is cannot be assigned to type {1}.", argumentName, expectedType, actualType));
        }
      }

      return actualType;
    }

    /// <summary>Checks whether all items in <paramref name="collection"/> are of type <paramref name="itemType"/> or a null reference.</summary>
    /// <exception cref="ArgumentItemTypeException"> If at least one element is not of the specified type or a derived type. </exception>
    public static T CheckItemsType<T> (string argumentName, T collection, Type itemType)
        where T: ICollection
    {
      if (collection != null)
      {
        int index = 0;
        foreach (object item in collection)
        {
          if (item != null && !itemType.IsInstanceOfType (item))
            throw new ArgumentItemTypeException (argumentName, index, itemType, item.GetType());
          ++index;
        }
      }

      return collection;
    }

    /// <summary>Checks whether all items in <paramref name="collection"/> are of type <paramref name="itemType"/> and not null references.</summary>
    /// <exception cref="ArgumentItemTypeException"> If at least one element is not of the specified type or a derived type. </exception>
    /// <exception cref="ArgumentItemNullException"> If at least one element is a null reference. </exception>
    public static T CheckItemsNotNullAndType<T> (string argumentName, T collection, Type itemType)
        where T: ICollection
    {
      if (collection != null)
      {
        int index = 0;
        foreach (object item in collection)
        {
          if (item == null)
            throw new ArgumentItemNullException (argumentName, index);
          if (!itemType.IsInstanceOfType (item))
            throw new ArgumentItemTypeException (argumentName, index, itemType, item.GetType());
          ++index;
        }
      }

      return collection;
    }

    /// <summary>Checks whether <paramref name="enumValue"/> is defined within its enumeration type.</summary>
    /// <exception cref="ArgumentNullException"> If <paramref name="enumValue"/> is a null reference. </exception>
    /// <exception cref="ArgumentOutOfRangeException"> If <paramref name="enumValue"/> has a numeric value that is not completely defined within its 
    /// enumeration type. For flag types, every bit must correspond to at least one enumeration value. </exception>
    public static Enum CheckValidEnumValue (string argumentName, Enum enumValue)
    {
      if (enumValue == null)
        throw new ArgumentNullException (argumentName);

      if (! EnumUtility.IsValidEnumValue (enumValue))
        throw new ArgumentOutOfRangeException (argumentName);

      return enumValue;
    }

    /// <summary>Checks whether <paramref name="enumValue"/> is of the enumeration type <typeparamref name="TEnum"/> and defined within this type.</summary>
    /// <remarks>
    /// When successful, the value is returned as a <c>Nullable</c> of the specified type for direct assignment. 
    /// </remarks>
    /// <exception cref="ArgumentTypeException"> If <paramref name="enumValue"/> is not of the specified type. </exception>
    /// <exception cref="ArgumentOutOfRangeException"> If <paramref name="enumValue"/> has a numeric value that is not completely defined within its 
    /// enumeration type. For flag types, every bit must correspond to at least one enumeration value. </exception>
    public static TEnum? CheckValidEnumValueAndType <TEnum> (string argumentName, object enumValue)
      where TEnum: struct
    {
      if (enumValue == null)
        return default (TEnum?);

      if (! (enumValue is TEnum))
        throw new ArgumentTypeException (argumentName, typeof(TEnum), enumValue.GetType());

      if (! EnumUtility.IsValidEnumValue (enumValue))
        throw new ArgumentOutOfRangeException (argumentName);

      return (TEnum?) enumValue;
    }

    /// <summary>Checks whether <paramref name="enumValue"/> is of the enumeration type <typeparamref name="TEnum"/>, is defined within this 
    /// type, and is not a null reference.</summary>
    /// <remarks>
    /// When successful, the value is returned as the specified type for direct assignment. 
    /// </remarks>
    /// <exception cref="ArgumentNullException"> If <paramref name="enumValue"/> is a null reference. </exception>
    /// <exception cref="ArgumentTypeException"> If <paramref name="enumValue"/> is not of the specified type. </exception>
    /// <exception cref="ArgumentOutOfRangeException"> If <paramref name="enumValue"/> has a numeric value that is not completely defined within its 
    /// enumeration type. For flag types, every bit must correspond to at least one enumeration value. </exception>
    public static TEnum CheckValidEnumValueAndTypeAndNotNull <TEnum> (string argumentName, object enumValue)
      where TEnum: struct
    {
      if (enumValue == null)
        throw new ArgumentNullException (argumentName);

      if (! (enumValue is TEnum))
        throw new ArgumentTypeException (argumentName, typeof(TEnum), enumValue.GetType());

      if (!EnumUtility.IsValidEnumValue (enumValue))
        throw new ArgumentOutOfRangeException (argumentName);

      return (TEnum) enumValue;
    }

    [System.ComponentModel.EditorBrowsable (System.ComponentModel.EditorBrowsableState.Never)]
    [Obsolete ("Use CheckValidEnumValue (string argumentName, Enum enumValue) instead.")]
    public static void CheckValidEnumValue (Enum enumValue, string argumentName)
    {
      CheckValidEnumValue (argumentName, enumValue);
    }
  }

  /// <summary>
  /// This exception is thrown if an argument has an invalid type.
  /// </summary>
  [Serializable]
  public class ArgumentTypeException : ArgumentException
  {
    public ArgumentTypeException (string argumentName, Type expectedType, Type actualType)
      : base (FormatMessage (argumentName, expectedType, actualType), argumentName)
    {
    }

    public ArgumentTypeException (string argumentName, Type actualType)
      : base (FormatMessage (argumentName, null, actualType), argumentName)
    {
    }

    public ArgumentTypeException (SerializationInfo info, StreamingContext context)
      : base (info, context)
    {
    }

    public ArgumentTypeException (string message)
      : base (message)
    {
    }

    private static string FormatMessage (string argumentName, Type expectedType, Type actualType)
    {
      if (expectedType == null)
      {
        return string.Format ("Argument {0} has unexpected type {1}", argumentName, actualType);
      }
      else
      {
        return string.Format ("Argument {0} has type {2} when type {1} was expected.",
            argumentName, expectedType, actualType);
      }
    }
  }

  /// <summary>
  /// This exception is thrown if an argument is empty although it must have a content.
  /// </summary>
  [Serializable]
  public class ArgumentEmptyException : ArgumentException
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
  public class ArgumentItemNullException : ArgumentException
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
  public class ArgumentItemTypeException : ArgumentException
  {
    public ArgumentItemTypeException (string argumentName, int index, Type expectedType, Type actualType)
      : base ( FormatMessage (argumentName, index, expectedType, actualType))
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
