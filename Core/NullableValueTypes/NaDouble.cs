using System;
using System.Runtime.Serialization;
using System.Data.SqlTypes;
using System.Globalization;

namespace Rubicon.Data.NullableValueTypes
{

/// <summary>
/// Represents a double-precision floating point number that can be <c>Null</c>. The corresponding system type is System.Double.
/// </summary>
/// <remarks>
///   <para>
///     <c>NaDouble</c> is basically a structure that can contain any double-precision floating point value or <c>Null</c>. Use 
///     <see cref="IsNull"/> to find out whether a specific structure contains <c>Null</c>, or <c>NaDouble.Null</c> to assign 
///     <c>Null</c> to a <c>NaDouble</c> variable.
///   </para>
///   <para>
///     You can use the <see cref="Value"/> property to access the floating point value, or the explicit <c>Double</c> conversion operator. Either
///     method results in a <see cref="NaNullValueException"/> if the structure is <c>Null</c>.
///   </para>
///   <para>
///     NaDouble can be used as a replacement for <c>System.Data.SqlTypes.SqlDouble</c> if you prefer the null-value semantics of <c>NaDouble</c>
///     or need serializability. Implicit conversion operators for <c>SqlDouble </c>allow <c>NaDouble</c> to be used seamlessly with ADO.NET.
///   </para>
///   <para>
///     The following null-value semantics are used for <c>NaDouble</c> structures:
///   </para>
///   <list type="table">
///     <listheader>
///       <term>Category</term>
///       <description>Semantics</description>
///     </listheader>
///     <item>
///       <term>Equality</term>
///       <description>
///         The standard equality methods and operators of <c>NaDouble</c> consider two <see cref="Null"/> values equal.
///         <para>
///           Applies to <see cref="Equals"/>, <see cref="NotEquals"/>, <see cref="operator =="/>, <see cref="operator !="/>.
///         </para>
///       </description>
///     </item>
///     <item>
///       <term>SQL-style Equality</term>
///       <description>
///         The SQL-style equality methods of <c>NaDouble</c> return <c>NaBoolean.Null</c> if either of the compared values
///         is <see cref="Null"/>. 
///         <para>
///           Applies to <see cref="EqualsSql"/>, <see cref="NotEqualsSql"/>.
///         </para>
///       </description>
///     </item>
///     <item>
///       <term>Relative Comparison</term>
///       <description>
///         The standard compare methods and operators of <c>NaDouble</c> return <c>NaBoolean.Null</c> if either of the compared values
///         is <see cref="Null"/>. 
///         <para>
///           Applies to <see cref="LessThan"/>, <see cref="LessThanOrEqual"/>, <see cref="GreaterThan"/>, <see cref="GreaterThanOrEqual"/>,
///           <see cref="operator &lt;"/>, <see cref="operator &lt;="/>, <see cref="operator &gt;"/>, <see cref="operator &gt;="/>.
///         </para>
///       </description>
///     </item>
///     <item>
///       <term>Relative Comparision using <c>CompareTo</c></term>
///       <description>
///         The CompareTo methods of <c>NaDouble</c> consider <see cref="Null"/> and null references to be less than any other value.
///         <para>
///           Applies to <see cref="CompareTo"/>.
///         </para>
///       </description>
///     </item>
///     <item>
///       <term>Arithmetics</term>
///       <description>
///         The arithmetic methods and operators of <c>NaDouble</c> return <see cref="Null"/> if eihter of their arguments are <c>Null</c>.
///         All arithmetic methods are checked, i.e. OverflowException and DivideByZeroException may be thrown where the same operation on
///         System.Double valuews would only return infinite values.
///         <para>
///           Applies to <see cref="Add"/>, <see cref="Subtract"/>, <see cref="Multiply"/>, <see cref="Divide"/>, <see cref="Mod"/>, 
///           <see cref="operator +"/>, <see cref="operator -"/>, <see cref="operator *"/>, <see cref="operator /"/>, <see cref="operator %"/>.
///         </para>
///       </description>
///     </item>
///     <item>
///       <term>Type Conversion</term>
///       <description>
///         If a <c>NaDouble</c> null-value is converted to an <c>Double</c>, a <see cref="NaNullValueException"/> is thrown. Conversions 
///         from <c>Double</c> to <c>NaDouble</c>, and conversions to and from <c>SqlDouble</c> never throw exceptions.
///       </description>
///     </item>
///     <item>
///       <term>Formatting and Parsing</term>
///       <description>
///         <para>
///           If the instance is not <c>Null</c>, <c>ToString</c> returns the same string that <c>Double.ToString</c> would return. If it is
///           <c>Null</c>, <c>ToString</c> returns the value <see cref="NullString"/> ("null"). Prefix the format string with the tilde 
///           symbol ("~") to return a
///           zero-length string for <c>Null</c>.
///         </para>
///         <para>
///           <c>Parse</c> returns <c>Null</c> if the string is a null reference, a zero-length string or <see cref="NullString"/> ("null"). 
///           Otherwise, it returns the same value that <c>Double.Parse</c> would return.
///         </para>
///       </description>
///     </item>
///   </list>
/// </remarks>
[Serializable]
public struct NaDouble: INullable, IComparable, ISerializable, IFormattable
{
  #region member fields

