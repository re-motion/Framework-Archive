using System;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomainWithErrors
{
  public abstract class ClassWithInvalidBidirectionalRelation: DomainObject
  {
    protected ClassWithInvalidBidirectionalRelation ()
    {
    }

    protected ClassWithInvalidBidirectionalRelation (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    [DBBidirectionalRelation ("RightSide")]
    public abstract ClassWithInvalidBidirectionalRelation LeftSide { get; set; }

    [DBBidirectionalRelation ("LeftSide")]
    public abstract ClassWithInvalidBidirectionalRelation RightSide { get; set; }
  }
}