using System;
using NUnit.Framework;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Development;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Mapping.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.UnitTests.Database;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests
{
  public class StandardMappingTest: DatabaseTest
  {
    // types

    // static members and constants

    private const string c_createTestDataFileName = "CreateTestData.sql";

    private static readonly MappingConfiguration s_mappingConfiguration = MappingConfiguration.CreateConfigurationFromFileBasedLoader (@"Mapping.xml");

    // member fields

    private DomainObjectIDs _domainObjectIDs;
    private FakeDomainObjectsConfiguration _domainObjectsConfiguration;

    // construction and disposing

    protected StandardMappingTest()
        : base (new StandardMappingTestDataLoader (c_connectionString), c_createTestDataFileName)
    {
    }

    // methods and properties

    [TestFixtureSetUp]
    public void TestFixtureSetUp()
    {
      ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = StorageProviderDefinitionFactory.Create(); 
      PersistenceConfiguration persistenceConfiguration =
          new PersistenceConfiguration (storageProviderDefinitionCollection, storageProviderDefinitionCollection["TestDomain"]);

      _domainObjectsConfiguration = new FakeDomainObjectsConfiguration (new MappingLoaderConfiguration (), persistenceConfiguration);
      DomainObjectsConfiguration.SetCurrent (_domainObjectsConfiguration);
      
      MappingConfiguration.SetCurrent (s_mappingConfiguration);
      TestMappingConfiguration.Reset();

      _domainObjectIDs = new DomainObjectIDs();
    }

    public override void SetUp ()
    {
      DomainObjectsConfiguration.SetCurrent (_domainObjectsConfiguration);
      base.SetUp ();
    }

    public override void TearDown ()
    {
      base.TearDown ();
      DomainObjectsConfiguration.SetCurrent (null);
    }

    protected DomainObjectIDs DomainObjectIDs
    {
      get { return _domainObjectIDs; }
    }
  }
}