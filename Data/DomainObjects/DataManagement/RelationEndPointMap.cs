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

  public RelationEndPointMap ()
  {
  }

  public RelationEndPointMap (RelationEndPointCollection collection, bool isCollectionReadOnly) 
      : base (collection, isCollectionReadOnly)
  {
  }

  // methods and properties

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
            ObjectEndPoint endPoint = new ObjectEndPoint (dataContainer, endPointDefinition, oppositeObjectID);
            Add (endPoint);

            if (endPoint.OppositeEndPointDefinition.Cardinality == CardinalityType.One && endPoint.OppositeObjectID != null)
            {
              ObjectEndPoint oppositeEndPoint = new ObjectEndPoint (
                  endPoint.OppositeObjectID, endPoint.OppositeEndPointDefinition, endPoint.ObjectID);

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

    RelationEndPointCollection oppositeEndPoints = new RelationEndPointCollection ();

    foreach (RelationEndPointID endPointID in domainObject.DataContainer.RelationEndPointIDs)
    {
      if (endPointID.Definition.Cardinality == CardinalityType.One)
      {
        DomainObject oppositeDomainObject = ClientTransaction.Current.GetRelatedObject (endPointID);
        if (oppositeDomainObject != null)
        {
          if (endPointID.OppositeEndPointDefinition.Cardinality == CardinalityType.One)
          {
            ObjectEndPoint oppositeEndPoint = new ObjectEndPoint (
                oppositeDomainObject, endPointID.OppositeEndPointDefinition, domainObject.ID);

            oppositeEndPoints.Add (oppositeEndPoint);    
          }
          else
          {
            RelationEndPointID oppositeEndPointID = new RelationEndPointID (
                oppositeDomainObject.ID, endPointID.OppositeEndPointDefinition); 

            DomainObjectCollection domainObjects = ClientTransaction.Current.GetRelatedObjects (oppositeEndPointID);
            
            CollectionEndPoint oppositeCollectionEndPoint = new CollectionEndPoint (
                oppositeDomainObject, 
                (VirtualRelationEndPointDefinition) oppositeEndPointID.Definition, 
                domainObjects);

            oppositeEndPoints.Add (oppositeCollectionEndPoint);
          }
        }
      }
      else
      {
        foreach (DomainObject oppositeDomainObject in ClientTransaction.Current.GetRelatedObjects (endPointID))
        {
          ObjectEndPoint oppositeEndPoint = new ObjectEndPoint (
              oppositeDomainObject, endPointID.OppositeEndPointDefinition, domainObject.ID);

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
      ChangeLink (relationEndPoint.ID, newRelatedEndPoint.DomainObject);
      ChangeLink (newRelatedEndPoint.ID, relationEndPoint.DomainObject);
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

  #region ICollectionEndPointChangeDelegate Members

  void ICollectionEndPointChangeDelegate.PerformAdd  (CollectionEndPoint endPoint, DomainObject domainObject)
  {
    ObjectEndPoint addingEndPoint = (ObjectEndPoint) ClientTransaction.Current.GetRelationEndPoint (
        domainObject, endPoint.OppositeEndPointDefinition);

    RelationEndPoint oldRelatedEndPoint = (CollectionEndPoint) ClientTransaction.Current.GetRelationEndPoint (
        ClientTransaction.Current.GetRelatedObject (addingEndPoint.ID), endPoint.Definition);

    DomainObjectCollection oldCollection = null;
    if (!oldRelatedEndPoint.IsNull)
      oldCollection = ClientTransaction.Current.GetRelatedObjects (oldRelatedEndPoint.ID);
    
    if (addingEndPoint.BeginRelationChange (oldRelatedEndPoint, endPoint)
        && oldRelatedEndPoint.BeginRelationChange (ClientTransaction.Current.GetRelationEndPoint (domainObject, endPoint.OppositeEndPointDefinition))
        && endPoint.BeginRelationChange (RelationEndPoint.CreateNullRelationEndPoint (addingEndPoint.Definition), addingEndPoint))
    {
      addingEndPoint.SetOppositeEndPoint (endPoint);
      ChangeLink (addingEndPoint.ID, endPoint.DomainObject);
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
    ObjectEndPoint removingEndPoint = (ObjectEndPoint) ClientTransaction.Current.GetRelationEndPoint (domainObject, endPoint.OppositeEndPointDefinition);

    if (removingEndPoint.BeginRelationChange (endPoint)
        && endPoint.BeginRelationChange (ClientTransaction.Current.GetRelationEndPoint (domainObject, endPoint.OppositeEndPointDefinition)))
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
