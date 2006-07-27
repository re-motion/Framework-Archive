using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Data.DomainObjects;
using Rubicon.Collections;
using Rubicon.Data.DomainObjects.Queries;

namespace Rubicon.Security.Data.DomainObjects
{
  public class SecurityClientTransactionExtension : IClientTransactionExtension
  {
    // types

    // static members

    // member fields

    private bool _isActive;
    private SecurityClient _securityClient;

    // construction and disposing

    public SecurityClientTransactionExtension ()
    {
    }

    // methods and properties

    #region IClientTransactionExtension Implementation

    void IClientTransactionExtension.ObjectsLoaded (DomainObjectCollection loadedDomainObjects)
    {
    }

    void IClientTransactionExtension.ObjectDeleted (DomainObject domainObject)
    {
    }

    void IClientTransactionExtension.PropertyValueRead (DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
    }

    void IClientTransactionExtension.PropertyValueChanged (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
    }

    void IClientTransactionExtension.RelationRead (DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess)
    {
    }

    void IClientTransactionExtension.RelationRead (DomainObject domainObject, string propertyName, DomainObjectCollection relatedObjects, ValueAccess valueAccess)
    {
    }

    void IClientTransactionExtension.RelationChanged (DomainObject domainObject, string propertyName)
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

    #endregion

    public virtual void FilterQueryResult (DomainObjectCollection queryResult, IQuery query)
    {
      ArgumentUtility.CheckNotNull ("queryResult", queryResult);

      if (_isActive)
        return;

      if (SecurityFreeSection.IsActive)
        return;

      SecurityClient securityClient = GetSecurityClient ();

      for (int i = queryResult.Count - 1; i >= 0; i--)
      {
        ISecurableObject securableObject = queryResult[i] as ISecurableObject;
        if (securableObject == null)
          continue;

        bool hasAccess;
        try
        {
          _isActive = true;
          hasAccess = securityClient.HasAccess (securableObject, AccessType.Get (GeneralAccessType.Find));
        }
        finally
        {
          _isActive = false;
        }
        if (!hasAccess)
          queryResult.RemoveAt (i);
      }
    }

    public virtual void NewObjectCreating (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      if (_isActive)
        return;

      if (!(typeof (ISecurableObject).IsAssignableFrom (type)))
        return;

      if (SecurityFreeSection.IsActive)
        return;

      SecurityClient securityClient = GetSecurityClient ();
      try
      {
        _isActive = true;
        securityClient.CheckConstructorAccess (type);
      }
      finally
      {
        _isActive = false;
      }
    }

    public virtual void ObjectDeleting (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      if (_isActive)
        return;

      if (SecurityFreeSection.IsActive)
        return;

      if (domainObject.State == StateType.New)
        return;

      ISecurableObject securableObject = domainObject as ISecurableObject;
      if (securableObject == null)
        return;

      SecurityClient securityClient = GetSecurityClient ();
      try
      {
        _isActive = true;
        securityClient.CheckAccess (securableObject, AccessType.Get (GeneralAccessType.Delete));
      }
      finally
      {
        _isActive = false;
      }
    }

    public virtual void PropertyValueReading (DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

      PropertyReading (dataContainer.DomainObject, propertyValue.Name);
    }

    public virtual void RelationReading (DomainObject domainObject, string propertyName, ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      PropertyReading (domainObject, propertyName);
    }

    private void PropertyReading (DomainObject domainObject, string propertyName)
    {
      if (_isActive)
        return;

      if (SecurityFreeSection.IsActive)
        return;

      ISecurableObject securableObject = domainObject as ISecurableObject;
      if (securableObject == null)
        return;

      SecurityClient securityClient = GetSecurityClient ();
      try
      {
        _isActive = true;
        securityClient.CheckPropertyReadAccess (securableObject, propertyName);
      }
      finally
      {
        _isActive = false;
      }
    }

    public virtual void PropertyValueChanging (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

      PropertyChanging (dataContainer.DomainObject, propertyValue.Name);
    }

    public virtual void RelationChanging (DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNull ("propertyName", propertyName);

      PropertyChanging (domainObject, propertyName);
    }

    private void PropertyChanging (DomainObject domainObject, string propertyName)
    {
      if (_isActive)
        return;

      if (SecurityFreeSection.IsActive)
        return;

      ISecurableObject securableObject = domainObject as ISecurableObject;
      if (securableObject == null)
        return;

      SecurityClient securityClient = GetSecurityClient ();
      try
      {
        _isActive = true;
        securityClient.CheckPropertyWriteAccess (securableObject, propertyName);
      }
      finally
      {
        _isActive = false;
      }
    }

    private SecurityClient GetSecurityClient ()
    {
      if (_securityClient == null)
        _securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      return _securityClient;
    }
  }
}