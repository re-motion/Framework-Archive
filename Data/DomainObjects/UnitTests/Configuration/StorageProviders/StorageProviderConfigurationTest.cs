using System;
using System.IO;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
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

  [Test]
  public void InitializeWithFileNames ()
  {
    StorageProviderConfiguration.SetCurrent (
        new StorageProviderConfiguration (@"..\..\storageProviders.xml", @"..\..\..\storageProviders.xsd"));

    string configurationFile = Path.GetFullPath (@"..\..\storageProviders.xml");
    string schemaFile = Path.GetFullPath (@"..\..\..\storageProviders.xsd");

    Assert.AreEqual (configurationFile, StorageProviderConfiguration.Current.ConfigurationFile);
    Assert.AreEqual (schemaFile, StorageProviderConfiguration.Current.SchemaFile);
  }
}
}
