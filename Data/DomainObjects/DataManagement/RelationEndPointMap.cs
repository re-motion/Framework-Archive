using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public class RelationEndPointMap : RelationEndPointCollection, ICollectionEndPointChangeDelegate, ICloneable
{
  // types

  // static members and constants

  // member fields

  private ClientTransaction _clientTransaction;

  // construction and disposing

  public RelationEndPointMap (ClientTransaction clientTransaction)
  {
    Initialize (clientTransaction);
  }

  public RelationEndPointMap (
      ClientTransaction clientTransaction,
      RelationEndPointCollection collection, 
      bool isCollectionReadOnly) 
      : base (collection, isCollectionReadOnly)
  {
    Initialize (clientTransaction);
  }


  private void Initialize (ClientTransaction clientTransaction)
  {
    ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
    _clientTransaction = clientTransaction;
  }

  // methods and properties

  public void Commit ()
  {
    foreach (RelationEndPoint endPoint in this)
      endPoint.Commit ();
  }

  public void Rollback (DomainObjectCollection newDomainObjects)
  {
    ArgumentUtility.CheckNotNull ("newDomainObjects", newDomainObjects);

    foreach (RelationEndPoint endPoint in this)
      endPoint.Rollback ();

    foreach (DomainObject newDomainObject in newDomainObjects)
    {
      foreach (RelationEndPointID endPointID in newDomainObject.DataContainer.RelationEndPointIDs)
        Remove (endPointID);
    }
  }

  public DomainObject GetRelatedObject (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    ObjectEndPoint objectEndPoint = (ObjectEndPoint) this[relationEndPointID];
    if (objectEndPoint == null)
      return _clientTransaction.LoadRelatedObject (relationEndPointID);
    
    if (objectEndPoint.OppositeObjectID != null)
      return _clientTransaction.GetObject (objectEndPoint.OppositeObjectID);

    return null;
  }

  public DomainObject GetOriginalRelatedObject (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    ObjectEndPoint objectEndPoint = (ObjectEndPoint) this[relationEndPointID];
    if (objectEndPoint == null)
      return _clientTransaction.LoadRelatedObject (relationEndPointID);

    if (objectEndPoint.OriginalOppositeObjectID != null)
      return _clientTransaction.GetObject (objectEndPoint.OriginalOppositeObjectID);

    return null;
  }
  
  public DomainObjectCollection GetRelatedObjects (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    CollectionEndPoint collectionEndPoint = (CollectionEndPoint) this[relationEndPointID];
    if (collectionEndPoint == null)  
      return _clientTransaction.LoadRelatedObjects (relationEndPointID);

    return collectionEndPoint.OppositeDomainObjects;
  }

  public DomainObjectCollection GetOriginalRelatedObjects (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    CollectionEndPoint collectionEndPoint = (CollectionEndPoint) this[relationEndPointID];
    if (collectionEndPoint == null)
      return _clientTransaction.LoadRelatedObjects (relationEndPointID); 

    return collectionEndPoint.OriginalOppositeDomainObjects;
  }

  public void SetRelatedObject (RelationEndPointID relationEndPointID, DomainObject newRelatedObject)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    RelationEndPoint relationEndPoint = GetRelationEndPointWithLazyLoad (relationEndPointID);

    RelationEndPoint newRelatedEndPoint = GetRelationEndPoint (
        newRelatedObject, relationEndPoint.OppositeEndPointDefinition);

    RelationEndPoint oldRelatedEndPoint = GetRelationEndPoint (
        GetRelatedObject (relationEndPointID), newRelatedEndPoint.Definition);

    if (object.ReferenceEquals (newRelatedEndPoint.GetDomainObject (), oldRelatedEndPoint.GetDomainObject ()))
      return;

    if (newRelatedEndPoint.Definition.Cardinality == CardinalityType.One)
    {
      SetRelatedObjectForOneToOneRelation (
          (ObjectEndPoint) relationEndPoint, 
          (ObjectEndPoint) newRelatedEndPoint, 
          (ObjectEndPoint) oldRelatedEndPoint);
    }
    else
      SetRelatedObjectForOneToManyRelation (
          (ObjectEndPoint) relationEndPoint, 
          newRelatedEndPoint, 
          oldRelatedEndPoint);
  }

  private void SetRelatedObjectForOneToOneRelation (
      ObjectEndPoint relationEndPoint, 
      ObjectEndPoint newRelatedEndPoint,
      ObjectEndPoint oldRelatedEndPoint)
  {
    ObjectEndPoint oldRelatedEndPointOfNewRelatedEndPoint = (ObjectEndPoint)
        RelationEndPoint.CreateNullRelationEndPoint (relationEndPoint.Definition);

    if (!newRelatedEndPoint.IsNull)
    {
      oldRelatedEndPointOfNewRelatedEndPoint = (ObjectEndPoint) GetRelationEndPoint (
          GetRelatedObject (newRelatedEndPoint.ID), relationEndPoint.Definition);
    }

    if (BeginRelationChange (relationEndPoint, newRelatedEndPoint, oldRelatedEndPoint, oldRelatedEndPointOfNewRelatedEndPoint))
    {
      WriteAssociatedPropertiesForRelationChange (
          relationEndPoint, 
          newRelatedEndPoint, 
          oldRelatedEndPoint, 
          oldRelatedEndPointOfNewRelatedEndPoint);

      ChangeLinks (relationEndPoint, newRelatedEndPoint, oldRelatedEndPoint, oldRelatedEndPointOfNewRelatedEndPoint);

      EndRelationChange (
          relationEndPoint, newRelatedEndPoint, oldRelatedEndPoint, oldRelatedEndPointOfNewRelatedEndPoint);
    }
  }

  private void SetRelatedObjectForOneToManyRelation (
      ObjectEndPoint relationEndPoint, 
      RelationEndPoint newRelatedEndPoint,
      RelationEndPoint oldRelatedEndPoint)
  {
    if (!newRelatedEndPoint.IsNull)
    {
      DomainObjectCollection collection = GetRelatedObjects (newRelatedEndPoint.ID);
      collection.Add (relationEndPoint.GetDomainObject ());
    }
    else
    {
      DomainObjectCollection collection = GetRelatedObjects (oldRelatedEndPoint.ID);
      collection.Remove (relationEndPoint.GetDomainObject ());
    }
  }

  private bool BeginRelationChange (
      RelationEndPoint relationEndPoint,
      RelationEndPoint newRelatedEndPoint,
      RelationEndPoint oldRelatedEndPoint,
      RelationEndPoint oldRelatedEndPointOfNewRelatedEndPoint)
  {
    return relationEndPoint.BeginRelationChange (oldRelatedEndPoint, newRelatedEndPoint)
        && oldRelatedEndPoint.BeginRelationChange (relationEndPoint, RelationEndPoint.CreateNullRelationEndPoint (relationEndPoint.Definition))
        && newRelatedEndPoint.BeginRelationChange (oldRelatedEndPointOfNewRelatedEndPoint, relationEndPoint)
        && oldRelatedEndPointOfNewRelatedEndPoint.BeginRelationChange (newRelatedEndPoint, RelationEndPoint.CreateNullRelationEndPoint (newRelatedEndPoint.Definition));
  }

  private void EndRelationChange (
      RelationEndPoint relationEndPoint,
      RelationEndPoint newRelatedEndPoint,
      RelationEndPoint oldRelatedEndPoint,
      RelationEndPoint oldRelatedEndPointOfNewRelatedEndPoint)
  {
    relationEndPoint.EndRelationChange ();
    oldRelatedEndPoint.EndRelationChange ();
    newRelatedEndPoint.EndRelationChange ();
    oldRelatedEndPointOfNewRelatedEndPoint.EndRelationChange ();
  }

  public RelationEndPoint GetRelationEndPoint (DomainObject domainObject, IRelationEndPointDefinition definition)
  {
    // TODO: This method probably should be private.
    //       Name this method differently.
    ArgumentUtility.CheckNotNull ("definition", definition);

    if (domainObject != null)
      return GetRelationEndPointWithLazyLoad (new RelationEndPointID (domainObject.ID, definition));
    else
      return RelationEndPoint.CreateNullRelationEndPoint (definition); 
  }

  public RelationEndPoint GetRelationEndPointWithLazyLoad (RelationEndPointID endPointID)
  {
    // TODO: This method probably should be private.
    ArgumentUtility.CheckNotNull ("endPointID", endPointID);

    if (Contains (endPointID))
      return this[endPointID];

    if (endPointID.Definition.Cardinality == CardinalityType.One)
      _clientTransaction.LoadRelatedObject (endPointID);
    else
      _clientTransaction.LoadRelatedObjects (endPointID);

    return this[endPointID];
  }

  public void RegisterObjectEndPoint (RelationEndPointID endPointID, ObjectID oppositeObjectID)
  {
    ArgumentUtility.CheckNotNull ("endPointID", endPointID);
    Add (new ObjectEndPoint (_clientTransaction, endPointID, oppositeObjectID));
  }

  public void RegisterCollectionEndPoint (RelationEndPointID endPointID, DomainObjectCollection domainObjects)
  {
    ArgumentUtility.CheckNotNull ("endPointID", endPointID);
    ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

    CollectionEndPoint collectionEndPoint = new CollectionEndPoint (_clientTransaction, endPointID, domainObjects);
    collectionEndPoint.ChangeDelegate = this;
    Add (collectionEndPoint);
  }

  public override void Add (RelationEndPoint endPoint)
  {
    ArgumentUtility.CheckNotNull ("endPoint", endPoint);
    if (endPoint.IsNull)
      throw new ArgumentNullException ("endPoint", "A NullRelationEndPoint cannot be added to a RelationEndPointMap.");

    base.Add (endPoint);
  }

  public void Register (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    ClassDefinition classDefinition = dataContainer.ClassDefinition;

    foreach (RelationDefinition relationDefinition in classDefinition.GetAllRelationDefinitions ())
    {
      foreach (IRelationEndPointDefinition endPointDefinition in relationDefinition.EndPointDefinitions)
      {
        if (!endPointDefinition.IsVirtual)
        {
          if (classDefinition.IsRelationEndPoint (endPointDefinition))
          {
            ObjectID oppositeObjectID = dataContainer.GetObjectID (endPointDefinition.PropertyName);
            
            ObjectEndPoint endPoint = new ObjectEndPoint (
                _clientTransaction, dataContainer, endPointDefinition, oppositeObjectID);
            
            Add (endPoint);

            if (endPoint.OppositeEndPointDefinition.Cardinality == CardinalityType.One && endPoint.OppositeObjectID != null)
            {
              ObjectEndPoint oppositeEndPoint = new ObjectEndPoint (
                  _clientTransaction, 
                  endPoint.OppositeObjectID, 
                  endPoint.OppositeEndPointDefinition, 
                  endPoint.ObjectID);

              Add (oppositeEndPoint);
            }
          }
        }
      }
    }
  }

  public RelationEndPointMap CloneForDelete (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);

    foreach (RelationEndPointID endPointID in domainObject.DataContainer.RelationEndPointIDs)
      GetRelationEndPointWithLazyLoad (endPointID);

    return (RelationEndPointMap) this.Clone ();
  }

  public RelationEndPointCollection GetAllRelationEndPointsWithLazyLoad (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);

    RelationEndPointCollection allRelationEndPoints = new RelationEndPointCollection ();

    foreach (RelationEndPointID endPointID in domainObject.DataContainer.RelationEndPointIDs)
    {
      RelationEndPoint endPoint = GetRelationEndPointWithLazyLoad (endPointID);

      allRelationEndPoints.Add (endPoint);
      allRelationEndPoints.Merge (GetOppositeRelationEndPoints (endPoint));
    }

    return allRelationEndPoints;
  }

  private void ChangeLinks (
      ObjectEndPoint relationEndPoint,
      ObjectEndPoint newRelatedEndPoint,
      ObjectEndPoint oldRelatedEndPoint,
      ObjectEndPoint oldRelatedEndPointOfNewRelatedEndPoint)
  {
    ArgumentUtility.CheckNotNull ("relationEndPoint", relationEndPoint);
    ArgumentUtility.CheckNotNull ("newRelatedEndPoint", newRelatedEndPoint);
    ArgumentUtility.CheckNotNull ("oldRelatedEndPoint", oldRelatedEndPoint);
    ArgumentUtility.CheckNotNull ("oldRelatedEndPointOfNewRelatedEndPoint", oldRelatedEndPointOfNewRelatedEndPoint);

    if (!newRelatedEndPoint.IsNull)
    {
      ChangeLink (relationEndPoint.ID, newRelatedEndPoint.GetDomainObject ());
      ChangeLink (newRelatedEndPoint.ID, relationEndPoint.GetDomainObject ());
    }
    else
    {
      ChangeLink (relationEndPoint.ID, null);
    }

    if (!oldRelatedEndPoint.IsNull)
      ChangeLink (oldRelatedEndPoint.ID, null);

    if (!oldRelatedEndPointOfNewRelatedEndPoint.IsNull)
      ChangeLink (oldRelatedEndPointOfNewRelatedEndPoint.ID, null);
  }

  public void ChangeLink (RelationEndPointID id, DomainObject newRelatedObject)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    ObjectEndPoint endPoint = (ObjectEndPoint) this[id];

    if (newRelatedObject != null)
      endPoint.OppositeObjectID = newRelatedObject.ID;
    else
      endPoint.OppositeObjectID = null;
  }

  public void CheckMandatoryRelations (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);

    foreach (RelationEndPointID endPointID in domainObject.DataContainer.RelationEndPointIDs)
    {
      if (endPointID.Definition.IsMandatory)
      {
        RelationEndPoint endPoint = this[endPointID];
        if (endPoint != null)
          endPoint.CheckMandatory ();
      }
    }
  }

  public void RegisterNewDataContainer (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    foreach (RelationEndPointID endPointID in dataContainer.RelationEndPointIDs)
    {
      if (endPointID.Definition.IsVirtual)
      {
        if (endPointID.Definition.Cardinality == CardinalityType.One) 
        {
          RegisterObjectEndPoint (endPointID, null);
        }
        else
        {
          DomainObjectCollection domainObjects = DomainObjectCollection.Create (endPointID.Definition.PropertyType);
          RegisterCollectionEndPoint (endPointID, domainObjects);
        }
      }
    }
  }

  private void WriteAssociatedPropertiesForRelationChange (
      ObjectEndPoint relationEndPoint,
      ObjectEndPoint newRelatedEndPoint,
      ObjectEndPoint oldRelatedEndPoint,
      ObjectEndPoint oldRelatedEndPointOfNewRelatedEndPoint)
  {
    ArgumentUtility.CheckNotNull ("relationEndPoint", relationEndPoint);
    ArgumentUtility.CheckNotNull ("newRelatedEndPoint", newRelatedEndPoint);
    ArgumentUtility.CheckNotNull ("oldRelatedEndPoint", oldRelatedEndPoint);
    ArgumentUtility.CheckNotNull ("oldRelatedEndPointOfNewRelatedEndPoint", oldRelatedEndPointOfNewRelatedEndPoint);

    relationEndPoint.SetOppositeEndPoint (newRelatedEndPoint);
    newRelatedEndPoint.SetOppositeEndPoint (relationEndPoint);
    oldRelatedEndPoint.SetOppositeEndPoint (RelationEndPoint.CreateNullRelationEndPoint (relationEndPoint.Definition));

    oldRelatedEndPointOfNewRelatedEndPoint.SetOppositeEndPoint (
        RelationEndPoint.CreateNullRelationEndPoint (newRelatedEndPoint.Definition));
  }

  public bool HasRelationChanged (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    foreach (RelationEndPointID endPointID in dataContainer.RelationEndPointIDs)
    {
      RelationEndPoint endPoint = this[endPointID];
      if (endPoint != null && endPoint.HasChanged)
        return true;
    }

    return false;
  }

  #region ICollectionEndPointChangeDelegate Members

  void ICollectionEndPointChangeDelegate.PerformAdd  (CollectionEndPoint endPoint, DomainObject domainObject)
  {
    ObjectEndPoint addingEndPoint = (ObjectEndPoint) GetRelationEndPoint (
        domainObject, endPoint.OppositeEndPointDefinition);

    RelationEndPoint oldRelatedEndPoint = (CollectionEndPoint) GetRelationEndPoint (
        _clientTransaction.GetRelatedObject (addingEndPoint.ID), endPoint.Definition);

    DomainObjectCollection oldCollection = null;
    if (!oldRelatedEndPoint.IsNull)
      oldCollection = _clientTransaction.GetRelatedObjects (oldRelatedEndPoint.ID);
    
    if (addingEndPoint.BeginRelationChange (oldRelatedEndPoint, endPoint)
        && oldRelatedEndPoint.BeginRelationChange (GetRelationEndPoint (domainObject, endPoint.OppositeEndPointDefinition))
        && endPoint.BeginRelationChange (RelationEndPoint.CreateNullRelationEndPoint (addingEndPoint.Definition), addingEndPoint))
    {
      addingEndPoint.SetOppositeEndPoint (endPoint);
      ChangeLink (addingEndPoint.ID, endPoint.GetDomainObject ());
      endPoint.OppositeDomainObjects.PerformAdd (domainObject);

      if (oldCollection != null)
        oldCollection.PerformRemove (domainObject);

      addingEndPoint.EndRelationChange ();
      oldRelatedEndPoint.EndRelationChange ();
      endPoint.EndRelationChange ();
    }
  }

  void ICollectionEndPointChangeDelegate.PerformRemove (CollectionEndPoint endPoint, DomainObject domainObject)
  {
    ObjectEndPoint removingEndPoint = (ObjectEndPoint) GetRelationEndPoint (domainObject, endPoint.OppositeEndPointDefinition);

    if (removingEndPoint.BeginRelationChange (endPoint)
        && endPoint.BeginRelationChange (GetRelationEndPoint (domainObject, endPoint.OppositeEndPointDefinition)))
    {
      removingEndPoint.SetOppositeEndPoint (RelationEndPoint.CreateNullRelationEndPoint (endPoint.Definition));
      ChangeLink (removingEndPoint.ID, null);
      endPoint.OppositeDomainObjects.PerformRemove (domainObject);

      removingEndPoint.EndRelationChange ();
      endPoint.EndRelationChange ();
    }
  }

  #endregion

  #region ICloneable Members

  public object Clone ()
  {
    RelationEndPointMap clone = new RelationEndPointMap (_clientTransaction);

    foreach (RelationEndPoint endPoint in this)
      clone.Add (endPoint);

    return clone;
  }

  #endregion
}
}
