using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Persistence;

namespace Rubicon.Data.DomainObjects
{
public class ClientTransaction : ICollectionEndPointChangeDelegate, IDisposable
{
  // types

  // static members and constants

  [ThreadStatic]
  private static ClientTransaction s_clientTransaction;

  public static ClientTransaction Current
  {
    get 
    {
      if (s_clientTransaction == null)
        s_clientTransaction = new ClientTransaction ();
      
      return s_clientTransaction;
    }
  }

  public static void SetCurrent (ClientTransaction clientTransaction)
  {
    if (s_clientTransaction != null)
      s_clientTransaction.Dispose ();

    s_clientTransaction = clientTransaction;
  }

  // member fields

  public event LoadedEventHandler Loaded;
  public event CommittedEventHandler Committed;
  
  private DataManager _dataManager;
  private PersistenceManager _persistenceManager;

  // construction and disposing

  protected ClientTransaction ()
  {
    Initialize ();
  }

  #region IDisposable Members

  public void Dispose ()
  {
    _persistenceManager.Dispose ();
  }

  #endregion

  // methods and properties

  public void Commit ()
  {
    DomainObjectCollection changedDomainObjects = _dataManager.GetChangedDomainObjects ();
    DataContainerCollection changedDataContainers = new DataContainerCollection ();

    foreach (DomainObject domainObject in changedDomainObjects)
    {
      CheckMandatoryRelations (domainObject);

      if (domainObject.DataContainer.State != StateType.Original)
        changedDataContainers.Add (domainObject.DataContainer);
    }

    _persistenceManager.Save (changedDataContainers);
    _dataManager.Commit ();
    OnCommitted (new CommittedEventArgs (new DomainObjectCollection (changedDomainObjects, true)));
  }

  public void Rollback ()
  {
    _dataManager.Rollback ();
  }

  internal DataContainer CreateNewDataContainer (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);

    ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[type];
    DataContainer newDataContainer = _persistenceManager.CreateNewDataContainer (classDefinition); 
    _dataManager.RegisterNewDataContainer (newDataContainer);

    return newDataContainer;
  }

  internal protected void Delete (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);

    // TODO: Move BeginDelete and EndDelete to DomainObject!!!

    RelationEndPointCollection oppositeEndPoints = domainObject.GetOppositeRelationEndPoints ();
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

