using System;

namespace Rubicon.Data.DomainObjects.Configuration.StorageProviders
{
public abstract class StorageProviderDefinition
{
  // types

  // static members and constants

  // member fields

  private string _storageProviderID;
  private Type _storageProviderType;

  // construction and disposing

  protected StorageProviderDefinition (string storageProviderID, Type storageProviderType)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);
    ArgumentUtility.CheckNotNull ("storageProviderType", storageProviderType);

    _storageProviderID = storageProviderID;
    _storageProviderType = storageProviderType;
  }

  // methods and properties

  public string StorageProviderID
  {
    get { return _storageProviderID; }
  }

  public Type StorageProviderType
  {
    get { return _storageProviderType; }
  }
}
}
