using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Rubicon.Data.DomainObjects;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.Metadata
{
  public class MetadataImporter
  {
    private delegate T CreateItemDelegate<T> (XmlNamespaceManager namespaceManager, XmlNode itemNode) where T: MetadataObject;

    private DomainObjectCollection _importedObjects;
    private ClientTransaction _clientTransaction;

    public MetadataImporter (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      _clientTransaction = transaction;
      _importedObjects = new DomainObjectCollection ();
    }

    public DomainObjectCollection ImportedObjects
    {
      get { return _importedObjects; }
    }

    public void Import (XmlDocument metadataXmlDocument)
    {
      XmlNamespaceManager namespaceManager = new XmlNamespaceManager (metadataXmlDocument.NameTable);
      // TODO: namespace should be extracted to only one location!
      namespaceManager.AddNamespace ("md", "http://www.rubicon-it.com/Security/Metadata/1.0");

      AddItem<SecurableClassDefinition> (metadataXmlDocument, "/md:securityMetadata/md:classes/md:class", namespaceManager, CreateSecurableClassDefinition);
      AddItem<StatePropertyDefinition> (metadataXmlDocument, "/md:securityMetadata/md:stateProperties/md:stateProperty", namespaceManager, CreateStatePropertyDefinition);
      AddItem<AbstractRoleDefinition> (metadataXmlDocument, "/md:securityMetadata/md:abstractRoles/md:abstractRole", namespaceManager, CreateAbstractRoleDefinition);
      AddItem<AccessTypeDefinition> (metadataXmlDocument, "/md:securityMetadata/md:accessTypes/md:accessType", namespaceManager, CreateAccessTypeDefinition);
    }

    private void AddItem<T> (
        XmlNode parentNode,
        string xpath,
        XmlNamespaceManager namespaceManager,
        CreateItemDelegate<T> createItemDelegate) where T : MetadataObject
    {
      AddItem<T> (_importedObjects, parentNode, xpath, namespaceManager, createItemDelegate);
    }

    private void AddItem<T> (
        DomainObjectCollection collection,
        XmlNode parentNode, 
        string xpath, 
        XmlNamespaceManager namespaceManager, 
        CreateItemDelegate<T> createItemDelegate) where T: MetadataObject
    {
      XmlNodeList itemNodes = parentNode.SelectNodes (xpath, namespaceManager);
      foreach (XmlNode itemNode in itemNodes)
        collection.Add (createItemDelegate (namespaceManager, itemNode));
    }

    private SecurableClassDefinition CreateSecurableClassDefinition (XmlNamespaceManager namespaceManager, XmlNode securableClassDefinitionNode)
    {
      SecurableClassDefinition securableClassDefinition = new SecurableClassDefinition (_clientTransaction);
      securableClassDefinition.Name = securableClassDefinitionNode.Attributes["name"].Value;
      securableClassDefinition.MetadataItemID = new Guid (securableClassDefinitionNode.Attributes["id"].Value);

      return securableClassDefinition;
    }

    private AbstractRoleDefinition CreateAbstractRoleDefinition (XmlNamespaceManager namespaceManager, XmlNode abstractRoleDefinitionNode)
    {
      AbstractRoleDefinition roleDefinition = new AbstractRoleDefinition (_clientTransaction);
      roleDefinition.Name = abstractRoleDefinitionNode.Attributes["name"].Value;
      roleDefinition.MetadataItemID = new Guid (abstractRoleDefinitionNode.Attributes["id"].Value);
      roleDefinition.Value = long.Parse (abstractRoleDefinitionNode.Attributes["value"].Value);

      return roleDefinition;
    }

    private AccessTypeDefinition CreateAccessTypeDefinition (XmlNamespaceManager namespaceManager, XmlNode accessTypeDefinitionNode)
    {
      AccessTypeDefinition accessTypeDefinition = new AccessTypeDefinition (_clientTransaction);
      accessTypeDefinition.Name = accessTypeDefinitionNode.Attributes["name"].Value;
      accessTypeDefinition.MetadataItemID = new Guid (accessTypeDefinitionNode.Attributes["id"].Value);
      accessTypeDefinition.Value = long.Parse (accessTypeDefinitionNode.Attributes["value"].Value);

      return accessTypeDefinition;
    }

    private StatePropertyDefinition CreateStatePropertyDefinition (XmlNamespaceManager namespaceManager, XmlNode statePropertyDefinitionNode)
    {
      StatePropertyDefinition statePropertyDefinition = new StatePropertyDefinition (_clientTransaction);
      statePropertyDefinition.Name = statePropertyDefinitionNode.Attributes["name"].Value;
      statePropertyDefinition.MetadataItemID = new Guid (statePropertyDefinitionNode.Attributes["id"].Value);

      AddItem<StateDefinition> (statePropertyDefinition.DefinedStates, statePropertyDefinitionNode, "md:state", namespaceManager, CreateStateDefinition);

      return statePropertyDefinition;
    }

    private StateDefinition CreateStateDefinition (XmlNamespaceManager namespaceManager, XmlNode stateDefinitionNode)
    {
      StateDefinition stateDefinition = new StateDefinition (_clientTransaction);
      stateDefinition.Name = stateDefinitionNode.Attributes["name"].Value;
      stateDefinition.Value = long.Parse (stateDefinitionNode.Attributes["value"].Value);

      return stateDefinition;
    }
  }
}
