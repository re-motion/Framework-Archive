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

namespace Remotion.Data.DomainObjects.Legacy.UnitTests
{
  public class StandardMappingTest: DatabaseTest
  {
    // types

    // static members and constants

    public const string CreateTestDataFileName = "DataDomainObjectsLegacy_CreateTestData.sql";

    private static readonly MappingConfiguration s_mappingConfiguration = XmlBasedMappingConfiguration.Create (@"Mapping.xml");

    // member fields

    private DomainObjectIDs _domainObjectIDs;

    // construction and disposing

    protected StandardMappingTest()
        : base (new StandardMappingDatabaseAgent (TestDomainConnectionString), CreateTestDataFileName)
    {
    }

    // methods and properties

    [TestFixtureSetUp]
    public virtual void TestFixtureSetUp()
    {
      ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = StorageProviderDefinitionFactory.Create ();
      PersistenceConfiguration persistenceConfiguration =
          new PersistenceConfiguration (storageProviderDefinitionCollection, storageProviderDefinitionCollection[DatabaseTest.c_testDomainProviderID]);

      DomainObjectsConfiguration.SetCurrent (new FakeDomainObjectsConfiguration (new MappingLoaderConfiguration (), persistenceConfiguration, new QueryConfiguration()));

      MappingConfiguration.SetCurrent (s_mappingConfiguration);
      ConfigurationWrapper.SetCurrent (null);
      TestMappingConfiguration.Reset ();

      _domainObjectIDs = new DomainObjectIDs();
    }

    public override void SetUp ()
    {
      base.SetUp ();
      MappingConfiguration.SetCurrent (s_mappingConfiguration);
      ConfigurationWrapper.SetCurrent (null);
    }
    
    protected DomainObjectIDs DomainObjectIDs
    {
      get { return _domainObjectIDs; }
    }
  }
}