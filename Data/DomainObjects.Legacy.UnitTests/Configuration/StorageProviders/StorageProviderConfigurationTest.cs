using System;
using System.IO;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.ConfigurationLoader.FileBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.Configuration.StorageProviders
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
    public void IStorageProviderConfigurationLoader_Loading ()
    {
      IStorageProviderConfigurationLoader loader = new StorageProviderConfigurationLoader (@"StorageProvidersForLoaderTest.xml");

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
        StorageProviderConfiguration.SetCurrent (StorageProviderConfiguration.CreateConfigurationFromFileBasedLoader(@"StorageProvidersForLoaderTest.xml"));
        string configurationFile = Path.GetFullPath (@"StorageProvidersForLoaderTest.xml");

        Assert.IsNotNull (StorageProviderConfiguration.Current.Loader);
        Assert.AreEqual (configurationFile, ((StorageProviderConfigurationLoader) StorageProviderConfiguration.Current.Loader).ConfigurationFile);
      }
      finally
      {
        StorageProviderConfiguration.SetCurrent (null);
      }
    }

    [Test]
    public void ApplicationName ()
    {
      Assert.AreEqual ("UnitTests", ((StorageProviderConfigurationLoader) StorageProviderConfiguration.Current.Loader).GetApplicationName ());
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
