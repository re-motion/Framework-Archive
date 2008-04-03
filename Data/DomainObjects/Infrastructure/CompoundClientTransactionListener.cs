using System;
using System.Collections.Generic;
using System.Text;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Implements a collection of <see cref="IClientTransactionListener"/> objects.
  /// </summary>
  [Serializable]
  public class CompoundClientTransactionListener : IClientTransactionListener
  {
    private readonly List<IClientTransactionListener> _listeners = new List<IClientTransactionListener> ();

    public void AddListener (IClientTransactionListener listener)
    {
      ArgumentUtility.CheckNotNull ("listener", listener);

      _listeners.Add (listener);
    }

    public void SubTransactionCreating ()
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.SubTransactionCreating ();
    }

    public void SubTransactionCreated (ClientTransaction subTransaction)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.SubTransactionCreated (subTransaction);
    }

    public void NewObjectCreating (Type type, DomainObject instance)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.NewObjectCreating (type, instance);
    }

    public void ObjectLoading (ObjectID id)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.ObjectLoading (id);
    }

    public void ObjectInitializedFromDataContainer (ObjectID id, DomainObject instance)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.ObjectInitializedFromDataContainer (id, instance);
    }

    public void ObjectsLoaded (DomainObjectCollection domainObjects)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.ObjectsLoaded (domainObjects);
    }

    public void ObjectDeleting (DomainObject domainObject)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.ObjectDeleting (domainObject);
    }

    public void ObjectDeleted (DomainObject domainObject)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.ObjectDeleted (domainObject);
    }

    public void PropertyValueReading (DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.PropertyValueReading (dataContainer, propertyValue, valueAccess);
    }

    public void PropertyValueRead (DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.PropertyValueRead (dataContainer, propertyValue, value, valueAccess);
    }

    public void PropertyValueChanging (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.PropertyValueChanging (dataContainer, propertyValue, oldValue, newValue);
    }

    public void PropertyValueChanged (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.PropertyValueChanged (dataContainer, propertyValue, oldValue, newValue);
    }

    public void RelationReading (DomainObject domainObject, string propertyName, ValueAccess valueAccess)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.RelationReading (domainObject, propertyName, valueAccess);
    }

    public void RelationRead (DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.RelationRead (domainObject, propertyName, relatedObject, valueAccess);
    }

    public void RelationRead (DomainObject domainObject, string propertyName, DomainObjectCollection relatedObjects, ValueAccess valueAccess)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.RelationRead (domainObject, propertyName, relatedObjects, valueAccess);
    }

    public void RelationChanging (DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.RelationChanging (domainObject, propertyName, oldRelatedObject, newRelatedObject);
    }

    public void RelationChanged (DomainObject domainObject, string propertyName)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.RelationChanged (domainObject, propertyName);
    }

    public void FilterQueryResult (DomainObjectCollection queryResult, IQuery query)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.FilterQueryResult (queryResult, query);
    }

    public void TransactionCommitting (DomainObjectCollection domainObjects)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.TransactionCommitting (domainObjects);
    }

    public void TransactionCommitted (DomainObjectCollection domainObjects)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.TransactionCommitted (domainObjects);
    }

    public void TransactionRollingBack (DomainObjectCollection domainObjects)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.TransactionRollingBack (domainObjects);
    }

    public void TransactionRolledBack (DomainObjectCollection domainObjects)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.TransactionRolledBack (domainObjects);
    }

    public void RelationEndPointMapRegistering (RelationEndPoint endPoint)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.RelationEndPointMapRegistering (endPoint);
    }

    public void RelationEndPointMapUnregistering (RelationEndPointID endPointID)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.RelationEndPointMapUnregistering (endPointID);
    }

    public void RelationEndPointMapPerformingDelete (RelationEndPointID[] endPointIDs)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.RelationEndPointMapPerformingDelete (endPointIDs);
    }

    public void RelationEndPointMapCopyingFrom (RelationEndPointMap source)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.RelationEndPointMapCopyingFrom (source);
    }

    public void RelationEndPointMapCopyingTo (RelationEndPointMap source)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.RelationEndPointMapCopyingTo (source);
    }

    public void DataManagerMarkingObjectDiscarded (ObjectID id)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.DataManagerMarkingObjectDiscarded (id);
    }

    public void DataManagerCopyingFrom (DataManager source)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.DataManagerCopyingFrom (source);
    }

    public void DataManagerCopyingTo (DataManager destination)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.DataManagerCopyingTo (destination);
    }

    public void DataContainerMapRegistering (DataContainer container)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.DataContainerMapRegistering (container);
    }

    public void DataContainerMapUnregistering (DataContainer container)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.DataContainerMapUnregistering (container);
    }

    public void DataContainerMapCopyingFrom (DataContainerMap source)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.DataContainerMapCopyingFrom (source);
    }

    public void DataContainerMapCopyingTo (DataContainerMap destination)
    {
      foreach (IClientTransactionListener listener in _listeners)
        listener.DataContainerMapCopyingTo (destination);
    }
  }
}
