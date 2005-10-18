using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Data.SqlTypes;
using System.Globalization;

namespace Rubicon.NullableValueTypes
{

/// <summary>
/// Represents a double-precision floating point number that can be <see langword="null"/>. The corresponding system type is System.Double.
/// </summary>
/// <include file='doc\include\NullableValueTypes\include.xml' path='Comments/NaDouble/remarks' />
[Serializable]
[NaBasicType (typeof(Double))]
[TypeConverter (typeof (NaDoubleConverter))]
public struct NaDouble: INaNullable, IComparable, IFormattable
{
  #region member fields

  private Double _value;
  private bool _isNotNull;

  #endregion

  #region construction and disposal

  /// <summary>
  /// Initializes a new instance of the <see cref="NaDouble"/> structure using the supplied double value.
  /// </summary>
  /// <param name="value">The double to be converted.</param>
  public NaDouble (Double value)
  {
    _value = value;
    _isNotNull = true;
  }

  private NaDouble (bool isNull)
  {
    _value = 0d;
    _isNotNull = ! isNull;
  }

  #endregion

  #region type conversion

  /// <summary>
  /// Converts an <see cref="NaDouble"/> structure to a <see cref="String"/>.
  /// </summary>
  /// <returns>
  /// A <see cref="String"/> object representing the <see cref="Value"/> of this instance of <see cref="NaDouble"/>. If this
  /// instance is <see langword="null"/>, <see cref="NullString"/> ("null") is returned.
  /// </returns>
  public override string ToString()
  {
    return ToString (null, null);
  }

  /// <summary>
  /// Converts an <see cref="NaDouble"/> structure to a <see cref="String"/>.
  /// </summary>
  /// <param name="format">A format specification.</param>
  /// <returns>
  /// A <see cref="String"/> object representing the <see cref="Value"/> of this instance of <see cref="NaDouble"/>. If this
  /// instance is <see langword="null"/>, <see cref="NullString"/> ("null") is returned. If <paramref name="format"/> is prefixed with a tilde symbol
  /// ("~"), a zero-length string is returned instead.
  /// </returns>
  public string ToString (string format)
  {
    return ToString (format, null);
  }

  /// <summary>
  /// Converts an <see cref="NaDouble"/> structure to a <see cref="String"/>.
  /// </summary>
  /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information. </param>
  /// <returns>
  /// A <see cref="String"/> object representing the <see cref="Value"/> of this instance of <see cref="NaDouble"/>. If this
  /// instance is <see langword="null"/>, <see cref="NullString"/> ("null") is returned.
  /// </returns>
  public string ToString (IFormatProvider provider)
  {
    return ToString (null, provider);
  }

  /// <summary>
  /// Converts an <see cref="NaDouble"/> structure to a <see cref="String"/>.
  /// </summary>
  /// <param name="format">A format specification.</param>
  /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information. </param>
  /// <returns>
  /// A <see cref="String"/> object representing the <see cref="Value"/> of this instance of <see cref="NaDouble"/>. If this
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
  /// Converts the <see cref="String"/> representation of a number to its NaDouble equivalent.
  /// </summary>
  /// <param name="s">The <see cref="String"/> to be parsed.</param>
  /// <param name="styles">The combination of one or more <see cref="NumberStyles"/> constants that indicates the permitted format of <paramref name="s"/>.</param>
  /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information about <paramref name="s"/>.</param>
  /// <returns>
  /// An <see cref="NaDouble"/> equivalent to the value contained in the specified string. If the string is a null reference, 
  /// a zero-length string or <see cref="NullString"/> ("null"), <see cref="NaDouble.Null"/> is returned. Otherwise, 
  /// <see cref="Value"/> contains the same value that <see cref="Double.Parse"/> would return.
  /// </returns>
  public static NaDouble Parse (string s, NumberStyles styles, IFormatProvider provider)
  {
    if (s == null || s.Length == 0 || s == NullString)
      return NaDouble.Null;
    else return new NaDouble (Double.Parse(s, styles, provider));
  }

