using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Configuration.Loader;
using Rubicon.Data.DomainObjects.Configuration.StorageProviders;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.StorageProviders
{
[TestFixture]
public class StorageProviderConfigurationTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public StorageProviderConfigurationTest ()
  {
  }

  // methods and properties

  [Test]
  public void Loading ()
  {
    StorageProviderConfigurationLoader loader = new StorageProviderConfigurationLoader (
        @"..\..\storageProviders.xml", @"..\..\..\storageProviders.xsd");

    StorageProviderDefinitionCollection actualProviders = loader.GetStorageProviderDefinitions ();

    StorageProviderDefinitionCollection expectedProviders = StorageProviderDefinitionFactory.Create ();

    StorageProviderDefinitionChecker checker = new StorageProviderDefinitionChecker ();
    checker.Check (expectedProviders, actualProviders);
  }
}
}
