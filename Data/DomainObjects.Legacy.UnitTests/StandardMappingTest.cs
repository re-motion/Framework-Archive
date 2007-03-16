using System;
using NUnit.Framework;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Development;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.Database;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.Factories;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Mapping.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests
{
  public class StandardMappingTest: DatabaseTest
  {
    // types

    // static members and constants

    private const string c_createTestDataFileName = "CreateTestData.sql";

    private static readonly MappingConfiguration s_mappingConfiguration = MappingConfiguration.CreateConfigurationFromFileBasedLoader (@"Mapping.xml");

    // member fields

    private DomainObjectIDs _domainObjectIDs;

    // construction and disposing

    protected StandardMappingTest()
        : base (new StandardMappingTestDataLoader (c_connectionString), c_createTestDataFileName)
    {
    }

    // methods and properties

    [TestFixtureSetUp]
    public void TestFixtureSetUp()
    {
      ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = new ProviderCollection<StorageProviderDefinition>();
      storageProviderDefinitionCollection.Add (
          new RdbmsProviderDefinition (
              "TestDomain",
              typeof (SqlProvider),
              "Integrated Security=SSPI;Initial Catalog=TestDomain;Data Source=localhost"));
      storageProviderDefinitionCollection.Add (
          new UnitTestStorageProviderStubDefinition (
              "UnitTestStorageProviderStub",
              typeof (UnitTestStorageProviderStub)));

      PersistenceConfiguration persistenceConfiguration =
          new PersistenceConfiguration (storageProviderDefinitionCollection, storageProviderDefinitionCollection["TestDomain"]);

      DomainObjectsConfiguration.SetCurrent (new FakeDomainObjectsConfiguration (new MappingLoaderConfiguration (), persistenceConfiguration));

      MappingConfiguration.SetCurrent (s_mappingConfiguration);
      TestMappingConfiguration.Reset();

      _domainObjectIDs = new DomainObjectIDs();
    }

    [TestFixtureTearDown]
    public void TestFixtureTearDown ()
    {
      DomainObjectsConfiguration.SetCurrent (null);
    }

    public override void SetUp ()
    {
      MappingConfiguration.SetCurrent (s_mappingConfiguration);
      base.SetUp ();
    }
    
    protected DomainObjectIDs DomainObjectIDs
    {
      get { return _domainObjectIDs; }
    }
  }
}