using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Queries;
using System.ComponentModel;

namespace Rubicon.Data.DomainObjects
{
  /// <summary>
  /// A collection of <see cref="IClientTransactionExtension"/>s.
  /// </summary>
  [Serializable]
  public class ClientTransactionExtensionCollection : CommonCollection
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    /// <summary>
    /// Creates a new object.
    /// </summary>
    public ClientTransactionExtensionCollection ()
    {
    }

    // methods and properties

    /// <summary>
    /// Gets an <see cref="IClientTransactionExtension"/> by the extension name.
    /// </summary>
    /// <param name="extensionName">The name of the extension. Must not be <see langword="null"/> or <see cref="System.String.Empty"/>.</param>
    /// <returns>The <see cref="IClientTransactionExtension"/> of the given <paramref name="extensionName"/> or <see langword="null"/> if the name was not found.</returns>
    public IClientTransactionExtension this[string extensionName]
    {
      get 
      {
        ArgumentUtility.CheckNotNullOrEmpty ("extensionName", extensionName);

        return (IClientTransactionExtension) BaseGetObject (extensionName); 
      }
    }

    /// <summary>
    /// Gets the <see cref="IClientTransactionExtension"/> of a given <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The index of the extension to be retrieved.</param>
    /// <returns>The <see cref="IClientTransactionExtension"/> of the given <paramref name="index"/>.</returns>
    public IClientTransactionExtension this[int index]
    {
      get { return (IClientTransactionExtension) BaseGetObject (index); }
    }

    /// <summary>
    /// Adds an <see cref="IClientTransactionExtension"/> to the collection.
    /// </summary>
    /// <param name="extensionName">A name for the extension. Must not be <see langword="null"/> or <see cref="System.String.Empty"/>.</param>
    /// <param name="clientTransactionExtension">The extension to add. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.ArgumentException">An extension with the given <paramref name="extensionName"/> is already part of the collection.</exception>
    /// <remarks>The order of the extensions in the collection is the order in which they are notified.</remarks>
    public void Add (string extensionName, IClientTransactionExtension clientTransactionExtension)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("extensionName", extensionName);
      ArgumentUtility.CheckNotNull ("clientTransactionExtension", clientTransactionExtension);
      if (BaseContainsKey (extensionName)) 
        throw CreateArgumentException ("extensionName", "An extension with name '{0}' is already part of the collection.", extensionName);
      
      BaseAdd (extensionName, clientTransactionExtension);
    }

    /// <summary>
    /// Removes an <see cref="IClientTransactionExtension"/> from the collection.
    /// </summary>
    /// <param name="extensionName">The name of the extension. Must not be <see langword="null"/> or <see cref="System.String.Empty"/>.</param>
    public void Remove (string extensionName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("extensionName", extensionName);

      BaseRemove (extensionName);
    }

    /// <summary>
    /// Gets the index of an <see cref="IClientTransactionExtension"/> with a given <paramref name="extensionName"/>.
    /// </summary>
    /// <param name="extensionName">The name of the extension. Must not be <see langword="null"/> or <see cref="System.String.Empty"/>.</param>
    /// <returns>The index of the extension, or -1 if <paramref name="extensionName"/> is not found.</returns>
    public int IndexOf (string extensionName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("extensionName", extensionName);

      return BaseIndexOfKey (extensionName);
    }

    /// <summary>
    /// Inserts an <see cref="IClientTransactionExtension"/> intto the collection at a specified index.
    /// </summary>
    /// <param name="extensionName">A name for the extension. Must not be <see langword="null"/> or <see cref="System.String.Empty"/>.</param>
    /// <param name="clientTransactionExtension">The extension to insert. Must not be <see langword="null"/>.</param>
    /// <param name="index">The index where the extension should be inserted.</param>
    /// <exception cref="System.ArgumentException">An extension with the given <paramref name="extensionName"/> is already part of the collection.</exception>
    /// <remarks>The order of the extensions in the collection is the order in which they are notified.</remarks>
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
    public void ObjectsLoaded (DomainObjectCollection loadedDomainObjects)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("loadedDomainObjects", loadedDomainObjects);

      foreach (IClientTransactionExtension extension in this)
        extension.ObjectsLoaded (loadedDomainObjects);
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
      ArgumentUtility.CheckValidEnumValue ("valueAccess", valueAccess);

      foreach (IClientTransactionExtension extension in this)
        extension.PropertyValueReading (dataContainer, propertyValue, valueAccess);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void PropertyValueRead (DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);
      ArgumentUtility.CheckValidEnumValue ("valueAccess", valueAccess);

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
    public void RelationReading (DomainObject domainObject, string propertyName, ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckValidEnumValue ("valueAccess", valueAccess);

      foreach (IClientTransactionExtension extension in this)
        extension.RelationReading (domainObject, propertyName, valueAccess);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void RelationRead (DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckValidEnumValue ("valueAccess", valueAccess);

      foreach (IClientTransactionExtension extension in this)
        extension.RelationRead (domainObject, propertyName, relatedObject, valueAccess);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void RelationRead (DomainObject domainObject, string propertyName, DomainObjectCollection relatedObjects, ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckNotNull ("relatedObjects", relatedObjects);
      ArgumentUtility.CheckValidEnumValue ("valueAccess", valueAccess);

      foreach (IClientTransactionExtension extension in this)
        extension.RelationRead (domainObject, propertyName, relatedObjects, valueAccess);
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
    public void FilterQueryResult (DomainObjectCollection queryResult, IQuery query)
    {
      ArgumentUtility.CheckNotNull ("queryResult", query);
      ArgumentUtility.CheckNotNull ("queryResult", query);

      foreach (IClientTransactionExtension extension in this)
        extension.FilterQueryResult (queryResult, query);
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
