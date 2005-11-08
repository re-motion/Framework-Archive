using System;
using System.Collections;
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

  public int GetMandatoryOrdinal (IDataReader dataReader, string columnName)
  {
    ArgumentUtility.CheckNotNull ("dataReader", dataReader);
    ArgumentUtility.CheckNotNullOrEmpty ("columnName", columnName);

    try
    {  
      return dataReader.GetOrdinal (columnName);
    }
    catch (IndexOutOfRangeException)
    {
      throw CreateRdbmsProviderException ("The mandatory column '{0}' could not be found.", columnName);
    }
  }

  public object GetValue (ClassDefinition classDefinition, PropertyDefinition propertyDefinition, IDataReader dataReader, int columnOrdinal)
  {
    // Note: Parameter columnOrdinal is provided for performance reasons. IDataReader.GetOrdinal takes a signifacant amount of time.

    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
    ArgumentUtility.CheckNotNull ("dataReader", dataReader);
    if (columnOrdinal < 0) throw new ArgumentOutOfRangeException ("columnOrdinal", columnOrdinal, "Parameter 'columnOrdinal' must be greater than or equal to zero."); 

    if (propertyDefinition.PropertyType != typeof (ObjectID))
      return GetValue (classDefinition, propertyDefinition, dataReader.GetValue (columnOrdinal));
    else
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

  public override ObjectID GetObjectID (ClassDefinition classDefinition, object dataValue)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

    if (dataValue == DBNull.Value)
      dataValue = null;
    
    return base.GetObjectID (classDefinition, dataValue);
  }

  private ObjectID GetObjectID (ClassDefinition classDefinition, PropertyDefinition propertyDefinition, IDataReader dataReader, int objectIDColumnOrdinal)
  {
    CheckObjectIDColumn (classDefinition, propertyDefinition, dataReader, objectIDColumnOrdinal);

    ClassDefinition relatedClassDefinition = GetMandatoryOppositeClassDefinition (classDefinition, propertyDefinition, dataReader, objectIDColumnOrdinal);
    return GetObjectID (relatedClassDefinition, dataReader.GetValue (objectIDColumnOrdinal));
  }

  private void CheckObjectIDColumn (ClassDefinition classDefinition, PropertyDefinition propertyDefinition, IDataReader dataReader, int objectIDColumnOrdinal)
  {
    IRelationEndPointDefinition endPointDefinition = classDefinition.GetMandatoryRelationEndPointDefinition (propertyDefinition.PropertyName);
    if (endPointDefinition.IsMandatory && dataReader.IsDBNull (objectIDColumnOrdinal))
    {
      throw CreateConverterException (
          "Invalid null value for not-nullable relation property '{0}' encountered. Class: '{1}'.", 
          propertyDefinition.PropertyName, classDefinition.ID);
    }
  }

  private ClassDefinition GetMandatoryOppositeClassDefinition (
      ClassDefinition classDefinition, 
      PropertyDefinition propertyDefinition, 
      IDataReader dataReader,
      int objectIDColumnOrdinal)
  {
    ClassDefinition relatedMappingClassDefinition = classDefinition.GetMandatoryOppositeClassDefinition (propertyDefinition.PropertyName);
    if (relatedMappingClassDefinition.IsPartOfInheritanceHierarchy && classDefinition.StorageProviderID == relatedMappingClassDefinition.StorageProviderID)
    {
      int classIDColumnOrdinal = -1;
      try
      {
        classIDColumnOrdinal = dataReader.GetOrdinal (GetClassIDColumnName (propertyDefinition.ColumnName));
      }
      catch (IndexOutOfRangeException)
      {
        throw CreateRdbmsProviderException (
            "Incorrect database format encountered."
            + " Entity must have column '{0}' defined, because opposite class '{1}' is part of an inheritance hierarchy.",
            GetClassIDColumnName (propertyDefinition.ColumnName), 
            relatedMappingClassDefinition.ID);    
      }

      if (dataReader.IsDBNull (objectIDColumnOrdinal) && !dataReader.IsDBNull (classIDColumnOrdinal))
      {
        throw CreateRdbmsProviderException (
            "Incorrect database value encountered. Column '{0}' of entity '{1}' must not contain a value.", 
            GetClassIDColumnName (propertyDefinition.ColumnName),
            relatedMappingClassDefinition.EntityName);
      }


      if (!dataReader.IsDBNull (objectIDColumnOrdinal) && dataReader.IsDBNull (classIDColumnOrdinal))
      {
        throw CreateRdbmsProviderException (
            "Incorrect database value encountered. Column '{0}' of entity '{1}' must not contain null.",
            GetClassIDColumnName (propertyDefinition.ColumnName), 
            classDefinition.EntityName);

      }

      if (dataReader.IsDBNull (classIDColumnOrdinal))
        return relatedMappingClassDefinition;
      else
        return MappingConfiguration.Current.ClassDefinitions.GetMandatory (dataReader.GetString (classIDColumnOrdinal));
    }
    else
    {
      try
      {
        dataReader.GetOrdinal (GetClassIDColumnName (propertyDefinition.ColumnName));

        throw CreateRdbmsProviderException (
            "Incorrect database format encountered. Entity '{0}' must not contain column '{1}', because opposite class '{2}' is not part of an inheritance hierarchy.",
            classDefinition.EntityName,
            GetClassIDColumnName (propertyDefinition.ColumnName),
            relatedMappingClassDefinition.ID);
      }
      catch (IndexOutOfRangeException)
      {
        return relatedMappingClassDefinition;
      }
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
