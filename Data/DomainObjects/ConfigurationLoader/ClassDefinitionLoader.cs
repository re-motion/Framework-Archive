using System;
using System.Xml;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Configuration.Loader
{
public class ClassDefinitionLoader
{
  // types

  // static members and constants

  // member fields

  private XmlDocument _document;
  private XmlNodeList _classNodeList;
  private ConfigurationNamespaceManager _namespaceManager;

  // construction and disposing

  public ClassDefinitionLoader (XmlDocument document, ConfigurationNamespaceManager namespaceManager)
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
  }

  // methods and properties

  private string FormatXPath (string xPath)
  {
    return _namespaceManager.FormatXPath (xPath, PrefixNamespace.MappingNamespace.Uri);
  }

  public ClassDefinitionCollection GetClassDefinitions ()
  {
    ClassDefinitionCollection classDefinitions = new ClassDefinitionCollection ();

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
      ClassDefinition derivedClass = classDefinitions.GetByClassID (
          classNodeWithBaseClass.SelectSingleNode ("@id").InnerText);

      string baseClassID = classNodeWithBaseClass.SelectSingleNode ("@baseClass").InnerText;
      ClassDefinition baseClass = classDefinitions.GetByClassID (baseClassID);

      if (baseClass == null)
        throw CreateMappingException ("Class '{0}' refers to non-existing base class '{1}'.", derivedClass.ID, baseClassID);

      derivedClass.SetBaseClass (baseClass);
    }
  }

  private ClassDefinition GetClassDefinition (XmlNode classNode)
  {
    string id = classNode.SelectSingleNode ("@id", _namespaceManager).InnerText;
    string entityName = classNode.SelectSingleNode (FormatXPath ("{0}:entity/@name"), _namespaceManager).InnerText;
    Type classType = LoaderUtility.GetType (classNode, FormatXPath ("{0}:type"), _namespaceManager);

    string storageProviderID = classNode.SelectSingleNode (FormatXPath (
        "{0}:storageProviderID"), _namespaceManager).InnerText;

    ClassDefinition classDefinition = new ClassDefinition (id, entityName, classType, storageProviderID);
    FillPropertyDefinitions (classDefinition, classNode);

    return classDefinition;
  }

  private void FillPropertyDefinitions (ClassDefinition classDefinition, XmlNode classNode)
  {
    foreach (XmlNode propertyNode in classNode.SelectNodes (
        FormatXPath ("{0}:properties/{0}:property"), _namespaceManager))
    {
      PropertyDefinition propertyDefinition = GetPropertyDefinition (classDefinition, propertyNode);
      classDefinition.PropertyDefinitions.Add (propertyDefinition);
    }
  }

  private PropertyDefinition GetPropertyDefinition (ClassDefinition classDefinition, XmlNode propertyNode)
  {
    string propertyName = propertyNode.SelectSingleNode ("@name", _namespaceManager).InnerText;
    string columnName = propertyNode.SelectSingleNode (FormatXPath ("{0}:column"), _namespaceManager).InnerText;
    string mappingType = propertyNode.SelectSingleNode (FormatXPath ("{0}:type"), _namespaceManager).InnerText;

    bool isNullable = false;
    XmlNode isNullableNode = propertyNode.SelectSingleNode (FormatXPath ("{0}:nullable"), _namespaceManager);
    if (isNullableNode != null)
      isNullable = bool.Parse (isNullableNode.InnerText);

    NaInt32 maxLength = NaInt32.Null;
    XmlNode maxLengthNode = propertyNode.SelectSingleNode (FormatXPath ("{0}:maxLength"), _namespaceManager);
    if (maxLengthNode != null)
      maxLength = NaInt32.Parse (maxLengthNode.InnerText);

    return new PropertyDefinition (propertyName, columnName, mappingType, isNullable, maxLength);
  }

  private MappingException CreateMappingException (string message, params object[] args)
  {
    return new MappingException (string.Format (message, args));
  }
}
}
