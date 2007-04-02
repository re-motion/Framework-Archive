using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [StorageProviderStub]
  public class StorageProviderStubDomainBase : DomainObject
  {
    // types

    // static members and constants

    public static new StorageProviderStubDomainBase GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (StorageProviderStubDomainBase) DomainObject.GetObject (id, clientTransaction);
    }

    public static new StorageProviderStubDomainBase GetObject (ObjectID id, bool includeDeleted)
    {
      return (StorageProviderStubDomainBase) DomainObject.GetObject (id, includeDeleted);
    }

    public static new StorageProviderStubDomainBase GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (StorageProviderStubDomainBase) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    // construction and disposing

    // TODO: [Obsolete ("Use ctor(ClientTransaction, objectID) instead.")]
    protected StorageProviderStubDomainBase ()
    {
    }

    // TODO: [Obsolete ("Use ctor(ClientTransaction, objectID) instead.")]
    protected StorageProviderStubDomainBase (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected StorageProviderStubDomainBase (ClientTransaction clientTransaction, ObjectID objectID)
      : base (clientTransaction, objectID)
    {
    }

    // TODO: [Obsolete("Use ctor(ClientTransaction, objectID) instead.")]
    protected StorageProviderStubDomainBase (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    [StorageClassNone]
    public new DataContainer DataContainer
    {
      get { return base.DataContainer; }
    }

    public new DomainObject GetRelatedObject (string propertyName)
    {
      return base.GetRelatedObject (propertyName);
    }

    public new DomainObjectCollection GetRelatedObjects (string propertyName)
    {
      return base.GetRelatedObjects (propertyName);
    }

    public new DomainObject GetOriginalRelatedObject (string propertyName)
    {
      return base.GetOriginalRelatedObject (propertyName);
    }

    public new DomainObjectCollection GetOriginalRelatedObjects (string propertyName)
    {
      return base.GetOriginalRelatedObjects (propertyName);
    }

    public new void SetRelatedObject (string propertyName, DomainObject newRelatedObject)
    {
      base.SetRelatedObject (propertyName, newRelatedObject);
    }
  }
}
