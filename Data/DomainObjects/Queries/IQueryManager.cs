namespace Rubicon.Data.DomainObjects.Queries
{
  /// <summary>
  /// <see cref="IQueryManager"/> provides an interface for methods to execute queries within a <see cref="ClientTransaction"/>.
  /// </summary>
  public interface IQueryManager
  {
    /// <summary>
    /// Gets the <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/> that is associated with the <see cref="RootQueryManager"/>.
    /// </summary>
    ClientTransaction ClientTransaction { get; }

    /// <summary>
    /// Executes a given <see cref="IQuery"/> and returns the scalar value.
    /// </summary>
    /// <param name="query">The query to execute. Must not be <see langword="null"/>.</param>
    /// <returns>The scalar value that is returned by the query.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="query"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.ArgumentException"><paramref name="query"/> does not have a <see cref="Configuration.QueryType"/> of <see cref="Configuration.QueryType.Scalar"/>.</exception>
    /// <exception cref="Rubicon.Data.DomainObjects.Persistence.Configuration.StorageProviderConfigurationException">
    ///   The <see cref="IQuery.StorageProviderID"/> of <paramref name="query"/> could not be found.
    /// </exception>
    /// <exception cref="Rubicon.Data.DomainObjects.Persistence.PersistenceException">
    ///   The <see cref="Rubicon.Data.DomainObjects.Persistence.StorageProvider"/> for the given <see cref="IQuery"/> could not be instantiated.
    /// </exception>
    /// <exception cref="Rubicon.Data.DomainObjects.Persistence.StorageProviderException">
    ///   An error occurred while executing the query.
    /// </exception>
    object GetScalar (IQuery query);

    /// <summary>
    /// Executes a given <see cref="IQuery"/> and returns a collection of the <see cref="DomainObject"/>s returned by the query.
    /// </summary>
    /// <param name="query">The query to execute. Must not be <see langword="null"/>.</param>
    /// <returns>The scalar value that is returned by the query.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="query"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.ArgumentException"><paramref name="query"/> does not have a <see cref="Configuration.QueryType"/> of <see cref="Configuration.QueryType.Collection"/>.</exception>
    /// <exception cref="Rubicon.Data.DomainObjects.Persistence.Configuration.StorageProviderConfigurationException">
    ///   The <see cref="IQuery.StorageProviderID"/> of <paramref name="query"/> could not be found.
    /// </exception>
    /// <exception cref="Rubicon.Data.DomainObjects.Persistence.PersistenceException">
    ///   The <see cref="Rubicon.Data.DomainObjects.Persistence.StorageProvider"/> for the given <see cref="IQuery"/> could not be instantiated.
    /// </exception>
    /// <exception cref="Rubicon.Data.DomainObjects.Persistence.StorageProviderException">
    ///   An error occurred while executing the query.
    /// </exception>
    DomainObjectCollection GetCollection (IQuery query);
  }
}