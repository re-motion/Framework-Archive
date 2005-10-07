using System;
using System.Runtime.Remoting.Messaging;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Utilities;


namespace Rubicon.Data.DomainObjects
{
/// <summary>
/// Represents an in-memory transaction.
/// </summary>
/// <remarks>
/// There are two ways a <see cref="ClientTransaction"/> can be used:<br />
/// <list type="bullet">
///   <item>
///     <description>
///       The transaction is initialized automatically through <see cref="Current"/> and is associated with the current <see cref="System.Threading.Thread"/>.
///     </description>
///   </item>
///   <item>   
///     <description>
///       Multiple transactions can be instantiated with the constructor and used side-by-side. 
///       Every <see cref="DomainObject"/> must then be associated with the particular transaction.
///     </description>
///   </item>
/// </list>
/// </remarks>
public class ClientTransaction : ITransaction
{
  // types

  // static members and constants

  private const string c_callContextKey = "Rubicon.Data.DomainObjects.ClientTransaction.Current";

  /// <summary>
  /// Gets a value indicating if a <see cref="ClientTransaction"/> is currently set as <see cref="Current"/>. 
  /// </summary>
  /// <remarks>
  /// Even if the value returned by <b>HasCurrent</b> is false, <see cref="Current"/> will return a <see cref="ClientTransaction"/>. See <see cref="Current"/> for further de.
  /// </remarks>
  public static bool HasCurrent
  {
    get { return GetCurrentInternal () != null ; }
  }

  /// <summary>
  /// Gets the default <b>ClientTransaction</b> of the current thread. 
  /// </summary>
  /// <remarks>If there is no <see cref="ClientTransaction"/> associated with the current thread, a new <see cref="ClientTransaction"/> is created.</remarks>
  public static ClientTransaction Current
  {
    get 
    {
      if (!HasCurrent)
        SetCurrent (new ClientTransaction ());
      
      return GetCurrentInternal ();
    }
  }

  /// <summary>
  /// Sets the default <b>ClientTransaction</b> of the current thread.
  /// </summary>
  /// <param name="clientTransaction">The <b>ClientTransaction</b> to which the current <b>ClientTransaction</b> is set.</param>
  public static void SetCurrent (ClientTransaction clientTransaction)
  {
    CallContext.SetData (c_callContextKey, clientTransaction);
  }

  private static ClientTransaction GetCurrentInternal ()
  {
    return (ClientTransaction) CallContext.GetData (c_callContextKey);
  }

  // member fields

  /// <summary>
  /// Occurs after the <b>ClientTransaction</b> has loaded a new object.
  /// </summary>
  public event ClientTransactionEventHandler Loaded;

  /// <summary>
  /// Occurs immediately before the <b>ClientTransaction</b> performs a <see cref="Commit"/> operation.
  /// </summary>
  public event ClientTransactionEventHandler Committing;

  /// <summary>
  /// Occurs immediately after the <b>ClientTransaction</b> has successfully performed a <see cref="Commit"/> operation.
  /// </summary>
  public event ClientTransactionEventHandler Committed;

  private DataManager _dataManager;
  private QueryManager _queryManager;

  // construction and disposing

  /// <summary>
  /// Initializes a new instance of the <b>ClientTransaction</b> class.
  /// </summary>
  public ClientTransaction ()
  {
    _dataManager = new DataManager (this);
  }

  // methods and properties

  /// <summary>
  /// Commits all changes within the <b>ClientTransaction</b> to the persistent datasources.
  /// </summary>
  /// <exception cref="Persistence.PersistenceException">Changes to objects from multiple storage providers were made.</exception>
  /// <exception cref="Persistence.StorageProviderException">An error occurred while committing the changes to the datasource.</exception>
  public virtual void Commit ()
  {
    BeginCommit ();
    DomainObjectCollection changedButNotDeletedDomainObjects = _dataManager.GetChangedDomainObjects (false); 

    DataContainerCollection changedDataContainers = _dataManager.GetChangedDataContainersForCommit ();
    if (changedDataContainers.Count > 0)
    {
      using (PersistenceManager persistenceManager = new PersistenceManager ())
      {
        persistenceManager.Save (changedDataContainers);
      }
    }

    _dataManager.Commit ();
    EndCommit (changedButNotDeletedDomainObjects);
  }

