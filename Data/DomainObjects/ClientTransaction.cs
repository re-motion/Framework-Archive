using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Persistence;

namespace Rubicon.Data.DomainObjects
{
public class ClientTransaction : IDisposable
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
      _dataManager.RelationEndPointMap.CheckMandatoryRelations (domainObject);

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

  internal protected DomainObject GetObject (ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    DataContainer dataContainer = _dataManager.DataContainerMap[id];
    if (dataContainer != null)
      return dataContainer.DomainObject;

    return LoadObject (id);
  }

  internal protected DomainObject GetRelatedObject (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    ObjectEndPoint objectEndPoint = (ObjectEndPoint) _dataManager.RelationEndPointMap[relationEndPointID];
    if (objectEndPoint == null)
      return LoadRelatedObject (relationEndPointID);
    
    if (objectEndPoint.OppositeObjectID != null)
      return GetObject (objectEndPoint.OppositeObjectID);

    return null;
  }

  internal protected DomainObject GetOriginalRelatedObject (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    ObjectEndPoint objectEndPoint = (ObjectEndPoint) _dataManager.RelationEndPointMap[relationEndPointID];
    if (objectEndPoint == null)
      return LoadRelatedObject (relationEndPointID);

    if (objectEndPoint.OriginalOppositeObjectID != null)
      return GetObject (objectEndPoint.OriginalOppositeObjectID);

    return null;
  }

  internal protected DomainObjectCollection GetRelatedObjects (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    DomainObjectCollection domainObjects = _dataManager.RelationEndPointMap.GetRelatedObjects (relationEndPointID);
    if (domainObjects != null)
      return domainObjects;

    DataContainerCollection relatedDataContainers = _persistenceManager.LoadRelatedDataContainers (relationEndPointID);

    DataContainerCollection newLoadedDataContainers = _dataManager.GetNotExisting (relatedDataContainers);
    _dataManager.Register (newLoadedDataContainers);

    domainObjects = GetDomainObjects (relationEndPointID, relatedDataContainers);

    _dataManager.RelationEndPointMap.RegisterCollectionEndPoint (relationEndPointID, domainObjects);

    foreach (DataContainer loadedDataContainer in newLoadedDataContainers)
      OnLoaded (new LoadedEventArgs (loadedDataContainer.DomainObject));

    return domainObjects;
  }

  internal protected DomainObjectCollection GetOriginalRelatedObjects (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    CollectionEndPoint collectionEndPoint = (CollectionEndPoint) _dataManager.RelationEndPointMap[relationEndPointID];

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

    if (object.ReferenceEquals (newRelatedEndPoint.GetDomainObject (), oldRelatedEndPoint.GetDomainObject ()))
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
  
  internal protected void Delete (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    _dataManager.Delete (domainObject);
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

    DomainObject domainObject = GetObject (relationEndPointID.ObjectID);

    DataContainer relatedDataContainer = _persistenceManager.LoadRelatedDataContainer (
        domainObject.DataContainer, relationEndPointID);

    if (relatedDataContainer != null)
    {
      _dataManager.Register (relatedDataContainer);
      OnLoaded (new LoadedEventArgs (relatedDataContainer.DomainObject));
      return relatedDataContainer.DomainObject;
    }
    else
    {
      _dataManager.RelationEndPointMap.RegisterObjectEndPoint (relationEndPointID, null);
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
    _dataManager = new DataManager (this);
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
      _dataManager.RelationEndPointMap.WriteAssociatedPropertiesForRelationChange (
          relationEndPoint, 
          newRelatedEndPoint, 
          oldRelatedEndPoint, 
          oldRelatedEndPointOfNewRelatedEndPoint);

      _dataManager.RelationEndPointMap.ChangeLinks (relationEndPoint, newRelatedEndPoint, oldRelatedEndPoint, oldRelatedEndPointOfNewRelatedEndPoint);

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
      collection.Add (relationEndPoint.GetDomainObject ());
    }
    else
    {
      DomainObjectCollection collection = GetRelatedObjects (oldRelatedEndPoint.ID);
      collection.Remove (relationEndPoint.GetDomainObject ());
    }
  }

  internal RelationEndPoint GetRelationEndPoint (DomainObject domainObject, IRelationEndPointDefinition definition)
  {
    // TODO: Move code to RelationMap
    if (domainObject != null)
      return GetRelationEndPoint (new RelationEndPointID (domainObject.ID, definition));
    else
      return RelationEndPoint.CreateNullRelationEndPoint (definition); 
  }

  internal RelationEndPoint GetRelationEndPoint (RelationEndPointID endPointID)
  {
    // TODO: Move code to RelationMap
    if (_dataManager.RelationEndPointMap.Contains (endPointID))
      return _dataManager.RelationEndPointMap[endPointID];

    if (endPointID.Definition.Cardinality == CardinalityType.One)
      GetRelatedObject (endPointID);
    else
      GetRelatedObjects (endPointID);

    return _dataManager.RelationEndPointMap[endPointID];
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
      RelationEndPointID relationEndPointID,
      DataContainerCollection relatedDataContainers)
  {
    return DomainObjectCollection.Create (
        relationEndPointID.Definition.PropertyType,
        _dataManager.MergeWithExisting (relatedDataContainers));
  }

  internal protected DataManager DataManager
  {
    get { return _dataManager; }
  }
}
}