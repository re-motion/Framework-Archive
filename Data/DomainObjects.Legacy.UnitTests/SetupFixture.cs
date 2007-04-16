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
using Rubicon.Data.DomainObjects.Legacy.UnitTests.TableInheritance;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Mapping.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Development.UnitTesting.Data.SqlClient;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private StandardMappingDatabaseAgent _standardMappingAgent;

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

      DatabaseAgent masterAgent = new DatabaseAgent (DatabaseTest.MasterConnectionString);
      masterAgent.ExecuteBatch ("LegacyCreateDB.sql", false);
      DatabaseAgent testDomainAgent = new DatabaseAgent (DatabaseTest.TestDomainConnectionString);
      testDomainAgent.ExecuteBatch ("LegacySetupDB.sql", true);
      
      _standardMappingAgent = new StandardMappingDatabaseAgent (DatabaseTest.TestDomainConnectionString);
      _standardMappingAgent.ExecuteBatch (StandardMappingTest.CreateTestDataFileName, true);
      _standardMappingAgent.ExecuteBatch (TableInheritanceMappingTest.CreateTestDataFileName, true);
      _standardMappingAgent.SetDatabaseReadOnly (DatabaseTest.DatabaseName);
    }

    [TearDown]
    public void TearDown()
    {
      _standardMappingAgent.SetDatabaseReadWrite (DatabaseTest.DatabaseName);
      SqlConnection.ClearAllPools();
    }
  }
}