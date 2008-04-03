using System;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
[Obsolete("(Version (1.7.42)", true)]
public sealed class StorageProviderDefinitionCollection : CommonCollection
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  private StorageProviderDefinitionCollection ()
  {
  }

  // standard constructor for collections
  private StorageProviderDefinitionCollection (
      StorageProviderDefinitionCollection collection,
      bool makeCollectionReadOnly)
  {
    ArgumentUtility.CheckNotNull ("collection", collection);

    foreach (StorageProviderDefinition storageProviderDefinition in collection)  
    {
      Add (storageProviderDefinition);
    }

    this.SetIsReadOnly (makeCollectionReadOnly);
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

    return BaseContains (storageProviderDefinition.Name, storageProviderDefinition);
  }

  public bool Contains (string storageProviderID)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);
    return BaseContainsKey (storageProviderID);
  }

  public StorageProviderDefinition this [int index]  
  {
    get { return (StorageProviderDefinition) BaseGetObject (index); }
  }

  public StorageProviderDefinition this [string storageProviderID]  
  {
    get 
    {
      ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);
      return (StorageProviderDefinition) BaseGetObject (storageProviderID); 
    }
  }

  public int Add (StorageProviderDefinition value)  
  {
    ArgumentUtility.CheckNotNull ("value", value);

    if (Contains (value.Name))
      throw CreateArgumentException ("StorageProviderDefinition '{0}' already exists in collection.", "value", value.Name);

    return BaseAdd (value.Name, value);
  }

  #endregion
}
}
