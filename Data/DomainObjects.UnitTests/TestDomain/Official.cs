using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [StorageProviderStub]
  [NotAbstract]
  public abstract class Official : StorageProviderStubDomainBase
  {
    public static new Official GetObject (ObjectID id)
    {
      return (Official) DomainObject.GetObject (id);
    }

    public static Official Create ()
    {
      return DomainObject.Create<Official> ();
    }

    protected Official (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("Official")]
    public virtual ObjectList<Order> Orders { get { return (ObjectList<Order>) GetRelatedObjects(); } }
  }
}
