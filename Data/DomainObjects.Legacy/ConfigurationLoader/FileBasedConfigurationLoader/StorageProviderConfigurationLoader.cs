using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.ConfigurationLoader.FileBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Legacy.ConfigurationLoader.FileBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Legacy.Schemas;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Schemas;

namespace Rubicon.Data.DomainObjects.Legacy.ConfigurationLoader.FileBasedConfigurationLoader
{
  public class StorageProviderConfigurationLoader : BaseFileLoader, IStorageProviderConfigurationLoader
  {
    // types

    // static members and constants

    [Obsolete ("Check after Refactoring. (Version 1.7.42)")]
    public const string ConfigurationAppSettingKey = StorageProviderConfiguration.ConfigurationAppSettingKey;
  
    [Obsolete ("Check after Refactoring. (Version 1.7.42)")]
    public const string DefaultConfigurationFile = StorageProviderConfiguration.DefaultConfigurationFile;

    [Obsolete ("Use StorageProviderConfiguration.CreateConfigurationFromFileBasedLoader (). (Version 1.7.42", true)]
    public static StorageProviderConfigurationLoader Create ()
    {
      return new StorageProviderConfigurationLoader (LoaderUtility.GetConfigurationFileName (ConfigurationAppSettingKey, DefaultConfigurationFile));
    }

    // member fields

    // construction and disposing

    public StorageProviderConfigurationLoader (string configurationFile)
    {
      try
      {
        base.Initialize (
            configurationFile,
            LegacySchemaLoader.StorageProviders, 
            true,
            LegacyPrefixNamespace.StorageProviderConfigurationNamespace);

      }
      catch (ConfigurationException e)
      {
        throw CreateStorageProviderConfigurationException (
            e, "Error while reading storage provider configuration: {0} File: '{1}'.", e.Message, Path.GetFullPath (configurationFile));
      }
      catch (XmlSchemaException e)
      {
        throw CreateStorageProviderConfigurationException (
            e, "Error while reading storage provider configuration: {0} File: '{1}'.", e.Message, Path.GetFullPath (configurationFile));
      }
      catch (XmlException e)
      {
        throw CreateStorageProviderConfigurationException (
            e, "Error while reading storage provider configuration: {0} File: '{1}'.", e.Message, Path.GetFullPath (configurationFile));
      }
    }
  
    // methods and properties

    public StorageProviderDefinitionCollection GetStorageProviderDefinitions ()
    {
      StorageProviderDefinitionCollection storageProviders = new StorageProviderDefinitionCollection ();
      FillStorageProviderDefinitions (storageProviders);
      return storageProviders;
    }

    private void FillStorageProviderDefinitions (StorageProviderDefinitionCollection storageProviders)
    {
      XmlNodeList providerNodeList = Document.SelectNodes (FormatXPath (
                                                               "{0}:storageProviders/{0}:storageProvider"), NamespaceManager);

      foreach (XmlNode providerNode in providerNodeList)
      {
        StorageProviderDefinition provider = GetStorageProviderDefinition (providerNode);
        storageProviders.Add (provider);
      }
    }

    private StorageProviderDefinition GetStorageProviderDefinition (XmlNode storageProviderNode)
    {
      string id = storageProviderNode.SelectSingleNode ("@id", NamespaceManager).InnerText;
    
      Type storageProviderType = LoaderUtility.GetType (storageProviderNode, 
                                                        FormatXPath ("{0}:type"), NamespaceManager);

      XmlNode configurationNode = storageProviderNode.SelectSingleNode (FormatXPath (
                                                                            "{0}:configuration"), NamespaceManager);

      Type storageProviderDefinitionType = LoaderUtility.GetType (
          configurationNode, "@type", NamespaceManager);

      object storageProviderDefinitionObject = ReflectionUtility.CreateObject (
          storageProviderDefinitionType, new object[] {id, storageProviderType, configurationNode});

      if (storageProviderDefinitionObject as StorageProviderDefinition == null)
      {
        throw CreateStorageProviderConfigurationException (
            "Error loading configuration for storage provider '{0}'. " +
                "Configuration class is not derived from " + 
                    "Rubicon.Data.DomainObjects.Persistence.Configuration.StorageProviderDefinition.",
            id);
      }

      return (StorageProviderDefinition) storageProviderDefinitionObject;
    }

    private string FormatXPath (string xPath)
    {
      return NamespaceManager.FormatXPath (xPath, LegacyPrefixNamespace.StorageProviderConfigurationNamespace.Uri);
    }

    private StorageProviderConfigurationException CreateStorageProviderConfigurationException (
        string message, 
        params object[] args)
    {
      return CreateStorageProviderConfigurationException (null, message, args);
    }

    private StorageProviderConfigurationException CreateStorageProviderConfigurationException (
        Exception inner,
        string message, 
        params object[] args)
    {
      return new StorageProviderConfigurationException (string.Format (message, args), inner);
    }
  }
}