  private Double _value;
  private bool _isNull;

  #endregion

  #region construction and disposal

  /// <summary>
  /// Initializes a new instance of the <see cref="NaDouble"/> structure using the supplied integer value.
  /// </summary>
  /// <param name="value">The integer to be converted.</param>
  public NaDouble (Double value)
  {
    _value = value;
    _isNull = false;
  }

  private NaDouble (bool isNull)
  {
    _value = 0;
    _isNull = isNull;
  }

  #endregion

  #region serialization

  /// <summary>
  /// Serialization constructor. 
  /// </summary>
  /// <remarks>
  /// See <c>ISerializable</c> interface.
  /// </remarks>
  private NaDouble (SerializationInfo info, StreamingContext context)
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

  #endregion

  #region type conversion

  /// <summary>
  /// Converts a <see cref="NaDouble"/> structure to a <c>String</c>.
  /// </summary>
  /// <returns>
  /// A <c>String</c> object representing the <see cref="Value"/> of this instance of <c>NaDouble</c>. If this
  /// instance is <c>Null</c>, <see cref="NullString"/> ("null") is returned.
  /// </returns>
  public override string ToString()
  {
    return ToString (null, null);
  }

  /// <summary>
  /// Converts a <see cref="NaDouble"/> structure to a <c>String</c>.
  /// </summary>
  /// <param name="format">A format specification.</param>
  /// <returns>
  /// A <c>String</c> object representing the <see cref="Value"/> of this instance of <c>NaDouble</c>. If this
  /// instance is <c>Null</c>, <see cref="NullString"/> ("null") is returned. If <c>format</c> is prefixed with a tilde symbol
  /// ("~"), a zero-length string is returned instead.
  /// </returns>
  public string ToString (string format)
  {
    return ToString (format, null);
  }

  /// <summary>
  /// Converts a <see cref="NaDouble"/> structure to a <c>String</c>.
  /// </summary>
  /// <param name="provider">An <c>IFormatProvider</c> that supplies culture-specific formatting information. </param>
  /// <returns>
  /// A <c>String</c> object representing the <see cref="Value"/> of this instance of <c>NaDouble</c>. If this
  /// instance is <c>Null</c>, <see cref="NullString"/> ("null") is returned.
  /// </returns>
  public string ToString (IFormatProvider provider)
  {
    return ToString (null, provider);
  }

  /// <summary>
  /// Converts a <see cref="NaDouble"/> structure to a <c>String</c>.
  /// </summary>
  /// <param name="format">A format specification.</param>
  /// <param name="provider">An <c>IFormatProvider</c> that supplies culture-specific formatting information. </param>
  /// <returns>
  /// A <c>String</c> object representing the <see cref="Value"/> of this instance of <c>NaDouble</c>. If this
  /// instance is <c>Null</c>, <see cref="NullString"/> ("null") is returned. If <c>format</c> is prefixed with a tilde symbol
  /// ("~"), a zero-length string is returned instead.
  /// </returns>
  public string ToString (string format, IFormatProvider provider)
  {
    if (format != null && format.Length > 0 && format[0] == '~')
    {
      if (_isNull)
        return string.Empty;
      format = format.Substring (1);
    }
    else
    {
      if (_isNull)
        return NullString;
    }
    return _value.ToString (format, provider);
  }

