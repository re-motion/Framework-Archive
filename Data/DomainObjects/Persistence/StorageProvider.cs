using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.Configuration.StorageProviders;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence
{
public abstract class StorageProvider : IDisposable
{
  // types

  // static members and constants

  // member fields

  private StorageProviderDefinition _storageProviderDefinition;
  private bool _disposed = false;

  // construction and disposing

  public StorageProvider (StorageProviderDefinition storageProviderDefinition)
  {
    ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
    _storageProviderDefinition = storageProviderDefinition;
  }

  ~StorageProvider ()
  {
    Dispose (false);
  }

  public void Dispose ()
  {
    Dispose (true);
    GC.SuppressFinalize (this);
  }

  protected virtual void Dispose (bool disposing)
  {
    if (disposing)
      _storageProviderDefinition = null;

    _disposed = true;
  }

  // abstract methods and properties

  public abstract DataContainer LoadDataContainer (ObjectID id);

  public abstract DataContainerCollection LoadDataContainersByRelatedID (
      ClassDefinition classDefinition,
      string propertyName,
      ObjectID relatedID);

  public abstract void Save (DataContainerCollection dataContainers);
  public abstract void SetTimestamp (DataContainerCollection dataContainers);
  public abstract void BeginTransaction ();
  public abstract void Commit ();
  public abstract void Rollback ();
  public abstract DataContainer CreateNewDataContainer (ClassDefinition classDefinition);

  // methods and properties

  public string ID
  {
    get 
    {
      CheckDisposed ();
      return _storageProviderDefinition.StorageProviderID; 
    }
  }

  protected StorageProviderDefinition StorageProviderDefinition
  {
    get 
    {
      CheckDisposed ();
      return _storageProviderDefinition; 
    }
  }

  protected bool IsDisposed 
  {
    get { return _disposed; }
  }

  protected void CheckDisposed ()
  {
    if (_disposed)
      throw new ObjectDisposedException ("StorageProvider", "A disposed StorageProvider cannot be accessed.");
  }
}
}