  /// <summary>
  /// Performs a rollback of all changes withing the <b>ClientTransaction</b>.
  /// </summary>
  public virtual void Rollback ()
  {
    _dataManager.Rollback ();
  }

  /// <summary>
  /// Gets a <see cref="DomainObject"/> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> that should be loaded. Must not be <see langword="null"/>.</param>
  /// <returns>The <see cref="DomainObject"/> with the specified <i>id</i>.</returns>
  /// <exception cref="System.ArgumentNullException"><i>id</i> is <see langword="null"/>.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <i>id</i>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  public virtual DomainObject GetObject (ObjectID id)
  {
    return GetObject (id, false);
  }

  /// <summary>
  /// Gets a <see cref="DomainObject"/> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> that should be loaded. Must not be <see langword="null"/>.</param>
  /// <param name="includeDeleted">Indicates if the method should return <see cref="DomainObject"/>s that are already deleted.</param>
  /// <returns>The <see cref="DomainObject"/> with the specified <i>id</i>.</returns>
  /// <exception cref="System.ArgumentNullException"><i>id</i> is <see langword="null"/>.</exception>
  /// <exception cref="DataManagement.ObjectDeletedException"><i>includeDeleted</i> is false and the DomainObject with <i>id</i> has been deleted.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <i>id</i>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  public virtual DomainObject GetObject (ObjectID id, bool includeDeleted)
  {
    ArgumentUtility.CheckNotNull ("id", id);
    return _dataManager.DataContainerMap.GetObject (id, includeDeleted);
  }

  internal DataContainer CreateNewDataContainer (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);

    ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (type);
    
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
  /// <param name="domainObject">The <see cref="DomainObject"/> to evaluate. Must not be <see langword="null"/>.</param>
  /// <returns><b>true</b> if any relations have changed; otherwise, <b>false</b>.</returns>
  /// <exception cref="System.ArgumentNullException"><i>domainObject</i> is <see langword="null"/>.</exception>
  internal protected virtual bool HasRelationChanged (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);

    return _dataManager.RelationEndPointMap.HasRelationChanged (domainObject.DataContainer);
  }

  /// <summary>
  /// Gets the related object of a given <see cref="DataManagement.RelationEndPointID"/>.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> to evaluate. It must refer to a <see cref="DataManagement.ObjectEndPoint"/>. Must not be <see langword="null"/>.</param>
  /// <returns>The <see cref="DomainObject"/> that is the current related object.</returns>
  /// <exception cref="System.ArgumentNullException"><i>relationEndPointID</i> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentException"><i>relationEndPointID</i> does not refer to an <see cref="DataManagement.ObjectEndPoint"/></exception>
  internal protected virtual DomainObject GetRelatedObject (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    return _dataManager.RelationEndPointMap.GetRelatedObject (relationEndPointID);
  }

  /// <summary>
  /// Gets the original related object of a given <see cref="DataManagement.RelationEndPointID"/> at the point of instantiation, loading, commit or rollback.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> to evaluate. It must refer to a <see cref="DataManagement.ObjectEndPoint"/>. Must not be <see langword="null"/>.</param>
  /// <returns>The <see cref="DomainObject"/> that is the original related object.</returns>
  /// <exception cref="System.ArgumentNullException"><i>relationEndPointID</i> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentException"><i>relationEndPointID</i> does not refer to an <see cref="DataManagement.ObjectEndPoint"/></exception>
  internal protected virtual DomainObject GetOriginalRelatedObject (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    return _dataManager.RelationEndPointMap.GetOriginalRelatedObject (relationEndPointID);
  }

  /// <summary>
  /// Gets the related objects of a given <see cref="DataManagement.RelationEndPointID"/>.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> to evaluate. It must refer to a <see cref="DataManagement.CollectionEndPoint"/>. Must not be <see langword="null"/>.</param>
  /// <returns>A <see cref="DomainObjectCollection"/> containing the current related objects.</returns>
  /// <exception cref="System.ArgumentNullException"><i>relationEndPointID</i> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentException"><i>relationEndPointID</i> does not refer to a <see cref="DataManagement.CollectionEndPoint"/></exception>
  internal protected virtual DomainObjectCollection GetRelatedObjects (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    return _dataManager.RelationEndPointMap.GetRelatedObjects (relationEndPointID);
  }

