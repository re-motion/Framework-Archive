using System;
using System.Runtime.Serialization;
using System.Data.SqlTypes;

namespace Rubicon.Data.NullableValueTypes
{

/// <summary>
/// Represents a 32-bit signed integer that can be <c>Null</c>.
/// </summary>
[Serializable]
public struct NaInt32: INullable, IComparable, ISerializable
{
  // member fields

  private Int32 _value;
  private bool _isNull;

  // construction and disposal

  /// <summary>
  /// Initializes a new instance of the <see cref="NaInt32"/> structure using the supplied integer value.
  /// </summary>
  /// <param name="value">The integer to be converted.</param>
  public NaInt32 (int value)
  {
    _value = value;
    _isNull = false;
  }

  private NaInt32 (bool isNull)
  {
    _value = 0;
    _isNull = isNull;
  }

  // serialization

  /// <summary>
  /// Serialization constructor. 
  /// </summary>
  /// <remarks>
  /// See <c>ISerializable</c> interface.
  /// </remarks>
  private NaInt32 (SerializationInfo info, StreamingContext context)
  {
    _isNull = info.GetBoolean ("IsNull");
    _value = info.GetInt32 ("Value");
  }

  /// <summary>
  /// Serialization method. 
  /// </summary>
  /// <remarks>
  /// See <c>ISerializable</c> interface.
  /// </remarks>
  public void GetObjectData (SerializationInfo info, StreamingContext context)
  {
    info.AddValue ("IsNull", _isNull);
    info.AddValue ("Value", _value);
  }

  // type conversion

  /// <summary>
  /// Converts a <see cref="NaInt32"/> structure to a <c>String</c>.
  /// </summary>
  /// <returns>
  /// A <c>String</c> object representing the <see cref="Value"/> of this instance of <c>NaInt32</c>. If this
  /// instance is null, a zero-length string is returned.
  /// </returns>
  public override string ToString()
  {
    if (_isNull)
      return String.Empty;
    return _value.ToString();
  }

  /// <summary>
  /// Converts the <c>String</c> representation of a number to its Int32 equivalent.
  /// </summary>
  /// <param name="s">The <c>String</c> to be parsed.</param>
  /// <returns>
  /// A Int32 equivalent to the value contained in the specified string. If the string is a null reference or
  /// a zero-length string, NaInt32.Null is returned.
  /// </returns>
  public static NaInt32 Parse (string s)
  {
    if (s == null || s.Length == 0)
      return NaInt32.Null;
    else return new NaInt32 (Int32.Parse(s));
  }

  /// <summary>
  /// Converts the supplied <c>Int32</c> to a <see cref="NaInt32"/> structure.
  /// </summary>
  /// <param name="value">An <c>Int32</c> value.</param>
  /// <returns>The converted <see cref="NaInt32"/> value.</returns>
  public static implicit operator NaInt32 (Int32 value)
  {
    return new NaInt32 (value);
  }

  /// <summary>
  /// Converts the supplied <see cref="NaInt32"/> structure to an <c>Int32</c>.
  /// </summary>
  /// <param name="value">A <see cref="NaInt32"/> structure.</param>
  /// <returns>The converted integer value.</returns>
  /// <exception cref="NaNullValueException">
  /// The passed value is null.
  /// </exception>
  public static explicit operator Int32 (NaInt32 value)
  {
    if (value._isNull)
      throw NaNullValueException.AccessingMember ("NaInt32 to Int32 Conversion");
    return value._value;
  }

  /// <summary>
  /// Converts the supplied <c>SqlInt32</c> structure to a <see cref="NaInt32"/> structure.
  /// </summary>
  public static implicit operator NaInt32 (SqlInt32 value)
  {
    if (value.IsNull)
      return NaInt32.Null;
    else
      return new NaInt32 (value.Value);
  }

  /// <summary>
  /// Converts the supplied <see cref="NaInt32"/> structure to a <c>SqlInt32</c>structure.
  /// </summary>
  public static implicit operator SqlInt32 (NaInt32 value)
  {
    if (value.IsNull)
      return SqlInt32.Null;
    else
      return new SqlInt32 (value.Value);
  }

  // value

