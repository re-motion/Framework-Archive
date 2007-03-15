using System;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.Factories
{
  public sealed class StorageProviderDefinitionFactory
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    private StorageProviderDefinitionFactory()
    {
    }

    // methods and properties

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