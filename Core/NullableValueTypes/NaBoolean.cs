using System;
using System.Runtime.Serialization;
using System.Data.SqlTypes;
using System.Globalization;

namespace Rubicon.Data.NullableValueTypes
{

/// <summary>
/// Represents a boolean value that can be <c>Null</c>. The corresponding system type is System.Boolean.
/// </summary>
/// <remarks>
///   <para>
///     <c>NaBoolean</c> is basically a structure that can contain <c>true</c>, <c>false</c> or <c>Null</c>. Use <see cref="IsNull"/> to find 
///     out whether a specific structure contains <c>Null</c>, or <c>NaBoolean.Null</c> to assign <c>Null</c> to a <c>NaBoolean</c> variable.
///   </para>
///   <para>
///     You can use the <see cref="Value"/> property to access the boolean value, or the explicit <c>Boolean</c> conversion operator. Either
///     method results in a <see cref="NaNullValueException"/> if the structure is <c>Null</c>.
///   </para>
///   <para>
///     NaBoolean can be used as a replacement for <c>System.Data.SqlTypes.SqlBoolean</c> if you prefer the null-value semantics of <c>NaBoolean</c>
///     or need serializability. Implicit conversion operators for <c>SqlBoolean </c>allow <c>NaBoolean</c> to be used seamlessly with ADO.NET.
///   </para>
///   <para>
///     The following null-value semantics are used for <c>NaBoolean</c> structures:
///   </para>
///   <list type="table">
///     <listheader>
///       <term>Category</term>
///       <description>Semantics</description>
///     </listheader>
///     <item>
///       <term>Equality</term>
///       <description>
///         The standard equality methods and operators of <c>NaBoolean</c> consider two <see cref="Null"/> values equal.
///         <para>
///           Applies to <see cref="Equals"/>, <see cref="NotEquals"/>, <see cref="operator =="/>, <see cref="operator !="/>.
///         </para>
///       </description>
///     </item>
///     <item>
///       <term>SQL-style Equality</term>
///       <description>
///         The SQL-style equality methods of <c>NaBoolean</c> return <c>NaBoolean.Null</c> if either of the compared values
///         is <see cref="Null"/>. 
///         <para>
///           Applies to <see cref="EqualsSql"/>, <see cref="NotEqualsSql"/>.
///         </para>
///       </description>
///     </item>
///     <item>
///       <term>Relative Comparision using <c>CompareTo</c></term>
///       <description>
///         The CompareTo methods of <c>NaBoolean</c> consider <see cref="Null"/> and null references to be less than any other value.
///         Note that <c>false</c> is considerd greater than <c>true</c> and two <c>Null</c> values are considered equal:
///         <para>
///           <c>Null</c> &lt; <c>true</c> &lt; <c>false</c>
///         </para>
///         <para>
///           Applies to <see cref="CompareTo"/>.
///         </para>
///       </description>
///     </item>
///     <item>
///       <term>Logical</term>
///       <description>
///         The logical methods and operators of <c>NaBoolean</c> return <see cref="Null"/> if eihter of their arguments are <c>Null</c>.
///         <para>
///           Applies to <see cref="And"/>, <see cref="Or"/>, <see cref="Xor"/>, <see cref="Not"/>, 
///           <see cref="operator &amp;"/>, <see cref="operator |"/>, <see cref="operator ^"/>, <see cref="operator !"/>.
///         </para>
///       </description>
///     </item>
///     <item>
///       <term>Type Conversion</term>
///       <description>
///         If a <c>NaBoolean</c> null-value is converted to a <c>Boolean</c>, a <see cref="NaNullValueException"/> is thrown. Conversions 
///         from <c>Boolean</c> to <c>NaBoolean</c>, and conversions to and from <c>SqlBoolean</c> never throw exceptions.
///       </description>
///     </item>
///     <item>
///       <term>Formatting and Parsing</term>
///       <description>
///         <para>
///           If the instance is not <c>Null</c>, <c>ToString</c> returns the same string that <c>Boolean.ToString</c> would return. If it is
///           <c>Null</c>, <c>ToString</c> returns the value <see cref="NullString"/> ("null"). Prefix the format string with the tilde 
///           symbol ("~") to return a zero-length string for <c>Null</c>.
///         </para>
///         <para>
///           <c>Parse</c> returns <c>Null</c> if the string is a null reference, a zero-length string or <see cref="NullString"/> ("null"). 
///           Otherwise, it returns the same value that <c>Boolean.Parse</c> would return.
///         </para>
///       </description>
///     </item>
///   </list>
/// </remarks>
[Serializable]
public struct NaBoolean: INullable, IComparable, ISerializable, IFormattable
{
  #region constants

