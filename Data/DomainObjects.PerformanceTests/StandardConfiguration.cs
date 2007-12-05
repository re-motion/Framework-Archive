using System.ComponentModel.Design;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Development;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Mapping.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.Reflection;

namespace Rubicon.Data.DomainObjects.PerformanceTests
{
  public class StandardConfiguration
  {
    public const string ConnectionString = "Integrated Security=SSPI;Initial Catalog=PerformanceTestDomain;Data Source=localhost";

    public static void Initialize ()
    {
      ProviderCollection<StorageProviderDefinition> providers = new ProviderCollection<StorageProviderDefinition> ();
      providers.Add (new RdbmsProviderDefinition ("PerformanceTestDomain", typeof (SqlProvider), ConnectionString));
      PersistenceConfiguration persistenceConfiguration = new PersistenceConfiguration (providers, providers["PerformanceTestDomain"]);

      DomainObjectsConfiguration.SetCurrent (new FakeDomainObjectsConfiguration (new MappingLoaderConfiguration (), persistenceConfiguration, new QueryConfiguration()));


      ITypeDiscoveryService typeDiscoveryService = new AssemblyFinderTypeDiscoveryService (
            new AssemblyFinder (ApplicationAssemblyFinderFilter.Instance, typeof (StandardConfiguration).Assembly));
      MappingConfiguration mappingConfiguration = new MappingConfiguration (new MappingReflector (typeDiscoveryService));
      MappingConfiguration.SetCurrent (mappingConfiguration);
    }
  }
}