using System;
using System.Xml;
using System.Xml.Schema;

using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.Queries.Configuration;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader
{
public class QueryConfigurationLoader : BaseLoader
{
  // types

  // static members and constants

  public const string ConfigurationAppSettingKey = 
      "Rubicon.Data.DomainObjects.Queries.Configuration.ConfigurationFile";

  public const string SchemaAppSettingKey = "Rubicon.Data.DomainObjects.Queries.Configuration.SchemaFile";

  private const string c_defaultConfigurationFile = "queries.xml";
  private const string c_defaultSchemaFile = "queries.xsd";

  public static QueryConfigurationLoader Create ()
  {
    return new QueryConfigurationLoader (
        LoaderUtility.GetXmlFileName (ConfigurationAppSettingKey, c_defaultConfigurationFile),
        LoaderUtility.GetXmlFileName (SchemaAppSettingKey, c_defaultSchemaFile));
  }

  // member fields

  // construction and disposing

  public QueryConfigurationLoader (string configurationFile, string schemaFile)
  {
    try
    {
      base.Initialize (
          configurationFile, 
          schemaFile, 
          new PrefixNamespace[] {PrefixNamespace.QueryConfigurationNamespace}, 
          PrefixNamespace.QueryConfigurationNamespace);
    }
    catch (XmlSchemaException e)
    {
      throw CreateQueryConfigurationException (
          e, "Error while reading query configuration: {0}", e.Message);
    }
    catch (XmlException e)
    {
      throw CreateQueryConfigurationException (
          e, "Error while reading storage provider configuration: {0}", e.Message);
    }
  }
  
  // methods and properties

  public QueryDefinitionCollection GetQueryDefinitions ()
  {
    QueryDefinitionCollection queries = new QueryDefinitionCollection ();
    FillQueryDefinitions (queries);
    return queries;
  }

  private void FillQueryDefinitions (QueryDefinitionCollection queries)
  {
    XmlNodeList queryNodeList = Document.SelectNodes (FormatXPath (
        "{0}:queries/{0}:query"), NamespaceManager);

    foreach (XmlNode queryNode in queryNodeList)
      queries.Add (GetQueryDefinition (queryNode));
  }

  private QueryDefinition GetQueryDefinition (XmlNode queryNode)
  {
    string queryID = queryNode.SelectSingleNode ("@id", NamespaceManager).InnerText;
    string queryTypeAsString = queryNode.SelectSingleNode ("@type", NamespaceManager).InnerText;
    QueryType queryType = (QueryType) Enum.Parse (typeof (QueryType), queryTypeAsString, true);
    
    string storageProviderID = queryNode.SelectSingleNode (FormatXPath (
        "{0}:storageProviderID"), NamespaceManager).InnerText;
    
    string statement = queryNode.SelectSingleNode (FormatXPath (
        "{0}:statement"), NamespaceManager).InnerText;

    Type collectionType = LoaderUtility.GetOptionalType (queryNode, 
        FormatXPath ("{0}:collectionType"), NamespaceManager);

    if (queryType == QueryType.Scalar && collectionType != null)
      throw CreateQueryConfigurationException ("A scalar query '{0}' must not specify a collectionType.", queryID);

    return new QueryDefinition (queryID, storageProviderID, statement, queryType, collectionType);
  }

  private string FormatXPath (string xPath)
  {
    return NamespaceManager.FormatXPath (xPath, PrefixNamespace.QueryConfigurationNamespace.Uri);
  }

  private QueryConfigurationException CreateQueryConfigurationException (
      string message, 
      params object[] args)
  {
    return CreateQueryConfigurationException (null, message, args);
  }

  private QueryConfigurationException CreateQueryConfigurationException (
      Exception inner,
      string message, 
      params object[] args)
  {
    return new QueryConfigurationException (string.Format (message, args), inner);
  }
}
}
