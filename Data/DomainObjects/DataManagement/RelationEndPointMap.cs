using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public class RelationEndPointMap : RelationEndPointCollection
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

}
}
