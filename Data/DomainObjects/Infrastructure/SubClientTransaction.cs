using System;
using System.Collections.Generic;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Queries;
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
    private bool _isDiscarded;

    private SubQueryManager _queryManager;

    public SubClientTransaction (ClientTransaction parentTransaction)
        : base (ArgumentUtility.CheckNotNull("parentTransaction", parentTransaction).ApplicationData,
            ArgumentUtility.CheckNotNull("parentTransaction", parentTransaction).Extensions)
    {
      ArgumentUtility.CheckNotNull ("parentTransaction", parentTransaction);

      parentTransaction.NotifyOfSubTransactionCreating ();
      Assertion.IsTrue (parentTransaction.IsReadOnly);
      _parentTransaction = parentTransaction;

      DataManager.CopyFrom (_parentTransaction.DataManager);
      // commit the data manager to the data we got from the parent transaction, this will set all DomainObject states as needed (mostly
      // StateType.Unchanged, unless the object deleted, in which case it will be discarded), and set the original values to the current values
      DataManager.Commit ();

      parentTransaction.NotifyOfSubTransactionCreated (this);
      _isDiscarded = false;
    }

    public override ClientTransaction ParentTransaction
    {
      get { return _parentTransaction; }
    }

    public override ClientTransaction RootTransaction
    {
      get { return ParentTransaction.RootTransaction; }
    }

    public override bool IsDiscarded
    {
      get { return _isDiscarded; }
    }

    public override IQueryManager QueryManager
    {
      get
      {
        if (_queryManager == null)
          _queryManager = new SubQueryManager (this);

        return _queryManager;
      }
    }

    public override bool ReturnToParentTransaction ()
    {
      ParentTransaction.IsReadOnly = false;
      _isDiscarded = true;
      AddListener (new InvalidatedSubTransactionListener ());
      return true;
    }

    protected internal override void DoEnlistDomainObject (DomainObject domainObject)
    {
      ParentTransaction.DoEnlistDomainObject (domainObject);
    }

    protected internal override bool IsEnlisted (DomainObject domainObject)
    {
      return ParentTransaction.IsEnlisted (domainObject);
    }

    protected internal override ObjectID CreateNewObjectID (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      return ParentTransaction.CreateNewObjectID (classDefinition);
    }

    protected override DataContainer LoadDataContainer (ObjectID id)
    {
      if (DataManager.IsDiscarded (id))
      {
        // Trying to load a data container for a discarded object. To mimic the behavior of RootClientTransaction, we will throw an
        // ObjectNotFoundException here.
        throw new ObjectNotFoundException (id);
      }
      else
      {
        TransactionEventSink.ObjectLoading (id);

        using (TransactionUnlocker.MakeWriteable (ParentTransaction))
        {
          DomainObject parentObject = ParentTransaction.GetObject (id);
          DataContainer parentDataContainer = parentObject.GetDataContainerForTransaction (ParentTransaction);

          DataContainer thisDataContainer = TransferParentContainer(parentDataContainer);

          thisDataContainer.SetClientTransaction (this);
          thisDataContainer.SetDomainObject (parentObject);

          DataManager.RegisterExistingDataContainer (thisDataContainer);

          return thisDataContainer;
        }
      }
    }

    private DataContainer TransferParentContainer (DataContainer parentDataContainer)
    {
      Assertion.IsFalse (DataManager.IsDiscarded (parentDataContainer.ID));

      DataContainer thisDataContainer = parentDataContainer.Clone();
      thisDataContainer.Commit(); // for the new DataContainer, the current parent DC state becomes the Unchanged state
      return thisDataContainer;
    }

    protected internal override DomainObject LoadRelatedObject (RelationEndPointID relationEndPointID)
    {
      DomainObject parentObject;
      using (TransactionUnlocker.MakeWriteable (ParentTransaction))
      {
        parentObject = ParentTransaction.GetRelatedObject (relationEndPointID);
      }

      if (parentObject != null)
      {
        DataContainer loadedDataContainer = parentObject.GetDataContainerForTransaction (this);
        Assertion.IsTrue (parentObject == loadedDataContainer.DomainObject, "invariant");
      }
      else
        DataManager.RelationEndPointMap.RegisterObjectEndPoint (relationEndPointID, null);

      return parentObject;
    }

    protected internal override DomainObjectCollection LoadRelatedObjects (RelationEndPointID relationEndPointID)
    {
      DomainObjectCollection parentObjects;
      using (TransactionUnlocker.MakeWriteable (ParentTransaction))
      {
        parentObjects = ParentTransaction.GetRelatedObjects (relationEndPointID);
      }

      DataContainerCollection transferredContainers = new DataContainerCollection();
      foreach (DomainObject parentObject in parentObjects)
      {
        DataContainer transferredContainer = TransferParentContainer (parentObject.GetDataContainerForTransaction (ParentTransaction));
        transferredContainers.Add (transferredContainer);
        Assertion.IsTrue (parentObject == transferredContainer.DomainObject, "invariant");
      }

      return MergeLoadedDomainObjects (transferredContainers, relationEndPointID);
    }

    protected override void PersistData (DataContainerCollection changedDataContainers)
    {
      using (TransactionUnlocker.MakeWriteable (ParentTransaction))
      {
        PersistDataContainers (changedDataContainers);
        PersistRelationEndPoints (DataManager.GetChangedRelationEndPoints());
      }
    }

    private void PersistDataContainers (DataContainerCollection changedDataContainers)
    {
      foreach (DataContainer dataContainer in changedDataContainers)
      {
        Assertion.IsFalse (
            dataContainer.IsDiscarded,
            "changedDataContainers cannot contain discarded DataContainers, because its items come"
            + "from DataManager.DataContainerMap, which does not contain discarded objects");
        Assertion.IsTrue (dataContainer.State != StateType.Unchanged, "changedDataContainers cannot contain an unchanged container");
        Assertion.IsTrue (
            dataContainer.State == StateType.New || dataContainer.State == StateType.Changed
            || dataContainer.State == StateType.Deleted,
            "Invalid dataContainer.State: " + dataContainer.State);

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
        }
      }
    }

    private void PersistNewDataContainer (DataContainer dataContainer)
    {
      DataContainer parentDataContainer = GetParentDataContainerWithoutLoading (dataContainer.ID);
      Assertion.IsNull (parentDataContainer, "a new data container cannot be known to the parent");

      parentDataContainer = CreateParentDataContainer (dataContainer);
      ParentTransaction.DataManager.RegisterNewDataContainer (parentDataContainer);

      Assertion.IsTrue (parentDataContainer.DomainObject == dataContainer.DomainObject, "invariant");
    }

    private void PersistChangedDataContainer (DataContainer dataContainer)
    {
      DataContainer parentDataContainer = GetParentDataContainerWithoutLoading (dataContainer.ID);
      Assertion.IsNotNull (
          parentDataContainer,
          "a changed DataContainer must have been loaded through ParentTransaction, so the "
          + "ParentTransaction must know it");
      Assertion.IsFalse (parentDataContainer.IsDiscarded, "a changed DataContainer cannot be discarded in the ParentTransaction");
      Assertion.IsTrue (parentDataContainer.State != StateType.Deleted, "a changed DataContainer cannot be deleted in the ParentTransaction");
      Assertion.IsTrue (parentDataContainer.DomainObject == dataContainer.DomainObject, "invariant");

      StateType previousState = parentDataContainer.State;
      parentDataContainer.AssumeSameState (dataContainer, false);

      Assertion.IsTrue (
          (previousState == StateType.New && parentDataContainer.State == StateType.New)
          || (previousState != StateType.New && parentDataContainer.State == StateType.Changed));
    }

    private void PersistDeletedDataContainer (DataContainer dataContainer)
    {
      DataContainer parentDataContainer = GetParentDataContainerWithoutLoading (dataContainer.ID);
      Assertion.IsNotNull (
          parentDataContainer,
          "a deleted DataContainer must have been loaded through ParentTransaction, so the "
          + "ParentTransaction must know it");

      Assertion.IsTrue (
          !parentDataContainer.IsDiscarded && parentDataContainer.State != StateType.Deleted,
          "deleted DataContainers cannot "
          + "be discarded or deleted in the ParentTransaction");
      Assertion.IsTrue (parentDataContainer.DomainObject == dataContainer.DomainObject, "invariant");

      ParentTransaction.Delete (parentDataContainer.DomainObject);
    }

    private DataContainer GetParentDataContainerWithoutLoading (ObjectID id)
    {
      Assertion.IsFalse (ParentTransaction.DataManager.IsDiscarded (id), "this method is not called in situations where the ID could be discarded");
      return ParentTransaction.DataManager.DataContainerMap[id];
    }

    private DataContainer CreateParentDataContainer (DataContainer dataContainer)
    {
      DataContainer parentDataContainer = dataContainer.Clone();
      parentDataContainer.SetClientTransaction (ParentTransaction);
      return parentDataContainer;
    }

    private void PersistRelationEndPoints (IEnumerable<RelationEndPoint> changedEndPoints)
    {
      foreach (RelationEndPoint endPoint in changedEndPoints)
      {
        Assertion.IsTrue (endPoint.HasChanged);

        RelationEndPoint parentEndPoint = ParentTransaction.DataManager.RelationEndPointMap[endPoint.ID];
        if (parentEndPoint == null)
          Assertion.IsTrue (
              DataManager.DataContainerMap[endPoint.ObjectID].State == StateType.Deleted
              && ParentTransaction.DataManager.IsDiscarded (endPoint.ObjectID),
              "Because the DataContainers are processed before the RelationEndPoints, the RelationEndPointMaps of ParentTransaction and this now "
              + "contain end points for the same end point IDs. The only scenario in which the ParentTransaction doesn't know an end point known "
              + "to the child transaction is when the object was of state New in the ParentTransaction and its DataContainer was just discarded.");
        else
          parentEndPoint.AssumeSameState (endPoint);
      }
    }
  }
}