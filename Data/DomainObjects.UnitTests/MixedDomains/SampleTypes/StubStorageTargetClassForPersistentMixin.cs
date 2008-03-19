using System;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Mixins;

namespace Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes
{
  [Uses (typeof (StubStoragePersistentMixin))]
  [Uses (typeof (NullMixin))]
  [DBTable]
  [StorageProviderStub]
  public class StubStorageTargetClassForPersistentMixin : DomainObject
  {
    public static StubStorageTargetClassForPersistentMixin NewObject ()
    {
      return DomainObject.NewObject<StubStorageTargetClassForPersistentMixin> ().With ();
    }

    public static StubStorageTargetClassForPersistentMixin GetObject (ObjectID id)
    {
      return DomainObject.GetObject<StubStorageTargetClassForPersistentMixin> (id);
    }
  }
}