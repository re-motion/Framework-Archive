using System;
using System.Collections;
using System.Data;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence.Rdbms
{
public class ValueConverter : ValueConverterBase
{
  // types

  // static members and constants

  private static Hashtable s_hasClassIDColumn = new Hashtable ();

  // member fields

  // construction and disposing

  public ValueConverter ()
  {
  }

  // methods and properties

  public virtual object GetDBValue (object value)
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

  public virtual object GetDBValue (ObjectID id, string storageProviderID)
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

  public ObjectID GetID (IDataReader dataReader)
  {
    ArgumentUtility.CheckNotNull ("dataReader", dataReader);

    object idValue = dataReader.GetValue (GetMandatoryOrdinal (dataReader, "ID"));
    ClassDefinition classDefinition = GetClassDefinition (dataReader, idValue);
   
    return GetObjectID (classDefinition, idValue);
  }

  public override ObjectID GetObjectID (ClassDefinition classDefinition, object dataValue)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

    if (dataValue == DBNull.Value)
      dataValue = null;
    
    return base.GetObjectID (classDefinition, dataValue);
  }

  private ClassDefinition GetClassDefinition (IDataReader dataReader, object idValue)
  {
    string classID = GetClassID (dataReader);

    ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[classID];
    if (classDefinition == null)
      throw CreateRdbmsProviderException ("Invalid ClassID '{0}' for ID '{1}' encountered.", classID, idValue);

    if (classDefinition.ClassType.IsAbstract && !classDefinition.ShouldUseFactoryForInstantiation)
    {
      throw CreateRdbmsProviderException (
          "Invalid database value encountered. Column 'ClassID' of row with ID '{0}' refers to abstract class '{1}'.",
          idValue, classDefinition.ID);
    }

    return classDefinition;
  }

  private string GetClassID (IDataReader dataReader)
  {
    int classIDColumnOrdinal = GetMandatoryOrdinal (dataReader, "ClassID");
    if (dataReader.IsDBNull (classIDColumnOrdinal))
      throw CreateRdbmsProviderException ("Invalid database value encountered. Column 'ClassID' must not contain null.");

    return dataReader.GetString (classIDColumnOrdinal);
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
    ClassDefinition relatedClassDefinition = classDefinition.GetMandatoryOppositeClassDefinition (propertyDefinition.PropertyName);
    if (relatedClassDefinition.IsPartOfInheritanceHierarchy && classDefinition.StorageProviderID == relatedClassDefinition.StorageProviderID)
    {
      int classIDColumnOrdinal = -1;
      try
      {
        classIDColumnOrdinal = dataReader.GetOrdinal (RdbmsProvider.GetClassIDColumnName (propertyDefinition.ColumnName));
      }
      catch (IndexOutOfRangeException)
      {
        throw CreateRdbmsProviderException (
            "Incorrect database format encountered."
            + " Entity '{0}' must have column '{1}' defined, because opposite class '{2}' is part of an inheritance hierarchy.",
            classDefinition.GetEntityName (),
            RdbmsProvider.GetClassIDColumnName (propertyDefinition.ColumnName), 
            relatedClassDefinition.ID);    
      }

      if (dataReader.IsDBNull (objectIDColumnOrdinal) && !dataReader.IsDBNull (classIDColumnOrdinal))
      {
        throw CreateRdbmsProviderException (
            "Incorrect database value encountered. Column '{0}' of entity '{1}' must not contain a value.",
            RdbmsProvider.GetClassIDColumnName (propertyDefinition.ColumnName),
            classDefinition.GetEntityName ());
      }

      if (!dataReader.IsDBNull (objectIDColumnOrdinal) && dataReader.IsDBNull (classIDColumnOrdinal))
      {
        throw CreateRdbmsProviderException (
            "Incorrect database value encountered. Column '{0}' of entity '{1}' must not contain null.",
            RdbmsProvider.GetClassIDColumnName (propertyDefinition.ColumnName),
            classDefinition.GetEntityName ());
      }

      if (dataReader.IsDBNull (classIDColumnOrdinal))
        return relatedClassDefinition;
      else
        return MappingConfiguration.Current.ClassDefinitions.GetMandatory (dataReader.GetString (classIDColumnOrdinal));
    }
    else
    {
      // Note: We cannot ask an IDataReader if a specific column exists without an exception thrown by IDataReader.
      // Because throwing and catching exceptions is a very time consuming operation the result is cached per entity
      // and relation and is reused in subsequent calls.
      lock (typeof (ValueConverter))
      {
        int hashKey = GetClassIDColumnHashKey (classDefinition, propertyDefinition);
        if (!s_hasClassIDColumn.Contains (hashKey))
        {      
          try
          {
            dataReader.GetOrdinal (RdbmsProvider.GetClassIDColumnName (propertyDefinition.ColumnName));
            s_hasClassIDColumn[hashKey] = true;
          }
          catch (IndexOutOfRangeException)
          {
            s_hasClassIDColumn[hashKey] = false;
          }
        }

        if ((bool) s_hasClassIDColumn[hashKey])
        {
          throw CreateRdbmsProviderException (
              "Incorrect database format encountered. Entity '{0}' must not contain column '{1}', because opposite class '{2}' is not part of an inheritance hierarchy.",
              classDefinition.GetEntityName (),
              RdbmsProvider.GetClassIDColumnName (propertyDefinition.ColumnName),
              relatedClassDefinition.ID);
        }
      }

      return relatedClassDefinition;
    }
  }

  private int GetClassIDColumnHashKey (ClassDefinition classDefinition, PropertyDefinition propertyDefinition)
  {
    StorageProviderDefinition storageProviderDefinition = 
        StorageProviderConfiguration.Current.StorageProviderDefinitions.GetMandatory (classDefinition.StorageProviderID);

    return storageProviderDefinition.GetHashCode ()
        ^ classDefinition.GetEntityName ().GetHashCode ()
        ^ propertyDefinition.ColumnName.GetHashCode ();
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
