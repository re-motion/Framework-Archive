using System;
using System.Xml;
using System.Xml.Schema;

using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Schemas;
using System.IO;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader
{
public class StorageProviderConfigurationLoader : BaseFileLoader
{
  // types

  // static members and constants

  public const string ConfigurationAppSettingKey = "Rubicon.Data.DomainObjects.Persistence.Configuration.ConfigurationFile";
  public const string DefaultConfigurationFile = "storageProviders.xml";

  public static StorageProviderConfigurationLoader Create ()
  {
    return new StorageProviderConfigurationLoader (LoaderUtility.GetXmlFileName (ConfigurationAppSettingKey, DefaultConfigurationFile));
  }

  // member fields

  // construction and disposing

  public StorageProviderConfigurationLoader (string configurationFile)
  {
    try
    {
      base.Initialize (
          configurationFile, 
          SchemaType.StorageProviders, 
          true,
          PrefixNamespace.StorageProviderConfigurationNamespace);

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
    return NamespaceManager.FormatXPath (xPath, PrefixNamespace.StorageProviderConfigurationNamespace.Uri);
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
