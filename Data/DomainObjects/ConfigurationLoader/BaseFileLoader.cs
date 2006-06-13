using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;

using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Schemas;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader
{
// TODO for all configuration loaders: Check if every field is trimmed during loading.
public class BaseFileLoader
{
  // types

  // static members and constants

  // member fields

  private XmlDocument _document;
  private ConfigurationNamespaceManager _namespaceManager;
  private string _configurationFile;
  private bool _resolveTypes;

  // construction and disposing

  protected BaseFileLoader ()
  {
  }

  protected void Initialize (
      string configurationFile,
      SchemaRetriever schemaRetriever,
      bool resolveTypes,
      PrefixNamespace[] namespaces,
      PrefixNamespace schemaNamespace)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("configurationFile", configurationFile);
    ArgumentUtility.CheckNotNull ("schemaRetriever", schemaRetriever);
    ArgumentUtility.CheckNotNull ("namespaces", namespaces);
    ArgumentUtility.CheckNotNull ("schemaNamespace", schemaNamespace);

    if (!File.Exists (configurationFile))
      throw new FileNotFoundException (string.Format ("Configuration file '{0}' could not be found.", configurationFile), configurationFile);

    _configurationFile = Path.GetFullPath (configurationFile);
    _resolveTypes = resolveTypes;

    _document = LoadConfigurationFile (_configurationFile, schemaRetriever, schemaNamespace.Uri);
    _namespaceManager = new ConfigurationNamespaceManager (_document, namespaces);
  }

  // methods and properties

  private XmlDocument LoadConfigurationFile (
      string configurationFile,
      SchemaRetriever schemaRetriever,
      string schemaNamespace)
  {
    using (XmlTextReader textReader = new XmlTextReader (configurationFile))
    {
      XmlReaderSettings validatingReaderSettings = new XmlReaderSettings ();
      validatingReaderSettings.ValidationType = ValidationType.Schema;
      validatingReaderSettings.Schemas.Add (schemaRetriever.GetSchemaSet ());

      using (XmlReader validatingReader = XmlReader.Create (textReader, validatingReaderSettings))
      {
        XmlDocument document = new XmlDocument (new NameTable ());
        document.Load (validatingReader);

        if (document.DocumentElement.NamespaceURI != schemaNamespace)
        {
          throw CreateConfigurationException (
              "The root element has namespace '{0}' but was expected to have '{1}'.",
              document.DocumentElement.NamespaceURI,
              schemaNamespace);
        }

        return document;
      }
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

  public bool ResolveTypes
  {
    get { return _resolveTypes; }
  }

  protected XmlDocument Document
  {
    get { return _document; }
  }

  protected ConfigurationNamespaceManager NamespaceManager
  {
    get { return _namespaceManager; }
  }

  private ConfigurationException CreateConfigurationException (string format, params string[] args)
  {
    return new ConfigurationException (string.Format (format, args));
  }
}
}
