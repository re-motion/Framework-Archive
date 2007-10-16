using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.MixedDomains
{
  [TestFixture]
  public class PersistentMixinTest : ClientTransactionBaseTest
  {
    [Test]
    public void ClassDefinitionIncludesPersistentProperties ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (TargetClassForPersistentMixin));
      Assert.IsNotNull (classDefinition.GetPropertyDefinition (typeof (MixinAddingPeristentProperties).FullName + ".PersistentProperty"));
      Assert.IsNotNull (classDefinition.GetPropertyDefinition (typeof (MixinAddingPeristentProperties).FullName + ".ExtraPersistentProperty"));
    }

    [Test]
    public void ClassDefinitionExcludesNonPersistentProperties ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (TargetClassForPersistentMixin));
      Assert.IsNull (classDefinition.GetPropertyDefinition (typeof (MixinAddingPeristentProperties).FullName + ".NonPersistentProperty"));
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
  }
}