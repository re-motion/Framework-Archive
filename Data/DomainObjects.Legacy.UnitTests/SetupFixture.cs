using System;
using System.Data.SqlClient;
using System.IO;
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Legacy.Mapping;
using Remotion.Data.DomainObjects.Legacy.UnitTests;
using Remotion.Data.DomainObjects.Legacy.UnitTests.Database;
using Remotion.Data.DomainObjects.Legacy.UnitTests.Factories;
using Remotion.Data.DomainObjects.Legacy.UnitTests.TableInheritance;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Development.UnitTesting.Data.SqlClient;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests
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

      DomainObjectsConfiguration.SetCurrent (new FakeDomainObjectsConfiguration (new MappingLoaderConfiguration (), persistenceConfiguration, new QueryConfiguration ()));

      MappingConfiguration mappingConfiguration =
          XmlBasedMappingConfiguration.Create (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Mapping.xml"));
      MappingConfiguration.SetCurrent (mappingConfiguration);
      TestMappingConfiguration.Reset ();

      SqlConnection.ClearAllPools();

      DatabaseAgent masterAgent = new DatabaseAgent (DatabaseTest.MasterConnectionString);
      masterAgent.ExecuteBatch ("DataDomainObjectsLegacy_CreateDB.sql", false);
      DatabaseAgent testDomainAgent = new DatabaseAgent (DatabaseTest.TestDomainConnectionString);
      testDomainAgent.ExecuteBatch ("DataDomainObjectsLegacy_SetupDB.sql", true);
      
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