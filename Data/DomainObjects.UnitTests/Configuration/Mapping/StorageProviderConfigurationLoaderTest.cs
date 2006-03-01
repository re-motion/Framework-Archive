using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.Persistence.Configuration;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{

[TestFixture]
public class StorageProviderConfigurationLoaderTest
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
    StorageProviderConfigurationLoader loader = new StorageProviderConfigurationLoader (
        @"storageProviders.xml", 
        @"storageProviders.xsd");

    // expectation: no exception
  }

  [Test]
  [ExpectedException (typeof (StorageProviderConfigurationException))]
  public void StorageProvidersWithSchemaException ()
  {
    StorageProviderConfigurationLoader loader = new StorageProviderConfigurationLoader (
        @"storageProvidersWithSchemaException.xml", 
        @"storageProviders.xsd");
  }

  [Test]
  [ExpectedException (typeof (StorageProviderConfigurationException), 
      "Error while reading storage provider configuration:"
      + " '<', hexadecimal value 0x3C, is an invalid attribute character. Line 10, position 3.")]
  public void StorageProvidersWithXmlException ()
  {
    StorageProviderConfigurationLoader loader = new StorageProviderConfigurationLoader (
        @"storageProvidersWithXmlException.xml", 
        @"storageProviders.xsd");
  }
}

}
