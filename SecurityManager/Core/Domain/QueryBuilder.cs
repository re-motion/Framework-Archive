using System;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain
{
  public abstract class QueryBuilder
  {
    // types

    // static members and constants

    // member fields

    private string _queryName;
    private Type _domainObjectResultType;
    private QueryType _queryType = QueryType.Collection;
    private Type _collectionType = typeof (DomainObjectCollection);
    private QueryParameterCollection _parameters;

    // construction and disposing

    protected QueryBuilder (string queryName, Type domainObjectResultType)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("queryName", queryName);
      ArgumentUtility.CheckNotNull ("domainObjectResultType", domainObjectResultType);

      _queryName = queryName;
      _domainObjectResultType = domainObjectResultType;
      _parameters = new QueryParameterCollection ();
    }

    // methods and properties

    public QueryType QueryType
    {
      get { return _queryType; }
    }

    public Type CollectionType
    {
      get { return _collectionType; }
      set { _collectionType = value; }
    }

    protected QueryParameterCollection Parameters
    {
      get { return _parameters; }
    }

    protected virtual Query CreateQueryFromStatement (string statement)
    {
      string storageProviderID = GetStorageProviderID ();
      QueryDefinition queryDefinition = new QueryDefinition (_queryName, storageProviderID, statement, _queryType, _collectionType);
      return new Query (queryDefinition, _parameters);
    }

    protected string GetStorageProviderID ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (_domainObjectResultType);
      return classDefinition.StorageProviderID;
    }
  }
}
