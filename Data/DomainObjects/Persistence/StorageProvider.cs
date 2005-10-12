using System;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.Queries.Configuration;
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
  public abstract DataContainerCollection ExecuteCollectionQuery (IQuery query);
  public abstract object ExecuteScalarQuery (IQuery query);

  // methods and properties

  public string ID
  {
    get 
    {
      CheckDisposed ();
      return _storageProviderDefinition.StorageProviderID; 
    }
  }

  protected virtual void CheckQuery (IQuery query, QueryType expectedQueryType, string argumentName)
  {
    CheckDisposed ();
    ArgumentUtility.CheckNotNull ("query", query);

    if (query.StorageProviderID != ID)
    {
      throw CreateArgumentException (
          "query", 
          "The StorageProviderID '{0}' of the provided query '{1}' does not match with this StorageProvider's ID '{2}'.",
          query.StorageProviderID, 
          query.QueryID,
          ID);
    }

    if (query.QueryType != expectedQueryType)
      throw CreateArgumentException (argumentName, "Expected query type is '{0}', but was '{1}'.", expectedQueryType, query.QueryType);
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

  protected ArgumentException CreateArgumentException (string argumentName, string formatString, params object[] args)
  {
    return new ArgumentException (string.Format (formatString, args), argumentName);
  }
}
}