  /// <summary>
  /// Gets the value of this <see cref="NaInt32"/> structure. This property is read-only.
  /// </summary>
  /// <value>
  /// An integer representing the value of this NaInt32 structure.
  /// </value>
  /// <exception cref="NaNullValueException">
  /// The property contains <see cref="Null"/>.
  /// </exception>
  public int Value 
  {
    get 
    { 
      if (_isNull)
        throw NaNullValueException.AccessingMember ("Value");
      return _value;
    }
  }

  // nullable

  /// <summary>
  /// Represents a null value that can be assigned to the <see cref="Value"/> property of 
  /// an instance of the <see cref="NaInt32"/> structure.
  /// </summary>
  /// <remarks>
  /// Null functions as a constant for the <see cref="NaInt32"/> structure.
  /// </remarks>
  public static readonly NaInt32 Null = new NaInt32(true);

  /// <summary>
  /// Indicates whether or not <see cref="Value"/> is null.
  /// </summary>
  /// <value>
  /// This property is <c>true</c> if <see cref="Value"/> is null, otherwise <c>false</c>.
  /// </value>
  public bool IsNull 
  {
    get { return _isNull; }
  }

  // public fields

  /// <summary>
  /// Represents a zero value that can be assigned to the <see cref="Value"/> property of 
  /// an instance of the <see cref="NaInt32"/> structure.
  /// </summary>
  /// <remarks>
  /// Zero functions as a constant for the <see cref="NaInt32"/> structure.
  /// </remarks>
  public static readonly NaInt32 Zero = new NaInt32 (0);

  /// <summary>
  /// A constant representing the largest possible value of a NaInt32.
  /// </summary>
  public static readonly NaInt32 MaxValue = new NaInt32 (int.MaxValue);

  /// <summary>
  /// A constant representing the smallest possible value of a NaInt32.
  /// </summary>
  public static readonly NaInt32 MinValue = new NaInt32 (int.MinValue);

  // equality

  /// <summary>
  /// Compares the supplied object parameter to the <see cref="Value"/> property of the <see cref="NaInt32"/> object.
  /// </summary>
  /// <param name="obj">The object to be compared. </param>
  /// <returns>
  /// <c>true</c> if object is an instance of <see cref="NaInt32"/> and the two are equal; otherwise <c>false</c>.
  /// If both values are <c>Null</c>, Equals returns <c>true</c>. (No SQL null equality logic is applied.)
  /// If object is a null reference, <c>false</c> is returned.
  /// Note that <c>false</c> is returned if object is an Int32 (types are not equal).
  /// </returns>
  public override bool Equals (object obj)
  {
    if (! (obj is NaInt32))
      return false; // obj is a null reference or another type then NaInt32

    return Equals (this, (NaInt32) obj);
  }

  /// <summary>
  /// Compares the supplied <see cref="NaInt32"/> parameter to the <see cref="Value"/> property of this instance.
  /// </summary>
  /// <param name="value">The <see cref="NaInt32"/> instance to be compared. </param>
  /// <returns>
  /// <c>true</c> if the two are equal; otherwise <c>false</c>.
  /// If both values are <c>Null</c>, Equals returns <c>true</c>. (Use <see cref="EqualsSql"/> if you require SQL-style equality.)
  /// </returns>
  public bool Equals (NaInt32 value)
  {
    return Equals (this, value);
  }

  /// <summary>
  /// Performs a logical comparison of the two NaInt32 parameters to determine if they are equal.
  /// </summary>
  /// <returns>
  /// <c>true</c> if the two instances are equal or <c>false</c> if they are not equal. 
  /// If both values are <c>Null</c>, Equals returns <c>true</c>. (Use <see cref="EqualsSql"/> if you require SQL-style equality.)
  /// </returns>
  public static bool Equals (NaInt32 x, NaInt32 y)
  {
    return x == y;
  }

  /// <summary>
  /// Performs a logical comparison of the two NaInt32 parameters to determine if they are equal.
  /// </summary>
  /// <returns>
  /// <c>true</c> if the two instances are equal or <c>false</c> if they are not equal. 
  /// If both values are <c>Null</c>, this operator returns <c>true</c>. (Use <see cref="EqualsSql"/> if you require SQL-style equality.)
  /// </returns>
  public static bool operator == (NaInt32 x, NaInt32 y)
  {
    if (x._isNull && y._isNull)
      return true;
    if (x._isNull != y._isNull)
      return false;
    return x._value == y._value;
  }

