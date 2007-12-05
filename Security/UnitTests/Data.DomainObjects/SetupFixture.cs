using System;
using System.ComponentModel.Design;
using System.IO;
using NUnit.Framework;
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
using Rubicon.Security.UnitTests.Data.DomainObjects.TestDomain;

namespace Rubicon.Security.UnitTests.Data.DomainObjects
{
  [SetUpFixture]
  public class SetUpFixture
  {
    [SetUp]
    public void SetUp()
    {
      ProviderCollection<StorageProviderDefinition> providers = new ProviderCollection<StorageProviderDefinition>();
      providers.Add (new RdbmsProviderDefinition ("StorageProvider", typeof (StubStorageProvider), "NonExistingRdbms"));
      PersistenceConfiguration persistenceConfiguration = new PersistenceConfiguration (providers, providers["StorageProvider"]);
      DomainObjectsConfiguration.SetCurrent (new FakeDomainObjectsConfiguration (new MappingLoaderConfiguration (), persistenceConfiguration,
          new QueryConfiguration (GetFullPath (@"Rubicon.Security.UnitTests.Data.DomainObjects.Queries.xml"))));

      ITypeDiscoveryService typeDiscoveryService = new AssemblyFinderTypeDiscoveryService (
            new AssemblyFinder (ApplicationAssemblyFinderFilter.Instance, GetType().Assembly));
      MappingConfiguration.SetCurrent (new MappingConfiguration (new MappingReflector (typeDiscoveryService)));
    }

    private string GetFullPath (string fileName)
    {
      return Path.Combine (AppDomain.CurrentDomain.BaseDirectory, fileName);
    }
  }
}