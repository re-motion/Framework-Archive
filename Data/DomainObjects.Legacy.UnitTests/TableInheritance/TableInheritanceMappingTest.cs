using System;
using NUnit.Framework;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Development;
using Rubicon.Data.DomainObjects.Legacy.Mapping;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.Database;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.Factories;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Mapping.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Development.UnitTesting.Data.SqlClient;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TableInheritance
{
  public class TableInheritanceMappingTest : DatabaseTest
  {
    // types

    // static members and constants

    public const string CreateTestDataFileName = "DataDomainObjectsLegacy_CreateTableInheritanceTestData.sql";

    private static readonly MappingConfiguration s_mappingConfiguration = XmlBasedMappingConfiguration.Create (@"DataDomainObjectsLegacy_TableInheritanceMapping.xml");

    // member fields

    private DomainObjectIDs _domainObjectIDs;
    private FakeDomainObjectsConfiguration _domainObjectsConfiguration;
    private ClientTransactionScope _clientTransactionScope;

    // construction and disposing

    public TableInheritanceMappingTest () : base (new DatabaseAgent (TestDomainConnectionString), CreateTestDataFileName)
    {
    }

    // methods and properties

    protected DomainObjectIDs DomainObjectIDs
    {
      get { return _domainObjectIDs; }
    }

    [TestFixtureSetUp]
    public virtual void TestFixtureSetUp ()
    {
      ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = StorageProviderDefinitionFactory.Create ();
      PersistenceConfiguration persistenceConfiguration =
          new PersistenceConfiguration (storageProviderDefinitionCollection, storageProviderDefinitionCollection[DatabaseTest.c_testDomainProviderID]);

      _domainObjectsConfiguration = new FakeDomainObjectsConfiguration (new MappingLoaderConfiguration (), persistenceConfiguration);
      DomainObjectsConfiguration.SetCurrent (_domainObjectsConfiguration);

      MappingConfiguration.SetCurrent (s_mappingConfiguration);
      _domainObjectIDs = new DomainObjectIDs ();
    }

    public override void SetUp ()
    {
      base.SetUp ();
      MappingConfiguration.SetCurrent (s_mappingConfiguration);
      _clientTransactionScope = ClientTransaction.NewTransaction ().EnterNonDiscardingScope ();
    }

    public override void TearDown ()
    {
      _clientTransactionScope.Leave ();
      base.TearDown ();
    }
  }
}
