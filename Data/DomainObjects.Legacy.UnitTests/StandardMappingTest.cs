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

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests
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

      DomainObjectsConfiguration.SetCurrent (new FakeDomainObjectsConfiguration (new MappingLoaderConfiguration (), persistenceConfiguration));

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