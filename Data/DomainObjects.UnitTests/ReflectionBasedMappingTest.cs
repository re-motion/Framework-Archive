using System;
using NUnit.Framework;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Development;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Mapping.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.UnitTests.Database;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests
{
  public class ReflectionBasedMappingTest: DatabaseTest
  {
    private const string c_createTestDataFileName = "CreateTestData.sql";

    private static readonly MappingConfiguration s_mappingConfiguration;
    private static readonly PersistenceConfiguration s_persistenceConfiguration;

    static ReflectionBasedMappingTest ()
    {
      ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = StorageProviderDefinitionFactory.Create();
      s_persistenceConfiguration = new PersistenceConfiguration (storageProviderDefinitionCollection, storageProviderDefinitionCollection[c_testDomainProviderID]);
      s_persistenceConfiguration.StorageGroups.Add (new StorageGroupElement (new TestDomainAttribute(), c_testDomainProviderID));
      s_persistenceConfiguration.StorageGroups.Add (new StorageGroupElement (new StorageProviderStubAttribute(), c_unitTestStorageProviderStubID));

      DomainObjectsConfiguration.SetCurrent (new FakeDomainObjectsConfiguration (new MappingLoaderConfiguration (), s_persistenceConfiguration));

      s_mappingConfiguration = new MappingConfiguration (new MappingReflector (typeof (ReflectionBasedMappingTest).Assembly));
    }

    private DomainObjectIDs _domainObjectIDs;

    protected ReflectionBasedMappingTest ()
        : base (new StandardMappingTestDataLoader (c_connectionString), c_createTestDataFileName)
    {
    }

    [TestFixtureSetUp]
    public void TestFixtureSetUp()
    {
      DomainObjectsConfiguration.SetCurrent (new FakeDomainObjectsConfiguration (new MappingLoaderConfiguration(), s_persistenceConfiguration));

      MappingConfiguration.SetCurrent (s_mappingConfiguration);
      TestMappingConfiguration.Reset();

      _domainObjectIDs = new DomainObjectIDs();
    }

    public override void SetUp()
    {
      DomainObjectsConfiguration.SetCurrent (new FakeDomainObjectsConfiguration (new MappingLoaderConfiguration (), s_persistenceConfiguration));
      base.SetUp ();
    }

    public override void TearDown()
    {
      base.TearDown();
      DomainObjectsConfiguration.SetCurrent (null);
    }

    protected DomainObjectIDs DomainObjectIDs
    {
      get { return _domainObjectIDs; }
    }
  }
}