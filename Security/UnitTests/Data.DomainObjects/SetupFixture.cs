using System;
using System.ComponentModel.Design;
using System.IO;
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Reflection;
using Remotion.Security.UnitTests.Data.DomainObjects.TestDomain;

namespace Remotion.Security.UnitTests.Data.DomainObjects
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
          new QueryConfiguration (GetFullPath (@"Remotion.Security.UnitTests.Data.DomainObjects.Queries.xml"))));

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