using System;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Queries
{
/// <summary>
/// <see cref="QueryManager"/> provides methods to execute queries within a <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/>.
/// </summary>
public class QueryManager
{
  // types

  // static members and constants

  // member fields

  private ClientTransaction _clientTransaction;

  // construction and disposing

  /// <summary>
  /// Initializes a new instance of the <see cref="QueryManager"/> class. 
  /// </summary>
  /// <remarks>
  /// All <see cref="DomainObject"/>s that are loaded by the <b>QueryManager</b> will exist within the given <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/>.
  /// </remarks>
  /// <param name="clientTransaction">The <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/> to be used in the <b>QueryManager</b>.</param>
  /// <exception cref="System.ArgumentNullException"><i>clientTransaction</i> is a null reference.</exception>
  public QueryManager (ClientTransaction clientTransaction)
  {
    ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

    _clientTransaction = clientTransaction;
  }

  // methods and properties

  /// <summary>
  /// Gets the <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/> that is associated with the <see cref="QueryManager"/>.
  /// </summary>
  public ClientTransaction ClientTransaction
  {
    get { return _clientTransaction; }
  }

  /// <summary>
  /// Executes a given <see cref="IQuery"/> and returns the scalar value.
  /// </summary>
  /// <param name="query">The query to execute.</param>
  /// <returns>The scalar value that is returned by the query.</returns>
  /// <exception cref="System.ArgumentNullException"><i>query</i> is a null reference.</exception>
  /// <exception cref="System.ArgumentException"><i>query</i> does not have a <see cref="Configuration.QueryType"/> of <see cref="Configuration.QueryType.Scalar"/>.</exception>
  /// <exception cref="Rubicon.Data.DomainObjects.Persistence.Configuration.StorageProviderConfigurationException">
  ///   The <see cref="IQuery.StorageProviderID"/> of <i>query</i> could not be found.
  /// </exception>
  /// <exception cref="Rubicon.Data.DomainObjects.Persistence.PersistenceException">
  ///   The <see cref="Rubicon.Data.DomainObjects.Persistence.StorageProvider"/> for the given <see cref="IQuery"/> could not be instantiated.
  /// </exception>
  /// <exception cref="Rubicon.Data.DomainObjects.Persistence.StorageProviderException">
  ///   An error occurred while executing the query.
  /// </exception>
  public object GetScalar (IQuery query)
  {
    ArgumentUtility.CheckNotNull ("query", query);

    if (query.QueryType == QueryType.Collection)
      throw new ArgumentException ("A collection query cannot be used with GetScalar.", "query");

    using (StorageProviderManager storageProviderManager = new StorageProviderManager ())
    {
      StorageProvider provider = storageProviderManager.GetMandatory (query.StorageProviderID);
      return provider.ExecuteScalarQuery (query);
    }
  }

  /// <summary>
  /// Executes a given <see cref="IQuery"/> and returns a collection of the <see cref="DomainObject"/>s returned by the query.
  /// </summary>
  /// <param name="query">The query to execute.</param>
  /// <returns>The scalar value that is returned by the query.</returns>
  /// <exception cref="System.ArgumentNullException"><i>query</i> is a null reference.</exception>
  /// <exception cref="System.ArgumentException"><i>query</i> does not have a <see cref="Configuration.QueryType"/> of <see cref="Configuration.QueryType.Collection"/>.</exception>
  /// <exception cref="Rubicon.Data.DomainObjects.Persistence.Configuration.StorageProviderConfigurationException">
  ///   The <see cref="IQuery.StorageProviderID"/> of <i>query</i> could not be found.
  /// </exception>
  /// <exception cref="Rubicon.Data.DomainObjects.Persistence.PersistenceException">
  ///   The <see cref="Rubicon.Data.DomainObjects.Persistence.StorageProvider"/> for the given <see cref="IQuery"/> could not be instantiated.
  /// </exception>
  /// <exception cref="Rubicon.Data.DomainObjects.Persistence.StorageProviderException">
  ///   An error occurred while executing the query.
  /// </exception>
  public DomainObjectCollection GetCollection (IQuery query)
  {
    ArgumentUtility.CheckNotNull ("query", query);

    if (query.QueryType == QueryType.Scalar)
      throw new ArgumentException ("A scalar query cannot be used with GetCollection.", "query");

    using (StorageProviderManager storageProviderManager = new StorageProviderManager ())
    {
      StorageProvider provider = storageProviderManager.GetMandatory (query.StorageProviderID);
      DataContainerCollection dataContainers = provider.ExecuteCollectionQuery (query);
      return _clientTransaction.MergeLoadedDomainObjects (dataContainers, query.CollectionType);  
    }    
  }
}
}
