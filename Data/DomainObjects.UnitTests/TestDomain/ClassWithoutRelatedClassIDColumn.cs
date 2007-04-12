using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable (Name = "TableWithoutRelatedClassIDColumn")]
  [TestDomain]
  [NotAbstract]
  public abstract class ClassWithoutRelatedClassIDColumn : TestDomainBase
  {
    public static ClassWithoutRelatedClassIDColumn Create ()
    {
      return DomainObject.Create<ClassWithoutRelatedClassIDColumn> ();
    }

    protected ClassWithoutRelatedClassIDColumn (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    [DBBidirectionalRelation ("ClassWithoutRelatedClassIDColumn", ContainsForeignKey = true)]
    public abstract Distributor Distributor { get; set; }
  }
}