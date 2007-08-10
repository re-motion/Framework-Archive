using System;
using System.Collections.Generic;
using System.Xml;
using Rubicon.Data.DomainObjects;
using Rubicon.Security.Metadata;
using Rubicon.Utilities;

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
      using (_transaction.EnterScope())
      {
        SecurityMetadataLocalizationSchema schema = new SecurityMetadataLocalizationSchema();
        if (!document.Schemas.Contains (schema.SchemaUri))
          document.Schemas.Add (schema.LoadSchemaSet());

        document.Validate (null);

        XmlNamespaceManager namespaceManager = new XmlNamespaceManager (document.NameTable);
        namespaceManager.AddNamespace ("mdl", schema.SchemaUri);

        Culture culture = ImportCulture (document.DocumentElement, namespaceManager);
        ImportLocalizedNames (culture, document, namespaceManager);
      }
    }

    private Culture ImportCulture (XmlElement rootElement, XmlNamespaceManager namespaceManager)
    {
      string cultureName = rootElement.Attributes["culture"].Value;
      // TODO: Convert to CultureInfo via GetCulture
      Culture culture = Culture.NewObject (ClientTransactionScope.CurrentTransaction, cultureName);

      _cultures.Add (culture);

      return culture;
    }

    private void ImportLocalizedNames (Culture culture, XmlNode parentNode, XmlNamespaceManager namespaceManager)
    {
      XmlNodeList nameNodes = parentNode.SelectNodes ("/mdl:localizedNames/mdl:localizedName", namespaceManager);

      foreach (XmlNode nameNode in nameNodes)
      {
        LocalizedName localizedName = ImportLocalizedName (culture, namespaceManager, nameNode);
        _localizedNames.Add (localizedName);
      }
    }

    private LocalizedName ImportLocalizedName (Culture culture, XmlNamespaceManager namespaceManager, XmlNode nameNode)
    {
      string metadataID = nameNode.Attributes["ref"].Value;
      XmlAttribute commentAttribute = nameNode.Attributes["comment"];
      
      MetadataObject metadataObject = MetadataObject.Find (metadataID, _transaction);
      if (metadataObject == null)
      {
        string objectDetails = commentAttribute == null ? string.Empty : "('" + commentAttribute.Value + "') ";
        throw new ImportException (string.Format ("The metadata object with the ID '{0}' {1}could not be found.", metadataID, objectDetails));
      }

      string text = nameNode.InnerText.Trim ();

      LocalizedName localizedName = metadataObject.GetLocalizedName (culture);
      if (localizedName != null)
      {
        localizedName.Text = text;
        return localizedName;
      }

      return LocalizedName.NewObject (_transaction, text, culture, metadataObject);
    }
  }
}