  /// <summary>
  /// Converts the <c>String</c> representation of a number to its NaDouble equivalent.
  /// </summary>
  /// <param name="s">The <c>String</c> to be parsed.</param>
  /// <param name="styles">The combination of one or more <c>NumberStyles</c> constants that indicates the permitted format of <c>s</c>.</param>
  /// <param name="provider">An <c>IFormatProvider</c> that supplies culture-specific formatting information about <c>s</c>.</param>
  /// <returns>
  /// An <c>NaDouble</c> equivalent to the value contained in the specified string. If the string is a null reference, 
  /// a zero-length string or <see cref="NullString"/> ("null"), <c>NaDouble.Null</c> is returned. Otherwise, 
  /// <see cref="Value"/> contains the same value that <c>Double.Parse</c> would return.
  /// </returns>
  public static NaDouble Parse (string s, NumberStyles styles, IFormatProvider provider)
  {
    if (s == null || s.Length == 0 || s == NullString)
      return NaDouble.Null;
    else return new NaDouble (Double.Parse(s, styles, provider));
  }

  /// <summary>
  /// Converts the <c>String</c> representation of a number to its NaDouble equivalent.
  /// </summary>
  /// <param name="s">The <c>String</c> to be parsed.</param>
  /// <param name="provider">An <c>IFormatProvider</c> that supplies culture-specific formatting information about <c>s</c>.</param>
  /// <returns>
  /// An <c>NaDouble</c> equivalent to the value contained in the specified string. If the string is a null reference, 
  /// a zero-length string or <see cref="NullString"/> ("null"), <c>NaDouble.Null</c> is returned. Otherwise, 
  /// <see cref="Value"/> contains the same value that <c>Double.Parse</c> would return.
  /// </returns>
  public static NaDouble Parse (string s, IFormatProvider provider)
  {
    return Parse (s, NumberStyles.Integer, provider);
  }

  /// <summary>
  /// Converts the <c>String</c> representation of a number to its NaDouble equivalent.
  /// </summary>
  /// <param name="s">The <c>String</c> to be parsed.</param>
  /// <param name="styles">The combination of one or more <c>NumberStyles</c> constants that indicates the permitted format of <c>s</c>.</param>
  /// <returns>
  /// An <c>NaDouble</c> equivalent to the value contained in the specified string. If the string is a null reference, 
  /// a zero-length string or <see cref="NullString"/> ("null"), <c>NaDouble.Null</c> is returned. Otherwise, 
  /// <see cref="Value"/> contains the same value that <c>Double.Parse</c> would return.
  /// </returns>
  public static NaDouble Parse (string s, NumberStyles styles)
  {
    return Parse (s, styles, null);
  }

  /// <summary>
  /// Converts the <c>String</c> representation of a number to its NaDouble equivalent.
  /// </summary>
  /// <param name="s">The <c>String</c> to be parsed.</param>
  /// <returns>
  /// An <c>NaDouble</c> equivalent to the value contained in the specified string. If the string is a null reference, 
  /// a zero-length string or <see cref="NullString"/> ("null"), <c>NaDouble.Null</c> is returned. Otherwise, 
  /// <see cref="Value"/> contains the same value that <c>Double.Parse</c> would return.
  /// </returns>
  public static NaDouble Parse (string s)
  {
    return Parse (s, NumberStyles.Integer, null);
  }

  /// <summary>
  /// Converts the supplied <c>Double</c> to a <see cref="NaDouble"/> structure.
  /// </summary>
  /// <param name="value">An <c>Double</c> value.</param>
  /// <returns>The converted <see cref="NaDouble"/> value.</returns>
  public static implicit operator NaDouble (Double value)
  {
    return new NaDouble (value);
  }

