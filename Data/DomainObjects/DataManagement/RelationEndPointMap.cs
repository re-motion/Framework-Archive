using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public class RelationEndPointMap : ICollectionEndPointChangeDelegate
{
  // types

  // static members and constants

  // member fields

  private ClientTransaction _clientTransaction;
  private RelationEndPointCollection _relationEndPoints;

  // construction and disposing

  public RelationEndPointMap (ClientTransaction clientTransaction)
  {
    ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
    _clientTransaction = clientTransaction;
    _relationEndPoints = new RelationEndPointCollection ();
  }

  // methods and properties

  public RelationEndPoint this[RelationEndPointID endPointID]
  {
    get { return _relationEndPoints[endPointID]; }
  }

  public int Count
  {
    get { return _relationEndPoints.Count; }
  }

  public void Commit ()
  {
    foreach (RelationEndPoint endPoint in _relationEndPoints)
      endPoint.Commit ();
  }

  public void Rollback (DomainObjectCollection newDomainObjects)
  {
    ArgumentUtility.CheckNotNull ("newDomainObjects", newDomainObjects);

    foreach (RelationEndPoint endPoint in _relationEndPoints)
      endPoint.Rollback ();

    foreach (DomainObject newDomainObject in newDomainObjects)
    {
      foreach (RelationEndPointID endPointID in newDomainObject.DataContainer.RelationEndPointIDs)
        _relationEndPoints.Remove (endPointID);
    }
  }

  public DomainObject GetRelatedObject (RelationEndPointID endPointID)
  {
    ArgumentUtility.CheckNotNull ("endPointID", endPointID);

    ObjectEndPoint objectEndPoint = (ObjectEndPoint) _relationEndPoints[endPointID];
    if (objectEndPoint == null)
      return _clientTransaction.LoadRelatedObject (endPointID);
    
    if (objectEndPoint.OppositeObjectID != null)
      return _clientTransaction.GetObject (objectEndPoint.OppositeObjectID);

    return null;
  }

  public DomainObject GetOriginalRelatedObject (RelationEndPointID endPointID)
  {
    ArgumentUtility.CheckNotNull ("endPointID", endPointID);

    ObjectEndPoint objectEndPoint = (ObjectEndPoint) _relationEndPoints[endPointID];
    if (objectEndPoint == null)
      return _clientTransaction.LoadRelatedObject (endPointID);

    if (objectEndPoint.OriginalOppositeObjectID != null)
      return _clientTransaction.GetObject (objectEndPoint.OriginalOppositeObjectID);

    return null;
  }
  
  public DomainObjectCollection GetRelatedObjects (RelationEndPointID endPointID)
  {
    ArgumentUtility.CheckNotNull ("endPointID", endPointID);

    CollectionEndPoint collectionEndPoint = (CollectionEndPoint) _relationEndPoints[endPointID];
    if (collectionEndPoint == null)  
      return _clientTransaction.LoadRelatedObjects (endPointID);

    return collectionEndPoint.OppositeDomainObjects;
  }

  public DomainObjectCollection GetOriginalRelatedObjects (RelationEndPointID endPointID)
  {
    ArgumentUtility.CheckNotNull ("endPointID", endPointID);

    CollectionEndPoint collectionEndPoint = (CollectionEndPoint) _relationEndPoints[endPointID];
    if (collectionEndPoint == null)
      return _clientTransaction.LoadRelatedObjects (endPointID); 

    return collectionEndPoint.OriginalOppositeDomainObjects;
  }

  public void SetRelatedObject (RelationEndPointID endPointID, DomainObject newRelatedObject)
  {
    ArgumentUtility.CheckNotNull ("endPointID", endPointID);

    RelationEndPoint endPoint = GetRelationEndPointWithLazyLoad (endPointID);

    RelationEndPoint newRelatedEndPoint = GetRelationEndPoint (
        newRelatedObject, endPoint.OppositeEndPointDefinition);

    RelationEndPoint oldRelatedEndPoint = GetRelationEndPoint (
        GetRelatedObject (endPointID), newRelatedEndPoint.Definition);

    if (object.ReferenceEquals (newRelatedEndPoint.GetDomainObject (), oldRelatedEndPoint.GetDomainObject ()))
      return;

    if (newRelatedEndPoint.Definition.Cardinality == CardinalityType.One)
    {
      SetRelatedObjectForOneToOneRelation (
          (ObjectEndPoint) endPoint, 
          (ObjectEndPoint) newRelatedEndPoint, 
          (ObjectEndPoint) oldRelatedEndPoint);
    }
    else
      SetRelatedObjectForOneToManyRelation (
          (ObjectEndPoint) endPoint, 
          newRelatedEndPoint, 
          oldRelatedEndPoint);
  }

  private void SetRelatedObjectForOneToOneRelation (
      ObjectEndPoint endPoint, 
      ObjectEndPoint newRelatedEndPoint,
      ObjectEndPoint oldRelatedEndPoint)
  {
    ObjectEndPoint oldRelatedEndPointOfNewRelatedEndPoint = (ObjectEndPoint)
        RelationEndPoint.CreateNullRelationEndPoint (endPoint.Definition);

    if (!newRelatedEndPoint.IsNull)
    {
      oldRelatedEndPointOfNewRelatedEndPoint = (ObjectEndPoint) GetRelationEndPoint (
          GetRelatedObject (newRelatedEndPoint.ID), endPoint.Definition);
    }

    if (BeginRelationChange (endPoint, newRelatedEndPoint, oldRelatedEndPoint, oldRelatedEndPointOfNewRelatedEndPoint))
    {
      WriteAssociatedPropertiesForRelationChange (
          endPoint, 
          newRelatedEndPoint, 
          oldRelatedEndPoint, 
          oldRelatedEndPointOfNewRelatedEndPoint);

      ChangeLinks (endPoint, newRelatedEndPoint, oldRelatedEndPoint, oldRelatedEndPointOfNewRelatedEndPoint);

      EndRelationChange (
          endPoint, newRelatedEndPoint, oldRelatedEndPoint, oldRelatedEndPointOfNewRelatedEndPoint);
    }
  }

  private void SetRelatedObjectForOneToManyRelation (
      ObjectEndPoint endPoint, 
      RelationEndPoint newRelatedEndPoint,
      RelationEndPoint oldRelatedEndPoint)
  {
    if (!newRelatedEndPoint.IsNull)
    {
      DomainObjectCollection collection = GetRelatedObjects (newRelatedEndPoint.ID);
      collection.Add (endPoint.GetDomainObject ());
    }
    else
    {
      DomainObjectCollection collection = GetRelatedObjects (oldRelatedEndPoint.ID);
      collection.Remove (endPoint.GetDomainObject ());
    }
  }

  private bool BeginRelationChange (
      RelationEndPoint endPoint,
      RelationEndPoint newRelatedEndPoint,
      RelationEndPoint oldRelatedEndPoint,
      RelationEndPoint oldRelatedEndPointOfNewRelatedEndPoint)
  {
    return endPoint.BeginRelationChange (oldRelatedEndPoint, newRelatedEndPoint)
        && oldRelatedEndPoint.BeginRelationChange (endPoint, RelationEndPoint.CreateNullRelationEndPoint (endPoint.Definition))
        && newRelatedEndPoint.BeginRelationChange (oldRelatedEndPointOfNewRelatedEndPoint, endPoint)
        && oldRelatedEndPointOfNewRelatedEndPoint.BeginRelationChange (newRelatedEndPoint, RelationEndPoint.CreateNullRelationEndPoint (newRelatedEndPoint.Definition));
  }

  private void EndRelationChange (
      RelationEndPoint endPoint,
      RelationEndPoint newRelatedEndPoint,
      RelationEndPoint oldRelatedEndPoint,
      RelationEndPoint oldRelatedEndPointOfNewRelatedEndPoint)
  {
    endPoint.EndRelationChange ();
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

    if (_relationEndPoints.Contains (endPointID))
      return _relationEndPoints[endPointID];

    if (endPointID.Definition.Cardinality == CardinalityType.One)
      _clientTransaction.LoadRelatedObject (endPointID);
    else
      _clientTransaction.LoadRelatedObjects (endPointID);

    return _relationEndPoints[endPointID];
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

  public RelationEndPointCollection GetAllRelationEndPointsWithLazyLoad (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);

    RelationEndPointCollection allRelationEndPoints = new RelationEndPointCollection ();

    foreach (RelationEndPointID endPointID in domainObject.DataContainer.RelationEndPointIDs)
    {
      RelationEndPoint endPoint = GetRelationEndPointWithLazyLoad (endPointID);

      allRelationEndPoints.Add (endPoint);
      allRelationEndPoints.Merge (_relationEndPoints.GetOppositeRelationEndPoints (endPoint));
    }

    return allRelationEndPoints;
  }

  private void ChangeLinks (
      ObjectEndPoint endPoint,
      ObjectEndPoint newRelatedEndPoint,
      ObjectEndPoint oldRelatedEndPoint,
      ObjectEndPoint oldRelatedEndPointOfNewRelatedEndPoint)
  {
    ArgumentUtility.CheckNotNull ("endPoint", endPoint);
    ArgumentUtility.CheckNotNull ("newRelatedEndPoint", newRelatedEndPoint);
    ArgumentUtility.CheckNotNull ("oldRelatedEndPoint", oldRelatedEndPoint);
    ArgumentUtility.CheckNotNull ("oldRelatedEndPointOfNewRelatedEndPoint", oldRelatedEndPointOfNewRelatedEndPoint);

    if (!newRelatedEndPoint.IsNull)
    {
      ChangeLink (endPoint.ID, newRelatedEndPoint.GetDomainObject ());
      ChangeLink (newRelatedEndPoint.ID, endPoint.GetDomainObject ());
    }
    else
    {
      ChangeLink (endPoint.ID, null);
    }

    if (!oldRelatedEndPoint.IsNull)
      ChangeLink (oldRelatedEndPoint.ID, null);

    if (!oldRelatedEndPointOfNewRelatedEndPoint.IsNull)
      ChangeLink (oldRelatedEndPointOfNewRelatedEndPoint.ID, null);
  }

  public void ChangeLink (RelationEndPointID endPointID, DomainObject newRelatedObject)
  {
    ArgumentUtility.CheckNotNull ("endPointID", endPointID);

    ObjectEndPoint endPoint = (ObjectEndPoint) _relationEndPoints[endPointID];

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
        RelationEndPoint endPoint = _relationEndPoints[endPointID];
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
      ObjectEndPoint endPoint,
      ObjectEndPoint newRelatedEndPoint,
      ObjectEndPoint oldRelatedEndPoint,
      ObjectEndPoint oldRelatedEndPointOfNewRelatedEndPoint)
  {
    ArgumentUtility.CheckNotNull ("endPoint", endPoint);
    ArgumentUtility.CheckNotNull ("newRelatedEndPoint", newRelatedEndPoint);
    ArgumentUtility.CheckNotNull ("oldRelatedEndPoint", oldRelatedEndPoint);
    ArgumentUtility.CheckNotNull ("oldRelatedEndPointOfNewRelatedEndPoint", oldRelatedEndPointOfNewRelatedEndPoint);

    endPoint.SetOppositeEndPoint (newRelatedEndPoint);
    newRelatedEndPoint.SetOppositeEndPoint (endPoint);
    oldRelatedEndPoint.SetOppositeEndPoint (RelationEndPoint.CreateNullRelationEndPoint (endPoint.Definition));

    oldRelatedEndPointOfNewRelatedEndPoint.SetOppositeEndPoint (
        RelationEndPoint.CreateNullRelationEndPoint (newRelatedEndPoint.Definition));
  }

  public bool HasRelationChanged (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    foreach (RelationEndPointID endPointID in dataContainer.RelationEndPointIDs)
    {
      RelationEndPoint endPoint = _relationEndPoints[endPointID];
      if (endPoint != null && endPoint.HasChanged)
        return true;
    }

    return false;
  }

  private void Add (RelationEndPoint endPoint)
  {
    ArgumentUtility.CheckNotNull ("endPoint", endPoint);
    if (endPoint.IsNull)
      throw new ArgumentNullException ("endPoint", "A NullRelationEndPoint cannot be added to a RelationEndPointMap.");

    _relationEndPoints.Add (endPoint);
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
}
}
