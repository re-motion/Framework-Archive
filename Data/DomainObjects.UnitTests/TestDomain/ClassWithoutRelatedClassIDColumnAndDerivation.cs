using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable (Name = "TableWithoutRelatedClassIDColumnAndDerivation")]
  [TestDomain]
  [NotAbstract]
  public abstract class ClassWithoutRelatedClassIDColumnAndDerivation : TestDomainBase
  {
    public static ClassWithoutRelatedClassIDColumnAndDerivation Create ()
    {
      return DomainObject.Create<ClassWithoutRelatedClassIDColumnAndDerivation> ();
    }

    protected ClassWithoutRelatedClassIDColumnAndDerivation (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    [DBBidirectionalRelation ("ClassWithoutRelatedClassIDColumnAndDerivation", ContainsForeignKey = true)]
    public abstract Company Company { get; set; }
  }
}
