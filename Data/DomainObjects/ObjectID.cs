using System;

namespace Rubicon.Data.DomainObjects
{
/// <summary>
/// Uniquely identifies a domain object.
/// </summary>
public class ObjectID
{
  // types

  // static members and constants

  private const char c_delimiter = '|';
  private const string c_escapedDelimiter = "&pipe;";
  private const string c_escapedDelimiterPlaceholder = "&amp;pipe;";

  public static bool operator == (ObjectID id1, ObjectID id2)
  {
    return Equals (id1, id2);
  }

  public static bool operator != (ObjectID id1, ObjectID id2)
  {
    return !Equals (id1, id2);
  }

  public static bool Equals (ObjectID id1, ObjectID id2)
  {
    if (object.ReferenceEquals (id1, id2)) return true;
    if (object.ReferenceEquals (id1, null)) return false;

    return id1.Equals (id2);
  }

  /// <summary>
  /// Converts the string representation of the ID to an <see cref="ObjectID"/> instance.
  /// </summary>
  /// <param name="objectIDString">A string containing the object ID to convert.</param>
  /// <returns>
  ///   An <see cref="ObjectID"/> instance equivalent to the object ID contained in <i>objectIDString</i>.
  /// </returns>
  /// <exception cref="ArgumentNullException"><i>objectIDString</i> is a null reference.</exception>
  /// <exception cref="ArgumentEmptyException"><i>objectIDString</i> is an empty string.</exception>
  /// <exception cref="FormatException">
  ///   <i>objectIDString</i> does not contain the string representation of an object ID.
  /// </exception>
  public static ObjectID Parse (string objectIDString)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("objectIDString", objectIDString);

    string[] parts = objectIDString.Split (c_delimiter);

    if (parts.Length != 4)
    {
      throw new FormatException (string.Format (
          "Serialized ObjectID '{0}' is not correctly formatted.",
          objectIDString));
    }

    for (int i = 0; i < parts.Length; i++)
    {
      parts[i] = Unescape (parts[i]);
    }

    object value = GetValue (parts[3], parts[2]);

    return new ObjectID (parts[0], parts[1], value);
  }

  /// <summary>
  /// Determines whether a type can be used as an ID value.
  /// </summary>
  /// <param name="valueType">The <see cref="Type"/> which should be used as an ID value.</param>
  /// <returns>
  ///   <b>true</b> if <i>valueType</i> can be used as an ID value; otherwise, <b>false</b>.
  /// </returns>
  public static bool IsValidType (Type valueType)
  {
    ArgumentUtility.CheckNotNull ("valueType", valueType);

    return (valueType == typeof (Guid)
        || valueType == typeof (int)
        || valueType == typeof (string));
  }

  private static object GetValue (string typeName, string value)
  {
    Type type = Type.GetType (typeName);

    if (type == typeof (Guid))
      return new Guid (value);
    else if (type == typeof (int))
      return int.Parse (value);
    else if (type == typeof (string))
      return value;
    else
      throw new FormatException (string.Format ("Type '{0}' is not supported.", typeName));
  }

  private static string Unescape (string value)
  {
    if (value.IndexOf (c_escapedDelimiter) >= 0)
      value = value.Replace (c_escapedDelimiter, c_delimiter.ToString ());

    if (value.IndexOf (c_escapedDelimiterPlaceholder) >= 0)
      value = value.Replace (c_escapedDelimiterPlaceholder, c_escapedDelimiter);

    return value;
  }

  // member fields

  private string _storageProviderID;
  private object _value;
  private string _classID;

  // construction and disposing

  /// <summary>
  /// Initializes a new instance of the <b>ObjectID</b> class with the specified storage provider ID,
  /// class ID and ID value.
  /// </summary>
  /// <param name="storageProviderID">
  ///   The ID of the <see cref="StorageProvider"/> which stores the object.
  /// </param>
  /// <param name="classID">The ID of the class of the object.</param>
  /// <param name="value">The ID value used to identify the object in the storage provider.</param>
  /// <exception cref="ArgumentNullException">
  ///   <i>storageProviderID</i>, <i>classID</i> or <i>value</i> is a null reference.
  /// </exception>
  /// <exception cref="ArgumentEmptyException">
  ///   <i>storageProviderID</i> or <i>classID</i> is an empty string.
  /// </exception>
  /// <exception cref="ArgumentException">
  ///   <i>value</i> has an unsupported type or is a string and contains invalid characters.
  /// </exception>
  public ObjectID (string storageProviderID, string classID, object value)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);
    ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);
    ArgumentUtility.CheckNotNull ("value", value);
    CheckValue ("value", value);

    _classID = classID;
    _storageProviderID = storageProviderID;
    _value = value;
  }

  // methods and properties

  /// <summary>
  /// Gets the ID of the <see cref="StorageProvider"/> which stores the object.
  /// </summary>
  public string StorageProviderID
  {
    get { return _storageProviderID; }
  }

  /// <summary>
  /// Gets the ID value used to identify the object in the storage provider.
  /// </summary>
  /// <exception cref="ArgumentException">
  ///   The value has an unsupported type or is a string and contains invalid characters.
  /// </exception>
  public object Value
  {
    get { return _value; }
  }

  /// <summary>
  /// The class ID of the object class.
  /// </summary>
  public string ClassID
  {
    get { return _classID; }
  }

  /// <summary>
  /// Returns the string representation of the current <see cref="ObjectID"/>.
  /// </summary>
  /// <returns>A <see cref="String"/> that represents the current <see cref="ObjectID"/>.</returns>
  public override string ToString ()
  {
    Type valueType = _value.GetType();

    return Escape (_storageProviderID) + c_delimiter + 
        Escape (_classID) + c_delimiter + 
        Escape (_value.ToString ()) + c_delimiter + 
        Escape (valueType.FullName);
  }

  /// <summary>
  /// Returns the hash code for this instance.
  /// </summary>
  /// <returns>A 32-bit signed integer hash code.</returns>
  public override int GetHashCode()
  {
    return _storageProviderID.GetHashCode () ^ _classID.GetHashCode () ^ _value.GetHashCode ();
  }

  public override bool Equals (object obj)
  {
    if (obj == null) return false;
    if (this.GetType () != obj.GetType ()) return false;
    
    ObjectID other = (ObjectID) obj;
    if (!object.Equals (this._storageProviderID, other._storageProviderID)) return false;
    if (!object.Equals (this._classID, other._classID)) return false;
    if (!object.Equals (this._value, other._value)) return false;

    return true;
  }

  private void CheckValue (string argumentName, object value)
  {
    Type valueType = value.GetType ();

    if (!IsValidType (valueType))
    {
      throw new ArgumentException (string.Format (
          "Type '{0}' is not supported.", valueType.FullName), 
          argumentName);
    }

    if (valueType == typeof (string) && ((string) value).IndexOf (c_escapedDelimiterPlaceholder) >= 0)
    {
      throw new ArgumentException (string.Format (
          "Value cannot contain '{0}'.", c_escapedDelimiterPlaceholder), "value");
    }
  }

  private string Escape (string value)
  {
    if (value.IndexOf (c_escapedDelimiter) >= 0)
      value = value.Replace (c_escapedDelimiter, c_escapedDelimiterPlaceholder);

    if (value.IndexOf (c_delimiter) >= 0)
      value = value.Replace (c_delimiter.ToString (), c_escapedDelimiter);

    return value;
  }
}
}