  private const byte c_null = 0;
  private const byte c_true = 1;
  private const byte c_false = 2;

  #endregion

  #region member fields

  private byte _byteValue;

  #endregion

  #region construction and disposal

  /// <summary>
  /// Initializes a new instance of the <see cref="NaBoolean"/> structure using the supplied boolean value.
  /// </summary>
  /// <param name="value">The boolean to be converted.</param>
  public NaBoolean (Boolean value)
  {
    _byteValue = value ? c_true : c_false;
  }

  private NaBoolean (int isNull)
  {
    _byteValue = c_null;
  }

  #endregion

  #region serialization

  /// <summary>
  /// Serialization constructor. 
  /// </summary>
  /// <remarks>
  /// See <c>ISerializable</c> interface.
  /// </remarks>
  private NaBoolean (SerializationInfo info, StreamingContext context)
  {
    _byteValue = info.GetByte ("Value");
  }

  /// <summary>
  /// Serialization method. 
  /// </summary>
  /// <remarks>
  /// See <c>ISerializable</c> interface.
  /// </remarks>
  public void GetObjectData (SerializationInfo info, StreamingContext context)
  {
    info.AddValue ("Value", _byteValue);
  }

  #endregion

  #region type conversion

  /// <summary>
  /// Converts a <see cref="NaBoolean"/> structure to a <c>String</c>.
  /// </summary>
  /// <returns>
  /// A <c>String</c> object representing the <see cref="Value"/> of this instance of <c>NaBoolean</c>. If this
  /// instance is <c>Null</c>, <see cref="NullString"/> ("null") is returned.
  /// </returns>
  public override string ToString()
  {
    return ToString (null, null);
  }

  /// <summary>
  /// Converts a <see cref="NaBoolean"/> structure to a <c>String</c>.
  /// </summary>
  /// <param name="format">A format specification.</param>
  /// <returns>
  /// A <c>String</c> object representing the <see cref="Value"/> of this instance of <c>NaBoolean</c>. If this
  /// instance is <c>Null</c>, <see cref="NullString"/> ("null") is returned. If <c>format</c> is prefixed with a tilde symbol
  /// ("~"), a zero-length string is returned instead.
  /// </returns>
  public string ToString (string format)
  {
    return ToString (format, null);
  }

  /// <summary>
  /// Converts a <see cref="NaBoolean"/> structure to a <c>String</c>.
  /// </summary>
  /// <param name="provider">An <c>IFormatProvider</c> that supplies culture-specific formatting information. </param>
  /// <returns>
  /// A <c>String</c> object representing the <see cref="Value"/> of this instance of <c>NaBoolean</c>. If this
  /// instance is <c>Null</c>, <see cref="NullString"/> ("null") is returned.
  /// </returns>
  public string ToString (IFormatProvider provider)
  {
    return ToString (null, provider);
  }

  /// <summary>
  /// Converts a <see cref="NaBoolean"/> structure to a <c>String</c>.
  /// </summary>
  /// <param name="format">A format specification.</param>
  /// <param name="provider">An <c>IFormatProvider</c> that supplies culture-specific formatting information. </param>
  /// <returns>
  /// A <c>String</c> object representing the <see cref="Value"/> of this instance of <c>NaBoolean</c>. If this
  /// instance is <c>Null</c>, <see cref="NullString"/> ("null") is returned. If <c>format</c> is prefixed with a tilde symbol
  /// ("~"), a zero-length string is returned instead.
  /// </returns>
  public string ToString (string format, IFormatProvider provider)
  {
    if (IsNull)
    {
      if (format != null && format.Length > 0 && format[0] == '~')
        return string.Empty;
      else
        return NullString;
    }
    else
    {
      return Value.ToString (provider);
    }
  }

  /// <summary>
  /// Converts the <c>String</c> representation of a number to its boolean equivalent.
  /// </summary>
  /// <param name="s">The <c>String</c> to be parsed.</param>
  /// <returns>
  /// An <c>NaBoolean</c> equivalent to the value contained in the specified string. If the string is a null reference, 
  /// a zero-length string or <see cref="NullString"/> ("null"), <c>NaBoolean.Null</c> is returned. Otherwise, 
  /// <see cref="Value"/> contains the same value that <c>Boolean.Parse</c> would return.
  /// </returns>
  public static NaBoolean Parse (string s)
  {
    if (s == null || s.Length == 0 || 0 == string.Compare (s, NullString, false, CultureInfo.InvariantCulture))
      return NaBoolean.Null;
    else return new NaBoolean (Boolean.Parse(s));
  }

