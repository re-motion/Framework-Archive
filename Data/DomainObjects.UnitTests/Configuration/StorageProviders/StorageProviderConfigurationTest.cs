using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.StorageProviders
{
  [TestFixture]
  public class StorageProviderConfigurationTest : StandardMappingTest
  {
    [Test]
    [Obsolete]
    public void GetCurrent()
    {
      PersistenceConfiguration persistenceConfiguration = new PersistenceConfiguration();
      DomainObjectsConfiguration.SetCurrent (new FakeDomainObjectsConfiguration (new MappingLoaderConfiguration (), persistenceConfiguration, new QueryConfiguration()));

      Assert.AreSame (persistenceConfiguration.StorageProviderDefinitions, DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions);
      Assert.AreSame (
          DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions,
          StorageProviderConfiguration.Current.StorageProviderDefinitions);
    }
  }
}