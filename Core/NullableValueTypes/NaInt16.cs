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
/// Represents a 16-bit signed integer that can be <see langword="null"/>. The corresponding system type is System.Int16.
/// </summary>
/// <include file='doc\include\NullableValueTypes\include.xml' path='Comments/NaInt16/remarks' />
[Serializable]
[NaBasicType (typeof (Int16))]
[TypeConverter (typeof (NaInt16Converter))]
public struct NaInt16: INaNullable, IComparable, IFormattable, IXmlSerializable
{
  #region member fields

  private short _value;
  private bool _isNotNull;

  #endregion

  #region construction and disposal

  /// <summary>
  /// Initializes a new instance of the <see cref="NaInt16"/> structure using the supplied integer value.
  /// </summary>
  /// <param name="value">The integer to be converted.</param>
  public NaInt16 (Int16 value)
  {
    _value = value;
    _isNotNull = true;
  }

  private NaInt16 (bool isNull)
  {
    _value = 0;
    _isNotNull = ! isNull;
  }

  #endregion

  #region serialization

  static XmlSchema s_schema = null;
  
  XmlSchema IXmlSerializable.GetSchema()
  {
    if (s_schema == null)
    {
      lock (typeof (NaInt16))
      {
        if (s_schema == null)
          s_schema = NaTypeUtility.CreateXmlSchema (typeof(NaInt16).Name, "short");
      }
    }
    return s_schema;
  }

  void IXmlSerializable.ReadXml (XmlReader reader)
  {
    string strValue = NaTypeUtility.ReadXml (reader);
    _isNotNull = strValue != null;
    if (strValue != null)
      _value = XmlConvert.ToInt16 (strValue);
  }

  void IXmlSerializable.WriteXml (XmlWriter writer)
  {
    NaTypeUtility.WriteXml (writer, IsNull ? null : XmlConvert.ToString (_value));
  }

  #endregion

  #region type conversion

  /// <summary>
  /// Converts an <see cref="NaInt16"/> structure to a <see cref="String"/>.
  /// </summary>
  /// <returns>
  /// A <see cref="String"/> object representing the <see cref="Value"/> of this instance of <see cref="NaInt16"/>. If this
  /// instance is <see langword="null"/>, <see cref="NullString"/> ("null") is returned.
  /// </returns>
  public override string ToString()
  {
    return ToString (null, null);
  }

  /// <summary>
  /// Converts an <see cref="NaInt16"/> structure to a <see cref="String"/>.
  /// </summary>
  /// <param name="format">A format specification.</param>
  /// <returns>
  /// A <see cref="String"/> object representing the <see cref="Value"/> of this instance of <see cref="NaInt16"/>. If this
  /// instance is <see langword="null"/>, <see cref="NullString"/> ("null") is returned. If <paramref name="format"/> is prefixed with a tilde symbol
  /// ("~"), a zero-length string is returned instead.
  /// </returns>
  public string ToString (string format)
  {
    return ToString (format, null);
  }

  /// <summary>
  /// Converts an <see cref="NaInt16"/> structure to a <see cref="String"/>.
  /// </summary>
  /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information. </param>
  /// <returns>
  /// A <see cref="String"/> object representing the <see cref="Value"/> of this instance of <see cref="NaInt16"/>. If this
  /// instance is <see langword="null"/>, <see cref="NullString"/> ("null") is returned.
  /// </returns>
  public string ToString (IFormatProvider provider)
  {
    return ToString (null, provider);
  }