  /// <summary>
  /// Gets the original related objects of a given <see cref="DataManagement.RelationEndPointID"/> at the point of instantiation, loading, commit or rollback.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> to evaluate. It must refer to a <see cref="DataManagement.CollectionEndPoint"/>. Must not be <see langword="null"/>.</param>
  /// <returns>A <see cref="DomainObjectCollection"/> containing the original related objects.</returns>
  /// <exception cref="System.ArgumentNullException"><i>relationEndPointID</i> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentException"><i>relationEndPointID</i> does not refer to a <see cref="DataManagement.CollectionEndPoint"/></exception>
  internal protected virtual DomainObjectCollection GetOriginalRelatedObjects (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    return _dataManager.RelationEndPointMap.GetOriginalRelatedObjects (relationEndPointID);
  }  

  /// <summary>
  /// Sets a relation between two relationEndPoints.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> referring the <see cref="DataManagement.RelationEndPoint"/> that should relate to <i>newRelatedObject</i>. Must not be <see langword="null"/>.</param>
  /// <param name="newRelatedObject">The new <see cref="DomainObject"/> that should be related; <see langword="null"/> indicates that no object should be referenced.</param>
  /// <exception cref="System.ArgumentNullException"><i>relationEndPointID</i> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentException">
  ///   <i>relationEndPointID</i> does not refer to an <see cref="DataManagement.ObjectEndPoint"/><br /> -or- <br />
  ///   <i>relationEndPointID</i> belongs to a <see cref="DomainObject"/> that has been deleted.<br /> -or- <br />
  ///   <i>newRelatedObject</i> has been deleted.
  /// </exception>
  /// <exception cref="DataManagement.ClientTransactionsDifferException">
  ///   <i>newRelatedObject</i> does belongs to a different <b>ClientTransaction</b>.
  /// </exception>
  internal protected virtual void SetRelatedObject (RelationEndPointID relationEndPointID, DomainObject newRelatedObject)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    _dataManager.RelationEndPointMap.SetRelatedObject (relationEndPointID, newRelatedObject);
  }
  
  /// <summary>
  /// Deletes a <see cref="DomainObject"/>.
  /// </summary>
  /// <param name="domainObject">The <see cref="DomainObject"/> to delete. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><i>domainObject</i> is <see langword="null"/>.</exception>
  /// <exception cref="DataManagement.ClientTransactionsDifferException">
  ///   <i>domainObject</i> belongs to a different <see cref="ClientTransaction"/>. 
  /// </exception>
  public virtual void Delete (DomainObject domainObject)
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
  /// <param name="id">An <see cref="ObjectID"/> object indicating which <see cref="DomainObject"/> to load. Must not be <see langword="null"/>.</param>
  /// <returns>The <see cref="DomainObject"/> object that was loaded.</returns>
  /// <exception cref="System.ArgumentNullException"><i>id</i> is <see langword="null"/>.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <i>id</i>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  internal protected virtual DomainObject LoadObject (ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    using (PersistenceManager persistenceManager = new PersistenceManager ())
    {
      DataContainer dataContainer = persistenceManager.LoadDataContainer (id);
      SetClientTransaction (dataContainer);

      _dataManager.RegisterExistingDataContainer (dataContainer);

      DomainObjectCollection loadedDomainObjects = new DomainObjectCollection (new DomainObject[] {dataContainer.DomainObject}, true);
      OnLoaded (new ClientTransactionEventArgs (loadedDomainObjects));

      return dataContainer.DomainObject;
    }
  }

  /// <summary>
  /// Loads the related <see cref="DomainObject"/> of a given <see cref="DataManagement.RelationEndPointID"/>.
  /// </summary>
  /// <remarks>
  /// This method raises the <see cref="Loaded"/> event.
  /// </remarks>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> that should be evaluated. <i>relationEndPoint</i> must refer to a <see cref="ObjectEndPoint"/>. Must not be <see langword="null"/>.</param>
  /// <returns>The related <see cref="DomainObject"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><i>relationEndPointID</i> is <see langword="null"/>.</exception>
  /// <exception cref="System.InvalidCastException"><i>relationEndPointID</i> does not refer to an <see cref="DataManagement.ObjectEndPoint"/></exception>
  /// <exception cref="DataManagement.ObjectDeletedException"><i>includeDeleted</i> is false and the DomainObject with <i>id</i> has been deleted.</exception>
  /// <exception cref="Persistence.PersistenceException">
  ///   The related object could not be loaded, but is mandatory.<br /> -or- <br />
  ///   The relation refers to non-existing object.<br /> -or- <br />
  ///   <i>relationEndPointID</i> does not refer to an <see cref="DataManagement.ObjectEndPoint"/>.
  /// </exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <i>id</i>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
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

        DomainObjectCollection loadedDomainObjects = new DomainObjectCollection (new DomainObject[] {relatedDataContainer.DomainObject}, true);
        OnLoaded (new ClientTransactionEventArgs (loadedDomainObjects));

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
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> that should be evaluated. <i>relationEndPoint</i> must refer to a <see cref="CollectionEndPoint"/>. Must not be <see langword="null"/>.</param>
  /// <returns>A <see cref="DomainObjectCollection"/> containing all related <see cref="DomainObject"/>s.</returns>
  /// <exception cref="System.ArgumentNullException"><i>relationEndPointID</i> is <see langword="null"/>.</exception>
  /// <exception cref="Persistence.PersistenceException">
  ///   <i>relationEndPointID</i> does not refer to one-to-many relation.<br /> -or- <br />
  ///   The StorageProvider for the related objects could not be initialized.
  /// </exception>
  internal protected virtual DomainObjectCollection LoadRelatedObjects (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    using (PersistenceManager persistenceManager = new PersistenceManager ())
    {
      DataContainerCollection relatedDataContainers = persistenceManager.LoadRelatedDataContainers (relationEndPointID);
      return MergeLoadedDomainObjects (relatedDataContainers, relationEndPointID);
    }
  }

  /// <summary>
  /// Creates a new <see cref="DomainObjectCollection"/>, registers the <see cref="DataContainer"/>s with this <b>ClientTransaction</b>, discards already loaded <see cref="DataContainer"/>s, raises the <see cref="Loaded"/> event and optionally registers the relation with the specified <see cref="DataManagement.RelationEndPointID"/>.
  /// </summary>
  /// <param name="dataContainers">The newly loaded <see cref="DataContainer"/>s.</param>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> that should be evaluated. <i>relationEndPoint</i> must refer to a <see cref="CollectionEndPoint"/>.</param>
  /// <returns>A <see cref="DomainObjectCollection"/>.</returns>
  /// <exception cref="System.InvalidCastException"><i>collectionType</i> cannot be casted to <see cref="DomainObjectCollection"/>.</exception>
  internal DomainObjectCollection MergeLoadedDomainObjects (
      DataContainerCollection dataContainers, 
      RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    return MergeLoadedDomainObjects (
        dataContainers, 
        relationEndPointID.Definition.PropertyType,
        relationEndPointID.OppositeEndPointDefinition.ClassDefinition.ClassType,
        relationEndPointID);
  }

  /// <summary>
  /// Creates a new <see cref="DomainObjectCollection"/> with the specified <i>collectionType</i>, registers the <see cref="DataContainer"/>s with this <b>ClientTransaction</b>, discards already loaded <see cref="DataContainer"/>s and raises the <see cref="Loaded"/> event.
  /// </summary>
  /// <param name="dataContainers">The newly loaded <see cref="DataContainer"/>s.</param>
  /// <param name="collectionType">The <see cref="Type"/> of the new collection that should be instantiated.</param>
  /// <returns>A <see cref="DomainObjectCollection"/>.</returns>
  /// <exception cref="System.InvalidCastException"><i>collectionType</i> cannot be casted to <see cref="DomainObjectCollection"/>.</exception>
  internal DomainObjectCollection MergeLoadedDomainObjects (
      DataContainerCollection dataContainers, 
      Type collectionType)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);
    ArgumentUtility.CheckNotNull ("collectionType", collectionType);

    return MergeLoadedDomainObjects (dataContainers, collectionType, null, null);
  }

  /// <summary>
  /// Creates a new <see cref="DomainObjectCollection"/> with the specified <i>collectionType.</i>, registers the <see cref="DataContainer"/>s with this <b>ClientTransaction</b>, discards already loaded <see cref="DataContainer"/>s, raises the <see cref="Loaded"/> event and optionally registers the relation with the specified <see cref="DataManagement.RelationEndPointID"/>.
  /// </summary>
  /// <param name="dataContainers">The newly loaded <see cref="DataContainer"/>s.</param>
  /// <param name="collectionType">The <see cref="Type"/> of the new collection that should be instantiated.</param>
  /// <param name="requiredItemType">The permitted <see cref="Type"/> of an item in the <see cref="DomainObjectCollection"/>. If specified only this type or derived types can be added to the <b>DomainObjectCollection</b>.</param>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> that should be evaluated.</param>
  /// <returns>A <see cref="DomainObjectCollection"/>.</returns>
  /// <exception cref="System.InvalidCastException"><i>collectionType</i> cannot be casted to <see cref="DomainObjectCollection"/>.</exception>
  internal DomainObjectCollection MergeLoadedDomainObjects (
      DataContainerCollection dataContainers, 
      Type collectionType,
      Type requiredItemType,
      RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);
    ArgumentUtility.CheckNotNull ("collectionType", collectionType);

    DataContainerCollection newLoadedDataContainers = _dataManager.DataContainerMap.GetNotRegisteredDataContainers (dataContainers);
    SetClientTransaction (newLoadedDataContainers);
    _dataManager.RegisterExistingDataContainers (newLoadedDataContainers);

    DomainObjectCollection domainObjects = DomainObjectCollection.Create (
        collectionType, _dataManager.DataContainerMap.MergeWithRegisteredDataContainers (dataContainers), requiredItemType);

    if (relationEndPointID != null)
      _dataManager.RelationEndPointMap.RegisterCollectionEndPoint (relationEndPointID, domainObjects);

    DomainObjectCollection newLoadedDomainObjects = new DomainObjectCollection (newLoadedDataContainers, true);
    OnLoaded (new ClientTransactionEventArgs (newLoadedDomainObjects));

    return domainObjects;
  }
      
  /// <summary>
  /// Sets the ClientTransaction property of all <see cref="DataContainer"/>s of a given <see cref="DataManagement.DataContainerCollection"/>.
  /// </summary>
  /// <param name="dataContainers">The <see cref="DataContainerCollection"/> with all the <see cref="DataContainer"/> objects that should be set to the <b>ClientTransaction</b>. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><i>dataContainers</i> is <see langword="null"/>.</exception>
  protected void SetClientTransaction (DataContainerCollection dataContainers)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);
    foreach (DataContainer dataContainer in dataContainers)
      SetClientTransaction (dataContainer);
  }

  /// <summary>
  /// Sets the ClientTransaction property of a given <see cref="DataContainer"/>
  /// </summary>
  /// <param name="dataContainer">The <see cref="DataContainer"/> that should be set to the <b>ClientTransaction</b>. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><i>dataContainer</i> is <see langword="null"/>.</exception>
  protected void SetClientTransaction (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
    dataContainer.SetClientTransaction (this);
  }

  /// <summary>
  /// Raises the <see cref="Loaded"/> event.
  /// </summary>
  /// <param name="args">A <see cref="ClientTransactionEventArgs"/> object that contains the event data.</param>
  protected virtual void OnLoaded (ClientTransactionEventArgs args)
  {
    foreach (DomainObject loadedDomainObject in args.DomainObjects)
      loadedDomainObject.EndObjectLoading ();

    if (Loaded != null)
      Loaded (this, args);
  }

  /// <summary>
  /// Raises the <see cref="Committing"/> event.
  /// </summary>
  /// <param name="args">A <see cref="ClientTransactionEventArgs"/> object that contains the event data.</param>
  protected virtual void OnCommitting (ClientTransactionEventArgs args)
  {
    if (Committing != null)
      Committing (this, args);
  }

  /// <summary>
  /// Raises the <see cref="Committed"/> event.
  /// </summary>
  /// <param name="args">A <see cref="ClientTransactionEventArgs"/> object that contains the event data.</param>
  protected virtual void OnCommitted (ClientTransactionEventArgs args)
  {
    if (Committed != null)
      Committed (this, args);
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
  public virtual QueryManager QueryManager 
  {
    get 
    {
      if (_queryManager == null)
        _queryManager = new QueryManager (this);

      return _queryManager; 
    }
  }

  private void BeginCommit ()
  {
    DomainObjectCollection changedDomainObjects = _dataManager.GetChangedDomainObjects ();
    DomainObjectCollection domainObjectComittingEventRaised = new DomainObjectCollection ();
    DomainObjectCollection clientTransactionCommittingEventRaised = new DomainObjectCollection ();

    DomainObjectCollection clientTransactionCommittingEventNotRaised = changedDomainObjects;
    do
    {
      DomainObjectCollection domainObjectCommittingEventNotRaised = domainObjectComittingEventRaised.GetItemsNotInCollection (changedDomainObjects);
      while (domainObjectCommittingEventNotRaised.Count > 0)
      {
        foreach (DomainObject domainObject in domainObjectCommittingEventNotRaised)
        {
          if (!domainObject.IsDiscarded)
          {
            domainObject.BeginCommit ();

            if (!domainObject.IsDiscarded)
              domainObjectComittingEventRaised.Add (domainObject);
          }
        }

        changedDomainObjects = _dataManager.GetChangedDomainObjects ();
        domainObjectCommittingEventNotRaised = domainObjectComittingEventRaised.GetItemsNotInCollection (changedDomainObjects);
      }

      clientTransactionCommittingEventNotRaised = clientTransactionCommittingEventRaised.GetItemsNotInCollection (changedDomainObjects);
      
      OnCommitting (new ClientTransactionEventArgs (clientTransactionCommittingEventNotRaised.Clone (true)));
      foreach (DomainObject domainObject in clientTransactionCommittingEventNotRaised)
      {
        if (!domainObject.IsDiscarded)
          clientTransactionCommittingEventRaised.Add (domainObject);
      }

      changedDomainObjects = _dataManager.GetChangedDomainObjects ();
      clientTransactionCommittingEventNotRaised = clientTransactionCommittingEventRaised.GetItemsNotInCollection (changedDomainObjects);
    } while (clientTransactionCommittingEventNotRaised.Count > 0);
  }

  private void EndCommit (DomainObjectCollection changedDomainObjects)
  {
    foreach (DomainObject changedDomainObject in changedDomainObjects)
      changedDomainObject.EndCommit ();
    
    OnCommitted (new ClientTransactionEventArgs (changedDomainObjects.Clone (true)));
  }
 
  #region ITransaction Members

  /// <summary>
  /// Child Transactions are not supported by the framework so an exception is thrown when this method is called.
  /// </summary>
  /// <exception cref="System.NotImplementedException">Method is called.</exception>
  ITransaction ITransaction.CreateChild ()
  {
    throw new NotImplementedException ("ClientTransactions do not support nested transactions.");
  }

  /// <summary>
  /// Child Transactions are not supported by the framework, therefore the property always returns false.
  /// </summary>
  bool ITransaction.IsChild
  {
    get { return false; }
  }

  /// <summary>
  /// Child Transactions are not supported by the framework, therefore the property always returns false.
  /// </summary>
  bool ITransaction.CanCreateChild
  {
    get { return false; }
  }

  /// <summary>
  /// No action is performed by this method.
  /// </summary>
  void ITransaction.Release ()
  {
  }

  /// <summary>
  /// Child Transactions are not supported by the framework, therefore the property always returns <see langword="null"/>.
  /// </summary>
  ITransaction ITransaction.Parent
  {
    get { return null; }
  }

  #endregion
}
}