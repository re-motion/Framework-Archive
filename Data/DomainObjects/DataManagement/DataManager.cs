using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public class DataManager
{
  // types

  // static members and constants

  // member fields

  private DataContainerMap _dataContainerMap;
  private RelationEndPointMap _relationEndPointMap;

  // construction and disposing

  public DataManager (ClientTransaction clientTransaction)
  {
    ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

    _dataContainerMap = new DataContainerMap (clientTransaction);
    _relationEndPointMap = new RelationEndPointMap (clientTransaction);
  }

  // methods and properties

  public DataContainerCollection GetChangedDataContainersForCommit ()
  {
    DataContainerCollection changedDataContainers = new DataContainerCollection ();
    foreach (DomainObject domainObject in GetChangedDomainObjects ())
    {
      _relationEndPointMap.CheckMandatoryRelations (domainObject);

      if (domainObject.DataContainer.State != StateType.Original)
        changedDataContainers.Add (domainObject.DataContainer);
    }

    return changedDataContainers;
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

  public void Delete (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);

    RelationEndPointCollection allAffectedRelationEndPoints = 
        _relationEndPointMap.GetAllRelationEndPointsWithLazyLoad (domainObject);

    if (BeginDelete (domainObject, allAffectedRelationEndPoints))
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

      EndDelete (domainObject, allAffectedRelationEndPoints);
    }
  }

  public void RegisterExistingDataContainers (DataContainerCollection dataContainers)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

    foreach (DataContainer dataContainer in dataContainers)
      RegisterExistingDataContainer (dataContainer); 
  }

  public void RegisterExistingDataContainer (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    _dataContainerMap.Register (dataContainer);
    _relationEndPointMap.RegisterExistingDataContainer (dataContainer);
  }

  public void RegisterNewDataContainer (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    _dataContainerMap.Register (dataContainer);
    _relationEndPointMap.RegisterNewDataContainer (dataContainer);
  }

  public void Commit ()
  {
    _relationEndPointMap.Commit ();
    _dataContainerMap.Commit ();
  }

  public void Rollback ()
  {
    _relationEndPointMap.Rollback (_dataContainerMap.GetNewDomainObjects ());
    _dataContainerMap.Rollback ();
  }

  public DataContainerMap DataContainerMap
  {
    get { return _dataContainerMap; }
  }

  public RelationEndPointMap RelationEndPointMap
  {
    get { return _relationEndPointMap; }
  }

  private bool BeginDelete (DomainObject domainObject, RelationEndPointCollection allAffectedRelationEndPoints)
  {
    if (!domainObject.BeginDelete ())
      return false;

    return allAffectedRelationEndPoints.BeginDelete (domainObject);
  }

  private void EndDelete (DomainObject domainObject, RelationEndPointCollection allAffectedRelationEndPoints)
  {
    domainObject.EndDelete ();
    allAffectedRelationEndPoints.EndDelete (domainObject);
  }
}
}