  /// <summary>
  /// Converts an <see cref="NaInt16"/> structure to a <see cref="String"/>.
  /// </summary>
  /// <param name="format">A format specification.</param>
  /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information. </param>
  /// <returns>
  /// A <see cref="String"/> object representing the <see cref="Value"/> of this instance of <see cref="NaInt16"/>. If this
  /// instance is <see langword="null"/>, <see cref="NullString"/> ("null") is returned. If <paramref name="format"/> is prefixed with a tilde symbol
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
  /// Converts the <see cref="String"/> representation of a number to its NaInt16 equivalent.
  /// </summary>
  /// <param name="s">The <see cref="String"/> to be parsed.</param>
  /// <param name="styles">The combination of one or more <see cref="NumberStyles"/> constants that indicates the permitted format of <paramref name="s"/>.</param>
  /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information about <paramref name="s"/>.</param>
  /// <returns>
  /// An <see cref="NaInt16"/> equivalent to the value contained in the specified string. If the string is a null reference, 
  /// a zero-length string or <see cref="NullString"/> ("null"), <see cref="NaInt16.Null"/> is returned. Otherwise, 
  /// <see cref="Value"/> contains the same value that <see cref="Int16.Parse"/> would return.
  /// </returns>
  public static NaInt16 Parse (string s, NumberStyles styles, IFormatProvider provider)
  {
    if (s == null || s.Length == 0 || s == NullString)
      return NaInt16.Null;
    else return new NaInt16 (Int16.Parse(s, styles, provider));
  }

  /// <summary>
  /// Converts the <see cref="String"/> representation of a number to its NaInt16 equivalent.
  /// </summary>
  /// <param name="s">The <see cref="String"/> to be parsed.</param>
  /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information about <paramref name="s"/>.</param>
  /// <returns>
  /// An <see cref="NaInt16"/> equivalent to the value contained in the specified string. If the string is a null reference, 
  /// a zero-length string or <see cref="NullString"/> ("null"), <see cref="NaInt16.Null"/> is returned. Otherwise, 
  /// <see cref="Value"/> contains the same value that <see cref="Int16.Parse"/> would return.
  /// </returns>
  public static NaInt16 Parse (string s, IFormatProvider provider)
  {
    return Parse (s, NumberStyles.Integer, provider);
  }

  /// <summary>
  /// Converts the <see cref="String"/> representation of a number to its NaInt16 equivalent.
  /// </summary>
  /// <param name="s">The <see cref="String"/> to be parsed.</param>
  /// <param name="styles">The combination of one or more <see cref="NumberStyles"/> constants that indicates the permitted format of <paramref name="s"/>.</param>
  /// <returns>
  /// An <see cref="NaInt16"/> equivalent to the value contained in the specified string. If the string is a null reference, 
  /// a zero-length string or <see cref="NullString"/> ("null"), <see cref="NaInt16.Null"/> is returned. Otherwise, 
  /// <see cref="Value"/> contains the same value that <see cref="Int16.Parse"/> would return.
  /// </returns>
  public static NaInt16 Parse (string s, NumberStyles styles)
  {
    return Parse (s, styles, null);
  }

  /// <summary>
  /// Converts the <see cref="String"/> representation of a number to its NaInt16 equivalent.
  /// </summary>
  /// <param name="s">The <see cref="String"/> to be parsed.</param>
  /// <returns>
  /// An <see cref="NaInt16"/> equivalent to the value contained in the specified string. If the string is a null reference, 
  /// a zero-length string or <see cref="NullString"/> ("null"), <see cref="NaInt16.Null"/> is returned. Otherwise, 
  /// <see cref="Value"/> contains the same value that <see cref="Int16.Parse"/> would return.
  /// </returns>
  public static NaInt16 Parse (string s)
  {
    return Parse (s, NumberStyles.Integer, null);
  }

  /// <summary>
  /// Converts the supplied <see cref="Int16"/> to an <see cref="NaInt16"/> structure.
  /// </summary>
  /// <param name="value">A <see cref="Int16"/> value.</param>
  /// <returns>The converted <see cref="NaInt16"/> value.</returns>
  public static implicit operator NaInt16 (Int16 value)
  {
    return new NaInt16 (value);
  }

  /// <summary>
  /// Converts the supplied <see cref="NaInt16"/> structure to a <see cref="Int16"/>.
  /// </summary>
  /// <param name="value">An <see cref="NaInt16"/> structure.</param>
  /// <returns>The converted integer value.</returns>
  /// <exception cref="NaNullValueException">
  /// The passed value is null.
  /// </exception>
  public static explicit operator Int16 (NaInt16 value)
  {
    if (value.IsNull)
      throw NaNullValueException.AccessingMember ("NaInt16 to Int16 Conversion");
    return value._value;
  }

