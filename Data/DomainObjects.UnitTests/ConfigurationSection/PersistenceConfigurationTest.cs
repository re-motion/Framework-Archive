using System;
using System.Configuration;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.UnitTests.Configuration;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.ConfigurationSection
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
    public void Deserialize_WithRdbmsProviderDefinition()
    {
      string xmlFragment = @" 
          <storage defaultProviderDefinition=""Rdbms"">
            <providerDefinitions>
              <add type=""Rubicon.Data.DomainObjects::Persistence.Rdbms.RdbmsProviderDefinition"" 
                  name=""Rdbms"" 
                  providerType=""Rubicon.Data.DomainObjects::Persistence.Rdbms.SqlProvider""
                  connectionString=""Rdbms""/>
            </providerDefinitions>
          </storage>";
      _configSystemHelper.ReplayConfigSystem();

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Assert.IsInstanceOfType (typeof (RdbmsProviderDefinition), _configuration.StorageProviderDefinition);
      Assert.AreEqual (1, _configuration.StorageProviderDefinitions.Count);
      Assert.AreSame (_configuration.StorageProviderDefinition, _configuration.StorageProviderDefinitions["Rdbms"]);
      Assert.AreSame (typeof (SqlProvider), _configuration.StorageProviderDefinition.StorageProviderType);
      Assert.AreEqual ("ConnectionString", ((RdbmsProviderDefinition) _configuration.StorageProviderDefinition).ConnectionString);
      _configSystemHelper.VerifyConfigSystem ();
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        "The provider 'Invalid' specified for the defaultProviderDefinition does not exist in the providers collection.")]
    public void Test_WithRdbmsProviderDefinitionAndInvalidName()
    {
      string xmlFragment = @"
          <storage defaultProviderDefinition=""Invalid"">
            <providerDefinitions>
              <add type=""Rubicon.Data.DomainObjects::Persistence.Rdbms.RdbmsProviderDefinition"" 
                  name=""Rdbms"" 
                  providerType=""Rubicon.Data.DomainObjects::Persistence.Rdbms.SqlProvider""
                  connectionString=""Rdbms""/>
            </providerDefinitions>
          </storage>";
      _configSystemHelper.ReplayConfigSystem();

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Dev.Null = _configuration.StorageProviderDefinition;
      _configSystemHelper.VerifyConfigSystem();
    }
  }
}