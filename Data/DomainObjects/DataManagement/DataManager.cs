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

  public DataManager (ClientTransaction clientTransaction)
  {
    ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

    _dataContainerMap = new DataContainerCollection ();
    _relationEndPointMap = new RelationEndPointMap (clientTransaction);
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
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    Register (dataContainer);
    _relationEndPointMap.RegisterNewDataContainer (dataContainer);
  }

  public void Register (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    _dataContainerMap.Add (dataContainer);
    _relationEndPointMap.Register (dataContainer);
  }

  public void Commit ()
  {
    _relationEndPointMap.Commit ();

    foreach (DomainObject domainObject in GetChangedDomainObjects ())
      domainObject.DataContainer.Commit ();
  }

  public void Rollback ()
  {
    _relationEndPointMap.Rollback (GetNewDomainObjects ());

    foreach (DomainObject domainObject in GetChangedDomainObjects ())
    {
      domainObject.DataContainer.Rollback ();

      if (domainObject.State == StateType.New)
        _dataContainerMap.Remove (domainObject.ID);
    }
  }

  public DataContainerCollection DataContainerMap
  {
    get { return _dataContainerMap; }
  }
  public RelationEndPointMap RelationEndPointMap
  {
    get { return _relationEndPointMap; }
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

  private DomainObjectCollection GetNewDomainObjects ()
  {
    DomainObjectCollection newDomainObjects = new DomainObjectCollection ();
    foreach (DataContainer dataContainer in _dataContainerMap)
    {
      if (dataContainer.State == StateType.New)
        newDomainObjects.Add (dataContainer.DomainObject);
    }

    return newDomainObjects;
  }
}
}