  /// <summary>
  /// Converts the supplied <see cref="NaDouble"/> structure to an <c>Double</c>.
  /// </summary>
  /// <param name="value">A <see cref="NaDouble"/> structure.</param>
  /// <returns>The converted integer value.</returns>
  /// <exception cref="NaNullValueException">
  /// The passed value is null.
  /// </exception>
  public static explicit operator Double (NaDouble value)
  {
    if (value._isNull)
      throw NaNullValueException.AccessingMember ("NaDouble to Double Conversion");
    return value._value;
  }

  /// <summary>
  /// Converts the supplied <c>SqlDouble</c> structure to a <see cref="NaDouble"/> structure.
  /// </summary>
  public static NaDouble FromSqlInt32 (SqlDouble value)
  {
    return (NaDouble) value;
  }

  /// <summary>
  /// Converts the supplied <c>SqlDouble</c> structure to a <see cref="NaDouble"/> structure.
  /// </summary>
  public static implicit operator NaDouble (SqlDouble value)
  {
    if (value.IsNull)
      return NaDouble.Null;
    else
      return new NaDouble (value.Value);
  }

  /// <summary>
  /// Converts the supplied <see cref="NaDouble"/> structure to a <c>SqlDouble</c>structure.
  /// </summary>
  public static SqlDouble ToSqlInt32 (NaDouble value)
  {
    return (SqlDouble) value;
  }

  /// <summary>
  /// Converts the supplied <see cref="NaDouble"/> structure to a <c>SqlDouble</c>structure.
  /// </summary>
  public static implicit operator SqlDouble (NaDouble value)
  {
    if (value.IsNull)
      return SqlDouble.Null;
    else
      return new SqlDouble (value.Value);
  }

  /// <summary>
  /// Converts a <see cref="NaDouble"/> value to a boxed <c>Double</c> value or a null reference.
  /// </summary>
  /// <returns>The integer value of <c>value</c> as an <c>Double</c> if it is not null, a null reference otherwise.</returns>
  /// <remarks>
  /// Use this method to easily pass <c>NaDouble</c> values to methods that expect an untyped parameter which is either an <c>Double</c>
  /// value or a null reference.
  /// </remarks>
  public static object ToBoxedDouble (NaDouble value)
  {
    if (value._isNull )
      return null;
    else
      return value._value;
  }

  /// <summary>
  /// Converts a boxed <c>Double</c> value or a null reference to a <see cref="NaDouble"/> value.
  /// </summary>
  /// <returns>A <see cref="NaDouble"/> with its <see cref="Value"/> set to the integer value of <c>value</c> if it is an <c>Double</c>, 
  /// <c>NaDouble.Null</c> if it is a null reference.</returns>
  /// <remarks>
  /// Use this method to easily create an <c>NaDouble</c> value from an untyped value which is either an <c>Double</c> value or a null reference.
  /// </remarks>
  /// <exception cref="ArgumentException"><c>value</c> is neither a null reference nor an <c>Double</c> value.</exception>
  public static NaDouble FromBoxedDouble (object value)
  {
    if (value == null)
      return NaDouble.Null;

    if (! (value is Double))
      throw new ArgumentException ("Must be a Double value", "value");

    return new NaDouble ((Double) value);
  }

  /// <summary>
  /// Converts a <see cref="NaDouble"/> value to a boxed <c>Double</c> value or to DBNull.Value.
  /// </summary>
  /// <returns>The integer value of <c>value</c> as an <c>Double</c> if it is not null, DBNull.Value otherwise.</returns>
  /// <remarks>
  /// Use this method to easily pass <c>NaDouble</c> values to methods that expect an untyped parameter which is either an <c>Double</c>
  /// value or DBNull.Value.
  /// </remarks>
  public static object ToBoxedDoubleDBNull (NaDouble value)
  {
    if (value._isNull )
      return DBNull.Value;
    else
      return value._value;
  }

