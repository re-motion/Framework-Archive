using System;
using System.Collections;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public class DataContainerMap : IEnumerable
{
  // types

  // static members and constants

  // member fields

  private ClientTransaction _clientTransaction;
  private DataContainerCollection _dataContainers;

  // construction and disposing

  public DataContainerMap (ClientTransaction clientTransaction)
  {
    ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

    _clientTransaction = clientTransaction;
    _dataContainers = new DataContainerCollection ();
  }

  // methods and properties

  public DataContainer this[ObjectID id]
  {
    get { return _dataContainers[id]; }
  }

  public int Count
  {
    get { return _dataContainers.Count; }
  }

  public DomainObject GetObject (ObjectID id, bool includeDeleted)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    if (_dataContainers.Contains (id))
    {
      DataContainer dataContainer = this[id];

      if (!includeDeleted && dataContainer.State == StateType.Deleted)
        throw new ObjectDeletedException (id);

      return dataContainer.DomainObject;
    }

    return _clientTransaction.LoadObject (id);
  }

  public void Register (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    _dataContainers.Add (dataContainer);
  }

  public void PerformDelete (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
    CheckClientTransactionForDeletion (dataContainer);

    if (dataContainer.State == StateType.New)
      _dataContainers.Remove (dataContainer);    
  }

  public void Commit ()
  {
    for (int i = _dataContainers.Count - 1; i >= 0; i--)
    {
      DataContainer dataContainer = _dataContainers[i];
      
      if (dataContainer.State == StateType.Deleted)
        _dataContainers.Remove (i);

      dataContainer.Commit ();
    }
  }

  public void Rollback ()
  {
    for (int i = _dataContainers.Count - 1; i >= 0; i--)
    {
      DataContainer dataContainer = _dataContainers[i];

      if (dataContainer.State == StateType.New)
        _dataContainers.Remove (dataContainer);

      dataContainer.Rollback ();
    }
  }

  public DomainObjectCollection GetByState (StateType state)
  {
    ArgumentUtility.CheckValidEnumValue (state, "state");

    DomainObjectCollection domainObjects = new DomainObjectCollection ();

    DataContainerCollection dataContainers = _dataContainers.GetByState (state);
    foreach (DataContainer dataContainer in dataContainers)
      domainObjects.Add (dataContainer.DomainObject);

    return domainObjects;
  }

  public DataContainerCollection MergeWithRegisteredDataContainers (DataContainerCollection dataContainers)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

    return dataContainers.Merge (_dataContainers);
  }

  public DataContainerCollection GetNotRegisteredDataContainers (DataContainerCollection dataContainers)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

    return dataContainers.GetDifference (_dataContainers);
  }

  private void CheckClientTransactionForDeletion (DataContainer dataContainer)
  {
    if (dataContainer.ClientTransaction != _clientTransaction)
    {
      throw CreateClientTransactionsDifferException (
          "Cannot remove DataContainer '{0}' from DataContainerMap, because it belongs to a different ClientTransaction.",
          dataContainer.ID);
    }
  }

  private ClientTransactionsDifferException CreateClientTransactionsDifferException (string message, params object[] args)
  {
    return new ClientTransactionsDifferException (string.Format (message, args));
  }

  #region IEnumerable Members

  public IEnumerator GetEnumerator ()
  {
    return _dataContainers.GetEnumerator ();
  }

  #endregion
}
}
