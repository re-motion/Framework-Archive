using System;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain
{
  [TestDomain]
  [Instantiable]
  [Serializable]
  [DBTable]
  public abstract class OppositeBidirectionalBindableDomainObject : DomainObject
  {
    [DBBidirectionalRelation ("RequiredBidirectionalRelatedObjectProperty", ContainsForeignKey = true)]
    public abstract BindableDomainObjectWithProperties OppositeRequiredRelatedObject { get; set; }
    [DBBidirectionalRelation ("NonRequiredBidirectionalRelatedObjectProperty", ContainsForeignKey = true)]
    public abstract BindableDomainObjectWithProperties OppositeNonRequiredRelatedObject { get; set; }

    [Mandatory]
    [DBBidirectionalRelation ("RequiredBidirectionalRelatedObjectsProperty")]
    public abstract BindableDomainObjectWithProperties OppositeRequiredRelatedObjects { get; set; }
    [DBBidirectionalRelation ("NonRequiredBidirectionalRelatedObjectsProperty")]
    public abstract BindableDomainObjectWithProperties OppositeNonRequiredRelatedObjects { get; set; }
  }
}
