using System;
using System.Collections;
using System.Collections.Specialized;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Queries.Configuration
{
public class QueryDefinitionCollection : CollectionBase
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public QueryDefinitionCollection ()
  {
  }

  // standard constructor for collections
  public QueryDefinitionCollection (
      QueryDefinitionCollection collection,
      bool makeCollectionReadOnly)
  {
    ArgumentUtility.CheckNotNull ("collection", collection);

    foreach (QueryDefinition queryDefinition in collection)  
    {
      Add (queryDefinition);
    }

    this.SetIsReadOnly (makeCollectionReadOnly);
  }

  // methods and properties

  public QueryDefinition GetMandatory (string queryID)
  {
    if (!Contains (queryID))
      throw CreateQueryConfigurationException ("QueryDefinition '{0}' does not exist.", queryID);

    return this[queryID];
  }

  private ArgumentException CreateArgumentException (string message, string parameterName, params object[] args)
  {
    return new ArgumentException (string.Format (message, args), parameterName);
  }

  private QueryConfigurationException CreateQueryConfigurationException (
      string message, 
      params object[] args)
  {
    return new QueryConfigurationException (string.Format (message, args));
  }

  #region Standard implementation for "add-only" collections

  public bool Contains (QueryDefinition queryDefinition)
  {
    ArgumentUtility.CheckNotNull ("queryDefinition", queryDefinition);

    return Contains (queryDefinition.QueryID);
  }

  public bool Contains (string queryID)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("queryID", queryID);
    return base.ContainsKey (queryID);
  }

  public QueryDefinition this [int index]  
  {
    get { return (QueryDefinition) GetObject (index); }
  }

  public QueryDefinition this [string queryID]  
  {
    get 
    {
      ArgumentUtility.CheckNotNullOrEmpty ("queryID", queryID);
      return (QueryDefinition) GetObject (queryID); 
    }
  }

  public void Add (QueryDefinition value)  
  {
    ArgumentUtility.CheckNotNull ("value", value);

    if (Contains (value.QueryID))
    {
      throw CreateArgumentException (
          "QueryDefinition '{0}' already exists in collection.", "value", value.QueryID);
    }

    base.Add (value.QueryID, value);
  }

  #endregion
}
}
