using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Rubicon.Data.DomainObjects;
using Rubicon.Utilities;
using Rubicon.Security.Metadata;
using Rubicon.Security;

namespace Rubicon.SecurityManager.Domain.Metadata
{
  public class MetadataImporter
  {
    private delegate T CreateItemDelegate<T> (XmlNamespaceManager namespaceManager, XmlNode itemNode) where T: MetadataObject;

    private ClientTransaction _clientTransaction;
    private Dictionary<Guid, SecurableClassDefinition> _classes;
    private Dictionary<Guid, StatePropertyDefinition> _stateProperties;
    private Dictionary<Guid, AbstractRoleDefinition> _abstractRoles;
    private Dictionary<Guid, AccessTypeDefinition> _accessTypes;

    private Dictionary<Guid, Guid> _baseClassReferences;
    private Dictionary<Guid, List<Guid>> _statePropertyReferences;
    private Dictionary<Guid, List<Guid>> _accessTypeReferences;

    public MetadataImporter (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      _clientTransaction = transaction;
      _classes = new Dictionary<Guid, SecurableClassDefinition> ();
      _stateProperties = new Dictionary<Guid, StatePropertyDefinition> ();
      _abstractRoles = new Dictionary<Guid, AbstractRoleDefinition> ();
      _accessTypes = new Dictionary<Guid, AccessTypeDefinition> ();

      _baseClassReferences = new Dictionary<Guid, Guid> ();
      _statePropertyReferences = new Dictionary<Guid, List<Guid>> ();
      _accessTypeReferences = new Dictionary<Guid, List<Guid>> ();
    }

    public ClientTransaction Transaction
    {
      get { return _clientTransaction; }
    }

    public Dictionary<Guid, SecurableClassDefinition> Classes
    {
      get { return _classes; }
    }

    public Dictionary<Guid, StatePropertyDefinition> StateProperties
    {
      get { return _stateProperties; }
    }

    public Dictionary<Guid, AbstractRoleDefinition> AbstractRoles
    {
      get { return _abstractRoles; }
    }

    public Dictionary<Guid, AccessTypeDefinition> AccessTypes
    {
      get { return _accessTypes; }
    }

    public void Import (string metadataFilePath)
    {
      XmlDocument metadataXmlDocument = new XmlDocument ();
      metadataXmlDocument.Load (metadataFilePath);

      Import (metadataXmlDocument);
    }

    public void Import (XmlDocument metadataXmlDocument)
    {
      SecurityMetadataSchema metadataSchema = new SecurityMetadataSchema ();
      if (!metadataXmlDocument.Schemas.Contains (metadataSchema.SchemaUri))
        metadataXmlDocument.Schemas.Add (metadataSchema.LoadSchemaSet ());

      metadataXmlDocument.Validate (null);

      XmlNamespaceManager namespaceManager = new XmlNamespaceManager (metadataXmlDocument.NameTable);
      namespaceManager.AddNamespace ("md", metadataSchema.SchemaUri);

      AddItem (_classes, metadataXmlDocument, "/md:securityMetadata/md:classes/md:class", namespaceManager, CreateSecurableClassDefinition);
      AddItem (_stateProperties, metadataXmlDocument, "/md:securityMetadata/md:stateProperties/md:stateProperty", namespaceManager, CreateStatePropertyDefinition);
      AddItem (_abstractRoles, metadataXmlDocument, "/md:securityMetadata/md:abstractRoles/md:abstractRole", namespaceManager, CreateAbstractRoleDefinition);
      AddItem (_accessTypes, metadataXmlDocument, "/md:securityMetadata/md:accessTypes/md:accessType", namespaceManager, CreateAccessTypeDefinition);

      LinkDerivedClasses ();
      LinkStatePropertiesToClasses ();
      LinkAccessTypesToClasses ();
    }

    private void AddItem<T> (
        Dictionary<Guid, T> dictionary,
        XmlNode parentNode,
        string xpath,
        XmlNamespaceManager namespaceManager,
        CreateItemDelegate<T> createItemDelegate) where T : MetadataObject
    {
      XmlNodeList itemNodes = parentNode.SelectNodes (xpath, namespaceManager);
      foreach (XmlNode itemNode in itemNodes)
      {
        T item = createItemDelegate (namespaceManager, itemNode);
        dictionary.Add (item.MetadataItemID, item);
      }
    }

    private void LinkDerivedClasses ()
    {
      foreach (Guid classID in _baseClassReferences.Keys)
      {
        SecurableClassDefinition securableClass = _classes[classID];
        Guid baseClassID = _baseClassReferences[classID];

        if (!_classes.ContainsKey (baseClassID))
          throw new ImportException (string.Format ("The base class '{0}' referenced by the class '{1}' could not be found.", baseClassID, classID));

        SecurableClassDefinition baseClass = _classes[_baseClassReferences[classID]];
        securableClass.BaseClass = baseClass;
      }
    }