  /// <summary>
  /// Converts a boxed <c>Double</c> value or DBNull.Value to a <see cref="NaDouble"/> value.
  /// </summary>
  /// <returns>A <see cref="NaDouble"/> with its <see cref="Value"/> set to the integer value of <c>value</c> if it is an <c>Double</c>, 
  /// <c>NaDouble.Null</c> if it is DBNull.Value.</returns>
  /// <remarks>
  /// Use this method to easily create an <c>NaDouble</c> value from an untyped value which is either an <c>Double</c> value or DBNull.Value.
  /// </remarks>
  /// <exception cref="ArgumentException"><c>value</c> is neither DBNull.Value nor an <c>Double</c> value.</exception>
  public static NaDouble FromBoxedDoubleDBNull (object value)
  {
    if (value == DBNull.Value)
      return NaDouble.Null;

    if (! (value is Double))
      throw new ArgumentException ("Must be a Double value", "value");

    return new NaDouble ((Double) value);
  }

  #endregion

  #region value

  /// <summary>
  /// Gets the value of this <see cref="NaDouble"/> structure. This property is read-only.
  /// </summary>
  /// <value>
  /// An integer representing the value of this NaDouble structure.
  /// </value>
  /// <exception cref="NaNullValueException">
  /// The property contains <see cref="Null"/>.
  /// </exception>
  public Double Value 
  {
    get 
    { 
      if (_isNull)
        throw NaNullValueException.AccessingMember ("Value");
      return _value;
    }
  }

  #endregion

  #region nullable

  /// <summary>
  /// Represents a null value that can be assigned to the <see cref="Value"/> property of 
  /// an instance of the <see cref="NaDouble"/> structure.
  /// </summary>
  /// <remarks>
  /// Null functions as a constant for the <see cref="NaDouble"/> structure.
  /// </remarks>
  public static readonly NaDouble Null = new NaDouble(true);

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
  /// an instance of the <see cref="NaDouble"/> structure.
  /// </summary>
  /// <remarks>
  /// Zero functions as a constant for the <see cref="NaDouble"/> structure.
  /// </remarks>
  public static readonly NaDouble Zero = new NaDouble (0);

  /// <summary>
  /// A constant representing the largest possible value of a NaDouble.
  /// </summary>
  public static readonly NaDouble MaxValue = new NaDouble (Double.MaxValue);

  /// <summary>
  /// A constant representing the smallest possible value of a NaDouble.
  /// </summary>
  public static readonly NaDouble MinValue = new NaDouble (Double.MinValue);

  #endregion

  #region equality

  /// <summary>
  /// Compares the supplied object parameter to the <see cref="Value"/> property of the <see cref="NaDouble"/> object.
  /// </summary>
  /// <param name="obj">The object to be compared. </param>
  /// <returns>
  /// <c>true</c> if object is an instance of <see cref="NaDouble"/> and the two are equal; otherwise <c>false</c>.
  /// If object is a null reference, <c>false</c> is returned.
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// Note that <c>false</c> is returned if object is an Double (types are not equal).
  /// </remarks>
  public override bool Equals (object obj)
  {
    if (! (obj is NaDouble))
      return false; // obj is a null reference or another type then NaDouble

    return Equals (this, (NaDouble) obj);
  }

  /// <summary>
  /// Compares the supplied <see cref="NaDouble"/> parameter to the <see cref="Value"/> property of this instance.
  /// </summary>
  /// <param name="value">The <see cref="NaDouble"/> instance to be compared. </param>
  /// <returns>
  /// <c>true</c> if the two are equal; otherwise <c>false</c>.
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public bool Equals (NaDouble value)
  {
    return Equals (this, value);
  }

  /// <summary>
  /// Performs a logical comparison of the two NaDouble parameters to determine if they are equal.
  /// </summary>
  /// <returns>
  /// <c>true</c> if the two instances are equal or <c>false</c> if they are not equal. 
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public static bool Equals (NaDouble x, NaDouble y)
  {
    return x == y;
  }