  /// <summary>
  /// Converts the supplied <see cref="SqlInt16"/> structure to an <see cref="NaInt16"/> structure.
  /// </summary>
  public static NaInt16 FromSqlInt16 (SqlInt16 value)
  {
    return (NaInt16) value;
  }

  /// <summary>
  /// Converts the supplied <see cref="SqlInt16"/> structure to an <see cref="NaInt16"/> structure.
  /// </summary>
  public static implicit operator NaInt16 (SqlInt16 value)
  {
    if (value.IsNull)
      return NaInt16.Null;
    else
      return new NaInt16 (value.Value);
  }

  /// <summary>
  /// Converts the supplied <see cref="NaInt16"/> structure to a <see cref="SqlInt16"/>structure.
  /// </summary>
  public static SqlInt16 ToSqlInt16 (NaInt16 value)
  {
    return (SqlInt16) value;
  }

  /// <summary>
  /// Converts the supplied <see cref="NaInt16"/> structure to a <see cref="SqlInt16"/>structure.
  /// </summary>
  public static implicit operator SqlInt16 (NaInt16 value)
  {
    if (value.IsNull)
      return SqlInt16.Null;
    else
      return new SqlInt16 (value.Value);
  }

  /// <summary>
  /// Converts an <see cref="NaInt16"/> value to a boxed <see cref="Int16"/> value or a null reference.
  /// </summary>
  /// <returns>The integer value of <paramref name="value"/> as an <see cref="Int16"/> if it is not null, a null reference otherwise.</returns>
  /// <remarks>
  /// Use this method to easily pass <see cref="NaInt16"/> values to methods that expect an untyped parameter which is either a <see cref="Int16"/>
  /// value or a null reference.
  /// </remarks>
  public static object ToBoxedInt16 (NaInt16 value)
  {
    if (value.IsNull )
      return null;
    else
      return value._value;
  }

  /// <summary>
  /// Converts a boxed <see cref="NaInt16"/> value or a null reference to an <see cref="NaInt16"/> value.
  /// </summary>
  /// <returns>An <see cref="NaInt16"/> with its <see cref="Value"/> set to the integer value of <paramref name="value"/> if it is an <see cref="Int16"/>, 
  /// <see cref="NaInt16.Null"/> if it is a null reference.</returns>
  /// <remarks>
  /// Use this method to easily create an <see cref="NaInt16"/> value from an untyped value which is either an <see cref="Int16"/> value or a null reference.
  /// </remarks>
  /// <exception cref="ArgumentException"><paramref name="value"/> is neither a null reference nor an <see cref="Int16"/> value.</exception>
  public static NaInt16 FromBoxedInt16 (object value)
  {
    if (value == null)
      return NaInt16.Null;

    if (! (value is Int16))
      throw new ArgumentException ("Must be a Int16 value", "value");

    return new NaInt16 ((Int16) value);
  }

  /// <summary>
  /// Converts an <see cref="NaInt16"/> value to a boxed <see cref="Int16"/> value or to DBNull.Value.
  /// </summary>
  /// <returns>The integer value of <paramref name="value"/> as a <see cref="Int16"/> if it is not null, DBNull.Value otherwise.</returns>
  /// <remarks>
  /// Use this method to easily pass <see cref="NaInt16"/> values to methods that expect an untyped parameter which is either a <see cref="Int16"/>
  /// value or DBNull.Value.
  /// </remarks>
  public static object ToBoxedInt16DBNull (NaInt16 value)
  {
    if (value.IsNull )
      return DBNull.Value;
    else
      return value._value;
  }

  /// <summary>
  /// Converts a boxed <see cref="Int16"/> value or DBNull.Value to an <see cref="NaInt16"/> value.
  /// </summary>
  /// <returns>An <see cref="NaInt16"/> with its <see cref="Value"/> set to the integer value of <paramref name="value"/> if it is a <see cref="Int16"/>, 
  /// <see cref="NaInt16.Null"/> if it is DBNull.Value.</returns>
  /// <remarks>
  /// Use this method to easily create an <see cref="NaInt16"/> value from an untyped value which is either a <see cref="Int16"/> value or DBNull.Value.
  /// </remarks>
  /// <exception cref="ArgumentException"><paramref name="value"/> is neither DBNull.Value nor an <see cref="Int32"/> value.</exception>
  public static NaInt16 FromBoxedInt16DBNull (object value)
  {
    if (value == DBNull.Value)
      return NaInt16.Null;

    if (! (value is Int16))
      throw new ArgumentException ("Must be a Int16 value", "value");

    return new NaInt16 ((Int16) value);
  }

