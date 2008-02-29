using System;

namespace Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes
{
  [DBTable]
  [Instantiable]
  public abstract class RelationTargetForPersistentMixin : SimpleDomainObject<RelationTargetForPersistentMixin>
  {
    [DBBidirectionalRelation ("RelationProperty")]
    public abstract TargetClassForPersistentMixin RelationProperty1 { get; set; }

    [DBBidirectionalRelation ("VirtualRelationProperty", ContainsForeignKey = true)]
    public abstract TargetClassForPersistentMixin RelationProperty2 { get; set; }

    [DBBidirectionalRelation ("CollectionPropertyNSide")]
    public abstract TargetClassForPersistentMixin RelationProperty3 { get; set; }

    [DBBidirectionalRelation ("CollectionProperty1Side")]
    public abstract ObjectList<TargetClassForPersistentMixin> RelationProperty4 { get; }
  }
}