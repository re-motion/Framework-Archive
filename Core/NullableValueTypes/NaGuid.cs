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
/// Represents a globally unique identifier (GUID) that can be <c>Null</c>. The corresponding system type is System.Guid.
/// </summary>
/// <include file='doc\include\include.xml' path='Comments/NaGuid/remarks' />
[Serializable]
[NaBasicType (typeof (Guid))]
[TypeConverter (typeof (NaGuidConverter))]
public struct NaGuid: INaNullable, IFormattable, IXmlSerializable
{
  #region member fields

  private Guid _value;
  private bool _isNotNull;

  #endregion

  #region construction and disposal

  /// <summary>
  /// Initializes a new instance of the <see cref="NaGuid"/> structure using the supplied Guid value.
  /// </summary>
  /// <param name="value">The Guid to be converted.</param>
  public NaGuid (Guid value)
  {
    _value = value;
    _isNotNull = true;
  }

  private NaGuid (bool isNull)
  {
    _value = Guid.Empty;
    _isNotNull = ! isNull;
  }

  #endregion

  #region serialization

  static XmlSchema s_schema = null;
  
  XmlSchema IXmlSerializable.GetSchema()
  {
    if (s_schema == null)
    {
      lock (typeof (NaGuid))
      {
        if (s_schema == null)
          s_schema = NaTypeUtility.CreateXmlSchema (typeof(NaGuid).Name, "string");
      }
    }
    return s_schema;
  }

  void IXmlSerializable.ReadXml (XmlReader reader)
  {
    string strValue = NaTypeUtility.ReadXml (reader);
    _isNotNull = strValue != null;
    if (strValue != null)
      _value = XmlConvert.ToGuid (strValue);
  }

  void IXmlSerializable.WriteXml (XmlWriter writer)
  {
    NaTypeUtility.WriteXml (writer, IsNull ? null : XmlConvert.ToString (_value));
  }

  #endregion

  #region type conversion

  /// <summary>
  /// Converts a <see cref="NaGuid"/> structure to a <c>String</c>.
  /// </summary>
  /// <returns>
  /// A <c>String</c> object representing the <see cref="Value"/> of this instance of <c>NaGuid</c>. If this
  /// instance is <c>Null</c>, <see cref="NullString"/> ("null") is returned.
  /// </returns>
  public override string ToString()
  {
    return ToString (null, null);
  }

  /// <summary>
  /// Converts a <see cref="NaGuid"/> structure to a <c>String</c>.
  /// </summary>
  /// <param name="format">A format specification.</param>
  /// <returns>
  /// A <c>String</c> object representing the <see cref="Value"/> of this instance of <c>NaGuid</c>. If this
  /// instance is <c>Null</c>, <see cref="NullString"/> ("null") is returned. If <c>format</c> is prefixed with a tilde symbol
  /// ("~"), a zero-length string is returned instead.
  /// </returns>
  public string ToString (string format)
  {
    return ToString (format, null);
  }

  /// <summary>
  /// Converts a <see cref="NaGuid"/> structure to a <c>String</c>.
  /// </summary>
  /// <param name="provider">An <c>IFormatProvider</c> that supplies culture-specific formatting information. </param>
  /// <returns>
  /// A <c>String</c> object representing the <see cref="Value"/> of this instance of <c>NaGuid</c>. If this
  /// instance is <c>Null</c>, <see cref="NullString"/> ("null") is returned.
  /// </returns>
  public string ToString (IFormatProvider provider)
  {
    return ToString (null, provider);
  }

  /// <summary>
  /// Converts a <see cref="NaGuid"/> structure to a <c>String</c>.
  /// </summary>
  /// <param name="format">A format specification.</param>
  /// <param name="provider">An <c>IFormatProvider</c> that supplies culture-specific formatting information. </param>
  /// <returns>
  /// A <c>String</c> object representing the <see cref="Value"/> of this instance of <c>NaGuid</c>. If this
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
  /// Converts the supplied <c>Guid</c> to a <see cref="NaGuid"/> structure.
  /// </summary>
  /// <param name="value">A <c>Guid</c> value.</param>
  /// <returns>The converted <see cref="NaGuid"/> value.</returns>
  public static implicit operator NaGuid (Guid value)
  {
    return new NaGuid (value);
  }

  /// <summary>
  /// Converts the supplied <see cref="NaGuid"/> structure to a <c>Guid</c>.
  /// </summary>
  /// <param name="value">A <see cref="NaGuid"/> structure.</param>
  /// <returns>The converted Guid value.</returns>
  /// <exception cref="NaNullValueException">
  /// The passed value is null.
  /// </exception>
  public static explicit operator Guid (NaGuid value)
  {
    if (value.IsNull)
      throw NaNullValueException.AccessingMember ("NaGuid to Guid Conversion");
    return value._value;
  }

