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
    StorageProviderConfigurationLoader loader = new StorageProviderConfigurationLoader (@"storageProvidersForLoaderTest.xml", @"storageProviders.xsd");

    StorageProviderDefinitionCollection actualProviders = loader.GetStorageProviderDefinitions ();
    StorageProviderDefinitionCollection expectedProviders = StorageProviderDefinitionFactory.Create ();

    StorageProviderDefinitionChecker checker = new StorageProviderDefinitionChecker ();
    checker.Check (expectedProviders, actualProviders);
  }

  [Test]
  public void InitializeWithFileNames ()
  {
    try
    {
      StorageProviderConfiguration.SetCurrent (new StorageProviderConfiguration (@"storageProvidersForLoaderTest.xml", @"storageProviders.xsd"));
      string configurationFile = Path.GetFullPath (@"storageProvidersForLoaderTest.xml");
      string schemaFile = Path.GetFullPath (@"storageProviders.xsd");

      Assert.AreEqual (configurationFile, StorageProviderConfiguration.Current.ConfigurationFile);
      Assert.AreEqual (schemaFile, StorageProviderConfiguration.Current.SchemaFile);
    }
    finally
    {
      StorageProviderConfiguration.SetCurrent (null);
    }
  }
}
}
