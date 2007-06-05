using System;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects
{
/// <summary>
/// Represents a container for the persisted properties of a DomainObject.
/// </summary>
[Serializable]
public class DataContainer
{
  // types

  private enum DataContainerStateType
  {
    Existing = 0,
    New = 1,
    Deleted = 2
  }

  // static members and constants

  /// <summary>
  /// Creates a <see cref="DataContainer"/> for a new <see cref="Rubicon.Data.DomainObjects.DomainObject"/>.
  /// </summary>
  /// <remarks>
  /// The new <see cref="DataContainer"/> has a <see cref="State"/> of <see cref="StateType.New"/>. All <see cref="PropertyValue"/>s for the class specified by <see cref="ObjectID.ClassID"/> are created.
  /// </remarks>
  /// <param name="id">The <see cref="ObjectID"/> of the new <see cref="DataContainer"/> to create. Must not be <see langword="null"/>.</param>
  /// <returns>The new <see cref="DataContainer"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
  public static DataContainer CreateNew (ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    DataContainer newDataContainer = new DataContainer (id);
    newDataContainer._state = DataContainerStateType.New;
    
    foreach (PropertyDefinition propertyDefinition in newDataContainer.ClassDefinition.GetPropertyDefinitions ())
      newDataContainer.PropertyValues.Add (new PropertyValue (propertyDefinition));

    return newDataContainer;
  }

  /// <summary>
  /// Creates a <see cref="DataContainer"/> for an existing <see cref="Rubicon.Data.DomainObjects.DomainObject"/>.
  /// </summary>
  /// <remarks>
  /// The new <see cref="DataContainer"/> has a <see cref="State"/> of <see cref="StateType.Unchanged"/>. All <see cref="PropertyValue"/>s for the class specified by <see cref="ObjectID.ClassID"/> are created.
  /// </remarks>
  /// <param name="id">The <see cref="ObjectID"/> of the new <see cref="DataContainer"/> to create. Must not be <see langword="null"/>.</param>
  /// <param name="timestamp">The timestamp value of the existing object in the datasource.</param>
  /// <returns>The new <see cref="DataContainer"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
  /// <exception cref="Mapping.MappingException">ClassDefinition of <paramref name="id"/> does not exist in mapping.</exception>
  public static DataContainer CreateForExisting (ObjectID id, object timestamp)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    DataContainer dataContainer = new DataContainer (id, timestamp);
    dataContainer._state = DataContainerStateType.Existing;
    return dataContainer;
  }

  // member fields

  /// <summary>
  /// Occurs before a <see cref="PropertyValue"/> is changed.
  /// </summary>
  /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
  public event PropertyChangeEventHandler PropertyChanging;
  /// <summary>
  /// Occurs after a <see cref="PropertyValue"/> is changed.
  /// </summary>
  /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
  public event PropertyChangeEventHandler PropertyChanged;

  private ClientTransaction _clientTransaction;
  private ObjectID _id;
  private object _timestamp;
  private PropertyValueCollection _propertyValues;
  private DataContainerStateType _state;
  private DomainObject _domainObject;
  private ClassDefinition _classDefinition;
  private RelationEndPointID[] _relationEndPointIDs = null;
  private bool _isDiscarded = false;
  
  // construction and disposing

  private DataContainer (ObjectID id) : this (id, null)
  {
  }

  private DataContainer (ObjectID id, object timestamp)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    _id = id;
    _classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (id.ClassID);
    _timestamp = timestamp;

    _propertyValues = new PropertyValueCollection ();
    _propertyValues.RegisterForChangeNotification (this);
  }

  // methods and properties

  /// <summary>
  /// Gets the value of the <see cref="PropertyValue"/> specified by <paramref name="propertyName"/>.
  /// </summary>
  /// <param name="propertyName">The name of the property. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><paramref name="propertyName"/> is an empty string.</exception>
  /// <exception cref="System.ArgumentException">The given <paramref name="propertyName"/> does not exist in the data container.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public object this[string propertyName]
  {
    get 
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      CheckDiscarded ();

      return _propertyValues[propertyName].Value; 
    }
    set 
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      CheckDiscarded ();

      _propertyValues[propertyName].Value = value; 
    }
  }

  /// <summary>
  /// Gets the value of the <see cref="PropertyValue"/> specified by <paramref name="propertyName"/>.
  /// </summary>
  /// <param name="propertyName">The name of the <see cref="PropertyValue"/>. Must not be <see langword="null"/>.</param>
  /// <returns>The value of the <see cref="PropertyValue"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><paramref name="propertyName"/> is an empty string.</exception>
  /// <exception cref="System.ArgumentException">The given <paramref name="propertyName"/> does not exist in the data container.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public object GetValue (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return this[propertyName];
  }

  /// <summary>
  /// Sets the value of the <see cref="PropertyValue"/> specified by <paramref name="propertyName"/>.
  /// </summary>
  /// <param name="propertyName">The name of the <see cref="PropertyValue"/>. Must not be <see langword="null"/>.</param>
  /// <param name="value">The value the <see cref="PropertyValue"/> is set to.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><paramref name="propertyName"/> is an empty string.</exception>
  /// <exception cref="System.ArgumentException">The given <paramref name="propertyName"/> does not exist in the data container.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public void SetValue (string propertyName, object value)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    this[propertyName] = value;
  }


  /// <summary>
  /// Gets the <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/> which the <see cref="DataContainer"/> is part of.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public ClientTransaction ClientTransaction 
  {
    get 
    {
      CheckDiscarded ();

      if (_clientTransaction == null)
        throw new DomainObjectException ("Internal error: ClientTransaction of DataContainer is not set.");

      return _clientTransaction;
    }
  }
  
  /// <summary>
  /// Gets the <see cref="Rubicon.Data.DomainObjects.DomainObject"/> associated with the <see cref="DataContainer"/>.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public DomainObject DomainObject
  {
    get 
    {
      CheckDiscarded ();

      if (_domainObject == null)
        _domainObject = DomainObject.CreateWithDataContainer (this);

      return _domainObject; 
    }
  }

  /// <summary>
  /// Gets the <see cref="ObjectID"/> of the <see cref="DataContainer"/>.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public ObjectID ID
  {
    get 
    { 
      CheckDiscarded ();
      return _id; 
    }
  }

  /// <summary>
  /// Gets the <see cref="Mapping.ClassDefinition"/> of the <see cref="DataContainer"/>.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public ClassDefinition ClassDefinition
  {
    get 
    { 
      CheckDiscarded ();
      return _classDefinition; 
    }
  }

  /// <summary>
  /// Gets the <see cref="Type"/> of the <see cref="Rubicon.Data.DomainObjects.DomainObject"/> of the <see cref="DataContainer"/>.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public Type DomainObjectType
  {
    get 
    { 
      CheckDiscarded ();
      return _classDefinition.ClassType; 
    }
  }


  /// <summary>
  /// Gets the <see cref="PropertyValueCollection"/> of all <see cref="PropertyValue"/>s that are part of the <see cref="DataContainer"/>.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public PropertyValueCollection PropertyValues
  {
    get 
    {
      CheckDiscarded ();
      return _propertyValues; 
    }
  }


  /// <summary>
  /// Gets the state of the <see cref="DataContainer"/>.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public StateType State
  {
    get 
    { 
      CheckDiscarded ();

      if (_state == DataContainerStateType.Existing)
        return GetStateForPropertyValues ();

      if (_state == DataContainerStateType.New) 
        return StateType.New;
      
      return StateType.Deleted;
    }
  }

  /// <summary>
  /// Gets the timestamp of the last committed change of the data in the <see cref="DataContainer"/>.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public object Timestamp
  {
    get 
    {
      CheckDiscarded ();
      return _timestamp; 
    }
  }

  /// <summary>
  /// Gets a value indicating the discarded status of the <see cref="DataContainer"/>.
  /// </summary>
  /// <remarks>
  /// For more information why and when a <see cref="DataContainer"/> is discarded see <see cref="Rubicon.Data.DomainObjects.DataManagement.ObjectDiscardedException"/>.
  /// </remarks>
  public bool IsDiscarded
  {
    get { return _isDiscarded; }
  }

  internal RelationEndPointID[] RelationEndPointIDs
  {
    get
    {
      if (_relationEndPointIDs == null)
        _relationEndPointIDs = RelationEndPointID.GetAllRelationEndPointIDs (this);

      return _relationEndPointIDs;
    }
  }

  internal void SetClientTransaction (ClientTransaction clientTransaction)
  {
    ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
    _clientTransaction = clientTransaction;
  }


  /// <summary>
  /// Raises the <see cref="PropertyChanging"/> event.
  /// </summary>
  /// <param name="args">A <see cref="PropertyChangeEventArgs"/> object that contains the event data.</param>
  protected virtual void OnPropertyChanging (PropertyChangeEventArgs args)
  {
    if (PropertyChanging != null)
      PropertyChanging (this, args);
  }

  /// <summary>
  /// Raises the <see cref="PropertyChanged"/> event.
  /// </summary>
  /// <param name="args">A <see cref="PropertyChangeEventArgs"/> object that contains the event data.</param>
  protected virtual void OnPropertyChanged (PropertyChangeEventArgs args)
  {
    if (PropertyChanged != null)
      PropertyChanged (this, args);
  }


  internal ObjectID GetID ()
  {
    return _id;
  }

  internal void Delete ()
  {
    if (_state == DataContainerStateType.New)
      Discard ();

    _state = DataContainerStateType.Deleted;
  }

  internal void SetTimestamp (object timestamp)
  {
    ArgumentUtility.CheckNotNull ("timestamp", timestamp);

    _timestamp = timestamp;
  }

  internal void Commit ()
  {
    if (_state == DataContainerStateType.Deleted)
    {
      Discard ();
    }
    else
    {
      foreach (PropertyValue propertyValue in _propertyValues)
        propertyValue.Commit ();

      _state = DataContainerStateType.Existing;
    }
  }

  internal void Rollback ()
  {
    if (_state == DataContainerStateType.New)
    {
      Discard ();
    }
    else
    {
      foreach (PropertyValue propertyValue in _propertyValues)
        propertyValue.Rollback ();
 
      _state = DataContainerStateType.Existing;
    }
  }

  internal void SetDomainObject (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    
    _domainObject = domainObject;
  }

  internal object GetFieldValue (string propertyName, ValueAccess valueAccess)
  {
    return _propertyValues[propertyName].GetFieldValue (valueAccess);
  }

  internal void PropertyValueChanging (PropertyValueCollection propertyValueCollection, PropertyChangeEventArgs args)
  {
    if (_state == DataContainerStateType.Deleted)
      throw new ObjectDeletedException (_id);

    if (_clientTransaction != null)
      _clientTransaction.PropertyValueChanging (this, args.PropertyValue, args.OldValue, args.NewValue);

    // Note: .NET 1.1 will not deserialize delegates to non-public (that means internal, protected, private) methods. 
    // Therefore notification of DomainObject when changing property values is not organized through events.
    DomainObject.PropertyValueChanging (this, args);

    OnPropertyChanging (args);
  }

  internal void PropertyValueChanged (PropertyValueCollection propertyValueCollection, PropertyChangeEventArgs args)
  {
    OnPropertyChanged (args);

    // Note: .NET 1.1 will not deserialize delegates to non-public (that means internal, protected, private) methods. 
    // Therefore notification of DomainObject when changing property values is not organized through events.
    DomainObject.PropertyValueChanged (this, args);

    if (_clientTransaction != null)
      _clientTransaction.PropertyValueChanged (this, args.PropertyValue, args.OldValue, args.NewValue);
  }

  internal void PropertyValueReading (PropertyValue propertyValue, ValueAccess valueAccess)
  {
    if (_clientTransaction != null)
      _clientTransaction.PropertyValueReading (this, propertyValue, valueAccess);
  }

  internal void PropertyValueRead (PropertyValue propertyValue, object value, ValueAccess valueAccess)
  {
    if (_clientTransaction != null)
      _clientTransaction.PropertyValueRead (this, propertyValue, value, valueAccess);
  }

  private StateType GetStateForPropertyValues ()
  {
    foreach (PropertyValue propertyValue in _propertyValues)
    {
      if (propertyValue.HasChanged)
        return StateType.Changed;
    }

    return StateType.Unchanged;
  }

  private void Discard ()
  {
    _propertyValues.Discard ();
    _clientTransaction = null;

    _isDiscarded = true;
  }

  private void CheckDiscarded ()
  {
    if (_isDiscarded)
      throw new ObjectDiscardedException (_id);
  }
}
}
