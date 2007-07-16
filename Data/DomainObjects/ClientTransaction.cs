using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects
{
/// <summary>
/// Represents an in-memory transaction.
/// </summary>
/// <remarks>
/// <para>
/// There are two ways a <see cref="ClientTransaction"/> can be used:<br />
/// <list type="bullet">
///   <item>
///     <description>
///       The transaction is initialized automatically through <see cref="ClientTransactionScope.CurrentTransaction"/> and is associated with the
///       current <see cref="System.Threading.Thread"/>.
///     </description>
///   </item>
///   <item>   
///     <description>
///       Multiple transactions can be instantiated with the constructor and used side-by-side. 
///       Every <see cref="DomainObject"/> must then be associated with the current thread via <see cref="ClientTransactionScope"/>, e.g. by calling
///       <see cref="EnterScope"/>.
///     </description>
///   </item>
/// </list>
/// </para>
/// <para>
/// <see cref="ClientTransaction">ClientTransaction's</see> methods temporarily set the <see cref="ClientTransactionScope"/> to this instance to
/// ensure they are executed in the right context.
/// </para>
/// </remarks>
[Serializable]
public class ClientTransaction : ITransaction
{
  // types

  // static members and constants

  // member fields

  /// <summary>
  /// Occurs after the <b>ClientTransaction</b> has loaded a new object.
  /// </summary>
  /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
  public event ClientTransactionEventHandler Loaded;

  /// <summary>
  /// Occurs immediately before the <b>ClientTransaction</b> performs a <see cref="Commit"/> operation.
  /// </summary>
  /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
  public event ClientTransactionEventHandler Committing;

  /// <summary>
  /// Occurs immediately after the <b>ClientTransaction</b> has successfully performed a <see cref="Commit"/> operation.
  /// </summary>
  /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
  public event ClientTransactionEventHandler Committed;

  /// <summary>
  /// Occurs immediately before the <b>ClientTransaction</b> performs a <see cref="Rollback"/> operation.
  /// </summary>
  /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
  public event ClientTransactionEventHandler RollingBack;

  /// <summary>
  /// Occurs immediately after the <b>ClientTransaction</b> has successfully performed a <see cref="Rollback"/> operation.
  /// </summary>
  /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
  public event ClientTransactionEventHandler RolledBack;

  private readonly ClientTransaction _parentTransaction;
  private readonly DataManager _dataManager;
  private QueryManager _queryManager;
  private readonly Dictionary<Enum, object> _applicationData;
  private readonly CompoundClientTransactionListener _listeners;
  private readonly ClientTransactionExtensionCollection _extensions;

  private bool _isReadOnly;
  
  // construction and disposing

  /// <summary>
  /// Initializes a new instance of the <b>ClientTransaction</b> class.
  /// </summary>
  public ClientTransaction ()
  {
    _listeners = new CompoundClientTransactionListener ();

    _extensions = new ClientTransactionExtensionCollection ();
    _listeners.AddListener (new LoggingClientTransactionListener ());
    _listeners.AddListener (new ReadOnlyClientTransactionListener (this));
    _listeners.AddListener (new ExtensionClientTransactionListener (_extensions));

    _isReadOnly = false;
    _dataManager = new DataManager (this);
    _applicationData = new Dictionary<Enum, object> ();
  }

  /// <summary>
  /// Initializes a new subtransaction.
  /// </summary>
  public ClientTransaction (ClientTransaction parentTransaction) : this ()
  {
    ArgumentUtility.CheckNotNull ("parentTransaction", parentTransaction);

    _parentTransaction = parentTransaction;
    parentTransaction.IsReadOnly = true;
  }

  // methods and properties

  /// <summary>
  /// Indicates whether this transaction is set read-only.
  /// </summary>
  /// <value>True if this instance is set read-only; otherwise, false.</value>
  /// <remarks>Transactions are set read-only while there exist open subtransactions for them. A read-only transaction can only be used for
  /// operations that do not cause any change of transaction state. Most reading operations that do not require objects to be loaded
  /// from the data store are safe to be used on read-only transactions, but any method that would cause a state change will throw an exception.
  /// </remarks>
  public bool IsReadOnly
  {
    get { return _isReadOnly; }
    internal protected set { _isReadOnly = value; }
  }

  /// <summary>
  /// Gets the parent transaction for this <see cref="ClientTransaction"/>.
  /// </summary>
  /// <value>The parent transaction.</value>
  public ClientTransaction ParentTransaction
  {
    get { return _parentTransaction; }
  }

  /// <summary>
  /// Gets the collection of <see cref="IClientTransactionExtension"/>s of this <see cref="ClientTransaction"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  ///   Use <see cref="ClientTransactionExtensionCollection.Add"/> and <see cref="ClientTransactionExtensionCollection.Remove"/> 
  ///   to register and unregister an extension.
  /// </para>
  /// <para>
  ///   The order of the extensions in this collection is the order in which they are notified.
  /// </para>
  /// </remarks>
  public ClientTransactionExtensionCollection Extensions
  {
    get { return _extensions; }
  }

  /// <summary>
  /// Gets the transaction event sink for this transaction.
  /// </summary>
  /// <value>The transaction event sink for this transaction.</value>
  /// <remarks>
  /// Objects such as <see cref="DataManager"/>, changes to which logically represent changes to the transaction, can use the object returned by
  /// this property in order to inform the <see cref="ClientTransaction"/> and its listeners of events.
  /// </remarks>
  internal IClientTransactionListener TransactionEventSink
  {
    get { return _listeners; }
  }

  protected internal void AddListener (IClientTransactionListener listener)
  {
    _listeners.AddListener (listener);
  }

  /// <summary>
  /// Creates a new <see cref="ClientTransactionScope"/> for this transaction and enters it.
  /// </summary>
  /// <returns>A new <see cref="ClientTransactionScope"/> fot rhis transaction.</returns>
  /// <remarks>This method exists for convenience and is equivalent to <c>new ClientTransactionScope (this)</c>.</remarks>
  public ClientTransactionScope EnterScope ()
  {
    return new ClientTransactionScope (this);
  }

  /// <summary>
  /// Returns whether at least one <see cref="DomainObject"/> in this <b>ClientTransaction</b> has been changed.
  /// </summary>
  /// <returns><see langword="true"/> if at least one <see cref="DomainObject"/> in this <b>ClientTransaction</b> has been changed; otherwise, <see langword="false"/>.</returns>
  public virtual bool HasChanged ()
  {
    using (EnterScope ())
    {
      DomainObjectCollection changedDomainObjects = _dataManager.GetChangedDomainObjects();
      return changedDomainObjects.Count > 0;
    }
  }

  /// <summary>
  /// Commits all changes within the <b>ClientTransaction</b> to the persistent datasources.
  /// </summary>
  /// <exception cref="Persistence.PersistenceException">Changes to objects from multiple storage providers were made.</exception>
  /// <exception cref="Persistence.StorageProviderException">An error occurred while committing the changes to the datasource.</exception>
  public virtual void Commit ()
  {
    using (EnterScope ())
    {
      BeginCommit();
      DomainObjectCollection changedButNotDeletedDomainObjects = _dataManager.GetDomainObjects (new StateType[] {StateType.Changed, StateType.New});

      DataContainerCollection changedDataContainers = _dataManager.GetChangedDataContainersForCommit();
      if (changedDataContainers.Count > 0)
      {
        using (PersistenceManager persistenceManager = new PersistenceManager())
        {
          persistenceManager.Save (changedDataContainers);
        }
      }

      _dataManager.Commit();
      EndCommit (changedButNotDeletedDomainObjects);
    }
  }

  /// <summary>
  /// Performs a rollback of all changes within the <b>ClientTransaction</b>.
  /// </summary>
  public virtual void Rollback ()
  {
    using (EnterScope ())
    {
      BeginRollback();
      DomainObjectCollection changedButNotNewDomainObjects = _dataManager.GetDomainObjects (new StateType[] {StateType.Changed, StateType.Deleted});

      _dataManager.Rollback();

      EndRollback (changedButNotNewDomainObjects);
    }
  }

  /// <summary>
  /// Gets a <see cref="DomainObject"/> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> that should be loaded. Must not be <see langword="null"/>.</param>
  /// <returns>The <see cref="DomainObject"/> with the specified <paramref name="id"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  protected internal virtual DomainObject GetObject (ObjectID id)
  {
    using (EnterScope ())
    {
      return GetObject (id, false);
    }
  }

  /// <summary>
  /// Gets a <see cref="DomainObject"/> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> that should be loaded. Must not be <see langword="null"/>.</param>
  /// <param name="includeDeleted">Indicates if the method should return <see cref="DomainObject"/>s that are already deleted.</param>
  /// <returns>The <see cref="DomainObject"/> with the specified <paramref name="id"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
  /// <exception cref="DataManagement.ObjectDeletedException"><paramref name="includeDeleted"/> is false and the DomainObject with <paramref name="id"/> has been deleted.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  protected internal virtual DomainObject GetObject (ObjectID id, bool includeDeleted)
  {
    ArgumentUtility.CheckNotNull ("id", id);
    using (EnterScope ())
    {
      return _dataManager.DataContainerMap.GetObject (id, includeDeleted);
    }
  }

  internal DataContainer CreateNewDataContainer (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);
    using (EnterScope ())
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (type);

      using (PersistenceManager persistenceManager = new PersistenceManager())
      {
        DataContainer newDataContainer = persistenceManager.CreateNewDataContainer (classDefinition);
        SetClientTransaction (newDataContainer);
        _dataManager.RegisterNewDataContainer (newDataContainer);

        return newDataContainer;
      }
    }
  }

  /// <summary>
  /// Evaluates if any relations of the given <see cref="DomainObject"/> have changed since instantiation, loading, commit or rollback.
  /// </summary>
  /// <param name="domainObject">The <see cref="DomainObject"/> to evaluate. Must not be <see langword="null"/>.</param>
  /// <returns><see langword="true"/> if any relations have changed; otherwise, <see langword="false"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="domainObject"/> is <see langword="null"/>.</exception>
  internal protected virtual bool HasRelationChanged (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);

    using (EnterScope ())
    {
      return _dataManager.RelationEndPointMap.HasRelationChanged (domainObject.GetDataContainer());
    }
  }

  /// <summary>
  /// Gets the related object of a given <see cref="DataManagement.RelationEndPointID"/>.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> to evaluate. It must refer to a <see cref="DataManagement.ObjectEndPoint"/>. Must not be <see langword="null"/>.</param>
  /// <returns>The <see cref="DomainObject"/> that is the current related object.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointID"/> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentException"><paramref name="relationEndPointID"/> does not refer to an <see cref="DataManagement.ObjectEndPoint"/></exception>
  internal protected virtual DomainObject GetRelatedObject (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    using (EnterScope ())
    {
      DomainObject domainObject = GetObject (relationEndPointID.ObjectID, true);

      TransactionEventSink.RelationReading (domainObject, relationEndPointID.PropertyName, ValueAccess.Current);
      DomainObject relatedObject = _dataManager.RelationEndPointMap.GetRelatedObject (relationEndPointID);
      TransactionEventSink.RelationRead (domainObject, relationEndPointID.PropertyName, relatedObject, ValueAccess.Current);

      return relatedObject;
    }
  }

  /// <summary>
  /// Gets the original related object of a given <see cref="DataManagement.RelationEndPointID"/> at the point of instantiation, loading, commit or rollback.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> to evaluate. It must refer to a <see cref="DataManagement.ObjectEndPoint"/>. Must not be <see langword="null"/>.</param>
  /// <returns>The <see cref="DomainObject"/> that is the original related object.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointID"/> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentException"><paramref name="relationEndPointID"/> does not refer to an <see cref="DataManagement.ObjectEndPoint"/></exception>
  internal protected virtual DomainObject GetOriginalRelatedObject (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    using (EnterScope ())
    {
      DomainObject domainObject = GetObject (relationEndPointID.ObjectID, true);

      TransactionEventSink.RelationReading (domainObject, relationEndPointID.PropertyName, ValueAccess.Original);
      DomainObject relatedObject = _dataManager.RelationEndPointMap.GetOriginalRelatedObject (relationEndPointID);
      TransactionEventSink.RelationRead (domainObject, relationEndPointID.PropertyName, relatedObject, ValueAccess.Original);

      return relatedObject;
    }
  }

  /// <summary>
  /// Gets the related objects of a given <see cref="DataManagement.RelationEndPointID"/>.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> to evaluate. It must refer to a <see cref="DataManagement.CollectionEndPoint"/>. Must not be <see langword="null"/>.</param>
  /// <returns>A <see cref="DomainObjectCollection"/> containing the current related objects.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointID"/> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentException"><paramref name="relationEndPointID"/> does not refer to a <see cref="DataManagement.CollectionEndPoint"/></exception>
  internal protected virtual DomainObjectCollection GetRelatedObjects (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    using (EnterScope ())
    {
      DomainObject domainObject = GetObject (relationEndPointID.ObjectID, true);

      TransactionEventSink.RelationReading (domainObject, relationEndPointID.PropertyName, ValueAccess.Current);
      DomainObjectCollection relatedObjects = _dataManager.RelationEndPointMap.GetRelatedObjects (relationEndPointID);
      TransactionEventSink.RelationRead (domainObject, relationEndPointID.PropertyName, relatedObjects.Clone (true), ValueAccess.Current);

      return relatedObjects;
    }
  }

  /// <summary>
  /// Gets the original related objects of a given <see cref="DataManagement.RelationEndPointID"/> at the point of instantiation, loading, commit or rollback.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> to evaluate. It must refer to a <see cref="DataManagement.CollectionEndPoint"/>. Must not be <see langword="null"/>.</param>
  /// <returns>A <see cref="DomainObjectCollection"/> containing the original related objects.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointID"/> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentException"><paramref name="relationEndPointID"/> does not refer to a <see cref="DataManagement.CollectionEndPoint"/></exception>
  internal protected virtual DomainObjectCollection GetOriginalRelatedObjects (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    using (EnterScope ())
    {
      DomainObject domainObject = GetObject (relationEndPointID.ObjectID, true);

      TransactionEventSink.RelationReading (domainObject, relationEndPointID.PropertyName, ValueAccess.Original);
      DomainObjectCollection relatedObjects = _dataManager.RelationEndPointMap.GetOriginalRelatedObjects (relationEndPointID);
      TransactionEventSink.RelationRead (domainObject, relationEndPointID.PropertyName, relatedObjects, ValueAccess.Original);

      return relatedObjects;
    }
  }  

  /// <summary>
  /// Sets a relation between two relationEndPoints.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> referring the <see cref="DataManagement.RelationEndPoint"/> that should relate to <paramref name="newRelatedObject"/>. Must not be <see langword="null"/>.</param>
  /// <param name="newRelatedObject">The new <see cref="DomainObject"/> that should be related; <see langword="null"/> indicates that no object should be referenced.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointID"/> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentException">
  ///   <paramref name="relationEndPointID"/> does not refer to an <see cref="DataManagement.ObjectEndPoint"/><br /> -or- <br />
  ///   <paramref name="relationEndPointID"/> belongs to a <see cref="DomainObject"/> that has been deleted.<br /> -or- <br />
  ///   <paramref name="newRelatedObject"/> has been deleted.
  /// </exception>
  /// <exception cref="DataManagement.ClientTransactionsDifferException">
  ///   <paramref name="newRelatedObject"/> does belongs to a different <b>ClientTransaction</b>.
  /// </exception>
  internal protected virtual void SetRelatedObject (RelationEndPointID relationEndPointID, DomainObject newRelatedObject)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    using (EnterScope ())
    {
      _dataManager.RelationEndPointMap.SetRelatedObject (relationEndPointID, newRelatedObject);
    }
  }
  
  /// <summary>
  /// Deletes a <see cref="DomainObject"/>.
  /// </summary>
  /// <param name="domainObject">The <see cref="DomainObject"/> to delete. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="domainObject"/> is <see langword="null"/>.</exception>
  /// <exception cref="DataManagement.ClientTransactionsDifferException">
  ///   <paramref name="domainObject"/> belongs to a different <see cref="ClientTransaction"/>. 
  /// </exception>
  protected internal virtual void Delete (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    using (EnterScope ())
    {
      _dataManager.Delete (domainObject);
    }
  }

  /// <summary>
  /// Loads an object from the datasource.
  /// </summary>
  /// <remarks>
  /// This method raises the <see cref="Loaded"/> event.
  /// </remarks>
  /// <param name="id">An <see cref="ObjectID"/> object indicating which <see cref="DomainObject"/> to load. Must not be <see langword="null"/>.</param>
  /// <returns>The <see cref="DomainObject"/> object that was loaded.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  internal protected virtual DomainObject LoadObject (ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);
    using (EnterScope ())
    {
      DataContainer dataContainer = LoadDataContainer (id);

      DomainObjectCollection loadedDomainObjects = new DomainObjectCollection (new DomainObject[] {dataContainer.DomainObject}, true);
      OnLoaded (new ClientTransactionEventArgs (loadedDomainObjects));

      return dataContainer.DomainObject;
    }
  }

  /// <summary>
  /// Loads a <see cref="DataContainer"/> from the datasource for an existing <see cref="DomainObject"/>.
  /// </summary>
  /// <remarks>
  /// This method raises the <see cref="Loaded"/> event.
  /// </remarks>
  /// <param name="domainObject">The <see cref="DomainObject"/> to load the <see cref="DataContainer"/> for. Must not be <see langword="null"/>.</param>
  /// <returns>The <see cref="DataContainer"/> that was loaded.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="domainObject"/> is <see langword="null"/>.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="domainObject"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  internal protected virtual DataContainer LoadDataContainerForExistingObject (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    using (EnterScope ())
    {
      DataContainer dataContainer = LoadDataContainer (domainObject.ID);
      dataContainer.SetDomainObject (domainObject);

      DomainObjectCollection loadedDomainObjects = new DomainObjectCollection (new DomainObject[] {dataContainer.DomainObject}, true);
      OnLoaded (new ClientTransactionEventArgs (loadedDomainObjects));

      return dataContainer;
    }
  }

  private DataContainer LoadDataContainer (ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    using (PersistenceManager persistenceManager = new PersistenceManager ())
    {
      DataContainer dataContainer = persistenceManager.LoadDataContainer (id);
      TransactionEventSink.ObjectLoading (dataContainer.ID);
      SetClientTransaction (dataContainer);

      _dataManager.RegisterExistingDataContainer (dataContainer);
      return dataContainer;
    }
  }

  /// <summary>
  /// Loads the related <see cref="DomainObject"/> of a given <see cref="DataManagement.RelationEndPointID"/>.
  /// </summary>
  /// <remarks>
  /// This method raises the <see cref="Loaded"/> event.
  /// </remarks>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> that should be evaluated. <paramref name="relationEndPoint"/> must refer to a <see cref="ObjectEndPoint"/>. Must not be <see langword="null"/>.</param>
  /// <returns>The related <see cref="DomainObject"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointID"/> is <see langword="null"/>.</exception>
  /// <exception cref="System.InvalidCastException"><paramref name="relationEndPointID"/> does not refer to an <see cref="DataManagement.ObjectEndPoint"/></exception>
  /// <exception cref="DataManagement.ObjectDeletedException"><paramref name="includeDeleted"/> is false and the DomainObject with <paramref name="id"/> has been deleted.</exception>
  /// <exception cref="Persistence.PersistenceException">
  ///   The related object could not be loaded, but is mandatory.<br /> -or- <br />
  ///   The relation refers to non-existing object.<br /> -or- <br />
  ///   <paramref name="relationEndPointID"/> does not refer to an <see cref="DataManagement.ObjectEndPoint"/>.
  /// </exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  internal protected virtual DomainObject LoadRelatedObject (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    using (EnterScope ())
    {
      DomainObject domainObject = GetObject (relationEndPointID.ObjectID, false);

      using (PersistenceManager persistenceManager = new PersistenceManager())
      {
        DataContainer relatedDataContainer = persistenceManager.LoadRelatedDataContainer (domainObject.GetDataContainer(), relationEndPointID);
        if (relatedDataContainer != null)
        {
          TransactionEventSink.ObjectLoading (relatedDataContainer.ID);
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
  }

  /// <summary>
  /// Loads all related <see cref="DomainObject"/>s of a given <see cref="DataManagement.RelationEndPointID"/>. 
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> that should be evaluated. <paramref name="relationEndPoint"/> must refer to a <see cref="CollectionEndPoint"/>. Must not be <see langword="null"/>.</param>
  /// <returns>A <see cref="DomainObjectCollection"/> containing all related <see cref="DomainObject"/>s.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointID"/> is <see langword="null"/>.</exception>
  /// <exception cref="Persistence.PersistenceException">
  ///   <paramref name="relationEndPointID"/> does not refer to one-to-many relation.<br /> -or- <br />
  ///   The StorageProvider for the related objects could not be initialized.
  /// </exception>
  internal protected virtual DomainObjectCollection LoadRelatedObjects (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    using (EnterScope ())
    {
      using (PersistenceManager persistenceManager = new PersistenceManager())
      {
        DataContainerCollection relatedDataContainers = persistenceManager.LoadRelatedDataContainers (relationEndPointID);
        return MergeLoadedDomainObjects (relatedDataContainers, relationEndPointID);
      }
    }
  }

  /// <summary>
  /// Creates a new <see cref="DomainObjectCollection"/>, registers the <see cref="DataContainer"/>s with this <b>ClientTransaction</b>, discards already loaded <see cref="DataContainer"/>s, raises the <see cref="Loaded"/> event and optionally registers the relation with the specified <see cref="DataManagement.RelationEndPointID"/>.
  /// </summary>
  /// <param name="dataContainers">The newly loaded <see cref="DataContainer"/>s.</param>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> that should be evaluated. <paramref name="relationEndPoint"/> must refer to a <see cref="CollectionEndPoint"/>.</param>
  /// <returns>A <see cref="DomainObjectCollection"/>.</returns>
  /// <exception cref="System.InvalidCastException"><paramref name="collectionType"/> cannot be casted to <see cref="DomainObjectCollection"/>.</exception>
  internal DomainObjectCollection MergeLoadedDomainObjects (
      DataContainerCollection dataContainers, 
      RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    using (EnterScope ())
    {
      return MergeLoadedDomainObjects (
          dataContainers,
          relationEndPointID.Definition.PropertyType,
          relationEndPointID.OppositeEndPointDefinition.ClassDefinition.ClassType,
          relationEndPointID);
    }
  }

  /// <summary>
  /// Creates a new <see cref="DomainObjectCollection"/> with the specified <paramref name="collectionType"/>, registers the <see cref="DataContainer"/>s with this <b>ClientTransaction</b>, discards already loaded <see cref="DataContainer"/>s and raises the <see cref="Loaded"/> event.
  /// </summary>
  /// <param name="dataContainers">The newly loaded <see cref="DataContainer"/>s.</param>
  /// <param name="collectionType">The <see cref="Type"/> of the new collection that should be instantiated.</param>
  /// <returns>A <see cref="DomainObjectCollection"/>.</returns>
  /// <exception cref="System.InvalidCastException"><paramref name="collectionType"/> cannot be casted to <see cref="DomainObjectCollection"/>.</exception>
  internal DomainObjectCollection MergeLoadedDomainObjects (
      DataContainerCollection dataContainers, 
      Type collectionType)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);
    ArgumentUtility.CheckNotNull ("collectionType", collectionType);
    using (EnterScope ())
    {
      return MergeLoadedDomainObjects (dataContainers, collectionType, null, null);
    }
  }

  /// <summary>
  /// Creates a new <see cref="DomainObjectCollection"/> with the specified <paramref name="collectionType"/>, registers the <see cref="DataContainer"/>s with this <b>ClientTransaction</b>, discards already loaded <see cref="DataContainer"/>s, raises the <see cref="Loaded"/> event and optionally registers the relation with the specified <see cref="DataManagement.RelationEndPointID"/>.
  /// </summary>
  /// <param name="dataContainers">The newly loaded <see cref="DataContainer"/>s.</param>
  /// <param name="collectionType">The <see cref="Type"/> of the new collection that should be instantiated.</param>
  /// <param name="requiredItemType">The permitted <see cref="Type"/> of an item in the <see cref="DomainObjectCollection"/>. If specified only this type or derived types can be added to the <b>DomainObjectCollection</b>.</param>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> that should be evaluated.</param>
  /// <returns>A <see cref="DomainObjectCollection"/>.</returns>
  /// <exception cref="System.InvalidCastException"><paramref name="collectionType"/> cannot be casted to <see cref="DomainObjectCollection"/>.</exception>
  internal DomainObjectCollection MergeLoadedDomainObjects (
      DataContainerCollection dataContainers, 
      Type collectionType,
      Type requiredItemType,
      RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);
    ArgumentUtility.CheckNotNull ("collectionType", collectionType);

    using (EnterScope ())
    {
      DataContainerCollection newLoadedDataContainers = _dataManager.DataContainerMap.GetNotRegisteredDataContainers (dataContainers);
      NotifyOfLoading (newLoadedDataContainers);
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
  }
      
  /// <summary>
  /// Sets the ClientTransaction property of all <see cref="DataContainer"/>s of a given <see cref="DataManagement.DataContainerCollection"/>.
  /// </summary>
  /// <param name="dataContainers">The <see cref="DataContainerCollection"/> with all the <see cref="DataContainer"/> objects that should be set to the <b>ClientTransaction</b>. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="dataContainers"/> is <see langword="null"/>.</exception>
  protected void SetClientTransaction (DataContainerCollection dataContainers)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);
    using (EnterScope ())
    {
      foreach (DataContainer dataContainer in dataContainers)
        SetClientTransaction (dataContainer);
    }
  }

  private void NotifyOfLoading (DataContainerCollection loadedDataContainers)
  {
    foreach (DataContainer dataContainer in loadedDataContainers)
      TransactionEventSink.ObjectLoading (dataContainer.ID);
  }

  /// <summary>
  /// Sets the ClientTransaction property of a given <see cref="DataContainer"/>
  /// </summary>
  /// <param name="dataContainer">The <see cref="DataContainer"/> that should be set to the <b>ClientTransaction</b>. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="dataContainer"/> is <see langword="null"/>.</exception>
  protected void SetClientTransaction (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
    using (EnterScope ())
    {
      dataContainer.SetClientTransaction (this);
    }
  }

  /// <summary>
  /// Raises the <see cref="Loaded"/> event.
  /// </summary>
  /// <param name="args">A <see cref="ClientTransactionEventArgs"/> object that contains the event data.</param>
  protected virtual void OnLoaded (ClientTransactionEventArgs args)
  {
    using (EnterScope ())
    {
      foreach (DomainObject loadedDomainObject in args.DomainObjects)
        loadedDomainObject.EndObjectLoading();

      if (args.DomainObjects.Count != 0)
      {
        TransactionEventSink.ObjectsLoaded (args.DomainObjects);

        if (Loaded != null)
          Loaded (this, args);
      }
    }
  }

  /// <summary>
  /// Raises the <see cref="Committing"/> event.
  /// </summary>
  /// <param name="args">A <see cref="ClientTransactionEventArgs"/> object that contains the event data.</param>
  protected virtual void OnCommitting (ClientTransactionEventArgs args)
  {
    using (EnterScope ())
    {
      TransactionEventSink.TransactionCommitting (args.DomainObjects);

      if (Committing != null)
        Committing (this, args);
    }
  }


  /// <summary>
  /// Raises the <see cref="Committed"/> event.
  /// </summary>
  /// <param name="args">A <see cref="ClientTransactionEventArgs"/> object that contains the event data.</param>
  protected virtual void OnCommitted (ClientTransactionEventArgs args)
  {
    using (EnterScope ())
    {
      if (Committed != null)
        Committed (this, args);

      TransactionEventSink.TransactionCommitted (args.DomainObjects);
    }
  }

  /// <summary>
  /// Raises the <see cref="RollingBack"/> event.
  /// </summary>
  /// <param name="args">A <see cref="ClientTransactionEventArgs"/> object that contains the event data.</param>
  protected virtual void OnRollingBack (ClientTransactionEventArgs args)
  {
    using (EnterScope ())
    {
      TransactionEventSink.TransactionRollingBack (args.DomainObjects);

      if (RollingBack != null)
        RollingBack (this, args);
    }
  }

  /// <summary>
  /// Raises the <see cref="RolledBack"/> event.
  /// </summary>
  /// <param name="args">A <see cref="ClientTransactionEventArgs"/> object that contains the event data.</param>
  protected virtual void OnRolledBack (ClientTransactionEventArgs args)
  {
    using (EnterScope ())
    {
      if (RolledBack != null)
        RolledBack (this, args);

      TransactionEventSink.TransactionRolledBack (args.DomainObjects);
    }
  }

  /// <summary>
  /// Gets the <see cref="DataManager"/> of the <b>ClientTransaction</b>.
  /// </summary>
  protected internal DataManager DataManager
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
      using (EnterScope ())
      {
        if (_queryManager == null)
          _queryManager = new QueryManager (this);

        return _queryManager;
      }
    }
  }

  /// <summary>
  /// Gets a <see cref="System.Collections.Generic.Dictionary {TKey, TValue}"/> to store application specific objects 
  /// within the <see cref="ClientTransaction"/> which have the same lifetime.<br/>
  /// To store and access values create project specific <see cref="System.Enum"/>(s) which ensure namespace separation of keys in the dictionary.
  /// </summary>
  public Dictionary<Enum, object> ApplicationData
  {
    get { return _applicationData; }
  }

  private void BeginCommit ()
  {
    // TODO Doc: ES
    
    // Note regarding to Committing: 
    // Every object raises a Committing event even if another object's Committing event changes the first object's state back to original 
    // during its own Committing event. Because the event order of .NET is not deterministic, this behavior is desired to ensure consistency: 
    // Every object changed during a ClientTransaction raises a Committing event regardless of the Committing event order of specific objects.  
    // But: The same object is not included in the ClientTransaction's Committing event, because this order (DomainObject Committing events are raised
    // before the ClientTransaction Committing events) IS deterministic.
    
    // Note regarding to Committed: 
    // If an object is changed back to its original state during the Committing phase, no Committed event will be raised,
    // because in this case the object won't be committed to the underlying backend (e.g. database).

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

  private void BeginRollback ()
  {
    // TODO Doc: ES

    // Note regarding to RollingBack: 
    // Every object raises a RollingBack event even if another object's RollingBack event changes the first object's state back to original 
    // during its own RollingBack event. Because the event order of .NET is not deterministic, this behavior is desired to ensure consistency: 
    // Every object changed during a ClientTransaction raises a RollingBack event regardless of the RollingBack event order of specific objects.  
    // But: The same object is not included in the ClientTransaction's RollingBack event, because this order (DomainObject RollingBack events are raised
    // before the ClientTransaction RollingBack events) IS deterministic.

    // Note regarding to RolledBack: 
    // If an object is changed back to its original state during the RollingBack phase, no RolledBack event will be raised,
    // because the object actually has never been changed from a ClientTransaction's perspective.

    DomainObjectCollection changedDomainObjects = _dataManager.GetChangedDomainObjects ();
    DomainObjectCollection domainObjectRollingBackEventRaised = new DomainObjectCollection ();
    DomainObjectCollection clientTransactionRollingBackEventRaised = new DomainObjectCollection ();

    DomainObjectCollection clientTransactionRollingBackEventNotRaised = changedDomainObjects;
    do
    {
      DomainObjectCollection domainObjectRollingBackEventNotRaised = domainObjectRollingBackEventRaised.GetItemsNotInCollection (changedDomainObjects);
      while (domainObjectRollingBackEventNotRaised.Count > 0)
      {
        foreach (DomainObject domainObject in domainObjectRollingBackEventNotRaised)
        {
          if (!domainObject.IsDiscarded)
          {
            domainObject.BeginRollback ();

            if (!domainObject.IsDiscarded)
              domainObjectRollingBackEventRaised.Add (domainObject);
          }
        }

        changedDomainObjects = _dataManager.GetChangedDomainObjects ();
        domainObjectRollingBackEventNotRaised = domainObjectRollingBackEventRaised.GetItemsNotInCollection (changedDomainObjects);
      }

      clientTransactionRollingBackEventNotRaised = clientTransactionRollingBackEventRaised.GetItemsNotInCollection (changedDomainObjects);

      OnRollingBack (new ClientTransactionEventArgs (clientTransactionRollingBackEventNotRaised.Clone (true)));
      foreach (DomainObject domainObject in clientTransactionRollingBackEventNotRaised)
      {
        if (!domainObject.IsDiscarded)
          clientTransactionRollingBackEventRaised.Add (domainObject);
      }

      changedDomainObjects = _dataManager.GetChangedDomainObjects ();
      clientTransactionRollingBackEventNotRaised = clientTransactionRollingBackEventRaised.GetItemsNotInCollection (changedDomainObjects);
    } while (clientTransactionRollingBackEventNotRaised.Count > 0);
  }

  private void EndRollback (DomainObjectCollection changedDomainObjects)
  {
    foreach (DomainObject changedDomainObject in changedDomainObjects)
      changedDomainObject.EndRollback ();

    OnRolledBack (new ClientTransactionEventArgs (changedDomainObjects.Clone (true)));
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