using System;
using System.Data;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence
{
public sealed class DBValueConverter
{
  // types

  // static members and constants

  public static object GetDBValue (object value)
  {
    if (value == null)
      return DBNull.Value;

    Type type = value.GetType ();

    INaNullable naValueType = value as INaNullable;
    if (naValueType != null)
    {
      if (!naValueType.IsNull)
        return naValueType.Value;
      else
        return DBNull.Value;
    }

    if (type.IsEnum)
      return (int) value;
    
    return value;
  }

  public static ObjectID GetObjectID (
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

  public static object GetValue (ClassDefinition classDefinition, PropertyDefinition propertyDefinition, IDataReader dataReader)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
    ArgumentUtility.CheckNotNull ("dataReader", dataReader);

    int columnOrdinal = dataReader.GetOrdinal (propertyDefinition.ColumnName);

    if (dataReader.IsDBNull (columnOrdinal))
      return propertyDefinition.DefaultValue;

    if (propertyDefinition.PropertyType == typeof (ObjectID))
    {
      ClassDefinition relatedClassDefinition = GetRelatedClassDefinition (propertyDefinition, dataReader);
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

      return GetObjectID (relatedClassDefinition, dataReader.GetValue (columnOrdinal));
    }

    if (propertyDefinition.PropertyType.IsEnum)
      return GetEnumValue (propertyDefinition, dataReader.GetValue (columnOrdinal));

    if (propertyDefinition.PropertyType == typeof (NaBoolean))
      return new NaBoolean (dataReader.GetBoolean (columnOrdinal));

    if (propertyDefinition.PropertyType == typeof (NaByte))
      return new NaByte (dataReader.GetByte (columnOrdinal));

    if (propertyDefinition.PropertyType == typeof (NaDateTime))
      return new NaDateTime (dataReader.GetDateTime (columnOrdinal));

    if (propertyDefinition.PropertyType == typeof (NaDouble))
      return new NaDouble (dataReader.GetDouble (columnOrdinal));

    if (propertyDefinition.PropertyType == typeof (NaDecimal))
      return new NaDecimal (dataReader.GetDecimal (columnOrdinal));

    if (propertyDefinition.PropertyType == typeof (NaGuid))
      return new NaGuid (dataReader.GetGuid (columnOrdinal));

    if (propertyDefinition.PropertyType == typeof (NaInt16))
      return new NaInt16 (dataReader.GetInt16 (columnOrdinal));

    if (propertyDefinition.PropertyType == typeof (NaInt32))
      return new NaInt32 (dataReader.GetInt32 (columnOrdinal));

    if (propertyDefinition.PropertyType == typeof (NaInt64))
      return new NaInt64 (dataReader.GetInt64 (columnOrdinal));

    if (propertyDefinition.PropertyType == typeof (NaSingle))
      return new NaSingle (dataReader.GetFloat (columnOrdinal));

    return dataReader.GetValue (columnOrdinal);
  }

  private static ClassDefinition GetRelatedClassDefinition (PropertyDefinition propertyDefinition, IDataReader dataReader)
  {
    string relatedClassIDColumnName = GetRelatedClassIDColumnName (propertyDefinition.ColumnName);
    try
    {
      string classID = dataReader.GetString (dataReader.GetOrdinal (relatedClassIDColumnName));
      return MappingConfiguration.Current.ClassDefinitions.GetMandatory (classID);
    }
    catch (IndexOutOfRangeException)
    {
    }

    return null;
  }

  private static ClassDefinition GetRelatedClassDefinition (
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

  private static object GetEnumValue (PropertyDefinition propertyDefinition, object dataValue)
  {
    if (Enum.IsDefined (propertyDefinition.PropertyType, dataValue))
      return Enum.ToObject (propertyDefinition.PropertyType, dataValue);

    throw CreateStorageProviderException (
        "Enumeration '{0}' does not define the value '{1}', property '{2}'.",
        propertyDefinition.PropertyType.FullName, dataValue, propertyDefinition.PropertyName);
  }

  private static string GetRelatedClassIDColumnName (string columnName)
  {
    return columnName + "ClassID";
  }

  private static StorageProviderException CreateStorageProviderException (
      string formatString,
      params object[] args)
  {
    return CreateStorageProviderException (null, formatString, args);
  }

  private static StorageProviderException CreateStorageProviderException (
      Exception innerException,
      string formatString,
      params object[] args)
  {
    return new StorageProviderException (string.Format (formatString, args), innerException);
  }

  // member fields

  // construction and disposing

  private DBValueConverter ()
  {
  }

  // methods and properties
}
}
