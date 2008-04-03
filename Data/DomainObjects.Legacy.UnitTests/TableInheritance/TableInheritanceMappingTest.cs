using System;
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Legacy.Mapping;
using Remotion.Data.DomainObjects.Legacy.UnitTests.Database;
using Remotion.Data.DomainObjects.Legacy.UnitTests.Factories;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Development.UnitTesting.Data.SqlClient;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TableInheritance
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

      _domainObjectsConfiguration = new FakeDomainObjectsConfiguration (new MappingLoaderConfiguration (), persistenceConfiguration, new QueryConfiguration ());
      DomainObjectsConfiguration.SetCurrent (_domainObjectsConfiguration);

      MappingConfiguration.SetCurrent (s_mappingConfiguration);
      _domainObjectIDs = new DomainObjectIDs ();
    }

    public override void SetUp ()
    {
      base.SetUp ();
      MappingConfiguration.SetCurrent (s_mappingConfiguration);
      _clientTransactionScope = ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ();
    }

    public override void TearDown ()
    {
      _clientTransactionScope.Leave ();
      base.TearDown ();
    }
  }
}