  /// <summary>
  /// Converts the supplied <c>Boolean</c> to a <see cref="NaBoolean"/> structure.
  /// </summary>
  /// <param name="value">A <c>Boolean</c> value.</param>
  /// <returns>The converted <see cref="NaBoolean"/> value.</returns>
  public static implicit operator NaBoolean (Boolean value)
  {
    return new NaBoolean (value);
  }

  /// <summary>
  /// Converts the supplied <see cref="NaBoolean"/> structure to an <c>Boolean</c>.
  /// </summary>
  /// <param name="value">A <see cref="NaBoolean"/> structure.</param>
  /// <returns>The converted boolean value.</returns>
  /// <exception cref="NaNullValueException">The passed value is null.</exception>
  public static explicit operator Boolean (NaBoolean value)
  {
    if (value.IsNull)
      throw NaNullValueException.AccessingMember ("NaBoolean to Boolean Conversion");
    return value.Value;
  }

  /// <summary>
  /// Converts the supplied <c>SqlBoolean</c> structure to a <see cref="NaBoolean"/> structure.
  /// </summary>
  public static NaBoolean FromSqlBoolean (SqlBoolean value)
  {
    return (NaBoolean) value;
  }
  /// <summary>
  /// Converts the supplied <c>SqlBoolean</c> structure to a <see cref="NaBoolean"/> structure.
  /// </summary>
  public static implicit operator NaBoolean (SqlBoolean value)
  {
    if (value.IsNull)
      return NaBoolean.Null;
    else
      return new NaBoolean (value.Value);
  }

  /// <summary>
  /// Converts the supplied <see cref="NaBoolean"/> structure to a <c>SqlBoolean</c>structure.
  /// </summary>
  public static SqlBoolean ToSqlBoolean (NaBoolean value)
  {
    return (SqlBoolean) value;
  }

  /// <summary>
  /// Converts the supplied <see cref="NaBoolean"/> structure to a <c>SqlBoolean</c>structure.
  /// </summary>
  public static implicit operator SqlBoolean (NaBoolean value)
  {
    if (value.IsNull)
      return SqlBoolean.Null;
    else
      return new SqlBoolean (value.Value);
  }

  /// <summary>
  /// The true operator can be used to test the <see cref="Value"/> of the <c>SqlBoolean</c> to determine whether it is true.
  /// </summary>
  /// <returns><c>true</c> if the supplied parameter is true, <c>false</c> otherwise.</returns>
  public static bool operator true (NaBoolean x)
  {
    return x._byteValue == c_true;
  }

  /// <summary>
  /// The false operator can be used to test the <see cref="Value"/> of the <c>SqlBoolean</c> to determine whether it is false.
  /// </summary>
  /// <returns><c>true</c> if the supplied parameter is false, <c>false</c> otherwise.</returns>
  public static bool operator false (NaBoolean x)
  {
    return x._byteValue == c_false;
  }

  /// <summary>
  /// Converts a <see cref="NaBoolean"/> value to a boxed <c>Boolean</c> value or a null reference.
  /// </summary>
  /// <returns>The integer value of <c>value</c> as an <c>Boolean</c> if it is not null, a null reference otherwise.</returns>
  /// <remarks>
  /// Use this method to easily pass <c>NaBoolean</c> values to methods that expect an untyped parameter which is either an <c>Boolean</c>
  /// value or a null reference.
  /// </remarks>
  public static object ToBoxedBoolean (NaBoolean value)
  {
    if (value.IsNull )
      return null;
    else
      return value.Value;
  }

  /// <summary>
  /// Converts a boxed <c>Boolean</c> value or a null reference to a <see cref="NaBoolean"/> value.
  /// </summary>
  /// <returns>A <see cref="NaBoolean"/> with its <see cref="Value"/> set to the integer value of <c>value</c> if it is an <c>Boolean</c>, 
  /// <c>NaBoolean.Null</c> if it is a null reference.</returns>
  /// <remarks>
  /// Use this method to easily create an <c>NaBoolean</c> value from an untyped value which is either an <c>Boolean</c> value or a null reference.
  /// </remarks>
  /// <exception cref="ArgumentException"><c>value</c> is neither a null reference nor an <c>Boolean</c> value.</exception>
  public static NaBoolean FromBoxedBoolean (object value)
  {
    if (value == null)
      return NaBoolean.Null;

    if (! (value is Boolean))
      throw new ArgumentException ("Must be a Boolean value", "value");

    return new NaBoolean ((Boolean) value);
  }

