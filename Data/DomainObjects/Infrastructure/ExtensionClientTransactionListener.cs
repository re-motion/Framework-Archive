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

    public ExtensionClientTransactionListener (ClientTransactionExtensionCollection extensions)
    {
      _extensions = extensions;
    }

    public ClientTransactionExtensionCollection Extensions
    {
      get { return _extensions; }
    }

    public void NewObjectCreating (Type type)
    {
      _extensions.NewObjectCreating (type);
    }

    public void ObjectLoading (ObjectID id)
    {
      _extensions.ObjectLoading (id);
    }

    public void ObjectsLoaded (DomainObjectCollection domainObjects)
    {
      _extensions.ObjectsLoaded (domainObjects);
    }

    public void ObjectDeleting (DomainObject domainObject)
    {
      _extensions.ObjectDeleting (domainObject);
    }

    public void ObjectDeleted (DomainObject domainObject)
    {
      _extensions.ObjectDeleted (domainObject);
    }

    public void PropertyValueReading (DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
      _extensions.PropertyValueReading (dataContainer, propertyValue, valueAccess);
    }

    public void PropertyValueRead (DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
      _extensions.PropertyValueRead (dataContainer, propertyValue, value, valueAccess);
    }

    public void PropertyValueChanging (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      _extensions.PropertyValueChanging (dataContainer, propertyValue, oldValue, newValue);
    }

    public void PropertyValueChanged (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      _extensions.PropertyValueChanged (dataContainer, propertyValue, oldValue, newValue);
    }

    public void RelationReading (DomainObject domainObject, string propertyName, ValueAccess valueAccess)
    {
      _extensions.RelationReading (domainObject, propertyName, valueAccess);
    }

    public void RelationRead (DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess)
    {
      _extensions.RelationRead (domainObject, propertyName, relatedObject, valueAccess);
    }

    public void RelationRead (DomainObject domainObject, string propertyName, DomainObjectCollection relatedObjects, ValueAccess valueAccess)
    {
      _extensions.RelationRead (domainObject, propertyName, relatedObjects, valueAccess);
    }

    public void RelationChanging (DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      _extensions.RelationChanging (domainObject, propertyName, oldRelatedObject, newRelatedObject);
    }

    public void RelationChanged (DomainObject domainObject, string propertyName)
    {
      _extensions.RelationChanged (domainObject, propertyName);
    }

    public void FilterQueryResult (DomainObjectCollection queryResult, IQuery query)
    {
      _extensions.FilterQueryResult (queryResult, query);
    }

    public void TransactionCommitting (DomainObjectCollection domainObjects)
    {
      _extensions.Committing (domainObjects);
    }

    public void TransactionCommitted (DomainObjectCollection domainObjects)
    {
      _extensions.Committed (domainObjects);
    }

    public void TransactionRollingBack (DomainObjectCollection domainObjects)
    {
      _extensions.RollingBack (domainObjects);
    }

    public void TransactionRolledBack (DomainObjectCollection domainObjects)
    {
      _extensions.RolledBack (domainObjects);
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
