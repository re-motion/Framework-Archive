using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes;
using Rubicon.Mixins;

namespace Rubicon.Data.DomainObjects.UnitTests.MixedDomains
{
  [TestFixture]
  public class PersistentMixinTest : ClientTransactionBaseTest
  {
    [Test]
    public void ClassDefinitionIncludesPersistentProperties ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (TargetClassForPersistentMixin));
      Assert.IsNotNull (classDefinition.GetPropertyDefinition (typeof (MixinAddingPersistentProperties).FullName + ".PersistentProperty"));
      Assert.IsNotNull (classDefinition.GetPropertyDefinition (typeof (MixinAddingPersistentProperties).FullName + ".ExtraPersistentProperty"));
    }

    [Test]
    public void ClassDefinitionExcludesNonPersistentProperties ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (TargetClassForPersistentMixin));
      Assert.IsNull (classDefinition.GetPropertyDefinition (typeof (MixinAddingPersistentProperties).FullName + ".NonPersistentProperty"));
    }

    [Test]
    public void ClassDefinitionIncludesRelationProperty ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (TargetClassForPersistentMixin));
      Assert.IsNotNull (classDefinition.GetPropertyDefinition (typeof (MixinAddingPersistentProperties).FullName + ".RelationProperty"));
      Assert.IsNotNull (classDefinition.GetRelationDefinition (typeof (MixinAddingPersistentProperties).FullName + ".RelationProperty"));
    }

    [Test]
    public void RelationTargetClassDefinitionIncludesRelationProperty ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (RelationTargetForPersistentMixin));
      Assert.IsNull (classDefinition.GetPropertyDefinition (typeof (RelationTargetForPersistentMixin).FullName + ".RelationProperty"));
      Assert.IsNotNull (classDefinition.GetRelationDefinition (typeof (RelationTargetForPersistentMixin).FullName + ".RelationProperty1"));
    }

    [Test]
    public void GetSetCommitRollbackPersistentProperties ()
    {
      IMixinAddingPeristentProperties properties = TargetClassForPersistentMixin.NewObject () as IMixinAddingPeristentProperties;
      Assert.IsNotNull (properties);
      
      properties.ExtraPersistentProperty = 10;
      properties.PersistentProperty = 11;
      properties.NonPersistentProperty = 12;

      Assert.AreEqual (10, properties.ExtraPersistentProperty);
      Assert.AreEqual (11, properties.PersistentProperty);
      Assert.AreEqual (12, properties.NonPersistentProperty);

      ClientTransactionMock.Commit ();

      Assert.AreEqual (10, properties.ExtraPersistentProperty);
      Assert.AreEqual (11, properties.PersistentProperty);
      Assert.AreEqual (12, properties.NonPersistentProperty);

      properties.ExtraPersistentProperty = 13;
      properties.PersistentProperty = 14;
      properties.NonPersistentProperty = 15;

      Assert.AreEqual (13, properties.ExtraPersistentProperty);
      Assert.AreEqual (14, properties.PersistentProperty);
      Assert.AreEqual (15, properties.NonPersistentProperty);

      ClientTransactionMock.Rollback ();

      Assert.AreEqual (10, properties.ExtraPersistentProperty);
      Assert.AreEqual (11, properties.PersistentProperty);
      Assert.AreEqual (15, properties.NonPersistentProperty);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "A persistence-related mixin was removed from the domain object type "
        + "Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes.TargetClassForPersistentMixin after the mapping information was built: "
        + "Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes.MixinAddingPersistentProperties.")]
    public void DynamicChangeInPersistentMixinConfigurationThrowsInNewObject ()
    {
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        TargetClassForPersistentMixin.NewObject ();
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "A persistence-related mixin was removed from the domain object type "
        + "Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes.TargetClassForPersistentMixin after the mapping information was built: "
        + "Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes.MixinAddingPersistentProperties.")]
    public void DynamicChangeInPersistentMixinConfigurationThrowsInGetObject ()
    {
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        TargetClassForPersistentMixin.GetObject (new ObjectID (typeof (TargetClassForPersistentMixin), Guid.NewGuid()));
      }
    }

    [Test]
    public void DynamicChangeInNonPersistentMixinConfigurationDoesntMatter ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (TargetClassForPersistentMixin)).Clear().AddMixins (typeof (MixinAddingPersistentProperties)).EnterScope()) // no NullMixin
      {
        TargetClassForPersistentMixin.NewObject ();
        TargetClassForPersistentMixin.GetObject (new ObjectID (typeof (TargetClassForPersistentMixin), Guid.NewGuid ()));
      }
    }

    [Test]
    public void RelationPropertyRealSide ()
    {
      TargetClassForPersistentMixin  tc = TargetClassForPersistentMixin.NewObject ();
      RelationTargetForPersistentMixin relationTarget = RelationTargetForPersistentMixin.NewObject().With();
      MixinAddingPersistentProperties mixin = Mixin.Get<MixinAddingPersistentProperties> (tc);
      mixin.RelationProperty = relationTarget;
      Assert.AreSame (relationTarget, mixin.RelationProperty);
      Assert.AreSame (tc, relationTarget.RelationProperty1);
    }

    [Test]
    public void VirtualRelationProperty ()
    {
      TargetClassForPersistentMixin tc = TargetClassForPersistentMixin.NewObject ();
      RelationTargetForPersistentMixin relationTarget = RelationTargetForPersistentMixin.NewObject ().With ();
      MixinAddingPersistentProperties mixin = Mixin.Get<MixinAddingPersistentProperties> (tc);
      mixin.VirtualRelationProperty = relationTarget;
      Assert.AreSame (relationTarget, mixin.VirtualRelationProperty);
      Assert.AreSame (tc, relationTarget.RelationProperty2);
    }

    [Test]
    public void CollectionPropertyNSide ()
    {
      TargetClassForPersistentMixin tc = TargetClassForPersistentMixin.NewObject ();
      RelationTargetForPersistentMixin relationTarget1 = RelationTargetForPersistentMixin.NewObject ().With ();
      RelationTargetForPersistentMixin relationTarget2 = RelationTargetForPersistentMixin.NewObject ().With ();
      MixinAddingPersistentProperties mixin = Mixin.Get<MixinAddingPersistentProperties> (tc);
      mixin.CollectionPropertyNSide.Add (relationTarget1);
      mixin.CollectionPropertyNSide.Add (relationTarget2);
      Assert.AreSame (relationTarget1, mixin.CollectionPropertyNSide[0]);
      Assert.AreSame (relationTarget2, mixin.CollectionPropertyNSide[1]);
      Assert.AreSame (tc, relationTarget1.RelationProperty3);
      Assert.AreSame (tc, relationTarget2.RelationProperty3);
    }

    [Test]
    public void CollectionProperty1Side ()
    {
      TargetClassForPersistentMixin tc1 = TargetClassForPersistentMixin.NewObject ();
      TargetClassForPersistentMixin tc2 = TargetClassForPersistentMixin.NewObject ();
      RelationTargetForPersistentMixin relationTarget = RelationTargetForPersistentMixin.NewObject ().With ();
      MixinAddingPersistentProperties mixin1 = Mixin.Get<MixinAddingPersistentProperties> (tc1);
      MixinAddingPersistentProperties mixin2 = Mixin.Get<MixinAddingPersistentProperties> (tc2);
      mixin1.CollectionProperty1Side = relationTarget;
      mixin2.CollectionProperty1Side = relationTarget;
      Assert.AreSame (relationTarget, mixin1.CollectionProperty1Side);
      Assert.AreSame (relationTarget, mixin2.CollectionProperty1Side);
      Assert.AreSame (tc1, relationTarget.RelationProperty4[0]);
      Assert.AreSame (tc2, relationTarget.RelationProperty4[1]);
    }
  }
}