using System;
using System.Runtime.Serialization;

using Rubicon.Data.NullableValueTypes;

namespace Rubicon.Development.CodeDom.NullableValueTypes.Sample
{

public enum ValueType { Value1, Value2 }

[Serializable]
[NaBasicType (typeof (ValueType))]
public struct NullableType: INaNullable, IComparable, ISerializable, IFormattable
{
  private ValueType _value;
  private bool _isNotNull;

  public static readonly string NullString = "null";

  public NullableType (ValueType value)
	{
    _isNotNull = true;
    _value = value;
	}

  public int CompareTo (object obj)
  {
    if (obj == null)
      return IsNull ? 0 : 1;
    if (obj.GetType() != typeof (NullableType))
      throw new ArgumentException ("obj");

    return CompareTo ((NullableType) obj);
  }

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

  public override string ToString()
  {
    if (IsNull)
      return NullString;
    return _value.ToString();
  }

  public string ToString (string format, IFormatProvider formatProvider)
  {
    if (IsNull)
      return NullString;
    return _value.ToString(format, formatProvider);
  }

  public bool IsNull
  {
    get { return ! _isNotNull; }
  }

  public ValueType Value
  {
    get 
    {
      if (IsNull)
        throw NaNullValueException.AccessingMember ("Value");
      return _value;
    }
  }

  public void GetObjectData (SerializationInfo info, StreamingContext context)
  {
    info.AddValue ("IsNull", IsNull);
    info.AddValue ("Value", (int) _value);
  }

  public NullableType (SerializationInfo info, StreamingContext context)
  {
    _isNotNull = ! info.GetBoolean ("IsNull");
    _value = (ValueType) info.GetInt32 ("Value");
  }

  public override bool Equals (object obj)
  {
    if (obj == null || obj.GetType() != typeof (NullableType))
      return false; 

    return Equals (this, (NullableType) obj);
  }

  public bool Equals (NullableType value)
  {
    return Equals (this, value);
  }

  public static bool Equals (NullableType x, NullableType y)
  {
    if (x.IsNull && y.IsNull)
      return true;
    if (x.IsNull || y.IsNull)
      return false;
    return x._value.Equals (y._value);
  }

  public override int GetHashCode()
  {
    if (IsNull)
      return 0;
    return _value.GetHashCode();
  }

  public static bool operator == (NullableType x, NullableType y)
  {
    return Equals (x, y);
  }

  public static bool operator != (NullableType x, NullableType y)
  {
    return ! Equals (x, y);
  }

  public static implicit operator NullableType (ValueType value)
  {
    return new NullableType (value);
  }

  public static explicit operator ValueType (NullableType value)
  {
    return value.Value;
  }
}

}