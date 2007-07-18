using System;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Queries;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  public class InvalidatedSubTransactionListener : IClientTransactionListener
  {
    private void ThrowException ()
    {
      throw new InvalidOperationException ("The subtransaction can no longer be used because control has returned to its parent transaction.");
    }

    public void SubTransactionCreating ()
    {
      ThrowException();
    }

    public void SubTransactionCreated (ClientTransaction subTransaction)
    {
      ThrowException();
    }

    public void NewObjectCreating (Type type)
    {
      ThrowException();
    }

    public void ObjectLoading (ObjectID id)
    {
      ThrowException();
    }

    public void ObjectsLoaded (DomainObjectCollection domainObjects)
    {
      ThrowException();
    }

    public void ObjectDeleting (DomainObject domainObject)
    {
      ThrowException();
    }

    public void ObjectDeleted (DomainObject domainObject)
    {
      ThrowException();
    }

    public void PropertyValueReading (DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
      ThrowException();
    }

    public void PropertyValueRead (DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
      ThrowException();
    }

    public void PropertyValueChanging (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      ThrowException();
    }

    public void PropertyValueChanged (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      ThrowException();
    }

    public void RelationReading (DomainObject domainObject, string propertyName, ValueAccess valueAccess)
    {
      ThrowException();
    }

    public void RelationRead (DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess)
    {
      ThrowException();
    }

    public void RelationRead (DomainObject domainObject, string propertyName, DomainObjectCollection relatedObjects, ValueAccess valueAccess)
    {
      ThrowException();
    }

    public void RelationChanging (DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      ThrowException();
    }

    public void RelationChanged (DomainObject domainObject, string propertyName)
    {
      ThrowException();
    }

    public void FilterQueryResult (DomainObjectCollection queryResult, IQuery query)
    {
      ThrowException();
    }

    public void TransactionCommitting (DomainObjectCollection domainObjects)
    {
      ThrowException();
    }

    public void TransactionCommitted (DomainObjectCollection domainObjects)
    {
      ThrowException();
    }

    public void TransactionRollingBack (DomainObjectCollection domainObjects)
    {
      ThrowException();
    }

    public void TransactionRolledBack (DomainObjectCollection domainObjects)
    {
      ThrowException();
    }

    public void RelationEndPointMapRegistering (RelationEndPoint endPoint)
    {
      ThrowException();
    }

    public void RelationEndPointMapUnregistering (RelationEndPointID endPointID)
    {
      ThrowException();
    }

    public void RelationEndPointMapPerformingDelete (RelationEndPointID[] endPointIDs)
    {
      ThrowException();
    }

    public void RelationEndPointMapCopyingFrom (RelationEndPointMap source)
    {
      ThrowException();
    }

    public void RelationEndPointMapCopyingTo (RelationEndPointMap source)
    {
      ThrowException();
    }

    public void DataManagerMarkingObjectDiscarded (ObjectID id)
    {
      ThrowException();
    }

    public void DataManagerCopyingFrom (DataManager source)
    {
      ThrowException();
    }

    public void DataManagerCopyingTo (DataManager destination)
    {
      ThrowException();
    }

    public void DataContainerMapRegistering (DataContainer container)
    {
      ThrowException();
    }

    public void DataContainerMapUnregistering (DataContainer container)
    {
      ThrowException();
    }

    public void DataContainerMapCopyingFrom (DataContainerMap source)
    {
      ThrowException();
    }

    public void DataContainerMapCopyingTo (DataContainerMap destination)
    {
      ThrowException();
    }
  }
}