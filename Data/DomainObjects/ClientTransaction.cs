using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Utilities;


namespace Rubicon.Data.DomainObjects
{
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

  // construction and disposing

  /// <summary>
  /// Initializes a new instance of the <b>ClientTransaction</b> class.
  /// </summary>
  public ClientTransaction ()
  {
    Initialize ();
  }

  // methods and properties

  /// <summary>
  /// Commits all changes within the <b>ClientTransaction</b> to the persistent datasources.
  /// </summary>
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
  public virtual void Rollback ()
  {
    _dataManager.Rollback ();
  }

//TODO documentation: What happens if no object is found
  /// <summary>
  /// Gets a <see cref="DomainObject"/> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> that should be loaded.</param>
  /// <returns>The <see cref="DomainObject"/> with the specified <i>id</i>.</returns>
  /// <exception cref="System.ArgumentNullException"><i>id</i> is a null reference.</exception>
  public virtual DomainObject GetObject (ObjectID id)
  {
    return GetObject (id, false);
  }

//TODO documentation: What happens if no object is found
  /// <summary>
  /// Gets a <see cref="DomainObject"/> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> that should be loaded.</param>
  /// <param name="includeDeleted">Indicates if the method should return <see cref="DomainObject"/>s that are already deleted.</param>
  /// <returns>The <see cref="DomainObject"/> with the specified <i>id</i>.</returns>
  /// <exception cref="System.ArgumentNullException"><i>id</i> is a null reference.</exception>
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

//TODO documentation: check if the statement about the invalidCastOperation is right
  /// <summary>
  /// Gets the related object of a given <see cref="DataManagement.RelationEndPointID"/>.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> to evaluate. It must refer to a <see cref="DataManagement.ObjectEndPoint"/>.</param>
  /// <returns>The <see cref="DomainObject"/> that is the current related object.</returns>
  /// <exception cref="System.ArgumentNullException"><i>relationEndPointID</i> is a null reference.</exception>
  /// <exception cref="System.InvalidCastException"><i>relationEndPointID</i> does not refer to an <see cref="DataManagement.ObjectEndPoint"/></exception>
  internal protected virtual DomainObject GetRelatedObject (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    return _dataManager.RelationEndPointMap.GetRelatedObject (relationEndPointID);
  }

//TODO documentation: check if the statement about the invalidCastOperation is right
  /// <summary>
  /// Gets the original related object of a given <see cref="DataManagement.RelationEndPointID"/> at the point of instantiation, loading, commit or rollback.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> to evaluate. It must refer to a <see cref="DataManagement.ObjectEndPoint"/>.</param>
  /// <returns>The <see cref="DomainObject"/> that is the original related object.</returns>
  /// <exception cref="System.ArgumentNullException"><i>relationEndPointID</i> is a null reference.</exception>
  /// <exception cref="System.InvalidCastException"><i>relationEndPointID</i> does not refer to an <see cref="DataManagement.ObjectEndPoint"/></exception>
  internal protected virtual DomainObject GetOriginalRelatedObject (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    return _dataManager.RelationEndPointMap.GetOriginalRelatedObject (relationEndPointID);
  }

//TODO documentation: check if the statement about the invalidCastOperation is right
  /// <summary>
  /// Gets the related objects of a given <see cref="DataManagement.RelationEndPointID"/>.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> to evaluate. It must refer to a <see cref="DataManagement.CollectionEndPoint"/>.</param>
  /// <returns>A <see cref="DomainObjectCollection"/> containing the current related objects.</returns>
  /// <exception cref="System.ArgumentNullException"><i>relationEndPointID</i> is a null reference.</exception>
  /// <exception cref="System.InvalidCastException"><i>relationEndPointID</i> does not refer to a <see cref="DataManagement.CollectionEndPoint"/></exception>
  internal protected virtual DomainObjectCollection GetRelatedObjects (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    return _dataManager.RelationEndPointMap.GetRelatedObjects (relationEndPointID);
  }

//TODO documentation: check if the statement about the invalidCastOperation is right
  /// <summary>
  /// Gets the original related objects of a given <see cref="DataManagement.RelationEndPointID"/> at the point of instantiation, loading, commit or rollback.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> to evaluate. It must refer to a <see cref="DataManagement.CollectionEndPoint"/>.</param>
  /// <returns>A <see cref="DomainObjectCollection"/> containing the original related objects.</returns>
  /// <exception cref="System.ArgumentNullException"><i>relationEndPointID</i> is a null reference.</exception>
  /// <exception cref="System.InvalidCastException"><i>relationEndPointID</i> does not refer to a <see cref="DataManagement.CollectionEndPoint"/></exception>
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

//TODO documentation: check if the statement about the invalidCastOperation is right
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
  internal protected virtual DomainObjectCollection LoadRelatedObjects (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    using (PersistenceManager persistenceManager = new PersistenceManager ())
    {
      DataContainerCollection relatedDataContainers = persistenceManager.LoadRelatedDataContainers (relationEndPointID);

      DataContainerCollection newLoadedDataContainers = _dataManager.DataContainerMap.GetNotExisting (relatedDataContainers);
      SetClientTransaction (newLoadedDataContainers);
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
  /// Gets the current <see cref="DataManager"/> of the <b>ClientTransaction</b>.
  /// </summary>
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