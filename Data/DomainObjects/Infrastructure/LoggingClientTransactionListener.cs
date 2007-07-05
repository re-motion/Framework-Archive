using System;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Logging;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// A listener implementation logging all transaction events.
  /// </summary>
  [Serializable]
  public class LoggingClientTransactionListener : IClientTransactionListener
  {
    private static ILog s_log = LogManager.GetLogger (typeof (LoggingClientTransactionListener));

    public void NewObjectCreating (Type type)
    {
      s_log.Info ("NewObjectCreating");
    }

    public void ObjectLoading (ObjectID id)
    {
      s_log.Info ("ObjectLoading");
    }

    public void ObjectsLoaded (DomainObjectCollection domainObjects)
    {
      s_log.Info ("ObjectsLoaded");
    }

    public void ObjectDeleting (DomainObject domainObject)
    {
      s_log.Info ("ObjectDeleting");
    }

    public void ObjectDeleted (DomainObject domainObject)
    {
      s_log.Info ("ObjectDeleted");
    }

    public void PropertyValueReading (DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
      s_log.Info ("PropertyValueReading");
    }

    public void PropertyValueRead (DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
      s_log.Info ("PropertyValueRead");
    }

    public void PropertyValueChanging (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      s_log.Info ("PropertyValueChanging");
    }

    public void PropertyValueChanged (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      s_log.Info ("PropertyValueChanged");
    }

    public void RelationReading (DomainObject domainObject, string propertyName, ValueAccess valueAccess)
    {
      s_log.Info ("RelationReading");
    }

    public void RelationRead (DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess)
    {
      s_log.Info ("RelationRead");
    }

    public void RelationRead (DomainObject domainObject, string propertyName, DomainObjectCollection relatedObjects, ValueAccess valueAccess)
    {
      s_log.Info ("RelationRead");
    }

    public void RelationChanging (DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      s_log.Info ("RelationChanging");
    }

    public void RelationChanged (DomainObject domainObject, string propertyName)
    {
      s_log.Info ("RelationChanged");
    }

    public void FilterQueryResult (DomainObjectCollection queryResult, IQuery query)
    {
      s_log.Info ("FilterQueryResult");
    }

    public void TransactionCommitting (DomainObjectCollection domainObjects)
    {
      s_log.Info ("TransactionCommitting");
    }

    public void TransactionCommitted (DomainObjectCollection domainObjects)
    {
      s_log.Info ("TransactionCommitted");
    }

    public void TransactionRollingBack (DomainObjectCollection domainObjects)
    {
      s_log.Info ("TransactionRollingBack");
    }

    public void TransactionRolledBack (DomainObjectCollection domainObjects)
    {
      s_log.Info ("TransactionRolledBack");
    }

    public void RelationEndPointMapRegistering (RelationEndPoint endPoint)
    {
      s_log.Info ("RelationEndPointMapRegistering");
    }

    public void RelationEndPointMapUnregistering (RelationEndPointID endPointID)
    {
      s_log.Info ("RelationEndPointMapUnregistering");
    }

    public void RelationEndPointMapPerformingDelete (RelationEndPointID[] endPointIDs)
    {
      s_log.Info ("RelationEndPointMapPerformingDelete");
    }

    public void DataManagerMarkingObjectDiscarded (ObjectID id)
    {
      s_log.Info ("DataManagerMarkingObjectDiscarded");
    }

    public void DataContainerMapRegistering (DataContainer container)
    {
      s_log.Info ("DataContainerMapRegistering");
    }

    public void DataContainerMapUnregistering (DataContainer container)
    {
      s_log.Info ("DataContainerMapUnregistering");
    }
  }
}