using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Rubicon.Data.DomainObjects;

namespace Rubicon.Security.Service.Domain.Metadata
{
  public class MetadataImporter
  {
    public DomainObjectCollection Import (ClientTransaction transaction, XmlDocument metadataXmlDocument)
    {
      DomainObjectCollection importedObjects = new DomainObjectCollection ();

      XmlNamespaceManager namespaceManager = new XmlNamespaceManager (metadataXmlDocument.NameTable);
      // TODO: namespace should be extracted to only one location!
      namespaceManager.AddNamespace ("md", "http://www.rubicon-it.com/Security/Metadata/1.0");

      XmlNodeList classNodes = metadataXmlDocument.SelectNodes ("/md:securityMetadata/md:classes/md:class", namespaceManager);
      foreach (XmlNode classNode in classNodes)
        importedObjects.Add (CreateSecurableClassDefinition (transaction, classNode));

      XmlNodeList roleNodes = metadataXmlDocument.SelectNodes ("/md:securityMetadata/md:abstractRoles/md:abstractRole", namespaceManager);
      foreach (XmlNode abstractRoleNode in roleNodes)
        importedObjects.Add (CreateAbstractRoleDefinition (transaction, abstractRoleNode));

      return importedObjects;
    }

    private SecurableClassDefinition CreateSecurableClassDefinition (ClientTransaction transaction, XmlNode securableClassDefinitionNode)
    {
      SecurableClassDefinition securableClassDefinition = new SecurableClassDefinition (transaction);
      securableClassDefinition.Name = securableClassDefinitionNode.Attributes["name"].Value;
      securableClassDefinition.MetadataItemID = new Guid (securableClassDefinitionNode.Attributes["id"].Value);

      return securableClassDefinition;
    }

    private AbstractRoleDefinition CreateAbstractRoleDefinition (ClientTransaction transaction, XmlNode abstractRoleDefinitionNode)
    {
      AbstractRoleDefinition roleDefinition = new AbstractRoleDefinition ();
      roleDefinition.Name = abstractRoleDefinitionNode.Attributes["name"].Value;
      roleDefinition.MetadataItemID = new Guid (abstractRoleDefinitionNode.Attributes["id"].Value);
      roleDefinition.Value = long.Parse (abstractRoleDefinitionNode.Attributes["value"].Value);

      return roleDefinition;
    }
  }
}
