using System;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Utilities;


namespace Rubicon.Data.DomainObjects
{
//Documentation: All exceptions checked, except TODOs

//TODO documentation: Write summary for class
public class ClientTransaction
{
  // types

  // static members and constants

  [ThreadStatic]
  private static ClientTransaction s_clientTransaction;

  /// <summary>
  /// Gets the default <b>ClientTransaction</b> of the current thread.
  /// </summary>
  public static ClientTransaction Current
  {
    get 
    {
      if (s_clientTransaction == null)
        s_clientTransaction = new ClientTransaction ();
      
      return s_clientTransaction;
    }
  }

  /// <summary>
  /// Sets the default <b>ClientTransaction</b> of the current thread.
  /// </summary>
  /// <param name="clientTransaction">The <b>ClientTransaction</b> to which the current <b>ClientTransaction</b> is set.</param>
  public static void SetCurrent (ClientTransaction clientTransaction)
  {
    s_clientTransaction = clientTransaction;
  }

  // member fields

  /// <summary>
  /// Occurs after the <b>ClientTransaction</b> has loaded a new object.
  /// </summary>
  public event LoadedEventHandler Loaded;
  
  private DataManager _dataManager;
  private QueryManager _queryManager;

  // construction and disposing

  /// <summary>
  /// Initializes a new instance of the <b>ClientTransaction</b> class.
  /// </summary>
  public ClientTransaction ()
  {
    _dataManager = new DataManager (this);
    _queryManager = new QueryManager (this);
  }

  // methods and properties

  /// <summary>
  /// Commits all changes within the <b>ClientTransaction</b> to the persistent datasources.
  /// </summary>
  //todo documentation: exceptions
  public virtual void Commit ()
  {
    DataContainerCollection changedDataContainers = _dataManager.GetChangedDataContainersForCommit ();
    using (PersistenceManager persistenceManager = new PersistenceManager ())
    {
      persistenceManager.Save (changedDataContainers);
    }

    _dataManager.Commit ();
  }

  /// <summary>
  /// Performs a rollback of all changes withing the <b>ClientTransaction</b>.
  /// </summary>
  //todo documentation: exceptions
  public virtual void Rollback ()
  {
    _dataManager.Rollback ();
  }

  /// <summary>
  /// Gets a <see cref="DomainObject"/> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> that should be loaded.</param>
  /// <returns>The <see cref="DomainObject"/> with the specified <i>id</i>.</returns>
  /// <exception cref="System.ArgumentNullException"><i>id</i> is a null reference.</exception>
//todo documentation: all exceptions from other overload
  public virtual DomainObject GetObject (ObjectID id)
  {
    return GetObject (id, false);
  }

  /// <summary>
  /// Gets a <see cref="DomainObject"/> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> that should be loaded.</param>
  /// <param name="includeDeleted">Indicates if the method should return <see cref="DomainObject"/>s that are already deleted.</param>
  /// <returns>The <see cref="DomainObject"/> with the specified <i>id</i>.</returns>
  /// <exception cref="System.ArgumentNullException"><i>id</i> is a null reference.</exception>
  /// <exception cref="DataManagement.ObjectDeletedException"><i>includeDeleted</i> is false and the DomainObject with <i>id</i> has been deleted.</exception>
  // Todo documentation: all exceptions from ClientTransaction.LoadObject
  public virtual DomainObject GetObject (ObjectID id, bool includeDeleted)
  {
    ArgumentUtility.CheckNotNull ("id", id);
    return _dataManager.DataContainerMap.GetObject (id, includeDeleted);
  }

  internal DataContainer CreateNewDataContainer (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);

    ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[type];
    
    using (PersistenceManager persistenceManager = new PersistenceManager ())
    {
      DataContainer newDataContainer = persistenceManager.CreateNewDataContainer (classDefinition); 
      SetClientTransaction (newDataContainer);
      _dataManager.RegisterNewDataContainer (newDataContainer);
      
      return newDataContainer;
    }    
  }

  /// <summary>
  /// Evaluates if any relations of the given <see cref="DomainObject"/> have changed since instantiation, loading, commit or rollback.
  /// </summary>
  /// <param name="domainObject">The <see cref="DomainObject"/> to evaluate.</param>
  /// <returns><b>true</b> if any relations have changed; otherwise, <b>false</b>.</returns>
  /// <exception cref="System.ArgumentNullException"><i>domainObject</i> is a null reference.</exception>
  internal protected virtual bool HasRelationChanged (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);