  /// <summary>
  /// Converts the <see cref="String"/> representation of a number to its NaDouble equivalent.
  /// </summary>
  /// <param name="s">The <see cref="String"/> to be parsed.</param>
  /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information about <paramref name="s"/>.</param>
  /// <returns>
  /// An <see cref="NaDouble"/> equivalent to the value contained in the specified string. If the string is a null reference, 
  /// a zero-length string or <see cref="NullString"/> ("null"), <see cref="NaDouble.Null"/> is returned. Otherwise, 
  /// <see cref="Value"/> contains the same value that <see cref="Double.Parse"/> would return.
  /// </returns>
  public static NaDouble Parse (string s, IFormatProvider provider)
  {
    return Parse (s, NumberStyles.Float, provider);
  }

  /// <summary>
  /// Converts the <see cref="String"/> representation of a number to its NaDouble equivalent.
  /// </summary>
  /// <param name="s">The <see cref="String"/> to be parsed.</param>
  /// <param name="styles">The combination of one or more <see cref="NumberStyles"/> constants that indicates the permitted format of <paramref name="s"/>.</param>
  /// <returns>
  /// An <see cref="NaDouble"/> equivalent to the value contained in the specified string. If the string is a null reference, 
  /// a zero-length string or <see cref="NullString"/> ("null"), <see cref="NaDouble.Null"/> is returned. Otherwise, 
  /// <see cref="Value"/> contains the same value that <see cref="Double.Parse"/> would return.
  /// </returns>
  public static NaDouble Parse (string s, NumberStyles styles)
  {
    return Parse (s, styles, null);
  }

  /// <summary>
  /// Converts the <see cref="String"/> representation of a number to its NaDouble equivalent.
  /// </summary>
  /// <param name="s">The <see cref="String"/> to be parsed.</param>
  /// <returns>
  /// An <see cref="NaDouble"/> equivalent to the value contained in the specified string. If the string is a null reference, 
  /// a zero-length string or <see cref="NullString"/> ("null"), <see cref="NaDouble.Null"/> is returned. Otherwise, 
  /// <see cref="Value"/> contains the same value that <see cref="Double.Parse"/> would return.
  /// </returns>
  public static NaDouble Parse (string s)
  {
    return Parse (s, NumberStyles.Float, null);
  }

  /// <summary>
  /// Converts the supplied <see cref="Double"/> to an <see cref="NaDouble"/> structure.
  /// </summary>
  /// <param name="value">An <see cref="Double"/> value.</param>
  /// <returns>The converted <see cref="NaDouble"/> value.</returns>
  public static implicit operator NaDouble (Double value)
  {
    return new NaDouble (value);
  }

  /// <summary>
  /// Converts the supplied <see cref="NaDouble"/> structure to an <see cref="Double"/>.
  /// </summary>
  /// <param name="value">An <see cref="NaDouble"/> structure.</param>
  /// <returns>The converted double value.</returns>
  /// <exception cref="NaNullValueException">
  /// The passed value is null.
  /// </exception>
  public static explicit operator Double (NaDouble value)
  {
    if (value.IsNull)
      throw NaNullValueException.AccessingMember ("NaDouble to Double Conversion");
    return value._value;
  }

  /// <summary>
  /// Converts the supplied <see cref="SqlDouble"/> structure to an <see cref="NaDouble"/> structure.
  /// </summary>
  public static NaDouble FromSqlInt32 (SqlDouble value)
  {
    return (NaDouble) value;
  }

  /// <summary>
  /// Converts the supplied <see cref="SqlDouble"/> structure to an <see cref="NaDouble"/> structure.
  /// </summary>
  public static implicit operator NaDouble (SqlDouble value)
  {
    if (value.IsNull)
      return NaDouble.Null;
    else
      return new NaDouble (value.Value);
  }

  /// <summary>
  /// Converts the supplied <see cref="NaDouble"/> structure to a <see cref="SqlDouble"/>structure.
  /// </summary>
  public static SqlDouble ToSqlInt32 (NaDouble value)
  {
    return (SqlDouble) value;
  }

  /// <summary>
  /// Converts the supplied <see cref="NaDouble"/> structure to a <see cref="SqlDouble"/>structure.
  /// </summary>
  public static implicit operator SqlDouble (NaDouble value)
  {
    if (value.IsNull)
      return SqlDouble.Null;
    else
      return new SqlDouble (value.Value);
  }

