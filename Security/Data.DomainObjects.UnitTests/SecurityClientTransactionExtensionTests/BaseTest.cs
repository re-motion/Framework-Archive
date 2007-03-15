using System;
using NUnit.Framework;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Development;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.Security.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Security.Data.DomainObjects.UnitTests.SecurityClientTransactionExtensionTests
{
  public class BaseTest
  {
    [TestFixtureSetUp]
    public virtual void TestFixtureSetUp()
    {
      MappingConfiguration.SetCurrent (
          MappingConfiguration.CreateConfigurationFromFileBasedLoader (@"Rubicon.Security.Data.DomainObjects.UnitTests.Mapping.xml"));
      QueryConfiguration.SetCurrent (new QueryConfiguration (@"Rubicon.Security.Data.DomainObjects.UnitTests.Queries.xml"));

      ProviderCollection<StorageProviderDefinition> providers = new ProviderCollection<StorageProviderDefinition>();
      providers.Add (new RdbmsProviderDefinition ("StorageProvider", typeof (StubStorageProvider), "NonExistingRdbms"));
      PersistenceConfiguration persistenceConfiguration = new PersistenceConfiguration (providers, providers["StorageProvider"]);
      DomainObjectsConfiguration.SetCurrent (new FakeDomainObjectsConfiguration (persistenceConfiguration));
    }
  }
}