  /// <summary>
  /// Performs a logical comparison of the two NaInt32 parameters to determine if they are not equal.
  /// </summary>
  /// <returns>
  /// <c>false</c> if the two instances are equal or <c>true</c> if they are not equal. 
  /// If both values are <c>Null</c>, this operator returns <c>false</c>.
  /// </returns>
  public static bool operator != (NaInt32 x, NaInt32 y)
  {
    return ! (x == y);
  }

  /// <summary>
  /// Performs a SQL-style comparison of the two NaInt32 parameters to determine if they are equal.
  /// </summary>
  /// <returns>
  /// <c>True</c> if the two instances are equal, or <c>False</c> if they are not equal or <c>Null</c> if either of them is null.
  /// </returns>
  public static NaBoolean EqualsSql (NaInt32 x, NaInt32 y)
  {
    if (x._isNull || y._isNull)
      return NaBoolean.Null;
    else return new NaBoolean (x._value == y._value);
  }

  /// <summary>
  /// Returns the hash code for this instance.
  /// </summary>
  public override int GetHashCode()
  {
    if (_isNull)
      return 0;
    else return _value.GetHashCode();
  }

  /// <summary>
  /// Compares this instance to the supplied object and returns an indication of their relative values.
  /// </summary>
  /// <param name="obj">The object to be compared.</param>
  /// <returns>
  ///   A signed number indicating the relative values of the instance and the object.
  ///   <list type="table">
  ///     <listheader>
  ///       <term>Return Value</term>
  ///       <description>Condition</description>
  ///     </listheader>
  ///     <item>
  ///       <term>Less than zero</term>
  ///       <description>This instance is Null or less than the object.</description>
  ///     </item>
  ///     <item>
  ///       <term>Zero</term>
  ///       <description>This instance is equal to the object (or both are Null).</description>
  ///     </item>
  ///     <item>
  ///       <term>Greater than zero</term>
  ///       <description>This instance is greater than the object, or the object is Null, or the object is a null reference.</description>
  ///     </item>
  ///   </list>
  /// </returns>
  public int CompareTo (object obj)
  {
    if (! (obj is NaInt32))
      throw new ArgumentException ("obj");

    return CompareTo ((NaInt32) obj);
  }

  /// <summary>
  /// Compares this instance to the supplied object and returns an indication of their relative values.
  /// </summary>
  /// <param name="value">The object to be compared.</param>
  /// <returns>
  ///   A signed number indicating the relative values of the instance and the object.
  ///   <list type="table">
  ///     <listheader>
  ///       <term>Return Value</term>
  ///       <description>Condition</description>
  ///     </listheader>
  ///     <item>
  ///       <term>Less than zero</term>
  ///       <description>This instance is Null or less than the object.</description>
  ///     </item>
  ///     <item>
  ///       <term>Zero</term>
  ///       <description>This instance is equal to the object (or both are Null).</description>
  ///     </item>
  ///     <item>
  ///       <term>Greater than zero</term>
  ///       <description>This instance is greater than the object, or the object is Null.</description>
  ///     </item>
  ///   </list>
  /// </returns>
  public int CompareTo (NaInt32 value)
  {
    if (this._isNull)
    {
      if (value._isNull)
        return 0; // both are null
      else
        return -1; // this is null
    }
    if (value._isNull)
      return 1; // value is null

    if (this._value < value._value)
      return -1;
    if (this._value > value._value)
      return 1;

    return 0;
  }

  // arithmetics

  public static NaInt32 Add (NaInt32 x, NaInt32 y)
  {
    return x + y;
  }

  public static NaInt32 operator + (NaInt32 x, NaInt32 y)
  {
	  if (x.IsNull || y.IsNull)
		  return NaInt32.Null;

	  Int32 result = x._value + y._value;

	  if (NaInt32.SameSignInt(x._value, y._value) && !(NaInt32.SameSignInt(x._value, result)))
		  throw new OverflowException (NaResources.ArithmeticOverflowMsg);

	  return new NaInt32 (result);
  }

  private static bool SameSignInt (Int32 x, Int32 y)
  {
    return ((x ^ y) & 0x80000000) == 0;
  }
  
}


}