    // TODO: Move code below to RelationEndPointList
    foreach (RelationEndPoint oppositeEndPoint in oppositeEndPoints)
    {
      IRelationEndPointDefinition endPointDefinition = oppositeEndPoint.OppositeEndPointDefinition;
      RelationEndPoint oldEndPoint = GetRelationEndPoint (domainObject, endPointDefinition);
      RelationEndPoint newEndPoint = RelationEndPoint.CreateNullRelationEndPoint (endPointDefinition);

      if (!oppositeEndPoint.BeginRelationChange (oldEndPoint, newEndPoint))
        return false;
    }
    return true;
  }

  private void EndDelete (DomainObject domainObject, RelationEndPointCollection oppositeEndPoints)
  {
    domainObject.EndDelete ();

    // TODO: Move code below to RelationEndPointList

    foreach (RelationEndPoint oppositeEndPoint in oppositeEndPoints)
      oppositeEndPoint.EndRelationChange ();
  }

  internal protected DomainObject GetObject (ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    DataContainer dataContainer = _dataManager.GetDataContainer (id);
    if (dataContainer != null)
      return dataContainer.DomainObject;

    return LoadObject (id);
  }

  internal protected DomainObject GetRelatedObject (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    ObjectEndPoint objectEndPoint = (ObjectEndPoint) _dataManager.GetRelationEndPoint (relationEndPointID);
    if (objectEndPoint == null)
      return LoadRelatedObject (relationEndPointID);
    
    if (objectEndPoint.OppositeObjectID != null)
      return GetObject (objectEndPoint.OppositeObjectID);

    return null;
  }

  internal protected DomainObject GetOriginalRelatedObject (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    ObjectEndPoint objectEndPoint = (ObjectEndPoint) _dataManager.GetRelationEndPoint (relationEndPointID);
    if (objectEndPoint == null)
      return LoadRelatedObject (relationEndPointID);

    if (objectEndPoint.OriginalOppositeObjectID != null)
      return GetObject (objectEndPoint.OriginalOppositeObjectID);

    return null;
  }

  internal protected DomainObjectCollection GetRelatedObjects (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    DomainObjectCollection domainObjects = _dataManager.GetRelatedObjects (relationEndPointID);
    if (domainObjects != null)
      return domainObjects;

    RelationEndPoint relationEndPoint = new RelationEndPoint (relationEndPointID);
    DataContainerCollection relatedDataContainers = _persistenceManager.LoadRelatedDataContainers (relationEndPoint);

    DataContainerCollection newLoadedDataContainers = _dataManager.GetNotExisting (relatedDataContainers);
    _dataManager.Register (newLoadedDataContainers);

    domainObjects = GetDomainObjects (relationEndPoint, relatedDataContainers);

    _dataManager.Register (new CollectionEndPoint (relationEndPointID, domainObjects));

    foreach (DataContainer loadedDataContainer in newLoadedDataContainers)
      OnLoaded (new LoadedEventArgs (loadedDataContainer.DomainObject));

    return domainObjects;
  }

  internal protected DomainObjectCollection GetOriginalRelatedObjects (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    CollectionEndPoint collectionEndPoint = (CollectionEndPoint) _dataManager.GetRelationEndPoint (relationEndPointID);

    if (collectionEndPoint != null)
      return collectionEndPoint.OriginalOppositeDomainObjects;
    else
      return GetRelatedObjects (relationEndPointID); 
  }  

  internal protected void SetRelatedObject (RelationEndPointID relationEndPointID, DomainObject newRelatedObject)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    RelationEndPoint relationEndPoint = GetRelationEndPoint (relationEndPointID);

    RelationEndPoint newRelatedEndPoint = GetRelationEndPoint (
        newRelatedObject, relationEndPoint.OppositeEndPointDefinition);

    RelationEndPoint oldRelatedEndPoint = GetRelationEndPoint (
        GetRelatedObject (relationEndPointID), newRelatedEndPoint.Definition);

    if (object.ReferenceEquals (newRelatedEndPoint.DomainObject, oldRelatedEndPoint.DomainObject))
      return;

    if (newRelatedEndPoint.Definition.Cardinality == CardinalityType.One)
    {
      SetRelatedObjectForOneToOneRelation (
          (ObjectEndPoint) relationEndPoint, 
          (ObjectEndPoint) newRelatedEndPoint, 
          (ObjectEndPoint) oldRelatedEndPoint);
    }
    else
      SetRelatedObjectForOneToManyRelation (
          (ObjectEndPoint) relationEndPoint, 
          newRelatedEndPoint, 
          oldRelatedEndPoint);
  }

  protected virtual DomainObject LoadObject (ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    DataContainer dataContainer = _persistenceManager.LoadDataContainer (id);
    _dataManager.Register (dataContainer);

    OnLoaded (new LoadedEventArgs (dataContainer.DomainObject));

    return dataContainer.DomainObject;
  }

  protected virtual DomainObject LoadRelatedObject (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    DataContainer relatedDataContainer = _persistenceManager.LoadRelatedDataContainer (
        new RelationEndPoint (relationEndPointID));

    if (relatedDataContainer != null)
    {
      _dataManager.Register (relatedDataContainer);
      OnLoaded (new LoadedEventArgs (relatedDataContainer.DomainObject));
      return relatedDataContainer.DomainObject;
    }
    else
    {
      _dataManager.Register (new ObjectEndPoint (relationEndPointID, null));
      return null;
    }
  }

  protected virtual void OnLoaded (LoadedEventArgs args)
  {
    args.LoadedDomainObject.EndObjectLoading ();

    if (Loaded != null)
      Loaded (this, args);
  }

  protected virtual void OnCommitted (CommittedEventArgs args)
  {
    foreach (DomainObject domainObject in args.CommittedDomainObjects)
      domainObject.EndCommit ();

    if (Committed != null)
      Committed (this, args);
  }

  private void Initialize ()
  {
    _dataManager = new DataManager ();
    _dataManager.CollectionEndPointChangeDelegate = this;
    _persistenceManager = new PersistenceManager ();
  }

  private void SetRelatedObjectForOneToOneRelation (
      ObjectEndPoint relationEndPoint, 
      ObjectEndPoint newRelatedEndPoint,
      ObjectEndPoint oldRelatedEndPoint)
  {
    ObjectEndPoint oldRelatedEndPointOfNewRelatedEndPoint = (ObjectEndPoint)
        RelationEndPoint.CreateNullRelationEndPoint (relationEndPoint.Definition);

    if (!newRelatedEndPoint.IsNull)
    {
      oldRelatedEndPointOfNewRelatedEndPoint = (ObjectEndPoint) GetRelationEndPoint (
          GetRelatedObject (newRelatedEndPoint.ID), relationEndPoint.Definition);
    }

    if (BeginRelationChange (relationEndPoint, newRelatedEndPoint, oldRelatedEndPoint, oldRelatedEndPointOfNewRelatedEndPoint))
    {
      _dataManager.WriteAssociatedPropertiesForRelationChange (
          relationEndPoint, 
          newRelatedEndPoint, 
          oldRelatedEndPoint, 
          oldRelatedEndPointOfNewRelatedEndPoint);

      _dataManager.ChangeLinks (relationEndPoint, newRelatedEndPoint, oldRelatedEndPoint, oldRelatedEndPointOfNewRelatedEndPoint);

      EndRelationChange (
          relationEndPoint, newRelatedEndPoint, oldRelatedEndPoint, oldRelatedEndPointOfNewRelatedEndPoint);
    }
  }

  private void SetRelatedObjectForOneToManyRelation (
      ObjectEndPoint relationEndPoint, 
      RelationEndPoint newRelatedEndPoint,
      RelationEndPoint oldRelatedEndPoint)
  {
    if (!newRelatedEndPoint.IsNull)
    {
      DomainObjectCollection collection = GetRelatedObjects (newRelatedEndPoint.ID);
      collection.Add (relationEndPoint.DomainObject);
    }
    else
    {
      DomainObjectCollection collection = GetRelatedObjects (oldRelatedEndPoint.ID);
      collection.Remove (relationEndPoint.DomainObject);
    }
  }

  private RelationEndPoint GetRelationEndPoint (DomainObject domainObject, IRelationEndPointDefinition definition)
  {
    if (domainObject != null)
      return GetRelationEndPoint (new RelationEndPointID (domainObject.ID, definition));
    else
      return RelationEndPoint.CreateNullRelationEndPoint (definition); 
  }

  private RelationEndPoint GetRelationEndPoint (RelationEndPointID endPointID)
  {
    if (_dataManager.Contains (endPointID))
      return _dataManager.GetRelationEndPoint (endPointID);

    if (endPointID.Definition.Cardinality == CardinalityType.One)
      GetRelatedObject (endPointID);
    else
      GetRelatedObjects (endPointID);

    return _dataManager.GetRelationEndPoint (endPointID);
  }

  private bool BeginRelationChange (
      RelationEndPoint relationEndPoint,
      RelationEndPoint newRelatedEndPoint,
      RelationEndPoint oldRelatedEndPoint,
      RelationEndPoint oldRelatedEndPointOfNewRelatedEndPoint)
  {
    return relationEndPoint.BeginRelationChange (oldRelatedEndPoint, newRelatedEndPoint)
        && oldRelatedEndPoint.BeginRelationChange (relationEndPoint, RelationEndPoint.CreateNullRelationEndPoint (relationEndPoint.Definition))
        && newRelatedEndPoint.BeginRelationChange (oldRelatedEndPointOfNewRelatedEndPoint, relationEndPoint)
        && oldRelatedEndPointOfNewRelatedEndPoint.BeginRelationChange (newRelatedEndPoint, RelationEndPoint.CreateNullRelationEndPoint (newRelatedEndPoint.Definition));
  }

  private void EndRelationChange (
      RelationEndPoint relationEndPoint,
      RelationEndPoint newRelatedEndPoint,
      RelationEndPoint oldRelatedEndPoint,
      RelationEndPoint oldRelatedEndPointOfNewRelatedEndPoint)
  {
    relationEndPoint.EndRelationChange ();
    oldRelatedEndPoint.EndRelationChange ();
    newRelatedEndPoint.EndRelationChange ();
    oldRelatedEndPointOfNewRelatedEndPoint.EndRelationChange ();
  }

  private DomainObjectCollection GetDomainObjects (
      RelationEndPoint relationEndPoint,
      DataContainerCollection relatedDataContainers)
  {
    return DomainObjectCollection.Create (
        relationEndPoint.Definition.PropertyType,
        _dataManager.MergeWithExisting (relatedDataContainers));
  }

  private void CheckMandatoryRelations (DomainObject domainObject)
  {
    foreach (RelationEndPointID endPointID in domainObject.DataContainer.RelationEndPointIDs)
    {
      if (endPointID.Definition.IsMandatory)
      {
        RelationEndPoint endPoint = _dataManager.GetRelationEndPoint (endPointID);
        if (endPoint != null)
          endPoint.CheckMandatory ();
      }
    }
  }

  internal protected DataManager DataManager
  {
    get { return _dataManager; }
  }

  #region ICollectionEndPointChangeDelegate Members

  void ICollectionEndPointChangeDelegate.PerformAdd  (CollectionEndPoint endPoint, DomainObject domainObject)
  {
    ObjectEndPoint addingEndPoint = (ObjectEndPoint) GetRelationEndPoint (
        domainObject, endPoint.OppositeEndPointDefinition);

    RelationEndPoint oldRelatedEndPoint = (CollectionEndPoint) GetRelationEndPoint (
        GetRelatedObject (addingEndPoint.ID), endPoint.Definition);

    DomainObjectCollection oldCollection = null;
    if (!oldRelatedEndPoint.IsNull)
      oldCollection = GetRelatedObjects (oldRelatedEndPoint.ID);
    
    if (addingEndPoint.BeginRelationChange (oldRelatedEndPoint, endPoint)
        && oldRelatedEndPoint.BeginRelationChange (GetRelationEndPoint (domainObject, endPoint.OppositeEndPointDefinition))
        && endPoint.BeginRelationChange (RelationEndPoint.CreateNullRelationEndPoint (addingEndPoint.Definition), addingEndPoint))
    {
      addingEndPoint.SetOppositeEndPoint (endPoint);
      _dataManager.ChangeLink (addingEndPoint.ID, endPoint.DomainObject);
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
    ObjectEndPoint removingEndPoint = (ObjectEndPoint) GetRelationEndPoint (domainObject, endPoint.OppositeEndPointDefinition);

    if (removingEndPoint.BeginRelationChange (endPoint)
        && endPoint.BeginRelationChange (GetRelationEndPoint (domainObject, endPoint.OppositeEndPointDefinition)))
    {
      removingEndPoint.SetOppositeEndPoint (RelationEndPoint.CreateNullRelationEndPoint (endPoint.Definition));
      _dataManager.ChangeLink (removingEndPoint.ID, null);
      endPoint.OppositeDomainObjects.PerformRemove (domainObject);

      removingEndPoint.EndRelationChange ();
      endPoint.EndRelationChange ();
    }
  }

  #endregion
}
}