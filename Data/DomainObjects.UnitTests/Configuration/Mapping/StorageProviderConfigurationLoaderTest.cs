using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.ConfigurationLoader;
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
      StorageProviderConfigurationLoader loader = new StorageProviderConfigurationLoader (@"storageProviders.xml");

      // expectation: no exception
    }

    [Test]
    [ExpectedException (typeof (StorageProviderConfigurationException))]
    public void StorageProvidersWithSchemaException ()
    {
      StorageProviderConfigurationLoader loader = new StorageProviderConfigurationLoader (@"storageProvidersWithSchemaException.xml");
    }

    [Test]
    [ExpectedException (typeof (StorageProviderConfigurationException),
        "Error while reading storage provider configuration:"
        + " '<', hexadecimal value 0x3C, is an invalid attribute character. Line 10, position 3.")]
    public void StorageProvidersWithXmlException ()
    {
      StorageProviderConfigurationLoader loader = new StorageProviderConfigurationLoader (@"storageProvidersWithXmlException.xml");
    }

    [Test]
    [ExpectedException (typeof (StorageProviderConfigurationException),
        "Error while reading storage provider configuration: The root element has namespace 'http://www.rubicon-it.com/Data/DomainObjects/InvalidMappingNamespace'"
        + " but was expected to have 'http://www.rubicon-it.com/Data/DomainObjects/Persistence/1.0'.")]
    public void StorageProvidersWithInvalidNamespace ()
    {
      StorageProviderConfigurationLoader loader = new StorageProviderConfigurationLoader (@"storageProvidersWithInvalidNamespace.xml");
    }
  }

}
