using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable ("TableWithoutRelatedClassIDColumnAndDerivation")]
  [TestDomain]
  [Instantiable]
  public abstract class ClassWithoutRelatedClassIDColumnAndDerivation : TestDomainBase
  {
    public static ClassWithoutRelatedClassIDColumnAndDerivation NewObject ()
    {
      return NewObject<ClassWithoutRelatedClassIDColumnAndDerivation> ().With();
    }

    protected ClassWithoutRelatedClassIDColumnAndDerivation()
    {
    }

    protected ClassWithoutRelatedClassIDColumnAndDerivation (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    [DBBidirectionalRelation ("ClassWithoutRelatedClassIDColumnAndDerivation", ContainsForeignKey = true)]
    public abstract Company Company { get; set; }
  }
}
