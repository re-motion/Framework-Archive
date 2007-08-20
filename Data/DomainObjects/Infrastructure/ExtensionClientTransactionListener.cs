using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Queries;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// A <see cref="IClientTransactionListener"/> implementation that notifies <see cref="IClientTransactionExtension">IClientTransactionExtensions</see>
  /// about transaction events.
  /// </summary>
  /// <remarks>
  /// The <see cref="ClientTransaction"/> class uses this listener to implement its extension mechanism.
  /// </remarks>
  [Serializable]
  public class ExtensionClientTransactionListener : IClientTransactionListener
  {
    private ClientTransactionExtensionCollection _extensions;
    private ClientTransaction _clientTransaction;

    public ExtensionClientTransactionListener (ClientTransaction clientTransaction, ClientTransactionExtensionCollection extensions)
    {
      _clientTransaction = clientTransaction;
      _extensions = extensions;
    }

    public ClientTransactionExtensionCollection Extensions
    {
      get { return _extensions; }
    }

    public void SubTransactionCreating ()
    {
    }

    public void SubTransactionCreated (ClientTransaction subTransaction)
    {
    }

    public void NewObjectCreating (Type type)
    {
      Extensions.NewObjectCreating (_clientTransaction, type);
    }

    public void ObjectLoading (ObjectID id)
    {
      Extensions.ObjectLoading (_clientTransaction, id);
    }

    public void ObjectsLoaded (DomainObjectCollection domainObjects)
    {
      Extensions.ObjectsLoaded (_clientTransaction, domainObjects);
    }

    public void ObjectDeleting (DomainObject domainObject)
    {
      Extensions.ObjectDeleting (_clientTransaction, domainObject);
    }

    public void ObjectDeleted (DomainObject domainObject)
    {
      Extensions.ObjectDeleted (_clientTransaction, domainObject);
    }

    public void PropertyValueReading (DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
      Extensions.PropertyValueReading (_clientTransaction, dataContainer, propertyValue, valueAccess);
    }

    public void PropertyValueRead (DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
      Extensions.PropertyValueRead (_clientTransaction, dataContainer, propertyValue, value, valueAccess);
    }

    public void PropertyValueChanging (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      Extensions.PropertyValueChanging (_clientTransaction, dataContainer, propertyValue, oldValue, newValue);
    }

    public void PropertyValueChanged (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      Extensions.PropertyValueChanged (_clientTransaction, dataContainer, propertyValue, oldValue, newValue);
    }

    public void RelationReading (DomainObject domainObject, string propertyName, ValueAccess valueAccess)
    {
      Extensions.RelationReading (_clientTransaction, domainObject, propertyName, valueAccess);
    }

    public void RelationRead (DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess)
    {
      Extensions.RelationRead (_clientTransaction, domainObject, propertyName, relatedObject, valueAccess);
    }

    public void RelationRead (DomainObject domainObject, string propertyName, DomainObjectCollection relatedObjects, ValueAccess valueAccess)
    {
      Extensions.RelationRead (_clientTransaction, domainObject, propertyName, relatedObjects, valueAccess);
    }

    public void RelationChanging (DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      Extensions.RelationChanging (_clientTransaction, domainObject, propertyName, oldRelatedObject, newRelatedObject);
    }

    public void RelationChanged (DomainObject domainObject, string propertyName)
    {
      Extensions.RelationChanged (_clientTransaction, domainObject, propertyName);
    }

    public void FilterQueryResult (DomainObjectCollection queryResult, IQuery query)
    {
      Extensions.FilterQueryResult (_clientTransaction, queryResult, query);
    }

    public void TransactionCommitting (DomainObjectCollection domainObjects)
    {
      Extensions.Committing (_clientTransaction, domainObjects);
    }

    public void TransactionCommitted (DomainObjectCollection domainObjects)
    {
      Extensions.Committed (_clientTransaction, domainObjects);
    }

    public void TransactionRollingBack (DomainObjectCollection domainObjects)
    {
      Extensions.RollingBack (_clientTransaction, domainObjects);
    }

    public void TransactionRolledBack (DomainObjectCollection domainObjects)
    {
      Extensions.RolledBack (_clientTransaction, domainObjects);
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
