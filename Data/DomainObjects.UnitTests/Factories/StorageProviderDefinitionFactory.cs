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
          new RdbmsProviderDefinition (
              "TestDomain",
              typeof (SqlProvider),
              "Integrated Security=SSPI;Initial Catalog=TestDomain;Data Source=localhost"));
      storageProviderDefinitionCollection.Add (
          new UnitTestStorageProviderStubDefinition (
              "UnitTestStorageProviderStub",
              typeof (UnitTestStorageProviderStub)));

      return storageProviderDefinitionCollection;
    }
  }
}