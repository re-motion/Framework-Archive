using System;
using System.Collections;
using System.Collections.Specialized;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence.Configuration
{
public class StorageProviderDefinitionCollection : CollectionBase
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public StorageProviderDefinitionCollection ()
  {
  }

  // standard constructor for collections
  public StorageProviderDefinitionCollection (
      StorageProviderDefinitionCollection collection,
      bool isCollectionReadOnly)
  {
    ArgumentUtility.CheckNotNull ("collection", collection);

    foreach (StorageProviderDefinition storageProviderDefinition in collection)  
    {
      Add (storageProviderDefinition);
    }

    this.SetIsReadOnly (isCollectionReadOnly);
  }

  // methods and properties

  public StorageProviderDefinition GetMandatory (string storageProviderID)
  {
    if (!Contains (storageProviderID))
      throw CreateStorageProviderConfigurationException ("StorageProviderDefinition '{0}' does not exist.", storageProviderID);

    return this[storageProviderID];
  }

  private ArgumentException CreateArgumentException (string message, string parameterName, params object[] args)
  {
    return new ArgumentException (string.Format (message, args), parameterName);
  }

  private StorageProviderConfigurationException CreateStorageProviderConfigurationException (
      string message, 
      params object[] args)
  {
    return new StorageProviderConfigurationException (string.Format (message, args));
  }

  #region Standard implementation for "add-only" collections

  public bool Contains (StorageProviderDefinition storageProviderDefinition)
  {
    ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

    return Contains (storageProviderDefinition.StorageProviderID);
  }

  public bool Contains (string storageProviderID)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);
    return base.ContainsKey (storageProviderID);
  }

  public StorageProviderDefinition this [int index]  
  {
    get { return (StorageProviderDefinition) GetObject (index); }
  }

  public StorageProviderDefinition this [string storageProviderID]  
  {
    get 
    {
      ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);
      return (StorageProviderDefinition) GetObject (storageProviderID); 
    }
  }

  public void Add (StorageProviderDefinition value)  
  {
    ArgumentUtility.CheckNotNull ("value", value);

    if (Contains (value.StorageProviderID))
    {
      throw CreateArgumentException (
          "StorageProviderDefinition '{0}' already exists in collection.", "value", value.StorageProviderID);
    }

    base.Add (value.StorageProviderID, value);
  }

  #endregion
}
}
