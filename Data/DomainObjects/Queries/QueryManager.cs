using System;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Queries
{
public class QueryManager
{
  // types

  // static members and constants

  // member fields

  private ClientTransaction _clientTransaction;

  // construction and disposing

  public QueryManager (ClientTransaction clientTransaction)
  {
    ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

    _clientTransaction = clientTransaction;
  }

  // methods and properties

  public ClientTransaction ClientTransaction
  {
    get { return _clientTransaction; }
  }

  public object GetScalar (Query query)
  {
    ArgumentUtility.CheckNotNull ("query", query);

    using (StorageProviderManager storageProviderManager = new StorageProviderManager ())
    {
      StorageProvider provider = storageProviderManager.GetMandatory (query.StorageProviderID);
      return provider.ExecuteScalarQuery (query);
    }
  }

  public DomainObjectCollection GetCollection (Query query)
  {
    ArgumentUtility.CheckNotNull ("query", query);

    using (StorageProviderManager storageProviderManager = new StorageProviderManager ())
    {
      StorageProvider provider = storageProviderManager.GetMandatory (query.StorageProviderID);
      DataContainerCollection dataContainers = provider.ExecuteCollectionQuery (query);
      return _clientTransaction.GetLoadedDomainObjects (dataContainers, query.CollectionType);  
    }    
  }
}
}
