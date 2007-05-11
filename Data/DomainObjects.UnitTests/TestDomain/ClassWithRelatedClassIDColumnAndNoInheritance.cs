using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable ("TableWithRelatedClassIDColumnAndNoInheritance")]
  [TestDomain]
  [Instantiable]
  public abstract class ClassWithRelatedClassIDColumnAndNoInheritance : TestDomainBase
  {
    public static ClassWithRelatedClassIDColumnAndNoInheritance NewObject ()
    {
      return NewObject<ClassWithRelatedClassIDColumnAndNoInheritance> ().With();
    }

    protected ClassWithRelatedClassIDColumnAndNoInheritance()
    {
    }

    protected ClassWithRelatedClassIDColumnAndNoInheritance (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    [DBBidirectionalRelation ("ClassWithRelatedClassIDColumnAndNoInheritance", ContainsForeignKey = true)]
    [DBColumn ("TableWithGuidKeyID")]
    public abstract ClassWithGuidKey ClassWithGuidKey { get; set; }
  }
}
