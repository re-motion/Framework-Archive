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

  public void Delete (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);

    foreach (RelationEndPointID endPointID in domainObject.DataContainer.RelationEndPointIDs)
    {
      RelationEndPoint endPoint = GetRelationEndPointWithLazyLoad (endPointID);

      RelationEndPointCollection allOppositeEndPoints = 
          _relationEndPoints.GetOppositeRelationEndPoints (endPoint);

      foreach (RelationEndPoint oppositeEndPoint in allOppositeEndPoints)
      {
        if (oppositeEndPoint.Definition.Cardinality == CardinalityType.One)
        {
          ((ObjectEndPoint) oppositeEndPoint).SetOppositeEndPoint (
              RelationEndPoint.CreateNullRelationEndPoint (oppositeEndPoint.OppositeEndPointDefinition));
        }
        else
        {
          ((CollectionEndPoint) oppositeEndPoint).OppositeDomainObjects.PerformRemove (domainObject);
        }
      }

      if (endPointID.Definition.Cardinality == CardinalityType.One)
      {
        ((ObjectEndPoint) endPoint).SetOppositeEndPoint (
            RelationEndPoint.CreateNullRelationEndPoint (endPoint.OppositeEndPointDefinition));
      }
      else
      {
        ((CollectionEndPoint) endPoint).OppositeDomainObjects.PerformClear ();
      }
    }

    /*
      foreach (RelationEndPointID endPointID in domainObject.DataContainer.RelationEndPointIDs)
      {
        RelationEndPoint relationEndPoint = GetRelationEndPoint (endPointID);
        if (relationEndPoint.Definition.Cardinality == CardinalityType.One)
        {
          // TODO: opposite object can be null!
          DomainObject oppositeDomainObject = GetRelatedObject (relationEndPoint);
          RelationEndPoint oppositeEndPoint = GetRelationEndPoint (oppositeDomainObject, relationEndPoint.OppositeEndPointDefinition);

          _dataManager.WriteAssociatedPropertiesForRelationChange (
              relationEndPoint, 
              new NullRelationEndPoint (oppositeEndPoint.Definition), 
              oppositeEndPoint, 
              new NullRelationEndPoint (relationEndPoint.Definition));

          _dataManager.ChangeLinks (
            relationEndPoint, 
              new NullRelationEndPoint (oppositeEndPoint.Definition), 
              oppositeEndPoint, 
              new NullRelationEndPoint (relationEndPoint.Definition));    
        }
        else
        {
          // TODO: visit every domain object of opposite collection        
        }
      }
      */
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

  public void RegisterExistingDataContainer (DataContainer dataContainer)
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

  public void RegisterNewDataContainer (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    foreach (RelationEndPointID endPointID in dataContainer.RelationEndPointIDs)
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

  public RelationEndPointCollection GetAllRelationEndPointsWithLazyLoad (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);

    RelationEndPointCollection allRelationEndPoints = new RelationEndPointCollection ();

    foreach (RelationEndPointID endPointID in domainObject.DataContainer.RelationEndPointIDs)
    {
      RelationEndPoint endPoint = GetRelationEndPointWithLazyLoad (endPointID);
      
      if (endPoint.OppositeEndPointDefinition.Cardinality == CardinalityType.Many)
      {
        ObjectEndPoint objectEndPoint = (ObjectEndPoint) endPoint;
        if (objectEndPoint.OppositeObjectID != null)
        {
          RelationEndPointID oppositeEndPointID = 
              new RelationEndPointID (objectEndPoint.OppositeObjectID, objectEndPoint.OppositeEndPointDefinition);

          GetRelatedObjects (oppositeEndPointID);
        }
      }

      allRelationEndPoints.Add (endPoint);
      allRelationEndPoints.Merge (_relationEndPoints.GetOppositeRelationEndPoints (endPoint));
    }

    return allRelationEndPoints;
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

  private RelationEndPoint GetRelationEndPoint (DomainObject domainObject, IRelationEndPointDefinition definition)
  {
    ArgumentUtility.CheckNotNull ("definition", definition);

    if (domainObject != null)
      return GetRelationEndPointWithLazyLoad (new RelationEndPointID (domainObject.ID, definition));
    else
      return RelationEndPoint.CreateNullRelationEndPoint (definition); 
  }

  private RelationEndPoint GetRelationEndPointWithLazyLoad (RelationEndPointID endPointID)
  {
    if (_relationEndPoints.Contains (endPointID))
      return _relationEndPoints[endPointID];

    if (endPointID.Definition.Cardinality == CardinalityType.One)
      _clientTransaction.LoadRelatedObject (endPointID);
    else
      _clientTransaction.LoadRelatedObjects (endPointID);

    return _relationEndPoints[endPointID];
  }

  private void ChangeLinks (
      ObjectEndPoint endPoint,
      ObjectEndPoint newRelatedEndPoint,
      ObjectEndPoint oldRelatedEndPoint,
      ObjectEndPoint oldRelatedEndPointOfNewRelatedEndPoint)
  {
    ChangeLinkBidirectional (endPoint, newRelatedEndPoint);

    ChangeLinkBidirectional (
        oldRelatedEndPoint, 
        RelationEndPoint.CreateNullRelationEndPoint (oldRelatedEndPoint.OppositeEndPointDefinition));

    ChangeLinkBidirectional (
        oldRelatedEndPointOfNewRelatedEndPoint, 
        RelationEndPoint.CreateNullRelationEndPoint (oldRelatedEndPointOfNewRelatedEndPoint.OppositeEndPointDefinition));
  }

  private void ChangeLinkBidirectional (ObjectEndPoint endPoint, RelationEndPoint newRelatedEndPoint)
  {
    endPoint.SetOppositeEndPoint (newRelatedEndPoint);

    ObjectEndPoint newObjectEndPoint = newRelatedEndPoint as ObjectEndPoint;
    if (newObjectEndPoint != null)
      newObjectEndPoint.SetOppositeEndPoint (endPoint);
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
      ChangeLinkBidirectional (addingEndPoint, endPoint);
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
      ChangeLinkBidirectional (
          removingEndPoint, 
          RelationEndPoint.CreateNullRelationEndPoint (removingEndPoint.OppositeEndPointDefinition));
      
      endPoint.OppositeDomainObjects.PerformRemove (domainObject);

      removingEndPoint.EndRelationChange ();
      endPoint.EndRelationChange ();
    }
  }

  #endregion
}
}
