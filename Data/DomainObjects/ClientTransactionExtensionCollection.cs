using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Queries;
using System.ComponentModel;

namespace Rubicon.Data.DomainObjects
{
  //TODO: Doc
  [Serializable]
  public class ClientTransactionExtensionCollection : CommonCollection
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public ClientTransactionExtensionCollection ()
    {
    }

    // methods and properties

    public IClientTransactionExtension this[string extensionName]
    {
      get 
      {
        ArgumentUtility.CheckNotNullOrEmpty ("extensionName", extensionName);

        return (IClientTransactionExtension) BaseGetObject (extensionName); 
      }
    }

    public IClientTransactionExtension this[int index]
    {
      get { return (IClientTransactionExtension) BaseGetObject (index); }
    }

    public void Add (string extensionName, IClientTransactionExtension clientTransactionExtension)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("extensionName", extensionName);
      ArgumentUtility.CheckNotNull ("clientTransactionExtension", clientTransactionExtension);
      if (BaseContainsKey (extensionName)) 
        throw CreateArgumentException ("extensionName", "An extension with name '{0}' is already part of the collection.", extensionName);
      
      BaseAdd (extensionName, clientTransactionExtension);
    }

    public void Remove (string extensionName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("extensionName", extensionName);

      BaseRemove (extensionName);
    }

    public int IndexOf (string extensionName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("extensionName", extensionName);

      return BaseIndexOfKey (extensionName);
    }

    public void Insert (int index, string extensionName, IClientTransactionExtension clientTransactionExtension)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("extensionName", extensionName);
      ArgumentUtility.CheckNotNull ("clientTransactionExtension", clientTransactionExtension);
      if (BaseContainsKey (extensionName))
        throw CreateArgumentException ("extensionName", "An extension with name '{0}' is already part of the collection.", extensionName);

      BaseInsert (index, extensionName, clientTransactionExtension);
    }

    private ArgumentException CreateArgumentException (string parameterName, string message, params object[] args)
    {
      return new ArgumentException (string.Format (message, args), parameterName);
    }

    #region Notification methods

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void NewObjectCreating (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      foreach (IClientTransactionExtension extension in this)
        extension.NewObjectCreating (type);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void NewObjectCreated (DomainObject newDomainObject)
    {
      ArgumentUtility.CheckNotNull ("newDomainObject", newDomainObject);

      foreach (IClientTransactionExtension extension in this)
        extension.NewObjectCreated (newDomainObject);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void ObjectsLoaded (DomainObjectCollection loadedDomainObjects)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("loadedDomainObjects", loadedDomainObjects);

      foreach (IClientTransactionExtension extension in this)
        extension.ObjectsLoaded (loadedDomainObjects);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void FilterQueryResult (DomainObjectCollection queryResult, IQuery query)
    {
      ArgumentUtility.CheckNotNull ("queryResult", query);
      ArgumentUtility.CheckNotNull ("queryResult", query);

      foreach (IClientTransactionExtension extension in this)
        extension.FilterQueryResult (queryResult, query);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void ObjectDeleting (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      foreach (IClientTransactionExtension extension in this)
        extension.ObjectDeleting (domainObject);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void ObjectDeleted (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      foreach (IClientTransactionExtension extension in this)
        extension.ObjectDeleted (domainObject);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void PropertyValueReading (DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

      foreach (IClientTransactionExtension extension in this)
        extension.PropertyValueReading (dataContainer, propertyValue, valueAccess);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void PropertyValueRead (DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

      foreach (IClientTransactionExtension extension in this)
        extension.PropertyValueRead (dataContainer, propertyValue, value, valueAccess);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void PropertyValueChanging (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

      foreach (IClientTransactionExtension extension in this)
        extension.PropertyValueChanging (dataContainer, propertyValue, oldValue, newValue);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void PropertyValueChanged (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

      foreach (IClientTransactionExtension extension in this)
        extension.PropertyValueChanged (dataContainer, propertyValue, oldValue, newValue);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void RelationChanging (DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      foreach (IClientTransactionExtension extension in this)
        extension.RelationChanging (domainObject, propertyName, oldRelatedObject, newRelatedObject);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void RelationChanged (DomainObject domainObject, string propertyName)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      foreach (IClientTransactionExtension extension in this)
        extension.RelationChanged (domainObject, propertyName);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void Committing (DomainObjectCollection changedDomainObjects)
    {
      ArgumentUtility.CheckNotNull ("changedDomainObjects", changedDomainObjects);

      foreach (IClientTransactionExtension extension in this)
        extension.Committing (changedDomainObjects);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void Committed (DomainObjectCollection changedDomainObjects)
    {
      ArgumentUtility.CheckNotNull ("changedDomainObjects", changedDomainObjects);

      foreach (IClientTransactionExtension extension in this)
        extension.Committed (changedDomainObjects);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void RollingBack ()
    {
      foreach (IClientTransactionExtension extension in this)
        extension.RollingBack ();
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void RolledBack ()
    {
      foreach (IClientTransactionExtension extension in this)
        extension.RolledBack ();
    }

    #endregion
  }
}
