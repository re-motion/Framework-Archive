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
    DataContainerCollection changedDataContainers = _dataManager.GetChangedDataContainersForCommit ();
    _persistenceManager.Save (changedDataContainers);

    DomainObjectCollection changedDomainObjects = _dataManager.GetChangedDomainObjects ();
    _dataManager.Commit ();
    OnCommitted (new CommittedEventArgs (new DomainObjectCollection (changedDomainObjects, true)));
  }

  public void Rollback ()
  {
    _dataManager.Rollback ();
  }

  internal protected bool HasRelationChanged (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);

    return _dataManager.RelationEndPointMap.HasRelationChanged (domainObject.DataContainer);
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
    return _dataManager.RelationEndPointMap.GetRelatedObject (relationEndPointID);
  }

  internal protected DomainObject GetOriginalRelatedObject (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    return _dataManager.RelationEndPointMap.GetOriginalRelatedObject (relationEndPointID);
  }

  internal protected DomainObjectCollection GetRelatedObjects (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    return _dataManager.RelationEndPointMap.GetRelatedObjects (relationEndPointID);
  }

  internal protected DomainObjectCollection GetOriginalRelatedObjects (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    return _dataManager.RelationEndPointMap.GetOriginalRelatedObjects (relationEndPointID);
  }  

  internal protected void SetRelatedObject (RelationEndPointID relationEndPointID, DomainObject newRelatedObject)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    _dataManager.RelationEndPointMap.SetRelatedObject (relationEndPointID, newRelatedObject);
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
    _dataManager.RegisterExistingDataContainer (dataContainer);

    OnLoaded (new LoadedEventArgs (dataContainer.DomainObject));

    return dataContainer.DomainObject;
  }

  internal protected virtual DomainObject LoadRelatedObject (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    DomainObject domainObject = GetObject (relationEndPointID.ObjectID);

    DataContainer relatedDataContainer = _persistenceManager.LoadRelatedDataContainer (
        domainObject.DataContainer, relationEndPointID);

    if (relatedDataContainer != null)
    {
      _dataManager.RegisterExistingDataContainer (relatedDataContainer);
      OnLoaded (new LoadedEventArgs (relatedDataContainer.DomainObject));
      return relatedDataContainer.DomainObject;
    }
    else
    {
      _dataManager.RelationEndPointMap.RegisterObjectEndPoint (relationEndPointID, null);
      return null;
    }
  }

  internal protected virtual DomainObjectCollection LoadRelatedObjects (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    DataContainerCollection relatedDataContainers = _persistenceManager.LoadRelatedDataContainers (relationEndPointID);

    DataContainerCollection newLoadedDataContainers = _dataManager.DataContainerMap.GetNotExisting (relatedDataContainers);
    _dataManager.RegisterExistingDataContainers (newLoadedDataContainers);

    DomainObjectCollection domainObjects = GetDomainObjects (relationEndPointID, relatedDataContainers);

    _dataManager.RelationEndPointMap.RegisterCollectionEndPoint (relationEndPointID, domainObjects);

    foreach (DataContainer loadedDataContainer in newLoadedDataContainers)
      OnLoaded (new LoadedEventArgs (loadedDataContainer.DomainObject));

    return domainObjects;
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

  protected DataManager DataManager
  {
    get { return _dataManager; }
  }

  protected PersistenceManager PersistenceManager 
  {
    get { return _persistenceManager; }
  }

  private void Initialize ()
  {
    _dataManager = new DataManager (this);
    _persistenceManager = new PersistenceManager ();
  }

  private DomainObjectCollection GetDomainObjects (
      RelationEndPointID relationEndPointID,
      DataContainerCollection relatedDataContainers)
  {
    return DomainObjectCollection.Create (
        relationEndPointID.Definition.PropertyType,
        _dataManager.DataContainerMap.MergeWithExisting (relatedDataContainers));
  }
}
}