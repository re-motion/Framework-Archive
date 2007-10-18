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
      using (MixinConfiguration.ScopedEmpty ())
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
      using (MixinConfiguration.ScopedEmpty ())
      {
        TargetClassForPersistentMixin.GetObject (new ObjectID (typeof (TargetClassForPersistentMixin), Guid.NewGuid()));
      }
    }

    [Test]
    public void DynamicChangeInNonPersistentMixinConfigurationDoesntMatter ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (TargetClassForPersistentMixin), typeof (MixinAddingPersistentProperties))) // no NullMixin
      {
        TargetClassForPersistentMixin.NewObject ();
        TargetClassForPersistentMixin.GetObject (new ObjectID (typeof (TargetClassForPersistentMixin), Guid.NewGuid ()));
      }
    }
  }
}