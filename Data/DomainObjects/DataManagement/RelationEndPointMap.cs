using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public class RelationEndPointMap : RelationEndPointCollection, ICollectionEndPointChangeDelegate
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public RelationEndPointMap (ClientTransaction clientTransaction) : base (clientTransaction)
  {
  }

  public RelationEndPointMap (
      ClientTransaction clientTransaction,
      RelationEndPointCollection collection, 
      bool isCollectionReadOnly) 
      : base (clientTransaction, collection, isCollectionReadOnly)
  {
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
  
  public void RegisterObjectEndPoint (RelationEndPointID endPointID, ObjectID oppositeObjectID)
  {
    ArgumentUtility.CheckNotNull ("endPointID", endPointID);
    Add (new ObjectEndPoint (ClientTransaction, endPointID, oppositeObjectID));
  }

  public void RegisterCollectionEndPoint (RelationEndPointID endPointID, DomainObjectCollection domainObjects)
  {
    ArgumentUtility.CheckNotNull ("endPointID", endPointID);
    ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

    CollectionEndPoint collectionEndPoint = new CollectionEndPoint (ClientTransaction, endPointID, domainObjects);
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
                ClientTransaction, dataContainer, endPointDefinition, oppositeObjectID);
            
            Add (endPoint);

            if (endPoint.OppositeEndPointDefinition.Cardinality == CardinalityType.One && endPoint.OppositeObjectID != null)
            {
              ObjectEndPoint oppositeEndPoint = new ObjectEndPoint (
                  ClientTransaction, 
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

  public RelationEndPointCollection CloneOppositeRelationEndPoints (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);

    RelationEndPointCollection oppositeEndPoints = new RelationEndPointCollection (ClientTransaction);

    foreach (RelationEndPointID endPointID in domainObject.DataContainer.RelationEndPointIDs)
    {
      if (endPointID.Definition.Cardinality == CardinalityType.One)
      {
        DomainObject oppositeDomainObject = ClientTransaction.GetRelatedObject (endPointID);
        if (oppositeDomainObject != null)
        {
          if (endPointID.OppositeEndPointDefinition.Cardinality == CardinalityType.One)
          {
            ObjectEndPoint oppositeEndPoint = new ObjectEndPoint (
                ClientTransaction, oppositeDomainObject, endPointID.OppositeEndPointDefinition, domainObject.ID);

            oppositeEndPoints.Add (oppositeEndPoint);    
          }
          else
          {
            RelationEndPointID oppositeEndPointID = new RelationEndPointID (
                oppositeDomainObject.ID, endPointID.OppositeEndPointDefinition); 

            DomainObjectCollection domainObjects = ClientTransaction.GetRelatedObjects (oppositeEndPointID);
            
            CollectionEndPoint oppositeCollectionEndPoint = new CollectionEndPoint (
                ClientTransaction, 
                oppositeDomainObject, 
                (VirtualRelationEndPointDefinition) oppositeEndPointID.Definition, 
                domainObjects);

            oppositeEndPoints.Add (oppositeCollectionEndPoint);
          }
        }
      }
      else
      {
        foreach (DomainObject oppositeDomainObject in ClientTransaction.GetRelatedObjects (endPointID))
        {
          ObjectEndPoint oppositeEndPoint = new ObjectEndPoint (
              ClientTransaction, oppositeDomainObject, endPointID.OppositeEndPointDefinition, domainObject.ID);

          oppositeEndPoints.Add (oppositeEndPoint);
        }
      }
    }

    return oppositeEndPoints;
  }

  public void ChangeLinks (
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

  public DomainObjectCollection GetRelatedObjects (RelationEndPointID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    if (Contains (id))
    {
      CollectionEndPoint collectionEndPoint = (CollectionEndPoint) this[id];
      return collectionEndPoint.OppositeDomainObjects;
    }

    return null;
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

  public void WriteAssociatedPropertiesForRelationChange (
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
    ObjectEndPoint addingEndPoint = (ObjectEndPoint) ClientTransaction.GetRelationEndPoint (
        domainObject, endPoint.OppositeEndPointDefinition);

    RelationEndPoint oldRelatedEndPoint = (CollectionEndPoint) ClientTransaction.GetRelationEndPoint (
        ClientTransaction.GetRelatedObject (addingEndPoint.ID), endPoint.Definition);

    DomainObjectCollection oldCollection = null;
    if (!oldRelatedEndPoint.IsNull)
      oldCollection = ClientTransaction.GetRelatedObjects (oldRelatedEndPoint.ID);
    
    if (addingEndPoint.BeginRelationChange (oldRelatedEndPoint, endPoint)
        && oldRelatedEndPoint.BeginRelationChange (ClientTransaction.GetRelationEndPoint (domainObject, endPoint.OppositeEndPointDefinition))
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
    ObjectEndPoint removingEndPoint = (ObjectEndPoint) ClientTransaction.GetRelationEndPoint (domainObject, endPoint.OppositeEndPointDefinition);

    if (removingEndPoint.BeginRelationChange (endPoint)
        && endPoint.BeginRelationChange (ClientTransaction.GetRelationEndPoint (domainObject, endPoint.OppositeEndPointDefinition)))
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
