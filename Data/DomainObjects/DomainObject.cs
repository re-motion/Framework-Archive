using System;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects
{
/// <summary>
/// Base class for all objects that are persisted by the framework.
/// </summary>
public class DomainObject
{
  // types

  // static members and constants

  /// <summary>
  /// Gets a <b>DomainObject</b> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <b>DomainObject</b> that should be loaded.</param>
  /// <returns>The <b>DomainObject</b> with the specified <i>id</i>.</returns>
  /// <exception cref="System.ArgumentNullException"><i>id</i> is a null reference.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <i>id</i>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  protected static DomainObject GetObject (ObjectID id)
  {
    return GetObject (id, false);
  }

  /// <summary>
  /// Gets a <b>DomainObject</b> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <b>DomainObject</b> that should be loaded.</param>
  /// <param name="includeDeleted">Indicates if the method should return <b>DomainObject</b>s that are already deleted.</param>
  /// <returns>The <b>DomainObject</b> with the specified <i>id</i>.</returns>
  /// <exception cref="System.ArgumentNullException"><i>id</i> is a null reference.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <i>id</i>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  protected static DomainObject GetObject (ObjectID id, bool includeDeleted)
  {
    return GetObject (id, ClientTransaction.Current, includeDeleted);
  }

  /// <summary>
  /// Gets a <b>DomainObject</b> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <b>DomainObject</b> that is loaded.</param>
  /// <param name="clientTransaction">The <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/> that is used to load the <b>DomainObject</b>.</param>
  /// <returns>The <b>DomainObject</b> with the specified <i>id</i>.</returns>
  /// <exception cref="System.ArgumentNullException"><i>id</i> or <i>clientTransaction</i>is a null reference.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <i>id</i>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  protected static DomainObject GetObject (ObjectID id, ClientTransaction clientTransaction)
  {
    return GetObject (id, clientTransaction, false);
  }
 
  /// <summary>
  /// Gets a <b>DomainObject</b> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <b>DomainObject</b> that is loaded.</param>
  /// <param name="clientTransaction">The <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/> that us used to load the <b>DomainObject</b>.</param>
  /// <param name="includeDeleted">Indicates if the method should return <b>DomainObject</b>s that are already deleted.</param>
  /// <returns>The <b>DomainObject</b> with the specified <i>id</i>.</returns>
  /// <exception cref="System.ArgumentNullException"><i>id</i> or <i>clientTransaction</i>is a null reference.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <i>id</i>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  protected static DomainObject GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
  {
    ArgumentUtility.CheckNotNull ("id", id);
    ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

    return clientTransaction.GetObject (id, includeDeleted);
  }

  internal static DomainObject Create (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    return (DomainObject) ReflectionUtility.CreateObject (dataContainer.DomainObjectType, dataContainer);
  }

  // member fields

  /// <summary>
  /// Occurs before a <see cref="PropertyValue"/> of the <b>DomainObject</b> is changed.
  /// </summary>
  /// <remarks>
  /// This event does not fire when a <see cref="PropertyValue"/> has been changed due to a relation change.
  /// </remarks>
  public event PropertyChangingEventHandler PropertyChanging;

  /// <summary>
  /// Occurs after a <see cref="PropertyValue"/> of the <b>DomainObject</b> is changed.
  /// </summary>
  /// <remarks>
  /// This event does not fire when a <see cref="PropertyValue"/> has been changed due to a relation change.
  /// </remarks>
  public event PropertyChangedEventHandler PropertyChanged;

  /// <summary>
  /// Occurs before a Relation of the <b>DomainObject</b> is changed.
  /// </summary>
  public event RelationChangingEventHandler RelationChanging;

  /// <summary>
  /// Occurs after a Relation of the <b>DomainObject</b> has been changed.
  /// </summary>
  public event RelationChangedEventHandler RelationChanged;

  /// <summary>
  /// Occurs before the <b>DomainObject</b> is deleted.
  /// </summary>
  public event EventHandler Deleting;

  /// <summary>
  /// Occurs before the changes of a <b>DomainObject</b> are committed.
  /// </summary>
  public event EventHandler Committing;

  /// <summary>
  /// Occurs after the changes of a <b>DomainObject</b> are successfully committed.
  /// </summary>
  public event EventHandler Committed;

  /// <summary>
  /// Occurs after the <b>DomainObject</b> has been deleted.
  /// </summary>
  public event EventHandler Deleted;

  private DataContainer _dataContainer;

  // construction and disposing

  /// <summary>
  /// Initializes a new <b>DomainObject</b>.
  /// </summary>
  protected DomainObject () : this (ClientTransaction.Current)
  {
  }

  /// <summary>
  /// Initializes a new <b>DomainObject</b>.
  /// </summary>
  /// <param name="clientTransaction">The <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/> the <b>DomainObject</b> should be part of.</param>
  /// <exception cref="System.ArgumentNullException"><i>clientTransaction</i> is a null reference.</exception>
  protected DomainObject (ClientTransaction clientTransaction)
  {
    ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

    _dataContainer = clientTransaction.CreateNewDataContainer (this.GetType ());
    _dataContainer.SetDomainObject (this);
    RegisterDataContainerEvents ();
  }

  /// <summary>
  /// Infrastructure constructor necessary to load a <b>DomainObject</b> from a datasource.
  /// </summary>
  /// <remarks>
  /// All derived classes have to implement an (empty) constructor with this signature.
  /// Do not implement any initialization logic in this constructor, but use <see cref="DomainObject.OnLoaded"/> instead.
  /// </remarks>
  /// <param name="dataContainer"></param>
  protected DomainObject (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    _dataContainer = dataContainer;
    RegisterDataContainerEvents ();
  }

  // methods and properties

  /// <summary>
  /// Gets the <see cref="ObjectID"/> of the <b>DomainObject</b>.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public ObjectID ID
  {
    get 
    {
      CheckDiscarded ();
      return _dataContainer.ID; 
    }
  }

  /// <summary>
  /// Gets the current state of the <b>DomainObject</b>.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public StateType State
  {
    get
    {
      CheckDiscarded ();
      if (_dataContainer.State == StateType.Unchanged)
      {
        if (ClientTransaction.HasRelationChanged (this))
          return StateType.Changed;
        else
          return StateType.Unchanged;
      }

      return _dataContainer.State;
    }
  }

  /// <summary>
  /// Gets the <see cref="DataContainer"/> of the <b>DomainObject</b>.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  protected internal DataContainer DataContainer
  {
    get 
    { 
      CheckDiscarded ();
      return _dataContainer; 
    }
  }

  /// <summary>
  /// Deletes the <b>DomainObject</b>.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <remarks>To perform custom actions when a <see cref="DomainObject"/> is deleted <see cref="OnDeleting"/> and <see cref="OnDeleted"/> should be overridden.</remarks>
  public void Delete ()
  {
    CheckDiscarded ();
    ClientTransaction.Delete (this);

    if (IsDiscarded)
    {
      _dataContainer.PropertyChanging -= new PropertyChangingEventHandler (DataContainer_PropertyChanging);
      _dataContainer.PropertyChanged -= new PropertyChangedEventHandler (DataContainer_PropertyChanged);
    }
  }

  /// <summary>
  /// Gets the related object of a given <i>propertyName</i>.
  /// </summary>
  /// <param name="propertyName">The name of the property referring to the relation. <i>propertyName</i> must refer to a one-to-one or many-to-one relation.</param>
  /// <returns>The <b>DomainObject</b> that is the current related object.</returns>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.ArgumentException"><i>propertyName</i> does not refer to an one-to-one or many-to-one relation.</exception>
  protected virtual DomainObject GetRelatedObject (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return ClientTransaction.GetRelatedObject (new RelationEndPointID (ID, propertyName));
  }

  /// <summary>
  /// Gets the original related object of a given <i>propertyName</i> at the point of instantiation, loading, commit or rollback.
  /// </summary>
  /// <param name="propertyName">The name of the property referring to the relation. <i>propertyName</i> must refer to a one-to-many relation.</param>
  /// <returns>The <b>DomainObject</b> that is the current related object.</returns>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.InvalidCastException"><i>propertyName</i> does not refer to an <see cref="DataManagement.ObjectEndPoint"/>.</exception>
  /// <exception cref="System.ArgumentException"><i>propertyName</i> does not refer to an one-to-one or many-to-one relation.</exception>
  protected virtual DomainObject GetOriginalRelatedObject (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return ClientTransaction.GetOriginalRelatedObject (new RelationEndPointID (ID, propertyName));
  }

  /// <summary>
  /// Gets the related objects of a given <i>propertyName</i>.
  /// </summary>
  /// <param name="propertyName">The name of the property referring to the relation. <i>propertyName</i> must refer to a one-to-many relation.</param>
  /// <returns>A <see cref="DomainObjectCollection"/> containing the current related objects.</returns>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.InvalidCastException"><i>propertyName</i> does not refer to an <see cref="DataManagement.ObjectEndPoint"/>.</exception>
  /// <exception cref="System.ArgumentException"><i>propertyName</i> does not refer to an one-to-many relation.</exception>
  protected virtual DomainObjectCollection GetRelatedObjects (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return ClientTransaction.GetRelatedObjects (new RelationEndPointID (ID, propertyName));
  }

  /// <summary>
  /// Gets the original related objects of a given <i>propertyName</i> at the point of instantiation, loading, commit or rollback.
  /// </summary>
  /// <param name="propertyName">The name of the property referring to the relation. <i>propertyName</i> must refer to a one-to-many relation.</param>
  /// <returns>A <see cref="DomainObjectCollection"/> containing the original related objects.</returns>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.ArgumentException"><i>propertyName</i> does not refer to an one-to-many relation.</exception>
  protected virtual DomainObjectCollection GetOriginalRelatedObjects (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return ClientTransaction.GetOriginalRelatedObjects (new RelationEndPointID (ID, propertyName));
  }

  /// <summary>
  /// Sets a relation to another object.
  /// </summary>
  /// <param name="propertyName">The name of the property referring to the relation, that should relate to <i>newRelatedObject</i>.</param>
  /// <param name="newRelatedObject">The new <b>DomainObject</b> that should be related; null indicates that no object should be referenced.</param>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  protected void SetRelatedObject (string propertyName, DomainObject newRelatedObject)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    ClientTransaction.SetRelatedObject (new RelationEndPointID (ID, propertyName), newRelatedObject);
  }

  /// <summary>
  /// Gets a value indicating the discarded status of the object.
  /// </summary>
  /// <remarks>
  /// For more information why and when an object is discarded see <see cref="Rubicon.Data.DomainObjects.DataManagement.ObjectDiscardedException"/>.
  /// </remarks>
  public bool IsDiscarded 
  {
    get { return _dataContainer.IsDiscarded; }
  }

  // TODO Doc:
  public ClientTransaction ClientTransaction
  {
    get { return _dataContainer.ClientTransaction; }
  }

  /// <summary>
  /// Method is invoked after the loading process of the object is completed.
  /// </summary>
  /// <remarks>
  /// Override this method to initialize <b>DomainObject</b>s that are loaded from the datasource.
  /// </remarks>
  protected virtual void OnLoaded ()
  {
  }

  /// <summary>
  /// Raises the <see cref="Committing"/> event.
  /// </summary>
  /// <param name="args">A <see cref="System.EventArgs"/> object that contains the event data.</param>
  protected virtual void OnCommitting (EventArgs args)
  {
    if (Committing != null)
      Committing (this, args);
  }

  /// <summary>
  /// Raises the <see cref="Committed"/> event.
  /// </summary>
  /// <param name="args">A <see cref="System.EventArgs"/> object that contains the event data.</param>
  protected virtual void OnCommitted (EventArgs args)
  {
    if (Committed != null)
      Committed (this, args);
  }

  /// <summary>
  /// Raises the <see cref="RelationChanging"/> event.
  /// </summary>
  /// <param name="args">A <see cref="RelationChangingEventArgs"/> object that contains the event data.</param>
  protected virtual void OnRelationChanging (RelationChangingEventArgs args)
  {
    if (RelationChanging != null)
      RelationChanging (this, args);
  }

  /// <summary>
  /// Raises the <see cref="RelationChanged"/> event.
  /// </summary>
  /// <param name="args">A <see cref="RelationChangedEventArgs"/> object that contains the event data.</param>
  protected virtual void OnRelationChanged (RelationChangedEventArgs args)
  {
    if (RelationChanged != null)
      RelationChanged (this, args);
  }

  /// <summary>
  /// Raises the <see cref="PropertyChanging"/> event.
  /// </summary>
  /// <param name="args">A <see cref="PropertyChangingEventArgs"/> object that contains the event data.</param>
  protected virtual void OnPropertyChanging (PropertyChangingEventArgs args)
  {
    if (PropertyChanging != null)
      PropertyChanging (this, args);
  }

  /// <summary>
  /// Raises the <see cref="PropertyChanged"/> event.
  /// </summary>
  /// <param name="args">A <see cref="PropertyChangedEventArgs"/> object that contains the event data.</param>
  protected virtual void OnPropertyChanged (PropertyChangedEventArgs args)
  {
    if (PropertyChanged != null)
      PropertyChanged (this, args);
  }

  /// <summary>
  /// Raises the <see cref="Deleting"/> event.
  /// </summary>
  /// <param name="args">A <see cref="System.EventArgs"/> object that contains the event data.</param>
  protected virtual void OnDeleting (EventArgs args)
  {
    if (Deleting != null)
      Deleting (this, args);
  }

  /// <summary>
  /// Raises the <see cref="Deleted"/> event.
  /// </summary>
  /// <param name="args">A <see cref="EventArgs"/> object that contains the event data.</param>
  protected virtual void OnDeleted (EventArgs args)
  {
    if (Deleted != null)
      Deleted (this, args);
  }

  internal void BeginRelationChange (
    string propertyName,
    DomainObject oldRelatedObject,
    DomainObject newRelatedObject)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    RelationChangingEventArgs args = new RelationChangingEventArgs (propertyName, oldRelatedObject, newRelatedObject);
    OnRelationChanging (args);
  }

  internal void EndObjectLoading ()
  {
    OnLoaded ();
  }

  internal void EndRelationChange (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    OnRelationChanged (new RelationChangedEventArgs (propertyName));
  }

  internal void BeginDelete ()
  {
    OnDeleting (new EventArgs ());
  }

  internal void EndDelete ()
  {
    OnDeleted (new EventArgs ());
  }

  internal void BeginCommit ()
  {
    OnCommitting (new EventArgs ());
  }

  internal void EndCommit ()
  {
    OnCommitted (new EventArgs ());
  }

  private void DataContainer_PropertyChanging (object sender, PropertyChangingEventArgs args)
  {
    OnPropertyChanging (args);
  }

  private void DataContainer_PropertyChanged (object sender, PropertyChangedEventArgs args)
  {
    OnPropertyChanged (args);
  }

  private void RegisterDataContainerEvents ()
  {
    _dataContainer.PropertyChanging += new PropertyChangingEventHandler (DataContainer_PropertyChanging);
    _dataContainer.PropertyChanged += new PropertyChangedEventHandler (DataContainer_PropertyChanged);
  }

  private void CheckDiscarded ()
  {
    if (IsDiscarded)
      throw new ObjectDiscardedException (_dataContainer.GetID ());
  }
}
}