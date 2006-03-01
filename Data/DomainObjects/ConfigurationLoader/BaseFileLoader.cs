using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader
{
public class BaseLoader
{
  // types

  // static members and constants

  // member fields

  private XmlDocument _document;
  private ConfigurationNamespaceManager _namespaceManager;
  private string _configurationFile;
  private string _schemaFile;
  private bool _resolveTypeNames;

  // construction and disposing

  protected BaseLoader ()
  {
  }

  protected void Initialize (
      string configurationFile, 
      string schemaFile, 
      bool resolveTypeNames,
      PrefixNamespace[] namespaces, 
      PrefixNamespace schemaNamespace)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("configurationFile", configurationFile);
    ArgumentUtility.CheckNotNullOrEmpty ("schemaFile", schemaFile);
    ArgumentUtility.CheckNotNull ("namespaces", namespaces);
    ArgumentUtility.CheckNotNull ("schemaNamespace", schemaNamespace);

    if (!File.Exists (configurationFile)) throw new FileNotFoundException (string.Format ("Configuration file '{0}' could not be found.", configurationFile) , configurationFile);
    if (!File.Exists (schemaFile)) throw new FileNotFoundException (string.Format ("Schema file '{0}' could not be found.", schemaFile), schemaFile);

    _configurationFile = Path.GetFullPath (configurationFile);
    _schemaFile = Path.GetFullPath (schemaFile);
    _resolveTypeNames = resolveTypeNames;

    _document = LoadConfigurationFile (_configurationFile, _schemaFile, schemaNamespace.Uri);
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

  public string GetApplicationName ()
  {
    return _document.SelectSingleNode ("/*/@application", _namespaceManager).InnerText;
  }

  public string ConfigurationFile
  {
    get { return _configurationFile; }
  }

  public string SchemaFile
  {
    get { return _schemaFile; }
  }

  public bool ResolveTypeNames
  {
    get { return _resolveTypeNames; }
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
