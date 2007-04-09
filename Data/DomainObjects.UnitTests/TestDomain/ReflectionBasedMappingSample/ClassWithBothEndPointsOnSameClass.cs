using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  [TestDomain]
  [NotAbstract]
  public abstract class ClassWithBothEndPointsOnSameClass : TestDomainBase
  {
    protected ClassWithBothEndPointsOnSameClass (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    [AutomaticProperty]
    [DBBidirectionalRelation ("Children")]
    public abstract ClassWithBothEndPointsOnSameClass Parent { get; set; }

    [AutomaticProperty]
    [DBBidirectionalRelation ("Parent")]
    public abstract ObjectList<ClassWithBothEndPointsOnSameClass> Children { get; }
  }
}