    return _dataManager.RelationEndPointMap.HasRelationChanged (domainObject.DataContainer);
  }

  /// <summary>
  /// Gets the related object of a given <see cref="DataManagement.RelationEndPointID"/>.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> to evaluate. It must refer to a <see cref="DataManagement.ObjectEndPoint"/>.</param>
  /// <returns>The <see cref="DomainObject"/> that is the current related object.</returns>
  /// <exception cref="System.ArgumentNullException"><i>relationEndPointID</i> is a null reference.</exception>
  /// <exception cref="System.ArgumentException"><i>relationEndPointID</i> does not refer to an <see cref="DataManagement.ObjectEndPoint"/></exception>
  internal protected virtual DomainObject GetRelatedObject (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    return _dataManager.RelationEndPointMap.GetRelatedObject (relationEndPointID);
  }

  /// <summary>
  /// Gets the original related object of a given <see cref="DataManagement.RelationEndPointID"/> at the point of instantiation, loading, commit or rollback.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> to evaluate. It must refer to a <see cref="DataManagement.ObjectEndPoint"/>.</param>
  /// <returns>The <see cref="DomainObject"/> that is the original related object.</returns>
  /// <exception cref="System.ArgumentNullException"><i>relationEndPointID</i> is a null reference.</exception>
  /// <exception cref="System.ArgumentException"><i>relationEndPointID</i> does not refer to an <see cref="DataManagement.ObjectEndPoint"/></exception>
  internal protected virtual DomainObject GetOriginalRelatedObject (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    return _dataManager.RelationEndPointMap.GetOriginalRelatedObject (relationEndPointID);
  }

  /// <summary>
  /// Gets the related objects of a given <see cref="DataManagement.RelationEndPointID"/>.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> to evaluate. It must refer to a <see cref="DataManagement.CollectionEndPoint"/>.</param>
  /// <returns>A <see cref="DomainObjectCollection"/> containing the current related objects.</returns>
  /// <exception cref="System.ArgumentNullException"><i>relationEndPointID</i> is a null reference.</exception>
  /// <exception cref="System.ArgumentException"><i>relationEndPointID</i> does not refer to a <see cref="DataManagement.CollectionEndPoint"/></exception>
  internal protected virtual DomainObjectCollection GetRelatedObjects (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    return _dataManager.RelationEndPointMap.GetRelatedObjects (relationEndPointID);
  }

  /// <summary>
  /// Gets the original related objects of a given <see cref="DataManagement.RelationEndPointID"/> at the point of instantiation, loading, commit or rollback.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> to evaluate. It must refer to a <see cref="DataManagement.CollectionEndPoint"/>.</param>
  /// <returns>A <see cref="DomainObjectCollection"/> containing the original related objects.</returns>
  /// <exception cref="System.ArgumentNullException"><i>relationEndPointID</i> is a null reference.</exception>
  /// <exception cref="System.ArgumentException"><i>relationEndPointID</i> does not refer to a <see cref="DataManagement.CollectionEndPoint"/></exception>
  internal protected virtual DomainObjectCollection GetOriginalRelatedObjects (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    return _dataManager.RelationEndPointMap.GetOriginalRelatedObjects (relationEndPointID);
  }  

  /// <summary>
  /// Sets a relation between two relationEndPoints.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> referring the <see cref="DataManagement.RelationEndPoint"/> that should relate to <i>newRelatedObject</i>.</param>
  /// <param name="newRelatedObject">The new <see cref="DomainObject"/> that should be related; null indicates that no object should be referenced.</param>
  /// <exception cref="System.ArgumentNullException"><i>relationEndPointID</i> is a null reference.</exception>
  /// <exception cref="System.ArgumentException">
  ///   <i>relationEndPointID</i> does not refer to an <see cref="DataManagement.ObjectEndPoint"/>
  ///   <i>relationEndPointID</i> belongs to a <see cref="DomainObject"/> that has been deleted.
  ///   <i>newRelatedObject</i> has been deleted.
  /// </exception>
  internal protected virtual void SetRelatedObject (RelationEndPointID relationEndPointID, DomainObject newRelatedObject)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    _dataManager.RelationEndPointMap.SetRelatedObject (relationEndPointID, newRelatedObject);
  }
  
  /// <summary>
  /// Deletes a <see cref="DomainObject"/>.
  /// </summary>
  /// <param name="domainObject">The <see cref="DomainObject"/> to delete.</param>
  /// <exception cref="System.ArgumentNullException"><i>domainObject</i> is a null reference.</exception>
  internal protected virtual void Delete (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    _dataManager.Delete (domainObject);
  }
  
  /// <summary>
  /// Loads an object from the datasource.
  /// </summary>
  /// <remarks>
  /// This method raises the <see cref="Loaded"/> event.
  /// </remarks>
  /// <param name="id">An <see cref="ObjectID"/> object indicating which <see cref="DomainObject"/> to load.</param>
  /// <returns>The <see cref="DomainObject"/> object that was loaded.</returns>
  /// <exception cref="System.ArgumentNullException"><i>id</i> is a null reference.</exception>
  //todo documentation: exception of methods called
  internal protected virtual DomainObject LoadObject (ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    using (PersistenceManager persistenceManager = new PersistenceManager ())
    {
      DataContainer dataContainer = persistenceManager.LoadDataContainer (id);
      SetClientTransaction (dataContainer);

      _dataManager.RegisterExistingDataContainer (dataContainer);

      OnLoaded (new LoadedEventArgs (dataContainer.DomainObject));

      return dataContainer.DomainObject;
    }
  }

  /// <summary>
  /// Loads the related <see cref="DomainObject"/> of a given <see cref="DataManagement.RelationEndPointID"/>.
  /// </summary>
  /// <remarks>
  /// This method raises the <see cref="Loaded"/> event.
  /// </remarks>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> that should be evaluated. <i>relationEndPoint</i> must refer to a <see cref="ObjectEndPoint"/>.</param>
  /// <returns>The related <see cref="DomainObject"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><i>relationEndPointID</i> is a null reference.</exception>
  /// <exception cref="System.InvalidCastException"><i>relationEndPointID</i> does not refer to an <see cref="DataManagement.ObjectEndPoint"/></exception>
  /// <exception cref="DataManagement.ObjectDeletedException"><i>includeDeleted</i> is false and the DomainObject with <i>id</i> has been deleted.</exception>
  /// <exception cref="Persistence.PersistenceException">
  ///   The related object could not be loaded, but is mandatory.
  ///   The relation refers to non-existing object.
  ///   <i>relationEndPointID</i> does not refer to an <see cref="DataManagement.ObjectEndPoint"/>
  /// </exception>
  // Todo documentation: all exceptions from ClientTransaction.LoadObject
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
        SetClientTransaction (relatedDataContainer);
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

  /// <summary>
  /// Loads all related <see cref="DomainObject"/>s of a given <see cref="DataManagement.RelationEndPointID"/>. 
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> that should be evaluated. <i>relationEndPoint</i> must refer to a <see cref="CollectionEndPoint"/>.</param>
  /// <returns>A <see cref="DomainObjectCollection"/> containing all related <see cref="DomainObject"/>s.</returns>
  /// <exception cref="System.ArgumentNullException"><i>relationEndPointID</i> is a null reference.</exception>
  /// <exception cref="System.InvalidCastException"><i>relationEndPointID</i> does not refer to an <see cref="DataManagement.CollectionEndPoint"/></exception>
  //todo documentation: exceptions of called methods
  internal protected virtual DomainObjectCollection LoadRelatedObjects (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    using (PersistenceManager persistenceManager = new PersistenceManager ())
    {
      DataContainerCollection relatedDataContainers = persistenceManager.LoadRelatedDataContainers (relationEndPointID);
      return GetLoadedDomainObjects (relatedDataContainers, relationEndPointID);
    }
  }

  /// <summary>
  /// Creates a new <see cref="DomainObjectCollection"/>, registers the <see cref="DataContainer"/>s with this <b>ClientTransaction</b>, discards already loaded <see cref="DataContainers"/>, raises the <see cref="Load"/> event and optionally registers the relation with the specified <see cref="RelationEndPointID"/>.
  /// </summary>
  /// <param name="dataContainers">The newly loaded <see cref="DataContainer"/>s.</param>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> that should be evaluated. <i>relationEndPoint</i> must refer to a <see cref="CollectionEndPoint"/>.</param>
  /// <returns>A <see cref="DomainObjectCollection"/>.</returns>
  /// <exception cref="System.InvalidCastException"><i>collectionType</i> cannot be casted to <see cref="DomainObjectCollection"/>.</exception>
  internal protected virtual DomainObjectCollection GetLoadedDomainObjects (
      DataContainerCollection dataContainers, 
      RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    return GetLoadedDomainObjects (
        dataContainers, 
        relationEndPointID.Definition.PropertyType,
        relationEndPointID.OppositeEndPointDefinition.ClassDefinition.ClassType,
        relationEndPointID);
  }

  /// <summary>
  /// Creates a new <see cref="DomainObjectCollection"/> with the specified <i>collectionType.</i>, registers the <see cref="DataContainer"/>s with this <b>ClientTransaction</b>, discards already loaded <see cref="DataContainers"/> and raises the <see cref="Load"/> event.
  /// </summary>
  /// <param name="dataContainers">The newly loaded <see cref="DataContainer"/>s.</param>
  /// <param name="collectionType">The <see cref="Type"/> of the new collection that should be instantiated.</param>
  /// <returns>A <see cref="DomainObjectCollection"/>.</returns>
  /// <exception cref="System.InvalidCastException"><i>collectionType</i> cannot be casted to <see cref="DomainObjectCollection"/>.</exception>
  internal protected virtual DomainObjectCollection GetLoadedDomainObjects (
      DataContainerCollection dataContainers, 
      Type collectionType)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);
    ArgumentUtility.CheckNotNull ("collectionType", collectionType);

    return GetLoadedDomainObjects (dataContainers, collectionType, null, null);
  }

  /// <summary>
  /// Creates a new <see cref="DomainObjectCollection"/> with the specified <i>collectionType.</i>, registers the <see cref="DataContainer"/>s with this <b>ClientTransaction</b>, discards already loaded <see cref="DataContainers"/>, raises the <see cref="Load"/> event and optionally registers the relation with the specified <see cref="RelationEndPointID"/>.
  /// </summary>
  /// <param name="dataContainers">The newly loaded <see cref="DataContainer"/>s.</param>
  /// <param name="collectionType">The <see cref="Type"/> of the new collection that should be instantiated.</param>
  /// <param name="requiredItemType">The permitted <see cref="Type"/> of an item in the <see cref="DomainObjectCollection"/>. If specified only this type or derived types can be added to the <b>DomainObjectCollection</b>.</param>
  /// <param name="relationEndPointID"></param>
  /// <returns>A <see cref="DomainObjectCollection"/>.</returns>
  /// <exception cref="System.InvalidCastException"><i>collectionType</i> cannot be casted to <see cref="DomainObjectCollection"/>.</exception>
  internal protected virtual DomainObjectCollection GetLoadedDomainObjects (
      DataContainerCollection dataContainers, 
      Type collectionType,
      Type requiredItemType,
      RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);
    ArgumentUtility.CheckNotNull ("collectionType", collectionType);

    DataContainerCollection newLoadedDataContainers = _dataManager.DataContainerMap.GetNotExisting (dataContainers);
    SetClientTransaction (newLoadedDataContainers);
    _dataManager.RegisterExistingDataContainers (newLoadedDataContainers);

    DomainObjectCollection domainObjects = DomainObjectCollection.Create (
        collectionType, _dataManager.DataContainerMap.MergeWithExisting (dataContainers), requiredItemType);

    if (relationEndPointID != null)
      _dataManager.RelationEndPointMap.RegisterCollectionEndPoint (relationEndPointID, domainObjects);

    foreach (DataContainer newLoadedDataContainer in newLoadedDataContainers)
      OnLoaded (new LoadedEventArgs (newLoadedDataContainer.DomainObject));

    return domainObjects;
  }
      
  /// <summary>
  /// Sets the ClientTransaction property of all <see cref="DataContainer"/>s of a given <see cref="DataManagement.DataContainerCollection"/>.
  /// </summary>
  /// <param name="dataContainers">The <see cref="DataContainerCollection"/> with all the <see cref="DataContainer"/> objects that should be set to the <b>ClientTransaction</b>.</param>
  /// <exception cref="System.ArgumentNullException"><i>dataContainers</i> is a null reference.</exception>
  protected void SetClientTransaction (DataContainerCollection dataContainers)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);
    foreach (DataContainer dataContainer in dataContainers)
      SetClientTransaction (dataContainer);
  }

  /// <summary>
  /// Sets the ClientTransaction property of a given <see cref="DataContainer"/>
  /// </summary>
  /// <param name="dataContainer">The <see cref="DataContainer"/> that should be set to the <b>ClientTransaction</b>.</param>
  /// <exception cref="System.ArgumentNullException"><i>dataContainer</i> is a null reference.</exception>
  protected void SetClientTransaction (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
    dataContainer.SetClientTransaction (this);
  }

  /// <summary>
  /// Raises the <see cref="Loaded"/> event.
  /// </summary>
  /// <param name="args">A <see cref="LoadedEventArgs"/> object that contains the event data.</param>
  protected virtual void OnLoaded (LoadedEventArgs args)
  {
    args.LoadedDomainObject.EndObjectLoading ();

    if (Loaded != null)
      Loaded (this, args);
  }

  /// <summary>
  /// Gets the <see cref="DataManager"/> of the <b>ClientTransaction</b>.
  /// </summary>
  protected DataManager DataManager
  {
    get { return _dataManager; }
  }

  /// <summary>
  /// Gets the <see cref="QueryManager"/> of the <b>ClientTransaction</b>.
  /// </summary>
  public QueryManager QueryManager 
  {
    get { return _queryManager; }
  }
}
}