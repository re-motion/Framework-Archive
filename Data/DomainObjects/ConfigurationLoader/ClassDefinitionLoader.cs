using System;
using System.Collections;
using System.Xml;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader
{
public class ClassDefinitionLoader
{
  // types

  // static members and constants

  // member fields

  private XmlDocument _document;
  private XmlNodeList _classNodeList;
  private ConfigurationNamespaceManager _namespaceManager;
  private bool _resolveTypes;

  // construction and disposing

  public ClassDefinitionLoader (XmlDocument document, ConfigurationNamespaceManager namespaceManager, bool resolveTypes)
  {
    ArgumentUtility.CheckNotNull ("document", document);
    ArgumentUtility.CheckNotNull ("namespaceManager", namespaceManager);

    string xPath = namespaceManager.FormatXPath (
        "{0}:mapping/{0}:classes/{0}:class", PrefixNamespace.MappingNamespace.Uri);
    
    XmlNodeList classNodeList = document.SelectNodes (xPath, namespaceManager);
    if (classNodeList == null) throw CreateMappingException ("No class nodes were found in provided document.");

    _document = document;
    _classNodeList = classNodeList;
    _namespaceManager = namespaceManager;
    _resolveTypes = resolveTypes;
  }

  // methods and properties

  public bool ResolveTypes
  {
    get { return _resolveTypes; }
  }

  private string FormatXPath (string xPath)
  {
    return _namespaceManager.FormatXPath (xPath, PrefixNamespace.MappingNamespace.Uri);
  }

  public ClassDefinitionCollection GetClassDefinitions ()
  {
    ClassDefinitionCollection classDefinitions = new ClassDefinitionCollection (_resolveTypes);

    foreach (XmlNode classNode in _classNodeList)
    {
      ClassDefinition classDefinition = GetClassDefinition (classNode);
      classDefinitions.Add (classDefinition);
    }
    
    SetBaseClasses (classDefinitions);

    return classDefinitions;
  }

  private void SetBaseClasses (ClassDefinitionCollection classDefinitions)
  {
    XmlNodeList classNodesWithBaseClass = _document.SelectNodes (FormatXPath (
        "{0}:mapping/{0}:classes/{0}:class[@baseClass]"), _namespaceManager);

    foreach (XmlNode classNodeWithBaseClass in classNodesWithBaseClass)
    {
      ClassDefinition derivedClass = classDefinitions[classNodeWithBaseClass.SelectSingleNode ("@id").InnerText];

      string baseClassID = classNodeWithBaseClass.SelectSingleNode ("@baseClass").InnerText;
      ClassDefinition baseClass = classDefinitions[baseClassID];

      if (baseClass == null)
        throw CreateMappingException ("Class '{0}' refers to non-existing base class '{1}'.", derivedClass.ID, baseClassID);

      derivedClass.SetBaseClass (baseClass);
    }
  }

  private ClassDefinition GetClassDefinition (XmlNode classNode)
  {
    string id = classNode.SelectSingleNode ("@id", _namespaceManager).InnerText;
    string entityName = GetEntityName (classNode);

    string storageProviderID = classNode.SelectSingleNode (FormatXPath ("{0}:storageProviderID"), _namespaceManager).InnerText;
    string classTypeName = classNode.SelectSingleNode (FormatXPath ("{0}:type"), _namespaceManager).InnerText.Trim ();

    ClassDefinition classDefinition;
    if (_resolveTypes)
      classDefinition = new ClassDefinition (id, entityName, storageProviderID, LoaderUtility.GetType (classTypeName));
    else
      classDefinition = new ClassDefinition (id, entityName, storageProviderID, classTypeName, _resolveTypes);

    FillPropertyDefinitions (classDefinition, classNode);

    return classDefinition;
  }

  private string GetEntityName (XmlNode classNode)
  {
    XmlNode entityNode = classNode.SelectSingleNode (FormatXPath ("{0}:entity/@name"), _namespaceManager);
    if (entityNode != null)
      return entityNode.InnerText;

    return null;
  }

  private void FillPropertyDefinitions (ClassDefinition classDefinition, XmlNode classNode)
  {
    foreach (XmlNode propertyNode in classNode.SelectNodes (FormatXPath ("{0}:properties/{0}:property"), _namespaceManager))
    {
      PropertyDefinition propertyDefinition = GetPropertyDefinition (classDefinition, propertyNode);
      classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
    }

    foreach (XmlNode relationPropertyNode in classNode.SelectNodes (FormatXPath ("{0}:properties/{0}:relationProperty[{0}:column]"), _namespaceManager))
    {
      PropertyDefinition propertyDefinition = GetRelationPropertyDefinition (classDefinition, relationPropertyNode);
      classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
    }
  }

  private PropertyDefinition GetPropertyDefinition (ClassDefinition classDefinition, XmlNode propertyNode)
  {
    string propertyName = propertyNode.SelectSingleNode ("@name", _namespaceManager).InnerText;

    string columnName = propertyName;
    XmlNode columnNameNode = propertyNode.SelectSingleNode (FormatXPath ("{0}:column"), _namespaceManager);
    if (columnNameNode != null)
      columnName = columnNameNode.InnerText;
    
    string mappingType = propertyNode.SelectSingleNode (FormatXPath ("{0}:type"), _namespaceManager).InnerText.Trim ();

    bool isNullable = false;
    XmlNode isNullableNode = propertyNode.SelectSingleNode (FormatXPath ("{0}:nullable"), _namespaceManager);
    if (isNullableNode != null)
      isNullable = bool.Parse (isNullableNode.InnerText);

    NaInt32 maxLength = NaInt32.Null;
    XmlNode maxLengthNode = propertyNode.SelectSingleNode (FormatXPath ("{0}:maxLength"), _namespaceManager);
    if (maxLengthNode != null)
      maxLength = NaInt32.Parse (maxLengthNode.InnerText);

    return new PropertyDefinition (propertyName, columnName, mappingType, _resolveTypes, isNullable, maxLength);
  }

  private PropertyDefinition GetRelationPropertyDefinition (ClassDefinition classDefinition, XmlNode propertyNode)
  {
    string propertyName = propertyNode.SelectSingleNode ("@name", _namespaceManager).InnerText;
    string columnName = propertyNode.SelectSingleNode (FormatXPath ("{0}:column"), _namespaceManager).InnerText;

    return new PropertyDefinition (propertyName, columnName, TypeInfo.ObjectIDMappingTypeName, _resolveTypes, true, NaInt32.Null);
  }

  private MappingException CreateMappingException (string message, params object[] args)
  {
    return new MappingException (string.Format (message, args));
  }
}
}
