using System;

namespace Rubicon.Data.DomainObjects
{
  public interface IClientTransactionExtension
  {
    void PropertyReading (DataContainer dataContainer, PropertyValue propertyValue, RetrievalType retrievalType);
    void PropertyRead (DataContainer dataContainer, PropertyValue propertyValue, object value, RetrievalType retrievalType);

    void PropertyChanging (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue);
    void PropertyChanged (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue);
    
    void RelationChanging (DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject);
    void RelationChanged (DomainObject domainObject, string propertyName);

    void NewObjectCreating (Type type);
    void NewObjectCreated (DomainObject domainObject);

    void ObjectDeleting (DomainObject domainObject);
    //TODO Doc: Object can be discarded already!!!
    void ObjectDeleted (DomainObject domainObject);
  }
}
