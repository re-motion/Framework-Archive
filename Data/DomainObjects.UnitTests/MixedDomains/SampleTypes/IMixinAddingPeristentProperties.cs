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
  }
}