  /// <summary>
  /// Performs a logical comparison of the two NaDouble parameters to determine if they are equal.
  /// </summary>
  /// <returns>
  /// <c>true</c> if the two instances are equal or <c>false</c> if they are not equal. 
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public static bool operator == (NaDouble x, NaDouble y)
  {
    if (x._isNull && y._isNull)
      return true;
    if (x._isNull != y._isNull)
      return false;
    return x._value == y._value;
  }

  /// <summary>
  /// Performs a logical comparison of the two NaDouble parameters to determine if they are not equal.
  /// </summary>
  /// <returns>
  /// <c>false</c> if the two instances are equal or <c>true</c> if they are not equal. 
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="NotEqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public static bool operator != (NaDouble x, NaDouble y)
  {
    return ! (x == y);
  }

  /// <summary>
  /// Performs a logical comparison of the two NaDouble parameters to determine if they are not equal.
  /// </summary>
  /// <returns>
  /// <c>false</c> if the two instances are equal or <c>true</c> if they are not equal. 
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="NotEqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public static bool NotEquals (NaDouble x, NaDouble y)
  {
    return x != y;
  }

  /// <summary>
  /// Performs a SQL-style comparison of the two NaDouble parameters to determine if they are equal.
  /// </summary>
  /// <returns>
  /// <c>True</c> if the two instances are equal, or <c>False</c> if they are not equal, or <c>Null</c> if either of them is <c>Null</c>.
  /// </returns>
  public static NaBoolean EqualsSql (NaDouble x, NaDouble y)
  {
    if (x._isNull || y._isNull)
      return NaBoolean.Null;
    else return new NaBoolean (x._value == y._value);
  }