  /// <summary>
  /// Converts a <see cref="NaBoolean"/> value to a boxed <c>Boolean</c> value or to DBNull.Value.
  /// </summary>
  /// <returns>The integer value of <c>value</c> as an <c>Boolean</c> if it is not null, DBNull.Value otherwise.</returns>
  /// <remarks>
  /// Use this method to easily pass <c>NaBoolean</c> values to methods that expect an untyped parameter which is either an <c>Boolean</c>
  /// value or DBNull.Value.
  /// </remarks>
  public static object ToBoxedBooleanDBNull (NaBoolean value)
  {
    if (value.IsNull )
      return DBNull.Value;
    else
      return value.Value;
  }

  /// <summary>
  /// Converts a boxed <c>Boolean</c> value or DBNull.Value to a <see cref="NaBoolean"/> value.
  /// </summary>
  /// <returns>A <see cref="NaBoolean"/> with its <see cref="Value"/> set to the integer value of <c>value</c> if it is an <c>Boolean</c>, 
  /// <c>NaBoolean.Null</c> if it is DBNull.Value.</returns>
  /// <remarks>
  /// Use this method to easily create an <c>NaBoolean</c> value from an untyped value which is either an <c>Boolean</c> value or DBNull.Value.
  /// </remarks>
  /// <exception cref="ArgumentException"><c>value</c> is neither DBNull.Value nor an <c>Boolean</c> value.</exception>
  public static NaBoolean FromBoxedBooleanDBNull (object value)
  {
    if (value == DBNull.Value)
      return NaBoolean.Null;

    if (! (value is Boolean))
      throw new ArgumentException ("Must be a Boolean value", "value");

    return new NaBoolean ((Boolean) value);
  }

  #endregion

  #region value

  /// <summary>
  /// Gets the value of this <see cref="NaBoolean"/> structure. This property is read-only.
  /// </summary>
  /// <value>
  /// A Boolean representing the value of this NaBoolean structure.
  /// </value>
  /// <exception cref="NaNullValueException">
  /// The property contains <see cref="Null"/>.
  /// </exception>
  public Boolean Value 
  {
    get 
    { 
      switch (_byteValue)
      {
        case c_true:
          return true;
        case c_false:
          return false;
        default:
          throw NaNullValueException.AccessingMember ("Value");
      }
    }
  }

  #endregion

  #region nullable

  /// <summary>
  /// Represents a null value that can be assigned to the <see cref="Value"/> property of 
  /// an instance of the <see cref="NaBoolean"/> structure.
  /// </summary>
  /// <remarks>
  /// Null functions as a constant for the <see cref="NaBoolean"/> structure.
  /// </remarks>
  public static readonly NaBoolean Null = new NaBoolean(0);

