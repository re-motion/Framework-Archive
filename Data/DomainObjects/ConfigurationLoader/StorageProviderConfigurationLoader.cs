using System;
using System.Xml;
using System.Xml.Schema;

using Rubicon.Data.DomainObjects.Configuration.StorageProviders;

namespace Rubicon.Data.DomainObjects.Configuration.Loader
{
public class StorageProviderConfigurationLoader : BaseLoader
{
  // types

  // static members and constants

  private const string c_defaultConfigurationFile = "storageProviders.xml";
  private const string c_defaultSchemaFile = "storageProviders.xsd";

  public static StorageProviderConfigurationLoader Create ()
  {
    return new StorageProviderConfigurationLoader (
        LoaderUtility.GetXmlFileName (
            "Rubicon.Data.DomainObjects.Configuration.StorageProviderConfigurationFile", c_defaultConfigurationFile),
        LoaderUtility.GetXmlFileName (
            "Rubicon.Data.DomainObjects.Configuration.StorageProviderSchemaFile", c_defaultSchemaFile));
  }

  // member fields

  // construction and disposing

  public StorageProviderConfigurationLoader (string configurationFile, string schemaFile)
  {
    try
    {
      base.Initialize (
          configurationFile, 
          schemaFile, 
          new PrefixNamespace[] {PrefixNamespace.StorageProviderConfigurationNamespace}, 
          PrefixNamespace.StorageProviderConfigurationNamespace);
    }
    catch (XmlSchemaException e)
    {
      throw CreateStorageProviderConfigurationException (
          e, "Error while reading storage provider configuration: {0}", e.Message);
    }
    catch (XmlException e)
    {
      throw CreateStorageProviderConfigurationException (
          e, "Error while reading storage provider configuration: {0}", e.Message);
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
          "Rubicon.Data.DomainObjects.Configuration.StorageProviders.StorageProviderDefinition.",
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
