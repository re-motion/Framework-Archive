using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public class DataManager
{
  // types

  // static members and constants

  // member fields

  private DataContainerCollection _dataContainerMap;
  private RelationEndPointMap _relationEndPointMap;

  // construction and disposing

  public DataManager ()
  {
    _dataContainerMap = new DataContainerCollection ();
    _relationEndPointMap = new RelationEndPointMap ();
  }

  // methods and properties

  public void Delete (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);

    RelationEndPointCollection oppositeEndPoints = _relationEndPointMap.CloneOppositeRelationEndPoints (domainObject);
    if (BeginDelete (domainObject, oppositeEndPoints))
    {
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

      EndDelete (domainObject, oppositeEndPoints);
    }
  }

  private bool BeginDelete (DomainObject domainObject, RelationEndPointCollection oppositeEndPoints)
  {
    if (!domainObject.BeginDelete ())
      return false;

    return oppositeEndPoints.BeginDelete (domainObject);
  }

  private void EndDelete (DomainObject domainObject, RelationEndPointCollection oppositeEndPoints)
  {
    domainObject.EndDelete ();
    oppositeEndPoints.EndDelete ();
  }

  public bool Contains (RelationEndPointID id)
  {
    return _relationEndPointMap.Contains (id);
  }

  public DomainObjectCollection GetChangedDomainObjects ()
  {
    DomainObjectCollection changedDomainObjects = new DomainObjectCollection ();

    foreach (DataContainer dataContainer in _dataContainerMap)
    {
      if (dataContainer.DomainObject.State != StateType.Original)
        changedDomainObjects.Add (dataContainer.DomainObject);
    }

    return changedDomainObjects;
  }

  public bool HasRelationChanged (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    foreach (RelationEndPointID endPointID in dataContainer.RelationEndPointIDs)
    {
      RelationEndPoint endPoint = GetRelationEndPoint (endPointID);
      if (endPoint != null && endPoint.HasChanged)
        return true;
    }

    return false;
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

  public DataContainerCollection MergeWithExisting (DataContainerCollection dataContainers)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

    return dataContainers.Combine (_dataContainerMap);
  }

  public DataContainerCollection GetNotExisting (DataContainerCollection dataContainers)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

    return dataContainers.GetDifference (_dataContainerMap);
  }

  public void Register (DataContainerCollection dataContainers)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

    foreach (DataContainer dataContainer in dataContainers)
      Register (dataContainer); 
  }

  public void RegisterNewDataContainer (DataContainer dataContainer)
  {
    Register (dataContainer);

    foreach (RelationEndPointID endPointID in dataContainer.RelationEndPointIDs)
    {
      if (endPointID.Definition.IsVirtual)
      {
        if (endPointID.Definition.Cardinality == CardinalityType.One) 
        {
          Register (new ObjectEndPoint (dataContainer, endPointID.Definition, null));
        }
        else
        {
          DomainObjectCollection domainObjects = DomainObjectCollection.Create (endPointID.Definition.PropertyType);

          CollectionEndPoint collectionEndPoint = new CollectionEndPoint (
              dataContainer, (VirtualRelationEndPointDefinition) endPointID.Definition, domainObjects);

          Register (collectionEndPoint);
        }
      }
    }
  }

  public void ChangeLinks (
      ObjectEndPoint relationEndPoint,
      ObjectEndPoint newRelatedEndPoint,
      ObjectEndPoint oldRelatedEndPoint,
      ObjectEndPoint oldRelatedEndPointOfNewRelatedEndPoint)
  {
    _relationEndPointMap.ChangeLinks (
        relationEndPoint, newRelatedEndPoint, oldRelatedEndPoint, oldRelatedEndPointOfNewRelatedEndPoint);
  }

  public void Register (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    _dataContainerMap.Add (dataContainer);
    _relationEndPointMap.Register (dataContainer);
  }

  public void Register (ObjectEndPoint objectEndPoint)
  {
    ArgumentUtility.CheckNotNull ("objectEndPoint", objectEndPoint);
    _relationEndPointMap.Add (objectEndPoint);
  }

  public void Register (CollectionEndPoint collectionEndPoint)
  {
    ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);

    collectionEndPoint.ChangeDelegate = _relationEndPointMap;
    _relationEndPointMap.Add (collectionEndPoint);
  }

  public RelationEndPoint GetRelationEndPoint (RelationEndPointID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);
    return _relationEndPointMap[id];
  }

  public DomainObjectCollection GetRelatedObjects (RelationEndPointID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    if (_relationEndPointMap.Contains (id))
    {
      CollectionEndPoint collectionEndPoint = (CollectionEndPoint) _relationEndPointMap[id];
      return collectionEndPoint.OppositeDomainObjects;
    }

    return null;
  }

  public DataContainer GetDataContainer (ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    return _dataContainerMap[id];
  }

  public void Commit ()
  {
    DomainObjectCollection changedObjects = GetChangedDomainObjects ();

    foreach (DomainObject domainObject in changedObjects)
    {
      domainObject.DataContainer.Commit ();

      foreach (RelationEndPointID endPointID in domainObject.DataContainer.RelationEndPointIDs)
      {
        RelationEndPoint endPoint = GetRelationEndPoint (endPointID);
        if (endPoint != null)
          endPoint.Commit ();
      }
    }
  }

  public void Rollback ()
  {
    foreach (DomainObject domainObject in GetChangedDomainObjects ())
    {
      domainObject.DataContainer.Rollback ();

      if (domainObject.State == StateType.New)
        _dataContainerMap.Remove (domainObject.ID);

      foreach (RelationEndPointID endPointID in domainObject.DataContainer.RelationEndPointIDs)
      {
        RelationEndPoint endPoint = GetRelationEndPoint (endPointID);
        if (endPoint != null)
          endPoint.Rollback ();

        if (domainObject.State == StateType.New)
          _relationEndPointMap.Remove (endPointID);
      }
    }
  }
}
}
