using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Rubicon.Collections;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Infrastructure;

namespace Rubicon.Data.DomainObjects.Transport
{
  /// <summary>
  /// Assists in importing data exported by a <see cref="DomainObjectTransporter"/> object. This class is used by
  /// <see cref="DomainObjectTransporter.LoadTransportData"/> and is usually not instantiated by itself.
  /// </summary>
  public class DomainObjectImporter
  {
    private readonly ObjectID[] _transportedObjectIDs;
    private readonly DataContainer[] _transportedContainers;

    public DomainObjectImporter (byte[] data)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("data", data);
      _transportedContainers = DeserializeData (data);
      _transportedObjectIDs = GetIDs (_transportedContainers);
    }

    private ObjectID[] GetIDs (DataContainer[] containers)
    {
      return Array.ConvertAll<DataContainer, ObjectID> (containers, delegate (DataContainer container) { return container.ID; });
    }

    private DataContainer[] DeserializeData (byte[] data)
    {
      using (MemoryStream stream = new MemoryStream (data))
      {
        BinaryFormatter formatter = new BinaryFormatter ();
        try
        {
          Tuple<ClientTransaction, ObjectID[]> deserializedData = (Tuple<ClientTransaction, ObjectID[]>) formatter.Deserialize (stream);
          return GetTransportedContainers (deserializedData);
        }
        catch (SerializationException ex)
        {
          throw new ArgumentException ("Invalid data specified: " + ex.Message, "data", ex);
        }
      }
    }

    private DataContainer[] GetTransportedContainers (Tuple<ClientTransaction, ObjectID[]> deserializedData)
    {
      DataContainer[] dataContainers = new DataContainer[deserializedData.B.Length];
      for (int i = 0; i < dataContainers.Length; i++)
        dataContainers[i] = deserializedData.A.DataManager.DataContainerMap[deserializedData.B[i]];
      return dataContainers;
    }

    public TransportedDomainObjects GetImportedObjects ()
    {
      ClientTransaction targetTransaction = ClientTransaction.NewBindingTransaction ();
      List<Tuple<DataContainer, DataContainer>> dataContainerMapping = GetTargetDataContainersForSourceObjects (targetTransaction);

      // grab enlisted objects _before_ properties are synchronized, as synchronizing might load some additional objects
      List<DomainObject> transportedObjects = new List<DomainObject> (targetTransaction.EnlistedDomainObjects);
      SynchronizeData (dataContainerMapping);

      return new TransportedDomainObjects (targetTransaction, transportedObjects);
    }

    private List<Tuple<DataContainer, DataContainer>> GetTargetDataContainersForSourceObjects (ClientTransaction targetTransaction)
    {
      List<Tuple<DataContainer, DataContainer>> result = new List<Tuple<DataContainer, DataContainer>> ();
      if (_transportedContainers.Length > 0)
      {
        using (targetTransaction.EnterNonDiscardingScope())
        {
          ObjectList<DomainObject> existingObjects = targetTransaction.TryGetObjects<DomainObject> (_transportedObjectIDs);

          foreach (DataContainer sourceDataContainer in _transportedContainers)
          {
            DomainObject existingObject = existingObjects[sourceDataContainer.ID];
            DataContainer targetDataContainer;
            
            if (existingObject != null)
              targetDataContainer = existingObject.GetDataContainerForTransaction (targetTransaction);
            else
            {
              targetDataContainer = targetTransaction.CreateNewDataContainer (sourceDataContainer.ID);
              targetTransaction.EnlistDomainObject (targetDataContainer.DomainObject);
            }

            result.Add (Tuple.NewTuple (sourceDataContainer, targetDataContainer));
          }
        }
      }
      return result;
    }

    private void SynchronizeData (IEnumerable<Tuple<DataContainer, DataContainer>> sourceToTargetMapping)
    {
      foreach (Tuple<DataContainer, DataContainer> sourceToTargetContainer in sourceToTargetMapping)
      {
        DataContainer sourceContainer = sourceToTargetContainer.A;
        DataContainer targetContainer = sourceToTargetContainer.B;
        DomainObject targetObject = targetContainer.DomainObject;
        ClientTransaction targetTransaction = targetContainer.ClientTransaction;

        foreach (PropertyValue sourceProperty in sourceContainer.PropertyValues)
        {
          PropertyAccessor targetProperty = targetObject.Properties[sourceProperty.Name];
          switch (targetProperty.Kind)
          {
            case PropertyKind.PropertyValue:
              targetProperty.SetValueWithoutTypeCheckTx (targetTransaction, sourceProperty.Value);
              break;
            case PropertyKind.RelatedObject:
              if (!targetProperty.RelationEndPointDefinition.IsVirtual)
              {
                ObjectID relatedObjectID = (ObjectID) sourceProperty.Value;
                DomainObject targetRelatedObject = relatedObjectID != null ? targetTransaction.GetObject (relatedObjectID) : null;
                targetProperty.SetValueWithoutTypeCheckTx (targetTransaction, targetRelatedObject);
              }
              break;
          }
        }
      }
    }
  }
}
