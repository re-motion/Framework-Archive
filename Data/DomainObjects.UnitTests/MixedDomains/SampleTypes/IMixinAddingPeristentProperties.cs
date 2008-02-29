using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes
{
  public interface IMixinAddingPeristentProperties
  {
    int PersistentProperty { get; set; }

    [StorageClass (StorageClass.Persistent)]
    int ExtraPersistentProperty { get; set; }

    [StorageClassNone]
    int NonPersistentProperty { get; set; }

    //[DBBidirectionalRelation ("RelationProperty1", ContainsForeignKey = true)]
    //RelationTargetForPersistentMixin RelationProperty
    //{
    //  get;
    //  set;
    //}

    //[DBBidirectionalRelation ("RelationProperty2", ContainsForeignKey = false)]
    //RelationTargetForPersistentMixin VirtualRelationProperty
    //{
    //  get;
    //  set;
    //}

    //[DBBidirectionalRelation ("RelationProperty3")]
    //ObjectList<RelationTargetForPersistentMixin> CollectionPropertyNSide
    //{
    //  get;
    //  set;
    //}

    //[DBBidirectionalRelation ("RelationProperty4")]
    //RelationTargetForPersistentMixin CollectionProperty1Side
    //{
    //  get;
    //  set;
    //}
  }
}