using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  [TestDomain]
  [NotAbstract]
  public abstract class ClassWithBothEndPointsOnSameClass: DomainObject
  {
    protected ClassWithBothEndPointsOnSameClass()
    {
    }

    protected ClassWithBothEndPointsOnSameClass (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    [DBBidirectionalRelation ("Children")]
    public abstract ClassWithBothEndPointsOnSameClass Parent { get; set; }

    [DBBidirectionalRelation ("Parent")]
    public abstract ObjectList<ClassWithBothEndPointsOnSameClass> Children { get; }
  }
}