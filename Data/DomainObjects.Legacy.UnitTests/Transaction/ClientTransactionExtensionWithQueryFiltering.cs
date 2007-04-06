using System;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.Transaction
{
  [Serializable]
  public class ClientTransactionExtensionWithQueryFiltering : IClientTransactionExtension
  {
    public virtual void NewObjectCreating (Type type)
    {
    }

    public virtual void NewObjectCreated (DomainObject newDomainObject)
    {
    }

    public virtual void ObjectsLoaded (DomainObjectCollection loadedDomainObjects)
    {
    }

    public virtual void ObjectDeleting (DomainObject domainObject)
    {
    }

    public virtual void ObjectDeleted (DomainObject domainObject)
    {
    }

    public virtual void PropertyValueReading (DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
    }

    public virtual void PropertyValueRead (DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
    }

    public virtual void PropertyValueChanging (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
    }

    public virtual void PropertyValueChanged (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
    }

    public virtual void RelationReading (DomainObject domainObject, string propertyName, ValueAccess valueAccess)
    {
    }

    public virtual void RelationRead (DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess)
    {
    }

    public virtual void RelationRead (DomainObject domainObject, string propertyName, DomainObjectCollection relatedObjects, ValueAccess valueAccess)
    {
    }

    public virtual void RelationChanging (DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
    }

    public virtual void RelationChanged (DomainObject domainObject, string propertyName)
    {
    }

    public virtual void FilterQueryResult (DomainObjectCollection queryResult, Rubicon.Data.DomainObjects.Queries.IQuery query)
    {
      if (queryResult.Count >0)
        queryResult.Remove (queryResult[0]);
    }

    public virtual void Committing (DomainObjectCollection changedDomainObjects)
    {
    }

    public virtual void Committed (DomainObjectCollection changedDomainObjects)
    {
    }

    public virtual void RollingBack (DomainObjectCollection changedDomainObjects)
    {
    }

    public virtual void RolledBack (DomainObjectCollection changedDomainObjects)
    {
    }
  }
}
