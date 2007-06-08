using System;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors
{
  public abstract class ClassWithInvalidBidirectionalRelationLeftSide: DomainObject
  {
    protected ClassWithInvalidBidirectionalRelationLeftSide ()
    {
    }

    [DBBidirectionalRelation ("NoContainsKeyRightSide")]
    public abstract ClassWithInvalidBidirectionalRelationRightSide NoContainsKeyLeftSide { get; set; }

    [DBBidirectionalRelation ("Invalid")]
    public abstract ClassWithInvalidBidirectionalRelationRightSide InvalidOppositePropertyNameLeftSide { get; set; }

    [DBBidirectionalRelation ("InvalidOppositePropertyTypeRightSide")]
    public abstract ClassWithInvalidBidirectionalRelationRightSide InvalidOppositePropertyTypeLeftSide { get; set; }

    [DBBidirectionalRelation ("InvalidOppositeCollectionPropertyTypeRightSide")]
    public abstract ClassWithInvalidBidirectionalRelationRightSide InvalidOppositeCollectionPropertyTypeLeftSide { get; set; }

    [DBBidirectionalRelation ("MissingBidirectionalRelationAttributeRightSide")]
    public abstract ClassWithInvalidBidirectionalRelationRightSide MissingBidirectionalRelationAttributeLeftSide { get; set; }

    [DBBidirectionalRelation ("MissingBidirectionalRelationAttributeForCollectionPropertyRightSide")]
    public abstract ObjectList<ClassWithInvalidBidirectionalRelationRightSide> MissingBidirectionalRelationAttributeForCollectionPropertyLeftSide { get; set; }

    [DBBidirectionalRelation ("InvalidPropertyNameInBidirectionalRelationAttributeOnOppositePropertyRightSide")]
    public abstract ClassWithInvalidBidirectionalRelationRightSide InvalidPropertyNameInBidirectionalRelationAttributeOnOppositePropertyLeftSide { get; set; }
  
  }
}