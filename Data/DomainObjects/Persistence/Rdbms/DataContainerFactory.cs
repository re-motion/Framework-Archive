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
  private RdbmsProvider _provider;

  // construction and disposing

  public DataContainerFactory (RdbmsProvider provider, IDataReader dataReader)
  {
    ArgumentUtility.CheckNotNull ("provider", provider);
    ArgumentUtility.CheckNotNull ("dataReader", dataReader);
    _dataReader = dataReader;
    _provider = provider;
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
    ValueConverter valueConverter = _provider.ValueConverter;
    
    ObjectID id = valueConverter.GetID (_dataReader);
    object timestamp = _dataReader.GetValue (valueConverter.GetMandatoryOrdinal (_dataReader, "Timestamp"));

    DataContainer dataContainer = DataContainer.CreateForExisting (id, timestamp);

    foreach (PropertyDefinition propertyDefinition in id.ClassDefinition.GetPropertyDefinitions ())
    {
      int columnOrdinal = valueConverter.GetMandatoryOrdinal (_dataReader, propertyDefinition.ColumnName);

      object dataValue;

      try
      {
        dataValue = valueConverter.GetValue (id.ClassDefinition, propertyDefinition, _dataReader, columnOrdinal);
      }
      catch (Exception e)
      {
        throw CreateRdbmsProviderException (e, "Error while reading property '{0}' of object '{1}': {2}",
            propertyDefinition.PropertyName, id, e.Message);
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
