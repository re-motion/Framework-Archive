using System;
using System.Data.SqlClient;
using System.IO;
using NUnit.Framework;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Development;
using Rubicon.Data.DomainObjects.Legacy.UnitTests;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.Database;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.Factories;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Mapping.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private StandardMappingTestDataLoader _loader;

    [SetUp]
    public void SetUp()
    {
      ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = StorageProviderDefinitionFactory.Create ();
      PersistenceConfiguration persistenceConfiguration =
          new PersistenceConfiguration (storageProviderDefinitionCollection, storageProviderDefinitionCollection[DatabaseTest.c_testDomainProviderID]);

      DomainObjectsConfiguration.SetCurrent (new FakeDomainObjectsConfiguration (new MappingLoaderConfiguration (), persistenceConfiguration));

      MappingConfiguration mappingConfiguration =
          MappingConfiguration.CreateConfigurationFromFileBasedLoader (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Mapping.xml"));
      MappingConfiguration.SetCurrent (mappingConfiguration);
      TestMappingConfiguration.Reset ();

      SqlConnection.ClearAllPools();
      _loader = new StandardMappingTestDataLoader (DatabaseTest.c_connectionString);
      _loader.Load ("CreateTestData.sql");
      _loader.Load ("CreateTableInheritanceTestData.sql");
      _loader.SetDatabaseReadOnly (DatabaseTest.DatabaseName);
    }

    [TearDown]
    public void TearDown()
    {
      _loader.SetDatabaseReadWrite (DatabaseTest.DatabaseName);
      SqlConnection.ClearAllPools();
    }
  }
}