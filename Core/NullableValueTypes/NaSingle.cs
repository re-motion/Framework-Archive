using System;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Remotion.Utilities;

namespace Remotion.NullableValueTypes
{

/// <summary>
/// Represents a single-precision floating point number that can be <see langword="null"/>. The corresponding system type is System.Single.
/// </summary>
/// <include file='doc\include\NullableValueTypes\include.xml' path='Comments/NaSingle/remarks' />
[Serializable]
[NaBasicType (typeof (Single))]
[TypeConverter (typeof (NaSingleConverter))]
public struct NaSingle: INaNullable, IComparable, IFormattable, IXmlSerializable
{
  #region member fields

  private float _value;
  private bool _isNotNull;

  #endregion

  #region construction and disposal

  /// <summary>
  /// Initializes a new instance of the <see cref="NaSingle"/> structure using the supplied integer value.
  /// </summary>
  /// <param name="value">The single to be converted.</param>
  public NaSingle (Single value)
  {
    _value = value;
    _isNotNull = true;
  }

  private NaSingle (bool isNull)
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
      lock (typeof (NaSingle))
      {
        if (s_schema == null)
          s_schema = NaTypeUtility.CreateXmlSchema (typeof(NaSingle).Name, "float");
      }
    }
    return s_schema;
  }

  void IXmlSerializable.ReadXml (XmlReader reader)
  {
    string strValue = NaTypeUtility.ReadXml (reader);
    _isNotNull = strValue != null;
    if (strValue != null)
      _value = XmlConvert.ToSingle (strValue);
  }

  void IXmlSerializable.WriteXml (XmlWriter writer)
  {
    NaTypeUtility.WriteXml (writer, IsNull ? null : XmlConvert.ToString (_value));
  }

  #endregion

  #region type conversion

  /// <summary>
  /// Converts an <see cref="NaSingle"/> structure to a <see cref="String"/>.
  /// </summary>
  /// <returns>
  /// A <see cref="String"/> object representing the <see cref="Value"/> of this instance of <see cref="NaSingle"/>. If this
  /// instance is <see langword="null"/>, <see cref="NullString"/> ("null") is returned.
  /// </returns>
  public override string ToString()
  {
    return ToString (null, null);
  }

  /// <summary>
  /// Converts an <see cref="NaSingle"/> structure to a <see cref="String"/>.
  /// </summary>
  /// <param name="format">A format specification.</param>
  /// <returns>
  /// A <see cref="String"/> object representing the <see cref="Value"/> of this instance of <see cref="NaSingle"/>. If this
  /// instance is <see langword="null"/>, <see cref="NullString"/> ("null") is returned. If <paramref name="format"/> is prefixed with a tilde symbol
  /// ("~"), a zero-length string is returned instead.
  /// </returns>
  public string ToString (string format)
  {
    return ToString (format, null);
  }

  /// <summary>
  /// Converts an <see cref="NaSingle"/> structure to a <see cref="String"/>.
  /// </summary>
  /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information. </param>
  /// <returns>
  /// A <see cref="String"/> object representing the <see cref="Value"/> of this instance of <see cref="NaSingle"/>. If this
  /// instance is <see langword="null"/>, <see cref="NullString"/> ("null") is returned.
  /// </returns>
  public string ToString (IFormatProvider provider)
  {
    return ToString (null, provider);
  }

  /// <summary>
  /// Converts an <see cref="NaSingle"/> structure to a <see cref="String"/>.
  /// </summary>
  /// <param name="format">A format specification.</param>
  /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information. </param>
  /// <returns>
  /// A <see cref="String"/> object representing the <see cref="Value"/> of this instance of <see cref="NaSingle"/>. If this
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
  /// Converts the <see cref="String"/> representation of a number to its NaSingle equivalent.
  /// </summary>
  /// <param name="s">The <see cref="String"/> to be parsed.</param>
  /// <param name="styles">The combination of one or more <see cref="NumberStyles"/> constants that indicates the permitted format of <paramref name="s"/>.</param>
  /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information about <paramref name="s"/>.</param>
  /// <returns>
  /// An <see cref="NaSingle"/> equivalent to the value contained in the specified string. If the string is a null reference, 
  /// a zero-length string or <see cref="NullString"/> ("null"), <see cref="NaSingle.Null"/> is returned. Otherwise, 
  /// <see cref="Value"/> contains the same value that <see cref="Single.Parse"/> would return.
  /// </returns>
  public static NaSingle Parse (string s, NumberStyles styles, IFormatProvider provider)
  {
    if (s == null || s.Length == 0 || s == NullString)
      return NaSingle.Null;
    else return new NaSingle (Single.Parse(s, styles, provider));
  }

  /// <summary>
  /// Converts the <see cref="String"/> representation of a number to its NaSingle equivalent.
  /// </summary>
  /// <param name="s">The <see cref="String"/> to be parsed.</param>
  /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information about <paramref name="s"/>.</param>
  /// <returns>
  /// An <see cref="NaSingle"/> equivalent to the value contained in the specified string. If the string is a null reference, 
  /// a zero-length string or <see cref="NullString"/> ("null"), <see cref="NaSingle.Null"/> is returned. Otherwise, 
  /// <see cref="Value"/> contains the same value that <see cref="Single.Parse"/> would return.
  /// </returns>
  public static NaSingle Parse (string s, IFormatProvider provider)
  {
    return Parse (s, NumberStyles.Float | NumberStyles.AllowThousands, provider);
  }

  /// <summary>
  /// Converts the <see cref="String"/> representation of a number to its NaSingle equivalent.
  /// </summary>
  /// <param name="s">The <see cref="String"/> to be parsed.</param>
  /// <param name="styles">The combination of one or more <see cref="NumberStyles"/> constants that indicates the permitted format of <paramref name="s"/>.</param>
  /// <returns>
  /// An <see cref="NaSingle"/> equivalent to the value contained in the specified string. If the string is a null reference, 
  /// a zero-length string or <see cref="NullString"/> ("null"), <see cref="NaSingle.Null"/> is returned. Otherwise, 
  /// <see cref="Value"/> contains the same value that <see cref="Single.Parse"/> would return.
  /// </returns>
  public static NaSingle Parse (string s, NumberStyles styles)
  {
    return Parse (s, styles, null);
  }

  /// <summary>
  /// Converts the <see cref="String"/> representation of a number to its NaSingle equivalent.
  /// </summary>
  /// <param name="s">The <see cref="String"/> to be parsed.</param>
  /// <returns>
  /// An <see cref="NaSingle"/> equivalent to the value contained in the specified string. If the string is a null reference, 
  /// a zero-length string or <see cref="NullString"/> ("null"), <see cref="NaSingle.Null"/> is returned. Otherwise, 
  /// <see cref="Value"/> contains the same value that <see cref="Single.Parse"/> would return.
  /// </returns>
  public static NaSingle Parse (string s)
  {
    return Parse (s, NumberStyles.Float | NumberStyles.AllowThousands, null);
  }

  /// <summary>
  /// Converts the supplied <see cref="Single"/> to an <see cref="NaSingle"/> structure.
  /// </summary>
  /// <param name="value">A <see cref="Single"/> value.</param>
  /// <returns>The converted <see cref="NaSingle"/> value.</returns>
  public static implicit operator NaSingle (Single value)
  {
    return new NaSingle (value);
  }

  /// <summary>
  /// Converts the supplied <see cref="NaSingle"/> structure to a <see cref="Single"/>.
  /// </summary>
  /// <param name="value">An <see cref="NaSingle"/> structure.</param>
  /// <returns>The converted single value.</returns>
  /// <exception cref="NaNullValueException">
  /// The passed value is null.
  /// </exception>
  public static explicit operator Single (NaSingle value)
  {
    if (value.IsNull)
      throw NaNullValueException.AccessingMember ("NaSingle to Single Conversion");
    return value._value;
  }

  /// <summary>
  /// Converts the supplied <see cref="SqlSingle"/> structure to an <see cref="NaSingle"/> structure.
  /// </summary>
  public static NaSingle FromSqlSingle (SqlSingle value)
  {
    return (NaSingle) value;
  }

  /// <summary>
  /// Converts the supplied <see cref="SqlSingle"/> structure to an <see cref="NaSingle"/> structure.
  /// </summary>
  public static implicit operator NaSingle (SqlSingle value)
  {
    if (value.IsNull)
      return NaSingle.Null;
    else
      return new NaSingle (value.Value);
  }

  /// <summary>
  /// Converts the supplied <see cref="NaSingle"/> structure to a <see cref="SqlSingle"/>structure.
  /// </summary>
  public static SqlSingle ToSqlSingle (NaSingle value)
  {
    return (SqlSingle) value;
  }

  /// <summary>
  /// Converts the supplied <see cref="NaSingle"/> structure to a <see cref="SqlSingle"/>structure.
  /// </summary>
  public static implicit operator SqlSingle (NaSingle value)
  {
    if (value.IsNull)
      return SqlSingle.Null;
    else
      return new SqlSingle (value.Value);
  }

  /// <summary>
  /// Converts an <see cref="NaSingle"/> value to a boxed <see cref="Single"/> value or a null reference.
  /// </summary>
  /// <returns>The integer value of <paramref name="value"/> as an <see cref="Single"/> if it is not null, a null reference otherwise.</returns>
  /// <remarks>
  /// Use this method to easily pass <see cref="NaSingle"/> values to methods that expect an untyped parameter which is either a <see cref="Single"/>
  /// value or a null reference.
  /// </remarks>
  public static object ToBoxedSingle (NaSingle value)
  {
    if (value.IsNull )
      return null;
    else
      return value._value;
  }

  /// <summary>
  /// Converts a boxed <see cref="NaSingle"/> value or a null reference to an <see cref="NaSingle"/> value.
  /// </summary>
  /// <returns>An <see cref="NaSingle"/> with its <see cref="Value"/> set to the integer value of <paramref name="value"/> if it is an <see cref="Single"/>, 
  /// <see cref="NaSingle.Null"/> if it is a null reference.</returns>
  /// <remarks>
  /// Use this method to easily create an <see cref="NaSingle"/> value from an untyped value which is either an <see cref="Single"/> value or a null reference.
  /// </remarks>
  /// <exception cref="ArgumentException"><paramref name="value"/> is neither a null reference nor an <see cref="Single"/> value.</exception>
  public static NaSingle FromBoxedSingle (object value)
  {
    if (value == null)
      return NaSingle.Null;

    if (! (value is Single))
      throw new ArgumentException ("Must be a Single value", "value");

    return new NaSingle ((Single) value);
  }

  /// <summary>
  /// Converts an <see cref="NaSingle"/> value to a boxed <see cref="Single"/> value or to DBNull.Value.
  /// </summary>
  /// <returns>The integer value of <paramref name="value"/> as a <see cref="Single"/> if it is not null, DBNull.Value otherwise.</returns>
  /// <remarks>
  /// Use this method to easily pass <see cref="NaSingle"/> values to methods that expect an untyped parameter which is either a <see cref="Single"/>
  /// value or DBNull.Value.
  /// </remarks>
  public static object ToBoxedSingleDBNull (NaSingle value)
  {
    if (value.IsNull )
      return DBNull.Value;
    else
      return value._value;
  }

  /// <summary>
  /// Converts a boxed <see cref="Single"/> value or DBNull.Value to an <see cref="NaSingle"/> value.
  /// </summary>
  /// <returns>An <see cref="NaSingle"/> with its <see cref="Value"/> set to the integer value of <paramref name="value"/> if it is a <see cref="Single"/>, 
  /// <see cref="NaSingle.Null"/> if it is DBNull.Value.</returns>
  /// <remarks>
  /// Use this method to easily create an <see cref="NaSingle"/> value from an untyped value which is either a <see cref="Single"/> value or DBNull.Value.
  /// </remarks>
  /// <exception cref="ArgumentException"><paramref name="value"/> is neither DBNull.Value nor an <see cref="Int32"/> value.</exception>
  public static NaSingle FromBoxedSingleDBNull (object value)
  {
    if (value == DBNull.Value)
      return NaSingle.Null;

    if (! (value is Single))
      throw new ArgumentException ("Must be a Single value", "value");

    return new NaSingle ((Single) value);
  }

  #endregion

  #region value

  /// <summary>
  /// Gets the value of this <see cref="NaSingle"/> structure. This property is read-only.
  /// </summary>
  /// <value>
  /// An integer representing the value of this NaSingle structure.
  /// </value>
  /// <exception cref="NaNullValueException">
  /// The property contains <see cref="Null"/>.
  /// </exception>
  public Single Value 
  {
    get 
    { 
      if (IsNull)
        throw NaNullValueException.AccessingMember ("Value");
      return _value;
    }
  }


  /// <summary>
  /// Gets the value of this <see cref="NaSingle"/> structure. This property is read-only.
  /// </summary>
  /// <value>
  /// An Object representing the value of this NaSingle structure.
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
  /// Represents a null value that can be assigned to an instance of the <see cref="NaSingle"/> structure.
  /// </summary>
  /// <remarks>
  /// Null functions as a constant for the <see cref="NaSingle"/> structure.
  /// </remarks>
  public static readonly NaSingle Null = new NaSingle (true);

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
  /// an instance of the <see cref="NaSingle"/> structure.
  /// </summary>
  /// <remarks>
  /// Zero functions as a constant for the <see cref="NaSingle"/> structure.
  /// </remarks>
  public static readonly NaSingle Zero = new NaSingle (0);

  /// <summary>
  /// A constant representing the largest possible value of a NaSingle.
  /// </summary>
  public static readonly NaSingle MaxValue = new NaSingle (Single.MaxValue);

  /// <summary>
  /// A constant representing the smallest possible value of a NaSingle.
  /// </summary>
  public static readonly NaSingle MinValue = new NaSingle (Single.MinValue);

  #endregion

  #region equality

  /// <summary>
  /// Compares the supplied object parameter to the <see cref="Value"/> property of the <see cref="NaSingle"/> object.
  /// </summary>
  /// <param name="obj">The object to be compared. </param>
  /// <returns>
  /// <see langword="true"/> if object is an instance of <see cref="NaSingle"/> and the two are equal; otherwise <see langword="false"/>.
  /// If object is a null reference, <see langword="false"/> is returned.
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// Note that <see langword="false"/> is returned if object is a Single (types are not equal).
  /// </remarks>
  public override bool Equals (object obj)
  {
    if (! (obj is NaSingle))
      return false; // obj is a null reference or another type then NaSingle

    return Equals (this, (NaSingle) obj);
  }

  /// <summary>
  /// Compares the supplied <see cref="NaSingle"/> parameter to the <see cref="Value"/> property of this instance.
  /// </summary>
  /// <param name="value">The <see cref="NaSingle"/> instance to be compared. </param>
  /// <returns>
  /// <see langword="true"/> if the two are equal; otherwise <see langword="false"/>.
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public bool Equals (NaSingle value)
  {
    return Equals (this, value);
  }

  /// <summary>
  /// Performs a logical comparison of the two NaSingle parameters to determine if they are equal.
  /// </summary>
  /// <returns>
  /// <see langword="true"/> if the two instances are equal or <see langword="false"/> if they are not equal. 
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public static bool Equals (NaSingle x, NaSingle y)
  {
    return x == y;
  }

  /// <summary>
  /// Performs a logical comparison of the two NaSingle parameters to determine if they are equal.
  /// </summary>
  /// <returns>
  /// <see langword="true"/> if the two instances are equal or <see langword="false"/> if they are not equal. 
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public static bool operator == (NaSingle x, NaSingle y)
  {
    if (x.IsNull && y.IsNull)
      return true;
    if (x.IsNull != y.IsNull)
      return false;
    return x._value == y._value;
  }

  /// <summary>
  /// Performs a logical comparison of the two NaSingle parameters to determine if they are not equal.
  /// </summary>
  /// <returns>
  /// <see langword="false"/> if the two instances are equal or <see langword="true"/> if they are not equal. 
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="NotEqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public static bool operator != (NaSingle x, NaSingle y)
  {
    return ! (x == y);
  }

  /// <summary>
  /// Performs a logical comparison of the two NaSingle parameters to determine if they are not equal.
  /// </summary>
  /// <returns>
  /// <see langword="false"/> if the two instances are equal or <see langword="true"/> if they are not equal. 
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="NotEqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public static bool NotEquals (NaSingle x, NaSingle y)
  {
    return x != y;
  }

  /// <summary>
  /// Performs a SQL-style comparison of the two NaSingle parameters to determine if they are equal.
  /// </summary>
  /// <returns>
  /// <see cref="NaBoolean.True"/> if the two instances are equal, or <see cref="NaBoolean.False"/> if they are not equal, or <see langword="null"/> if either of them is <see langword="null"/>.
  /// </returns>
  public static NaBoolean EqualsSql (NaSingle x, NaSingle y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;
    else 
      return new NaBoolean (x._value == y._value);
  }

  /// <summary>
  /// Performs a SQL-style comparison of the two NaSingle parameters to determine if they are not equal.
  /// </summary>
  /// <returns>
  /// <see cref="NaBoolean.True"/> if the two instances are not equal, or <see cref="NaBoolean.False"/> if they are equal, or <see langword="null"/> if either of them is <see langword="null"/>.
  /// </returns>
  public static NaBoolean NotEqualsSql (NaSingle x, NaSingle y)
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

    if (! (obj is NaSingle))
      throw new ArgumentException ("obj");

    return CompareTo ((NaSingle) obj);
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
  public int CompareTo (NaSingle value)
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
  /// Computes the sum of the two specified <see cref="NaSingle"/> structures.
  /// </summary>
  /// <returns>
  /// An <see cref="NaSingle"/> structure whose <see cref="Value"/> property contains the sum of the specified <see cref="NaSingle"/> 
  /// structures, or <see cref="Null"/> if either parameter is <see langword="null"/>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public static NaSingle Add (NaSingle x, NaSingle y)
  {
    return x + y;
  }

  /// <summary>
  /// Computes the sum of the two specified <see cref="NaSingle"/> structures.
  /// </summary>
  /// <returns>
  /// An <see cref="NaSingle"/> structure whose <see cref="Value"/> property contains the sum of the specified <see cref="NaSingle"/> 
  /// structures, or <see cref="Null"/> if either parameter is <see langword="null"/>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public static NaSingle operator + (NaSingle x, NaSingle y)
  {
	  if (x.IsNull || y.IsNull)
		  return NaSingle.Null;

    checked
    {
      return new NaSingle (x._value + y._value);
    }
  }

  /// <summary>
  /// Subtracts the second <see cref="NaSingle"/> parameter from the first.
  /// </summary>
  /// <returns>
  /// An <see cref="NaSingle"/> structure whose <see cref="Value"/> property contains the result of the subtraction,
  /// or <see cref="Null"/> if either parameter is <see langword="null"/>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public static NaSingle Subtract (NaSingle x, NaSingle y)
  {
    return x - y;
  }

  /// <summary>
  /// Subtracts the second <see cref="NaSingle"/> parameter from the first.
  /// </summary>
  /// <returns>
  /// An <see cref="NaSingle"/> structure whose <see cref="Value"/> property contains the result of the subtraction,
  /// or <see cref="Null"/> if either parameter is <see langword="null"/>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public static NaSingle operator - (NaSingle x, NaSingle y)
  {
    if (x.IsNull || y.IsNull)
      return NaSingle.Null;

    checked
    {
      return new NaSingle (x._value - y._value);
    }
  }

  /// <summary>
  /// Divides the first <see cref="NaSingle"/> parameter by the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaSingle"/> structure whose <see cref="Value"/> property contains the result of the division,
  /// or <see cref="Null"/> if either parameter is <see langword="null"/>.
  /// </returns>
  /// <exception cref="DivideByZeroException">A division by zero is attempted.</exception>
  public static NaSingle Divide (NaSingle x, NaSingle y)
  {
    return x / y;
  }

  /// <summary>
  /// Divides the first <see cref="NaSingle"/> parameter by the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaSingle"/> structure whose <see cref="Value"/> property contains the result of the division,
  /// or <see cref="Null"/> if either parameter is <see langword="null"/>.
  /// </returns>
  /// <exception cref="DivideByZeroException">A division by zero is attempted.</exception>
  public static NaSingle operator / (NaSingle x, NaSingle y)
  {
    if (x.IsNull || y.IsNull)
      return NaSingle.Null;

    checked
    {
      return new NaSingle (x._value / y._value);
    }
  }

  /// <summary>
  /// Computes the product of the two <see cref="NaSingle"/> parameters.
  /// </summary>
  /// <returns>
  /// An <see cref="NaSingle"/> structure whose <see cref="Value"/> property contains the product of the two parameters,
  /// or <see cref="Null"/> if either parameter is <see langword="null"/>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public static NaSingle Multiply (NaSingle x, NaSingle y)
  {
    return x * y;
  }

  /// <summary>
  /// Computes the product of the two <see cref="NaSingle"/> parameters.
  /// </summary>
  /// <returns>
  /// An <see cref="NaSingle"/> structure whose <see cref="Value"/> property contains the product of the two parameters,
  /// or <see cref="Null"/> if either parameter is <see langword="null"/>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public static NaSingle operator * (NaSingle x, NaSingle y)
  {
    if (x.IsNull || y.IsNull)
      return NaSingle.Null;

    checked
    {
      return new NaSingle (x._value * y._value);
    }
  }

  /// <summary>
  /// Computes the remainder after dividing the first <see cref="NaSingle"/> parameter by the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaSingle"/> structure whose <see cref="Value"/> property contains the remainder,
  /// or <see cref="Null"/> if either parameter is <see langword="null"/>.
  /// </returns>
  /// <exception cref="DivideByZeroException">A division by zero is attempted.</exception>
  public static NaSingle Mod (NaSingle x, NaSingle y)
  {
    return x % y;
  }

  /// <summary>
  /// Computes the remainder after dividing the first <see cref="NaSingle"/> parameter by the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaSingle"/> structure whose <see cref="Value"/> property contains the remainder,
  /// or <see cref="Null"/> if either parameter is <see langword="null"/>.
  /// </returns>
  /// <exception cref="DivideByZeroException">A division by zero is attempted.</exception>
  public static NaSingle operator % (NaSingle x, NaSingle y)
  {
    if (x.IsNull || y.IsNull)
      return NaSingle.Null;

    checked
    {
      return new NaSingle (x._value % y._value);
    }
  }

  /// <summary>
  /// Increments the value by one. 
  /// </summary>
  /// <remarks>
  /// The <see cref="Value"/> property is incremented by one. If <paramref name="x"/> is <see langword="null"/>, the value is not modified.
  /// </remarks>
  /// <returns>
  /// An <see cref="NaSingle"/> structure whose <see cref="Value"/> property contains the incremented value when used as a prefix operator,
  /// or the original value when used as a postfix operator. 
  /// </returns>
  public static NaSingle operator ++ (NaSingle x)
  {
    if (x.IsNull) 
      return NaSingle.Null;

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
  /// An <see cref="NaSingle"/> structure whose <see cref="Value"/> property contains the decremented value when used as a prefix operator,
  /// or the original value when used as a postfix operator. 
  /// </returns>
  public static NaSingle operator -- (NaSingle x)
  {
    if (x.IsNull) 
      return NaSingle.Null;

    -- x._value;
    return x;
  }

  #endregion

  #region relative compare operators

  /// <summary>
  /// Compares the two <see cref="NaSingle "/> parameters to determine if the first is less than the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaBoolean"/> that is <see cref="NaBoolean.True"/> if the first parameter is less than the second, otherwise <see cref="NaBoolean.False"/>. If 
  /// either parameter is <see langword="null"/>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean LessThan (NaSingle x, NaSingle y)
  {
    return x < y;
  }

  /// <summary>
  /// Compares the two <see cref="NaSingle "/> parameters to determine if the first is less than the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaBoolean"/> that is <see cref="NaBoolean.True"/> if the first parameter is less than the second, otherwise <see cref="NaBoolean.False"/>. If 
  /// either parameter is <see langword="null"/>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean operator < (NaSingle x, NaSingle y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;

    return new NaBoolean (x._value < y._value);
  }

  /// <summary>
  /// Compares the two <see cref="NaSingle "/> parameters to determine if the first is less than or equal to the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaBoolean"/> that is <see cref="NaBoolean.True"/> if the first parameter is less than or equal to the second, otherwise <see cref="NaBoolean.False"/>. If 
  /// either parameter is <see langword="null"/>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean LessThanOrEqual (NaSingle x, NaSingle y)
  {
    return x <= y;
  }

  /// <summary>
  /// Compares the two <see cref="NaSingle "/> parameters to determine if the first is less than or equal to the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaBoolean"/> that is <see cref="NaBoolean.True"/> if the first parameter is less than or equal to the second, otherwise <see cref="NaBoolean.False"/>. If 
  /// either parameter is <see langword="null"/>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean operator <= (NaSingle x, NaSingle y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;

    return new NaBoolean (x._value <= y._value);
  }

  /// <summary>
  /// Compares the two <see cref="NaSingle "/> parameters to determine if the first is greater than the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaBoolean"/> that is <see cref="NaBoolean.True"/> if the first parameter is greater than the second, otherwise <see cref="NaBoolean.False"/>. If 
  /// either parameter is <see langword="null"/>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean GreaterThan (NaSingle x, NaSingle y)
  {
    return x > y;
  }

  /// <summary>
  /// Compares the two <see cref="NaSingle "/> parameters to determine if the first is greater than the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaBoolean"/> that is <see cref="NaBoolean.True"/> if the first parameter is greater than the second, otherwise <see cref="NaBoolean.False"/>. If 
  /// either parameter is <see langword="null"/>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean operator > (NaSingle x, NaSingle y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;

    return new NaBoolean (x._value > y._value);
  }

  /// <summary>
  /// Compares the two <see cref="NaSingle "/> parameters to determine if the first is greater than or equal to the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaBoolean"/> that is <see cref="NaBoolean.True"/> if the first parameter is greater than or equal to the second, otherwise <see cref="NaBoolean.False"/>. If 
  /// either parameter is <see langword="null"/>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean GreaterThanOrEqual (NaSingle x, NaSingle y)
  {
    return x >= y;
  }

  /// <summary>
  /// Compares the two <see cref="NaSingle "/> parameters to determine if the first is greater than or equal to the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaBoolean"/> that is <see cref="NaBoolean.True"/> if the first parameter is greater than or equal to the second, otherwise <see cref="NaBoolean.False"/>. If 
  /// either parameter is <see langword="null"/>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean operator >= (NaSingle x, NaSingle y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;

    return new NaBoolean (x._value >= y._value);
  }
  
  #endregion
}

}
