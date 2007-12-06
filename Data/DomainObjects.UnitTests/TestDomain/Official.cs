using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [StorageProviderStub]
  [Instantiable]
  public abstract class Official : StorageProviderStubDomainBase
  {
    public static Official NewObject ()
    {
      return NewObject<Official> ().With();
    }

    public static Official GetObject (ObjectID id)
    {
      return DomainObject.GetObject<Official> (id);
    }

    protected Official()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("Official")]
    public abstract ObjectList<Order> Orders { get; }
  }
}
