using System;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

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
      if (domainObject.State != StateType.Deleted)
        _relationEndPointMap.CheckMandatoryRelations (domainObject);

      if (domainObject.DataContainer.State != StateType.Unchanged)
        changedDataContainers.Add (domainObject.DataContainer);
    }

    return changedDataContainers;
  }

  public DomainObjectCollection GetChangedDomainObjects ()
  {
    DomainObjectCollection changedDomainObjects = new DomainObjectCollection ();

    foreach (DataContainer dataContainer in _dataContainerMap)
    {
      if (dataContainer.DomainObject.State != StateType.Unchanged)
        changedDomainObjects.Add (dataContainer.DomainObject);
    }

    return changedDomainObjects;
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
    _relationEndPointMap.Commit (_dataContainerMap.GetByState (StateType.Deleted));
    _dataContainerMap.Commit ();
  }

  public void Rollback ()
  {
    _relationEndPointMap.Rollback (_dataContainerMap.GetByState (StateType.New));
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

  public void Delete (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);

    if (domainObject.State == StateType.Deleted)
      return;

    RelationEndPointCollection allAffectedRelationEndPoints = 
      _relationEndPointMap.GetAllRelationEndPointsWithLazyLoad (domainObject);

    if (BeginDelete (domainObject, allAffectedRelationEndPoints))
    {
      RelationEndPointCollection allOppositeRelationEndPoints = 
          allAffectedRelationEndPoints.GetOppositeRelationEndPoints (domainObject);

      PerformDelete (domainObject);
      EndDelete (domainObject, allOppositeRelationEndPoints);
    }
  }

  private void PerformDelete (DomainObject domainObject)
  {
    _relationEndPointMap.PerformDelete (domainObject);
    _dataContainerMap.PerformDelete (domainObject.DataContainer);
    domainObject.DataContainer.Delete ();
  }

  private bool BeginDelete (DomainObject domainObject, RelationEndPointCollection allAffectedRelationEndPoints)
  {
    if (!domainObject.BeginDelete ())
      return false;

    return allAffectedRelationEndPoints.BeginDelete (domainObject);
  }

  private void EndDelete (DomainObject domainObject, RelationEndPointCollection allOppositeRelationEndPoints)
  {
    domainObject.EndDelete ();
    allOppositeRelationEndPoints.EndDelete ();
  }
}
}