  /// <summary>
  /// Converts an <see cref="NaDouble"/> value to a boxed <see cref="Double"/> value or a null reference.
  /// </summary>
  /// <returns>The double value of <paramref name="value"/> as an <see cref="Double"/> if it is not null, a null reference otherwise.</returns>
  /// <remarks>
  /// Use this method to easily pass <see cref="NaDouble"/> values to methods that expect an untyped parameter which is either an <see cref="Double"/>
  /// value or a null reference.
  /// </remarks>
  public static object ToBoxedDouble (NaDouble value)
  {
    if (value.IsNull )
      return null;
    else
      return value._value;
  }

  /// <summary>
  /// Converts a boxed <see cref="Double"/> value or a null reference to an <see cref="NaDouble"/> value.
  /// </summary>
  /// <returns>An <see cref="NaDouble"/> with its <see cref="Value"/> set to the double value of <paramref name="value"/> if it is an <see cref="Double"/>, 
  /// <see cref="NaDouble.Null"/> if it is a null reference.</returns>
  /// <remarks>
  /// Use this method to easily create an <see cref="NaDouble"/> value from an untyped value which is either an <see cref="Double"/> value or a null reference.
  /// </remarks>
  /// <exception cref="ArgumentException"><paramref name="value"/> is neither a null reference nor an <see cref="Double"/> value.</exception>
  public static NaDouble FromBoxedDouble (object value)
  {
    if (value == null)
      return NaDouble.Null;

    if (! (value is Double))
      throw new ArgumentException ("Must be a Double value", "value");

    return new NaDouble ((Double) value);
  }

  /// <summary>
  /// Converts an <see cref="NaDouble"/> value to a boxed <see cref="Double"/> value or to DBNull.Value.
  /// </summary>
  /// <returns>The double value of <paramref name="value"/> as an <see cref="Double"/> if it is not null, DBNull.Value otherwise.</returns>
  /// <remarks>
  /// Use this method to easily pass <see cref="NaDouble"/> values to methods that expect an untyped parameter which is either an <see cref="Double"/>
  /// value or DBNull.Value.
  /// </remarks>
  public static object ToBoxedDoubleDBNull (NaDouble value)
  {
    if (value.IsNull )
      return DBNull.Value;
    else
      return value._value;
  }

  /// <summary>
  /// Converts a boxed <see cref="Double"/> value or DBNull.Value to an <see cref="NaDouble"/> value.
  /// </summary>
  /// <returns>An <see cref="NaDouble"/> with its <see cref="Value"/> set to the double value of <paramref name="value"/> if it is an <see cref="Double"/>, 
  /// <see cref="NaDouble.Null"/> if it is DBNull.Value.</returns>
  /// <remarks>
  /// Use this method to easily create an <see cref="NaDouble"/> value from an untyped value which is either an <see cref="Double"/> value or DBNull.Value.
  /// </remarks>
  /// <exception cref="ArgumentException"><paramref name="value"/> is neither DBNull.Value nor an <see cref="Double"/> value.</exception>
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
  /// A Double representing the value of this NaDouble structure.
  /// </value>
  /// <exception cref="NaNullValueException">
  /// The property contains <see cref="Null"/>.
  /// </exception>
  public Double Value 
  {
    get 
    { 
      if (IsNull)
        throw NaNullValueException.AccessingMember ("Value");
      return _value;
    }
  }

  /// <summary>
  /// Gets the value of this <see cref="NaDouble"/> structure. This property is read-only.
  /// </summary>
  /// <value>
  /// An Object representing the value of this NaDouble structure.
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
  /// Represents a null value that can be assigned to an instance of the <see cref="NaDouble"/> structure.
  /// </summary>
  /// <remarks>
  /// Null functions as a constant for the <see cref="NaDouble"/> structure.
  /// </remarks>
  public static readonly NaDouble Null = new NaDouble(true);

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
  /// <see langword="true"/> if object is an instance of <see cref="NaDouble"/> and the two are equal; otherwise <see langword="false"/>.
  /// If object is a null reference, <see langword="false"/> is returned.
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// Note that <see langword="false"/> is returned if object is an Double (types are not equal).
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
  /// <see langword="true"/> if the two are equal; otherwise <see langword="false"/>.
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
  /// <see langword="true"/> if the two instances are equal or <see langword="false"/> if they are not equal. 
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
  /// <see langword="true"/> if the two instances are equal or <see langword="false"/> if they are not equal. 
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public static bool operator == (NaDouble x, NaDouble y)
  {
    if (x.IsNull && y.IsNull)
      return true;
    if (x.IsNull != y.IsNull)
      return false;
    return x._value == y._value;
  }

