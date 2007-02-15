using System;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence.Configuration
{
public abstract class StorageProviderDefinition
{
  // types

  // static members and constants

  // member fields

  private string _id;
  private Type _storageProviderType;

  // construction and disposing

  protected StorageProviderDefinition (string storageProviderID, Type storageProviderType)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);
    ArgumentUtility.CheckNotNull ("storageProviderType", storageProviderType);

    _id = storageProviderID;
    _storageProviderType = storageProviderType;
  }

  // abstract methods and properties

  public abstract bool IsIdentityTypeSupported (Type identityType);

  // methods and properties

  public void CheckIdentityType (Type identityType)
  {
    if (!IsIdentityTypeSupported (identityType))
      throw new IdentityTypeNotSupportedException (_storageProviderType, identityType);
  }

  public string ID
  {
    get { return _id; }
  }

  // TODO: Remove this property after 1.6.2006.
  [Obsolete ("Use property ID instead.", true)]
  public string StorageProviderID
  {
    get { return _id; }
  }

  public Type StorageProviderType
  {
    get { return _storageProviderType; }
  }
}
}
