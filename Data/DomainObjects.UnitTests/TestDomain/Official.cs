using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [StorageProviderStub]
  [Instantiable]
  public abstract class Official : StorageProviderStubDomainBase
  {
    public static new Official GetObject (ObjectID id)
    {
      return (Official) DomainObject.GetObject (id);
    }

    public static Official NewObject ()
    {
      return NewObject<Official> ().With();
    }

    protected Official()
    {
    }

    protected Official (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("Official")]
    public abstract ObjectList<Order> Orders { get; }
  }
}
