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
using Remotion.Data.DomainObjects.Mapping;
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
  public class SubPersistenceStrategy : IPersistenceStrategy
  {
    private readonly IDataManager _dataManager;
    private readonly ClientTransaction _parentTransaction;

    public SubPersistenceStrategy (IDataManager dataManager, ClientTransaction parentTransaction)
    {
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);
      ArgumentUtility.CheckNotNull ("parentTransaction", parentTransaction);

      if (!parentTransaction.IsReadOnly)
      {
        throw new ArgumentException (
            "In order for the subtransaction persistence strategy to work correctly, the parent transaction needs to be read-only.",
            "parentTransaction");
      }

      _dataManager = dataManager;
      _parentTransaction = parentTransaction;

      TransferDeletedAndInvalidObjects();
    }

    public ClientTransaction ParentTransaction
    {
      get { return _parentTransaction; }
    }

    public virtual ObjectID CreateNewObjectID (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      return _parentTransaction.CreateNewObjectID (classDefinition);
    }

    public virtual DataContainer LoadDataContainer (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      using (TransactionUnlocker.MakeWriteable (_parentTransaction))
      {
        DomainObject parentObject = _parentTransaction.GetObject (id, false);
        DataContainer thisDataContainer = TransferParentObject (parentObject.ID);
        return thisDataContainer;
      }
    }

    public virtual DataContainerCollection LoadDataContainers (ICollection<ObjectID> objectIDs, bool throwOnNotFound)
    {
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

      if (objectIDs.Count == 0)
        return new DataContainerCollection();

      using (TransactionUnlocker.MakeWriteable (_parentTransaction))
      {
        var parentObjects = _parentTransaction.GetObjects<DomainObject> (objectIDs, throwOnNotFound).Where (obj => obj != null);
        var loadedDataContainers = new DataContainerCollection();
        foreach (DomainObject parentObject in parentObjects)
        {
          DataContainer thisDataContainer = TransferParentObject (parentObject.ID);
          loadedDataContainers.Add (thisDataContainer);
        }

        return loadedDataContainers;
      }
    }

    public virtual DataContainer LoadRelatedDataContainer (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
      if (!relationEndPointID.Definition.IsVirtual)
        throw new ArgumentException ("LoadRelatedDataContainer can only be called for virtual end points.", "relationEndPointID");

      DomainObject parentRelatedObject;
      using (TransactionUnlocker.MakeWriteable (_parentTransaction))
      {
        parentRelatedObject = _parentTransaction.GetRelatedObject (relationEndPointID);
      }

      if (parentRelatedObject != null)
        return LoadDataContainer (parentRelatedObject.ID);
      else
        return null;
    }

    public virtual DataContainerCollection LoadRelatedDataContainers (RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
      
      using (TransactionUnlocker.MakeWriteable (_parentTransaction))
      {
        DomainObjectCollection parentObjects = _parentTransaction.GetRelatedObjects (relationEndPointID);

        var transferredContainers = new DataContainerCollection();
        foreach (DomainObject parentObject in parentObjects)
        {
          DataContainer transferredContainer = TransferParentObject (parentObject.ID);
          transferredContainers.Add (transferredContainer);
          Assertion.IsTrue (parentObject == transferredContainer.DomainObject, "invariant");
        }
        return transferredContainers;
      }
    }

    public virtual DataContainer[] LoadDataContainersForQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      using (TransactionUnlocker.MakeWriteable (_parentTransaction))
      {
        var queryResult = _parentTransaction.QueryManager.GetCollection (query);
        if (queryResult == null)
          throw new InvalidOperationException ("Parent transaction returned an invalid null query result.");

        var parentObjects = queryResult.AsEnumerable();

        var transferredContainers = new List<DataContainer>();
        foreach (var parentObject in parentObjects)
        {
          DataContainer transferredContainer = TransferParentObject (parentObject.ID);
          transferredContainers.Add (transferredContainer);
          Assertion.IsTrue (parentObject == transferredContainer.DomainObject, "invariant");
        }
        return transferredContainers.ToArray();
      }
    }

    public virtual object LoadScalarForQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      return _parentTransaction.QueryManager.GetScalar (query);
    }

    private void TransferDeletedAndInvalidObjects ()
    {
      var invalidObjects = _parentTransaction.DataManager.InvalidObjectIDs
          .Select (id => _parentTransaction.DataManager.GetInvalidObjectReference (id));
      var deletedObjects = _parentTransaction.DataManager.DataContainerMap.Where (dc => dc.State == StateType.Deleted).Select (dc => dc.DomainObject);
      
      foreach (var objectToBeMarkedInvalid in invalidObjects.Concat (deletedObjects))
        _dataManager.MarkObjectInvalid (objectToBeMarkedInvalid);
    }

    private DataContainer TransferParentObject (ObjectID objectID)
    {
      var parentDataContainer = _parentTransaction.DataManager.GetDataContainerWithLazyLoad (objectID);
      return TransferParentContainer (parentDataContainer);
    }

    private DataContainer TransferParentContainer (DataContainer parentDataContainer)
    {
      Assertion.IsFalse (_dataManager.IsInvalid (parentDataContainer.ID));
      Assertion.IsFalse (parentDataContainer.State == StateType.Deleted, "Implied by previous assertion");

      var thisDataContainer = DataContainer.CreateNew (parentDataContainer.ID);

      thisDataContainer.SetPropertyValuesFrom (parentDataContainer);
      thisDataContainer.SetTimestamp (parentDataContainer.Timestamp);
      thisDataContainer.SetDomainObject (parentDataContainer.DomainObject);
      thisDataContainer.CommitState(); // for the new DataContainer, the current parent DC state becomes the Unchanged state

      return thisDataContainer;
    }

    public virtual void PersistData (IEnumerable<DataContainer> changedDataContainers)
    {
      using (TransactionUnlocker.MakeWriteable (_parentTransaction))
      {
        PersistDataContainers (changedDataContainers);
        PersistRelationEndPoints (_dataManager.GetChangedRelationEndPoints());
      }
    }

    private void PersistDataContainers (IEnumerable<DataContainer> changedDataContainers)
    {
      foreach (DataContainer dataContainer in changedDataContainers)
      {
        Assertion.IsFalse (
            dataContainer.IsDiscarded,
            "changedDataContainers cannot contain discarded DataContainers, because its items come"
            + "from DataManager.DataContainerMap, which does not contain discarded containers");
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
      Assertion.IsTrue (_parentTransaction.DataManager.IsInvalid (dataContainer.ID));
      _parentTransaction.DataManager.ClearInvalidFlag (dataContainer.ID);

      Assertion.IsNull (GetParentDataContainerWithoutLoading (dataContainer.ID), "a new data container cannot be known to the parent");
      Assertion.IsFalse (dataContainer.IsDiscarded);

      var parentDataContainer = DataContainer.CreateNew (dataContainer.ID);

      parentDataContainer.SetPropertyValuesFrom (dataContainer);
      if (dataContainer.HasBeenMarkedChanged)
        parentDataContainer.MarkAsChanged();
      parentDataContainer.SetTimestamp (dataContainer.Timestamp);
      parentDataContainer.SetDomainObject (dataContainer.DomainObject);

      _parentTransaction.DataManager.RegisterDataContainer (parentDataContainer);

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
        parentDataContainer.MarkAsChanged();
    }

    private void PersistDeletedDataContainer (DataContainer dataContainer)
    {
      DataContainer parentDataContainer = GetParentDataContainerWithoutLoading (dataContainer.ID);
      Assertion.IsNotNull (
          parentDataContainer,
          "a deleted DataContainer must have been loaded through ParentTransaction, so the ParentTransaction must know it");

      Assertion.IsTrue (
          parentDataContainer.State != StateType.Invalid && parentDataContainer.State != StateType.Deleted,
          "deleted DataContainers cannot be discarded or deleted in the ParentTransaction");
      Assertion.IsTrue (parentDataContainer.DomainObject == dataContainer.DomainObject, "invariant");

      var deleteCommand = _parentTransaction.DataManager.CreateDeleteCommand (dataContainer.DomainObject);
      deleteCommand.Perform(); // no events, no bidirectional changes
    }

    private DataContainer GetParentDataContainerWithoutLoading (ObjectID id)
    {
      Assertion.IsFalse (_parentTransaction.DataManager.IsInvalid (id), "this method is not called in situations where the ID could be invalid");
      return _parentTransaction.DataManager.DataContainerMap[id];
    }

    private void PersistRelationEndPoints (IEnumerable<RelationEndPoint> changedEndPoints)
    {
      foreach (RelationEndPoint endPoint in changedEndPoints)
      {
        Assertion.IsTrue (endPoint.HasChanged);

        RelationEndPoint parentEndPoint = _parentTransaction.DataManager.RelationEndPointMap[endPoint.ID];
        if (parentEndPoint == null)
        {
          Assertion.IsTrue (
              _dataManager.DataContainerMap[endPoint.ObjectID].State == StateType.Deleted
              && _parentTransaction.DataManager.IsInvalid (endPoint.ObjectID),
              "Because the DataContainers are processed before the RelationEndPoints, the RelationEndPointMaps of ParentTransaction and this now "
              + "contain end points for the same end point IDs. The only scenario in which the ParentTransaction doesn't know an end point known "
              + "to the child transaction is when the object was of state New in the ParentTransaction and its DataContainer was just discarded.");
        }
        else
          parentEndPoint.SetValueFrom (endPoint);
      }
    }
  }
}