  #endregion

  #region value

  /// <summary>
  /// Gets the value of this <see cref="NaInt16"/> structure. This property is read-only.
  /// </summary>
  /// <value>
  /// An integer representing the value of this NaInt16 structure.
  /// </value>
  /// <exception cref="NaNullValueException">
  /// The property contains <see cref="Null"/>.
  /// </exception>
  public Int16 Value 
  {
    get 
    { 
      if (IsNull)
        throw NaNullValueException.AccessingMember ("Value");
      return _value;
    }
  }


  /// <summary>
  /// Gets the value of this <see cref="NaInt16"/> structure. This property is read-only.
  /// </summary>
  /// <value>
  /// An Object representing the value of this NaInt16 structure.
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
  /// Represents a null value that can be assigned to an instance of the <see cref="NaInt16"/> structure.
  /// </summary>
  /// <remarks>
  /// Null functions as a constant for the <see cref="NaInt16"/> structure.
  /// </remarks>
  public static readonly NaInt16 Null = new NaInt16 (true);

  /// <summary>
  /// Indicates whether or not <see cref="Value"/> is null.
  /// </summary>
  /// <value>
  /// This property is <see langword="true"/> if <see cref="Value"/> is null, otherwise <see langword="false"/>.
  /// </value>
  public bool IsNull 
  {
    get { return ! _isNotNull; }
  }

  #endregion

  #region public fields

  /// <summary>
  /// This value is used to convert a <see langword="null"/> value to and from strings.
  /// </summary>
  /// <value>
  /// The value of <see cref="NullString"/> is "null".
  /// </value>
  /// <remarks>
  /// Note that parsing <see cref="NullString"/> is case-sensitive.
  /// </remarks>
  public static readonly string NullString = "null";

  /// <summary>
  /// Represents a zero value that can be assigned to the <see cref="Value"/> property of 
  /// an instance of the <see cref="NaInt16"/> structure.
  /// </summary>
  /// <remarks>
  /// Zero functions as a constant for the <see cref="NaInt16"/> structure.
  /// </remarks>
  public static readonly NaInt16 Zero = new NaInt16 (0);

  /// <summary>
  /// A constant representing the largest possible value of a NaInt16.
  /// </summary>
  public static readonly NaInt16 MaxValue = new NaInt16 (Int16.MaxValue);

  /// <summary>
  /// A constant representing the smallest possible value of a NaInt16.
  /// </summary>
  public static readonly NaInt16 MinValue = new NaInt16 (Int16.MinValue);

  #endregion

  #region equality

  /// <summary>
  /// Compares the supplied object parameter to the <see cref="Value"/> property of the <see cref="NaInt16"/> object.
  /// </summary>
  /// <param name="obj">The object to be compared. </param>
  /// <returns>
  /// <see langword="true"/> if object is an instance of <see cref="NaInt16"/> and the two are equal; otherwise <see langword="false"/>.
  /// If object is a null reference, <see langword="false"/> is returned.
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// Note that <see langword="false"/> is returned if object is a Int16 (types are not equal).
  /// </remarks>
  public override bool Equals (object obj)
  {
    if (! (obj is NaInt16))
      return false; // obj is a null reference or another type then NaInt16

    return Equals (this, (NaInt16) obj);
  }

  /// <summary>
  /// Compares the supplied <see cref="NaInt16"/> parameter to the <see cref="Value"/> property of this instance.
  /// </summary>
  /// <param name="value">The <see cref="NaInt16"/> instance to be compared. </param>
  /// <returns>
  /// <see langword="true"/> if the two are equal; otherwise <see langword="false"/>.
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public bool Equals (NaInt16 value)
  {
    return Equals (this, value);
  }

  /// <summary>
  /// Performs a logical comparison of the two NaInt16 parameters to determine if they are equal.
  /// </summary>
  /// <returns>
  /// <see langword="true"/> if the two instances are equal or <see langword="false"/> if they are not equal. 
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public static bool Equals (NaInt16 x, NaInt16 y)
  {
    return x == y;
  }

