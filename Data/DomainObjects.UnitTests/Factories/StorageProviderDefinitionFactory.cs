using System;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.UnitTests.TableInheritance;

namespace Rubicon.Data.DomainObjects.UnitTests.Factories
{
  public static class StorageProviderDefinitionFactory
  {
    public static ProviderCollection<StorageProviderDefinition> Create()
    {
      ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = new ProviderCollection<StorageProviderDefinition>();

      storageProviderDefinitionCollection.Add (
          new RdbmsProviderDefinition (DatabaseTest.c_testDomainProviderID, typeof (SqlProvider), DatabaseTest.TestDomainConnectionString));

      storageProviderDefinitionCollection.Add (
          new RdbmsProviderDefinition (DatabaseTest.DefaultStorageProviderID, typeof (SqlProvider), DatabaseTest.TestDomainConnectionString));

      storageProviderDefinitionCollection.Add (
          new UnitTestStorageProviderStubDefinition (DatabaseTest.c_unitTestStorageProviderStubID, typeof (UnitTestStorageProviderStub)));

      storageProviderDefinitionCollection.Add (
          new RdbmsProviderDefinition (
              TableInheritanceMappingTest.TableInheritanceTestDomainProviderID, typeof (SqlProvider), DatabaseTest.TestDomainConnectionString));

      return storageProviderDefinitionCollection;
    }
  }
}