  /// <summary>
  /// Performs a SQL-style comparison of the two NaDouble parameters to determine if they are not equal.
  /// </summary>
  /// <returns>
  /// <c>True</c> if the two instances are not equal, or <c>False</c> if they are equal, or <c>Null</c> if either of them is <c>Null</c>.
  /// </returns>
  public static NaBoolean NotEqualsSql (NaDouble x, NaDouble y)
  {
    if (x._isNull || y._isNull)
      return NaBoolean.Null;
    else return new NaBoolean (x._value != y._value);
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
      return _isNull ? 0 : 1;

    if (! (obj is NaDouble))
      throw new ArgumentException ("obj");

    return CompareTo ((NaDouble) obj);
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
  public int CompareTo (NaDouble value)
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

  #endregion

  #region arithmetics

  /// <summary>
  /// Computes the sum of the two specified <see cref="NaDouble"/> structures.
  /// </summary>
  /// <returns>
  /// A <c>NaDouble</c> structure whose <see cref="Value"/> property contains the sum of the specified <c>NaDouble</c> 
  /// structures, or <see cref="Null"/> if either parameter is <c>Null</c>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public static NaDouble Add (NaDouble x, NaDouble y)
  {
    return x + y;
  }

  /// <summary>
  /// Computes the sum of the two specified <see cref="NaDouble"/> structures.
  /// </summary>
  /// <returns>
  /// A <c>NaDouble</c> structure whose <see cref="Value"/> property contains the sum of the specified <c>NaDouble</c> 
  /// structures, or <see cref="Null"/> if either parameter is <c>Null</c>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public static NaDouble operator + (NaDouble x, NaDouble y)
  {
	  if (x._isNull || y._isNull)
		  return NaDouble.Null;

    Double result = x._value + y._value;
    if (Double.IsInfinity (result))
      throw new OverflowException (NaResources.ArithmeticOverflowMsg);
    return new NaDouble (result);
  }

  /// <summary>
  /// Subtracts the second <see cref="NaDouble"/> parameter from the first.
  /// </summary>
  /// <returns>
  /// A <c>NaDouble</c> structure whose <see cref="Value"/> property contains the result of the subtraction,
  /// or <see cref="Null"/> if either parameter is <c>Null</c>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public static NaDouble Subtract (NaDouble x, NaDouble y)
  {
    return x - y;
  }

  /// <summary>
  /// Subtracts the second <see cref="NaDouble"/> parameter from the first.
  /// </summary>
  /// <returns>
  /// A <c>NaDouble</c> structure whose <see cref="Value"/> property contains the result of the subtraction,
  /// or <see cref="Null"/> if either parameter is <c>Null</c>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public static NaDouble operator - (NaDouble x, NaDouble y)
  {
    if (x.IsNull || y._isNull)
      return NaDouble.Null;

    Double result = x._value - y._value;
    if (Double.IsInfinity (result))
      throw new OverflowException (NaResources.ArithmeticOverflowMsg);
    return new NaDouble (result);
  }

  /// <summary>
  /// Divides the first <see cref="NaDouble"/> parameter by the second.
  /// </summary>
  /// <returns>
  /// A <c>NaDouble</c> structure whose <see cref="Value"/> property contains the result of the division,
  /// or <see cref="Null"/> if either parameter is <c>Null</c>.
  /// </returns>
  /// <exception cref="DivideByZeroException">A division by zero is attempted.</exception>
  public static NaDouble Divide (NaDouble x, NaDouble y)
  {
    return x / y;
  }

  /// <summary>
  /// Divides the first <see cref="NaDouble"/> parameter by the second.
  /// </summary>
  /// <returns>
  /// A <c>NaDouble</c> structure whose <see cref="Value"/> property contains the result of the division,
  /// or <see cref="Null"/> if either parameter is <c>Null</c>.
  /// </returns>
  /// <exception cref="DivideByZeroException">A division by zero is attempted.</exception>
  public static NaDouble operator / (NaDouble x, NaDouble y)
  {
    if (x.IsNull || y._isNull)
      return NaDouble.Null;

    Double result = x._value / y._value;
    if (Double.IsInfinity (result))
      throw new DivideByZeroException (NaResources.DivideByZeroMsg);
    return new NaDouble (result);
  }

  /// <summary>
  /// Computes the product of the two <see cref="NaDouble"/> parameters.
  /// </summary>
  /// <returns>
  /// A <c>NaDouble</c> structure whose <see cref="Value"/> property contains the product of the two parameters,
  /// or <see cref="Null"/> if either parameter is <c>Null</c>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public static NaDouble Multiply (NaDouble x, NaDouble y)
  {
    return x * y;
  }

  /// <summary>
  /// Computes the product of the two <see cref="NaDouble"/> parameters.
  /// </summary>
  /// <returns>
  /// A <c>NaDouble</c> structure whose <see cref="Value"/> property contains the product of the two parameters,
  /// or <see cref="Null"/> if either parameter is <c>Null</c>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public static NaDouble operator * (NaDouble x, NaDouble y)
  {
    if (x.IsNull || y._isNull)
      return NaDouble.Null;

    Double result = x._value * y._value;
    if (Double.IsInfinity (result))
      throw new OverflowException (NaResources.ArithmeticOverflowMsg);
    return new NaDouble (result);
  }

  /// <summary>
  /// Computes the remainder after dividing the first <see cref="NaDouble"/> parameter by the second.
  /// </summary>
  /// <returns>
  /// A <c>NaDouble</c> structure whose <see cref="Value"/> property contains the remainder,
  /// or <see cref="Null"/> if either parameter is <c>Null</c>.
  /// </returns>
  /// <exception cref="DivideByZeroException">A division by zero is attempted.</exception>
  public static NaDouble Mod (NaDouble x, NaDouble y)
  {
    return x % y;
  }

  /// <summary>
  /// Computes the remainder after dividing the first <see cref="NaDouble"/> parameter by the second.
  /// </summary>
  /// <returns>
  /// A <c>NaDouble</c> structure whose <see cref="Value"/> property contains the remainder,
  /// or <see cref="Null"/> if either parameter is <c>Null</c>.
  /// </returns>
  /// <exception cref="DivideByZeroException">A division by zero is attempted.</exception>
  public static NaDouble operator % (NaDouble x, NaDouble y)
  {
    if (x.IsNull || y._isNull)
      return NaDouble.Null;

    Double result = x._value % y._value;
    if (Double.IsNaN (result))
      throw new DivideByZeroException (NaResources.DivideByZeroMsg);
    return new NaDouble (result);
  }

  #endregion

  #region relative compare operators

  /// <summary>
  /// Compares the two <see cref="NaDouble "/> parameters to determine if the first is less than the second.
  /// </summary>
  /// <returns>
  /// A <see cref="NaBoolean"/> that is <c>True</c> if the first parameter is less than the second, otherwise <c>False</c>. If 
  /// either parameter is <c>Null</c>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean LessThan (NaDouble x, NaDouble y)
  {
    return x < y;
  }

  /// <summary>
  /// Compares the two <see cref="NaDouble "/> parameters to determine if the first is less than the second.
  /// </summary>
  /// <returns>
  /// A <see cref="NaBoolean"/> that is <c>True</c> if the first parameter is less than the second, otherwise <c>False</c>. If 
  /// either parameter is <c>Null</c>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean operator < (NaDouble x, NaDouble y)
  {
    if (x.IsNull || y._isNull)
      return NaBoolean.Null;

    return new NaBoolean (x._value < y._value);
  }

  /// <summary>
  /// Compares the two <see cref="NaDouble "/> parameters to determine if the first is less than or equal to the second.
  /// </summary>
  /// <returns>
  /// A <see cref="NaBoolean"/> that is <c>True</c> if the first parameter is less than or equal to the second, otherwise <c>False</c>. If 
  /// either parameter is <c>Null</c>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean LessThanOrEqual (NaDouble x, NaDouble y)
  {
    return x <= y;
  }

  /// <summary>
  /// Compares the two <see cref="NaDouble "/> parameters to determine if the first is less than or equal to the second.
  /// </summary>
  /// <returns>
  /// A <see cref="NaBoolean"/> that is <c>True</c> if the first parameter is less than or equal to the second, otherwise <c>False</c>. If 
  /// either parameter is <c>Null</c>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean operator <= (NaDouble x, NaDouble y)
  {
    if (x.IsNull || y._isNull)
      return NaBoolean.Null;

    return new NaBoolean (x._value <= y._value);
  }

  /// <summary>
  /// Compares the two <see cref="NaDouble "/> parameters to determine if the first is greater than the second.
  /// </summary>
  /// <returns>
  /// A <see cref="NaBoolean"/> that is <c>True</c> if the first parameter is greater than the second, otherwise <c>False</c>. If 
  /// either parameter is <c>Null</c>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean GreaterThan (NaDouble x, NaDouble y)
  {
    return x > y;
  }

  /// <summary>
  /// Compares the two <see cref="NaDouble "/> parameters to determine if the first is greater than the second.
  /// </summary>
  /// <returns>
  /// A <see cref="NaBoolean"/> that is <c>True</c> if the first parameter is greater than the second, otherwise <c>False</c>. If 
  /// either parameter is <c>Null</c>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean operator > (NaDouble x, NaDouble y)
  {
    if (x.IsNull || y._isNull)
      return NaBoolean.Null;

    return new NaBoolean (x._value > y._value);
  }

  /// <summary>
  /// Compares the two <see cref="NaDouble "/> parameters to determine if the first is greater than or equal to the second.
  /// </summary>
  /// <returns>
  /// A <see cref="NaBoolean"/> that is <c>True</c> if the first parameter is greater than or equal to the second, otherwise <c>False</c>. If 
  /// either parameter is <c>Null</c>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean GreaterThanOrEqual (NaDouble x, NaDouble y)
  {
    return x >= y;
  }

  /// <summary>
  /// Compares the two <see cref="NaDouble "/> parameters to determine if the first is greater than or equal to the second.
  /// </summary>
  /// <returns>
  /// A <see cref="NaBoolean"/> that is <c>True</c> if the first parameter is greater than or equal to the second, otherwise <c>False</c>. If 
  /// either parameter is <c>Null</c>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean operator >= (NaDouble x, NaDouble y)
  {
    if (x.IsNull || y._isNull)
      return NaBoolean.Null;

    return new NaBoolean (x._value >= y._value);
  }
  
  #endregion
}


}
