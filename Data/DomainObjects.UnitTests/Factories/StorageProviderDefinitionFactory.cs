using System;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;

namespace Rubicon.Data.DomainObjects.UnitTests.Factories
{
  public static class StorageProviderDefinitionFactory
  {
    public static ProviderCollection<StorageProviderDefinition> Create()
    {
      ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = new ProviderCollection<StorageProviderDefinition>();
      
      storageProviderDefinitionCollection.Add (
          new RdbmsProviderDefinition (DatabaseTest.c_testDomainProviderID, typeof (SqlProvider), DatabaseTest.c_connectionString));
      
      storageProviderDefinitionCollection.Add (
          new UnitTestStorageProviderStubDefinition (
              DatabaseTest.c_unitTestStorageProviderStubID,
              typeof (UnitTestStorageProviderStub)));

      return storageProviderDefinitionCollection;
    }
  }
}