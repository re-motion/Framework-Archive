using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Queries;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Defines an interface for objects listening for events occuring in the scope of a ClientTransaction.
  /// </summary>
  /// <remarks>
  /// This is similar to <see cref="IClientTransactionExtension"/>, but where <see cref="IClientTransactionExtension"/> is for the public,
  /// <see cref="IClientTransactionListener"/> is for internal usage (and therefore provides more events).
  /// </remarks>
  public interface IClientTransactionListener
  {
    void SubTransactionCreating (ClientTransaction subTransaction);

    void NewObjectCreating (Type type);
    
    void ObjectLoading (ObjectID id);
    void ObjectsLoaded (DomainObjectCollection domainObjects);
    
    void ObjectDeleting (DomainObject domainObject);
    void ObjectDeleted (DomainObject domainObject);

    void PropertyValueReading (DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess);
    void PropertyValueRead (DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess);
    void PropertyValueChanging (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue);
    void PropertyValueChanged (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue);

    void RelationReading (DomainObject domainObject, string propertyName, ValueAccess valueAccess);
    void RelationRead (DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess);
    void RelationRead (DomainObject domainObject, string propertyName, DomainObjectCollection relatedObjects, ValueAccess valueAccess);
    
    void RelationChanging (DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject);
    void RelationChanged (DomainObject domainObject, string propertyName);

    void FilterQueryResult (DomainObjectCollection queryResult, IQuery query);

    void TransactionCommitting (DomainObjectCollection domainObjects);
    void TransactionCommitted (DomainObjectCollection domainObjects);
    void TransactionRollingBack (DomainObjectCollection domainObjects);
    void TransactionRolledBack (DomainObjectCollection domainObjects);

    void RelationEndPointMapRegistering (RelationEndPoint endPoint);
    void RelationEndPointMapUnregistering (RelationEndPointID endPointID);
    void RelationEndPointMapPerformingDelete (RelationEndPointID[] endPointIDs);
    void RelationEndPointMapCopyingFrom (RelationEndPointMap source);
    void RelationEndPointMapCopyingTo (RelationEndPointMap source);

    void DataManagerMarkingObjectDiscarded (ObjectID id);
    void DataManagerCopyingFrom (DataManager source);
    void DataManagerCopyingTo (DataManager destination);

    void DataContainerMapRegistering (DataContainer container);
    void DataContainerMapUnregistering (DataContainer container);
    void DataContainerMapCopyingFrom (DataContainerMap source);
    void DataContainerMapCopyingTo (DataContainerMap destination);
  }
}
