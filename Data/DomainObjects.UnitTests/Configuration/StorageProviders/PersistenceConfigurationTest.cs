using System;
using System.Configuration;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
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
      Assert.That (configuration.StorageProviderDefinition, Is.SameAs (providerDefinition3));
      Assert.That (configuration.StorageProviderDefinitions, Is.Not.SameAs (providers));
      Assert.That (configuration.StorageProviderDefinitions.Count, Is.EqualTo (2));
      Assert.That (providers["ProviderDefinition1"], Is.SameAs (providerDefinition1));
      Assert.That (providers["ProviderDefinition2"], Is.SameAs (providerDefinition2));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Collection is read-only.")]
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

      Assert.That (_configuration.StorageProviderDefinition, Is.InstanceOfType (typeof (RdbmsProviderDefinition)));
      Assert.That (_configuration.StorageProviderDefinitions.Count, Is.EqualTo (1));
      Assert.That (_configuration.StorageProviderDefinitions["Rdbms"], Is.SameAs (_configuration.StorageProviderDefinition));
      Assert.That (_configuration.StorageProviderDefinition.StorageProviderType, Is.SameAs (typeof (SqlProvider)));
      Assert.That (((RdbmsProviderDefinition) _configuration.StorageProviderDefinition).ConnectionString, Is.EqualTo ("ConnectionString"));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "The provider 'Invalid' specified for the defaultProviderDefinition does not exist in the providers collection.")]
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

    [Test]
    public void Deserialize_WithStorageGroups()
    {
      string xmlFragment =
          @"<storage defaultProviderDefinition=""Rdbms"">
            <groups>
              <add type=""Rubicon.Data.DomainObjects.UnitTests::Configuration.StorageProviders.StubStorageGroup1Attribute"" 
                  provider=""Rdbms""/>
              <add type=""Rubicon.Data.DomainObjects.UnitTests::Configuration.StorageProviders.StubStorageGroup2Attribute"" 
                  provider=""Rdbms""/>
            </groups>
            <providerDefinitions>
              <add type=""Rubicon.Data.DomainObjects::Persistence.Rdbms.RdbmsProviderDefinition"" 
                  name=""Rdbms"" 
                  providerType=""Rubicon.Data.DomainObjects::Persistence.Rdbms.SqlProvider""
                  connectionString=""Rdbms""/>
            </providerDefinitions>
          </storage>";

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Assert.That (_configuration.StorageGroups.Count, Is.EqualTo (2));
      Assert.That (_configuration.StorageGroups[0].StorageGroup, Is.InstanceOfType (typeof (StubStorageGroup1Attribute)));
      Assert.That (_configuration.StorageGroups[0].StorageProviderName, Is.EqualTo ("Rdbms"));
      Assert.That (_configuration.StorageGroups[1].StorageGroup, Is.InstanceOfType (typeof (StubStorageGroup2Attribute)));
      Assert.That (_configuration.StorageGroups[1].StorageProviderName, Is.EqualTo ("Rdbms"));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "The value of the property 'type' cannot be parsed.",
        MatchType = MessageMatch.Contains)]
    public void Deserialize_WithStorageGroupHavingInvalidTypeName()
    {
      string xmlFragment =
          @"<storage defaultProviderDefinition=""Rdbms"">
            <groups>
              <add type=""Invalid, Assembly"" provider=""Rdbms""/>
            </groups>
            <providerDefinitions>
              <add type=""Rubicon.Data.DomainObjects::Persistence.Rdbms.RdbmsProviderDefinition"" 
                  name=""Rdbms"" 
                  providerType=""Rubicon.Data.DomainObjects::Persistence.Rdbms.SqlProvider""
                  connectionString=""Rdbms""/>
            </providerDefinitions>
          </storage>";

      ConfigurationHelper.DeserializeSection (_configuration, xmlFragment);

      Dev.Null = _configuration.StorageGroups;
    }
  }
}