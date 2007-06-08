using System;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors
{
  public abstract class ClassWithInvalidBidirectionalRelationRightSide: DomainObject
  {
    protected ClassWithInvalidBidirectionalRelationRightSide ()
    {
    }

    [DBBidirectionalRelation ("NoContainsKeyLeftSide")]
    public abstract ClassWithInvalidBidirectionalRelationLeftSide NoContainsKeyRightSide { get; set; }

    [DBBidirectionalRelation ("InvalidOppositePropertyTypeLeftSide")]
    public abstract OtherClassWithInvalidBidirectionalRelationLeftSide InvalidOppositePropertyTypeRightSide { get; set; }

    [DBBidirectionalRelation ("InvalidOppositeCollectionPropertyTypeLeftSide")]
    public abstract OtherClassWithInvalidBidirectionalRelationLeftSide InvalidOppositeCollectionPropertyTypeRightSide { get; set; }

    //[DBBidirectionalRelation ("MissingBidirectionalRelationAttributeLeftSide")]
    public abstract ClassWithInvalidBidirectionalRelationLeftSide MissingBidirectionalRelationAttributeRightSide { get; set; }

    //[DBBidirectionalRelation ("MissingBidirectionalRelationAttributeForCollectionPropertyLeftSide")]
    public abstract ClassWithInvalidBidirectionalRelationLeftSide MissingBidirectionalRelationAttributeForCollectionPropertyRightSide { get; set; }    
  
    [DBBidirectionalRelation ("Invalid")]
    public abstract ClassWithInvalidBidirectionalRelationLeftSide InvalidPropertyNameInBidirectionalRelationAttributeOnOppositePropertyRightSide { get; set; }
  }
}