  /// <summary>
  /// Performs a logical comparison of the two NaInt16 parameters to determine if they are equal.
  /// </summary>
  /// <returns>
  /// <see langword="true"/> if the two instances are equal or <see langword="false"/> if they are not equal. 
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public static bool operator == (NaInt16 x, NaInt16 y)
  {
    if (x.IsNull && y.IsNull)
      return true;
    if (x.IsNull != y.IsNull)
      return false;
    return x._value == y._value;
  }

  /// <summary>
  /// Performs a logical comparison of the two NaInt16 parameters to determine if they are not equal.
  /// </summary>
  /// <returns>
  /// <see langword="false"/> if the two instances are equal or <see langword="true"/> if they are not equal. 
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="NotEqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public static bool operator != (NaInt16 x, NaInt16 y)
  {
    return ! (x == y);
  }

  /// <summary>
  /// Performs a logical comparison of the two NaInt16 parameters to determine if they are not equal.
  /// </summary>
  /// <returns>
  /// <see langword="false"/> if the two instances are equal or <see langword="true"/> if they are not equal. 
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="NotEqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public static bool NotEquals (NaInt16 x, NaInt16 y)
  {
    return x != y;
  }

  /// <summary>
  /// Performs a SQL-style comparison of the two NaInt16 parameters to determine if they are equal.
  /// </summary>
  /// <returns>
  /// <see cref="NaBoolean.True"/> if the two instances are equal, or <see cref="NaBoolean.False"/> if they are not equal, or <see langword="null"/> if either of them is <see langword="null"/>.
  /// </returns>
  public static NaBoolean EqualsSql (NaInt16 x, NaInt16 y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;
    else 
      return new NaBoolean (x._value == y._value);
  }

  /// <summary>
  /// Performs a SQL-style comparison of the two NaInt16 parameters to determine if they are not equal.
  /// </summary>
  /// <returns>
  /// <see cref="NaBoolean.True"/> if the two instances are not equal, or <see cref="NaBoolean.False"/> if they are equal, or <see langword="null"/> if either of them is <see langword="null"/>.
  /// </returns>
  public static NaBoolean NotEqualsSql (NaInt16 x, NaInt16 y)
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

    if (! (obj is NaInt16))
      throw new ArgumentException ("obj");

    return CompareTo ((NaInt16) obj);
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
  public int CompareTo (NaInt16 value)
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
  /// Computes the sum of the two specified <see cref="NaInt16"/> structures.
  /// </summary>
  /// <returns>
  /// An <see cref="NaInt16"/> structure whose <see cref="Value"/> property contains the sum of the specified <see cref="NaInt16"/> 
  /// structures, or <see cref="Null"/> if either parameter is <see langword="null"/>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public static NaInt16 Add (NaInt16 x, NaInt16 y)
  {
    return x + y;
  }

  /// <summary>
  /// Computes the sum of the two specified <see cref="NaInt16"/> structures.
  /// </summary>
  /// <returns>
  /// An <see cref="NaInt16"/> structure whose <see cref="Value"/> property contains the sum of the specified <see cref="NaInt16"/> 
  /// structures, or <see cref="Null"/> if either parameter is <see langword="null"/>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public static NaInt16 operator + (NaInt16 x, NaInt16 y)
  {
	  if (x.IsNull || y.IsNull)
		  return NaInt16.Null;

    checked
    {
      return new NaInt16 ((Int16) (x._value + y._value));
    }
  }

  /// <summary>
  /// Subtracts the second <see cref="NaInt16"/> parameter from the first.
  /// </summary>
  /// <returns>
  /// An <see cref="NaInt16"/> structure whose <see cref="Value"/> property contains the result of the subtraction,
  /// or <see cref="Null"/> if either parameter is <see langword="null"/>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public static NaInt16 Subtract (NaInt16 x, NaInt16 y)
  {
    return x - y;
  }

