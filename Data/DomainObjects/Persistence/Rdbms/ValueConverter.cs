using System;
using System.Data;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence.Rdbms
{
public class ValueConverter
{
  // types

  // static members and constants

  public object GetDBValue (object value)
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

  public virtual ObjectID GetObjectID (
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

    if (value.GetType () != typeof (Guid))
      throw CreateArgumentException ("value", "ValueConverter does not support ObjectID values of type '{0}'.", value.GetType ());

    return new ObjectID (classDefinition.ID, (Guid) value);
  }

  public object GetDBValue (ObjectID id, string storageProviderID)
  {
    ArgumentUtility.CheckNotNull ("id", id);
    ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);

    if (id.StorageProviderID == storageProviderID)
      return id.Value;
    else
      return id.ToString ();
  }

  public object GetValue (ClassDefinition classDefinition, PropertyDefinition propertyDefinition, IDataReader dataReader)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
    ArgumentUtility.CheckNotNull ("dataReader", dataReader);
    
    if (propertyDefinition.PropertyType == typeof (ObjectID))
      CheckClassIDColumn (classDefinition, propertyDefinition, dataReader);

    int columnOrdinal = dataReader.GetOrdinal (propertyDefinition.ColumnName);

    if (dataReader.IsDBNull (columnOrdinal))
      return propertyDefinition.DefaultValue;

    if (propertyDefinition.PropertyType == typeof (ObjectID))
    {
      ClassDefinition relatedClassDefinition = GetOppositeClassDefinitionFromClassIDColumn (classDefinition, propertyDefinition, dataReader);
      if (relatedClassDefinition == null)
      {
        relatedClassDefinition = GetOppositeClassDefinition (classDefinition, propertyDefinition);

        if (classDefinition.StorageProviderID == relatedClassDefinition.StorageProviderID)
        {
          if (MappingConfiguration.Current.ClassDefinitions.IsPartOfInheritanceHierarchy (relatedClassDefinition))
          {
            throw CreateRdbmsProviderException (
                "Incorrect database format encountered."
                + " Entity must have column '{0}' defined, because opposite class '{1}' is part of an inheritance hierarchy.",
                GetClassIDColumnName (propertyDefinition.ColumnName), 
                relatedClassDefinition.ID);    
          }
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

  protected ArgumentException CreateArgumentException (string argumentName, string message, params object[] args)
  {
    return new ArgumentException (string.Format (message, args), argumentName);
  }

  private ClassDefinition GetOppositeClassDefinitionFromClassIDColumn (
      ClassDefinition classDefinition, 
      PropertyDefinition propertyDefinition, 
      IDataReader dataReader)
  {
    string relatedClassIDColumnName = GetClassIDColumnName (propertyDefinition.ColumnName);
    int columnOrdinal = -1;
    try
    {
      columnOrdinal = dataReader.GetOrdinal (relatedClassIDColumnName);
    }
    catch (IndexOutOfRangeException)
    {
      return null;
    }

    if (dataReader.IsDBNull (columnOrdinal))
    {
      throw CreateRdbmsProviderException (
          "Incorrect database value encountered. Column '{0}' of entity '{1}' must not contain null.",
          relatedClassIDColumnName, 
          classDefinition.EntityName);
    }

    return MappingConfiguration.Current.ClassDefinitions.GetMandatory (dataReader.GetString (columnOrdinal));
  }

  private void CheckClassIDColumn (ClassDefinition classDefinition, PropertyDefinition propertyDefinition, IDataReader dataReader)
  {
    if (HasClassIDColumn (propertyDefinition, dataReader))
    {
      ClassDefinition oppositeClassDefinition = classDefinition.GetOppositeClassDefinition (propertyDefinition.PropertyName);
      if (!MappingConfiguration.Current.ClassDefinitions.IsPartOfInheritanceHierarchy (oppositeClassDefinition))
      {
        throw CreateRdbmsProviderException (
            "Incorrect database format encountered. Entity '{0}' must not contain column '{1}', because opposite class '{2}' is not part of an inheritance hierarchy.",
            classDefinition.EntityName,
            GetClassIDColumnName (propertyDefinition.ColumnName),
            oppositeClassDefinition.ID);
      }
    }
  }

  private bool HasClassIDColumn (PropertyDefinition propertyDefinition, IDataReader dataReader)
  {
    try
    {
      return (dataReader.GetOrdinal (GetClassIDColumnName (propertyDefinition.ColumnName)) >= 0);
    }
    catch (IndexOutOfRangeException)
    {
      return false;
    }
  }

  private ClassDefinition GetOppositeClassDefinition (
      ClassDefinition classDefinition,
      PropertyDefinition propertyDefinition)
  {
    ClassDefinition relatedClassDefinition = classDefinition.GetOppositeClassDefinition (
        propertyDefinition.PropertyName);

    if (relatedClassDefinition == null)
    {
      throw CreateRdbmsProviderException (
          "Property '{0}' of class '{1}' has no relations assigned.", propertyDefinition.PropertyName, classDefinition.ID);  
    }

    return relatedClassDefinition;
  }

  private object GetEnumValue (PropertyDefinition propertyDefinition, object dataValue)
  {
    if (Enum.IsDefined (propertyDefinition.PropertyType, dataValue))
      return Enum.ToObject (propertyDefinition.PropertyType, dataValue);

    throw CreateRdbmsProviderException (
        "Enumeration '{0}' does not define the value '{1}', property '{2}'.",
        propertyDefinition.PropertyType.FullName, dataValue, propertyDefinition.PropertyName);
  }

  private string GetClassIDColumnName (string columnName)
  {
    return columnName + "ClassID";
  }

  private RdbmsProviderException CreateRdbmsProviderException (
      string formatString,
      params object[] args)
  {
    return CreateRdbmsProviderException (null, formatString, args);
  }

  private RdbmsProviderException CreateRdbmsProviderException (
      Exception innerException,
      string formatString,
      params object[] args)
  {
    return new RdbmsProviderException (string.Format (formatString, args), innerException);
  }

  // member fields

  // construction and disposing

  public ValueConverter ()
  {
  }

  // methods and properties
}
}
