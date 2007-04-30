using System;
using System.Collections;
using System.Data;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence.Rdbms
{
  public class ValueConverter : ValueConverterBase
  {
    private static Hashtable s_hasClassIDColumn = new Hashtable ();

    private readonly bool _usesView;

    public ValueConverter (bool usesView)
    {
      _usesView = usesView;
    }

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

    [Obsolete("Passing the columnOrdinal into GetValue is no longer supported. (Version 1.7.42)")]
    public object GetValue (ClassDefinition classDefinition, PropertyDefinition propertyDefinition, IDataReader dataReader, int columnOrdinal)
    {
      return GetValue (classDefinition, propertyDefinition, dataReader);
    }

    public object GetValue (ClassDefinition classDefinition, PropertyDefinition propertyDefinition, IDataReader dataReader)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);

      if (propertyDefinition.PropertyType != typeof (ObjectID))
        return GetValue (classDefinition, propertyDefinition, GetValue(dataReader, GetColumnName(classDefinition, propertyDefinition)));
      else
        return GetObjectID (classDefinition, propertyDefinition, dataReader, GetColumnName (classDefinition, propertyDefinition));
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

      object idValue = GetValue (dataReader, "ID");
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

    private object GetValue (IDataReader dataReader, string columnName)
    {
      return dataReader.GetValue (GetMandatoryOrdinal (dataReader, columnName));
    }

    private ClassDefinition GetClassDefinition (IDataReader dataReader, object idValue)
    {
      string classID = GetClassID (dataReader);

      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[classID];
      if (classDefinition == null)
        throw CreateRdbmsProviderException ("Invalid ClassID '{0}' for ID '{1}' encountered.", classID, idValue);

      if (classDefinition.IsAbstract)
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

    private ObjectID GetObjectID (ClassDefinition classDefinition, PropertyDefinition propertyDefinition, IDataReader dataReader, string columnName)
    {
      return GetObjectID (classDefinition, propertyDefinition, dataReader, GetMandatoryOrdinal (dataReader, columnName));
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
        int classIDColumnOrdinal;
        try
        {
          classIDColumnOrdinal = dataReader.GetOrdinal (RdbmsProvider.GetClassIDColumnName (GetColumnName (classDefinition, propertyDefinition)));
        }
        catch (IndexOutOfRangeException)
        {
          throw CreateRdbmsProviderException (
              "Incorrect database format encountered."
              + " Entity '{0}' must have column '{1}' defined, because opposite class '{2}' is part of an inheritance hierarchy.",
              classDefinition.GetEntityName (),
              RdbmsProvider.GetClassIDColumnName (propertyDefinition.StorageSpecificName),
              relatedClassDefinition.ID);
        }

        if (dataReader.IsDBNull (objectIDColumnOrdinal) && !dataReader.IsDBNull (classIDColumnOrdinal))
        {
          throw CreateRdbmsProviderException (
              "Incorrect database value encountered. Column '{0}' of entity '{1}' must not contain a value.",
              RdbmsProvider.GetClassIDColumnName (propertyDefinition.StorageSpecificName),
              classDefinition.GetEntityName ());
        }

        if (!dataReader.IsDBNull (objectIDColumnOrdinal) && dataReader.IsDBNull (classIDColumnOrdinal))
        {
          throw CreateRdbmsProviderException (
              "Incorrect database value encountered. Column '{0}' of entity '{1}' must not contain null.",
              RdbmsProvider.GetClassIDColumnName (propertyDefinition.StorageSpecificName),
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
              dataReader.GetOrdinal (RdbmsProvider.GetClassIDColumnName (GetColumnName (classDefinition, propertyDefinition)));
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
                RdbmsProvider.GetClassIDColumnName (propertyDefinition.StorageSpecificName),
                relatedClassDefinition.ID);
          }
        }

        return relatedClassDefinition;
      }
    }

    private int GetClassIDColumnHashKey (ClassDefinition classDefinition, PropertyDefinition propertyDefinition)
    {
      StorageProviderDefinition storageProviderDefinition =
          DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions.GetMandatory (classDefinition.StorageProviderID);

      return storageProviderDefinition.GetHashCode ()
          ^ classDefinition.GetEntityName ().GetHashCode ()
          ^ propertyDefinition.StorageSpecificName.GetHashCode ();
    }

    private string GetColumnName (ClassDefinition classDefinition, PropertyDefinition propertyDefinition)
    {
      if (_usesView)
        return classDefinition.GetFullyQualifiedStorageSpecificNameForProperty (propertyDefinition.PropertyName);
      else
        return propertyDefinition.StorageSpecificName;
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