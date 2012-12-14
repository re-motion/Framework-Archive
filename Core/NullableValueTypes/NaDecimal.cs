using System;
using System.Runtime.Serialization;
using System.Data.SqlTypes;
using System.Globalization;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace Rubicon.NullableValueTypes
{

/// <summary>
/// Represents a decimal number that can be <c>Null</c>. The corresponding system type is System.Decimal.
/// </summary>
/// <include file='doc\include\include.xml' path='Comments/NaDecimal/remarks' />
[Serializable]
[NaBasicType (typeof (Decimal))]
[TypeConverter (typeof (NaDecimalConverter))]
public struct NaDecimal: INaNullable, IComparable, ISerializable, IFormattable, IXmlSerializable
{
  #region member fields

  private decimal _value;
  private bool _isNotNull;

  #endregion

  #region construction and disposal

  /// <summary>
  /// Initializes a new instance of the <see cref="NaDecimal"/> structure using the supplied Decimal value.
  /// </summary>
  /// <param name="value">The decimal to be converted.</param>
  public NaDecimal (Decimal value)
  {
    _value = value;
    _isNotNull = true;
  }

  private NaDecimal (bool isNull)
  {
    _value = 0;
    _isNotNull = ! isNull;
  }

  #endregion

  #region serialization

  /// <summary>
  /// Serialization constructor. 
  /// </summary>
  /// <remarks>
  /// See <c>ISerializable</c> interface.
  /// </remarks>
  private NaDecimal (SerializationInfo info, StreamingContext context)
  {
    _isNotNull = ! info.GetBoolean ("IsNull");
    _value = info.GetDecimal ("Value");
  }

  /// <summary>
  /// Serialization method. 
  /// </summary>
  /// <remarks>
  /// See <c>ISerializable</c> interface.
  /// </remarks>
  public void GetObjectData (SerializationInfo info, StreamingContext context)
  {
    info.AddValue ("IsNull", IsNull);
    info.AddValue ("Value", _value);
  }

  static XmlSchema s_schema = null;
  
  XmlSchema IXmlSerializable.GetSchema()
  {
    if (s_schema == null)
    {
      lock (typeof (NaDecimal))
      {
        if (s_schema == null)
          s_schema = NaTypeUtility.CreateXmlSchema (typeof(NaDecimal).Name, "decimal");
      }
    }
    return s_schema;
  }

  void IXmlSerializable.ReadXml (XmlReader reader)
  {
    string strValue = NaTypeUtility.ReadXml (reader);
    _isNotNull = strValue != null;
    if (strValue != null)
      _value = XmlConvert.ToDecimal (strValue);
  }

  void IXmlSerializable.WriteXml (XmlWriter writer)
  {
    NaTypeUtility.WriteXml (writer, IsNull ? null : XmlConvert.ToString (_value));
  }

  #endregion

  #region type conversion

  /// <summary>
  /// Converts a <see cref="NaDecimal"/> structure to a <c>String</c>.
  /// </summary>
  /// <returns>
  /// A <c>String</c> object representing the <see cref="Value"/> of this instance of <c>NaDecimal</c>. If this
  /// instance is <c>Null</c>, <see cref="NullString"/> ("null") is returned.
  /// </returns>
  public override string ToString()
  {
    return ToString (null, null);
  }

  /// <summary>
  /// Converts a <see cref="NaDecimal"/> structure to a <c>String</c>.
  /// </summary>
  /// <param name="format">A format specification.</param>
  /// <returns>
  /// A <c>String</c> object representing the <see cref="Value"/> of this instance of <c>NaDecimal</c>. If this
  /// instance is <c>Null</c>, <see cref="NullString"/> ("null") is returned. If <c>format</c> is prefixed with a tilde symbol
  /// ("~"), a zero-length string is returned instead.
  /// </returns>
  public string ToString (string format)
  {
    return ToString (format, null);
  }

  /// <summary>
  /// Converts a <see cref="NaDecimal"/> structure to a <c>String</c>.
  /// </summary>
  /// <param name="provider">An <c>IFormatProvider</c> that supplies culture-specific formatting information. </param>
  /// <returns>
  /// A <c>String</c> object representing the <see cref="Value"/> of this instance of <c>NaDecimal</c>. If this
  /// instance is <c>Null</c>, <see cref="NullString"/> ("null") is returned.
  /// </returns>
  public string ToString (IFormatProvider provider)
  {
    return ToString (null, provider);
  }

  /// <summary>
  /// Converts a <see cref="NaDecimal"/> structure to a <c>String</c>.
  /// </summary>
  /// <param name="format">A format specification.</param>
  /// <param name="provider">An <c>IFormatProvider</c> that supplies culture-specific formatting information. </param>
  /// <returns>
  /// A <c>String</c> object representing the <see cref="Value"/> of this instance of <c>NaDecimal</c>. If this
  /// instance is <c>Null</c>, <see cref="NullString"/> ("null") is returned. If <c>format</c> is prefixed with a tilde symbol
  /// ("~"), a zero-length string is returned instead.
  /// </returns>
  public string ToString (string format, IFormatProvider provider)
  {
    if (format != null && format.Length > 0 && format[0] == '~')
    {
      if (IsNull)
        return string.Empty;
      format = format.Substring (1);
    }
    else
    {
      if (IsNull)
        return NullString;
    }
    return _value.ToString (format, provider);
  }

  /// <summary>
  /// Converts the <c>String</c> representation of a number to its NaDecimal equivalent.
  /// </summary>
  /// <param name="s">The <c>String</c> to be parsed.</param>
  /// <param name="styles">The combination of one or more <c>NumberStyles</c> constants that indicates the permitted format of <c>s</c>.</param>
  /// <param name="provider">An <c>IFormatProvider</c> that supplies culture-specific formatting information about <c>s</c>.</param>
  /// <returns>
  /// An <c>NaDecimal</c> equivalent to the value contained in the specified string. If the string is a null reference, 
  /// a zero-length string or <see cref="NullString"/> ("null"), <c>NaDecimal.Null</c> is returned. Otherwise, 
  /// <see cref="Value"/> contains the same value that <c>Decimal.Parse</c> would return.
  /// </returns>
  public static NaDecimal Parse (string s, NumberStyles styles, IFormatProvider provider)
  {
    if (s == null || s.Length == 0 || s == NullString)
      return NaDecimal.Null;
    else return new NaDecimal (Decimal.Parse(s, styles, provider));
  }

  /// <summary>
  /// Converts the <c>String</c> representation of a number to its NaDecimal equivalent.
  /// </summary>
  /// <param name="s">The <c>String</c> to be parsed.</param>
  /// <param name="provider">An <c>IFormatProvider</c> that supplies culture-specific formatting information about <c>s</c>.</param>
  /// <returns>
  /// An <c>NaDecimal</c> equivalent to the value contained in the specified string. If the string is a null reference, 
  /// a zero-length string or <see cref="NullString"/> ("null"), <c>NaDecimal.Null</c> is returned. Otherwise, 
  /// <see cref="Value"/> contains the same value that <c>Decimal.Parse</c> would return.
  /// </returns>
  public static NaDecimal Parse (string s, IFormatProvider provider)
  {
    return Parse (s, NumberStyles.Number, provider);
  }

  /// <summary>
  /// Converts the <c>String</c> representation of a number to its NaDecimal equivalent.
  /// </summary>
  /// <param name="s">The <c>String</c> to be parsed.</param>
  /// <param name="styles">The combination of one or more <c>NumberStyles</c> constants that indicates the permitted format of <c>s</c>.</param>
  /// <returns>
  /// An <c>NaDecimal</c> equivalent to the value contained in the specified string. If the string is a null reference, 
  /// a zero-length string or <see cref="NullString"/> ("null"), <c>NaDecimal.Null</c> is returned. Otherwise, 
  /// <see cref="Value"/> contains the same value that <c>Decimal.Parse</c> would return.
  /// </returns>
  public static NaDecimal Parse (string s, NumberStyles styles)
  {
    return Parse (s, styles, null);
  }

  /// <summary>
  /// Converts the <c>String</c> representation of a number to its NaDecimal equivalent.
  /// </summary>
  /// <param name="s">The <c>String</c> to be parsed.</param>
  /// <returns>
  /// An <c>NaDecimal</c> equivalent to the value contained in the specified string. If the string is a null reference, 
  /// a zero-length string or <see cref="NullString"/> ("null"), <c>NaDecimal.Null</c> is returned. Otherwise, 
  /// <see cref="Value"/> contains the same value that <c>Decimal.Parse</c> would return.
  /// </returns>
  public static NaDecimal Parse (string s)
  {
    return Parse (s, NumberStyles.Number, null);
  }

  /// <summary>
  /// Converts the supplied <c>Decimal</c> to a <see cref="NaDecimal"/> structure.
  /// </summary>
  /// <param name="value">A <c>Decimal</c> value.</param>
  /// <returns>The converted <see cref="NaDecimal"/> value.</returns>
  public static implicit operator NaDecimal (Decimal value)
  {
    return new NaDecimal (value);
  }

  /// <summary>
  /// Converts the supplied <see cref="NaDecimal"/> structure to a <c>Decimal</c>.
  /// </summary>
  /// <param name="value">A <see cref="NaDecimal"/> structure.</param>
  /// <returns>The converted decimal value.</returns>
  /// <exception cref="NaNullValueException">
  /// The passed value is null.
  /// </exception>
  public static explicit operator Decimal (NaDecimal value)
  {
    if (value.IsNull)
      throw NaNullValueException.AccessingMember ("NaDecimal to Decimal Conversion");
    return value._value;
  }

  /// <summary>
  /// Converts the supplied <c>SqlDecimal</c> structure to a <see cref="NaDecimal"/> structure.
  /// </summary>
  public static NaDecimal FromSqlDecimal (SqlDecimal value)
  {
    return (NaDecimal) value;
  }

  /// <summary>
  /// Converts the supplied <c>SqlDecimal</c> structure to a <see cref="NaDecimal"/> structure.
  /// </summary>
  public static implicit operator NaDecimal (SqlDecimal value)
  {
    if (value.IsNull)
      return NaDecimal.Null;
    else
      return new NaDecimal (value.Value);
  }

  /// <summary>
  /// Converts the supplied <see cref="NaDecimal"/> structure to a <c>SqlDecimal</c>structure.
  /// </summary>
  public static SqlDecimal ToSqlDecimal (NaDecimal value)
  {
    return (SqlDecimal) value;
  }

  /// <summary>
  /// Converts the supplied <see cref="NaDecimal"/> structure to a <c>SqlDecimal</c>structure.
  /// </summary>
  public static implicit operator SqlDecimal (NaDecimal value)
  {
    if (value.IsNull)
      return SqlDecimal.Null;
    else
      return new SqlDecimal (value.Value);
  }

  /// <summary>
  /// Converts a <see cref="NaDecimal"/> value to a boxed <c>Decimal</c> value or a null reference.
  /// </summary>
  /// <returns>The integer value of <c>value</c> as an <c>Decimal</c> if it is not null, a null reference otherwise.</returns>
  /// <remarks>
  /// Use this method to easily pass <c>NaDecimal</c> values to methods that expect an untyped parameter which is either a <c>Decimal</c>
  /// value or a null reference.
  /// </remarks>
  public static object ToBoxedDecimal (NaDecimal value)
  {
    if (value.IsNull )
      return null;
    else
      return value._value;
  }

  /// <summary>
  /// Converts a boxed <c>NaDecimal</c> value or a null reference to a <see cref="NaDecimal"/> value.
  /// </summary>
  /// <returns>A <see cref="NaDecimal"/> with its <see cref="Value"/> set to the integer value of <c>value</c> if it is an <c>Decimal</c>, 
  /// <c>NaDecimal.Null</c> if it is a null reference.</returns>
  /// <remarks>
  /// Use this method to easily create an <c>NaDecimal</c> value from an untyped value which is either an <c>Decimal</c> value or a null reference.
  /// </remarks>
  /// <exception cref="ArgumentException"><c>value</c> is neither a null reference nor an <c>Decimal</c> value.</exception>
  public static NaDecimal FromBoxedDecimal (object value)
  {
    if (value == null)
      return NaDecimal.Null;

    if (! (value is Decimal))
      throw new ArgumentException ("Must be a Decimal value", "value");

    return new NaDecimal ((Decimal) value);
  }

  /// <summary>
  /// Converts a <see cref="NaDecimal"/> value to a boxed <c>Decimal</c> value or to DBNull.Value.
  /// </summary>
  /// <returns>The integer value of <c>value</c> as a <c>Decimal</c> if it is not null, DBNull.Value otherwise.</returns>
  /// <remarks>
  /// Use this method to easily pass <c>NaDecimal</c> values to methods that expect an untyped parameter which is either a <c>Decimal</c>
  /// value or DBNull.Value.
  /// </remarks>
  public static object ToBoxedDecimalDBNull (NaDecimal value)
  {
    if (value.IsNull )
      return DBNull.Value;
    else
      return value._value;
  }

  /// <summary>
  /// Converts a boxed <c>Decimal</c> value or DBNull.Value to a <see cref="NaDecimal"/> value.
  /// </summary>
  /// <returns>A <see cref="NaDecimal"/> with its <see cref="Value"/> set to the integer value of <c>value</c> if it is a <c>Decimal</c>, 
  /// <c>NaDecimal.Null</c> if it is DBNull.Value.</returns>
  /// <remarks>
  /// Use this method to easily create a <c>NaDecimal</c> value from an untyped value which is either a <c>Decimal</c> value or DBNull.Value.
  /// </remarks>
  /// <exception cref="ArgumentException"><c>value</c> is neither DBNull.Value nor an <c>Int32</c> value.</exception>
  public static NaDecimal FromBoxedDecimalDBNull (object value)
  {
    if (value == DBNull.Value)
      return NaDecimal.Null;

    if (! (value is Decimal))
      throw new ArgumentException ("Must be a Decimal value", "value");

    return new NaDecimal ((Decimal) value);
  }

  #endregion

  #region value

  /// <summary>
  /// Gets the value of this <see cref="NaDecimal"/> structure. This property is read-only.
  /// </summary>
  /// <value>
  /// An integer representing the value of this NaDecimal structure.
  /// </value>
  /// <exception cref="NaNullValueException">
  /// The property contains <see cref="Null"/>.
  /// </exception>
  public Decimal Value 
  {
    get 
    { 
      if (IsNull)
        throw NaNullValueException.AccessingMember ("Value");
      return _value;
    }
  }


  /// <summary>
  /// Gets the value of this <see cref="NaDecimal"/> structure. This property is read-only.
  /// </summary>
  /// <value>
  /// An Object representing the value of this NaDecimal structure.
  /// </value>
  /// <exception cref="NaNullValueException">
  /// The property contains <see cref="Null"/>.
  /// </exception>
  object INaNullable.Value
  {
    get { return Value; }
  }

  /// <summary>
  /// Gets the value of the structure for debugger watch windows.
  /// </summary>
  /// <remarks>Modify Visual Studio's mcee_cs.dat file to set this as the default property for watch windows.</remarks>
  private object DebuggingValue
  {
    get 
    { 
      if (IsNull)
        return DebuggingNull.Null;
      else
        return _value;
    }
  }
  #endregion

  #region nullable

  /// <summary>
  /// Represents a null value that can be assigned to an instance of the <see cref="NaDecimal"/> structure.
  /// </summary>
  /// <remarks>
  /// Null functions as a constant for the <see cref="NaDecimal"/> structure.
  /// </remarks>
  public static readonly NaDecimal Null = new NaDecimal (true);

  /// <summary>
  /// Indicates whether or not <see cref="Value"/> is null.
  /// </summary>
  /// <value>
  /// This property is <c>true</c> if <see cref="Value"/> is null, otherwise <c>false</c>.
  /// </value>
  public bool IsNull 
  {
    get { return ! _isNotNull; }
  }

  #endregion

  #region public fields

  /// <summary>
  /// This value is used to convert a <c>Null</c> value to and from strings.
  /// </summary>
  /// <value>
  /// The value of <c>NullString</c> is "null".
  /// </value>
  /// <remarks>
  /// Note that parsing <c>NullString</c> is case-sensitive.
  /// </remarks>
  public static readonly string NullString = "null";

  /// <summary>
  /// Represents a zero value that can be assigned to the <see cref="Value"/> property of 
  /// an instance of the <see cref="NaDecimal"/> structure.
  /// </summary>
  /// <remarks>
  /// Zero functions as a constant for the <see cref="NaDecimal"/> structure.
  /// </remarks>
  public static readonly NaDecimal Zero = new NaDecimal (0);

  /// <summary>
  /// A constant representing the largest possible value of a NaDecimal.
  /// </summary>
  public static readonly NaDecimal MaxValue = new NaDecimal (Decimal.MaxValue);

  /// <summary>
  /// A constant representing the smallest possible value of a NaDecimal.
  /// </summary>
  public static readonly NaDecimal MinValue = new NaDecimal (Decimal.MinValue);

  #endregion

  #region equality

  /// <summary>
  /// Compares the supplied object parameter to the <see cref="Value"/> property of the <see cref="NaDecimal"/> object.
  /// </summary>
  /// <param name="obj">The object to be compared. </param>
  /// <returns>
  /// <c>true</c> if object is an instance of <see cref="NaDecimal"/> and the two are equal; otherwise <c>false</c>.
  /// If object is a null reference, <c>false</c> is returned.
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// Note that <c>false</c> is returned if object is a Decimal (types are not equal).
  /// </remarks>
  public override bool Equals (object obj)
  {
    if (! (obj is NaDecimal))
      return false; // obj is a null reference or another type then NaDecimal

    return Equals (this, (NaDecimal) obj);
  }

  /// <summary>
  /// Compares the supplied <see cref="NaDecimal"/> parameter to the <see cref="Value"/> property of this instance.
  /// </summary>
  /// <param name="value">The <see cref="NaDecimal"/> instance to be compared. </param>
  /// <returns>
  /// <c>true</c> if the two are equal; otherwise <c>false</c>.
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public bool Equals (NaDecimal value)
  {
    return Equals (this, value);
  }

  /// <summary>
  /// Performs a logical comparison of the two NaDecimal parameters to determine if they are equal.
  /// </summary>
  /// <returns>
  /// <c>true</c> if the two instances are equal or <c>false</c> if they are not equal. 
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public static bool Equals (NaDecimal x, NaDecimal y)
  {
    return x == y;
  }

  /// <summary>
  /// Performs a logical comparison of the two NaDecimal parameters to determine if they are equal.
  /// </summary>
  /// <returns>
  /// <c>true</c> if the two instances are equal or <c>false</c> if they are not equal. 
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public static bool operator == (NaDecimal x, NaDecimal y)
  {
    if (x.IsNull && y.IsNull)
      return true;
    if (x.IsNull != y.IsNull)
      return false;
    return x._value == y._value;
  }

  /// <summary>
  /// Performs a logical comparison of the two NaDecimal parameters to determine if they are not equal.
  /// </summary>
  /// <returns>
  /// <c>false</c> if the two instances are equal or <c>true</c> if they are not equal. 
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="NotEqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public static bool operator != (NaDecimal x, NaDecimal y)
  {
    return ! (x == y);
  }

  /// <summary>
  /// Performs a logical comparison of the two NaDecimal parameters to determine if they are not equal.
  /// </summary>
  /// <returns>
  /// <c>false</c> if the two instances are equal or <c>true</c> if they are not equal. 
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="NotEqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public static bool NotEquals (NaDecimal x, NaDecimal y)
  {
    return x != y;
  }

  /// <summary>
  /// Performs a SQL-style comparison of the two NaDecimal parameters to determine if they are equal.
  /// </summary>
  /// <returns>
  /// <c>True</c> if the two instances are equal, or <c>False</c> if they are not equal, or <c>Null</c> if either of them is <c>Null</c>.
  /// </returns>
  public static NaBoolean EqualsSql (NaDecimal x, NaDecimal y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;
    else 
      return new NaBoolean (x._value == y._value);
  }

  /// <summary>
  /// Performs a SQL-style comparison of the two NaDecimal parameters to determine if they are not equal.
  /// </summary>
  /// <returns>
  /// <c>True</c> if the two instances are not equal, or <c>False</c> if they are equal, or <c>Null</c> if either of them is <c>Null</c>.
  /// </returns>
  public static NaBoolean NotEqualsSql (NaDecimal x, NaDecimal y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;
    else 
      return new NaBoolean (x._value != y._value);
  }

  /// <summary>
  /// Returns the hash code for this instance.
  /// </summary>
  public override int GetHashCode()
  {
    if (IsNull)
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
  ///       <description>This instance is equal to the object (or both are Null or a null reference).</description>
  ///     </item>
  ///     <item>
  ///       <term>Greater than zero</term>
  ///       <description>This instance is greater than the object, or the object is Null, or the object is a null reference.</description>
  ///     </item>
  ///   </list>
  /// </returns>
  public int CompareTo (object obj)
  {
    if (obj == null)
      return IsNull ? 0 : 1;

    if (! (obj is NaDecimal))
      throw new ArgumentException ("obj");

    return CompareTo ((NaDecimal) obj);
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
  public int CompareTo (NaDecimal value)
  {
    if (this.IsNull)
    {
      if (value.IsNull)
        return 0; // both are null
      else
        return -1; // this is null
    }
    if (value.IsNull)
      return 1; // value is null

    if (this._value < value._value)
      return -1;
    if (this._value > value._value)
      return 1;

    return 0;
  }

  #endregion

  #region arithmetics

  /// <summary>
  /// Computes the sum of the two specified <see cref="NaDecimal"/> structures.
  /// </summary>
  /// <returns>
  /// A <c>NaDecimal</c> structure whose <see cref="Value"/> property contains the sum of the specified <c>NaDecimal</c> 
  /// structures, or <see cref="Null"/> if either parameter is <c>Null</c>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public static NaDecimal Add (NaDecimal x, NaDecimal y)
  {
    return x + y;
  }

  /// <summary>
  /// Computes the sum of the two specified <see cref="NaDecimal"/> structures.
  /// </summary>
  /// <returns>
  /// A <c>NaDecimal</c> structure whose <see cref="Value"/> property contains the sum of the specified <c>NaDecimal</c> 
  /// structures, or <see cref="Null"/> if either parameter is <c>Null</c>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public static NaDecimal operator + (NaDecimal x, NaDecimal y)
  {
	  if (x.IsNull || y.IsNull)
		  return NaDecimal.Null;

    checked
    {
      return new NaDecimal (x._value + y._value);
    }
  }

  /// <summary>
  /// Subtracts the second <see cref="NaDecimal"/> parameter from the first.
  /// </summary>
  /// <returns>
  /// A <c>NaDecimal</c> structure whose <see cref="Value"/> property contains the result of the subtraction,
  /// or <see cref="Null"/> if either parameter is <c>Null</c>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public static NaDecimal Subtract (NaDecimal x, NaDecimal y)
  {
    return x - y;
  }

  /// <summary>
  /// Subtracts the second <see cref="NaDecimal"/> parameter from the first.
  /// </summary>
  /// <returns>
  /// A <c>NaDecimal</c> structure whose <see cref="Value"/> property contains the result of the subtraction,
  /// or <see cref="Null"/> if either parameter is <c>Null</c>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public static NaDecimal operator - (NaDecimal x, NaDecimal y)
  {
    if (x.IsNull || y.IsNull)
      return NaDecimal.Null;

    checked
    {
      return new NaDecimal (x._value - y._value);
    }
  }

  /// <summary>
  /// Divides the first <see cref="NaDecimal"/> parameter by the second.
  /// </summary>
  /// <returns>
  /// A <c>NaDecimal</c> structure whose <see cref="Value"/> property contains the result of the division,
  /// or <see cref="Null"/> if either parameter is <c>Null</c>.
  /// </returns>
  /// <exception cref="DivideByZeroException">A division by zero is attempted.</exception>
  public static NaDecimal Divide (NaDecimal x, NaDecimal y)
  {
    return x / y;
  }

  /// <summary>
  /// Divides the first <see cref="NaDecimal"/> parameter by the second.
  /// </summary>
  /// <returns>
  /// A <c>NaDecimal</c> structure whose <see cref="Value"/> property contains the result of the division,
  /// or <see cref="Null"/> if either parameter is <c>Null</c>.
  /// </returns>
  /// <exception cref="DivideByZeroException">A division by zero is attempted.</exception>
  public static NaDecimal operator / (NaDecimal x, NaDecimal y)
  {
    if (x.IsNull || y.IsNull)
      return NaDecimal.Null;

    checked
    {
      return new NaDecimal (x._value / y._value);
    }
  }

  /// <summary>
  /// Computes the product of the two <see cref="NaDecimal"/> parameters.
  /// </summary>
  /// <returns>
  /// A <c>NaDecimal</c> structure whose <see cref="Value"/> property contains the product of the two parameters,
  /// or <see cref="Null"/> if either parameter is <c>Null</c>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public static NaDecimal Multiply (NaDecimal x, NaDecimal y)
  {
    return x * y;
  }

  /// <summary>
  /// Computes the product of the two <see cref="NaDecimal"/> parameters.
  /// </summary>
  /// <returns>
  /// A <c>NaDecimal</c> structure whose <see cref="Value"/> property contains the product of the two parameters,
  /// or <see cref="Null"/> if either parameter is <c>Null</c>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public static NaDecimal operator * (NaDecimal x, NaDecimal y)
  {
    if (x.IsNull || y.IsNull)
      return NaDecimal.Null;

    checked
    {
      return new NaDecimal (x._value * y._value);
    }
  }

  /// <summary>
  /// Computes the remainder after dividing the first <see cref="NaDecimal"/> parameter by the second.
  /// </summary>
  /// <returns>
  /// A <c>NaDecimal</c> structure whose <see cref="Value"/> property contains the remainder,
  /// or <see cref="Null"/> if either parameter is <c>Null</c>.
  /// </returns>
  /// <exception cref="DivideByZeroException">A division by zero is attempted.</exception>
  public static NaDecimal Mod (NaDecimal x, NaDecimal y)
  {
    return x % y;
  }

  /// <summary>
  /// Computes the remainder after dividing the first <see cref="NaDecimal"/> parameter by the second.
  /// </summary>
  /// <returns>
  /// A <c>NaDecimal</c> structure whose <see cref="Value"/> property contains the remainder,
  /// or <see cref="Null"/> if either parameter is <c>Null</c>.
  /// </returns>
  /// <exception cref="DivideByZeroException">A division by zero is attempted.</exception>
  public static NaDecimal operator % (NaDecimal x, NaDecimal y)
  {
    if (x.IsNull || y.IsNull)
      return NaDecimal.Null;

    checked
    {
      return new NaDecimal (x._value % y._value);
    }
  }

  /// <summary>
  /// Increments the value by one. 
  /// </summary>
  /// <remarks>
  /// The <c>Value</c> property is incremented by one. If <c>x</c> is <c>Null</c>, the value is not modified.
  /// </remarks>
  /// <returns>
  /// A <c>NaDecimal</c> structure whose <see cref="Value"/> property contains the incremented value when used as a prefix operator,
  /// or the original value when used as a postfix operator. 
  /// </returns>
  public static NaDecimal operator ++ (NaDecimal x)
  {
    if (x.IsNull) 
      return NaDecimal.Null;

    ++ x._value;
    return x;
  }

  /// <summary>
  /// Decrements the value by one. 
  /// </summary>
  /// <remarks>
  /// The <c>Value</c> property is decremented by one. If <c>x</c> is <c>Null</c>, the value is not modified.
  /// </remarks>
  /// <returns>
  /// A <c>NaDecimal</c> structure whose <see cref="Value"/> property contains the decremented value when used as a prefix operator,
  /// or the original value when used as a postfix operator. 
  /// </returns>
  public static NaDecimal operator -- (NaDecimal x)
  {
    if (x.IsNull) 
      return NaDecimal.Null;

    -- x._value;
    return x;
  }

  #endregion

  #region relative compare operators

  /// <summary>
  /// Compares the two <see cref="NaDecimal "/> parameters to determine if the first is less than the second.
  /// </summary>
  /// <returns>
  /// A <see cref="NaBoolean"/> that is <c>True</c> if the first parameter is less than the second, otherwise <c>False</c>. If 
  /// either parameter is <c>Null</c>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean LessThan (NaDecimal x, NaDecimal y)
  {
    return x < y;
  }

  /// <summary>
  /// Compares the two <see cref="NaDecimal "/> parameters to determine if the first is less than the second.
  /// </summary>
  /// <returns>
  /// A <see cref="NaBoolean"/> that is <c>True</c> if the first parameter is less than the second, otherwise <c>False</c>. If 
  /// either parameter is <c>Null</c>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean operator < (NaDecimal x, NaDecimal y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;

    return new NaBoolean (x._value < y._value);
  }

  /// <summary>
  /// Compares the two <see cref="NaDecimal "/> parameters to determine if the first is less than or equal to the second.
  /// </summary>
  /// <returns>
  /// A <see cref="NaBoolean"/> that is <c>True</c> if the first parameter is less than or equal to the second, otherwise <c>False</c>. If 
  /// either parameter is <c>Null</c>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean LessThanOrEqual (NaDecimal x, NaDecimal y)
  {
    return x <= y;
  }

  /// <summary>
  /// Compares the two <see cref="NaDecimal "/> parameters to determine if the first is less than or equal to the second.
  /// </summary>
  /// <returns>
  /// A <see cref="NaBoolean"/> that is <c>True</c> if the first parameter is less than or equal to the second, otherwise <c>False</c>. If 
  /// either parameter is <c>Null</c>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean operator <= (NaDecimal x, NaDecimal y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;

    return new NaBoolean (x._value <= y._value);
  }

  /// <summary>
  /// Compares the two <see cref="NaDecimal "/> parameters to determine if the first is greater than the second.
  /// </summary>
  /// <returns>
  /// A <see cref="NaBoolean"/> that is <c>True</c> if the first parameter is greater than the second, otherwise <c>False</c>. If 
  /// either parameter is <c>Null</c>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean GreaterThan (NaDecimal x, NaDecimal y)
  {
    return x > y;
  }

  /// <summary>
  /// Compares the two <see cref="NaDecimal "/> parameters to determine if the first is greater than the second.
  /// </summary>
  /// <returns>
  /// A <see cref="NaBoolean"/> that is <c>True</c> if the first parameter is greater than the second, otherwise <c>False</c>. If 
  /// either parameter is <c>Null</c>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean operator > (NaDecimal x, NaDecimal y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;

    return new NaBoolean (x._value > y._value);
  }

  /// <summary>
  /// Compares the two <see cref="NaDecimal "/> parameters to determine if the first is greater than or equal to the second.
  /// </summary>
  /// <returns>
  /// A <see cref="NaBoolean"/> that is <c>True</c> if the first parameter is greater than or equal to the second, otherwise <c>False</c>. If 
  /// either parameter is <c>Null</c>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean GreaterThanOrEqual (NaDecimal x, NaDecimal y)
  {
    return x >= y;
  }

  /// <summary>
  /// Compares the two <see cref="NaDecimal "/> parameters to determine if the first is greater than or equal to the second.
  /// </summary>
  /// <returns>
  /// A <see cref="NaBoolean"/> that is <c>True</c> if the first parameter is greater than or equal to the second, otherwise <c>False</c>. If 
  /// either parameter is <c>Null</c>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean operator >= (NaDecimal x, NaDecimal y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;

    return new NaBoolean (x._value >= y._value);
  }
  
  #endregion
}

}
