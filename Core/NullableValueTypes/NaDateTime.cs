using System;
using System.Runtime.Serialization;
using System.Data.SqlTypes;
using System.Globalization;

namespace Rubicon.Data.NullableValueTypes
{

/// <summary>
/// Represents date and time data that can be null. The corresponding system type is System.DateTime. 
/// </summary>
/// <include file='doc\include\include.xml' path='Comments/NaDateTime/remarks' />
[Serializable]
[NaBasicType (typeof(DateTime))]
public struct NaDateTime: INaNullable, IComparable, ISerializable, IFormattable
{
  #region member fields

  private DateTime _value;
  private bool _isNotNull;

  #endregion

  #region construction and disposal

  /// <summary>
  /// Initializes a new instance of the <see cref="NaDateTime"/> structure using the supplied integer value.
  /// </summary>
  /// <param name="value">The integer to be converted.</param>
  public NaDateTime (DateTime value)
  {
    _value = value;
    _isNotNull = true;
  }

  private NaDateTime (bool isNull)
  {
    _value = new DateTime (0);
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
  private NaDateTime (SerializationInfo info, StreamingContext context)
  {
    _isNotNull = ! info.GetBoolean ("IsNull");
    _value = info.GetDateTime ("Value");
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

  #endregion

  #region type conversion

  /// <summary>
  /// Converts a <see cref="NaDateTime"/> structure to a <c>String</c>.
  /// </summary>
  /// <returns>
  /// A <c>String</c> object representing the <see cref="Value"/> of this instance of <c>NaDateTime</c>. If this
  /// instance is <c>Null</c>, <see cref="NullString"/> ("null") is returned.
  /// </returns>
  public override string ToString()
  {
    return ToString (null, null);
  }

  /// <summary>
  /// Converts a <see cref="NaDateTime"/> structure to a <c>String</c>.
  /// </summary>
  /// <param name="format">A format specification.</param>
  /// <returns>
  /// A <c>String</c> object representing the <see cref="Value"/> of this instance of <c>NaDateTime</c>. If this
  /// instance is <c>Null</c>, <see cref="NullString"/> ("null") is returned. If <c>format</c> is prefixed with a tilde symbol
  /// ("~"), a zero-length string is returned instead.
  /// </returns>
  public string ToString (string format)
  {
    return ToString (format, null);
  }

  /// <summary>
  /// Converts a <see cref="NaDateTime"/> structure to a <c>String</c>.
  /// </summary>
  /// <param name="provider">An <c>IFormatProvider</c> that supplies culture-specific formatting information. </param>
  /// <returns>
  /// A <c>String</c> object representing the <see cref="Value"/> of this instance of <c>NaDateTime</c>. If this
  /// instance is <c>Null</c>, <see cref="NullString"/> ("null") is returned.
  /// </returns>
  public string ToString (IFormatProvider provider)
  {
    return ToString (null, provider);
  }

  /// <summary>
  /// Converts a <see cref="NaDateTime"/> structure to a <c>String</c>.
  /// </summary>
  /// <param name="format">A format specification.</param>
  /// <param name="provider">An <c>IFormatProvider</c> that supplies culture-specific formatting information. </param>
  /// <returns>
  /// A <c>String</c> object representing the <see cref="Value"/> of this instance of <c>NaDateTime</c>. If this
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
  /// Converts the <c>String</c> representation of a number to its NaDateTime equivalent.
  /// </summary>
  /// <param name="s">The <c>String</c> to be parsed.</param>
  /// <param name="styles">The combination of one or more <c>DateTimeStyles</c> constants that indicates the permitted format of <c>s</c>.</param>
  /// <param name="provider">An <c>IFormatProvider</c> that supplies culture-specific formatting information about <c>s</c>.</param>
  /// <returns>
  /// An <c>NaDateTime</c> equivalent to the value contained in the specified string. If the string is a null reference, 
  /// a zero-length string or <see cref="NullString"/> ("null"), <c>NaDateTime.Null</c> is returned. Otherwise, 
  /// <see cref="Value"/> contains the same value that <c>DateTime.Parse</c> would return.
  /// </returns>
  public static NaDateTime Parse (string s, DateTimeStyles styles, IFormatProvider provider)
  {
    if (s == null || s.Length == 0 || s == NullString)
      return NaDateTime.Null;
    else return new NaDateTime (DateTime.Parse(s, provider, styles));
  }

  /// <summary>
  /// Converts the <c>String</c> representation of a number to its NaDateTime equivalent.
  /// </summary>
  /// <param name="s">The <c>String</c> to be parsed.</param>
  /// <param name="provider">An <c>IFormatProvider</c> that supplies culture-specific formatting information about <c>s</c>.</param>
  /// <returns>
  /// An <c>NaDateTime</c> equivalent to the value contained in the specified string. If the string is a null reference, 
  /// a zero-length string or <see cref="NullString"/> ("null"), <c>NaDateTime.Null</c> is returned. Otherwise, 
  /// <see cref="Value"/> contains the same value that <c>DateTime.Parse</c> would return.
  /// </returns>
  public static NaDateTime Parse (string s, IFormatProvider provider)
  {
    return Parse (s, DateTimeStyles.None, provider);
  }

  /// <summary>
  /// Converts the <c>String</c> representation of a number to its NaDateTime equivalent.
  /// </summary>
  /// <param name="s">The <c>String</c> to be parsed.</param>
  /// <param name="styles">The combination of one or more <c>DateTimeStyles</c> constants that indicates the permitted format of <c>s</c>.</param>
  /// <returns>
  /// An <c>NaDateTime</c> equivalent to the value contained in the specified string. If the string is a null reference, 
  /// a zero-length string or <see cref="NullString"/> ("null"), <c>NaDateTime.Null</c> is returned. Otherwise, 
  /// <see cref="Value"/> contains the same value that <c>DateTime.Parse</c> would return.
  /// </returns>
  public static NaDateTime Parse (string s, DateTimeStyles styles)
  {
    return Parse (s, styles, null);
  }

  /// <summary>
  /// Converts the <c>String</c> representation of a number to its NaDateTime equivalent.
  /// </summary>
  /// <param name="s">The <c>String</c> to be parsed.</param>
  /// <returns>
  /// An <c>NaDateTime</c> equivalent to the value contained in the specified string. If the string is a null reference, 
  /// a zero-length string or <see cref="NullString"/> ("null"), <c>NaDateTime.Null</c> is returned. Otherwise, 
  /// <see cref="Value"/> contains the same value that <c>DateTime.Parse</c> would return.
  /// </returns>
  public static NaDateTime Parse (string s)
  {
    return Parse (s, DateTimeStyles.None, null);
  }

  /// <summary>
  /// Converts the supplied <c>DateTime</c> to a <see cref="NaDateTime"/> structure.
  /// </summary>
  /// <param name="value">An <c>DateTime</c> value.</param>
  /// <returns>The converted <see cref="NaDateTime"/> value.</returns>
  public static implicit operator NaDateTime (DateTime value)
  {
    return new NaDateTime (value);
  }

  /// <summary>
  /// Converts the supplied <see cref="NaDateTime"/> structure to an <c>DateTime</c>.
  /// </summary>
  /// <param name="value">A <see cref="NaDateTime"/> structure.</param>
  /// <returns>The converted integer value.</returns>
  /// <exception cref="NaNullValueException">
  /// The passed value is null.
  /// </exception>
  public static explicit operator DateTime (NaDateTime value)
  {
    if (value.IsNull)
      throw NaNullValueException.AccessingMember ("NaDateTime to DateTime Conversion");
    return value._value;
  }

  /// <summary>
  /// Converts the supplied <c>SqlDateTime</c> structure to a <see cref="NaDateTime"/> structure.
  /// </summary>
  public static NaDateTime FromSqlInt32 (SqlDateTime value)
  {
    return (NaDateTime) value;
  }

  /// <summary>
  /// Converts the supplied <c>SqlDateTime</c> structure to a <see cref="NaDateTime"/> structure.
  /// </summary>
  public static implicit operator NaDateTime (SqlDateTime value)
  {
    if (value.IsNull)
      return NaDateTime.Null;
    else
      return new NaDateTime (value.Value);
  }

  /// <summary>
  /// Converts the supplied <see cref="NaDateTime"/> structure to a <c>SqlDateTime</c>structure.
  /// </summary>
  public static SqlDateTime ToSqlInt32 (NaDateTime value)
  {
    return (SqlDateTime) value;
  }

  /// <summary>
  /// Converts the supplied <see cref="NaDateTime"/> structure to a <c>SqlDateTime</c>structure.
  /// </summary>
  public static implicit operator SqlDateTime (NaDateTime value)
  {
    if (value.IsNull)
      return SqlDateTime.Null;
    else
      return new SqlDateTime (value.Value);
  }

  /// <summary>
  /// Converts a <see cref="NaDateTime"/> value to a boxed <c>DateTime</c> value or a null reference.
  /// </summary>
  /// <returns>The integer value of <c>value</c> as an <c>DateTime</c> if it is not null, a null reference otherwise.</returns>
  /// <remarks>
  /// Use this method to easily pass <c>NaDateTime</c> values to methods that expect an untyped parameter which is either an <c>DateTime</c>
  /// value or a null reference.
  /// </remarks>
  public static object ToBoxedDateTime (NaDateTime value)
  {
    if (value.IsNull )
      return null;
    else
      return value._value;
  }

  /// <summary>
  /// Converts a boxed <c>DateTime</c> value or a null reference to a <see cref="NaDateTime"/> value.
  /// </summary>
  /// <returns>A <see cref="NaDateTime"/> with its <see cref="Value"/> set to the integer value of <c>value</c> if it is an <c>DateTime</c>, 
  /// <c>NaDateTime.Null</c> if it is a null reference.</returns>
  /// <remarks>
  /// Use this method to easily create an <c>NaDateTime</c> value from an untyped value which is either an <c>DateTime</c> value or a null reference.
  /// </remarks>
  /// <exception cref="ArgumentException"><c>value</c> is neither a null reference nor an <c>DateTime</c> value.</exception>
  public static NaDateTime FromBoxedDateTime (object value)
  {
    if (value == null)
      return NaDateTime.Null;

    if (! (value is DateTime))
      throw new ArgumentException ("Must be a DateTime value", "value");

    return new NaDateTime ((DateTime) value);
  }

  /// <summary>
  /// Converts a <see cref="NaDateTime"/> value to a boxed <c>DateTime</c> value or to DBNull.Value.
  /// </summary>
  /// <returns>The integer value of <c>value</c> as an <c>DateTime</c> if it is not null, DBNull.Value otherwise.</returns>
  /// <remarks>
  /// Use this method to easily pass <c>NaDateTime</c> values to methods that expect an untyped parameter which is either an <c>DateTime</c>
  /// value or DBNull.Value.
  /// </remarks>
  public static object ToBoxedDateTimeDBNull (NaDateTime value)
  {
    if (value.IsNull )
      return DBNull.Value;
    else
      return value._value;
  }

  /// <summary>
  /// Converts a boxed <c>DateTime</c> value or DBNull.Value to a <see cref="NaDateTime"/> value.
  /// </summary>
  /// <returns>A <see cref="NaDateTime"/> with its <see cref="Value"/> set to the integer value of <c>value</c> if it is an <c>DateTime</c>, 
  /// <c>NaDateTime.Null</c> if it is DBNull.Value.</returns>
  /// <remarks>
  /// Use this method to easily create an <c>NaDateTime</c> value from an untyped value which is either an <c>DateTime</c> value or DBNull.Value.
  /// </remarks>
  /// <exception cref="ArgumentException"><c>value</c> is neither DBNull.Value nor an <c>DateTime</c> value.</exception>
  public static NaDateTime FromBoxedDateTimeDBNull (object value)
  {
    if (value == DBNull.Value)
      return NaDateTime.Null;

    if (! (value is DateTime))
      throw new ArgumentException ("Must be a DateTime value", "value");

    return new NaDateTime ((DateTime) value);
  }

  #endregion

  #region value

  /// <summary>
  /// Gets the value of this <see cref="NaDateTime"/> structure. This property is read-only.
  /// </summary>
  /// <value>
  /// The DateTime value of this NaDateTime structure.
  /// </value>
  /// <exception cref="NaNullValueException"> The property contains <see cref="Null"/>. </exception>
  public DateTime Value 
  {
    get 
    { 
      if (IsNull)
        throw NaNullValueException.AccessingMember ("Value");
      return _value;
    }
  }

  /// <summary>
  /// Gets the date component of this instance.
  /// </summary>
  /// <value>
  /// A new <c>DateTime</c> with the same date as this instance, and the time value set to 12 A.M. (00:00:00).
  /// </value>
  /// <exception cref="NaNullValueException"> The property contains <see cref="Null"/>. </exception>
  public DateTime Date
  {
    get { return Value.Date; }
  }

  /// <summary>
  /// Gets the time of day for this instance.
  /// </summary>
  /// <value>
  /// A <c>TimeSpan </c>that represents the fraction of the day elapsed since midnight.
  /// </value>
  /// <exception cref="NaNullValueException"> The property contains <see cref="Null"/>. </exception>
  public TimeSpan Time
  {
    get { return Value.TimeOfDay; }
  }

  /// <summary> Gets the year of the current instance. </summary>
  /// <exception cref="NaNullValueException"> The property contains <see cref="Null"/>. </exception>
  public int Year
  {
    get { return Value.Year; }
  }

  /// <summary> Gets the month of the current instance. </summary>
  /// <value>The month, between 1 and 12.</value>
  /// <exception cref="NaNullValueException"> The property contains <see cref="Null"/>. </exception>
  public int Month
  {
    get { return Value.Month; }
  }

  /// <summary> Gets the day of the current instance. </summary>
  /// <value>The day value, between 1 and 31.</value>
  /// <exception cref="NaNullValueException"> The property contains <see cref="Null"/>. </exception>
  public int Day
  {
    get { return Value.Day; }
  }

  /// <summary> Gets the hour of the current instance. </summary>
  /// <value>The hour, between 0 and 23.</value>
  /// <exception cref="NaNullValueException"> The property contains <see cref="Null"/>. </exception>
  public int Hour
  {
    get { return Value.Hour; }
  }

  /// <summary> Gets the minute of the current instance. </summary>
  /// <value>The minute, between 0 and 59.</value>
  /// <exception cref="NaNullValueException"> The property contains <see cref="Null"/>. </exception>
  public int Minute
  {
    get { return Value.Minute; }
  }

  /// <summary> Gets the second of the current instance. </summary>
  /// <value>The second, between 0 and 59.</value>
  /// <exception cref="NaNullValueException"> The property contains <see cref="Null"/>. </exception>
  public int Second
  {
    get { return Value.Second; }
  }

  /// <summary> Gets the millisecond of the current instance. </summary>
  /// <value>The millisecond, between 0 and 999.</value>
  /// <exception cref="NaNullValueException"> The property contains <see cref="Null"/>. </exception>
  public int Millisecond
  {
    get { return Value.Millisecond; }
  }

  /// <summary> Gets the number of ticks that represent the date and time of this instance. </summary>
  /// <value>The number of ticks that represent the date and time of this instance. The value is between <see cref="MinValue"/> and <see cref="MaxValue"/>.</value>
  /// <remarks>The value of this property is the number of 100-nanosecond intervals that have elapsed since 12:00 A.M., January 1, 0001.</remarks>
  /// <exception cref="NaNullValueException"> The property contains <see cref="Null"/>. </exception>
  public long Ticks
  {
    get { return Value.Ticks; }
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
  /// Represents a null value that can be assigned to an instance of the <see cref="NaDateTime"/> structure.
  /// </summary>
  /// <remarks>
  /// Null functions as a constant for the <see cref="NaDateTime"/> structure.
  /// </remarks>
  public static readonly NaDateTime Null = new NaDateTime(true);

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
  /// A constant representing the largest possible value of a NaDateTime.
  /// </summary>
  public static readonly NaDateTime MaxValue = new NaDateTime (DateTime.MaxValue);

  /// <summary>
  /// A constant representing the smallest possible value of a NaDateTime.
  /// </summary>
  public static readonly NaDateTime MinValue = new NaDateTime (DateTime.MinValue);

  #endregion

  #region equality

  /// <summary>
  /// Compares the supplied object parameter to the <see cref="Value"/> property of the <see cref="NaDateTime"/> object.
  /// </summary>
  /// <param name="obj">The object to be compared. </param>
  /// <returns>
  /// <c>true</c> if object is an instance of <see cref="NaDateTime"/> and the two are equal; otherwise <c>false</c>.
  /// If object is a null reference, <c>false</c> is returned.
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// Note that <c>false</c> is returned if object is an DateTime (types are not equal).
  /// </remarks>
  public override bool Equals (object obj)
  {
    if (! (obj is NaDateTime))
      return false; // obj is a null reference or another type then NaDateTime

    return Equals (this, (NaDateTime) obj);
  }

  /// <summary>
  /// Compares the supplied <see cref="NaDateTime"/> parameter to the <see cref="Value"/> property of this instance.
  /// </summary>
  /// <param name="value">The <see cref="NaDateTime"/> instance to be compared. </param>
  /// <returns>
  /// <c>true</c> if the two are equal; otherwise <c>false</c>.
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public bool Equals (NaDateTime value)
  {
    return Equals (this, value);
  }

  /// <summary>
  /// Performs a logical comparison of the two NaDateTime parameters to determine if they are equal.
  /// </summary>
  /// <returns>
  /// <c>true</c> if the two instances are equal or <c>false</c> if they are not equal. 
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public static bool Equals (NaDateTime x, NaDateTime y)
  {
    return x == y;
  }

  /// <summary>
  /// Performs a logical comparison of the two NaDateTime parameters to determine if they are equal.
  /// </summary>
  /// <returns>
  /// <c>true</c> if the two instances are equal or <c>false</c> if they are not equal. 
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public static bool operator == (NaDateTime x, NaDateTime y)
  {
    if (x.IsNull && y.IsNull)
      return true;
    if (x.IsNull != y.IsNull)
      return false;
    return x._value == y._value;
  }

  /// <summary>
  /// Performs a logical comparison of the two NaDateTime parameters to determine if they are not equal.
  /// </summary>
  /// <returns>
  /// <c>false</c> if the two instances are equal or <c>true</c> if they are not equal. 
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="NotEqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public static bool operator != (NaDateTime x, NaDateTime y)
  {
    return ! (x == y);
  }

  /// <summary>
  /// Performs a logical comparison of the two NaDateTime parameters to determine if they are not equal.
  /// </summary>
  /// <returns>
  /// <c>false</c> if the two instances are equal or <c>true</c> if they are not equal. 
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="NotEqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public static bool NotEquals (NaDateTime x, NaDateTime y)
  {
    return x != y;
  }

  /// <summary>
  /// Performs a SQL-style comparison of the two NaDateTime parameters to determine if they are equal.
  /// </summary>
  /// <returns>
  /// <c>True</c> if the two instances are equal, or <c>False</c> if they are not equal, or <c>Null</c> if either of them is <c>Null</c>.
  /// </returns>
  public static NaBoolean EqualsSql (NaDateTime x, NaDateTime y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;
    else return new NaBoolean (x._value == y._value);
  }

  /// <summary>
  /// Performs a SQL-style comparison of the two NaDateTime parameters to determine if they are not equal.
  /// </summary>
  /// <returns>
  /// <c>True</c> if the two instances are not equal, or <c>False</c> if they are equal, or <c>Null</c> if either of them is <c>Null</c>.
  /// </returns>
  public static NaBoolean NotEqualsSql (NaDateTime x, NaDateTime y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;
    else return new NaBoolean (x._value != y._value);
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

    if (! (obj is NaDateTime))
      throw new ArgumentException ("obj");

    return CompareTo ((NaDateTime) obj);
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
  public int CompareTo (NaDateTime value)
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
  /// Computes the sum of the the current value and a <see cref="TimeSpan"/>.
  /// </summary>
  /// <returns>
  /// A <c>NaDateTime</c> structure whose <see cref="Value"/> property contains the sum,
  /// or <see cref="Null"/> if the current value is <c>Null</c>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public NaDateTime Add (TimeSpan timespan)
  {
    return this + timespan;
  }

  /// <summary>
  /// Computes the sum of the the specified <see cref="NaDateTime"/> and a <see cref="TimeSpan"/>.
  /// </summary>
  /// <returns>
  /// A <c>NaDateTime</c> structure whose <see cref="Value"/> property contains the sum of the specified parameters,
  /// or <see cref="Null"/> if <c>datetime</c> is <c>Null</c>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public static NaDateTime operator + (NaDateTime datetime, TimeSpan timespan)
  {
	  if (datetime.IsNull)
		  return NaDateTime.Null;

    try
    {
      return new NaDateTime (datetime._value.Add (timespan));
    }
    catch (ArgumentOutOfRangeException e)
    {
      throw new OverflowException (NaResources.ArithmeticOverflowMsg, e);
    }
  }

  /// <summary>
  /// Subtracts <c>timespan</c> from the current value.
  /// </summary>
  /// <returns>
  /// A <c>NaDateTime</c> structure whose <see cref="Value"/> property contains the result of the subtraction,
  /// or <see cref="Null"/> if the current value is <c>Null</c>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public NaDateTime Subtract (TimeSpan timespan)
  {
    return this - timespan;
  }

  /// <summary>
  /// Subtracts the second <see cref="NaDateTime"/> parameter from the first.
  /// </summary>
  /// <returns>
  /// A <c>NaDateTime</c> structure whose <see cref="Value"/> property contains the result of the subtraction,
  /// or <see cref="Null"/> if either parameter is <c>Null</c>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public static NaDateTime operator - (NaDateTime datetime, TimeSpan timespan)
  {
    if (datetime.IsNull)
      return NaDateTime.Null;

    try
    {
      return new NaDateTime (datetime._value.Subtract (timespan));
    }
    catch (ArgumentOutOfRangeException e)
    {
      throw new OverflowException (NaResources.ArithmeticOverflowMsg, e);
    }
  }

  /// <summary>
  /// Adds the specified number of years to the current value.
  /// </summary>
  /// <returns>The result of the addition, or <c>Null</c> if the current value is <c>Null</c>.</returns>
  /// <exception cref="OverflowException">The result is greater then <see cref="MaxValue"/>.</exception>
  public NaDateTime AddYears (int years)
  {
    return AddMonths (years * 12);
  }

  /// <summary>
  /// Adds the specified number of months to the current value.
  /// </summary>
  /// <returns>The result of the addition, or <c>Null</c> if the current value is <c>Null</c>.</returns>
  /// <exception cref="OverflowException">The result is greater then <see cref="MaxValue"/>.</exception>
  public NaDateTime AddMonths (int months)
  {
    if (this.IsNull)
      return NaDateTime.Null;

    try
    {
      return _value.AddMonths (months);
    }
    catch (ArgumentOutOfRangeException e)
    {
      throw new OverflowException (NaResources.ArithmeticOverflowMsg, e);
    }
  }

  /// <summary>
  /// Adds the specified number of days to the current value.
  /// </summary>
  /// <returns>The result of the addition, or <c>Null</c> if the current value is <c>Null</c>.</returns>
  /// <exception cref="OverflowException">The result is greater then <see cref="MaxValue"/>.</exception>
  public NaDateTime AddDays (double days)
  {
    if (this.IsNull)
      return NaDateTime.Null;

    try
    {
      return _value.AddDays (days);
    }
    catch (ArgumentOutOfRangeException e)
    {
      throw new OverflowException (NaResources.ArithmeticOverflowMsg, e);
    }
  }

  /// <summary>
  /// Adds the specified number of hours to the current value.
  /// </summary>
  /// <returns>The result of the addition, or <c>Null</c> if the current value is <c>Null</c>.</returns>
  /// <exception cref="OverflowException">The result is greater then <see cref="MaxValue"/>.</exception>
  public NaDateTime AddHours (double hours)
  {
    if (this.IsNull)
      return NaDateTime.Null;

    try
    {
      return _value.AddHours (hours);
    }
    catch (ArgumentOutOfRangeException e)
    {
      throw new OverflowException (NaResources.ArithmeticOverflowMsg, e);
    }
  }
  
  /// <summary>
  /// Adds the specified number of minutes to the current value.
  /// </summary>
  /// <returns>The result of the addition, or <c>Null</c> if the current value is <c>Null</c>.</returns>
  /// <exception cref="OverflowException">The result is greater then <see cref="MaxValue"/>.</exception>
  public NaDateTime AddMinutes (double minutes)
  {
    if (this.IsNull)
      return NaDateTime.Null;

    try
    {
      return _value.AddMinutes (minutes);
    }
    catch (ArgumentOutOfRangeException e)
    {
      throw new OverflowException (NaResources.ArithmeticOverflowMsg, e);
    }
  }
  
  /// <summary>
  /// Adds the specified number of seconds to the current value.
  /// </summary>
  /// <returns>The result of the addition, or <c>Null</c> if the current value is <c>Null</c>.</returns>
  /// <exception cref="OverflowException">The result is greater then <see cref="MaxValue"/>.</exception>
  public NaDateTime AddSeconds (double seconds)
  {
    if (this.IsNull)
      return NaDateTime.Null;

    try
    {
      return _value.AddSeconds (seconds);
    }
    catch (ArgumentOutOfRangeException e)
    {
      throw new OverflowException (NaResources.ArithmeticOverflowMsg, e);
    }
  }

  /// <summary>
  /// Adds the specified number of milliseconds to the current value.
  /// </summary>
  /// <returns>The result of the addition, or <c>Null</c> if the current value is <c>Null</c>.</returns>
  /// <exception cref="OverflowException">The result is greater then <see cref="MaxValue"/>.</exception>
  public NaDateTime AddMilliseconds  (double milliseconds )
  {
    if (this.IsNull)
      return NaDateTime.Null;

    try
    {
      return _value.AddMilliseconds (milliseconds );
    }
    catch (ArgumentOutOfRangeException e)
    {
      throw new OverflowException (NaResources.ArithmeticOverflowMsg, e);
    }
  }

  #endregion

  #region relative compare operators

  /// <summary>
  /// Compares the two <see cref="NaDateTime "/> parameters to determine if the first is less than the second.
  /// </summary>
  /// <returns>
  /// A <see cref="NaBoolean"/> that is <c>True</c> if the first parameter is less than the second, otherwise <c>False</c>. If 
  /// either parameter is <c>Null</c>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean LessThan (NaDateTime x, NaDateTime y)
  {
    return x < y;
  }

  /// <summary>
  /// Compares the two <see cref="NaDateTime "/> parameters to determine if the first is less than the second.
  /// </summary>
  /// <returns>
  /// A <see cref="NaBoolean"/> that is <c>True</c> if the first parameter is less than the second, otherwise <c>False</c>. If 
  /// either parameter is <c>Null</c>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean operator < (NaDateTime x, NaDateTime y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;

    return new NaBoolean (x._value < y._value);
  }

  /// <summary>
  /// Compares the two <see cref="NaDateTime "/> parameters to determine if the first is less than or equal to the second.
  /// </summary>
  /// <returns>
  /// A <see cref="NaBoolean"/> that is <c>True</c> if the first parameter is less than or equal to the second, otherwise <c>False</c>. If 
  /// either parameter is <c>Null</c>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean LessThanOrEqual (NaDateTime x, NaDateTime y)
  {
    return x <= y;
  }

  /// <summary>
  /// Compares the two <see cref="NaDateTime "/> parameters to determine if the first is less than or equal to the second.
  /// </summary>
  /// <returns>
  /// A <see cref="NaBoolean"/> that is <c>True</c> if the first parameter is less than or equal to the second, otherwise <c>False</c>. If 
  /// either parameter is <c>Null</c>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean operator <= (NaDateTime x, NaDateTime y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;

    return new NaBoolean (x._value <= y._value);
  }

  /// <summary>
  /// Compares the two <see cref="NaDateTime "/> parameters to determine if the first is greater than the second.
  /// </summary>
  /// <returns>
  /// A <see cref="NaBoolean"/> that is <c>True</c> if the first parameter is greater than the second, otherwise <c>False</c>. If 
  /// either parameter is <c>Null</c>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean GreaterThan (NaDateTime x, NaDateTime y)
  {
    return x > y;
  }

  /// <summary>
  /// Compares the two <see cref="NaDateTime "/> parameters to determine if the first is greater than the second.
  /// </summary>
  /// <returns>
  /// A <see cref="NaBoolean"/> that is <c>True</c> if the first parameter is greater than the second, otherwise <c>False</c>. If 
  /// either parameter is <c>Null</c>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean operator > (NaDateTime x, NaDateTime y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;

    return new NaBoolean (x._value > y._value);
  }

  /// <summary>
  /// Compares the two <see cref="NaDateTime "/> parameters to determine if the first is greater than or equal to the second.
  /// </summary>
  /// <returns>
  /// A <see cref="NaBoolean"/> that is <c>True</c> if the first parameter is greater than or equal to the second, otherwise <c>False</c>. If 
  /// either parameter is <c>Null</c>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean GreaterThanOrEqual (NaDateTime x, NaDateTime y)
  {
    return x >= y;
  }

  /// <summary>
  /// Compares the two <see cref="NaDateTime "/> parameters to determine if the first is greater than or equal to the second.
  /// </summary>
  /// <returns>
  /// A <see cref="NaBoolean"/> that is <c>True</c> if the first parameter is greater than or equal to the second, otherwise <c>False</c>. If 
  /// either parameter is <c>Null</c>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean operator >= (NaDateTime x, NaDateTime y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;

    return new NaBoolean (x._value >= y._value);
  }
  
  #endregion
}


}
