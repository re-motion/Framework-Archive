using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Rubicon.Data.DomainObjects;
using Rubicon.Utilities;

using Rubicon.Security.Metadata;

namespace Rubicon.SecurityManager.Domain.Metadata
{
  public class CultureImporter
  {
    private ClientTransaction _transaction;
    private List<LocalizedName> _localizedNames;
    private List<Culture> _cultures;

    public CultureImporter (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      _transaction = transaction;
      _localizedNames = new List<LocalizedName> ();
      _cultures = new List<Culture> ();
    }

    public List<LocalizedName> LocalizedNames
    {
      get { return _localizedNames; }
    }

    public List<Culture> Cultures
    {
      get { return _cultures; }
    }

    public void Import (string filePath)
    {
      XmlDocument xmlDocument = new XmlDocument ();
      xmlDocument.Load (filePath);

      Import (xmlDocument);
    }

    public void Import (XmlDocument document)
    {
      SecurityMetadataLocalizationSchema schema = new SecurityMetadataLocalizationSchema();
      if (!document.Schemas.Contains (schema.SchemaUri))
        document.Schemas.Add (schema.LoadSchemaSet ());

      document.Validate (null);

      XmlNamespaceManager namespaceManager = new XmlNamespaceManager (document.NameTable);
      namespaceManager.AddNamespace ("mdl", schema.SchemaUri);

      Culture culture = ImportCulture (document.DocumentElement, namespaceManager);
      ImportLocalizedNames (culture, document, namespaceManager);
    }

    private Culture ImportCulture (XmlElement rootElement, XmlNamespaceManager namespaceManager)
    {
      string cultureName = rootElement.Attributes["culture"].Value;
      Culture culture = new Culture (_transaction, cultureName);

      _cultures.Add (culture);

      return culture;
    }

    private void ImportLocalizedNames (Culture culture, XmlNode parentNode, XmlNamespaceManager namespaceManager)
    {
      XmlNodeList nameNodes = parentNode.SelectNodes ("/mdl:localizedNames/mdl:localizedName", namespaceManager);

      foreach (XmlNode nameNode in nameNodes)
      {
        LocalizedName localizedName = CreateLocalizedName (culture, namespaceManager, nameNode);
        _localizedNames.Add (localizedName);
      }
    }

    private LocalizedName CreateLocalizedName (Culture culture, XmlNamespaceManager namespaceManager, XmlNode nameNode)
    {
      string metadataID = nameNode.Attributes["ref"].Value;
      XmlAttribute commentAttribute = nameNode.Attributes["comment"];
      
      MetadataObject metadataObject = MetadataObject.Find (_transaction, metadataID);
      if (metadataObject == null)
      {
        string objectDetails = commentAttribute == null ? string.Empty : "('" + commentAttribute.Value + "') ";
        throw new ImportException (string.Format ("The metadata object with the ID '{0}' {1}could not be found.", metadataID, objectDetails));
      }

      return new LocalizedName (_transaction, nameNode.InnerText.Trim (), culture, metadataObject);
    }
  }
}
