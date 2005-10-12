using System;
using System.Data;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence.Rdbms
{
public class ValueConverter : ValueConverterBase
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public ValueConverter ()
  {
  }

  // methods and properties

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

    if (propertyDefinition.PropertyType != typeof (ObjectID) || dataReader.IsDBNull (columnOrdinal))
      return GetValue (classDefinition, propertyDefinition, dataReader.GetValue (columnOrdinal));

    return GetObjectID (classDefinition, propertyDefinition, dataReader, columnOrdinal);
  }

  public override object GetValue (ClassDefinition classDefinition, PropertyDefinition propertyDefinition, object dataValue)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

    if (dataValue == DBNull.Value)
      dataValue = null;
    
    return base.GetValue (classDefinition, propertyDefinition, dataValue);
  }

  private ObjectID GetObjectID (ClassDefinition classDefinition, PropertyDefinition propertyDefinition, IDataReader dataReader, int columnOrdinal)
  {
    ClassDefinition relatedClassDefinition = GetOppositeClassDefinitionFromClassIDColumn (classDefinition, propertyDefinition, dataReader);
    if (relatedClassDefinition == null)
    {
      relatedClassDefinition = GetOppositeClassDefinition (classDefinition, propertyDefinition);

      if (classDefinition.StorageProviderID == relatedClassDefinition.StorageProviderID)
      {
        if (relatedClassDefinition.IsPartOfInheritanceHierarchy)
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
      if (!oppositeClassDefinition.IsPartOfInheritanceHierarchy)
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

  private string GetClassIDColumnName (string columnName)
  {
    return columnName + "ClassID";
  }

  protected RdbmsProviderException CreateRdbmsProviderException (
      string formatString,
      params object[] args)
  {
    return CreateRdbmsProviderException (null, formatString, args);
  }

  protected RdbmsProviderException CreateRdbmsProviderException (
      Exception innerException,
      string formatString,
      params object[] args)
  {
    return new RdbmsProviderException (string.Format (formatString, args), innerException);
  }
}
}
