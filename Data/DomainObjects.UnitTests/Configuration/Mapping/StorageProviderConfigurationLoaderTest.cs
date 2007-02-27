using System;
using System.IO;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.ConfigurationLoader.FileBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Persistence.Configuration;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class StorageProviderConfigurationLoaderTest : StandardMappingTest
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public StorageProviderConfigurationLoaderTest ()
    {
    }

    // methods and properties

    [Test]
    public void ReadAndValidateStorageProviderFile ()
    {
      StorageProviderConfigurationLoader loader = new StorageProviderConfigurationLoader (@"StorageProviders.xml");

      // expectation: no exception
    }

    [Test]
    [ExpectedException (typeof (StorageProviderConfigurationException))]
    public void StorageProvidersWithSchemaException ()
    {
      StorageProviderConfigurationLoader loader = new StorageProviderConfigurationLoader (@"StorageProvidersWithSchemaException.xml");
    }

    [Test]
    public void StorageProvidersWithXmlException ()
    {
      string configurationFile = "StorageProvidersWithXmlException.xml";
      try
      {
        StorageProviderConfigurationLoader loader = new StorageProviderConfigurationLoader (configurationFile);

        Assert.Fail ("StorageProviderConfigurationException was expected");
      }
      catch (StorageProviderConfigurationException ex)
      {
        string expectedMessage = string.Format (
            "Error while reading storage provider configuration: '<', hexadecimal value 0x3C, is an invalid attribute character."
            + " Line 10, position 3. File: '{0}'.",  
            Path.GetFullPath (configurationFile));

        Assert.AreEqual (expectedMessage, ex.Message);
      }
    }

    [Test]
    public void StorageProvidersWithInvalidNamespace ()
    {
      string configurationFile = "StorageProvidersWithInvalidNamespace.xml";
      try
      {
        StorageProviderConfigurationLoader loader = new StorageProviderConfigurationLoader (configurationFile);

        Assert.Fail ("StorageProviderConfigurationException was expected");
      }
      catch (StorageProviderConfigurationException ex)
      {
        string expectedMessage = string.Format (
            "Error while reading storage provider configuration: The namespace 'http://www.rubicon-it.com/Data/DomainObjects/InvalidNamespace' of"
            + " the root element is invalid. Expected namespace: 'http://www.rubicon-it.com/Data/DomainObjects/Persistence/1.0'. File: '{0}'.",
            Path.GetFullPath (configurationFile));

        Assert.AreEqual (expectedMessage, ex.Message);
      }
    }
  }

}
