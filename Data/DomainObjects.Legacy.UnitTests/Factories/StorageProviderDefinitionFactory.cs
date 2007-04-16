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
          new RdbmsProviderDefinition (DatabaseTest.c_testDomainProviderID, typeof (SqlProvider), DatabaseTest.TestDomainConnectionString));
      storageProviderDefinitionCollection.Add (
          new UnitTestStorageProviderStubDefinition (
              DatabaseTest.c_unitTestStorageProviderStubID,
              typeof (UnitTestStorageProviderStub)));

      return storageProviderDefinitionCollection;
    }
  }
}