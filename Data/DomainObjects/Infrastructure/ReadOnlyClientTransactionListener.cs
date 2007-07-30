using System;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// An implementation of <see cref="IClientTransactionListener"/> which throws an exception if the <see cref="ClientTransaction"/> is about
  /// to be modified while in a read-only state.
  /// </summary>
  [Serializable]
  public class ReadOnlyClientTransactionListener : IClientTransactionListener
  {
    private readonly ClientTransaction _clientTransaction;

    public ReadOnlyClientTransactionListener (ClientTransaction clientTransaction)
    {
      _clientTransaction = clientTransaction;
    }

    private void EnsureWriteable (string operation)
    {
      if (_clientTransaction.IsReadOnly)
      {
        string message = string.Format (
            "The operation cannot be executed because the ClientTransaction is read-only. "
            + "Offending transaction modification: {0}.",
            operation);
        throw new ClientTransactionReadOnlyException (message);
      }
    }

    public void SubTransactionCreating ()
    {
      EnsureWriteable ("SubTransactionCreating");
    }

    public void SubTransactionCreated (ClientTransaction subTransaction)
    {
      Assertion.IsTrue (_clientTransaction.IsReadOnly); // after a subtransaction has been created, the parent must be read-only
    }

    public void NewObjectCreating (Type type)
    {
      EnsureWriteable ("NewObjectCreating");
    }

    public void ObjectLoading (ObjectID id)
    {
      EnsureWriteable ("ObjectLoading");
    }

    public void ObjectsLoaded (DomainObjectCollection domainObjects)
    {
      Assertion.IsFalse (_clientTransaction.IsReadOnly);
    }

    public void ObjectDeleting (DomainObject domainObject)
    {
      EnsureWriteable ("ObjectDeleting");
    }

    public void ObjectDeleted (DomainObject domainObject)
    {
      Assertion.IsFalse(_clientTransaction.IsReadOnly);
    }

    public void PropertyValueReading (DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
    }

    public void PropertyValueRead (DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
    }

    public void PropertyValueChanging (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      EnsureWriteable ("PropertyValueChanging");
    }

    public void PropertyValueChanged (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      Assertion.IsFalse (_clientTransaction.IsReadOnly);
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
      EnsureWriteable ("RelationChanging");
    }

    public void RelationChanged (DomainObject domainObject, string propertyName)
    {
      Assertion.IsFalse (_clientTransaction.IsReadOnly);
    }

    public void FilterQueryResult (DomainObjectCollection queryResult, IQuery query)
    {
    }

    public void TransactionCommitting (DomainObjectCollection domainObjects)
    {
      EnsureWriteable ("TransactionCommitting");
    }

    public void TransactionCommitted (DomainObjectCollection domainObjects)
    {
      Assertion.IsFalse (_clientTransaction.IsReadOnly);
    }

    public void TransactionRollingBack (DomainObjectCollection domainObjects)
    {
      EnsureWriteable ("TransactionRollingBack");
    }

    public void TransactionRolledBack (DomainObjectCollection domainObjects)
    {
      Assertion.IsFalse (_clientTransaction.IsReadOnly);
    }

    public void RelationEndPointMapRegistering (RelationEndPoint endPoint)
    {
      EnsureWriteable ("RelationEndPointMapRegistering");
    }

    public void RelationEndPointMapUnregistering (RelationEndPointID endPointID)
    {
      Assertion.IsFalse (_clientTransaction.IsReadOnly);
    }

    public void RelationEndPointMapPerformingDelete (RelationEndPointID[] endPointIDs)
    {
      Assertion.IsFalse (_clientTransaction.IsReadOnly);
    }

    public void RelationEndPointMapCopyingFrom (RelationEndPointMap source)
    {
      Assertion.IsFalse (_clientTransaction.IsReadOnly);
    }

    public void RelationEndPointMapCopyingTo (RelationEndPointMap source)
    {
    }

    public void DataManagerMarkingObjectDiscarded (ObjectID id)
    {
      Assertion.IsFalse (_clientTransaction.IsReadOnly);
    }

    public void DataManagerCopyingFrom (DataManager source)
    {
      Assertion.IsFalse (_clientTransaction.IsReadOnly);
    }

    public void DataManagerCopyingTo (DataManager destination)
    {
    }

    public void DataContainerMapRegistering (DataContainer container)
    {
      Assertion.IsFalse (_clientTransaction.IsReadOnly);
    }

    public void DataContainerMapUnregistering (DataContainer container)
    {
      Assertion.IsFalse (_clientTransaction.IsReadOnly);
    }

    public void DataContainerMapCopyingFrom (DataContainerMap source)
    {
      Assertion.IsFalse (_clientTransaction.IsReadOnly);
    }

    public void DataContainerMapCopyingTo (DataContainerMap destination)
    {
    }
  }
}