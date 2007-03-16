using System;
using System.Configuration;
using NUnit.Framework;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Development.UnitTesting;
using Rubicon.Development.UnitTesting.Configuration;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.StorageProviders
{
  [TestFixture]
  public class PersistenceConfigurationTest
  {
    private PersistenceConfiguration _configuration;
    private ConfigSystemHelper _configSystemHelper;

    [SetUp]
    public void SetUp()
    {
      _configuration = new PersistenceConfiguration();
      _configSystemHelper = new ConfigSystemHelper();
      _configSystemHelper.SetUpConfigSystem();

      _configSystemHelper.SetUpConnectionString ("Rdbms", "ConnectionString", null);
    }

    [TearDown]
    public void TearDown()
    {
      _configSystemHelper.TearDownConfigSystem();
    }

    [Test]
    public void Initialize_WithProviderCollectionAndProvider()
    {
      StorageProviderDefinition providerDefinition1 = new RdbmsProviderDefinition ("ProviderDefinition1", typeof (SqlProvider), "ConnectionString");
      StorageProviderDefinition providerDefinition2 = new RdbmsProviderDefinition ("ProviderDefinition2", typeof (SqlProvider), "ConnectionString");
      StorageProviderDefinition providerDefinition3 = new RdbmsProviderDefinition ("ProviderDefinition3", typeof (SqlProvider), "ConnectionString");
      ProviderCollection<StorageProviderDefinition> providers = new ProviderCollection<StorageProviderDefinition>();
      providers.Add (providerDefinition1);
      providers.Add (providerDefinition2);

      PersistenceConfiguration configuration = new PersistenceConfiguration (providers, providerDefinition3);
      Assert.AreSame (providerDefinition3, configuration.StorageProviderDefinition);
      Assert.AreNotSame (providers, configuration.StorageProviderDefinitions);
      Assert.AreEqual (2, configuration.StorageProviderDefinitions.Count);
      Assert.AreSame (providerDefinition1, providers["ProviderDefinition1"]);
      Assert.AreSame (providerDefinition2, providers["ProviderDefinition2"]);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), "Collection is read-only.")]
    public void Initialize_WithProviderCollectionAndProvider_Expect()
    {
      StorageProviderDefinition providerDefinition = new RdbmsProviderDefinition ("ProviderDefinition", typeof (SqlProvider), "ConnectionString");
      ProviderCollection<StorageProviderDefinition> providers = new ProviderCollection<StorageProviderDefinition>();

      PersistenceConfiguration configuration = new PersistenceConfiguration (providers, providerDefinition);
      configuration.StorageProviderDefinitions.Add (providerDefinition);
    }

    [Test]
    public void Deserialize_WithRdbmsProviderDefinition()
    {
      string xmlFragment =
          @"<storage defaultProviderDefinition=""Rdbms"">
            <providerDefinitions>
              <add type=""Rubicon.Data.DomainObjects::Persistence.Rdbms.RdbmsProviderDefinition"" 
                  name=""Rdbms"" 
                  providerType=""Rubicon.Data.DomainObjects::Persistence.Rdbms.SqlProvider""
                  connectionString=""Rdbms""/>
            </providerDefinitions>
          </storage>";

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (RdbmsProviderDefinition), _configuration.StorageProviderDefinition);
      Assert.AreEqual (1, _configuration.StorageProviderDefinitions.Count);
      Assert.AreSame (_configuration.StorageProviderDefinition, _configuration.StorageProviderDefinitions["Rdbms"]);
      Assert.AreSame (typeof (SqlProvider), _configuration.StorageProviderDefinition.StorageProviderType);
      Assert.AreEqual ("ConnectionString", ((RdbmsProviderDefinition) _configuration.StorageProviderDefinition).ConnectionString);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        "The provider 'Invalid' specified for the defaultProviderDefinition does not exist in the providers collection.")]
    public void Test_WithRdbmsProviderDefinitionAndInvalidName()
    {
      string xmlFragment =
          @"<storage defaultProviderDefinition=""Invalid"">
            <providerDefinitions>
              <add type=""Rubicon.Data.DomainObjects::Persistence.Rdbms.RdbmsProviderDefinition"" 
                  name=""Rdbms"" 
                  providerType=""Rubicon.Data.DomainObjects::Persistence.Rdbms.SqlProvider""
                  connectionString=""Rdbms""/>
            </providerDefinitions>
          </storage>";

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Dev.Null = _configuration.StorageProviderDefinition;
    }
  }
}