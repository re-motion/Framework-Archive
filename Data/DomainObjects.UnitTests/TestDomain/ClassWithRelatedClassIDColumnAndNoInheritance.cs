using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable (Name = "TableWithRelatedClassIDColumnAndNoInheritance")]
  [TestDomain]
  [NotAbstract]
  public abstract class ClassWithRelatedClassIDColumnAndNoInheritance : TestDomainBase
  {
    public static ClassWithRelatedClassIDColumnAndNoInheritance Create ()
    {
      return DomainObject.Create<ClassWithRelatedClassIDColumnAndNoInheritance> ();
    }

    protected ClassWithRelatedClassIDColumnAndNoInheritance (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    [DBBidirectionalRelation ("ClassWithRelatedClassIDColumnAndNoInheritance", ContainsForeignKey = true)]
    [DBColumn ("TableWithGuidKeyID")]
    public abstract ClassWithGuidKey ClassWithGuidKey { get; set; }
  }
}
