using System;
using System.Xml;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader
{
public class RelationDefinitionLoader
{
  // types

  // static members and constants

  // member fields

  private XmlDocument _document;
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

    _document = document;
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
    CheckNumberOfEndPoints ();

    RelationDefinitionCollection relationDefinitions = new RelationDefinitionCollection ();
    foreach (XmlNode distinctRelationPropertyNode in GetDistinctRelationPropertyNodes ())
    {
      RelationDefinition relationDefinition = GetRelationDefinition (distinctRelationPropertyNode);
      relationDefinitions.Add (relationDefinition);
    }
    
    return relationDefinitions;
  }

  private void CheckNumberOfEndPoints ()
  {
    foreach (string relationDefinitionID in GetAllRelationDefinitionIDs ())
    {
      string xPath = FormatXPath ("/{0}:mapping/{0}:classes/{0}:class/{0}:properties/{0}:relationProperty[@relationID = '{relationDefinitionID}']");
      xPath = xPath.Replace ("{relationDefinitionID}", relationDefinitionID);

      if (_document.SelectNodes(xPath, _namespaceManager).Count != 2)
        throw CreateMappingException ("Relation '{0}' does not have exactly two end points.", relationDefinitionID);  
    }
  }

  private XmlNodeList GetDistinctRelationPropertyNodes ()
  {
    // Select distinct the first relationProperty of every relation (identified through @relationID):
    string xPath = FormatXPath ("/{0}:mapping/{0}:classes/{0}:class/{0}:properties/{0}:relationProperty[not (preceding::{0}:relationProperty/@relationID = ./@relationID)]");
    return _document.SelectNodes (xPath, _namespaceManager);
  }

  private string[] GetAllRelationDefinitionIDs ()
  {
    XmlNodeList distinctRelationPropertyNodes = GetDistinctRelationPropertyNodes ();
    string[] allRelationDefinitionIDs = new string[distinctRelationPropertyNodes.Count];
    for (int i = 0; i < distinctRelationPropertyNodes.Count; i++)
    {
      XmlNode distinctRelationPropertyNode = distinctRelationPropertyNodes[i];
      allRelationDefinitionIDs[i] = distinctRelationPropertyNode.SelectSingleNode ("@relationID").InnerText;
    }

    return allRelationDefinitionIDs;
  }

  private RelationDefinition GetRelationDefinition (XmlNode relationPropertyNode)
  {
    string relationDefinitionID = relationPropertyNode.SelectSingleNode ("@relationID", _namespaceManager).InnerText;
    string propertyName = GetPropertyName (relationPropertyNode);

    XmlNode oppositeRelationPropertyNode = GetOppositeRelationPropertyNode (relationDefinitionID, propertyName);
    string oppositePropertyName = GetPropertyName (oppositeRelationPropertyNode);


    RelationDefinition newRelationDefinition = new RelationDefinition (
        relationDefinitionID,
        GetEndPointDefinition (relationDefinitionID, propertyName, relationPropertyNode),
        GetEndPointDefinition (relationDefinitionID, oppositePropertyName, oppositeRelationPropertyNode));

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

  private IRelationEndPointDefinition GetEndPointDefinition (string relationDefinitionID, string propertyName, XmlNode relationPropertyNode)
  {
    string classAsString = relationPropertyNode.SelectSingleNode ("../../@id", _namespaceManager).InnerText;
    ClassDefinition classDefinition = _classDefinitions[classAsString];

    bool isMandatory = bool.Parse (relationPropertyNode.SelectSingleNode ("@isMandatory", _namespaceManager).InnerText);

    XmlNode columnNode = relationPropertyNode.SelectSingleNode (FormatXPath ("{0}:column"), _namespaceManager);
    bool isVirtualEndPoint = (columnNode == null);

    CardinalityType cardinality = GetCardinality (relationPropertyNode);
    if (isVirtualEndPoint)
    {
      Type propertyType = GetTypeFromVirtualRelationPropertyNode (relationDefinitionID, propertyName, relationPropertyNode, cardinality);
      string sortExpression = GetSortExpression (relationPropertyNode);

      return new VirtualRelationEndPointDefinition (
          classDefinition, propertyName, isMandatory, cardinality, propertyType, sortExpression);
    }
    else
    {
      if (cardinality == CardinalityType.Many)
      {
        throw CreateMappingException (
            "Property '{0}' of relation '{1}' defines a column and a cardinality equal to 'many', which is not valid.", propertyName, relationDefinitionID);
      }

      return new RelationEndPointDefinition (classDefinition, propertyName, isMandatory);
    }
  }

  private string GetSortExpression (XmlNode relationPropertyNode)
  {
    XmlNode sortExpressionNode = relationPropertyNode.SelectSingleNode (FormatXPath ("{0}:sortExpression"), _namespaceManager);

    string sortExpression = null;
    if (sortExpressionNode != null)
      sortExpression = sortExpressionNode.InnerText;

    return sortExpression;
  }

  private string GetPropertyName (XmlNode propertyNodeOrRelationPropertyNode)
  {
    return propertyNodeOrRelationPropertyNode.SelectSingleNode ("@name", _namespaceManager).InnerText;
  }

  private Type GetTypeFromVirtualRelationPropertyNode (string relationDefinitionID, string propertyName, XmlNode relationPropertyNode, CardinalityType cardinality)
  {
    XmlNode typeNode = relationPropertyNode.SelectSingleNode (FormatXPath ("{0}:collectionType"), _namespaceManager);

    if (typeNode != null)
    {
      if (cardinality == CardinalityType.One)
      {
        throw CreateMappingException (
            "RelationProperty '{0}' of relation '{1}' must not contain element 'collectionType'."
                + " Element 'collectionType' is only valid for relation properties with cardinality equal to 'many'.",
            propertyName, relationDefinitionID);
      }
      else
      {
        return LoaderUtility.GetType (typeNode);
      }
    }
    else
    {
      if (cardinality == CardinalityType.One)
        return GetOppositeClassType (relationDefinitionID, propertyName);
      else
        return typeof (DomainObjectCollection);
    }
  }

  private XmlNode GetOppositeRelationPropertyNode (string relationDefinitionID, string propertyName)
  {
    string xPath = FormatXPath ("/{0}:mapping/{0}:classes/{0}:class/{0}:properties/{0}:relationProperty[@relationID = '{relationDefinitionID}' and @name != '{propertyName}']");
    xPath = xPath.Replace ("{relationDefinitionID}", relationDefinitionID);
    xPath = xPath.Replace ("{propertyName}", propertyName);

    XmlNode oppositeRelationPropertyNode = _document.SelectSingleNode (xPath, _namespaceManager);

    // Note: It has already been checked that there are exactly two end points for this relation.
    if (oppositeRelationPropertyNode == null)
      throw CreateMappingException ("Both property names of relation '{0}' are '{1}', which is not valid.", relationDefinitionID, propertyName);

    return oppositeRelationPropertyNode;
  }

  private Type GetOppositeClassType (string relationDefinitionID, string propertyName)
  {
    XmlNode oppositeRelationPropertyNode = GetOppositeRelationPropertyNode (relationDefinitionID, propertyName);
    XmlNode typeNode = oppositeRelationPropertyNode.SelectSingleNode (FormatXPath ("../../{0}:type"), _namespaceManager);
    
    return LoaderUtility.GetType (typeNode);
  }

  private CardinalityType GetCardinality (XmlNode relationPropertyNode)
  {
    string cardinality = relationPropertyNode.SelectSingleNode ("@cardinality", _namespaceManager).InnerText;
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