using System;
using NUnit.Framework;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Development;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.UnitTests.Database;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance
{
  public class TableInheritanceMappingTest : DatabaseTest
  {
    // types

    // static members and constants

    private const string c_createTestDataFileName = "CreateTableInheritanceTestData.sql";

    private static readonly MappingConfiguration s_mappingConfiguration = MappingConfiguration.CreateConfigurationFromFileBasedLoader(@"TableInheritanceMapping.xml");

    // member fields

    private DomainObjectIDs _domainObjectIDs;
    private FakeDomainObjectsConfiguration _domainObjectsConfiguration;

    // construction and disposing

    public TableInheritanceMappingTest () : base (new TestDataLoader (c_connectionString), c_createTestDataFileName)
    {
    }

    // methods and properties

    protected DomainObjectIDs DomainObjectIDs
    {
      get { return _domainObjectIDs; }
    }

    [TestFixtureSetUp]
    public void TestFixtureSetUp ()
    {
      ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = StorageProviderDefinitionFactory.Create ();
      PersistenceConfiguration persistenceConfiguration =
          new PersistenceConfiguration (storageProviderDefinitionCollection, storageProviderDefinitionCollection["TestDomain"]);

      _domainObjectsConfiguration = new FakeDomainObjectsConfiguration (persistenceConfiguration);
      DomainObjectsConfiguration.SetCurrent (_domainObjectsConfiguration);

      MappingConfiguration.SetCurrent (s_mappingConfiguration);
      _domainObjectIDs = new DomainObjectIDs ();
    }

    public override void SetUp ()
    {
      base.SetUp ();

      ClientTransaction.SetCurrent (null);
    }
  }
}
