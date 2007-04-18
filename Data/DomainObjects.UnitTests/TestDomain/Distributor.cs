using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [NotAbstract]
  public abstract class Distributor : Partner
  {
    public static new Distributor GetObject (ObjectID id)
    {
      return (Distributor) DomainObject.GetObject (id);
    }

    public static Distributor Create ()
    {
      return DomainObject.Create<Distributor> ();
    }

    protected Distributor (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    public abstract int NumberOfShops { get; set; }

    [DBBidirectionalRelation ("Distributor")]
    private ClassWithoutRelatedClassIDColumn ClassWithoutRelatedClassIDColumn
    {
      get { return (ClassWithoutRelatedClassIDColumn) GetRelatedObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Distributor.ClassWithoutRelatedClassIDColumn"); }
      set { SetRelatedObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Distributor.ClassWithoutRelatedClassIDColumn", value); }
    }
  }
}
