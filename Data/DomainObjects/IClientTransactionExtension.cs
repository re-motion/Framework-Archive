using System;
using Rubicon.Data.DomainObjects.Queries;

namespace Rubicon.Data.DomainObjects
{
  public interface IClientTransactionExtension
  {
    void NewObjectCreating (Type type);
    void NewObjectCreated (DomainObject newDomainObject);

    void ObjectsLoaded (DomainObjectCollection loadedDomainObjects);

    void ObjectDeleting (DomainObject domainObject);
    //TODO Doc: Object can be discarded already!!!
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

    //TODO Doc: Is raised after the committing events on the DomainObjects, but before the committing event on the CTx
    void Committing (DomainObjectCollection changedDomainObjects);
    //TODO Doc: Is raised after the committing event on the CTx
    void Committed (DomainObjectCollection changedDomainObjects);

    void RollingBack ();
    void RolledBack ();

    void FilterQueryResult (DomainObjectCollection queryResult, IQuery query);
  }
}
