using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.Relations;

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

    foreach (ObjectEndPoint objectEndPoint in dataContainer.ObjectEndPoints)
    {
      RelationLink link = GetRelationLink (objectEndPoint);
      if (link != null && link.HasChanged)
        return true;
    }

    return false;
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

  public void ChangeLink (ObjectEndPoint objectEndPoint, DomainObject newRelatedObject)
  {
    ArgumentUtility.CheckNotNull ("objectEndPoint", objectEndPoint);

    SingleObjectRelationLink backLink = _singleObjectRelationLinkMap[objectEndPoint.CreateRelationLinkID ()];

    if (newRelatedObject != null)
      backLink.DestinationObjectID = newRelatedObject.ID;
    else
      backLink.DestinationObjectID = null;
  }

  public void WriteAssociatedPropertiesForRelationChange (
      ObjectEndPoint objectEndPoint,
      ObjectEndPoint newRelatedEndPoint,
      ObjectEndPoint oldRelatedEndPoint,
      ObjectEndPoint oldRelatedEndPointOfNewRelatedEndPoint)
  {
    objectEndPoint.SetOppositeEndPoint (newRelatedEndPoint);
    newRelatedEndPoint.SetOppositeEndPoint (objectEndPoint);
    oldRelatedEndPoint.SetOppositeEndPoint (new NullRelationEndPoint (objectEndPoint.Definition));

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

    foreach (ObjectEndPoint objectEndPoint in dataContainer.ObjectEndPoints)
    {
      if (objectEndPoint.IsVirtual)
      {
        if (objectEndPoint.Definition.Cardinality == CardinalityType.One) 
        {
          RegisterInSingleObjectRelationLinkMap (objectEndPoint, null);
        }
        else
        {
          DomainObjectCollection domainObjects = DomainObjectCollection.Create (
            objectEndPoint.Definition.PropertyType);

          RegisterInMultipleObjectsRelationLinkMap (objectEndPoint, domainObjects);
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

  public void RegisterInSingleObjectRelationLinkMap (ObjectEndPoint objectEndPoint, ObjectID destinationObjectID)
  {
    ArgumentUtility.CheckNotNull ("objectEndPoint", objectEndPoint);

    SingleObjectRelationLink link = new SingleObjectRelationLink (
        objectEndPoint.CreateRelationLinkID (), destinationObjectID);

    _singleObjectRelationLinkMap.Add (link);
  }

  public void RegisterInMultipleObjectsRelationLinkMap (
      ObjectEndPoint objectEndPoint, 
      DomainObjectCollection domainObjects)
  {
    ArgumentUtility.CheckNotNull ("objectEndPoint", objectEndPoint);
    ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

    MultipleObjectsRelationLink link = new MultipleObjectsRelationLink (
        objectEndPoint.CreateRelationLinkID (), domainObjects);

    link.ChangeDelegate = this;
    _multipleObjectsRelationLinkMap.Add (link);
  }

  public SingleObjectRelationLink GetSingleObjectRelationLink (ObjectEndPoint objectEndPoint)
  {
    ArgumentUtility.CheckNotNull ("objectEndPoint", objectEndPoint);

    return _singleObjectRelationLinkMap[objectEndPoint.CreateRelationLinkID ()];
  }

  public MultipleObjectsRelationLink GetMultipleObjectsRelationLink (ObjectEndPoint objectEndPoint)
  {
    ArgumentUtility.CheckNotNull ("objectEndPoint", objectEndPoint);

    return _multipleObjectsRelationLinkMap[objectEndPoint.CreateRelationLinkID ()];
  }

  public RelationLink GetRelationLink (ObjectEndPoint objectEndPoint)
  {
    ArgumentUtility.CheckNotNull ("objectEndPoint", objectEndPoint);

    if (objectEndPoint.Definition.Cardinality == CardinalityType.One)
      return GetSingleObjectRelationLink (objectEndPoint);
    else
      return GetMultipleObjectsRelationLink (objectEndPoint);
  }

  public DomainObjectCollection GetRelatedObjects (ObjectEndPoint objectEndPoint)
  {
    ArgumentUtility.CheckNotNull ("objectEndPoint", objectEndPoint);

    RelationLinkID linkID = objectEndPoint.CreateRelationLinkID ();
    if (_multipleObjectsRelationLinkMap.Contains (linkID))
      return _multipleObjectsRelationLinkMap[linkID].DestinationDomainObjects;

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

      foreach (ObjectEndPoint objectEndPoint in domainObject.DataContainer.ObjectEndPoints)
      {
        RelationLink link = GetRelationLink (objectEndPoint);
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

      foreach (ObjectEndPoint objectEndPoint in domainObject.DataContainer.ObjectEndPoints)
      {
        RelationLink link = GetRelationLink (objectEndPoint);
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
