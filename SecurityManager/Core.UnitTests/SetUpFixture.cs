using System;
using System.Data.SqlClient;
using System.IO;
using NUnit.Framework;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Development;
using Rubicon.Data.DomainObjects.Legacy.Mapping;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Mapping.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.Development.UnitTesting.Data.SqlClient;
using Rubicon.SecurityManager.Domain;
using Rubicon.SecurityManager.Persistence;

namespace Rubicon.SecurityManager.UnitTests
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private const string c_testDomainConnectionString = "Integrated Security=SSPI;Initial Catalog=RubiconSecurityManager;Data Source=localhost";
    private const string c_masterConnectionString = "Integrated Security=SSPI;Initial Catalog=master;Data Source=localhost";

    [SetUp]
    public void SetUp()
    {
      ProviderCollection<StorageProviderDefinition> providers = new ProviderCollection<StorageProviderDefinition> ();
      providers.Add (new RdbmsProviderDefinition ("SecurityManager", typeof (SecurityManagerSqlProvider), c_testDomainConnectionString));
      PersistenceConfiguration persistenceConfiguration = new PersistenceConfiguration (providers, providers["SecurityManager"]);
      persistenceConfiguration.StorageGroups.Add (new StorageGroupElement (new SecurityManagerStorageGroupAttribute(), "SecurityManager"));

      DomainObjectsConfiguration.SetCurrent (new FakeDomainObjectsConfiguration (new MappingLoaderConfiguration (), persistenceConfiguration));

      MappingConfiguration.SetCurrent (new MappingConfiguration (new MappingReflector (typeof (BaseSecurityManagerObject).Assembly)));
      QueryConfiguration.SetCurrent (new QueryConfiguration (GetFullPath (@"SecurityManagerQueries.xml")));

      SqlConnection.ClearAllPools();

      DatabaseAgent masterAgent = new DatabaseAgent (c_masterConnectionString);
      masterAgent.ExecuteBatch ("SecurityManagerCreateDB.sql", false);
      DatabaseAgent databaseAgent = new DatabaseAgent (c_testDomainConnectionString);
      databaseAgent.ExecuteBatch ("SecurityManagerSetupDB.sql", true);
      databaseAgent.ExecuteBatch ("SecurityManagerSetupConstraints.sql", true);
      databaseAgent.ExecuteBatch ("SecurityManagerSetupDBSpecialTables.sql", true);
    }

    [TearDown]
    public void TearDown()
    {
      SqlConnection.ClearAllPools();
    }

    private string GetFullPath (string fileName)
    {
      return Path.Combine (AppDomain.CurrentDomain.BaseDirectory, fileName);
    }
  }
}