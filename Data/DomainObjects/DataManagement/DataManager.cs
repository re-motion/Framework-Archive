using System;
using System.Collections.Generic;
using Rubicon.Collections;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Infrastructure;

namespace Rubicon.Data.DomainObjects.DataManagement
{
[Serializable]
public class DataManager
{
  // types

  // static members and constants

  // member fields

  private readonly ClientTransaction _clientTransaction;
  private readonly IClientTransactionListener _transactionEventSink;

  private readonly DataContainerMap _dataContainerMap;
  private readonly RelationEndPointMap _relationEndPointMap;
  private readonly Dictionary<ObjectID, DataContainer> _discardedDataContainers;

  // construction and disposing

  public DataManager (ClientTransaction clientTransaction)
  {
    ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

    _clientTransaction = clientTransaction;
    _transactionEventSink = clientTransaction.TransactionEventSink;
    _dataContainerMap = new DataContainerMap (clientTransaction);
    _relationEndPointMap = new RelationEndPointMap (clientTransaction);
    _discardedDataContainers = new Dictionary<ObjectID, DataContainer> ();
  }

  // methods and properties

  public DataContainerCollection GetChangedDataContainersForCommit ()
  {
    DataContainerCollection changedDataContainers = new DataContainerCollection ();
    foreach (DomainObject domainObject in GetChangedDomainObjects ())
    {
      if (domainObject.State != StateType.Deleted)
        _relationEndPointMap.CheckMandatoryRelations (domainObject);

      if (domainObject.GetDataContainer().State != StateType.Unchanged)
        changedDataContainers.Add (domainObject.GetDataContainer());
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

  public IEnumerable<RelationEndPoint> GetChangedRelationEndPoints ()
  {
    foreach (RelationEndPoint endPoint in _relationEndPointMap)
    {
      if (endPoint.HasChanged)
        yield return endPoint;
    }
  }

  private bool ContainsState (StateType[] states, StateType state)
  {
    return (Array.IndexOf (states, state) >= 0);
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
    //TODO later: Start here when implementing oldRelatedObject and NewRelatedObject on IClientTransactionExtension.RelationChanged () 
    //      and RelationChanged events of ClientTransaction and DomainObject
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    CheckClientTransactionForDeletion (domainObject);

    if (domainObject.State == StateType.Deleted)
      return;

    RelationEndPointCollection allAffectedRelationEndPoints = _relationEndPointMap.GetAllRelationEndPointsWithLazyLoad (domainObject);
    RelationEndPointCollection allOppositeRelationEndPoints = allAffectedRelationEndPoints.GetOppositeRelationEndPoints (domainObject);

    BeginDelete (domainObject, allAffectedRelationEndPoints, allOppositeRelationEndPoints);
    PerformDelete (domainObject);
    EndDelete (domainObject, allOppositeRelationEndPoints);
  }

  private void PerformDelete (DomainObject domainObject)
  {
    DataContainer dataContainer = domainObject.GetDataContainer ();  // rescue dataContainer before the map deletes is

    _relationEndPointMap.PerformDelete (domainObject);
    _dataContainerMap.PerformDelete (dataContainer);
    
    dataContainer.Delete ();
  }

  private void BeginDelete (
      DomainObject domainObject, 
      RelationEndPointCollection allAffectedRelationEndPoints, 
      RelationEndPointCollection allOppositeRelationEndPoints)
  {
    _transactionEventSink.ObjectDeleting (domainObject);
    NotifyClientTransactionOfBeginRelationChange (domainObject, allAffectedRelationEndPoints, allOppositeRelationEndPoints);

    domainObject.BeginDelete ();
    NotifyOppositeEndPointsOfBeginRelationChange (domainObject, allAffectedRelationEndPoints, allOppositeRelationEndPoints);
  }

  private void NotifyClientTransactionOfBeginRelationChange (
      DomainObject domainObject, 
      RelationEndPointCollection allAffectedRelationEndPoints, 
      RelationEndPointCollection allOppositeRelationEndPoints)
  {
    foreach (RelationEndPoint oppositeEndPoint in allOppositeRelationEndPoints)
    {
      IRelationEndPointDefinition endPointDefinition = oppositeEndPoint.OppositeEndPointDefinition;
      RelationEndPoint oldEndPoint = allAffectedRelationEndPoints[new RelationEndPointID (domainObject.ID, endPointDefinition)];

      oppositeEndPoint.NotifyClientTransactionOfBeginRelationChange (oldEndPoint);
    }
  }

  private void NotifyOppositeEndPointsOfBeginRelationChange (
      DomainObject domainObject, 
      RelationEndPointCollection allAffectedRelationEndPoints, 
      RelationEndPointCollection allOppositeRelationEndPoints)
  {
    foreach (RelationEndPoint oppositeEndPoint in allOppositeRelationEndPoints)
    {
      IRelationEndPointDefinition endPointDefinition = oppositeEndPoint.OppositeEndPointDefinition;
      RelationEndPoint oldEndPoint = allAffectedRelationEndPoints[new RelationEndPointID (domainObject.ID, endPointDefinition)];

      oppositeEndPoint.BeginRelationChange (oldEndPoint);
    }
  }

  private void EndDelete (DomainObject domainObject, RelationEndPointCollection allOppositeRelationEndPoints)
  {
    NotifyClientTransactionOfEndRelationChange (allOppositeRelationEndPoints);
    _transactionEventSink.ObjectDeleted (domainObject);

    NotifyOppositeEndPointsOfEndRelationChange (allOppositeRelationEndPoints);
    domainObject.EndDelete ();
  }

  private void NotifyClientTransactionOfEndRelationChange (RelationEndPointCollection allOppositeRelationEndPoints)
  {
    foreach (RelationEndPoint endPoint in allOppositeRelationEndPoints)
      endPoint.NotifyClientTransactionOfEndRelationChange ();
  }

  private void NotifyOppositeEndPointsOfEndRelationChange (RelationEndPointCollection allOppositeRelationEndPoints)
  {
    foreach (RelationEndPoint endPoint in allOppositeRelationEndPoints)
      endPoint.EndRelationChange ();
  }

  private void CheckClientTransactionForDeletion (DomainObject domainObject)
  {
    if (domainObject.GetDataContainer().ClientTransaction != _clientTransaction)
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

  public void MarkDiscarded (DataContainer discardedDataContainer)
  {
    ArgumentUtility.CheckNotNull ("discardedDataContainer", discardedDataContainer);
    
    _transactionEventSink.DataManagerMarkingObjectDiscarded (discardedDataContainer.ID);
    _discardedDataContainers.Add (discardedDataContainer.ID, discardedDataContainer);
  }

  public bool IsDiscarded (ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);
    return _discardedDataContainers.ContainsKey (id);
  }

  public DataContainer GetDiscardedDataContainer (ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    DataContainer discardedDataContainer;
    if (!_discardedDataContainers.TryGetValue (id, out discardedDataContainer))
      throw new ArgumentException (string.Format ("The object '{0}' has not been discarded.", id), "id");
    else
      return discardedDataContainer;
  }

  public int DiscardedObjectCount
  {
    get { return _discardedDataContainers.Count; }
  }

  public void CopyFrom (DataManager source)
  {
    ArgumentUtility.CheckNotNull ("source", source);

    if (source == this)
      throw new ArgumentException ("Source cannot be the destination DataManager instance.", "source");

    _transactionEventSink.DataManagerCopyingFrom (source);
    source._transactionEventSink.DataManagerCopyingTo (this);

    CopyDiscardedDataContainers (source);
    DataContainerMap.CopyFrom (source.DataContainerMap);
    RelationEndPointMap.CopyFrom (source.RelationEndPointMap);
  }

  internal void CopyDiscardedDataContainers (DataManager source)
  {
    ArgumentUtility.CheckNotNull ("source", source);

    foreach (KeyValuePair<ObjectID, DataContainer> discardedItem in source._discardedDataContainers)
    {
      ObjectID discardedObjectID = discardedItem.Key;
      DataContainer discardedDataContainer = discardedItem.Value;
      CopyDiscardedDataContainer (discardedObjectID, discardedDataContainer);
    }
  }

  internal void CopyDiscardedDataContainer (ObjectID discardedObjectID, DataContainer discardedDataContainer)
  {
    ArgumentUtility.CheckNotNull ("discardedObjectID", discardedObjectID);

    ArgumentUtility.CheckNotNull ("discardedDataContainer", discardedDataContainer);
    DataContainer newDiscardedContainer = DataContainer.CreateNew (discardedObjectID);

    newDiscardedContainer.SetClientTransaction (_clientTransaction);
    newDiscardedContainer.SetDomainObject (discardedDataContainer.DomainObject);
    newDiscardedContainer.Delete ();

    Assertion.IsTrue (IsDiscarded (newDiscardedContainer.ID),
        "newDiscardedContainer.Delete must have inserted the DataContainer into the list of discarded objects");
    Assertion.IsTrue (GetDiscardedDataContainer (newDiscardedContainer.ID) == newDiscardedContainer);
  }
}
}
