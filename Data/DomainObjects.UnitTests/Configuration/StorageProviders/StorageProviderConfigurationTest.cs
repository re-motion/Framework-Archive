using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Development;
using Rubicon.Data.DomainObjects.Persistence.Configuration;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.StorageProviders
{
  [TestFixture]
  public class StorageProviderConfigurationTest: StandardMappingTest
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public StorageProviderConfigurationTest()
    {
    }

    // methods and properties

    [Test]
    [Obsolete]
    public void GetCurrent()
    {
      PersistenceConfiguration persistenceConfiguration = new PersistenceConfiguration();
      DomainObjectsConfiguration.SetCurrent (new FakeDomainObjectsConfiguration (persistenceConfiguration));

      Assert.AreSame (persistenceConfiguration.StorageProviderDefinitions, DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions);
      Assert.AreSame (
          DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions,
          StorageProviderConfiguration.Current.StorageProviderDefinitions);
    }
  }
}