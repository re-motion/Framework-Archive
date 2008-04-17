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
/// Represents a globally unique identifier (GUID) that can be <see langword="null"/>. The corresponding system type is System.Guid.
/// </summary>
/// <include file='doc\include\NullableValueTypes\include.xml' path='Comments/NaGuid/remarks' />
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
  /// Initializes a new instance of the <see cref="NaGuid"/> structure using the value represented by the specified string.
  /// </summary>
  /// <param name="value">A <see cref="System.String"/> that contains a GUID. See <see cref="System.Guid"/> for valid formats.</param>
  public NaGuid (string value) : this (new Guid (value))
  {
  }

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
  /// Converts an <see cref="NaGuid"/> structure to a <see cref="String"/>.
  /// </summary>
  /// <returns>
  /// A <see cref="String"/> object representing the <see cref="Value"/> of this instance of <see cref="NaGuid"/>. If this
  /// instance is <see langword="null"/>, <see cref="NullString"/> ("null") is returned.
  /// </returns>
  public override string ToString()
  {
    return ToString (null, null);
  }

  /// <summary>
  /// Converts an <see cref="NaGuid"/> structure to a <see cref="String"/>.
  /// </summary>
  /// <param name="format">A format specification.</param>
  /// <returns>
  /// A <see cref="String"/> object representing the <see cref="Value"/> of this instance of <see cref="NaGuid"/>. If this
  /// instance is <see langword="null"/>, <see cref="NullString"/> ("null") is returned. If <paramref name="format"/> is prefixed with a tilde symbol
  /// ("~"), a zero-length string is returned instead.
  /// </returns>
  public string ToString (string format)
  {
    return ToString (format, null);
  }

  /// <summary>
  /// Converts an <see cref="NaGuid"/> structure to a <see cref="String"/>.
  /// </summary>
  /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information. </param>
  /// <returns>
  /// A <see cref="String"/> object representing the <see cref="Value"/> of this instance of <see cref="NaGuid"/>. If this
  /// instance is <see langword="null"/>, <see cref="NullString"/> ("null") is returned.
  /// </returns>
  public string ToString (IFormatProvider provider)
  {
    return ToString (null, provider);
  }

  /// <summary>
  /// Converts an <see cref="NaGuid"/> structure to a <see cref="String"/>.
  /// </summary>
  /// <param name="format">A format specification.</param>
  /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information. </param>
  /// <returns>
  /// A <see cref="String"/> object representing the <see cref="Value"/> of this instance of <see cref="NaGuid"/>. If this
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
  /// Converts the <see cref="String"/> representation of a GUID to its NaGuid equivalent.
  /// </summary>
  /// <param name="s">The <see cref="String"/> to be parsed.</param>
  /// <returns>
  /// An <see cref="NaGuid"/> equivalent to the value contained in the specified string. 
  /// If the string is a null reference <see cref="NaGuid.Null"/> is returned. 
  /// If the string is a zero-length string an NaGuid with a <see cref="Value"/> equal to <see cref="System.Guid.Empty"/> is returned.
  /// Otherwise, <see cref="Value"/> contains the <see cref="System.Guid"/> equivalent of the provided <see cref="String"/>.
  /// </returns>
  public static NaGuid Parse (string s)
  {
    if (s == null || s.Length == 0 || 0 == string.Compare (s, NullString, false, CultureInfo.InvariantCulture)) 
      return NaGuid.Null;
    return new NaGuid (s);
  }

  /// <summary>
  /// Converts the supplied <see cref="Guid"/> to an <see cref="NaGuid"/> structure.
  /// </summary>
  /// <param name="value">A <see cref="Guid"/> value.</param>
  /// <returns>The converted <see cref="NaGuid"/> value.</returns>
  public static implicit operator NaGuid (Guid value)
  {
    return new NaGuid (value);
  }

  /// <summary>
  /// Converts the supplied <see cref="NaGuid"/> structure to a <see cref="Guid"/>.
  /// </summary>
  /// <param name="value">An <see cref="NaGuid"/> structure.</param>
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
  /// Converts the supplied <see cref="SqlGuid"/> structure to an <see cref="NaGuid"/> structure.
  /// </summary>
  public static NaGuid FromSqlGuid (SqlGuid value)
  {
    return (NaGuid) value;
  }

  /// <summary>
  /// Converts the supplied <see cref="SqlGuid"/> structure to an <see cref="NaGuid"/> structure.
  /// </summary>
  public static implicit operator NaGuid (SqlGuid value)
  {
    if (value.IsNull)
      return NaGuid.Null;
    else
      return new NaGuid (value.Value);
  }

  /// <summary>
  /// Converts the supplied <see cref="NaGuid"/> structure to a <see cref="SqlGuid"/>structure.
  /// </summary>
  public static SqlGuid ToSqlGuid (NaGuid value)
  {
    return (SqlGuid) value;
  }

  /// <summary>
  /// Converts the supplied <see cref="NaGuid"/> structure to a <see cref="SqlGuid"/>structure.
  /// </summary>
  public static implicit operator SqlGuid (NaGuid value)
  {
    if (value.IsNull)
      return SqlGuid.Null;
    else
      return new SqlGuid (value.Value);
  }

  /// <summary>
  /// Converts an <see cref="NaGuid"/> value to a boxed <see cref="Guid"/> value or a null reference.
  /// </summary>
  /// <returns>The Guid value of <paramref name="value"/> as an <see cref="Guid"/> if it is not null, a null reference otherwise.</returns>
  /// <remarks>
  /// Use this method to easily pass <see cref="NaGuid"/> values to methods that expect an untyped parameter which is either a <see cref="Guid"/>
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
  /// Converts a boxed <see cref="NaGuid"/> value or a null reference to an <see cref="NaGuid"/> value.
  /// </summary>
  /// <returns>An <see cref="NaGuid"/> with its <see cref="Value"/> set to the Guid value of <paramref name="value"/> if it is an <see cref="Guid"/>, 
  /// <see cref="NaGuid.Null"/> if it is a null reference.</returns>
  /// <remarks>
  /// Use this method to easily create an <see cref="NaGuid"/> value from an untyped value which is either an <see cref="Guid"/> value or a null reference.
  /// </remarks>
  /// <exception cref="ArgumentException"><paramref name="value"/> is neither a null reference nor an <see cref="Guid"/> value.</exception>
  public static NaGuid FromBoxedGuid (object value)
  {
    if (value == null)
      return NaGuid.Null;

    if (! (value is Guid))
      throw new ArgumentException ("Must be a Guid value", "value");

    return new NaGuid ((Guid) value);
  }

  /// <summary>
  /// Converts an <see cref="NaGuid"/> value to a boxed <see cref="Guid"/> value or to DBNull.Value.
  /// </summary>
  /// <returns>The Guid value of <paramref name="value"/> as a <see cref="Guid"/> if it is not null, DBNull.Value otherwise.</returns>
  /// <remarks>
  /// Use this method to easily pass <see cref="NaGuid"/> values to methods that expect an untyped parameter which is either a <see cref="Guid"/>
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
  /// Converts a boxed <see cref="Guid"/> value or DBNull.Value to an <see cref="NaGuid"/> value.
  /// </summary>
  /// <returns>An <see cref="NaGuid"/> with its <see cref="Value"/> set to the Guid value of <paramref name="value"/> if it is a <see cref="Guid"/>, 
  /// <see cref="NaGuid.Null"/> if it is DBNull.Value.</returns>
  /// <remarks>
  /// Use this method to easily create an <see cref="NaGuid"/> value from an untyped value which is either a <see cref="Guid"/> value or DBNull.Value.
  /// </remarks>
  /// <exception cref="ArgumentException"><paramref name="value"/> is neither DBNull.Value nor an <see cref="Int32"/> value.</exception>
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
  /// <see langword="true"/> if object is an instance of <see cref="NaGuid"/> and the two are equal; otherwise <see langword="false"/>.
  /// If object is a null reference, <see langword="false"/> is returned.
  /// </returns>
  /// <remarks>
  /// If both parameters are <see cref="Null"/>, they are considered equal. Use <see cref="EqualsSql"/> if you require SQL-style 
  /// equality logic.
  /// Note that <see langword="false"/> is returned if object is a Guid (types are not equal).
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
  /// <see langword="true"/> if the two are equal; otherwise <see langword="false"/>.
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
  /// <see langword="true"/> if the two instances are equal or <see langword="false"/> if they are not equal. 
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
  /// <see langword="true"/> if the two instances are equal or <see langword="false"/> if they are not equal. 
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
  /// <see langword="false"/> if the two instances are equal or <see langword="true"/> if they are not equal. 
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
  /// <see langword="false"/> if the two instances are equal or <see langword="true"/> if they are not equal. 
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
  /// <see cref="NaBoolean.True"/> if the two instances are equal, or <see cref="NaBoolean.False"/> if they are not equal, or <see langword="null"/> if either of them is <see langword="null"/>.
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
  /// <see cref="NaBoolean.True"/> if the two instances are not equal, or <see cref="NaBoolean.False"/> if they are equal, or <see langword="null"/> if either of them is <see langword="null"/>.
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
