using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public class DataManager : ILinkChangeDelegate
{
  // types

  // static members and constants

  // member fields

  private DataContainerCollection _dataContainerMap;
  private SingleObjectRelationLinkCollection _singleObjectRelationLinkMap;
  private MultipleObjectsRelationLinkCollection _multipleObjectsRelationLinkMap;
  private ILinkChangeDelegate _changeDelegate = null;

  // construction and disposing

  public DataManager ()
  {
    _dataContainerMap = new DataContainerCollection ();
    _singleObjectRelationLinkMap = new SingleObjectRelationLinkCollection ();
    _multipleObjectsRelationLinkMap = new MultipleObjectsRelationLinkCollection ();
  }

  // methods and properties

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

    foreach (RelationEndPoint relationEndPoint in dataContainer.RelationEndPoints)
    {
      RelationLink link = GetRelationLink (relationEndPoint);
      if (link != null && link.HasChanged)
        return true;
    }

    return false;
  }

  public void ChangeLinks (
      RelationEndPoint relationEndPoint,
      RelationEndPoint newRelatedEndPoint,
      RelationEndPoint oldRelatedEndPoint,
      RelationEndPoint oldRelatedEndPointOfNewRelatedEndPoint)
  {
    ArgumentUtility.CheckNotNull ("relationEndPoint", relationEndPoint);
    ArgumentUtility.CheckNotNull ("newRelatedEndPoint", newRelatedEndPoint);
    ArgumentUtility.CheckNotNull ("oldRelatedEndPoint", oldRelatedEndPoint);
    ArgumentUtility.CheckNotNull ("oldRelatedEndPointOfNewRelatedEndPoint", oldRelatedEndPointOfNewRelatedEndPoint);

    if (!newRelatedEndPoint.IsNull)
    {
      ChangeLink (relationEndPoint, newRelatedEndPoint.DomainObject);
      ChangeLink (newRelatedEndPoint, relationEndPoint.DomainObject);
    }
    else
    {
      ChangeLink (relationEndPoint, null);
    }

    if (!oldRelatedEndPoint.IsNull)
      ChangeLink (oldRelatedEndPoint, null);

    if (!oldRelatedEndPointOfNewRelatedEndPoint.IsNull)
      ChangeLink (oldRelatedEndPointOfNewRelatedEndPoint, null);
  }

  public void ChangeLink (RelationEndPoint relationEndPoint, DomainObject newRelatedObject)
  {
    ArgumentUtility.CheckNotNull ("relationEndPoint", relationEndPoint);

    SingleObjectRelationLink backLink = _singleObjectRelationLinkMap[relationEndPoint.LinkID];

    if (newRelatedObject != null)
      backLink.DestinationObjectID = newRelatedObject.ID;
    else
      backLink.DestinationObjectID = null;
  }

  public void WriteAssociatedPropertiesForRelationChange (
      RelationEndPoint relationEndPoint,
      RelationEndPoint newRelatedEndPoint,
      RelationEndPoint oldRelatedEndPoint,
      RelationEndPoint oldRelatedEndPointOfNewRelatedEndPoint)
  {
    relationEndPoint.SetOppositeEndPoint (newRelatedEndPoint);
    newRelatedEndPoint.SetOppositeEndPoint (relationEndPoint);
    oldRelatedEndPoint.SetOppositeEndPoint (new NullRelationEndPoint (relationEndPoint.Definition));

    oldRelatedEndPointOfNewRelatedEndPoint.SetOppositeEndPoint (
        new NullRelationEndPoint (newRelatedEndPoint.Definition));
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

    foreach (RelationEndPoint relationEndPoint in dataContainer.RelationEndPoints)
    {
      if (relationEndPoint.IsVirtual)
      {
        if (relationEndPoint.Definition.Cardinality == CardinalityType.One) 
        {
          RegisterInSingleObjectRelationLinkMap (relationEndPoint, null);
        }
        else
        {
          DomainObjectCollection domainObjects = DomainObjectCollection.Create (
            relationEndPoint.Definition.PropertyType);

          RegisterInMultipleObjectsRelationLinkMap (relationEndPoint, domainObjects);
        }
      }
    }
  }

  public void Register (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    _dataContainerMap.Add (dataContainer);

    SingleObjectRelationLinkCollection links = SingleObjectRelationLinkCollection.Create (dataContainer);
    foreach (SingleObjectRelationLink link in links)
    {
      _singleObjectRelationLinkMap.Add (link);
    }
  }

  public void RegisterInSingleObjectRelationLinkMap (RelationEndPoint relationEndPoint, ObjectID destinationObjectID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPoint", relationEndPoint);

    SingleObjectRelationLink link = new SingleObjectRelationLink (
        relationEndPoint.LinkID, destinationObjectID);

    _singleObjectRelationLinkMap.Add (link);
  }

  public void RegisterInMultipleObjectsRelationLinkMap (
      RelationEndPoint relationEndPoint, 
      DomainObjectCollection domainObjects)
  {
    ArgumentUtility.CheckNotNull ("relationEndPoint", relationEndPoint);
    ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

    MultipleObjectsRelationLink link = new MultipleObjectsRelationLink (
        relationEndPoint.LinkID, domainObjects);

    link.ChangeDelegate = this;
    _multipleObjectsRelationLinkMap.Add (link);
  }

  public SingleObjectRelationLink GetSingleObjectRelationLink (RelationEndPoint relationEndPoint)
  {
    ArgumentUtility.CheckNotNull ("relationEndPoint", relationEndPoint);

    return _singleObjectRelationLinkMap[relationEndPoint.LinkID];
  }

  public MultipleObjectsRelationLink GetMultipleObjectsRelationLink (RelationEndPoint relationEndPoint)
  {
    ArgumentUtility.CheckNotNull ("relationEndPoint", relationEndPoint);

    return _multipleObjectsRelationLinkMap[relationEndPoint.LinkID];
  }

  public RelationLink GetRelationLink (RelationEndPoint relationEndPoint)
  {
    ArgumentUtility.CheckNotNull ("relationEndPoint", relationEndPoint);

    if (relationEndPoint.Definition.Cardinality == CardinalityType.One)
      return GetSingleObjectRelationLink (relationEndPoint);
    else
      return GetMultipleObjectsRelationLink (relationEndPoint);
  }

  public DomainObjectCollection GetRelatedObjects (RelationEndPoint relationEndPoint)
  {
    ArgumentUtility.CheckNotNull ("relationEndPoint", relationEndPoint);

    if (_multipleObjectsRelationLinkMap.Contains (relationEndPoint.LinkID))
      return _multipleObjectsRelationLinkMap[relationEndPoint.LinkID].DestinationDomainObjects;

    return null;
  }

  public DataContainer GetObject (ObjectID id)
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

      foreach (RelationEndPoint relationEndPoint in domainObject.DataContainer.RelationEndPoints)
      {
        RelationLink link = GetRelationLink (relationEndPoint);
        if (link != null)
          link.Commit ();
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

      foreach (RelationEndPoint relationEndPoint in domainObject.DataContainer.RelationEndPoints)
      {
        RelationLink link = GetRelationLink (relationEndPoint);
        if (link != null)
          link.Rollback ();

        if (domainObject.State == StateType.New)
        {
          if (link as SingleObjectRelationLink != null)
            _singleObjectRelationLinkMap.Remove (link.ID);
          else
            _multipleObjectsRelationLinkMap.Remove (link.ID);
        }
      }
    }
  }

  internal ILinkChangeDelegate LinkChangeDelegate
  {
    set { _changeDelegate = value; }
  }

  #region ILinkChangeDelegate Members

  void ILinkChangeDelegate.PerformAdd (MultipleObjectsRelationLink link, DomainObject domainObject)
  {
    if (_changeDelegate == null)
      throw new DataManagementException ("Internal error: DataManager must have an ILinkChangeDelegate registered.");

    _changeDelegate.PerformAdd (link, domainObject);
  }

  void ILinkChangeDelegate.PerformRemove (MultipleObjectsRelationLink link, DomainObject domainObject)
  {
    if (_changeDelegate == null)
      throw new DataManagementException ("Internal error: DataManager must have an ILinkChangeDelegate registered.");

    _changeDelegate.PerformRemove (link, domainObject);
  }

  #endregion
}
}
