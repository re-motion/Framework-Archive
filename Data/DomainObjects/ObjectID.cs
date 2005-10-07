using System;
using System.Runtime.Serialization;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects
{
/// <summary>
/// Uniquely identifies a domain object.
/// </summary>
[Serializable]
public class ObjectID : ISerializable
{
  // types

  // static members and constants

  private const char c_delimiter = '|';
  private const string c_escapedDelimiter = "&pipe;";
  private const string c_escapedDelimiterPlaceholder = "&amp;pipe;";

  /// <summary>
  /// Tests whether two specified <see cref="ObjectID"/> objects are equivalent.
  /// </summary>
  /// <param name="id1">The <see cref="ObjectID"/> object that is to the left of the equality operator.</param>
  /// <param name="id2">The <see cref="ObjectID"/> object that is to the right of the equality operator.</param>
  /// <returns></returns>
  public static bool operator == (ObjectID id1, ObjectID id2)
  {
    return Equals (id1, id2);
  }

  /// <summary>
  /// Tests whether two specified <see cref="ObjectID"/> objects are different.
  /// </summary>
  /// <param name="id1">The <see cref="ObjectID"/> object that is to the left of the inequality operator.</param>
  /// <param name="id2">The <see cref="ObjectID"/> object that is to the right of the inequality operator.</param>
  /// <returns></returns>
  public static bool operator != (ObjectID id1, ObjectID id2)
  {
    return !Equals (id1, id2);
  }

  /// <summary>
  /// Determines whether the specified <see cref="ObjectID"/> instances are considered equal.
  /// </summary>
  /// <param name="id1">The first <see cref="ObjectID"/> to compare.</param>
  /// <param name="id2">The second <see cref="ObjectID"/> to compare.</param>
  /// <returns><b>true</b> if the both <see cref="ObjectID"/>s are equal; otherwise, <b>false</b>.</returns>
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
  ///   An <see cref="ObjectID"/> instance equivalent to the object ID contained in <i>objectIDString</i>. Must not be <see langword="null"/>.
  /// </returns>
  /// <exception cref="System.ArgumentNullException"><i>objectIDString</i> is <see langword="null"/>.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>objectIDString</i> is an empty string.</exception>
  /// <exception cref="System.FormatException">
  ///   <i>objectIDString</i> does not contain the string representation of an object ID.
  /// </exception>
  public static ObjectID Parse (string objectIDString)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("objectIDString", objectIDString);

    string[] parts = objectIDString.Split (c_delimiter);

    if (parts.Length != 3)
    {
      throw new FormatException (string.Format (
          "Serialized ObjectID '{0}' is not correctly formatted.",
          objectIDString));
    }

    for (int i = 0; i < parts.Length; i++)
    {
      parts[i] = Unescape (parts[i]);
    }

    object value = GetValue (parts[2], parts[1]);

    return new ObjectID (parts[0], value);
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

  private ClassDefinition _classDefinition;
  private object _value;

  // construction and disposing


  /// <summary>
  /// Initializes a new instance of the <b>ObjectID</b> class with the specified class ID and ID value.
  /// </summary>
  /// <param name="classID">The ID of the class of the object. Must not be <see langword="null"/>.</param>
  /// <param name="value">The ID value used to identify the object in the storage provider. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException">
  ///   <i>classID</i> is <see langword="null"/>.<br /> -or- <br />
  ///   <i>value</i> is <see langword="null"/>.
  /// </exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException">
  ///   <i>classID</i> is an empty string.<br /> -or- <br />
  ///   <i>value</i> is an empty string.<br /> -or- <br />
  ///   <i>value</i> is an empty Guid.
  /// </exception>
  /// <exception cref="System.ArgumentException">
  ///   <i>value</i> has an unsupported type or is a string and contains invalid characters.
  /// </exception>
  /// <exception cref="Mapping.MappingException"/>The specified <i>classID</i> could not be found in the mapping configuration.
  public ObjectID (string classID, int value) : this (classID, (object) value)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <b>ObjectID</b> class with the specified class ID and ID value.
  /// </summary>
  /// <param name="classID">The ID of the class of the object. Must not be <see langword="null"/>.</param>
  /// <param name="value">The ID value used to identify the object in the storage provider. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException">
  ///   <i>classID</i> is <see langword="null"/>.<br /> -or- <br />
  ///   <i>value</i> is <see langword="null"/>.
  /// </exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException">
  ///   <i>classID</i> is an empty string.<br /> -or- <br />
  ///   <i>value</i> is an empty string.<br /> -or- <br />
  ///   <i>value</i> is an empty Guid.
  /// </exception>
  /// <exception cref="System.ArgumentException">
  ///   <i>value</i> has an unsupported type or is a string and contains invalid characters.
  /// </exception>
  /// <exception cref="Mapping.MappingException"/>The specified <i>classID</i> could not be found in the mapping configuration.
  public ObjectID (string classID, string value) : this (classID, (object) value)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <b>ObjectID</b> class with the specified class ID and ID value.
  /// </summary>
  /// <param name="classID">The ID of the class of the object. Must not be <see langword="null"/>.</param>
  /// <param name="value">The ID value used to identify the object in the storage provider. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException">
  ///   <i>classID</i> is <see langword="null"/>.<br /> -or- <br />
  ///   <i>value</i> is <see langword="null"/>.
  /// </exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException">
  ///   <i>classID</i> is an empty string.<br /> -or- <br />
  ///   <i>value</i> is an empty string.<br /> -or- <br />
  ///   <i>value</i> is an empty Guid.
  /// </exception>
  /// <exception cref="System.ArgumentException">
  ///   <i>value</i> has an unsupported type or is a string and contains invalid characters.
  /// </exception>
  /// <exception cref="Mapping.MappingException"/>The specified <i>classID</i> could not be found in the mapping configuration.
  public ObjectID (string classID, Guid value) : this (classID, (object) value)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <b>ObjectID</b> class with the specified class type and ID value.
  /// </summary>
  /// <param name="classType">The <see cref="System.Type"/> of the class of the object. Must not be <see langword="null"/>.</param>
  /// <param name="value">The ID value used to identify the object in the storage provider. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException">
  ///   <i>classType</i> is <see langword="null"/>.<br /> -or- <br />
  ///   <i>value</i> is <see langword="null"/>.
  /// </exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException">
  ///   <i>value</i> is an empty string.<br /> -or- <br />
  ///   <i>value</i> is an empty Guid.
  /// </exception>
  /// <exception cref="System.ArgumentException">
  ///   <i>value</i> has an unsupported type or is a string and contains invalid characters.
  /// </exception>
  /// <exception cref="Mapping.MappingException"/>The specified <i>classType</i> could not be found in the mapping configuration.
  public ObjectID (Type classType, int value) : this (classType, (object) value)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <b>ObjectID</b> class with the specified class type and ID value.
  /// </summary>
  /// <param name="classType">The <see cref="System.Type"/> of the class of the object. Must not be <see langword="null"/>.</param>
  /// <param name="value">The ID value used to identify the object in the storage provider. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException">
  ///   <i>classType</i> is <see langword="null"/>.<br /> -or- <br />
  ///   <i>value</i> is <see langword="null"/>.
  /// </exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException">
  ///   <i>value</i> is an empty string.<br /> -or- <br />
  ///   <i>value</i> is an empty Guid.
  /// </exception>
  /// <exception cref="System.ArgumentException">
  ///   <i>value</i> has an unsupported type or is a string and contains invalid characters.
  /// </exception>
  /// <exception cref="Mapping.MappingException"/>The specified <i>classType</i> could not be found in the mapping configuration.
  public ObjectID (Type classType, string value) : this (classType, (object) value)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <b>ObjectID</b> class with the specified class type and ID value.
  /// </summary>
  /// <param name="classType">The <see cref="System.Type"/> of the class of the object. Must not be <see langword="null"/>.</param>
  /// <param name="value">The ID value used to identify the object in the storage provider. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException">
  ///   <i>classType</i> is <see langword="null"/>.<br /> -or- <br />
  ///   <i>value</i> is <see langword="null"/>.
  /// </exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException">
  ///   <i>value</i> is an empty string.<br /> -or- <br />
  ///   <i>value</i> is an empty Guid.
  /// </exception>
  /// <exception cref="System.ArgumentException">
  ///   <i>value</i> has an unsupported type or is a string and contains invalid characters.
  /// </exception>
  /// <exception cref="Mapping.MappingException"/>The specified <i>classType</i> could not be found in the mapping configuration.
  public ObjectID (Type classType, Guid value) : this (classType, (object) value)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <b>ObjectID</b> class with the specified <see cref="Mapping.ClassDefinition"/> and ID value.
  /// </summary>
  /// <param name="classDefinition">The <see cref="Mapping.ClassDefinition"/> of the object. Must not be <see langword="null"/>.</param>
  /// <param name="value">The ID value used to identify the object in the storage provider. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException">
  ///   <i>classDefinition</i> is <see langword="null"/>.<br /> -or- <br />
  ///   <i>value</i> is <see langword="null"/>.
  /// </exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException">
  ///   <i>value</i> is an empty string.<br /> -or- <br />
  ///   <i>value</i> is an empty Guid.
  /// </exception>
  /// <exception cref="System.ArgumentException">
  ///   <i>value</i> has an unsupported type or is a string and contains invalid characters.
  /// </exception>
  /// <exception cref="Mapping.MappingException"/>The specified <i>classDefinition</i> could not be found in the mapping configuration.
  public ObjectID (ClassDefinition classDefinition, int value) : this (classDefinition, (object) value)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <b>ObjectID</b> class with the specified <see cref="Mapping.ClassDefinition"/> and ID value.
  /// </summary>
  /// <param name="classDefinition">The <see cref="Mapping.ClassDefinition"/> of the object. Must not be <see langword="null"/>.</param>
  /// <param name="value">The ID value used to identify the object in the storage provider. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException">
  ///   <i>classDefinition</i> is <see langword="null"/>.<br /> -or- <br />
  ///   <i>value</i> is <see langword="null"/>.
  /// </exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException">
  ///   <i>value</i> is an empty string.<br /> -or- <br />
  ///   <i>value</i> is an empty Guid.
  /// </exception>
  /// <exception cref="System.ArgumentException">
  ///   <i>value</i> has an unsupported type or is a string and contains invalid characters.
  /// </exception>
  /// <exception cref="Mapping.MappingException"/>The specified <i>classDefinition</i> could not be found in the mapping configuration.
  public ObjectID (ClassDefinition classDefinition, string value) : this (classDefinition, (object) value)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <b>ObjectID</b> class with the specified <see cref="Mapping.ClassDefinition"/> and ID value.
  /// </summary>
  /// <param name="classDefinition">The <see cref="Mapping.ClassDefinition"/> of the object. Must not be <see langword="null"/>.</param>
  /// <param name="value">The ID value used to identify the object in the storage provider. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException">
  ///   <i>classDefinition</i> is <see langword="null"/>.<br /> -or- <br />
  ///   <i>value</i> is <see langword="null"/>.
  /// </exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException">
  ///   <i>value</i> is an empty string.<br /> -or- <br />
  ///   <i>value</i> is an empty Guid.
  /// </exception>
  /// <exception cref="System.ArgumentException">
  ///   <i>value</i> has an unsupported type or is a string and contains invalid characters.
  /// </exception>
  /// <exception cref="Mapping.MappingException"/>The specified <i>classDefinition</i> could not be found in the mapping configuration.
  public ObjectID (ClassDefinition classDefinition, Guid value) : this (classDefinition, (object) value)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <b>ObjectID</b> class with the specified class ID and ID value.
  /// </summary>
  /// <param name="classID">The ID of the class of the object. Must not be <see langword="null"/>.</param>
  /// <param name="value">The ID value used to identify the object in the storage provider. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException">
  ///   <i>classID</i> is <see langword="null"/>.<br /> -or- <br />
  ///   <i>value</i> is <see langword="null"/>.
  /// </exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException">
  ///   <i>classID</i> is an empty string.<br /> -or- <br />
  ///   <i>value</i> is an empty string.<br /> -or- <br />
  ///   <i>value</i> is an empty Guid.
  /// </exception>
  /// <exception cref="System.ArgumentException">
  ///   <i>value</i> has an unsupported type or is a string and contains invalid characters.
  /// </exception>
  /// <exception cref="Mapping.MappingException"/>The specified <i>classID</i> could not be found in the mapping configuration.
  protected ObjectID (string classID, object value)
  {
    Initialize (classID, value);
  }

  /// <summary>
  /// Initializes a new instance of the <b>ObjectID</b> class with the specified class type and ID value.
  /// </summary>
  /// <param name="classType">The <see cref="System.Type"/> of the class of the object. Must not be <see langword="null"/>.</param>
  /// <param name="value">The ID value used to identify the object in the storage provider. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException">
  ///   <i>classType</i> is <see langword="null"/>.<br /> -or- <br />
  ///   <i>value</i> is <see langword="null"/>.
  /// </exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException">
  ///   <i>value</i> is an empty string.<br /> -or- <br />
  ///   <i>value</i> is an empty Guid.
  /// </exception>
  /// <exception cref="System.ArgumentException">
  ///   <i>value</i> has an unsupported type or is a string and contains invalid characters.
  /// </exception>
  /// <exception cref="Mapping.MappingException"/>The specified <i>classType</i> could not be found in the mapping configuration.
  protected ObjectID (Type classType, object value)
  {
    ArgumentUtility.CheckNotNull ("classType", classType);

    ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (classType);
    Initialize (classDefinition.ID, value);
  }

  /// <summary>
  /// Initializes a new instance of the <b>ObjectID</b> class with the specified <see cref="Mapping.ClassDefinition"/> and ID value.
  /// </summary>
  /// <param name="classDefinition">The <see cref="Mapping.ClassDefinition"/> of the object. Must not be <see langword="null"/>.</param>
  /// <param name="value">The ID value used to identify the object in the storage provider. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException">
  ///   <i>classDefinition</i> is <see langword="null"/>.<br /> -or- <br />
  ///   <i>value</i> is <see langword="null"/>.
  /// </exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException">
  ///   <i>value</i> is an empty string.<br /> -or- <br />
  ///   <i>value</i> is an empty Guid.
  /// </exception>
  /// <exception cref="System.ArgumentException">
  ///   <i>value</i> has an unsupported type or is a string and contains invalid characters.
  /// </exception>
  /// <exception cref="Mapping.MappingException"/>The specified <i>classDefinition</i> could not be found in the mapping configuration.
  protected ObjectID (ClassDefinition classDefinition, object value)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

    ClassDefinition classDefinitionByClassType = MappingConfiguration.Current.ClassDefinitions.GetMandatory (classDefinition.ClassType);
    ClassDefinition classDefinitionByClassID = MappingConfiguration.Current.ClassDefinitions.GetMandatory (classDefinition.ID);

    if (!object.ReferenceEquals (classDefinitionByClassID, classDefinitionByClassType))
    {
      throw CreateArgumentException (
          "classDefinition",
          "The ClassID '{0}' and the ClassType '{1}' do not refer to the same ClassDefinition in the mapping configuration.",
          classDefinition.ID, 
          classDefinition.ClassType);
    }

    if (!object.ReferenceEquals (classDefinitionByClassID, classDefinition))
    {
      throw CreateArgumentException (
          "classDefinition",
          "The provided ClassDefinition '{0}' is not the same reference as the ClassDefinition found in the mapping configuration.",
          classDefinition.ID);
    }

    Initialize (classDefinition.ID, value);
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="ObjectID"/> class from the specified instances of the <see cref="System.Runtime.Serialization.SerializationInfo"/> and <see cref="System.Runtime.Serialization.StreamingContext"/> classes.
  /// </summary>
  /// <param name="info">An instance of the <see cref="System.Runtime.Serialization.SerializationInfo"/> class containing the information required to deserialize the new <see cref="ObjectID"/> instance.</param>
  /// <param name="context">An instance of the <see cref="System.Runtime.Serialization.StreamingContext"/> class containing the source of the serialized stream associated with the new <see cref="ObjectID"/> instance.</param>
  protected ObjectID (SerializationInfo info, StreamingContext context)
  {
    string classID = info.GetString ("ClassID");
    Type valueType = (Type) info.GetValue ("ValueType", typeof (Type));
    object value = info.GetValue ("Value", valueType);

    Initialize (classID, value);
  }

  private void Initialize (string classID, object value)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("classID", classID);
    ArgumentUtility.CheckNotNull ("value", value);
    CheckValue ("value", value);

    ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (classID);

    _classDefinition = classDefinition;
    _value = value;
  }

  // methods and properties

  /// <summary>
  /// Gets the ID of the <see cref="Persistence.StorageProvider"/> which stores the object.
  /// </summary>
  public string StorageProviderID
  {
    get { return _classDefinition.StorageProviderID; }
  }

  /// <summary>
  /// Gets the ID value used to identify the object in the storage provider.
  /// </summary>
  public object Value
  {
    get { return _value; }
  }

  /// <summary>
  /// The class ID of the object class.
  /// </summary>
  public string ClassID
  {
    get { return _classDefinition.ID; }
  }

  /// <summary>
  /// Gets the <see cref="Mapping.ClassDefinition"/> associated with this <b>ObjectID</b>.
  /// </summary>
  public ClassDefinition ClassDefinition 
  {
    get { return _classDefinition; }
  }

  /// <summary>
  /// Returns the string representation of the current <see cref="ObjectID"/>.
  /// </summary>
  /// <returns>A <see cref="String"/> that represents the current <see cref="ObjectID"/>.</returns>
  public override string ToString ()
  {
    Type valueType = Value.GetType();

    return Escape (ClassID) + c_delimiter + 
        Escape (Value.ToString ()) + c_delimiter + 
        Escape (valueType.FullName);
  }

  /// <summary>
  /// Returns the hash code for this instance.
  /// </summary>
  /// <returns>A 32-bit signed integer hash code.</returns>
  public override int GetHashCode()
  {
    return ClassID.GetHashCode () ^ Value.GetHashCode ();
  }

  /// <summary>
  /// Determines whether the specified <see cref="ObjectID"/> is equal to the current <b>ObjectID</b>.
  /// </summary>
  /// <param name="obj">The <see cref="ObjectID"/> to compare with the current <b>ObjectID</b>. </param>
  /// <returns><b>true</b> if the specified <see cref="ObjectID"/> is equal to the current <b>ObjectID</b>; otherwise, <b>false</b>.</returns>
  public override bool Equals (object obj)
  {
    if (obj == null) return false;
    if (this.GetType () != obj.GetType ()) return false;
    
    ObjectID other = (ObjectID) obj;
    if (!object.Equals (this.ClassID, other.ClassID)) return false;
    if (!object.Equals (this.Value, other.Value)) return false;

    return true;
  }

  private void CheckValue (string argumentName, object value)
  {
    Type valueType = value.GetType ();

    if (valueType == typeof (string) && ((string) value).IndexOf (c_escapedDelimiterPlaceholder) >= 0)
    {
      throw new ArgumentException (string.Format (
          "Value cannot contain '{0}'.", c_escapedDelimiterPlaceholder), "value");
    }

    if (valueType == typeof (string) && string.Empty.Equals (value))
    {
      throw new ArgumentEmptyException (argumentName);
    }

    if (valueType == typeof (Guid) && Guid.Empty.Equals (value))
    {
      throw new ArgumentEmptyException (argumentName);
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

  private ArgumentException CreateArgumentException (string argumentName, string message, params object[] args)
  {
    return new ArgumentException (string.Format (message, args), argumentName);
  }

  #region ISerializable Members

  /// <summary>
  /// Gets serialization information with all of the data needed to reinstantiate this <b>ObjectID</b>.
  /// </summary>
  /// <param name="info">The object to be populated with serialization information.</param>
  /// <param name="context">The destination context of the serialization.</param>
  void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
  {
    info.AddValue ("ClassID", ClassID);
    info.AddValue ("Value", Value);
    info.AddValue ("ValueType", Value.GetType ());
  }

  #endregion
}
}
