using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Rubicon.Data.DomainObjects.Configuration.Loader
{
public class BaseLoader
{
  // types

  // static members and constants

  // member fields

  private XmlDocument _document;
  private ConfigurationNamespaceManager _namespaceManager;

  // construction and disposing

  protected BaseLoader ()
  {
  }

  protected void Initialize (
      string configurationFile, 
      string schemaFile, 
      PrefixNamespace[] namespaces, 
      PrefixNamespace schemaNamespace)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("configurationFile", configurationFile);
    ArgumentUtility.CheckNotNullOrEmpty ("schemaFile", schemaFile);
    ArgumentUtility.CheckNotNull ("namespaces", namespaces);
    ArgumentUtility.CheckNotNull ("schemaNamespace", schemaNamespace);

    if (!File.Exists (configurationFile)) throw new FileNotFoundException (string.Format ("Configuration file '{0}' could not be found.", configurationFile) , configurationFile);
    if (!File.Exists (schemaFile)) throw new FileNotFoundException (string.Format ("Schema file '{0}' could not be found.", schemaFile), schemaFile);

    _document = LoadConfigurationFile (configurationFile, schemaFile, schemaNamespace.Uri);

    _namespaceManager = new ConfigurationNamespaceManager (_document, namespaces);
  }

  // methods and properties

  private XmlDocument LoadConfigurationFile (
      string configurationFile, 
      string schemaFile,
      string schemaNamespace)
  {
    XmlTextReader textReader = null;
    XmlValidatingReader validatingReader = null;

    try
    {
      textReader = new XmlTextReader (configurationFile);
      validatingReader = new XmlValidatingReader (textReader);
      validatingReader.ValidationType = ValidationType.Schema;
      validatingReader.Schemas.Add (schemaNamespace, schemaFile);

      XmlDocument document = new XmlDocument (new NameTable ());
      document.Load (validatingReader);

      return document;
    }
    finally 
    {
      if (textReader != null)
        textReader.Close ();

      if (validatingReader != null)
        validatingReader.Close ();
    }
  }

  protected XmlDocument Document
  {
    get { return _document; }
  }

  protected ConfigurationNamespaceManager NamespaceManager
  {
    get { return _namespaceManager; }
  }
}
}
