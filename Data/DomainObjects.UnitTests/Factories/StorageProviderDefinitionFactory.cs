using System;

using Rubicon.Data.DomainObjects.Configuration.StorageProviders;
using Rubicon.Data.DomainObjects.Persistence;

namespace Rubicon.Data.DomainObjects.UnitTests.Factories
{
public sealed class StorageProviderDefinitionFactory
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  private StorageProviderDefinitionFactory ()
  {
  }

  // methods and properties

  public static StorageProviderDefinitionCollection Create ()
  {
    StorageProviderDefinitionCollection storageProviders = new StorageProviderDefinitionCollection ();

    storageProviders.Add (new RdbmsProviderDefinition (
        DatabaseTest.c_testDomainProviderID, 
        typeof (SqlProvider), 
        "Integrated Security=SSPI;Initial Catalog=TestDomain;Data Source=localhost"));

    storageProviders.Add (new UnitTestStorageProviderStubDefinition (
        DatabaseTest.c_unitTestStorageProviderStubID, typeof (UnitTestStorageProviderStub)));

    return storageProviders;
  }
}
}
