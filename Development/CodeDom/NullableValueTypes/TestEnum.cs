using System;
using System.Runtime.Serialization;

namespace Rubicon.Data.NullableValueTypes.CodeDom
{

public enum TestEnum { Value1, Value2 }

[Serializable]
[NaBasicType (typeof (TestEnum))]
public struct NaTestEnum: INaNullable, IComparable, ISerializable, IFormattable
{
  private TestEnum _value;
  private bool _isNotNull;

  public NaTestEnum (TestEnum value)
	{
    _isNotNull = true;
    _value = value;
	}

  public int CompareTo (object obj)
  {
    if (obj == null)
      return IsNull ? 0 : 1;
    if (obj.GetType() != typeof (NaTestEnum))
      throw new ArgumentException ("obj");

    return CompareTo ((NaTestEnum) obj);
  }

  public int CompareTo (NaTestEnum val)
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
    return _value.ToString();
  }

  public string ToString (string format, IFormatProvider formatProvider)
  {
    if (IsNull)
      return "null";
    return _value.ToString(format, formatProvider);
  }

  public bool IsNull
  {
    get { return ! _isNotNull; }
  }

  public TestEnum Value
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

  public NaTestEnum (SerializationInfo info, StreamingContext context)
  {
    _isNotNull = ! info.GetBoolean ("IsNull");
    _value = (TestEnum) info.GetInt32 ("Value");
  }

  public override bool Equals (object obj)
  {
    if (obj == null || obj.GetType() != typeof (NaTestEnum))
      return false; 

    return Equals (this, (NaTestEnum) obj);
  }

  public bool Equals (NaTestEnum value)
  {
    return Equals (this, value);
  }

  public static bool Equals (NaTestEnum x, NaTestEnum y)
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

  public static bool operator == (NaTestEnum x, NaTestEnum y)
  {
    return Equals (x, y);
  }

  public static bool operator != (NaTestEnum x, NaTestEnum y)
  {
    return ! Equals (x, y);
  }

  public static implicit operator NaTestEnum (TestEnum value)
  {
    return new NaTestEnum (value);
  }

  public static explicit operator TestEnum (NaTestEnum value)
  {
    return value.Value;
  }
}

}











using System;
using System.Runtime.Serialization;

namespace Rubicon.Data.NullableValueTypes.CodeDom
{

public enum <valueType> { Value1, Value2 }

[Serializable]
[NaBasicType (typeof (<valueType>))]
public struct <nullableType>: INaNullable, IComparable, ISerializable, IFormattable
{
  private <valueType> _value;
  private bool _isNotNull;

  public <nullableType> (<valueType> value)
	{
    _isNotNull = true;
    _value = value;
	}

  public int CompareTo (object obj)
  {
    if (obj == null)
    {
      if (IsNull)
        return 0;
      else
        return 1;
    }
    if (obj.GetType() != typeof (<nullableType>))
      throw new ArgumentException ("obj");

    return CompareTo ((<nullableType>) obj);
  }

  public int CompareTo (<nullableType> val)
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
    return _value.ToString();
  }

  public string ToString (string format, IFormatProvider formatProvider)
  {
    if (IsNull)
      return "null";
    return _value.ToString(format, formatProvider);
  }

  public bool IsNull
  {
    get { return ! _isNotNull; }
  }

  public <valueType> Value
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

  public <nullableType> (SerializationInfo info, StreamingContext context)
  {
    _isNotNull = ! info.GetBoolean ("IsNull");
    _value = (<valueType>) info.GetInt32 ("Value");
  }

  public override bool Equals (object obj)
  {
    if (obj == null || obj.GetType() != typeof (<nullableType>))
      return false; 

    return Equals (this, (<nullableType>) obj);
  }

  public bool Equals (<nullableType> value)
  {
    return Equals (this, value);
  }

  public static bool Equals (<nullableType> x, <nullableType> y)
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

  public static bool operator == (<nullableType> x, <nullableType> y)
  {
    return Equals (x, y);
  }

  public static bool operator != (<nullableType> x, <nullableType> y)
  {
    return ! Equals (x, y);
  }

  public static implicit operator <nullableType> (<valueType> value)
  {
    return new <nullableType> (value);
  }

  public static explicit operator <valueType> (<nullableType> value)
  {
    return value.Value;
  }
}

}
