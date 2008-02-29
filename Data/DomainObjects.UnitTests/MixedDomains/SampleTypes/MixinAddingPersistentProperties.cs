using System;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes
{
  public class MixinAddingPersistentProperties : DomainObjectMixin<DomainObject>, IMixinAddingPeristentProperties
  {
    private int _nonPersistentProperty = 0;

    public int PersistentProperty
    {
      get { return Properties[typeof (MixinAddingPersistentProperties), "PersistentProperty"].GetValue<int>(); }
      set { Properties[typeof (MixinAddingPersistentProperties), "PersistentProperty"].SetValue (value); }
    }

    [StorageClass (StorageClass.Persistent)]
    public int ExtraPersistentProperty
    {
      get { return Properties[typeof (MixinAddingPersistentProperties), "ExtraPersistentProperty"].GetValue<int> (); }
      set { Properties[typeof (MixinAddingPersistentProperties), "ExtraPersistentProperty"].SetValue (value); }
    }

    [StorageClassNone]
    public int NonPersistentProperty
    {
      get { return _nonPersistentProperty; }
      set { _nonPersistentProperty = value; }
    }

    [DBBidirectionalRelation ("RelationProperty1", ContainsForeignKey = true)]
    public RelationTargetForPersistentMixin RelationProperty
    {
      get { return Properties[typeof (MixinAddingPersistentProperties), "RelationProperty"].GetValue<RelationTargetForPersistentMixin>(); }
      set { Properties[typeof (MixinAddingPersistentProperties), "RelationProperty"].SetValue (value); }
    }

    [DBBidirectionalRelation ("RelationProperty2", ContainsForeignKey = false)]
    public RelationTargetForPersistentMixin VirtualRelationProperty
    {
      get { return Properties[typeof (MixinAddingPersistentProperties), "VirtualRelationProperty"].GetValue<RelationTargetForPersistentMixin> (); }
      set { Properties[typeof (MixinAddingPersistentProperties), "VirtualRelationProperty"].SetValue (value); }
    }

    [DBBidirectionalRelation ("RelationProperty3")]
    public ObjectList<RelationTargetForPersistentMixin> CollectionPropertyNSide
    {
      get { return Properties[typeof (MixinAddingPersistentProperties), "CollectionPropertyNSide"].GetValue < ObjectList<RelationTargetForPersistentMixin>> (); }
    }

    [DBBidirectionalRelation ("RelationProperty4")]
    public RelationTargetForPersistentMixin CollectionProperty1Side
    {
      get { return Properties[typeof (MixinAddingPersistentProperties), "CollectionProperty1Side"].GetValue<RelationTargetForPersistentMixin> (); }
      set { Properties[typeof (MixinAddingPersistentProperties), "CollectionProperty1Side"].SetValue (value); }
    }
  }
}