using System;

using Rubicon.Data.DomainObjects.Configuration.StorageProviders;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence
{
public class StorageProviderManager : IDisposable
{
  // types

  // static members and constants

  // member fields

  private bool _disposed = false;
  private StorageProviderCollection _storageProviders;

  // construction and disposing

  public StorageProviderManager ()
  {
    _storageProviders = new StorageProviderCollection ();
  }

  #region IDisposable Members

  public void Dispose ()
  {
    if (!_disposed)
    {
      if (_storageProviders != null)
        _storageProviders.Dispose ();

      _storageProviders = null;
      
      _disposed = true;
      GC.SuppressFinalize (this);
    }
  }

  #endregion

  // methods and properties

  public StorageProvider GetMandatoryStorageProvider (string storageProviderID)
  {
    CheckDisposed ();
    ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);

    StorageProvider provider = this[storageProviderID];
    if (provider == null)
    {
      throw CreatePersistenceException (
        "Storage Provider with ID '{0}' could not be created.", storageProviderID);
    }

    return provider;
  }

  public StorageProvider this [string storageProviderID]
  {
    get 
    {
      CheckDisposed ();
      ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);

      if (_storageProviders.Contains (storageProviderID))
        return _storageProviders[storageProviderID];

      StorageProviderDefinition providerDefinition = StorageProviderConfiguration.Current[storageProviderID];

      if (providerDefinition == null)
        return null;

      StorageProvider provider = (StorageProvider) ReflectionUtility.CreateObject (
          providerDefinition.StorageProviderType, providerDefinition);

      _storageProviders.Add (provider);

      return provider;
    }
  }

  public StorageProviderCollection StorageProviders
  {
    get 
    { 
      CheckDisposed ();
      return _storageProviders; 
    }
  }

  private PersistenceException CreatePersistenceException (string message, params object[] args)
  {
    return new PersistenceException (string.Format (message, args));
  }

  private void CheckDisposed ()
  {
    if (_disposed)
      throw new ObjectDisposedException ("StorageProviderManager", "A disposed StorageProviderManager cannot be accessed.");
  }
}
}