  /// <summary>
  /// Converts the supplied <c>SqlGuid</c> structure to a <see cref="NaGuid"/> structure.
  /// </summary>
  public static NaGuid FromSqlGuid (SqlGuid value)
  {
    return (NaGuid) value;
  }

  /// <summary>
  /// Converts the supplied <c>SqlGuid</c> structure to a <see cref="NaGuid"/> structure.
  /// </summary>
  public static implicit operator NaGuid (SqlGuid value)
  {
    if (value.IsNull)
      return NaGuid.Null;
    else
      return new NaGuid (value.Value);
  }

  /// <summary>
  /// Converts the supplied <see cref="NaGuid"/> structure to a <c>SqlGuid</c>structure.
  /// </summary>
  public static SqlGuid ToSqlGuid (NaGuid value)
  {
    return (SqlGuid) value;
  }

  /// <summary>
  /// Converts the supplied <see cref="NaGuid"/> structure to a <c>SqlGuid</c>structure.
  /// </summary>
  public static implicit operator SqlGuid (NaGuid value)
  {
    if (value.IsNull)
      return SqlGuid.Null;
    else
      return new SqlGuid (value.Value);
  }

  /// <summary>
  /// Converts a <see cref="NaGuid"/> value to a boxed <c>Guid</c> value or a null reference.
  /// </summary>
  /// <returns>The Guid value of <c>value</c> as an <c>Guid</c> if it is not null, a null reference otherwise.</returns>
  /// <remarks>
  /// Use this method to easily pass <c>NaGuid</c> values to methods that expect an untyped parameter which is either a <c>Guid</c>
  /// value or a null reference.
  /// </remarks>
  public static object ToBoxedGuid (NaGuid value)
  {
    if (value.IsNull )
      return null;
    else
      return value._value;
  }

  /// <summary>
  /// Converts a boxed <c>NaGuid</c> value or a null reference to a <see cref="NaGuid"/> value.
  /// </summary>
  /// <returns>A <see cref="NaGuid"/> with its <see cref="Value"/> set to the Guid value of <c>value</c> if it is an <c>Guid</c>, 
  /// <c>NaGuid.Null</c> if it is a null reference.</returns>
  /// <remarks>
  /// Use this method to easily create an <c>NaGuid</c> value from an untyped value which is either an <c>Guid</c> value or a null reference.
  /// </remarks>
  /// <exception cref="ArgumentException"><c>value</c> is neither a null reference nor an <c>Guid</c> value.</exception>
  public static NaGuid FromBoxedGuid (object value)
  {
    if (value == null)
      return NaGuid.Null;

    if (! (value is Guid))
      throw new ArgumentException ("Must be a Guid value", "value");

    return new NaGuid ((Guid) value);
  }

  /// <summary>
  /// Converts a <see cref="NaGuid"/> value to a boxed <c>Guid</c> value or to DBNull.Value.
  /// </summary>
  /// <returns>The Guid value of <c>value</c> as a <c>Guid</c> if it is not null, DBNull.Value otherwise.</returns>
  /// <remarks>
  /// Use this method to easily pass <c>NaGuid</c> values to methods that expect an untyped parameter which is either a <c>Guid</c>
  /// value or DBNull.Value.
  /// </remarks>
  public static object ToBoxedGuidDBNull (NaGuid value)
  {
    if (value.IsNull )
      return DBNull.Value;
    else
      return value._value;
  }

  /// <summary>
  /// Converts a boxed <c>Guid</c> value or DBNull.Value to a <see cref="NaGuid"/> value.
  /// </summary>
  /// <returns>A <see cref="NaGuid"/> with its <see cref="Value"/> set to the Guid value of <c>value</c> if it is a <c>Guid</c>, 
  /// <c>NaGuid.Null</c> if it is DBNull.Value.</returns>
  /// <remarks>
  /// Use this method to easily create a <c>NaGuid</c> value from an untyped value which is either a <c>Guid</c> value or DBNull.Value.
  /// </remarks>
  /// <exception cref="ArgumentException"><c>value</c> is neither DBNull.Value nor an <c>Int32</c> value.</exception>
  public static NaGuid FromBoxedGuidDBNull (object value)
  {
    if (value == DBNull.Value)
      return NaGuid.Null;

    if (! (value is Guid))
      throw new ArgumentException ("Must be a Guid value", "value");

    return new NaGuid ((Guid) value);
  }

  #endregion

  #region value