  /// <summary>
  /// Subtracts the second <see cref="NaInt16"/> parameter from the first.
  /// </summary>
  /// <returns>
  /// An <see cref="NaInt16"/> structure whose <see cref="Value"/> property contains the result of the subtraction,
  /// or <see cref="Null"/> if either parameter is <see langword="null"/>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public static NaInt16 operator - (NaInt16 x, NaInt16 y)
  {
    if (x.IsNull || y.IsNull)
      return NaInt16.Null;

    checked
    {
      return new NaInt16 ((Int16) (x._value - y._value));
    }
  }

  /// <summary>
  /// Divides the first <see cref="NaInt16"/> parameter by the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaInt16"/> structure whose <see cref="Value"/> property contains the result of the division,
  /// or <see cref="Null"/> if either parameter is <see langword="null"/>.
  /// </returns>
  /// <exception cref="DivideByZeroException">A division by zero is attempted.</exception>
  public static NaInt16 Divide (NaInt16 x, NaInt16 y)
  {
    return x / y;
  }

  /// <summary>
  /// Divides the first <see cref="NaInt16"/> parameter by the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaInt16"/> structure whose <see cref="Value"/> property contains the result of the division,
  /// or <see cref="Null"/> if either parameter is <see langword="null"/>.
  /// </returns>
  /// <exception cref="DivideByZeroException">A division by zero is attempted.</exception>
  public static NaInt16 operator / (NaInt16 x, NaInt16 y)
  {
    if (x.IsNull || y.IsNull)
      return NaInt16.Null;

    checked
    {
      return new NaInt16 ((Int16) (x._value / y._value));
    }
  }

  /// <summary>
  /// Computes the product of the two <see cref="NaInt16"/> parameters.
  /// </summary>
  /// <returns>
  /// An <see cref="NaInt16"/> structure whose <see cref="Value"/> property contains the product of the two parameters,
  /// or <see cref="Null"/> if either parameter is <see langword="null"/>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public static NaInt16 Multiply (NaInt16 x, NaInt16 y)
  {
    return x * y;
  }

  /// <summary>
  /// Computes the product of the two <see cref="NaInt16"/> parameters.
  /// </summary>
  /// <returns>
  /// An <see cref="NaInt16"/> structure whose <see cref="Value"/> property contains the product of the two parameters,
  /// or <see cref="Null"/> if either parameter is <see langword="null"/>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public static NaInt16 operator * (NaInt16 x, NaInt16 y)
  {
    if (x.IsNull || y.IsNull)
      return NaInt16.Null;

    checked
    {
      return new NaInt16 ((Int16) (x._value * y._value));
    }
  }

  /// <summary>
  /// Computes the remainder after dividing the first <see cref="NaInt16"/> parameter by the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaInt16"/> structure whose <see cref="Value"/> property contains the remainder,
  /// or <see cref="Null"/> if either parameter is <see langword="null"/>.
  /// </returns>
  /// <exception cref="DivideByZeroException">A division by zero is attempted.</exception>
  public static NaInt16 Mod (NaInt16 x, NaInt16 y)
  {
    return x % y;
  }

  /// <summary>
  /// Computes the remainder after dividing the first <see cref="NaInt16"/> parameter by the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaInt16"/> structure whose <see cref="Value"/> property contains the remainder,
  /// or <see cref="Null"/> if either parameter is <see langword="null"/>.
  /// </returns>
  /// <exception cref="DivideByZeroException">A division by zero is attempted.</exception>
  public static NaInt16 operator % (NaInt16 x, NaInt16 y)
  {
    if (x.IsNull || y.IsNull)
      return NaInt16.Null;

    checked
    {
      return new NaInt16 ((Int16) (x._value % y._value));
    }
  }

  /// <summary>
  /// Increments the value by one. 
  /// </summary>
  /// <remarks>
  /// The <see cref="Value"/> property is incremented by one. If <paramref name="x"/> is <see langword="null"/>, the value is not modified.
  /// </remarks>
  /// <returns>
  /// An <see cref="NaInt16"/> structure whose <see cref="Value"/> property contains the incremented value when used as a prefix operator,
  /// or the original value when used as a postfix operator. 
  /// </returns>
  public static NaInt16 operator ++ (NaInt16 x)
  {
    if (x.IsNull) 
      return NaInt16.Null;

    ++ x._value;
    return x;
  }

