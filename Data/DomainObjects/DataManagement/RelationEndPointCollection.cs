using System;
using System.Collections;

using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public class RelationEndPointCollection : CollectionBase, ICloneable
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public RelationEndPointCollection ()
  {
  }

  public RelationEndPointCollection (RelationEndPointCollection collection, bool isCollectionReadOnly)
  {
    ArgumentUtility.CheckNotNull ("collection", collection);
    
    foreach (RelationEndPoint endPoint in collection)
    {
      Add (endPoint);
    }

    this.SetIsReadOnly (isCollectionReadOnly);
  }

  // methods and properties

  public void Merge (RelationEndPointCollection endPoints)
  {
    foreach (RelationEndPoint endPoint in endPoints)
      Add (endPoint);
  }

  public bool BeginDelete (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);

    foreach (RelationEndPoint oppositeEndPoint in GetOppositeRelationEndPoints (domainObject))
    {
      IRelationEndPointDefinition endPointDefinition = oppositeEndPoint.OppositeEndPointDefinition;
      RelationEndPoint oldEndPoint = this[new RelationEndPointID (domainObject.ID, endPointDefinition)];
      RelationEndPoint newEndPoint = RelationEndPoint.CreateNullRelationEndPoint (endPointDefinition);

      if (!oppositeEndPoint.BeginRelationChange (oldEndPoint, newEndPoint))
        return false;
    }
    return true;
  }

  public void EndDelete ()
  {
    foreach (RelationEndPoint endPoint in this)
      endPoint.EndRelationChange ();    
  }

  public RelationEndPointCollection GetOppositeRelationEndPoints (DomainObject domainObject)
  {
    RelationEndPointCollection oppositeEndPoints = new RelationEndPointCollection ();

    foreach (RelationEndPointID endPointID in domainObject.DataContainer.RelationEndPointIDs)
      oppositeEndPoints.Merge (GetOppositeRelationEndPoints (this[endPointID]));

    return oppositeEndPoints;
  }

  public RelationEndPointCollection GetOppositeRelationEndPoints (RelationEndPoint endPoint)
  {
    ArgumentUtility.CheckNotNull ("endPoint", endPoint);

    RelationEndPointCollection oppositeEndPoints = new RelationEndPointCollection ();
    if (endPoint.Definition.Cardinality == CardinalityType.One)
    {
      ObjectEndPoint objectEndPoint = (ObjectEndPoint) endPoint;
      if (objectEndPoint.OppositeObjectID != null)
      {
        RelationEndPointID oppositeEndPointID = new RelationEndPointID (
          objectEndPoint.OppositeObjectID, objectEndPoint.OppositeEndPointDefinition);

        oppositeEndPoints.Add (this[oppositeEndPointID]);
      }
    }
    else
    {
      CollectionEndPoint collectionEndPoint = (CollectionEndPoint) endPoint;
      foreach (DomainObject oppositeDomainObject in collectionEndPoint.OppositeDomainObjects)
      {
        RelationEndPointID oppositeEndPointID = new RelationEndPointID (
          oppositeDomainObject.ID, collectionEndPoint.OppositeEndPointDefinition);

        oppositeEndPoints.Add (this[oppositeEndPointID]);
      }
    }

    return oppositeEndPoints;
  }

  #region Standard implementation for collections

  public bool Contains (RelationEndPoint endPoint)
  {
    ArgumentUtility.CheckNotNull ("endPoint", endPoint);

    return Contains (endPoint.ID);
  }

  public bool Contains (RelationEndPointID id)
  {
    return this.ContainsKey (id);
  }

  public RelationEndPoint this[int index]
  {
    get {return (RelationEndPoint) GetObject (index); }
  }

  public RelationEndPoint this[RelationEndPointID id]
  {
    get { return (RelationEndPoint) GetObject (id); }
  }

  public void Add (RelationEndPoint endPoint)
  {
    ArgumentUtility.CheckNotNull ("endPoint", endPoint);
    base.Add (endPoint.ID, endPoint);
  }

  public void Remove (int index)
  {
    Remove (this[index]);
  }

  public void Remove (RelationEndPointID id)
  {
    Remove (this[id]);
  }

  public void Remove (RelationEndPoint endPoint)
  {
    ArgumentUtility.CheckNotNull ("endPoint", endPoint);

    base.Remove (endPoint.ID);
  }

  public void Clear ()
  {
    base.ClearCollection ();
  }

  #endregion

  #region ICloneable Members

  public virtual object Clone ()
  {
    RelationEndPointCollection newCollection = new RelationEndPointCollection ();

    foreach (RelationEndPoint endPoint in this)
      newCollection.Add ((RelationEndPoint) endPoint.Clone ());

    return newCollection;
  }

  #endregion
}
}