  /// <summary>
  /// Gets the value of this <see cref="NaGuid"/> structure. This property is read-only.
  /// </summary>
  /// <value>
  /// A Guid representing the value of this NaGuid structure.
  /// </value>
  /// <exception cref="NaNullValueException">
  /// The property contains <see cref="Null"/>.
  /// </exception>
  public Guid Value 
  {
    get 
    { 
      if (IsNull)
        throw NaNullValueException.AccessingMember ("Value");
      return _value;
    }
  }


  /// <summary>
  /// Gets the value of this <see cref="NaGuid"/> structure. This property is read-only.
  /// </summary>
  /// <value>
  /// An Object representing the value of this NaGuid structure.
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
  /// Represents a null value that can be assigned to an instance of the <see cref="NaGuid"/> structure.
  /// </summary>
  /// <remarks>
  /// Null functions as a constant for the <see cref="NaGuid"/> structure.
  /// </remarks>
  public static readonly NaGuid Null = new NaGuid (true);

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
  /// Represents an empty Guid that can be assigned to the <see cref="Value"/> property of 
  /// an instance of the <see cref="NaGuid"/> structure.
  /// </summary>
  /// <remarks>
  /// Empty functions as a constant for the <see cref="NaGuid"/> structure.
  /// </remarks>
  public static readonly NaGuid Empty = new NaGuid (Guid.Empty);

  #endregion

  #region equality

  /// <summary>
  /// Compares the supplied object parameter to the <see cref="Value"/> property of the <see cref="NaGuid"/> object.
  /// </summary>
  /// <param name="obj">The object to be compared. </param>
  /// <returns>
  /// <c>true</c> if object is an instance of <see cref="NaGuid"/> and the two are equal; otherwise <c>false</c>.
  /// If object is a null reference, <c>false</c> is returned.
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// Note that <c>false</c> is returned if object is a Guid (types are not equal).
  /// </remarks>
  public override bool Equals (object obj)
  {
    if (! (obj is NaGuid))
      return false; // obj is a null reference or another type then NaGuid

    return Equals (this, (NaGuid) obj);
  }

  /// <summary>
  /// Compares the supplied <see cref="NaGuid"/> parameter to the <see cref="Value"/> property of this instance.
  /// </summary>
  /// <param name="value">The <see cref="NaGuid"/> instance to be compared. </param>
  /// <returns>
  /// <c>true</c> if the two are equal; otherwise <c>false</c>.
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public bool Equals (NaGuid value)
  {
    return Equals (this, value);
  }

  /// <summary>
  /// Performs a logical comparison of the two NaGuid parameters to determine if they are equal.
  /// </summary>
  /// <returns>
  /// <c>true</c> if the two instances are equal or <c>false</c> if they are not equal. 
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public static bool Equals (NaGuid x, NaGuid y)
  {
    return x == y;
  }

  /// <summary>
  /// Performs a logical comparison of the two NaGuid parameters to determine if they are equal.
  /// </summary>
  /// <returns>
  /// <c>true</c> if the two instances are equal or <c>false</c> if they are not equal. 
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public static bool operator == (NaGuid x, NaGuid y)
  {
    if (x.IsNull && y.IsNull)
      return true;
    if (x.IsNull != y.IsNull)
      return false;
    return x._value == y._value;
  }

  /// <summary>
  /// Performs a logical comparison of the two NaGuid parameters to determine if they are not equal.
  /// </summary>
  /// <returns>
  /// <c>false</c> if the two instances are equal or <c>true</c> if they are not equal. 
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="NotEqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public static bool operator != (NaGuid x, NaGuid y)
  {
    return ! (x == y);
  }

  /// <summary>
  /// Performs a logical comparison of the two NaGuid parameters to determine if they are not equal.
  /// </summary>
  /// <returns>
  /// <c>false</c> if the two instances are equal or <c>true</c> if they are not equal. 
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="NotEqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// </remarks>
  public static bool NotEquals (NaGuid x, NaGuid y)
  {
    return x != y;
  }

  /// <summary>
  /// Performs a SQL-style comparison of the two NaGuid parameters to determine if they are equal.
  /// </summary>
  /// <returns>
  /// <c>True</c> if the two instances are equal, or <c>False</c> if they are not equal, or <c>Null</c> if either of them is <c>Null</c>.
  /// </returns>
  public static NaBoolean EqualsSql (NaGuid x, NaGuid y)
  {
    if (x.IsNull || y.IsNull)
      return NaBoolean.Null;
    else 
      return new NaBoolean (x._value == y._value);
  }

  /// <summary>
  /// Performs a SQL-style comparison of the two NaGuid parameters to determine if they are not equal.
  /// </summary>
  /// <returns>
  /// <c>True</c> if the two instances are not equal, or <c>False</c> if they are equal, or <c>Null</c> if either of them is <c>Null</c>.
  /// </returns>
  public static NaBoolean NotEqualsSql (NaGuid x, NaGuid y)
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

  #endregion
}

}
