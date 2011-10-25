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
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Implements the persistence of a subtransaction. Data is loaded via and persisted into the parent transaction.
  /// </summary>
  [Serializable]
  public class SubPersistenceStrategy : IPersistenceStrategy
  {
    private readonly IParentTransactionContext _parentTransactionContext;

    public SubPersistenceStrategy (IParentTransactionContext parentTransactionContext)
    {
      ArgumentUtility.CheckNotNull ("parentTransactionContext", parentTransactionContext);
      _parentTransactionContext = parentTransactionContext;
    }

    public IParentTransactionContext ParentTransactionContext
    {
      get { return _parentTransactionContext; }
    }

    public virtual ObjectID CreateNewObjectID (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      using (var parentTransactionOperations = _parentTransactionContext.AccessParentTransaction ())
      {
        return parentTransactionOperations.CreateNewObjectID (classDefinition);
      }
    }

    public virtual ILoadedObjectData LoadObjectData (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      using (var parentTransactionOperations = _parentTransactionContext.AccessParentTransaction ())
      {
        var parentObject = parentTransactionOperations.GetObject (id);
        return TransferParentObject (parentObject, parentTransactionOperations);
      }
    }

    public virtual IEnumerable<ILoadedObjectData> LoadObjectData (ICollection<ObjectID> objectIDs, bool throwOnNotFound)
    {
      ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

      if (objectIDs.Count == 0)
        return Enumerable.Empty<ILoadedObjectData> ();

      using (var parentTransactionOperations = _parentTransactionContext.AccessParentTransaction ())
      {
        var parentObjects = parentTransactionOperations.GetObjects (objectIDs, throwOnNotFound).Where (obj => obj != null);
        // Eager evaluation of sequence to keep parent transaction writeable as shortly as possible
        return parentObjects
            .Select (parentObject => (ILoadedObjectData) TransferParentObject (parentObject, parentTransactionOperations))
            .ToList ();
      }
    }

    public virtual ILoadedObjectData ResolveObjectRelationData (
        DataContainer originatingDataContainer,
        RelationEndPointID relationEndPointID,
        ILoadedObjectDataProvider alreadyLoadedObjectDataProvider)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
      ArgumentUtility.CheckNotNull ("alreadyLoadedObjectDataProvider", alreadyLoadedObjectDataProvider);
      
      if (!relationEndPointID.Definition.IsVirtual)
        throw new ArgumentException ("ResolveObjectRelationData can only be called for virtual end points.", "relationEndPointID");

      using (var parentTransactionOperations = _parentTransactionContext.AccessParentTransaction ())
      {
        var parentRelatedObject = parentTransactionOperations.GetRelatedObject (relationEndPointID);

        if (parentRelatedObject == null)
          return new NullLoadedObjectData ();
        else
          return TransferParentObject (parentRelatedObject, alreadyLoadedObjectDataProvider, parentTransactionOperations);
      }
    }

    public virtual IEnumerable<ILoadedObjectData> ResolveCollectionRelationData (
        RelationEndPointID relationEndPointID,
        ILoadedObjectDataProvider alreadyLoadedObjectDataProvider)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
      ArgumentUtility.CheckNotNull ("alreadyLoadedObjectDataProvider", alreadyLoadedObjectDataProvider);
      
      using (var parentTransactionOperations = _parentTransactionContext.AccessParentTransaction ())
      {
        var parentObjects = parentTransactionOperations.GetRelatedObjects (relationEndPointID);
        // Eager evaluation of sequence to keep parent transaction writeable as shortly as possible
        return parentObjects
            .Select (parentObject => TransferParentObject (parentObject, alreadyLoadedObjectDataProvider, parentTransactionOperations))
            .ToList ();
      }
    }

    public virtual IEnumerable<ILoadedObjectData> ExecuteCollectionQuery (IQuery query, ILoadedObjectDataProvider alreadyLoadedObjectDataProvider)
    {
      ArgumentUtility.CheckNotNull ("query", query);
      ArgumentUtility.CheckNotNull ("alreadyLoadedObjectDataProvider", alreadyLoadedObjectDataProvider);

      using (var parentTransactionOperations = _parentTransactionContext.AccessParentTransaction ())
      {
        var queryResult = parentTransactionOperations.ExecuteCollectionQuery (query);
        Assertion.IsNotNull (queryResult, "Parent transaction never returns a null query result for collection query.");

        var parentObjects = queryResult.AsEnumerable ();

        // Eager evaluation of sequence to keep parent transaction writeable as shortly as possible
        return parentObjects.Select (parentObject => TransferParentObject (parentObject, alreadyLoadedObjectDataProvider, parentTransactionOperations)).ToList ();
      }
    }

    public virtual object ExecuteScalarQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      using (var parentTransactionOperations = _parentTransactionContext.AccessParentTransaction ())
      {
        return parentTransactionOperations.ExecuteScalarQuery (query);
      }
    }

    private ILoadedObjectData TransferParentObject (
        DomainObject parentRelatedObject,
        ILoadedObjectDataProvider alreadyLoadedObjectDataProvider,
        IParentTransactionOperations parentTransactionOperations)
    {
      var existingLoadedObject = alreadyLoadedObjectDataProvider.GetLoadedObject (parentRelatedObject.ID);
      if (existingLoadedObject != null)
        return existingLoadedObject;
      else
        return TransferParentObject (parentRelatedObject, parentTransactionOperations);
    }

    private FreshlyLoadedObjectData TransferParentObject (DomainObject parentObject, IParentTransactionOperations parentTransactionOperations)
    {
      var parentDataContainer = parentTransactionOperations.GetDataContainerWithLazyLoad (parentObject.ID);
      var dataContainer = TransferParentContainer (parentDataContainer);
      return new FreshlyLoadedObjectData (dataContainer);
    }

    private DataContainer TransferParentContainer (DataContainer parentDataContainer)
    {
      if (parentDataContainer.State == StateType.Deleted)
      {
        var message = string.Format ("Object '{0}' is already deleted in the parent transaction.", parentDataContainer.ID);
        throw new ObjectDeletedException (message, parentDataContainer.ID);
      }

      var thisDataContainer = DataContainer.CreateNew (parentDataContainer.ID);

      thisDataContainer.SetPropertyDataFromSubTransaction (parentDataContainer);
      thisDataContainer.SetTimestamp (parentDataContainer.Timestamp);
      thisDataContainer.CommitState (); // for the new DataContainer, the current parent DC state becomes the Unchanged state

      return thisDataContainer;
    }

    public virtual void PersistData (ReadOnlyCollection<PersistableData> data)
    {
      ArgumentUtility.CheckNotNull ("data", data);

      // filter out those items whose state is only Changed due to relation changes
      var dataContainers = data.Select (item => item.DataContainer).Where (dc => dc.State != StateType.Unchanged);

      // only handle changed end-points; end-points of new and deleted objects will implicitly be handled by PersistDataContainers
      var endPoints = data.SelectMany (item => item.GetAssociatedEndPoints ()).Where (ep => ep.HasChanged);

      using (var parentTransactionOperations = _parentTransactionContext.AccessParentTransaction ())
      {
        PersistDataContainers (dataContainers, parentTransactionOperations);
        PersistRelationEndPoints (endPoints, parentTransactionOperations);
      }
    }

    private void PersistDataContainers (IEnumerable<DataContainer> dataContainers, IParentTransactionOperations parentTransactionOperations)
    {
      foreach (var dataContainer in dataContainers)
      {
        Assertion.IsFalse (
            dataContainer.IsDiscarded,
            "dataContainers cannot contain discarded DataContainers, because its items come"
            + "from DataManager.DataContainerMap, which does not contain discarded containers");
        Assertion.IsTrue (dataContainer.State != StateType.Unchanged, "dataContainers cannot contain an unchanged container");
        Assertion.IsTrue (dataContainer.State != StateType.NotLoadedYet, "dataContainers cannot contain an unloaded container");
        Assertion.IsTrue (
            dataContainer.State == StateType.New || dataContainer.State == StateType.Changed
            || dataContainer.State == StateType.Deleted,
            "Invalid dataContainer.State: " + dataContainer.State);

        switch (dataContainer.State)
        {
          case StateType.New:
            PersistNewDataContainer (dataContainer, parentTransactionOperations);
            break;
          case StateType.Changed:
            PersistChangedDataContainer (dataContainer, parentTransactionOperations);
            break;
          case StateType.Deleted:
            PersistDeletedDataContainer (dataContainer, parentTransactionOperations);
            break;
        }
      }
    }

    private void PersistNewDataContainer (DataContainer dataContainer, IParentTransactionOperations parentTransactionOperations)
    {
      Assertion.IsTrue (parentTransactionOperations.IsInvalid (dataContainer.ID));
      parentTransactionOperations.MarkNotInvalid (dataContainer.ID);

      Assertion.IsNull (
          parentTransactionOperations.GetDataContainerWithoutLoading (dataContainer.ID), 
          "a new data container cannot be known to the parent");
      Assertion.IsFalse (dataContainer.IsDiscarded);

      var parentDataContainer = DataContainer.CreateNew (dataContainer.ID);

      parentDataContainer.SetDomainObject (dataContainer.DomainObject);

      parentTransactionOperations.RegisterDataContainer (parentDataContainer);

      parentDataContainer.SetPropertyDataFromSubTransaction (dataContainer);
      if (dataContainer.HasBeenMarkedChanged)
        parentDataContainer.MarkAsChanged ();
      parentDataContainer.SetTimestamp (dataContainer.Timestamp);
    }

    private void PersistChangedDataContainer (DataContainer dataContainer, IParentTransactionOperations parentTransactionOperations)
    {
      DataContainer parentDataContainer = parentTransactionOperations.GetDataContainerWithoutLoading (dataContainer.ID);
      Assertion.IsNotNull (
          parentDataContainer,
          "a changed DataContainer must have been loaded through ParentTransaction, so the "
          + "ParentTransaction must know it");
      Assertion.IsFalse (parentDataContainer.IsDiscarded, "a changed DataContainer cannot be discarded in the ParentTransaction");
      Assertion.IsTrue (parentDataContainer.State != StateType.Deleted, "a changed DataContainer cannot be deleted in the ParentTransaction");
      Assertion.IsTrue (parentDataContainer.DomainObject == dataContainer.DomainObject, "invariant");

      parentDataContainer.SetTimestamp (dataContainer.Timestamp);
      parentDataContainer.SetPropertyDataFromSubTransaction (dataContainer);

      if (dataContainer.HasBeenMarkedChanged && (parentDataContainer.State == StateType.Unchanged || parentDataContainer.State == StateType.Changed))
        parentDataContainer.MarkAsChanged ();
    }

    private void PersistDeletedDataContainer (DataContainer dataContainer, IParentTransactionOperations parentTransactionOperations)
    {
      DataContainer parentDataContainer = parentTransactionOperations.GetDataContainerWithoutLoading (dataContainer.ID);
      Assertion.IsNotNull (
          parentDataContainer,
          "a deleted DataContainer must have been loaded through ParentTransaction, so the ParentTransaction must know it");

      Assertion.IsTrue (
          parentDataContainer.State != StateType.Invalid && parentDataContainer.State != StateType.Deleted,
          "deleted DataContainers cannot be discarded or deleted in the ParentTransaction");
      Assertion.IsTrue (parentDataContainer.DomainObject == dataContainer.DomainObject, "invariant");

      var deleteCommand = parentTransactionOperations.CreateDeleteCommand (dataContainer.DomainObject);
      // no events, no bidirectional changes
      deleteCommand.Perform ();
    }

    private void PersistRelationEndPoints (IEnumerable<IRelationEndPoint> endPoints, IParentTransactionOperations parentTransactionOperations)
    {
      foreach (var endPoint in endPoints)
      {
        var parentEndPoint = parentTransactionOperations.GetRelationEndPointWithoutLoading (endPoint.ID);

        // Because the DataContainers are processed before the RelationEndPoints, the RelationEndPointMaps of both parent and child transaction now
        // contain end points for the same end point IDs. The only scenario in which the ParentTransaction doesn't know an end point known
        // to the child transaction is when the object was of state New in the ParentTransaction and its DataContainer was just discarded.
        // Therefore, we can safely ignore end points unknown to the parent transaction.

        if (parentEndPoint != null)
          parentEndPoint.SetDataFromSubTransaction (endPoint);
      }
    }
  }
}