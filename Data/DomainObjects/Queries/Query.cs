using System;

using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Queries
{
/// <summary>
/// Represents a default implementation of <see cref="IQuery"/>.
/// </summary>
public class Query : IQuery
{
  // types

  // static members and constants

  // member fields

  private QueryDefinition _definition;
  private QueryParameterCollection _parameters;

  // construction and disposing

  /// <summary>
  /// Initializes a new instance of the <see cref="Query"/> class using a pre-defined query.
  /// </summary>
  /// <param name="queryID">The <i>queryID</i> of the query definition from queries.xml to use.</param>
  /// <exception cref="Configuration.QueryConfigurationException"><i>queryID</i> could not be found in the <see cref="Configuration.QueryConfiguration"/>.</exception>
  public Query (string queryID) : this (queryID, new QueryParameterCollection ()) 
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Query"/> class using a pre-defined query and a given collection of <see cref="QueryParameter"/>s.
  /// </summary>
  /// <param name="queryID">The <i>queryID</i> of the query definition from queries.xml to use.</param>
  /// <param name="parameters">The <see cref="QueryParameter"/>s to use to execute the query.</param>
  /// <exception cref="Configuration.QueryConfigurationException"><i>queryID</i> could not be found in the <see cref="Configuration.QueryConfiguration"/>.</exception>
  /// <exception cref="System.ArgumentNullException">
  ///   <i>parameters</i> is a null referecne.
  /// </exception>
  public Query (string queryID, QueryParameterCollection parameters) 
      : this (QueryConfiguration.Current.QueryDefinitions.GetMandatory (queryID), parameters)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Query"/> class using a <see cref="Configuration.QueryDefinition"/>.
  /// </summary>
  /// <param name="definition">The <see cref="Configuration.QueryDefinition"/> to use for the query.</param>
  /// <exception cref="System.ArgumentNullException">
  ///   <i>definition</i> is a null reference.
  /// </exception>
  public Query (QueryDefinition definition) : this (definition, new QueryParameterCollection ())
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Query"/> class using a <see cref="Configuration.QueryDefinition"/> and a given collection of <see cref="QueryParameter"/>s.
  /// </summary>
  /// <param name="definition">The <see cref="Configuration.QueryDefinition"/> to use for the query.</param>
  /// <param name="parameters">The <see cref="QueryParameter"/>s to use for executing the query.</param>
  /// <exception cref="System.ArgumentNullException">
  ///   <i>definition</i> is a null reference.<br /> -or- <br />
  ///   <i>parameters</i> is a null referecne.
  /// </exception>
  public Query (QueryDefinition definition, QueryParameterCollection parameters)
  {
    ArgumentUtility.CheckNotNull ("definition", definition);
    ArgumentUtility.CheckNotNull ("parameters", parameters);

    _definition = definition;
    _parameters = parameters;
  }

  // methods and properties

  /// <summary>
  /// Gets the <see cref="Configuration.QueryDefinition"/> that is associated with the query.
  /// </summary>
  public QueryDefinition Definition 
  {
    get { return _definition; }
  }

  /// <summary>
  /// Gets the <see cref="Configuration.QueryDefinition.QueryID"/> of the associated <see cref="Configuration.QueryDefinition"/>.
  /// </summary>
  public string QueryID
  {
    get { return _definition.QueryID; }
  }

  /// <summary>
  /// Gets the <see cref="Configuration.QueryDefinition.CollectionType"/> of the associated <see cref="Configuration.QueryDefinition"/>.
  /// </summary>
  public Type CollectionType 
  {
    get { return _definition.CollectionType; }
  }

  /// <summary>
  /// Gets the <see cref="Configuration.QueryDefinition.QueryType"/> of the associated <see cref="Configuration.QueryDefinition"/>.
  /// </summary>
  public QueryType QueryType 
  {
    get { return _definition.QueryType; }
  }

  /// <summary>
  /// Gets the <see cref="Configuration.QueryDefinition.Statement"/> of the associated <see cref="Configuration.QueryDefinition"/>.
  /// </summary>
  public string Statement 
  {
    get { return _definition.Statement; }
  }

  /// <summary>
  /// Gets the <see cref="Configuration.QueryDefinition.StorageProviderID"/> of the associated <see cref="Configuration.QueryDefinition"/>.
  /// </summary>
  public string StorageProviderID
  {
    get { return _definition.StorageProviderID; }
  }

  /// <summary>
  /// Gets the <see cref="QueryParameter"/>s that are used to execute the <see cref="Query"/>.
  /// </summary>
  public QueryParameterCollection Parameters
  {
    get { return _parameters; }
  }
}
}
