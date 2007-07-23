using System;
using System.Collections.Generic;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Represents a transaction that is part of a bigger parent transaction. Any changes made within this subtransaction are not visible in
  /// the parent transaction until the subtransaction is committed, and a commit operation will only commit the changes to the parent transaction, 
  /// not to any storage providers.
  /// </summary>
  /// <remarks>The parent transaction cannot be modified while a subtransaction is active.</remarks>
  [Serializable]
  public class SubClientTransaction : ClientTransaction
  {
    private readonly ClientTransaction _parentTransaction;

    public SubClientTransaction (ClientTransaction parentTransaction)
        : base (ArgumentUtility.CheckNotNull("parentTransaction", parentTransaction).ApplicationData,
            ArgumentUtility.CheckNotNull("parentTransaction", parentTransaction).Extensions)
    {
      ArgumentUtility.CheckNotNull ("parentTransaction", parentTransaction);

      parentTransaction.TransactionEventSink.SubTransactionCreating ();
      parentTransaction.IsReadOnly = true;
      _parentTransaction = parentTransaction;

      DataManager.CopyFrom (_parentTransaction.DataManager);
      // commit the data manager to the data we got from the parent transaction, this will set all DomainObject states as needed (mostly
      // StateType.Unchanged, unless the object deleted, in which case it will be discarded), and set the original values to the current values
      DataManager.Commit ();

      parentTransaction.TransactionEventSink.SubTransactionCreated (this);
    }

    public override ClientTransaction ParentTransaction
    {
      get { return _parentTransaction; }
    }

    public override ClientTransaction RootTransaction
    {
      get { return ParentTransaction.RootTransaction; }
    }

    public override bool ReturnToParentTransaction ()
    {
      ParentTransaction.IsReadOnly = false;
      AddListener (new InvalidatedSubTransactionListener ());
      return true;
    }

    protected override DataContainer LoadDataContainer (ObjectID id)
    {
      if (DataManager.IsDiscarded (id))
      {
        // Trying to load a data container for a discarded object. To mimic the behavior of RootClientTransaction, we will throw an
        // ObjectNotFoundException here.
        throw new ObjectNotFoundException (id);
      }
      return base.LoadDataContainer (id);
    }

    protected override void PersistData (DataContainerCollection changedDataContainers)
    {
      // need to temporarily release read-only lock from parent transaction
      Assertion.Assert (ParentTransaction.IsReadOnly);
      ParentTransaction.IsReadOnly = false;

      try
      {
        PersistDataContainers (changedDataContainers);
        PersistRelationEndPoints (GetChangedRelationEndPoints ());
      }
      finally
      {
        ParentTransaction.IsReadOnly = true;
      }
    }

    // TODO: move to DataManager
    private IEnumerable<RelationEndPoint> GetChangedRelationEndPoints ()
    {
      foreach (RelationEndPoint endPoint in DataManager.RelationEndPointMap)
      {
        if (endPoint.HasChanged)
          yield return endPoint;
      }
    }

    private void PersistDataContainers (DataContainerCollection changedDataContainers)
    {
      foreach (DataContainer dataContainer in changedDataContainers)
      {
        if (dataContainer.IsDiscarded)
          PersistDiscardedDataContainer (dataContainer);
        else
        {
          switch (dataContainer.State)
          {
            case StateType.New:
              PersistNewDataContainer (dataContainer);
              break;
            case StateType.Changed:
              PersistChangedDataContainer (dataContainer);
              break;
            case StateType.Deleted:
              PersistDeletedDataContainer (dataContainer);
              break;
            case StateType.Unchanged:
              PersistUnchangedDataContainer (dataContainer);
              break;
            default:
              Assertion.Assert (false, "Invalid dataContainer.State: " + dataContainer.State);
              break;
          }
        }
      }
    }

    private void PersistDiscardedDataContainer (DataContainer dataContainer)
    {
      DataContainer parentDataContainer = GetParentDataContainerWithoutLoading (dataContainer.ID);
      Assertion.Assert (parentDataContainer == null || parentDataContainer.IsDiscarded || parentDataContainer.State == StateType.Deleted);
      Assertion.Assert (parentDataContainer == null || parentDataContainer.DomainObject == dataContainer.DomainObject);

      // nothing to do here
    }

    private void PersistNewDataContainer (DataContainer dataContainer)
    {
      DataContainer parentDataContainer = GetParentDataContainerWithoutLoading (dataContainer.ID);
      Assertion.Assert (parentDataContainer == null, "a new data container cannot be known to the parent");

      parentDataContainer = CreateParentDataContainer (dataContainer);
      ParentTransaction.DataManager.RegisterNewDataContainer (parentDataContainer);

      Assertion.Assert (parentDataContainer.DomainObject == dataContainer.DomainObject);
    }

    private void PersistChangedDataContainer (DataContainer dataContainer)
    {
      DataContainer parentDataContainer = GetParentDataContainerWithLoading (dataContainer.DomainObject);
      Assertion.Assert (!parentDataContainer.IsDiscarded);
      Assertion.Assert (parentDataContainer.State != StateType.Deleted);
      Assertion.Assert (parentDataContainer.DomainObject == dataContainer.DomainObject);

      StateType previousState = parentDataContainer.State;
      parentDataContainer.AssumeSameState (dataContainer, false);
      Assertion.Assert ((previousState == StateType.New && parentDataContainer.State == StateType.New)
          || (previousState != StateType.New && parentDataContainer.State == StateType.Changed));
    }

    private void PersistDeletedDataContainer (DataContainer dataContainer)
    {
      DataContainer parentDataContainer = GetParentDataContainerWithoutLoading (dataContainer.ID);
      if (parentDataContainer == null)
      {
        // unknown in parent transaction
        parentDataContainer = CreateParentDataContainer (dataContainer);
        parentDataContainer.Rollback (); // reset data (State is now Unchanged rather than Deleted)
        Assertion.Assert (parentDataContainer.State == StateType.Unchanged);
        ParentTransaction.DataManager.RegisterExistingDataContainer (parentDataContainer);
      }

      Assertion.Assert (!parentDataContainer.IsDiscarded && parentDataContainer.State != StateType.Deleted);
      Assertion.Assert (parentDataContainer.DomainObject == dataContainer.DomainObject);

      ParentTransaction.Delete (parentDataContainer.DomainObject);
    }

    private void PersistUnchangedDataContainer (DataContainer dataContainer)
    {
      DataContainer parentDataContainer = GetParentDataContainerWithoutLoading (dataContainer.ID);
      Assertion.Assert (parentDataContainer == null || (!parentDataContainer.IsDiscarded && parentDataContainer.State != StateType.Deleted));
      Assertion.Assert (parentDataContainer == null || parentDataContainer.DomainObject == dataContainer.DomainObject);
      
      // Nothing to be done here
    }

    private DataContainer GetParentDataContainerWithoutLoading (ObjectID id)
    {
      if (ParentTransaction.DataManager.IsDiscarded (id))
        return ParentTransaction.DataManager.GetDiscardedDataContainer (id);
      else
        return ParentTransaction.DataManager.DataContainerMap[id];
    }

    private DataContainer GetParentDataContainerWithLoading (DomainObject domainObject)
    {
      DataContainer alreadyLoadedDataContainer = GetParentDataContainerWithoutLoading (domainObject.ID);
      if (alreadyLoadedDataContainer != null)
      {
        Assertion.Assert (alreadyLoadedDataContainer.DomainObject == domainObject);
        return alreadyLoadedDataContainer;
      }
      else
        return ParentTransaction.LoadDataContainerForExistingObject (domainObject);
    }

    private DataContainer CreateParentDataContainer (DataContainer dataContainer)
    {
      DataContainer parentDataContainer = dataContainer.Clone ();
      parentDataContainer.SetClientTransaction (ParentTransaction);
      return parentDataContainer;
    }

    private void PersistRelationEndPoints (IEnumerable<RelationEndPoint> changedEndPoints)
    {
      foreach (RelationEndPoint endPoint in changedEndPoints)
      {
        Assertion.Assert (endPoint.HasChanged);

        RelationEndPoint parentEndPoint = ParentTransaction.DataManager.RelationEndPointMap[endPoint.ID];
        if (parentEndPoint == null)
          Assertion.Assert (DataManager.DataContainerMap[endPoint.ObjectID].State == StateType.Deleted);
        else
          PersistChangedRelationEndPoint (endPoint, parentEndPoint);
      }
    }

    private void PersistChangedRelationEndPoint (RelationEndPoint endPoint, RelationEndPoint parentEndPoint)
    {
      parentEndPoint.AssumeSameState (endPoint);
    }
  }
}