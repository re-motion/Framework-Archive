using System;

using Rubicon.Data.DomainObjects.Configuration.Queries;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Queries
{
public class Query
{
  // types

  // static members and constants

  // member fields

  private QueryDefinition _definition;
  private QueryParameterCollection _parameters;

  // construction and disposing

  public Query (string queryID) : this (queryID, new QueryParameterCollection ()) 
  {
  }

  public Query (string queryID, QueryParameterCollection parameters) 
      : this (QueryConfiguration.Current.QueryDefinitions.GetMandatory (queryID), parameters)
  {
  }

  public Query (QueryDefinition definition) : this (definition, new QueryParameterCollection ())
  {
  }

  public Query (QueryDefinition definition, QueryParameterCollection parameters)
  {
    ArgumentUtility.CheckNotNull ("definition", definition);
    ArgumentUtility.CheckNotNull ("parameters", parameters);

    _definition = definition;
    _parameters = parameters;
  }

  // methods and properties

  public string QueryID
  {
    get { return _definition.QueryID; }
  }

  public QueryDefinition Definition 
  {
    get { return _definition; }
  }

  public QueryParameterCollection Parameters
  {
    get { return _parameters; }
  }
}
}
