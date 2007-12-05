using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Development;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Mapping.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.UnitTests.TableInheritance;
using Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.Reflection;
using System.Reflection;

namespace Rubicon.Data.DomainObjects.UnitTests.Factories
{
  public abstract class BaseConfiguration
  {
    public static AssemblyFinderTypeDiscoveryService GetTypeDiscoveryService (params Assembly[] rootAssemblies)
    {
      return new AssemblyFinderTypeDiscoveryService (new AssemblyFinder (ApplicationAssemblyFinderFilter.Instance, rootAssemblies));
    }

    private readonly PersistenceConfiguration _persistenceConfiguration;
    private readonly MappingLoaderConfiguration _mappingLoaderConfiguration;
    private readonly QueryConfiguration _queryConfiguration;
    private readonly MappingConfiguration _mappingConfiguration;

    protected BaseConfiguration ()
    {
      ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = StorageProviderDefinitionFactory.Create ();
      _persistenceConfiguration = new PersistenceConfiguration (storageProviderDefinitionCollection, storageProviderDefinitionCollection[DatabaseTest.DefaultStorageProviderID]);
      _persistenceConfiguration.StorageGroups.Add (new StorageGroupElement (new TestDomainAttribute (), DatabaseTest.c_testDomainProviderID));
      _persistenceConfiguration.StorageGroups.Add (new StorageGroupElement (new StorageProviderStubAttribute (), DatabaseTest.c_unitTestStorageProviderStubID));
      _persistenceConfiguration.StorageGroups.Add (new StorageGroupElement (new TableInheritanceTestDomainAttribute (), TableInheritanceMappingTest.TableInheritanceTestDomainProviderID));

      _mappingLoaderConfiguration = new MappingLoaderConfiguration ();
      _queryConfiguration = new QueryConfiguration ();
      DomainObjectsConfiguration.SetCurrent (new FakeDomainObjectsConfiguration (_mappingLoaderConfiguration, _persistenceConfiguration, _queryConfiguration));

      _mappingConfiguration = new MappingConfiguration (new MappingReflector (BaseConfiguration.GetTypeDiscoveryService (GetType ().Assembly)));
      MappingConfiguration.SetCurrent (_mappingConfiguration);
    }

    public MappingConfiguration GetMappingConfiguration ()
    {
      return _mappingConfiguration;
    }

    public PersistenceConfiguration GetPersistenceConfiguration ()
    {
      return _persistenceConfiguration;
    }

    public FakeDomainObjectsConfiguration GetDomainObjectsConfiguration()
    {
      return new FakeDomainObjectsConfiguration (_mappingLoaderConfiguration, _persistenceConfiguration, _queryConfiguration);
    }
  }
}