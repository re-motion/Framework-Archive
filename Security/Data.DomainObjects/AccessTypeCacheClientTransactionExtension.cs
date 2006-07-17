using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Data.DomainObjects;
using Rubicon.Collections;

namespace Rubicon.Security.Data.DomainObjects
{
  public class AccessTypeCacheClientTransactionExtension : IClientTransactionExtension
  {
    // types

    // static members

    // member fields

    private Cache<Tupel<SecurityContext, string>, AccessType[]> _cache = new Cache<Tupel<SecurityContext, string>, AccessType[]> ();

    // construction and disposing

    public AccessTypeCacheClientTransactionExtension ()
    {
    }

    // methods and properties

    public Cache<Tupel<SecurityContext, string>, AccessType[]> Cache
    {
      get { return _cache; }
    }

    void IClientTransactionExtension.NewObjectCreating (Type type)
    {
    }

    void IClientTransactionExtension.ObjectsLoaded (DomainObjectCollection loadedDomainObjects)
    {
    }

    void IClientTransactionExtension.ObjectDeleting (DomainObject domainObject)
    {
    }

    void IClientTransactionExtension.ObjectDeleted (DomainObject domainObject)
    {
    }

    void IClientTransactionExtension.PropertyValueReading (DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
    }

    void IClientTransactionExtension.PropertyValueRead (DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
    }

    void IClientTransactionExtension.PropertyValueChanging (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
    }

    void IClientTransactionExtension.PropertyValueChanged (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
    }

    void IClientTransactionExtension.RelationReading (DomainObject domainObject, string propertyName, ValueAccess valueAccess)
    {
    }

    void IClientTransactionExtension.RelationRead (DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess)
    {
    }

    void IClientTransactionExtension.RelationRead (DomainObject domainObject, string propertyName, DomainObjectCollection relatedObjects, ValueAccess valueAccess)
    {
    }

    void IClientTransactionExtension.RelationChanging (DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
    }

    void IClientTransactionExtension.RelationChanged (DomainObject domainObject, string propertyName)
    {
    }

    void IClientTransactionExtension.FilterQueryResult (DomainObjectCollection queryResult, Rubicon.Data.DomainObjects.Queries.IQuery query)
    {
    }

    void IClientTransactionExtension.Committing (DomainObjectCollection changedDomainObjects)
    {
    }

    void IClientTransactionExtension.Committed (DomainObjectCollection changedDomainObjects)
    {
    }

    void IClientTransactionExtension.RollingBack (DomainObjectCollection changedDomainObjects)
    {
    }

    void IClientTransactionExtension.RolledBack (DomainObjectCollection changedDomainObjects)
    {
    }
  }
}