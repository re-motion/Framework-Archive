using System;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomainWithErrors
{
  public abstract class ClassWithInvalidBidirectionalRelation: DomainObject
  {
    protected ClassWithInvalidBidirectionalRelation ()
    {
    }

    [DBBidirectionalRelation ("NoContainsKeyRightSide")]
    public abstract ClassWithInvalidBidirectionalRelation NoContainsKeyLeftSide { get; set; }

    [DBBidirectionalRelation ("NoContainsKeyLeftSide")]
    public abstract ClassWithInvalidBidirectionalRelation NoContainsKeyRightSide { get; set; }

    [DBBidirectionalRelation ("Invalid")]
    public abstract ClassWithInvalidBidirectionalRelation InvalidOppositePropertyNameLeftSide { get; set; }
  }
}