  /// <summary>
  /// Decrements the value by one. 
  /// </summary>
  /// <remarks>
  /// The <see cref="Value"/> property is decremented by one. If <paramref name="x"/> is <see langword="null"/>, the value is not modified.
  /// </remarks>
  /// <returns>
  /// An <see cref="NaInt16"/> structure whose <see cref="Value"/> property contains the decremented value when used as a prefix operator,
  /// or the original value when used as a postfix operator. 
  /// </returns>
  public static NaInt16 operator -- (NaInt16 x)
  {
    if (x.IsNull) 
      return NaInt16.Null;

    -- x._value;
    return x;
  }

  #endregion

  #region relative compare operators

  /// <summary>
  /// Compares the two <see cref="NaInt16 "/> parameters to determine if the first is less than the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaBoolean"/> that is <see cref="NaBoolean.True"/> if the first parameter is less than the second, otherwise <see cref="NaBoolean.False"/>. If 
  /// either parameter is <see langword="null"/>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean LessThan (NaInt16 x, NaInt16 y)
  {
    return x < y;
  }

  /// <summary>
  /// Compares the two <see cref="NaInt16 "/> parameters to determine if the first is less than the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaBoolean"/> that is <see cref="NaBoolean.True"/> if the first parameter is less than the second, otherwise <see cref="NaBoolean.False"/>. If 
  /// either parameter is <see langword="null"/>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean operator < (NaInt16 x, NaInt16 y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;

    return new NaBoolean (x._value < y._value);
  }

  /// <summary>
  /// Compares the two <see cref="NaInt16 "/> parameters to determine if the first is less than or equal to the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaBoolean"/> that is <see cref="NaBoolean.True"/> if the first parameter is less than or equal to the second, otherwise <see cref="NaBoolean.False"/>. If 
  /// either parameter is <see langword="null"/>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean LessThanOrEqual (NaInt16 x, NaInt16 y)
  {
    return x <= y;
  }

  /// <summary>
  /// Compares the two <see cref="NaInt16 "/> parameters to determine if the first is less than or equal to the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaBoolean"/> that is <see cref="NaBoolean.True"/> if the first parameter is less than or equal to the second, otherwise <see cref="NaBoolean.False"/>. If 
  /// either parameter is <see langword="null"/>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean operator <= (NaInt16 x, NaInt16 y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;

    return new NaBoolean (x._value <= y._value);
  }

  /// <summary>
  /// Compares the two <see cref="NaInt16 "/> parameters to determine if the first is greater than the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaBoolean"/> that is <see cref="NaBoolean.True"/> if the first parameter is greater than the second, otherwise <see cref="NaBoolean.False"/>. If 
  /// either parameter is <see langword="null"/>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean GreaterThan (NaInt16 x, NaInt16 y)
  {
    return x > y;
  }

  /// <summary>
  /// Compares the two <see cref="NaInt16 "/> parameters to determine if the first is greater than the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaBoolean"/> that is <see cref="NaBoolean.True"/> if the first parameter is greater than the second, otherwise <see cref="NaBoolean.False"/>. If 
  /// either parameter is <see langword="null"/>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean operator > (NaInt16 x, NaInt16 y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;

    return new NaBoolean (x._value > y._value);
  }

  /// <summary>
  /// Compares the two <see cref="NaInt16 "/> parameters to determine if the first is greater than or equal to the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaBoolean"/> that is <see cref="NaBoolean.True"/> if the first parameter is greater than or equal to the second, otherwise <see cref="NaBoolean.False"/>. If 
  /// either parameter is <see langword="null"/>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean GreaterThanOrEqual (NaInt16 x, NaInt16 y)
  {
    return x >= y;
  }

  /// <summary>
  /// Compares the two <see cref="NaInt16 "/> parameters to determine if the first is greater than or equal to the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaBoolean"/> that is <see cref="NaBoolean.True"/> if the first parameter is greater than or equal to the second, otherwise <see cref="NaBoolean.False"/>. If 
  /// either parameter is <see langword="null"/>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean operator >= (NaInt16 x, NaInt16 y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;

    return new NaBoolean (x._value >= y._value);
  }
  
  #endregion
}

}
