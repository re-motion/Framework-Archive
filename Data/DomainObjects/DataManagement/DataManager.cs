using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Infrastructure;

namespace Rubicon.Data.DomainObjects.DataManagement
{
[Serializable]
public class DataManager : ISerializable, IDeserializationCallback
{
  // types

  // static members and constants

  // member fields

  private ClientTransaction _clientTransaction;
  private IClientTransactionListener _transactionEventSink;

  private DataContainerMap _dataContainerMap;
  private RelationEndPointMap _relationEndPointMap;
  private Dictionary<ObjectID, DataContainer> _discardedDataContainers;
  
  private object[] _deserializedData; // only used for deserialization

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
      if (domainObject.GetStateForTransaction (_clientTransaction) != StateType.Deleted)
        _relationEndPointMap.CheckMandatoryRelations (domainObject);

      DataContainer dataContainer = domainObject.GetDataContainerForTransaction(_clientTransaction);
      if (dataContainer.State != StateType.Unchanged)
        changedDataContainers.Add (dataContainer);
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
      StateType domainObjectState = dataContainer.DomainObject.GetStateForTransaction (_clientTransaction);
      if (includeChanged && domainObjectState == StateType.Changed)
        domainObjects.Add (dataContainer.DomainObject);

      if (includeDeleted && domainObjectState == StateType.Deleted)
        domainObjects.Add (dataContainer.DomainObject);

      if (includeNew && domainObjectState == StateType.New)
        domainObjects.Add (dataContainer.DomainObject);

      if (includeUnchanged && domainObjectState == StateType.Unchanged)
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

    if (domainObject.GetStateForTransaction (_clientTransaction) == StateType.Deleted)
      return;

    RelationEndPointModificationCollection oppositeEndPointModifications =
        _relationEndPointMap.GetOppositeEndPointModificationsForDelete (domainObject);

    BeginDelete (domainObject, oppositeEndPointModifications);
    PerformDelete (domainObject, oppositeEndPointModifications);
    EndDelete (domainObject, oppositeEndPointModifications);
  }

  internal void PerformDelete (DomainObject domainObject, RelationEndPointModificationCollection oppositeEndPointModifications)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    ArgumentUtility.CheckNotNull ("oppositeEndPointModifications", oppositeEndPointModifications);

    DataContainer dataContainer = domainObject.GetDataContainerForTransaction (_clientTransaction);  // rescue dataContainer before the map deletes is
    if (dataContainer.State == StateType.Deleted)
      return;

    _relationEndPointMap.PerformDelete (domainObject, oppositeEndPointModifications);
    _dataContainerMap.PerformDelete (dataContainer);
    
    dataContainer.Delete ();
  }

  private void BeginDelete (DomainObject domainObject, RelationEndPointModificationCollection oppositeEndPointModifications)
  {
    _transactionEventSink.ObjectDeleting (domainObject);
    oppositeEndPointModifications.NotifyClientTransactionOfBegin();

    domainObject.BeginDelete ();
    oppositeEndPointModifications.Begin();
  }

  private void EndDelete (DomainObject domainObject, RelationEndPointModificationCollection oppositeEndPointModifications)
  {
    oppositeEndPointModifications.NotifyClientTransactionOfEnd();
    _transactionEventSink.ObjectDeleted (domainObject);

    oppositeEndPointModifications.End ();
    domainObject.EndDelete ();
  }

  private void CheckClientTransactionForDeletion (DomainObject domainObject)
  {
    if (!domainObject.CanBeUsedInTransaction (_clientTransaction))
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

  #region Serialization
  protected DataManager (SerializationInfo info, StreamingContext context)
  {
    _deserializedData = (object[]) info.GetValue ("doInfo.GetData", typeof (object[]));
  }

  void IDeserializationCallback.OnDeserialization (object sender)
  {
    DomainObjectDeserializationInfo doInfo = new DomainObjectDeserializationInfo (_deserializedData);
    _clientTransaction = doInfo.GetValueForHandle<ClientTransaction> ();
    _transactionEventSink = _clientTransaction.TransactionEventSink;
    _dataContainerMap = doInfo.GetValue<DataContainerMap> (DataContainerMap.DeserializeFromFlatStructure);
    _relationEndPointMap = doInfo.GetValue<RelationEndPointMap> ();
    _discardedDataContainers = new Dictionary<ObjectID, DataContainer> ();

    ObjectID[] discardedIDs = doInfo.GetArray<ObjectID> (ObjectID.DeserializeFromFlatStructure);
    DataContainer[] discardedContainers = doInfo.GetArray<DataContainer> ();

    if (discardedIDs.Length != discardedContainers.Length)
      throw new SerializationException ("Invalid serilization data: discarded ID and data container counts do not match.");

    for (int i = 0; i < discardedIDs.Length; ++i)
      _discardedDataContainers.Add (discardedIDs[i], discardedContainers[i]);

    _deserializedData = null;
  }

  void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
  {
    DomainObjectSerializationInfo doInfo = new DomainObjectSerializationInfo();
    doInfo.AddHandle (_clientTransaction);
    doInfo.AddValue (_dataContainerMap);
    doInfo.AddValue (_relationEndPointMap);

    ObjectID[] discardedIDs = new ObjectID[_discardedDataContainers.Count];
    _discardedDataContainers.Keys.CopyTo (discardedIDs, 0);
    doInfo.AddArray (discardedIDs);

    DataContainer[] discardedContainers = new DataContainer[_discardedDataContainers.Count];
    _discardedDataContainers.Values.CopyTo (discardedContainers, 0);
    doInfo.AddArray (discardedContainers);

    info.AddValue ("doInfo.GetData", doInfo.GetData());
  }
  #endregion
}
}
