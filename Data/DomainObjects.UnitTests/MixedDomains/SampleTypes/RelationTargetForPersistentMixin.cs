using System;

namespace Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes
{
  [DBTable]
  [Instantiable]
  public abstract class RelationTargetForPersistentMixin : SimpleDomainObject<RelationTargetForPersistentMixin>
  {
    [DBBidirectionalRelation ("RelationProperty")]
    public abstract TargetClassForPersistentMixin RelationProperty { get; set; }
  }
}