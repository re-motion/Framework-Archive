using System;
using System.IO;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.StorageProviders
{
  [TestFixture]
  public class StorageProviderConfigurationTest : StandardMappingTest
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
      StorageProviderConfigurationLoader loader = new StorageProviderConfigurationLoader (@"StorageProvidersForLoaderTest.xml");

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
        StorageProviderConfiguration.SetCurrent (new StorageProviderConfiguration (@"StorageProvidersForLoaderTest.xml"));
        string configurationFile = Path.GetFullPath (@"StorageProvidersForLoaderTest.xml");

        Assert.AreEqual (configurationFile, StorageProviderConfiguration.Current.ConfigurationFile);
      }
      finally
      {
        StorageProviderConfiguration.SetCurrent (null);
      }
    }

    [Test]
    public void ApplicationName ()
    {
      Assert.AreEqual ("UnitTests", StorageProviderConfiguration.Current.ApplicationName);
    }

    [Test]
    public void Contains ()
    {
      StorageProviderDefinitionCollection storageProviders = StorageProviderDefinitionFactory.Create ();
      Assert.IsFalse (StorageProviderConfiguration.Current.Contains (storageProviders[0]));
      Assert.IsTrue (StorageProviderConfiguration.Current.Contains (StorageProviderConfiguration.Current["TestDomain"]));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void ContainsNull ()
    {
      StorageProviderConfiguration.Current.Contains (null);
    }
  }
}
