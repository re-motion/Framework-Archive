using System;
using System.Data;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence.Rdbms
{
public class DataContainerFactory
{
  // types

  // static members and constants

  // member fields

  private IDataReader _dataReader;

  // construction and disposing

  public DataContainerFactory (IDataReader dataReader)
  {
    ArgumentUtility.CheckNotNull ("dataReader", dataReader);
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
    ValueConverter valueConverter = new ValueConverter ();

    string classID = _dataReader.GetString (valueConverter.GetMandatoryOrdinal (_dataReader, "ClassID"));
    ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[classID];

    object idValue = _dataReader.GetValue (valueConverter.GetMandatoryOrdinal (_dataReader, "ID"));
    object timestamp = _dataReader.GetValue (valueConverter.GetMandatoryOrdinal (_dataReader, "Timestamp"));

    if (classDefinition == null)
      throw CreateRdbmsProviderException ("Invalid ClassID '{0}' for ID '{1}' encountered.", classID, idValue);

    ObjectID id = valueConverter.GetObjectID (classDefinition, idValue);
    DataContainer dataContainer = DataContainer.CreateForExisting (id, timestamp);

    foreach (PropertyDefinition propertyDefinition in classDefinition.GetPropertyDefinitions ())
    {
      int columnOrdinal = valueConverter.GetMandatoryOrdinal (_dataReader, propertyDefinition.ColumnName);

      object dataValue;

      try
      {
        dataValue = valueConverter.GetValue (classDefinition, propertyDefinition, _dataReader, columnOrdinal);
      }
      catch (RdbmsProviderException e)
      {
        throw CreateRdbmsProviderException (e, "Error while reading property '{0}' for class '{1}': {2}",
            propertyDefinition.PropertyName, classDefinition.ID, e.Message);
      }
      catch (ConverterException e)
      {
        throw CreateRdbmsProviderException (e, "Error while reading property '{0}' for class '{1}': {2}",
            propertyDefinition.PropertyName, classDefinition.ID, e.Message);
      }
      catch (InvalidCastException e)
      {
        throw CreateRdbmsProviderException (e, "Error while reading property '{0}' for class '{1}': {2}",
            propertyDefinition.PropertyName, classDefinition.ID, e.Message);
      }

      dataContainer.PropertyValues.Add (new PropertyValue (propertyDefinition, dataValue));
    }

    return dataContainer;
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
