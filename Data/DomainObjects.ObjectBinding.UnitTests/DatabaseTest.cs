using System;
using NUnit.Framework;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Development;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Mapping.Configuration;
using Rubicon.Data.DomainObjects.ObjectBinding.UnitTests.Database;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;

namespace Rubicon.Data.DomainObjects.ObjectBinding.UnitTests
{
  public class DatabaseTest
  {
    // types

    // static members and constants

    private const string c_connectionString = "Integrated Security=SSPI;Initial Catalog=DomainObjects_ObjectBinding_UnitTests;Data Source=localhost";
    private static readonly MappingConfiguration s_mappingConfiguration = MappingConfiguration.CreateConfigurationFromFileBasedLoader (@"Mapping.xml");

    // member fields

    // construction and disposing

    protected DatabaseTest()
    {
    }

    // methods and properties


    [TestFixtureSetUp]
    public void TestFixtureSetUp()
    {
      ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = new ProviderCollection<StorageProviderDefinition>();
      storageProviderDefinitionCollection.Add (new RdbmsProviderDefinition ("TestDomain", typeof (SqlProvider), c_connectionString));

      PersistenceConfiguration persistenceConfiguration =
          new PersistenceConfiguration (storageProviderDefinitionCollection, storageProviderDefinitionCollection["TestDomain"]);

      DomainObjectsConfiguration.SetCurrent (new FakeDomainObjectsConfiguration (new MappingLoaderConfiguration (), persistenceConfiguration));
      MappingConfiguration.SetCurrent (s_mappingConfiguration);
    }

    [TestFixtureTearDown]
    public void TestFixtureTearDown ()
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