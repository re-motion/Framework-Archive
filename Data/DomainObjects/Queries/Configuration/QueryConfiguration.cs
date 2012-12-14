using System;

using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Queries.Configuration
{
public class QueryConfiguration
{
  // types

  // static members and constants

  private static QueryConfiguration s_queryConfiguration;

  public static QueryConfiguration Current
  {
    get 
    {
      lock (typeof (QueryConfiguration))
      {
        if (s_queryConfiguration == null)
        {
          s_queryConfiguration = new QueryConfiguration (QueryConfigurationLoader.Create ());
        }
        
        return s_queryConfiguration;
      }
    }
  }

  public static void SetCurrent (QueryConfiguration queryConfiguration)
  {
    lock (typeof (QueryConfiguration))
    {
      s_queryConfiguration = queryConfiguration;
    }
  }

  // member fields

  private QueryDefinitionCollection _queryDefinitions;
  private string _configurationFile;
  private string _schemaFile;

  // construction and disposing

  public QueryConfiguration (string configurationFile, string schemaFile) 
      : this (new QueryConfigurationLoader (configurationFile, schemaFile))
  {
  }

  public QueryConfiguration (QueryConfigurationLoader loader)
  {
    ArgumentUtility.CheckNotNull ("loader", loader);

    _queryDefinitions = loader.GetQueryDefinitions ();
    _configurationFile = loader.ConfigurationFile;
    _schemaFile = loader.SchemaFile;
  }

  // methods and properties

  public QueryDefinition this [string queryID]
  {
    get 
    {
      ArgumentUtility.CheckNotNullOrEmpty ("queryID", queryID);
      return _queryDefinitions[queryID]; 
    }
  }

  public QueryDefinitionCollection QueryDefinitions
  {
    get { return _queryDefinitions; }
  }

  public string ConfigurationFile
  {
    get { return _configurationFile; }
  }

  public string SchemaFile
  {
    get { return _schemaFile; }
  }
}
}
