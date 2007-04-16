using System;
using NUnit.Framework;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Development;
using Rubicon.Data.DomainObjects.Legacy;
using Rubicon.Data.DomainObjects.Legacy.Mapping;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Mapping.Configuration;
using Rubicon.Data.DomainObjects.PerformanceTests.Database;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;

namespace Rubicon.Data.DomainObjects.PerformanceTests
{
  public class DatabaseTest
  {
    // types

    // static members and constants

    private const string c_connectionString = "Integrated Security=SSPI;Initial Catalog=PerformanceTestDomain;Data Source=localhost";

    // member fields

    // construction and disposing

    public DatabaseTest()
    {
    }

    // methods and properties

    [TestFixtureSetUp]
    public virtual void TestFixtureSetUp()
    {
      ProviderCollection<StorageProviderDefinition> providers = new ProviderCollection<StorageProviderDefinition>();
      providers.Add (new RdbmsProviderDefinition ("PerformanceTestDomain", typeof (SqlProvider), c_connectionString));
      PersistenceConfiguration persistenceConfiguration = new PersistenceConfiguration (providers, providers["PerformanceTestDomain"]);

      DomainObjectsConfiguration.SetCurrent (new FakeDomainObjectsConfiguration (new MappingLoaderConfiguration(), persistenceConfiguration));
      MappingConfiguration.SetCurrent (XmlBasedMappingConfiguration.Create());
    }

    [TestFixtureTearDown]
    public virtual void TestFixtureTearDown ()
    {
      DomainObjectsConfiguration.SetCurrent (null);
    }

    [SetUp]
    public virtual void SetUp()
    {
      using (TestDataLoader loader = new TestDataLoader (c_connectionString))
      {
        loader.Load();
      }
    }

    [TearDown]
    public virtual void TearDown()
    {
    }
  }
}