using System;
using System.Data.SqlClient;
using System.IO;
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Legacy;
using Remotion.Data.DomainObjects.Legacy.Mapping;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Development.UnitTesting.Data.SqlClient;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private const string c_databaseName = "DomainObjects_ObjectBinding_UnitTests";
    private const string c_testDomainConnectionString = "Integrated Security=SSPI;Initial Catalog=DomainObjects_ObjectBinding_UnitTests;Data Source=localhost";
    private const string c_masterConnectionString = "Integrated Security=SSPI;Initial Catalog=master;Data Source=localhost";
    
    private DatabaseAgent _databaseAgent;

    [SetUp]
    public void SetUp()
    {
      ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = new ProviderCollection<StorageProviderDefinition> ();
      storageProviderDefinitionCollection.Add (new RdbmsProviderDefinition ("TestDomain", typeof (SqlProvider), c_testDomainConnectionString));

      PersistenceConfiguration persistenceConfiguration =
          new PersistenceConfiguration (storageProviderDefinitionCollection, storageProviderDefinitionCollection["TestDomain"]);

      DomainObjectsConfiguration.SetCurrent (new FakeDomainObjectsConfiguration (new MappingLoaderConfiguration (), persistenceConfiguration, new QueryConfiguration (GetFullPath (@"DataDomainObjectsObjectBinding_Queries.xml"))));
      MappingConfiguration.SetCurrent (XmlBasedMappingConfiguration.Create (GetFullPath (@"DataDomainObjectsObjectBinding_Mapping.xml")));

      SqlConnection.ClearAllPools();

      DatabaseAgent masterAgent = new DatabaseAgent (c_masterConnectionString);
      masterAgent.ExecuteBatch ("DataDomainObjectsObjectBinding_CreateDB.sql", false);
      _databaseAgent = new DatabaseAgent (c_testDomainConnectionString);
      _databaseAgent.ExecuteBatch ("DataDomainObjectsObjectBinding_SetupDB.sql", true);

      _databaseAgent.ExecuteBatch ("DataDomainObjectsObjectBinding_CreateTestData.sql", true);
      _databaseAgent.SetDatabaseReadOnly (c_databaseName);
    }

    [TearDown]
    public void TearDown()
    {
      _databaseAgent.SetDatabaseReadWrite (c_databaseName);
      SqlConnection.ClearAllPools();
    }

    private string GetFullPath (string fileName)
    {
      return Path.Combine (AppDomain.CurrentDomain.BaseDirectory, fileName);
    }
  }
}