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
    private Culture _culture;

    public CultureImporter (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      _transaction = transaction;
      _localizedNames = new List<LocalizedName> ();
    }

    public List<LocalizedName> LocalizedNames
    {
      get { return _localizedNames; }
    }

    public Culture Culture
    {
      get { return _culture; }
    }

    public void Import (XmlDocument document)
    {
      SecurityMetadataLocalizationSchema schema = new SecurityMetadataLocalizationSchema();

      XmlNamespaceManager namespaceManager = new XmlNamespaceManager (document.NameTable);
      namespaceManager.AddNamespace ("mdl", schema.SchemaUri);

      ImportCulture (document.DocumentElement, namespaceManager);
      ImportLocalizedNames (document, namespaceManager);
    }

    private void ImportCulture (XmlElement rootElement, XmlNamespaceManager namespaceManager)
    {
      string cultureName = rootElement.Attributes["culture"].Value;
      _culture = new Culture (_transaction, cultureName);
    }

    private void ImportLocalizedNames (XmlNode parentNode, XmlNamespaceManager namespaceManager)
    {
      XmlNodeList nameNodes = parentNode.SelectNodes ("/mdl:localizedNames/mdl:localizedName", namespaceManager);

      foreach (XmlNode nameNode in nameNodes)
      {
        LocalizedName localizedName = CreateLocalizedName (namespaceManager, nameNode);
        _localizedNames.Add (localizedName);
      }
    }

    private LocalizedName CreateLocalizedName (XmlNamespaceManager namespaceManager, XmlNode nameNode)
    {
      string metadataID = nameNode.Attributes["ref"].Value;
      XmlAttribute commentAttribute = nameNode.Attributes["comment"];
      
      MetadataObject metadataObject = MetadataObject.Find (_transaction, metadataID);
      if (metadataObject == null)
      {
        string objectDetails = commentAttribute == null ? string.Empty : "('" + commentAttribute.Value + "') ";
        throw new ImportException (string.Format ("The metadata object with the ID '{0}' {1}could not be found.", metadataID, objectDetails));
      }

      return new LocalizedName (_transaction, nameNode.InnerText.Trim (), _culture, metadataObject);
    }
  }
}
