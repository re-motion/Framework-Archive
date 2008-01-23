using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Rubicon.Collections;
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
    private readonly ClientTransaction _sourceTransaction;
    private readonly DomainObjectCollection _transportedObjects;
    private readonly ObjectID[] _transportedObjectIDs;

    public DomainObjectImporter (byte[] data)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("data", data);
      _sourceTransaction = DeserializeData (data);
      _transportedObjects = ExtractTransportedObjects (_sourceTransaction);
      _transportedObjectIDs = GetObjectIDs (_transportedObjects);
    }

    private DomainObjectCollection ExtractTransportedObjects (ClientTransaction transaction)
    {
      return new DomainObjectCollection (transaction.EnlistedDomainObjects, true);
    }

    private ObjectID[] GetObjectIDs (DomainObjectCollection domainObjects)
    {
      ObjectID[] ids = new ObjectID[domainObjects.Count];
      for (int i = 0; i < domainObjects.Count; ++i)
        ids[i] = domainObjects[i].ID;
      return ids;
    }

    private ClientTransaction DeserializeData (byte[] data)
    {
      using (MemoryStream stream = new MemoryStream (data))
      {
        BinaryFormatter formatter = new BinaryFormatter ();
        try
        {
          return (ClientTransaction) formatter.Deserialize (stream);
        }
        catch (SerializationException ex)
        {
          throw new ArgumentException ("Invalid data specified: " + ex.Message, "data", ex);
        }
      }
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
      if (_transportedObjects.Count > 0)
      {
        using (targetTransaction.EnterNonDiscardingScope())
        {
          ObjectList<DomainObject> existingObjects = targetTransaction.TryGetObjects<DomainObject> (_transportedObjectIDs);

          for (int i = 0; i < _transportedObjects.Count; ++i)
          {
            DomainObject sourceObject = _transportedObjects[i];
            DataContainer sourceDataContainer = sourceObject.GetDataContainerForTransaction (_sourceTransaction);

            DomainObject existingObject = existingObjects[sourceObject.ID];
            DataContainer targetDataContainer;
            
            if (existingObject != null)
              targetDataContainer = existingObject.GetDataContainerForTransaction (targetTransaction);
            else
            {
              targetDataContainer = targetTransaction.CreateNewDataContainer (sourceObject.ID);
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
        ClientTransaction targetTransaction = sourceToTargetContainer.B.ClientTransaction;

        DomainObject sourceObject = sourceToTargetContainer.A.DomainObject;
        DomainObject targetObject = sourceToTargetContainer.B.DomainObject;
        foreach (PropertyAccessor sourceProperty in sourceObject.Properties)
        {
          PropertyAccessor targetProperty = targetObject.Properties[sourceProperty.PropertyIdentifier];
          switch (sourceProperty.Kind)
          {
            case PropertyKind.PropertyValue:
              targetProperty.SetValueWithoutTypeCheckTx (targetTransaction, sourceProperty.GetValueWithoutTypeCheckTx (_sourceTransaction));
              break;
            case PropertyKind.RelatedObject:
              if (!sourceProperty.RelationEndPointDefinition.IsVirtual)
              {
                ObjectID relatedObjectID = sourceProperty.GetRelatedObjectIDTx (_sourceTransaction);
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