  /// <summary>
  /// Performs a logical comparison of the two NaDouble parameters to determine if they are not equal.
  /// </summary>
  /// <returns>
  /// <see langword="false"/> if the two instances are equal or <see langword="true"/> if they are not equal. 
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
  /// <see langword="false"/> if the two instances are equal or <see langword="true"/> if they are not equal. 
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
  /// <see cref="NaBoolean.True"/> if the two instances are equal, or <see cref="NaBoolean.False"/> if they are not equal, or <see langword="null"/> if either of them is <see langword="null"/>.
  /// </returns>
  public static NaBoolean EqualsSql (NaDouble x, NaDouble y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;
    else return new NaBoolean (x._value == y._value);
  }

  /// <summary>
  /// Performs a SQL-style comparison of the two NaDouble parameters to determine if they are not equal.
  /// </summary>
  /// <returns>
  /// <see cref="NaBoolean.True"/> if the two instances are not equal, or <see cref="NaBoolean.False"/> if they are equal, or <see langword="null"/> if either of them is <see langword="null"/>.
  /// </returns>
  public static NaBoolean NotEqualsSql (NaDouble x, NaDouble y)
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
  /// Computes the sum of the two specified <see cref="NaDouble"/> structures.
  /// </summary>
  /// <returns>
  /// An <see cref="NaDouble"/> structure whose <see cref="Value"/> property contains the sum of the specified <see cref="NaDouble"/> 
  /// structures, or <see cref="Null"/> if either parameter is <see langword="null"/>.
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
  /// An <see cref="NaDouble"/> structure whose <see cref="Value"/> property contains the sum of the specified <see cref="NaDouble"/> 
  /// structures, or <see cref="Null"/> if either parameter is <see langword="null"/>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public static NaDouble operator + (NaDouble x, NaDouble y)
  {
	  if (x.IsNull || y.IsNull)
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
  /// An <see cref="NaDouble"/> structure whose <see cref="Value"/> property contains the result of the subtraction,
  /// or <see cref="Null"/> if either parameter is <see langword="null"/>.
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
  /// An <see cref="NaDouble"/> structure whose <see cref="Value"/> property contains the result of the subtraction,
  /// or <see cref="Null"/> if either parameter is <see langword="null"/>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public static NaDouble operator - (NaDouble x, NaDouble y)
  {
    if (x.IsNull || y.IsNull)
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
  /// An <see cref="NaDouble"/> structure whose <see cref="Value"/> property contains the result of the division,
  /// or <see cref="Null"/> if either parameter is <see langword="null"/>.
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
  /// An <see cref="NaDouble"/> structure whose <see cref="Value"/> property contains the result of the division,
  /// or <see cref="Null"/> if either parameter is <see langword="null"/>.
  /// </returns>
  /// <exception cref="DivideByZeroException">A division by zero is attempted.</exception>
  public static NaDouble operator / (NaDouble x, NaDouble y)
  {
    if (x.IsNull || y.IsNull)
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
  /// An <see cref="NaDouble"/> structure whose <see cref="Value"/> property contains the product of the two parameters,
  /// or <see cref="Null"/> if either parameter is <see langword="null"/>.
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
  /// An <see cref="NaDouble"/> structure whose <see cref="Value"/> property contains the product of the two parameters,
  /// or <see cref="Null"/> if either parameter is <see langword="null"/>.
  /// </returns>
  /// <exception cref="OverflowException">An arithmetic overflow occurs.</exception>
  public static NaDouble operator * (NaDouble x, NaDouble y)
  {
    if (x.IsNull || y.IsNull)
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
  /// An <see cref="NaDouble"/> structure whose <see cref="Value"/> property contains the remainder,
  /// or <see cref="Null"/> if either parameter is <see langword="null"/>.
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
  /// An <see cref="NaDouble"/> structure whose <see cref="Value"/> property contains the remainder,
  /// or <see cref="Null"/> if either parameter is <see langword="null"/>.
  /// </returns>
  /// <exception cref="DivideByZeroException">A division by zero is attempted.</exception>
  public static NaDouble operator % (NaDouble x, NaDouble y)
  {
    if (x.IsNull || y.IsNull)
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
  /// An <see cref="NaBoolean"/> that is <see cref="NaBoolean.True"/> if the first parameter is less than the second, otherwise <see cref="NaBoolean.False"/>. If 
  /// either parameter is <see langword="null"/>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean LessThan (NaDouble x, NaDouble y)
  {
    return x < y;
  }

  /// <summary>
  /// Compares the two <see cref="NaDouble "/> parameters to determine if the first is less than the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaBoolean"/> that is <see cref="NaBoolean.True"/> if the first parameter is less than the second, otherwise <see cref="NaBoolean.False"/>. If 
  /// either parameter is <see langword="null"/>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean operator < (NaDouble x, NaDouble y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;

    return new NaBoolean (x._value < y._value);
  }

  /// <summary>
  /// Compares the two <see cref="NaDouble "/> parameters to determine if the first is less than or equal to the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaBoolean"/> that is <see cref="NaBoolean.True"/> if the first parameter is less than or equal to the second, otherwise <see cref="NaBoolean.False"/>. If 
  /// either parameter is <see langword="null"/>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean LessThanOrEqual (NaDouble x, NaDouble y)
  {
    return x <= y;
  }

  /// <summary>
  /// Compares the two <see cref="NaDouble "/> parameters to determine if the first is less than or equal to the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaBoolean"/> that is <see cref="NaBoolean.True"/> if the first parameter is less than or equal to the second, otherwise <see cref="NaBoolean.False"/>. If 
  /// either parameter is <see langword="null"/>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean operator <= (NaDouble x, NaDouble y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;

    return new NaBoolean (x._value <= y._value);
  }

  /// <summary>
  /// Compares the two <see cref="NaDouble "/> parameters to determine if the first is greater than the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaBoolean"/> that is <see cref="NaBoolean.True"/> if the first parameter is greater than the second, otherwise <see cref="NaBoolean.False"/>. If 
  /// either parameter is <see langword="null"/>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean GreaterThan (NaDouble x, NaDouble y)
  {
    return x > y;
  }

  /// <summary>
  /// Compares the two <see cref="NaDouble "/> parameters to determine if the first is greater than the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaBoolean"/> that is <see cref="NaBoolean.True"/> if the first parameter is greater than the second, otherwise <see cref="NaBoolean.False"/>. If 
  /// either parameter is <see langword="null"/>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean operator > (NaDouble x, NaDouble y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;

    return new NaBoolean (x._value > y._value);
  }

  /// <summary>
  /// Compares the two <see cref="NaDouble "/> parameters to determine if the first is greater than or equal to the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaBoolean"/> that is <see cref="NaBoolean.True"/> if the first parameter is greater than or equal to the second, otherwise <see cref="NaBoolean.False"/>. If 
  /// either parameter is <see langword="null"/>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean GreaterThanOrEqual (NaDouble x, NaDouble y)
  {
    return x >= y;
  }

  /// <summary>
  /// Compares the two <see cref="NaDouble "/> parameters to determine if the first is greater than or equal to the second.
  /// </summary>
  /// <returns>
  /// An <see cref="NaBoolean"/> that is <see cref="NaBoolean.True"/> if the first parameter is greater than or equal to the second, otherwise <see cref="NaBoolean.False"/>. If 
  /// either parameter is <see langword="null"/>, <see cref="NaBoolean.Null"/> is returned.
  /// </returns>
  public static NaBoolean operator >= (NaDouble x, NaDouble y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;

    return new NaBoolean (x._value >= y._value);
  }
  
  #endregion
}


}
