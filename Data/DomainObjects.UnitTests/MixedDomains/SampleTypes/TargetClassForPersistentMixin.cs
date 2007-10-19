using System;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Mixins;

namespace Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes
{
  [Uses (typeof (MixinAddingPersistentProperties))]
  [Uses (typeof (NullMixin))]
  [DBTable]
  [StorageProviderStub]
  public class TargetClassForPersistentMixin : DomainObject
  {
    public static TargetClassForPersistentMixin NewObject ()
    {
      return DomainObject.NewObject<TargetClassForPersistentMixin> ().With ();
    }

    public static new TargetClassForPersistentMixin GetObject (ObjectID id)
    {
      return DomainObject.GetObject<TargetClassForPersistentMixin> (id);
    }
  }
}