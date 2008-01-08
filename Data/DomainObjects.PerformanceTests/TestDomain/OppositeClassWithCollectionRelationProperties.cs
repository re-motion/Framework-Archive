using System;

namespace Rubicon.Data.DomainObjects.PerformanceTests.TestDomain
{
  [Instantiable]
  [DBTable]
  [Serializable]
  public abstract class OppositeClassWithCollectionRelationProperties : SimpleDomainObject<OppositeClassWithCollectionRelationProperties>
  {
    [DBBidirectionalRelation("Collection")]
    public abstract ClassWithRelationProperties EndOfCollection { get; set; }
    
  }
}