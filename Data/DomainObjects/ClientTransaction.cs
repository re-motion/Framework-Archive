using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Persistence;

namespace Rubicon.Data.DomainObjects
{
public class ClientTransaction
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
    s_clientTransaction = clientTransaction;
  }

  // member fields

  public event LoadedEventHandler Loaded;
  public event CommittedEventHandler Committed;
  
  private DataManager _dataManager;

  // construction and disposing

  protected ClientTransaction ()
  {
    Initialize ();
  }

  // methods and properties

  public void Commit ()
  {
    DataContainerCollection changedDataContainers = _dataManager.GetChangedDataContainersForCommit ();
    using (PersistenceManager persistenceManager = new PersistenceManager ())
    {
      persistenceManager.Save (changedDataContainers);
    }

    DomainObjectCollection changedDomainObjects = _dataManager.GetChangedDomainObjects ();
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
    
    using (PersistenceManager persistenceManager = new PersistenceManager ())
    {
      DataContainer newDataContainer = persistenceManager.CreateNewDataContainer (classDefinition); 
      _dataManager.RegisterNewDataContainer (newDataContainer);

      return newDataContainer;
    }    
  }

  internal protected bool HasRelationChanged (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);

    return _dataManager.RelationEndPointMap.HasRelationChanged (domainObject.DataContainer);
  }

  internal protected DomainObject GetObject (ObjectID id, bool includeDeleted)
  {
    ArgumentUtility.CheckNotNull ("id", id);
    return _dataManager.DataContainerMap.GetObject (id, includeDeleted);
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
  
  internal protected virtual DomainObject LoadObject (ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    using (PersistenceManager persistenceManager = new PersistenceManager ())
    {
      DataContainer dataContainer = persistenceManager.LoadDataContainer (id);

      _dataManager.RegisterExistingDataContainer (dataContainer);

      OnLoaded (new LoadedEventArgs (dataContainer.DomainObject));

      return dataContainer.DomainObject;
    }
  }

  internal protected virtual DomainObject LoadRelatedObject (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    DomainObject domainObject = GetObject (relationEndPointID.ObjectID, false);

    using (PersistenceManager persistenceManager = new PersistenceManager ())
    {
      DataContainer relatedDataContainer = persistenceManager.LoadRelatedDataContainer (
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
  }

  internal protected virtual DomainObjectCollection LoadRelatedObjects (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    using (PersistenceManager persistenceManager = new PersistenceManager ())
    {
      DataContainerCollection relatedDataContainers = persistenceManager.LoadRelatedDataContainers (relationEndPointID);

      DataContainerCollection newLoadedDataContainers = _dataManager.DataContainerMap.GetNotExisting (relatedDataContainers);
      _dataManager.RegisterExistingDataContainers (newLoadedDataContainers);

      DomainObjectCollection domainObjects = DomainObjectCollection.Create (
          relationEndPointID.Definition.PropertyType,
          _dataManager.DataContainerMap.MergeWithExisting (relatedDataContainers));

      _dataManager.RelationEndPointMap.RegisterCollectionEndPoint (relationEndPointID, domainObjects);

      foreach (DataContainer newLoadedDataContainer in newLoadedDataContainers)
        OnLoaded (new LoadedEventArgs (newLoadedDataContainer.DomainObject));

      return domainObjects;
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

  protected DataManager DataManager
  {
    get { return _dataManager; }
  }

  private void Initialize ()
  {
    _dataManager = new DataManager (this);
  }
}
}