  /// <summary>
  /// Indicates whether or not <see cref="Value"/> is null.
  /// </summary>
  /// <value>
  /// This property is <c>true</c> if <see cref="Value"/> is null, otherwise <c>false</c>.
  /// </value>
  public bool IsNull 
  {
    get { return _byteValue == c_null; }
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
  /// A constant representing the value <c>true</c>.
  /// </summary>
  public static readonly NaBoolean True = new NaBoolean (true);

  /// <summary>
  /// A constant representing the value <c>false</c>.
  /// </summary>
  public static readonly NaBoolean False = new NaBoolean (false);

  #endregion

  #region equality

  /// <summary>
  /// Compares the supplied object parameter to the <see cref="Value"/> property of the <see cref="NaBoolean"/> object.
  /// </summary>
  /// <param name="obj">The object to be compared. </param>
  /// <returns>
  /// <c>true</c> if object is an instance of <see cref="NaBoolean"/> and the two are equal; otherwise <c>false</c>.
  /// If object is a null reference, <c>false</c> is returned.
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// Note that <c>false</c> is returned if object is a Boolean (types are not equal).
  /// </remarks>
  public override bool Equals (object obj)
  {
    if (! (obj is NaBoolean))
      return false; // obj is a null reference or another type then NaBoolean

    return Equals (this, (NaBoolean) obj);
  }

  /// <summary>
  /// Compares the supplied <see cref="NaBoolean"/> parameter to the <see cref="Value"/> property of this instance.
  /// </summary>
  /// <param name="value">The <see cref="NaBoolean"/> instance to be compared. </param>
  /// <returns>
  /// <c>true</c> if the two are equal; otherwise <c>false</c>.
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public bool Equals (NaBoolean value)
  {
    return this._byteValue == value._byteValue;
  }

  /// <summary>
  /// Performs a logical comparison of the two NaBoolean parameters to determine if they are equal.
  /// </summary>
  /// <returns>
  /// <c>true</c> if the two instances are equal or <c>false</c> if they are not equal. 
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public static bool Equals (NaBoolean x, NaBoolean y)
  {
    return x._byteValue == y._byteValue;
  }

  /// <summary>
  /// Performs a logical comparison of the two NaBoolean parameters to determine if they are equal.
  /// </summary>
  /// <returns>
  /// <c>true</c> if the two instances are equal or <c>false</c> if they are not equal. 
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public static bool operator == (NaBoolean x, NaBoolean y)
  {
    return x._byteValue == y._byteValue;
  }

  /// <summary>
  /// Performs a logical comparison of the two NaBoolean parameters to determine if they are not equal.
  /// </summary>
  /// <returns>
  /// <c>false</c> if the two instances are equal or <c>true</c> if they are not equal. 
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="NotEqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public static bool operator != (NaBoolean x, NaBoolean y)
  {
    return x._byteValue != y._byteValue;
  }

  /// <summary>
  /// Performs a logical comparison of the two NaBoolean parameters to determine if they are not equal.
  /// </summary>
  /// <returns>
  /// <c>false</c> if the two instances are equal or <c>true</c> if they are not equal. 
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="NotEqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public static bool NotEquals (NaBoolean x, NaBoolean y)
  {
    return x._byteValue != y._byteValue;
  }

  /// <summary>
  /// Performs a SQL-style comparison of the two NaBoolean parameters to determine if they are equal.
  /// </summary>
  /// <returns>
  /// <c>True</c> if the two instances are equal, or <c>False</c> if they are not equal, or <c>Null</c> if either of them is <c>Null</c>.
  /// </returns>
  public static NaBoolean EqualsSql (NaBoolean x, NaBoolean y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;
    else return new NaBoolean (x._byteValue == y._byteValue);
  }

  /// <summary>
  /// Performs a SQL-style comparison of the two NaBoolean parameters to determine if they are not equal.
  /// </summary>
  /// <returns>
  /// <c>True</c> if the two instances are not equal, or <c>False</c> if they are equal, or <c>Null</c> if either of them is <c>Null</c>.
  /// </returns>
  public static NaBoolean NotEqualsSql (NaBoolean x, NaBoolean y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;
    else return new NaBoolean (x._byteValue != y._byteValue);
  }

  /// <summary>
  /// Returns the hash code for this instance.
  /// </summary>
  public override int GetHashCode()
  {
    return (int) _byteValue;
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
  ///       <description>This instance is less than the object.</description>
  ///     </item>
  ///     <item>
  ///       <term>Zero</term>
  ///       <description>This instance is equal to the object.</description>
  ///     </item>
  ///     <item>
  ///       <term>Greater than zero</term>
  ///       <description>This instance is greater than the object.</description>
  ///     </item>
  ///   </list>
  ///   The CompareTo methods of <c>NaBoolean</c> consider <see cref="Null"/> and null references to be less than any other value.
  ///   Note that 
  ///   <list type="bullets">
  ///     <item>
  ///       <c>false</c> is considerd greater than <c>true</c>,
  ///     </item>
  ///     <item>
  ///       two <c>Null</c> values are considered equal,
  ///     </item>
  ///     <item>
  ///       and <c>Null</c> and a null reference are considered equal.
  ///     </item>
  ///   </list>
  ///   <para>
  ///   <c>Null</c> &lt; <c>true</c> &lt; <c>false</c>
  ///   </para>
  /// </returns>
  public int CompareTo (object obj)
  {
    if (obj == null)
      return IsNull ? 0 : 1;

    if (! (obj is NaBoolean))
      throw new ArgumentException ("obj");

    return CompareTo ((NaBoolean) obj);
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
  ///       <description>This instance is less than the object.</description>
  ///     </item>
  ///     <item>
  ///       <term>Zero</term>
  ///       <description>This instance is equal to the object.</description>
  ///     </item>
  ///     <item>
  ///       <term>Greater than zero</term>
  ///       <description>This instance is greater than the object.</description>
  ///     </item>
  ///   </list>
  ///   The CompareTo methods of <c>NaBoolean</c> consider <see cref="Null"/> and null references to be less than any other value.
  ///   Note that 
  ///   <list type="bullets">
  ///     <item>
  ///       <c>false</c> is considerd greater than <c>true</c>,
  ///     </item>
  ///     <item>
  ///       and two <c>Null</c> values are considered equal,
  ///     </item>
  ///   </list>
  ///   <para>
  ///   <c>Null</c> &lt; <c>true</c> &lt; <c>false</c>
  ///   </para>
  /// </returns>
  public int CompareTo (NaBoolean value)
  {
    if (this._byteValue ==  value._byteValue)
      return 0;
    else if (this._byteValue > value._byteValue)
      return 1;
    else
      return -1;
  }

  #endregion

  #region logical

  /// <summary>
  /// Computes the bitwise AND of two specified <see cref="NaBoolean"/> structures.
  /// </summary>
  /// <returns>
  /// The result of the bitwise AND operation. If either parameter is <c>Null</c>, <c>Null</c> is returned.
  /// </returns>
  public static NaBoolean And (NaBoolean x, NaBoolean y)
  {
    return x & y;
  }

  /// <summary>
  /// Computes the bitwise AND of two specified <see cref="NaBoolean"/> structures.
  /// </summary>
  /// <returns>
  /// The result of the bitwise AND operation. If either parameter is <c>Null</c>, <c>Null</c> is returned.
  /// </returns>
  public static NaBoolean operator & (NaBoolean x, NaBoolean y)
  {
	  if (x.IsNull || y.IsNull)
		  return NaBoolean.Null;

    return (x._byteValue == c_true && y._byteValue == c_true);
  }

  /// <summary>
  /// Computes the bitwise OR of two specified <see cref="NaBoolean"/> structures.
  /// </summary>
  /// <returns>
  /// The result of the bitwise OR operation. If either parameter is <c>Null</c>, <c>Null</c> is returned.
  /// </returns>
  public static NaBoolean Or (NaBoolean x, NaBoolean y)
  {
    return x | y;
  }

  /// <summary>
  /// Computes the bitwise OR of two specified <see cref="NaBoolean"/> structures.
  /// </summary>
  /// <returns>
  /// The result of the bitwise OR operation. If either parameter is <c>Null</c>, <c>Null</c> is returned.
  /// </returns>
  public static NaBoolean operator | (NaBoolean x, NaBoolean y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;

    return (x._byteValue == c_true || y._byteValue == c_true);
  }

  /// <summary>
  /// Computes the bitwise exclusive-OR of two specified <see cref="NaBoolean"/> structures.
  /// </summary>
  /// <returns>
  /// The result of the bitwise exclusive-OR operation. If either parameter is <c>Null</c>, <c>Null</c> is returned.
  /// </returns>
  public static NaBoolean Xor (NaBoolean x, NaBoolean y)
  {
    return x ^ y;
  }

  /// <summary>
  /// Computes the bitwise exclusive-OR of two specified <see cref="NaBoolean"/> structures.
  /// </summary>
  /// <returns>
  /// The result of the bitwise exclusive-OR operation. If either parameter is <c>Null</c>, <c>Null</c> is returned.
  /// </returns>
  public static NaBoolean operator ^ (NaBoolean x, NaBoolean y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;

    return ((x._byteValue == c_true) != (y._byteValue == c_true));
  }

  /// <summary>
  /// Performs a NOT operation on a <see cref="NaBoolean"/>.
  /// </summary>
  /// <returns>
  /// A <c>SqlBoolean</c> with the <see cref="Value"/> <c>True</c> if argument was <c>False</c>, <c>Null</c> if argument was <c>Null</c>, 
  /// and <c>False</c> otherwise.
  /// </returns>
  public static NaBoolean Not (NaBoolean x)
  {
    return !x;
  }

  /// <summary>
  /// Performs a NOT operation on a <see cref="NaBoolean"/>.
  /// </summary>
  /// <returns>
  /// A <c>SqlBoolean</c> with the <see cref="Value"/> <c>True</c> if argument was <c>False</c>, <c>Null</c> if argument was <c>Null</c>, 
  /// and <c>False</c> otherwise.
  /// </returns>
  public static NaBoolean operator ! (NaBoolean x)
  {
    switch (x._byteValue)
    {
      case c_true:
        return False;
      case c_false:
        return True;
      default:
        return NaBoolean.Null;
    }
  }

  #endregion

}


}
