using System;
using System.Runtime.Serialization;

using Rubicon.NullableValueTypes;

namespace Rubicon.Development.CodeDom.NullableValueTypes.Sample
{

public enum ValueType { Value1, Value2 }

[Serializable]
[NaBasicType (typeof (ValueType))]
public struct NullableType: INaNullable, IComparable, ISerializable, IFormattable
{
  private ValueType _value;
  private bool _isNotNull;

  /// <summary>
  /// Represents a null value that can be assigned to this type.
  /// </summary>
  public static NullableType Null 
  {
    get { return new NullableType (true); }
  }

  /// <summary>
  /// The string representation of a null value.
  /// </summary>
  /// <value>The value is "null".</value>
  public const string NullString = "null";

  /// <summary>
  /// Creates a new instance with the specified value.
  /// </summary>
  public NullableType (ValueType value)
	{
    _isNotNull = true;
    _value = value;
	}

  private NullableType (bool isNull)
  {
    _isNotNull = ! isNull;
    _value = 0;
  }

  /// <summary>
  /// Returns -1 if the current value is less than the specified argument, 0 if it is equal and 1 if it is greater. Null and null references are considered equal.
  /// </summary>
  public int CompareTo (object obj)
  {
    if (obj == null)
      return IsNull ? 0 : 1;
    if (obj.GetType() != typeof (NullableType))
      throw new ArgumentException ("obj");

    return CompareTo ((NullableType) obj);
  }

  /// <summary>
  /// Returns -1 if the current value is less than the specified argument, 0 if it is equal and 1 if it is greater. Null references are considered equal.
  /// </summary>
  public int CompareTo (NullableType val)
  {
    if (this.IsNull && val.IsNull)
      return 0;
    if (this.IsNull)
      return -1;
    if (val.IsNull)
      return 1;
    if (this._value < val._value)
      return -1;
    if (this._value > val._value)
      return 1;
    return 0;
  }

  /// <summary>
  /// Returns a String that represents the current value.
  /// </summary>
  public override string ToString()
  {
    if (IsNull)
      return NullString;
    return _value.ToString();
  }

  /// <summary>
  /// Returns a String that represents the current value.
  /// </summary>
  public string ToString (string format, IFormatProvider formatProvider)
  {
    if (IsNull)
      return NullString;
    return _value.ToString(format, formatProvider);
  }

  /// <summary>
  /// Returns true if the current value is Null.
  /// </summary>
  public bool IsNull
  {
    get { return ! _isNotNull; }
  }

  /// <summary>
  /// Returns the value if the current value is not Null.
  /// </summary>
  /// <exception cref="NaNullValueException">The current value is Null.</exception>
  public ValueType Value
  {
    get 
    {
      if (IsNull)
        throw NaNullValueException.AccessingMember ("Value");
      return _value;
    }
  }

  /// <summary>
  /// Stores the current value in a serialization stream.
  /// </summary>
  public void GetObjectData (SerializationInfo info, StreamingContext context)
  {
    info.AddValue ("IsNull", IsNull);
    info.AddValue ("Value", (int) _value);
  }

  private NullableType (SerializationInfo info, StreamingContext context)
  {
    _isNotNull = ! info.GetBoolean ("IsNull");
    _value = (ValueType) info.GetInt32 ("Value");
  }

  /// <summary>
  /// Returns true if the value of the current object is equal to the specified object. 
  /// </summary>
  public override bool Equals (object obj)
  {
    if (obj == null || obj.GetType() != typeof (NullableType))
      return false; 

    return Equals (this, (NullableType) obj);
  }

  /// <summary>
  /// Returns true if the value of the current object is equal to the specified object. 
  /// </summary>
  public bool Equals (NullableType value)
  {
    return Equals (this, value);
  }

  /// <summary>
  /// Returns true if the values of the specified objects are equal. 
  /// </summary>
  public static bool Equals (NullableType x, NullableType y)
  {
    if (x.IsNull && y.IsNull)
      return true;
    if (x.IsNull || y.IsNull)
      return false;
    return x._value.Equals (y._value);
  }

  /// <summary>
  /// Returns the hash code for this instance.
  /// </summary>
  public override int GetHashCode()
  {
    if (IsNull)
      return 0;
    return _value.GetHashCode();
  }

  /// <summary>
  /// Returns true if the values of the specified objects are equal. 
  /// </summary>
  public static bool operator == (NullableType x, NullableType y)
  {
    return Equals (x, y);
  }

  /// <summary>
  /// Returns true if the values of the specified objects are not equal. 
  /// </summary>
  public static bool operator != (NullableType x, NullableType y)
  {
    return ! Equals (x, y);
  }

  /// <summary>
  /// Implicitly casts to an instance of this type from its underlying value type.
  /// </summary>
  public static implicit operator NullableType (ValueType value)
  {
    return new NullableType (value);
  }

  /// <summary>
  /// Explicitly casts an instance of this type to its underlying value type.
  /// </summary>
  /// <exception cref="NaNullValueException">The current value is Null.</exception>
  public static explicit operator ValueType (NullableType value)
  {
    return value.Value;
  }

  /// <summary>
  /// Creates a boxed instance of the underlying value type, or a null reference if the current instance is Null.
  /// </summary>
  public static object ToBoxedValueType (NullableType value)
  {
    if (value.IsNull)
      return null;
    else
      return value._value;
  }

  /// <summary>
  /// Creates a new instance from a boxed value type or a null reference.
  /// </summary>
  public static NullableType FromBoxedValueType (object value)
  {
    if (value == null)
      return NullableType.Null;

    if (! (value.GetType() == typeof (ValueType)))
      throw new ArgumentException ("Must be a ValueType value", "value");

    return new NullableType ((ValueType) value);
  }
}

}