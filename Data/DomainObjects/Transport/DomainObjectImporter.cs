using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Rubicon.Collections;
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
    private readonly DataContainer[] _transportedContainers;

    public DomainObjectImporter (byte[] data, IImportStrategy importStrategy)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("data", data);
      ArgumentUtility.CheckNotNull ("importStrategy", importStrategy);

      _transportedContainers = importStrategy.Import (data);
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
          ObjectID[] transportedObjectIDs = GetIDs (_transportedContainers);
          ObjectList<DomainObject> existingObjects = targetTransaction.TryGetObjects<DomainObject> (transportedObjectIDs);

          foreach (DataContainer sourceDataContainer in _transportedContainers)
          {
            DataContainer targetDataContainer = GetTargetDataContainer(sourceDataContainer, existingObjects, targetTransaction);
            result.Add (Tuple.NewTuple (sourceDataContainer, targetDataContainer));
          }
        }
      }
      return result;
    }

    private DataContainer GetTargetDataContainer (DataContainer sourceDataContainer, ObjectList<DomainObject> existingObjects, ClientTransaction targetTransaction)
    {
      DomainObject existingObject = existingObjects[sourceDataContainer.ID];
      if (existingObject != null)
        return existingObject.GetDataContainerForTransaction (targetTransaction);
      else
      {
        DataContainer targetDataContainer = targetTransaction.CreateNewDataContainer (sourceDataContainer.ID);
        targetTransaction.EnlistDomainObject (targetDataContainer.DomainObject);
        return targetDataContainer;
      }
    }

    private ObjectID[] GetIDs (DataContainer[] containers)
    {
      return Array.ConvertAll<DataContainer, ObjectID> (containers, delegate (DataContainer container) { return container.ID; });
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
