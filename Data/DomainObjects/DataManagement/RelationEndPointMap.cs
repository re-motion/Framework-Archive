using System;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.DataManagement
{
[Serializable]
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

  public void Commit (DomainObjectCollection deletedDomainObjects)
  {
    ArgumentUtility.CheckNotNull ("deletedDomainObjects", deletedDomainObjects);

    foreach (RelationEndPoint endPoint in _relationEndPoints)
      endPoint.Commit ();

    foreach (DomainObject deletedDomainObject in deletedDomainObjects)
    {
      foreach (RelationEndPointID endPointID in deletedDomainObject.DataContainer.RelationEndPointIDs)
        _relationEndPoints.Remove (endPointID);
    }
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

  public void PerformDelete (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    CheckClientTransactionForDeletion (domainObject);

    foreach (RelationEndPointID endPointID in domainObject.DataContainer.RelationEndPointIDs)
    {
      RelationEndPoint endPoint = GetRelationEndPointWithLazyLoad (endPointID);

      RelationEndPointCollection allOppositeEndPoints = 
          _relationEndPoints.GetOppositeRelationEndPoints (endPoint);

      foreach (RelationEndPoint oppositeEndPoint in allOppositeEndPoints)
        oppositeEndPoint.PerformRelationChange ();

      endPoint.PerformDelete ();

      if (domainObject.State == StateType.New)
        _relationEndPoints.Remove (endPointID);
    }
  }

  public DomainObject GetRelatedObject (RelationEndPointID endPointID)
  {
    ArgumentUtility.CheckNotNull ("endPointID", endPointID);
    CheckCardinality (endPointID, CardinalityType.One, "GetRelatedObject", "endPointID");

    ObjectEndPoint objectEndPoint = (ObjectEndPoint) _relationEndPoints[endPointID];
    if (objectEndPoint == null)
      return _clientTransaction.LoadRelatedObject (endPointID);
    
    if (objectEndPoint.OppositeObjectID == null)
      return null;

    return _clientTransaction.GetObject (objectEndPoint.OppositeObjectID, false);
  }

  public DomainObject GetOriginalRelatedObject (RelationEndPointID endPointID)
  {
    ArgumentUtility.CheckNotNull ("endPointID", endPointID);
    CheckCardinality (endPointID, CardinalityType.One, "GetOriginalRelatedObject", "endPointID");

    ObjectEndPoint objectEndPoint = (ObjectEndPoint) _relationEndPoints[endPointID];
    if (objectEndPoint == null)
      return _clientTransaction.LoadRelatedObject (endPointID);

    if (objectEndPoint.OriginalOppositeObjectID == null)
      return null;

    return _clientTransaction.GetObject (objectEndPoint.OriginalOppositeObjectID, true);
  }
  
  public DomainObjectCollection GetRelatedObjects (RelationEndPointID endPointID)
  {
    ArgumentUtility.CheckNotNull ("endPointID", endPointID);
    CheckCardinality (endPointID, CardinalityType.Many, "GetRelatedObjects", "endPointID");

    CollectionEndPoint collectionEndPoint = (CollectionEndPoint) _relationEndPoints[endPointID];
    if (collectionEndPoint == null)  
      return _clientTransaction.LoadRelatedObjects (endPointID);

    return collectionEndPoint.OppositeDomainObjects;
  }

  public DomainObjectCollection GetOriginalRelatedObjects (RelationEndPointID endPointID)
  {
    ArgumentUtility.CheckNotNull ("endPointID", endPointID);
    CheckCardinality (endPointID, CardinalityType.Many, "GetOriginalRelatedObjects", "endPointID");

    CollectionEndPoint collectionEndPoint = (CollectionEndPoint) _relationEndPoints[endPointID];
    if (collectionEndPoint == null)
    {
      _clientTransaction.LoadRelatedObjects (endPointID); 
      collectionEndPoint = (CollectionEndPoint) _relationEndPoints[endPointID];
    }

    return collectionEndPoint.OriginalOppositeDomainObjects;
  }

  public void SetRelatedObject (RelationEndPointID endPointID, DomainObject newRelatedObject)
  {
    ArgumentUtility.CheckNotNull ("endPointID", endPointID);
    CheckCardinality (endPointID, CardinalityType.One, "SetRelatedObject", "endPointID");
    CheckDeleted (newRelatedObject);
    CheckClientTransactionForObjectEndPoint (endPointID, newRelatedObject);
    CheckType (endPointID, newRelatedObject);

    RelationEndPoint endPoint = GetRelationEndPointWithLazyLoad (endPointID);
    CheckDeleted (endPoint);
    
    if (endPoint.OppositeEndPointDefinition.IsNull)
      SetRelatedObjectForUnidirectionalRelation (endPoint, newRelatedObject);
    else
      SetRelatedObjectForBidirectionalRelation (endPoint, newRelatedObject);
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

    foreach (RelationDefinition relationDefinition in classDefinition.GetRelationDefinitions ())
    {
      foreach (IRelationEndPointDefinition endPointDefinition in relationDefinition.EndPointDefinitions)
      {
        if (!endPointDefinition.IsVirtual)
        {
          if (classDefinition.IsRelationEndPoint (endPointDefinition))
          {
            ObjectID oppositeObjectID = (ObjectID) dataContainer.GetFieldValue (endPointDefinition.PropertyName, ValueAccess.Current);
            ObjectEndPoint endPoint = new ObjectEndPoint (dataContainer, endPointDefinition, oppositeObjectID);
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
        DomainObjectCollection domainObjects = DomainObjectCollection.Create (
            endPointID.Definition.PropertyType, endPointID.OppositeEndPointDefinition.ClassDefinition.ClassType);

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
      
      if (endPoint.OppositeEndPointDefinition.Cardinality == CardinalityType.Many && !endPoint.OppositeEndPointDefinition.IsNull)
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

      allRelationEndPoints.Combine (_relationEndPoints.GetOppositeRelationEndPoints (endPoint));
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

  private void SetRelatedObjectForUnidirectionalRelation (RelationEndPoint endPoint, DomainObject newRelatedObject)
  {
    AnonymousEndPoint newRelatedEndPoint = GetAnonymousEndPoint (newRelatedObject, endPoint.RelationDefinition);
    AnonymousEndPoint oldRelatedEndPoint = GetAnonymousEndPoint (GetRelatedObject (endPoint.ID), endPoint.RelationDefinition);
 
    if (object.ReferenceEquals (newRelatedEndPoint.GetDomainObject (), oldRelatedEndPoint.GetDomainObject ()))
      return;

    endPoint.BeginRelationChange (oldRelatedEndPoint, newRelatedEndPoint);
    endPoint.PerformRelationChange ();
    endPoint.EndRelationChange ();
  }

  private void SetRelatedObjectForBidirectionalRelation (RelationEndPoint endPoint, DomainObject newRelatedObject)
  {
    RelationEndPoint newRelatedEndPoint = GetRelationEndPoint (newRelatedObject, endPoint.OppositeEndPointDefinition);
    RelationEndPoint oldRelatedEndPoint = GetRelationEndPoint (GetRelatedObject (endPoint.ID), newRelatedEndPoint.Definition);

    if (object.ReferenceEquals (newRelatedEndPoint.GetDomainObject (), oldRelatedEndPoint.GetDomainObject ()))
      return;

    if (newRelatedEndPoint.Definition.Cardinality == CardinalityType.One)
      SetRelatedObjectForOneToOneRelation ((ObjectEndPoint) endPoint, (ObjectEndPoint) newRelatedEndPoint, (ObjectEndPoint) oldRelatedEndPoint);
    else
      SetRelatedObjectForOneToManyRelation ((ObjectEndPoint) endPoint, newRelatedEndPoint, oldRelatedEndPoint);
  }

  private void CheckDeleted (RelationEndPoint endPoint)
  {
    CheckDeleted (endPoint.GetDomainObject ());
  }

  private void CheckDeleted (DomainObject domainObject)
  {
    if (domainObject != null && domainObject.State == StateType.Deleted)
      throw new ObjectDeletedException (domainObject.ID);
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

    endPoint.BeginRelationChange (oldRelatedEndPoint, newRelatedEndPoint);
    oldRelatedEndPoint.BeginRelationChange (endPoint, RelationEndPoint.CreateNullRelationEndPoint (endPoint.Definition));
    newRelatedEndPoint.BeginRelationChange (oldRelatedEndPointOfNewRelatedEndPoint, endPoint);
    oldRelatedEndPointOfNewRelatedEndPoint.BeginRelationChange (newRelatedEndPoint, RelationEndPoint.CreateNullRelationEndPoint (newRelatedEndPoint.Definition));

    endPoint.PerformRelationChange ();
    oldRelatedEndPoint.PerformRelationChange ();
    newRelatedEndPoint.PerformRelationChange ();
    oldRelatedEndPointOfNewRelatedEndPoint.PerformRelationChange ();

    endPoint.EndRelationChange ();
    oldRelatedEndPoint.EndRelationChange ();
    newRelatedEndPoint.EndRelationChange ();
    oldRelatedEndPointOfNewRelatedEndPoint.EndRelationChange ();
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

  private AnonymousEndPoint GetAnonymousEndPoint (DomainObject domainObject, RelationDefinition relationDefinition)
  {
    if (domainObject != null)
      return new AnonymousEndPoint (domainObject, relationDefinition);
    else
      return new NullAnonymousEndPoint (relationDefinition);
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

  private void Add (RelationEndPoint endPoint)
  {
    ArgumentUtility.CheckNotNull ("endPoint", endPoint);
    if (endPoint.IsNull)
      throw new ArgumentNullException ("endPoint", "A NullRelationEndPoint cannot be added to a RelationEndPointMap.");

    _relationEndPoints.Add (endPoint);
  }

  private void CheckCardinality (
      RelationEndPointID endPointID, 
      CardinalityType expectedCardinality,
      string methodName,
      string argumentName)
  {
    if (endPointID.Definition.Cardinality != expectedCardinality)
    {
      throw CreateArgumentException (
          argumentName, 
          "{0} can only be called for end points with a cardinality of '{1}'.", 
          methodName,
          expectedCardinality);
    }
  }

  private void CheckClientTransactionForInsertionIntoCollectionEndPoint (RelationEndPointID endPointID, DomainObject newRelatedObject, int index)
  {
    if (newRelatedObject != null && newRelatedObject.DataContainer.ClientTransaction != _clientTransaction)
    {
      throw CreateClientTransactionsDifferException (
          "Cannot insert DomainObject '{0}' at position {1} into collection of property '{2}' of DomainObject '{3}',"
              + " because the objects do not belong to the same ClientTransaction.",
          newRelatedObject.ID, index, endPointID.PropertyName, endPointID.ObjectID);
    }
  }

  private void CheckClientTransactionForRemovalFromCollectionEndPoint (RelationEndPointID endPointID, DomainObject relatedObject)
  {
    if (relatedObject != null && relatedObject.DataContainer.ClientTransaction != _clientTransaction)
    {
      throw CreateClientTransactionsDifferException (
          "Cannot remove DomainObject '{0}' from collection of property '{1}' of DomainObject '{2}',"
              + " because the objects do not belong to the same ClientTransaction.",
          relatedObject.ID, endPointID.PropertyName, endPointID.ObjectID);
    }
  }

  private void CheckClientTransactionForObjectEndPoint (RelationEndPointID endPointID, DomainObject newRelatedObject)
  {
    if (newRelatedObject != null && newRelatedObject.DataContainer.ClientTransaction != _clientTransaction)
    {
      throw CreateClientTransactionsDifferException (
          "Property '{0}' of DomainObject '{1}' cannot be set to DomainObject '{2}',"
              + " because the objects do not belong to the same ClientTransaction.",
          endPointID.PropertyName, endPointID.ObjectID, newRelatedObject.ID);
    }
  }

  private void CheckClientTransactionForReplacementInCollectionEndPoint (RelationEndPointID endPointID, DomainObject newRelatedObject, int index)
  {
    if (newRelatedObject != null && newRelatedObject.DataContainer.ClientTransaction != _clientTransaction)
    {
      throw CreateClientTransactionsDifferException (
            "Cannot replace DomainObject at position {0} with DomainObject '{1}'"
          + " in collection of property '{2}' of DomainObject '{3}',"
          + " because the objects do not belong to the same ClientTransaction.",
          index, newRelatedObject.ID, endPointID.PropertyName, endPointID.ObjectID);
    }
  }

  private void CheckClientTransactionForDeletion (DomainObject domainObject)
  {
    if (domainObject.DataContainer.ClientTransaction != _clientTransaction)
    {
      throw CreateClientTransactionsDifferException (
          "Cannot remove DomainObject '{0}' from RelationEndPointMap, because it belongs to a different ClientTransaction.",
          domainObject.ID);
    }
  }

  private void CheckType (RelationEndPointID endPointID, DomainObject newRelatedObject)
  {
    if (newRelatedObject == null)
      return;

    if (!endPointID.OppositeEndPointDefinition.ClassDefinition.IsSameOrBaseClassOf (newRelatedObject.ID.ClassDefinition))
    {
      throw CreateDataManagementException (
          "DomainObject '{0}' cannot be assigned to property '{1}' of DomainObject '{2}',"
          + " because it is not compatible with the type of the property.",
          newRelatedObject.ID, endPointID.PropertyName, endPointID.ObjectID);
    }
  }

  private DataManagementException CreateDataManagementException (string message, params object[] args)
  {
    return new DataManagementException (string.Format (message, args));
  }

  private ArgumentException CreateArgumentException (string argumentName, string message, params object[] args)
  {
    return new ArgumentException (string.Format (message, args), argumentName);
  }

  private ClientTransactionsDifferException CreateClientTransactionsDifferException (string message, params object[] args)
  {
    return new ClientTransactionsDifferException (string.Format (message, args));
  }

  #region ICollectionEndPointChangeDelegate Members

  void ICollectionEndPointChangeDelegate.PerformInsert  (
      CollectionEndPoint endPoint, 
      DomainObject domainObject,
      int index)
  {
    CheckDeleted (endPoint);
    CheckDeleted (domainObject);
    CheckClientTransactionForInsertionIntoCollectionEndPoint (endPoint.ID, domainObject, index);

    ObjectEndPoint addingEndPoint = (ObjectEndPoint) GetRelationEndPoint (
        domainObject, endPoint.OppositeEndPointDefinition);

    RelationEndPoint oldRelatedEndPoint = (CollectionEndPoint) GetRelationEndPoint (
        GetRelatedObject (addingEndPoint.ID), endPoint.Definition);

    addingEndPoint.BeginRelationChange (oldRelatedEndPoint, endPoint);
    oldRelatedEndPoint.BeginRelationChange (GetRelationEndPoint (domainObject, endPoint.OppositeEndPointDefinition));
    endPoint.BeginInsert (RelationEndPoint.CreateNullRelationEndPoint (addingEndPoint.Definition), addingEndPoint, index);

    addingEndPoint.PerformRelationChange ();
    oldRelatedEndPoint.PerformRelationChange ();
    endPoint.PerformRelationChange ();

    addingEndPoint.EndRelationChange ();
    oldRelatedEndPoint.EndRelationChange ();
    endPoint.EndRelationChange ();
  }

  void ICollectionEndPointChangeDelegate.PerformReplace  (
      CollectionEndPoint endPoint, 
      DomainObject domainObject,
      int index)
  {
    CheckDeleted (endPoint);
    CheckDeleted (domainObject);
    CheckClientTransactionForReplacementInCollectionEndPoint (endPoint.ID, domainObject, index);

    ObjectEndPoint newEndPoint = (ObjectEndPoint) GetRelationEndPoint (
        domainObject, endPoint.OppositeEndPointDefinition);

    ObjectEndPoint oldEndPoint = (ObjectEndPoint) GetRelationEndPoint (
        endPoint.OppositeDomainObjects[index], endPoint.OppositeEndPointDefinition);
    
    CollectionEndPoint oldEndPointOfNewEndPoint = (CollectionEndPoint) GetRelationEndPoint (
        GetRelatedObject (newEndPoint.ID), newEndPoint.OppositeEndPointDefinition);
 
    oldEndPoint.BeginRelationChange (endPoint);
    newEndPoint.BeginRelationChange (oldEndPointOfNewEndPoint, endPoint);
    endPoint.BeginReplace (oldEndPoint, newEndPoint);
    oldEndPointOfNewEndPoint.BeginRelationChange (newEndPoint);

    oldEndPoint.PerformRelationChange ();
    newEndPoint.PerformRelationChange ();
    endPoint.PerformRelationChange ();
    oldEndPointOfNewEndPoint.PerformRelationChange ();

    oldEndPoint.EndRelationChange ();
    newEndPoint.EndRelationChange ();
    endPoint.EndRelationChange ();
    oldEndPointOfNewEndPoint.EndRelationChange ();
  }

  void ICollectionEndPointChangeDelegate.PerformRemove (CollectionEndPoint endPoint, DomainObject domainObject)
  {
    CheckDeleted (endPoint);
    CheckDeleted (domainObject);
    CheckClientTransactionForRemovalFromCollectionEndPoint (endPoint.ID, domainObject);

    ObjectEndPoint removingEndPoint = (ObjectEndPoint) GetRelationEndPoint (domainObject, endPoint.OppositeEndPointDefinition);

    removingEndPoint.BeginRelationChange (endPoint);
    endPoint.BeginRelationChange (GetRelationEndPoint (domainObject, endPoint.OppositeEndPointDefinition));

    removingEndPoint.PerformRelationChange ();
    endPoint.PerformRelationChange ();

    removingEndPoint.EndRelationChange ();
    endPoint.EndRelationChange ();
  }

  #endregion
}
}
