using System;
using System.Xml;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Configuration.Loader
{
public class RelationDefinitionLoader
{
  // types

  // static members and constants

  // member fields

  private XmlNodeList _relationNodeList;
  private ConfigurationNamespaceManager _namespaceManager;
  private ClassDefinitionCollection _classDefinitions;

  // construction and disposing

  public RelationDefinitionLoader (
      XmlDocument document, 
      ConfigurationNamespaceManager namespaceManager,
      ClassDefinitionCollection classDefinitions)
  {
    ArgumentUtility.CheckNotNull ("document", document);
    ArgumentUtility.CheckNotNull ("namespaceManager", namespaceManager);
    ArgumentUtility.CheckNotNull ("classDefinitions", classDefinitions);

    string xPath = namespaceManager.FormatXPath (
        "{0}:mapping/{0}:relations/{0}:relation", 
        PrefixNamespace.MappingNamespace.Uri);
    
    XmlNodeList relationNodeList = document.SelectNodes (xPath, namespaceManager);

    _relationNodeList = relationNodeList;
    _namespaceManager = namespaceManager;
    _classDefinitions = classDefinitions;
  }

  // methods and properties

  private string FormatXPath (string xPath)
  {
    return _namespaceManager.FormatXPath (xPath, PrefixNamespace.MappingNamespace.Uri);
  }

  public RelationDefinitionCollection GetRelationDefinitions ()
  {
    RelationDefinitionCollection relationDefinitions = new RelationDefinitionCollection ();

    foreach (XmlNode relationNode in _relationNodeList)
    {
      RelationDefinition relationDefinition = GetRelationDefinition (relationNode);
      relationDefinitions.Add (relationDefinition);
    }
    
    return relationDefinitions;
  }

  private RelationDefinition GetRelationDefinition (XmlNode relationNode)
  {
    string relationDefinitionID = relationNode.SelectSingleNode ("@id", _namespaceManager).InnerText;

    XmlNodeList endPointNodeList = relationNode.SelectNodes (
        FormatXPath ("{0}:virtualEndPoint | {0}:endPoint"), _namespaceManager);

    if (endPointNodeList.Count != 2)
      throw CreateMappingException ("Relation '{0}' does not have exactly two end points.", relationDefinitionID);

    RelationDefinition newRelationDefinition = new RelationDefinition (
        relationDefinitionID,
        GetEndPointDefinition (relationDefinitionID, endPointNodeList[0]),
        GetEndPointDefinition (relationDefinitionID, endPointNodeList[1]));

    AddRelationDefinitionToClassDefinitions (newRelationDefinition);
    return newRelationDefinition;
  }

  private void AddRelationDefinitionToClassDefinitions (RelationDefinition relationDefinition)
  {
    IRelationEndPointDefinition endPoint1 = relationDefinition.EndPointDefinitions[0];
    IRelationEndPointDefinition endPoint2 = relationDefinition.EndPointDefinitions[1];

    endPoint1.ClassDefinition.RelationDefinitions.Add (relationDefinition);

    if (endPoint1.ClassDefinition != endPoint2.ClassDefinition)
      endPoint2.ClassDefinition.RelationDefinitions.Add (relationDefinition);
  }

  private IRelationEndPointDefinition GetEndPointDefinition (string relationDefinitionID, XmlNode endPointNode)
  {
    string classAsString = endPointNode.SelectSingleNode (FormatXPath (
        "{0}:class/@id"), _namespaceManager).InnerText;

    ClassDefinition classDefinition = _classDefinitions.GetByClassID (classAsString);

    bool isMandatory = bool.Parse (endPointNode.SelectSingleNode ("@isMandatory", _namespaceManager).InnerText);

    XmlNode propertyNode = endPointNode.SelectSingleNode (FormatXPath ("{0}:property"), _namespaceManager);
    string propertyName = propertyNode.SelectSingleNode ("@name", _namespaceManager).InnerText;

    if (endPointNode.LocalName == "virtualEndPoint")
    {
      CardinalityType cardinality = GetCardinality (endPointNode);
      Type propertyType = GetTypeFromPropertyNode (propertyNode, cardinality, relationDefinitionID);

      return new VirtualRelationEndPointDefinition (
          classDefinition, propertyName, isMandatory, cardinality, propertyType);
    }
    else
    {
      return new RelationEndPointDefinition (classDefinition, propertyName, isMandatory);
    }
  }

  private Type GetTypeFromPropertyNode (XmlNode propertyNode, CardinalityType cardinality, string relationDefinitionID)
  {
    XmlNode typeNode = propertyNode.SelectSingleNode (FormatXPath ("{0}:collectionType"), _namespaceManager);

    if (typeNode != null)
    {
      if (cardinality == CardinalityType.One)
      {
        throw CreateMappingException (
            "Virtual end point of relation '{0}' must not contain element 'collectionType'."
                + " Element 'collectionType' is only valid for virtual end points with cardinality equal 'many'.",
            relationDefinitionID);
      }
      else
      {
        return LoaderUtility.MapType (typeNode);
      }
    }
    else
    {
      if (cardinality == CardinalityType.One)
      {
        string oppositeClassID = 
            propertyNode.SelectSingleNode (FormatXPath ("../../{0}:endPoint/{0}:class/@id"), _namespaceManager).InnerText;

        string xPath = FormatXPath ("/{0}:mapping/{0}:classes/{0}:class[@id = \"" + oppositeClassID + "\"]/{0}:type");
        return LoaderUtility.MapType (propertyNode, FormatXPath (xPath), _namespaceManager);
      }
      else
      {
        return typeof (DomainObjectCollection);
      }
    }
  }

  private CardinalityType GetCardinality (XmlNode endPointNode)
  {
    string cardinality = endPointNode.SelectSingleNode ("@cardinality", _namespaceManager).InnerText;
    if (cardinality == "one")
      return CardinalityType.One;
    else
      return CardinalityType.Many; 
  }

  private MappingException CreateMappingException (string message, params object[] args)
  {
    return new MappingException (string.Format (message, args));
  }
}
}