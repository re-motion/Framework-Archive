using System;

using Rubicon.Data.DomainObjects.Configuration.Loader;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Configuration.Queries
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

  // member fields

  private QueryDefinitionCollection _queryDefinitions;

  // construction and disposing

  private QueryConfiguration (QueryConfigurationLoader loader)
  {
    _queryDefinitions = loader.GetQueryDefinitions ();
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
}
}
