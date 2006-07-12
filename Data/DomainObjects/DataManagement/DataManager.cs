using System;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.DataManagement
{
[Serializable]
public class DataManager
{
  // types

  // static members and constants

  // member fields

  private ClientTransaction _clientTransaction;
  private DataContainerMap _dataContainerMap;
  private RelationEndPointMap _relationEndPointMap;

  // construction and disposing

  public DataManager (ClientTransaction clientTransaction)
  {
    ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

    _clientTransaction = clientTransaction;
    _dataContainerMap = new DataContainerMap (clientTransaction);
    _relationEndPointMap = new RelationEndPointMap (clientTransaction);
  }

  // methods and properties

  public DataContainerCollection GetChangedDataContainersForCommit ()
  {
    DataContainerCollection changedDataContainers = new DataContainerCollection ();
    foreach (DomainObject domainObject in GetChangedDomainObjects ())
    {
      if (domainObject.State != StateType.Deleted)
        _relationEndPointMap.CheckMandatoryRelations (domainObject);

      if (domainObject.DataContainer.State != StateType.Unchanged)
        changedDataContainers.Add (domainObject.DataContainer);
    }

    return changedDataContainers;
  }

  public DomainObjectCollection GetChangedDomainObjects ()
  {
    return GetDomainObjects (new StateType[] { StateType.Changed, StateType.Deleted, StateType.New });
  }

  public DomainObjectCollection GetDomainObjects (StateType stateType)
  {
    ArgumentUtility.CheckValidEnumValue ("stateType", stateType);

    return GetDomainObjects (new StateType[] { stateType });
  }

  public DomainObjectCollection GetDomainObjects (StateType[] states)
  {
    DomainObjectCollection domainObjects = new DomainObjectCollection ();

    bool includeChanged = ContainsState (states, StateType.Changed);
    bool includeDeleted = ContainsState (states, StateType.Deleted);
    bool includeNew = ContainsState (states, StateType.New);
    bool includeUnchanged = ContainsState (states, StateType.Unchanged);

    foreach (DataContainer dataContainer in _dataContainerMap)
    {
      if (includeChanged && dataContainer.DomainObject.State == StateType.Changed)
        domainObjects.Add (dataContainer.DomainObject);

      if (includeDeleted && dataContainer.DomainObject.State == StateType.Deleted)
        domainObjects.Add (dataContainer.DomainObject);

      if (includeNew && dataContainer.DomainObject.State == StateType.New)
        domainObjects.Add (dataContainer.DomainObject);

      if (includeUnchanged && dataContainer.DomainObject.State == StateType.Unchanged)
        domainObjects.Add (dataContainer.DomainObject);
    }

    return domainObjects;
  }

  private bool ContainsState (StateType[] states, StateType state)
  {
    foreach (StateType arrayState in states)
    {
      if (arrayState == state)
        return true;
    }

    return false;
  }

  public void RegisterExistingDataContainers (DataContainerCollection dataContainers)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

    foreach (DataContainer dataContainer in dataContainers)
      RegisterExistingDataContainer (dataContainer); 
  }

  public void RegisterExistingDataContainer (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    _dataContainerMap.Register (dataContainer);
    _relationEndPointMap.RegisterExistingDataContainer (dataContainer);
  }

  public void RegisterNewDataContainer (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    _dataContainerMap.Register (dataContainer);
    _relationEndPointMap.RegisterNewDataContainer (dataContainer);
  }

  public void Commit ()
  {
    _relationEndPointMap.Commit (_dataContainerMap.GetByState (StateType.Deleted));
    _dataContainerMap.Commit ();
  }

  public void Rollback ()
  {
    _relationEndPointMap.Rollback (_dataContainerMap.GetByState (StateType.New));
    _dataContainerMap.Rollback ();
  }

  public DataContainerMap DataContainerMap
  {
    get { return _dataContainerMap; }
  }

  public RelationEndPointMap RelationEndPointMap
  {
    get { return _relationEndPointMap; }
  }

  public void Delete (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    CheckClientTransactionForDeletion (domainObject);

    if (domainObject.State == StateType.Deleted)
      return;

    RelationEndPointCollection allAffectedRelationEndPoints = _relationEndPointMap.GetAllRelationEndPointsWithLazyLoad (domainObject);
    BeginDelete (domainObject, allAffectedRelationEndPoints);

    RelationEndPointCollection allOppositeRelationEndPoints = allAffectedRelationEndPoints.GetOppositeRelationEndPoints (domainObject);
    PerformDelete (domainObject);

    EndDelete (domainObject, allOppositeRelationEndPoints);
  }

  private void PerformDelete (DomainObject domainObject)
  {
    _relationEndPointMap.PerformDelete (domainObject);
    _dataContainerMap.PerformDelete (domainObject.DataContainer);
    domainObject.DataContainer.Delete ();
  }

  private void BeginDelete (DomainObject domainObject, RelationEndPointCollection allAffectedRelationEndPoints)
  {
    //TODO: Start here when implementing oldRelatedObject and NewRelatedObject on IClientTransactionExtension.RelationChanged () 
    //      and RelationChanged events of ClientTransaction and DomainObject
    _clientTransaction.ObjectDeleting (domainObject);
    allAffectedRelationEndPoints.NotifyClientTransactionOfBeginDelete (domainObject);

    domainObject.BeginDelete ();
    allAffectedRelationEndPoints.BeginDelete (domainObject);
  }

  private void EndDelete (DomainObject domainObject, RelationEndPointCollection allOppositeRelationEndPoints)
  {
    allOppositeRelationEndPoints.EndDelete ();
    domainObject.EndDelete ();

    allOppositeRelationEndPoints.NotifyClientTransactionOfEndDelete ();
    _clientTransaction.ObjectDeleted (domainObject);
  }

  private void CheckClientTransactionForDeletion (DomainObject domainObject)
  {
    if (domainObject.DataContainer.ClientTransaction != _clientTransaction)
    {
      throw CreateClientTransactionsDifferException (
          "Cannot delete DomainObject '{0}', because it belongs to a different ClientTransaction.",
          domainObject.ID);
    }
  }

  private ClientTransactionsDifferException CreateClientTransactionsDifferException (string message, params object[] args)
  {
    return new ClientTransactionsDifferException (string.Format (message, args));
  }
}
}
