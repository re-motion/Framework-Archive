using System;
using System.Data;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence
{
public class DataContainerFactory
{
  // types

  // static members and constants

  // member fields

  private ClassDefinition _classDefinition;
  private IDataReader _dataReader;

  // construction and disposing

  public DataContainerFactory (ClassDefinition classDefinition, IDataReader dataReader)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    ArgumentUtility.CheckNotNull ("dataReader", dataReader);

    _classDefinition = classDefinition;
    _dataReader = dataReader;
  }

  // methods and properties

  public virtual DataContainer CreateDataContainer ()
  {
    if (_dataReader.Read ())
      return CreateDataContainerFromReader ();
    else
      return null;
  }

  public virtual DataContainerCollection CreateCollection ()
  {
    DataContainerCollection dataContainerCollection = new DataContainerCollection ();

    while (_dataReader.Read ())
      dataContainerCollection.Add (CreateDataContainerFromReader ());

    return dataContainerCollection;
  }

  protected virtual DataContainer CreateDataContainerFromReader ()
  {
    CheckForMandatoryColumns ();

    string classID = _dataReader.GetString (_dataReader.GetOrdinal ("ClassID"));
    ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetByClassID (classID);

    if (classDefinition == null)
    {
      throw CreateStorageProviderException (
          "Invalid ClassID '{0}' for ID '{1}' in entity '{2}' encountered.",
          classID, _dataReader["ID"], _classDefinition.EntityName);
    }

    ObjectID id = GetObjectID (classDefinition, _dataReader["ID"]);
    DataContainer dataContainer = DataContainer.CreateForExisting (id, _dataReader["Timestamp"]);

    foreach (PropertyDefinition propertyDefinition in classDefinition.GetAllPropertyDefinitions ())
    {
      CheckColumn (propertyDefinition.ColumnName);

      object dataValue;

      try
      {
        dataValue = GetValue (classDefinition, propertyDefinition);
      }
      catch (StorageProviderException e)
      {
        throw CreateStorageProviderException (e, "Error while reading property '{0}' for class '{1}': {2}",
            propertyDefinition.PropertyName, classDefinition.ID, e.Message);
      }
      catch (InvalidCastException e)
      {
        throw CreateStorageProviderException (e, "Error while reading property '{0}' for class '{1}': {2}",
            propertyDefinition.PropertyName, classDefinition.ID, e.Message);
      }

      dataContainer.PropertyValues.Add (new PropertyValue (propertyDefinition, dataValue));
    }

    return dataContainer;
  }

  protected virtual ObjectID GetObjectID (
      ClassDefinition classDefinition, 
      object value)
  {
    if (value == null)
      return null;

    if (value.GetType () == typeof (string))
    {
      ObjectID id = null;
      try
      {
        id = ObjectID.Parse ((string) value);
      }
      catch (ArgumentException)
      {
      }
      catch (FormatException)
      {
      }

      if (id != null)
        return id;
    }

    return new ObjectID (classDefinition.StorageProviderID, classDefinition.ID, value);
  }

  protected virtual void CheckForMandatoryColumns ()
  {
    CheckColumn ("ID");
    CheckColumn ("ClassID");
    CheckColumn ("Timestamp");
  }

  protected virtual void CheckColumn (string columnName)
  {
    try
    {  
      _dataReader.GetOrdinal (columnName);
    }
    catch (IndexOutOfRangeException)
    {
      throw CreateStorageProviderException ("Entity '{0}' does not contain column '{1}'.",
          _classDefinition.EntityName, columnName);
    }
  }

  protected StorageProviderException CreateStorageProviderException (
      string formatString,
      params object[] args)
  {
    return CreateStorageProviderException (null, formatString, args);
  }

  protected StorageProviderException CreateStorageProviderException (
      Exception innerException,
      string formatString,
      params object[] args)
  {
    return new StorageProviderException (string.Format (formatString, args), innerException);
  }

  protected virtual object GetValue (ClassDefinition classDefinition, PropertyDefinition propertyDefinition)
  {
    int columnOrdinal = _dataReader.GetOrdinal (propertyDefinition.ColumnName);

    if (_dataReader.IsDBNull (columnOrdinal))
      return GetNullValue (propertyDefinition);

    switch (propertyDefinition.PropertyType.FullName)
    {
      case "Rubicon.Data.DomainObjects.ObjectID":
        ClassDefinition relatedClassDefinition = GetRelatedClassDefinition (propertyDefinition);
        if (relatedClassDefinition == null)
        {
          relatedClassDefinition = GetRelatedClassDefinition (classDefinition, propertyDefinition);

          if (relatedClassDefinition.BaseClass != null)
          {
            throw CreateStorageProviderException (
                "Incorrect database format encountered."
                + " Class must have column '{0}' defined, because it points to derived class '{1}'.",
                GetRelatedClassIDColumnName (propertyDefinition.ColumnName), 
                relatedClassDefinition.ID);    
          }

          ClassDefinitionCollection derivedClasses = 
              MappingConfiguration.Current.ClassDefinitions.GetDirectlyDerivedClassDefinitions (relatedClassDefinition);

          if (derivedClasses.Count > 0)
          {
            throw CreateStorageProviderException (
                "Incorrect database format encountered."
                + " Class must have column '{0}' defined, because at least one class inherits from '{1}'.",
                GetRelatedClassIDColumnName (propertyDefinition.ColumnName), 
                relatedClassDefinition.ID);    
          }
        }

        return GetObjectID (relatedClassDefinition, _dataReader.GetValue (columnOrdinal));

      case "System.Boolean":
        return _dataReader.GetBoolean (columnOrdinal);

      case "System.Byte":
        return _dataReader.GetByte (columnOrdinal);

      case "System.DateTime":
        return _dataReader.GetDateTime (columnOrdinal);

      case "System.Decimal":
        return _dataReader.GetDecimal (columnOrdinal);

      case "System.Double":
        return _dataReader.GetDouble (columnOrdinal);

      case "System.Guid":
        return _dataReader.GetGuid (columnOrdinal);

      case "System.Int16":
        return _dataReader.GetInt16 (columnOrdinal);

      case "System.Int32":
        return _dataReader.GetInt32 (columnOrdinal);

      case "System.Int64":
        return _dataReader.GetInt64 (columnOrdinal);

      case "System.Single":
        return _dataReader.GetFloat (columnOrdinal);

      case "System.String":
        return _dataReader.GetString (columnOrdinal);

      case "System.Char":
        return GetCharValue (propertyDefinition, _dataReader.GetString (columnOrdinal));

      case "Rubicon.NullableValueTypes.NaBoolean":
        return new NaBoolean (_dataReader.GetBoolean (columnOrdinal));

      case "Rubicon.NullableValueTypes.NaDateTime":
        return new NaDateTime (_dataReader.GetDateTime (columnOrdinal));

      case "Rubicon.NullableValueTypes.NaDouble":
        return new NaDouble (_dataReader.GetDouble (columnOrdinal));

      case "Rubicon.NullableValueTypes.NaInt32":
        return new NaInt32 (_dataReader.GetInt32 (columnOrdinal));

      default:
        if (propertyDefinition.PropertyType.IsEnum)
          return GetEnumValue (propertyDefinition, _dataReader.GetValue (columnOrdinal));

        throw CreateStorageProviderException ("Unknown property type '{0}' provided.",
            propertyDefinition.PropertyType.FullName);
    }
  }

  protected string GetRelatedClassIDColumnName (string columnName)
  {
    return columnName + "ClassID";
  }

  protected ClassDefinition GetRelatedClassDefinition (PropertyDefinition propertyDefinition)
  {
    string relatedClassIDColumnName = GetRelatedClassIDColumnName (propertyDefinition.ColumnName);
    try
    {
      string classID = _dataReader.GetString (_dataReader.GetOrdinal (relatedClassIDColumnName));
      return MappingConfiguration.Current.ClassDefinitions.GetMandatory (classID);
    }
    catch (IndexOutOfRangeException)
    {
    }

    return null;
  }

  protected ClassDefinition GetRelatedClassDefinition (
      ClassDefinition classDefinition,
      PropertyDefinition propertyDefinition)
  {
    ClassDefinition relatedClassDefinition = classDefinition.GetRelatedClassDefinition (
        propertyDefinition.PropertyName);

    if (relatedClassDefinition == null)
    {
      throw new StorageProviderException (string.Format (
          "Property '{0}' of class '{1}' has no relations assigned.", 
          propertyDefinition.PropertyName, classDefinition.ID));  
    }

    return relatedClassDefinition;
  }

  protected virtual object GetNullValue (PropertyDefinition propertyDefinition)
  {
    switch (propertyDefinition.PropertyType.FullName)
    {
      case "Rubicon.NullableValueTypes.NaBoolean":
        return NaBoolean.Null;

      case "Rubicon.NullableValueTypes.NaDateTime":
        return NaDateTime.Null;

      case "Rubicon.NullableValueTypes.NaDouble":
        return NaDouble.Null;

      case "Rubicon.NullableValueTypes.NaInt32":
        return NaInt32.Null;

      case "System.String":
      case "Rubicon.Data.DomainObjects.ObjectID":
        return null;

      default:
        throw CreateStorageProviderException ("Cannot convert null to {0}, property: {1}.",
            propertyDefinition.PropertyType.FullName, propertyDefinition.PropertyName);
    }
  }

  protected char GetCharValue (PropertyDefinition propertyDefinition, string stringValue)
  {
    if (stringValue.Length == 1)
    {
      return stringValue[0];
    }
    else 
    {
      throw CreateStorageProviderException (
          "Value '{0}' is not supported for System.Char datatype, property: {1}.",
          stringValue, propertyDefinition.PropertyName);
    }
  }

  protected object GetEnumValue (PropertyDefinition propertyDefinition, object dataValue)
  {
    if (Enum.IsDefined (propertyDefinition.PropertyType, dataValue))
      return Enum.ToObject (propertyDefinition.PropertyType, dataValue);

    throw CreateStorageProviderException (
        "Enumeration '{0}' does not define the value '{1}', property '{2}'.",
        propertyDefinition.PropertyType.FullName, dataValue, propertyDefinition.PropertyName);
  }
}
}