    private void LinkStatePropertiesToClasses ()
    {
      foreach (Guid classID in _statePropertyReferences.Keys)
      {
        List<Guid> statePropertyReferences = _statePropertyReferences[classID];

        foreach (Guid statePropertyID in statePropertyReferences)
        {
          if (!_stateProperties.ContainsKey (statePropertyID))
            throw new ImportException (string.Format ("The state property '{0}' referenced by the class '{1}' could not be found.", statePropertyID, classID));

          _classes[classID].AddStateProperty (_stateProperties[statePropertyID]);
        }
      }
    }

    private void LinkAccessTypesToClasses ()
    {
      foreach (Guid classID in _accessTypeReferences.Keys)
      {
        List<Guid> accessTypeReferences = _accessTypeReferences[classID];

        foreach (Guid accessTypeID in accessTypeReferences)
        {
          if (!_accessTypes.ContainsKey (accessTypeID))
            throw new ImportException (string.Format ("The access type '{0}' referenced by the class '{1}' could not be found.", accessTypeID, classID));

          _classes[classID].AddAccessType (_accessTypes[accessTypeID]);
        }
      }
    }

    private SecurableClassDefinition CreateSecurableClassDefinition (XmlNamespaceManager namespaceManager, XmlNode securableClassDefinitionNode)
    {
      SecurableClassDefinition securableClassDefinition = new SecurableClassDefinition (_clientTransaction);
      securableClassDefinition.Name = securableClassDefinitionNode.Attributes["name"].Value;
      securableClassDefinition.MetadataItemID = new Guid (securableClassDefinitionNode.Attributes["id"].Value);
      if (securableClassDefinitionNode.Attributes["base"] != null)
      {
        Guid baseClassID = new Guid (securableClassDefinitionNode.Attributes["base"].Value);
        _baseClassReferences.Add (securableClassDefinition.MetadataItemID, baseClassID);
      }

      CreateReferences (securableClassDefinition, securableClassDefinitionNode, namespaceManager, "md:stateProperties/md:statePropertyRef", _statePropertyReferences);
      CreateReferences (securableClassDefinition, securableClassDefinitionNode, namespaceManager, "md:accessTypes/md:accessTypeRef", _accessTypeReferences);

      return securableClassDefinition;
    }

    private void CreateReferences (
        SecurableClassDefinition securableClassDefinition, 
        XmlNode securableClassDefinitionNode,
        XmlNamespaceManager namespaceManager,
        string xpath,
        Dictionary<Guid, List<Guid>> referenceRegistry)
    {
      List<Guid> references = new List<Guid> ();
      XmlNodeList referenceNodes = securableClassDefinitionNode.SelectNodes (xpath, namespaceManager);

      foreach (XmlNode referenceNode in referenceNodes)
        references.Add (new Guid (referenceNode.InnerText));

      referenceRegistry.Add (securableClassDefinition.MetadataItemID, references);
    }

    private AbstractRoleDefinition CreateAbstractRoleDefinition (XmlNamespaceManager namespaceManager, XmlNode abstractRoleDefinitionNode)
    {
      AbstractRoleDefinition roleDefinition = new AbstractRoleDefinition (_clientTransaction);
      roleDefinition.Name = abstractRoleDefinitionNode.Attributes["name"].Value;
      roleDefinition.MetadataItemID = new Guid (abstractRoleDefinitionNode.Attributes["id"].Value);
      roleDefinition.Value = int.Parse (abstractRoleDefinitionNode.Attributes["value"].Value);

      return roleDefinition;
    }

    private AccessTypeDefinition CreateAccessTypeDefinition (XmlNamespaceManager namespaceManager, XmlNode accessTypeDefinitionNode)
    {
      AccessTypeDefinition accessTypeDefinition = new AccessTypeDefinition (_clientTransaction);
      accessTypeDefinition.Name = accessTypeDefinitionNode.Attributes["name"].Value;
      accessTypeDefinition.MetadataItemID = new Guid (accessTypeDefinitionNode.Attributes["id"].Value);
      accessTypeDefinition.Value = int.Parse (accessTypeDefinitionNode.Attributes["value"].Value);

      return accessTypeDefinition;
    }

    private StatePropertyDefinition CreateStatePropertyDefinition (XmlNamespaceManager namespaceManager, XmlNode statePropertyDefinitionNode)
    {
      StatePropertyDefinition statePropertyDefinition = new StatePropertyDefinition (_clientTransaction);
      statePropertyDefinition.Name = statePropertyDefinitionNode.Attributes["name"].Value;
      statePropertyDefinition.MetadataItemID = new Guid (statePropertyDefinitionNode.Attributes["id"].Value);

      XmlNodeList stateNodes = statePropertyDefinitionNode.SelectNodes ("md:state", namespaceManager);
      foreach (XmlNode stateNode in stateNodes)
        statePropertyDefinition.DefinedStates.Add (CreateStateDefinition (namespaceManager, stateNode));

      return statePropertyDefinition;
    }

    private StateDefinition CreateStateDefinition (XmlNamespaceManager namespaceManager, XmlNode stateDefinitionNode)
    {
      StateDefinition stateDefinition = new StateDefinition (_clientTransaction);
      stateDefinition.Name = stateDefinitionNode.Attributes["name"].Value;
      stateDefinition.Value = int.Parse (stateDefinitionNode.Attributes["value"].Value);

      return stateDefinition;
    }
  }
}
