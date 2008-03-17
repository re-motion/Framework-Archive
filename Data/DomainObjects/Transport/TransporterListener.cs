using System;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Transport
{
  [Serializable]
  public class TransporterListener : IClientTransactionListener
  {
    [NonSerialized]
    private readonly DomainObjectTransporter _transporter;

    public TransporterListener (DomainObjectTransporter transporter)
    {
      ArgumentUtility.CheckNotNull ("transporter", transporter);
      _transporter = transporter;
    }

    public void SubTransactionCreating ()
    {
      
    }

    public void SubTransactionCreated (ClientTransaction subTransaction)
    {
      
    }

    public void NewObjectCreating (Type type, DomainObject instance)
    {
      
    }

    public void ObjectLoading (ObjectID id)
    {
      
    }

    public void ObjectsLoaded (DomainObjectCollection domainObjects)
    {
      
    }

    public void ObjectInitializedFromDataContainer (ObjectID id, DomainObject instance)
    {
      
    }

    public void ObjectDeleting (DomainObject domainObject)
    {
      
    }

    public void ObjectDeleted (DomainObject domainObject)
    {
      
    }

    public void PropertyValueReading (DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
      
    }

    public void PropertyValueRead (DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
      
    }

    public void PropertyValueChanging (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      if (_transporter == null)
        throw new InvalidOperationException ("Cannot use the transported transaction for changing properties after it has been deserialized.");

      if (!_transporter.IsLoaded (dataContainer.ID))
      {
        string message = string.Format ("Object '{0}' cannot be modified for transportation because it hasn't been loaded yet. Load it before "
            + "manipulating it.", dataContainer.ID);
        throw new InvalidOperationException(message);
      }
    }

    public void PropertyValueChanged (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      
    }

    public void RelationReading (DomainObject domainObject, string propertyName, ValueAccess valueAccess)
    {
      
    }

    public void RelationRead (DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess)
    {
      
    }

    public void RelationRead (DomainObject domainObject, string propertyName, DomainObjectCollection relatedObjects, ValueAccess valueAccess)
    {
      
    }

    public void RelationChanging (DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      
    }

    public void RelationChanged (DomainObject domainObject, string propertyName)
    {
      
    }

    public void FilterQueryResult (DomainObjectCollection queryResult, IQuery query)
    {
      
    }

    public void TransactionCommitting (DomainObjectCollection domainObjects)
    {
      
    }

    public void TransactionCommitted (DomainObjectCollection domainObjects)
    {
      
    }

    public void TransactionRollingBack (DomainObjectCollection domainObjects)
    {
      
    }

    public void TransactionRolledBack (DomainObjectCollection domainObjects)
    {
      
    }

    public void RelationEndPointMapRegistering (RelationEndPoint endPoint)
    {
      
    }

    public void RelationEndPointMapUnregistering (RelationEndPointID endPointID)
    {
      
    }

    public void RelationEndPointMapPerformingDelete (RelationEndPointID[] endPointIDs)
    {
      
    }

    public void RelationEndPointMapCopyingFrom (RelationEndPointMap source)
    {
      
    }

    public void RelationEndPointMapCopyingTo (RelationEndPointMap source)
    {
      
    }

    public void DataManagerMarkingObjectDiscarded (ObjectID id)
    {
      
    }

    public void DataManagerCopyingFrom (DataManager source)
    {
      
    }

    public void DataManagerCopyingTo (DataManager destination)
    {
      
    }

    public void DataContainerMapRegistering (DataContainer container)
    {
      
    }

    public void DataContainerMapUnregistering (DataContainer container)
    {
      
    }

    public void DataContainerMapCopyingFrom (DataContainerMap source)
    {
      
    }

    public void DataContainerMapCopyingTo (DataContainerMap destination)
    {
      
    }
  }
}