using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Mixins;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence
{
  [TestFixture]
  public class MixedStorageProviderTest
  {
    [Test]
    public void StorageProvidersCanBeMixed ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (StorageProvider)).Clear().AddMixins (typeof (StorageProviderWithFixedGuidMixin)).EnterScope())
      {
        ClassDefinition orderDefinition = MappingConfiguration.Current.ClassDefinitions[typeof (Order)];
        StorageProvider provider = new StorageProviderManager ()[orderDefinition.StorageProviderID];
        Assert.IsNotNull (Mixin.Get<StorageProviderWithFixedGuidMixin> (provider));
      }
    }

    [Test]
    public void MixinsCanOverrideStorageProviderMethods ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (StorageProvider)).Clear().AddMixins (typeof (StorageProviderWithFixedGuidMixin)).EnterScope())
      {
        ClassDefinition orderDefinition = MappingConfiguration.Current.ClassDefinitions[typeof (Order)];
        StorageProvider provider = new StorageProviderManager ()[orderDefinition.StorageProviderID];
        ObjectID id1 = provider.CreateNewObjectID (orderDefinition);
        ObjectID id2 = provider.CreateNewObjectID (orderDefinition);
        Assert.AreEqual (id1, id2);
      }
    }

    [Test]
    public void MixinsCanIntroduceStorageProviderInterfaces ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (StorageProvider)).Clear().AddMixins (typeof (StorageProviderWithFixedGuidMixin)).EnterScope())
      {
        ClassDefinition orderDefinition = MappingConfiguration.Current.ClassDefinitions[typeof (Order)];
        StorageProvider provider = new StorageProviderManager ()[orderDefinition.StorageProviderID];
        
        Guid fixedGuid = Guid.NewGuid ();
        ((IStorageProviderWithFixedGuid) provider).FixedGuid = fixedGuid;

        ObjectID id = provider.CreateNewObjectID (orderDefinition);
        Assert.AreEqual (fixedGuid, id.Value);
      }
    }
  }
}