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

      dataContainer.Rollback ();

      if (dataContainer.State == StateType.New)
        _dataContainers.Remove (dataContainer);
    }
  }

  public DomainObjectCollection GetNewDomainObjects ()
  {
    DomainObjectCollection newDomainObjects = new DomainObjectCollection ();
    foreach (DataContainer dataContainer in _dataContainers)
    {
      if (dataContainer.State == StateType.New)
        newDomainObjects.Add (dataContainer.DomainObject);
    }

    return newDomainObjects;
  }

  public DataContainerCollection MergeWithExisting (DataContainerCollection dataContainers)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

    return dataContainers.Combine (_dataContainers);
  }

  public DataContainerCollection GetNotExisting (DataContainerCollection dataContainers)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

    return dataContainers.GetDifference (_dataContainers);
  }

  #region IEnumerable Members

  public IEnumerator GetEnumerator ()
  {
    return _dataContainers.GetEnumerator ();
  }

  #endregion
}
}
