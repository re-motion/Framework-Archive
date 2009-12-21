// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
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
    /// <summary>
    /// Do not use this method, use <see>ClientTransaction.CreateBindingTransaction</see> instead.
    /// </summary>
    /// <returns></returns>
    [Obsolete ("Use ClientTransaction.CreateBindingTransaction for clarity.")]
    public static new ClientTransaction CreateBindingTransaction ()
    {
      return ClientTransaction.CreateBindingTransaction ();
    }

    private readonly ClientTransaction _parentTransaction;

    [NonSerialized]
    private SubQueryManager _queryManager;

    protected SubClientTransaction (ClientTransaction parentTransaction)
        : base (
          ArgumentUtility.CheckNotNull("parentTransaction", parentTransaction).ApplicationData, 
          parentTransaction.Extensions, 
          new SubCollectionEndPointChangeDetectionStrategy())
    {
      parentTransaction.NotifyOfSubTransactionCreating ();
      Assertion.IsTrue (parentTransaction.IsReadOnly);

      _parentTransaction = parentTransaction;

      var discardedObjects = _parentTransaction.DataManager.DiscardedObjectIDs
          .Select (id => _parentTransaction.DataManager.GetDiscardedDataContainer (id).DomainObject);
      var deletedObjects = _parentTransaction.DataManager.DataContainerMap.GetByState (StateType.Deleted).Select (dc => dc.DomainObject);
      foreach (var objectToBeDiscarded in discardedObjects.Concat (deletedObjects))
        MarkAsDiscarded (objectToBeDiscarded);

      parentTransaction.NotifyOfSubTransactionCreated (this);
    }

    public override ClientTransaction ParentTransaction
    {
      get { return _parentTransaction; }
    }

    public override ClientTransaction RootTransaction
    {
      get { return ParentTransaction.RootTransaction; }
    }

    /// <summary>Initializes a new instance of this transaction.</summary>
    public override ClientTransaction CreateEmptyTransactionOfSameType ()
    {
      return _parentTransaction.CreateSubTransaction();
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

    protected internal override bool DoEnlistDomainObject (DomainObject domainObject)
    {
      return ParentTransaction.DoEnlistDomainObject (domainObject);
    }

    protected internal override bool IsEnlisted (DomainObject domainObject)
    {
      return ParentTransaction.IsEnlisted (domainObject);
    }

    protected internal override DomainObject GetEnlistedDomainObject (ObjectID objectID)
    {
      return ParentTransaction.GetEnlistedDomainObject (objectID);
    }

    protected internal override IEnumerable<DomainObject> EnlistedDomainObjects
    {
      get { return ParentTransaction.EnlistedDomainObjects; }
    }

    protected internal override int EnlistedDomainObjectCount
    {
      get { return ParentTransaction.EnlistedDomainObjectCount; }
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
          DataContainer thisDataContainer = TransferParentObject (parentObject);
          return thisDataContainer;
        }
      }
    }

    protected override DataContainerCollection LoadDataContainers (IEnumerable<ObjectID> objectIDs, bool throwOnNotFound)
    {
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

      foreach (ObjectID id in objectIDs)
      {
        if (DataManager.IsDiscarded (id))
          throw new ObjectDiscardedException (id);
      }

      using (TransactionUnlocker.MakeWriteable (ParentTransaction))
      {
        var parentObjects = ParentTransaction.GetObjects<DomainObject> (objectIDs.ToArray(), throwOnNotFound).Where (obj => obj != null);
        var loadedDataContainers = new DataContainerCollection();
        foreach (DomainObject parentObject in parentObjects)
        {
          DataContainer thisDataContainer = TransferParentObject(parentObject);
          loadedDataContainers.Add (thisDataContainer);
        }

        foreach (DataContainer dataContainer in loadedDataContainers)
          TransactionEventSink.ObjectLoading (dataContainer.ID);
        
        return loadedDataContainers;
      }
    }

    internal protected override DataContainer LoadDataContainerForExistingObject (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      // ensure that parent transaction knows the given object, that way, LoadDataContainer will associate the child DataContainer with it
      using (TransactionUnlocker.MakeWriteable (ParentTransaction))
      {
        ParentTransaction.GetDataContainer(domainObject);
      }

      using (EnterNonDiscardingScope ())
      {
        return LoadDataContainer (domainObject.ID);
      }
    }

    protected override DataContainer LoadRelatedDataContainer (RelationEndPointID relationEndPointID)
    {
      DomainObject parentRelatedObject;
      using (TransactionUnlocker.MakeWriteable (ParentTransaction))
      {
        parentRelatedObject = ParentTransaction.GetRelatedObject (relationEndPointID);
      }
      return parentRelatedObject != null ? LoadDataContainer (parentRelatedObject.ID) : null;
    }


    protected override DataContainerCollection LoadRelatedDataContainers (RelationEndPointID relationEndPointID)
    {
      DomainObjectCollection parentObjects;
      using (TransactionUnlocker.MakeWriteable (ParentTransaction))
      {
        parentObjects = ParentTransaction.GetRelatedObjects (relationEndPointID);
      }

      var transferredContainers = new DataContainerCollection ();
      foreach (DomainObject parentObject in parentObjects)
      {
        DataContainer transferredContainer = TransferParentContainer (ParentTransaction.GetDataContainer (parentObject));
        transferredContainers.Add (transferredContainer);
        Assertion.IsTrue (parentObject == transferredContainer.DomainObject, "invariant");
      }

      return transferredContainers;
    }

    private DataContainer TransferParentObject (DomainObject parentObject)
    {
      DataContainer parentDataContainer = ParentTransaction.GetDataContainer (parentObject);
      DataContainer thisDataContainer = TransferParentContainer (parentDataContainer);
      return thisDataContainer;
    }
    
    private DataContainer TransferParentContainer (DataContainer parentDataContainer)
    {
      Assertion.IsFalse (DataManager.IsDiscarded (parentDataContainer.ID));

      var thisDataContainer = DataContainer.CreateNew (parentDataContainer.ID);

      thisDataContainer.SetPropertyValuesFrom (parentDataContainer);
      thisDataContainer.SetTimestamp (parentDataContainer.Timestamp);
      thisDataContainer.SetDomainObject (parentDataContainer.DomainObject);
      thisDataContainer.Commit2(); // for the new DataContainer, the current parent DC state becomes the Unchanged state

      return thisDataContainer;
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
      Assertion.IsNull (GetParentDataContainerWithoutLoading (dataContainer.ID), "a new data container cannot be known to the parent");
      Assertion.IsFalse (dataContainer.IsDiscarded);

      var parentDataContainer = DataContainer.CreateNew (dataContainer.ID);
      
      parentDataContainer.SetPropertyValuesFrom (dataContainer);
      if (dataContainer.HasBeenMarkedChanged)
        parentDataContainer.MarkAsChanged();
      parentDataContainer.SetTimestamp (dataContainer.Timestamp);
      parentDataContainer.SetDomainObject (dataContainer.DomainObject);

      parentDataContainer.RegisterWithTransaction (ParentTransaction);

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

      parentDataContainer.SetTimestamp (dataContainer.Timestamp);
      parentDataContainer.SetPropertyValuesFrom (dataContainer);
      
      if (dataContainer.HasBeenMarkedChanged)
        parentDataContainer.MarkAsChanged ();
    }

    private void PersistDeletedDataContainer (DataContainer dataContainer)
    {
      DataContainer parentDataContainer = GetParentDataContainerWithoutLoading (dataContainer.ID);
      Assertion.IsNotNull (
          parentDataContainer,
          "a deleted DataContainer must have been loaded through ParentTransaction, so the ParentTransaction must know it");

      Assertion.IsTrue (
          !parentDataContainer.IsDiscarded && parentDataContainer.State != StateType.Deleted,
          "deleted DataContainers cannot be discarded or deleted in the ParentTransaction");
      Assertion.IsTrue (parentDataContainer.DomainObject == dataContainer.DomainObject, "invariant");

      DomainObject domainObject = dataContainer.DomainObject;
      
      // TODO 1914: Check whether this should be unified with DataManager.PerformDelete again
      
      // do not pass any opposite end point modifications to PerformDelete - this method only persists changes made directly to the deleted object
      var emptyOppositeBidirectionalEndPointModification = new CompositeRelationModificationWithEvents ();
      ParentTransaction.DataManager.RelationEndPointMap.PerformDelete2 (domainObject, emptyOppositeBidirectionalEndPointModification);
      ParentTransaction.DataManager.DataContainerMap.PerformDelete2 (parentDataContainer);
    
      parentDataContainer.Delete2 ();
    }

    private DataContainer GetParentDataContainerWithoutLoading (ObjectID id)
    {
      Assertion.IsFalse (ParentTransaction.DataManager.IsDiscarded (id), "this method is not called in situations where the ID could be discarded");
      return ParentTransaction.DataManager.DataContainerMap[id];
    }

    private void PersistRelationEndPoints (IEnumerable<RelationEndPoint> changedEndPoints)
    {
      foreach (RelationEndPoint endPoint in changedEndPoints)
      {
        Assertion.IsTrue (endPoint.HasChanged);

        RelationEndPoint parentEndPoint = ParentTransaction.DataManager.RelationEndPointMap[endPoint.ID];
        if (parentEndPoint == null)
        {
          Assertion.IsTrue (
              DataManager.DataContainerMap[endPoint.ObjectID].State == StateType.Deleted
              && ParentTransaction.DataManager.IsDiscarded (endPoint.ObjectID),
              "Because the DataContainers are processed before the RelationEndPoints, the RelationEndPointMaps of ParentTransaction and this now "
              + "contain end points for the same end point IDs. The only scenario in which the ParentTransaction doesn't know an end point known "
              + "to the child transaction is when the object was of state New in the ParentTransaction and its DataContainer was just discarded.");
        }
        else
        {
          parentEndPoint.SetValueFrom (endPoint);
        }
      }
    }

    private void MarkAsDiscarded (DomainObject objectToBeDiscarded)
    {
      var newDiscardedContainer = DataContainer.CreateNew (objectToBeDiscarded.ID);

      newDiscardedContainer.SetDomainObject (objectToBeDiscarded);
      newDiscardedContainer.RegisterWithTransaction (this);

      DataManager.PerformDelete2 (objectToBeDiscarded, new CompositeRelationModificationWithEvents());

      Assertion.IsTrue (DataManager.IsDiscarded (newDiscardedContainer.ID),
          "newDiscardedContainer.Delete must have inserted the DataContainer into the list of discarded objects");
      Assertion.IsTrue (DataManager.GetDiscardedDataContainer (newDiscardedContainer.ID) == newDiscardedContainer);
    }
  }
}
