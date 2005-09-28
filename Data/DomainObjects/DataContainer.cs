using System;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects
{
/// <summary>
/// Represents a container for the persisted properties of a DomainObject.
/// </summary>
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
  /// <param name="id">The <see cref="ObjectID"/> of the new <see cref="DataContainer"/> to create.</param>
  /// <returns>The new <see cref="DataContainer"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><i>id</i> is a null reference.</exception>
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
  /// <param name="id">The <see cref="ObjectID"/> of the new <see cref="DataContainer"/> to create.</param>
  /// <param name="timestamp">The timestamp value of the existing object in the datasource.</param>
  /// <returns>The new <see cref="DataContainer"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><i>id</i> is a null reference.</exception>
  /// <exception cref="Mapping.MappingException">ClassDefinition of <i>id</i> does not exist in mapping.</exception>
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
  public event PropertyChangingEventHandler PropertyChanging;
  /// <summary>
  /// Occurs after a <see cref="PropertyValue"/> is changed.
  /// </summary>
  public event PropertyChangedEventHandler PropertyChanged;

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

    _propertyValues.PropertyChanging += new PropertyChangingEventHandler(PropertyValues_PropertyChanging);
    _propertyValues.PropertyChanged += new PropertyChangedEventHandler(PropertyValues_PropertyChanged);
  }

  // methods and properties

  /// <summary>
  /// Returns whether the property specified by <i>propertyName</i> is null.
  /// </summary>
  /// <param name="propertyName">The name of the <see cref="PropertyValue"/>.</param>
  /// <returns><b>true</b> if the value of the <see cref="PropertyValue"/> specified by <i>propertyName</i> is null; otherwise, <b>false</b>.</returns>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public bool IsNull (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    object value = this[propertyName];

    if (value == null)
      return true;

    INaNullable naNullable = value as INaNullable;
    if (naNullable != null)
      return naNullable.IsNull;

    return false;
  }

  /// <summary>
  /// Returns whether the property specified by <i>propertyName</i> is null or empty.
  /// </summary>
  /// <remarks>If a <see cref="System.String"/> or a <see cref="System.Guid"/> are equal to empty this method returns true.</remarks>
  /// <param name="propertyName">The name of the <see cref="PropertyValue"/>.</param>
  /// <returns><b>true</b> if the value of the <see cref="PropertyValue"/> specified by <i>propertyName</i> is equal to null or empty; otherwise, <b>false</b>.</returns>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public bool IsNullOrEmpty (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    if (IsNull (propertyName))
      return true;

    object value = this[propertyName];
    
    if (value.GetType () == typeof (string))
      return (((string) value) == string.Empty);
    
    if (value.GetType () == typeof (Guid))
      return (((Guid) value) == Guid.Empty);

    if (value.GetType () == typeof (byte[]))
      return (((byte[]) value).Length == 0);

    return false;
  }

  /// <summary>
  /// Gets the value of the <see cref="PropertyValue"/> specified by <i>propertyName</i>.
  /// </summary>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
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
  /// Gets the value of the <see cref="PropertyValue"/> specified by <i>propertyName</i>.
  /// </summary>
  /// <param name="propertyName">The name of the <see cref="PropertyValue"/>.</param>
  /// <returns>The value of the <see cref="PropertyValue"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public object GetValue (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return this[propertyName];
  }

  /// <summary>
  /// Sets the value of the <see cref="PropertyValue"/> specified by <i>propertyName</i>.
  /// </summary>
  /// <param name="propertyName">The name of the <see cref="PropertyValue"/>.</param>
  /// <param name="value">The value the <see cref="PropertyValue"/> is set to.</param>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
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
        _domainObject = DomainObject.Create (this);

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

  private StateType GetStateForPropertyValues ()
  {
    foreach (PropertyValue propertyValue in _propertyValues)
    {
      if (propertyValue.HasChanged)
        return StateType.Changed;
    }

    return StateType.Unchanged;
  }

  private void PropertyValues_PropertyChanging (object sender, PropertyChangingEventArgs args)
  {
    if (_state == DataContainerStateType.Deleted)
      throw new ObjectDeletedException (_id);

    OnPropertyChanging (args);
  }

  private void PropertyValues_PropertyChanged (object sender, PropertyChangedEventArgs args)
  {
    OnPropertyChanged (args);
  }

  private void Discard ()
  {
    _propertyValues.PropertyChanging -= new PropertyChangingEventHandler(PropertyValues_PropertyChanging);
    _propertyValues.PropertyChanged -= new PropertyChangedEventHandler(PropertyValues_PropertyChanged);
    _propertyValues.Discard ();
    _clientTransaction = null;

    _isDiscarded = true;
  }

  private void CheckDiscarded ()
  {
    if (_isDiscarded)
      throw new ObjectDiscardedException (_id);
  }

  #region Typed property accessors

  /// <summary>
  /// Gets the value of the <see cref="PropertyValue"/> specified by <i>propertyName</i> as an <see cref="ObjectID"/>.
  /// </summary>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.InvalidCastException">The value cannot be casted to an <see cref="ObjectID"/>.</exception>
  public ObjectID GetObjectID (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    object value = _propertyValues[propertyName].Value;
    
    if (value == null)
      return null;

    ObjectID objectIDValue = value as ObjectID;
    if (objectIDValue == null)
    {
      throw new InvalidCastException (
          string.Format ("Property '{0}' is of type '{1}', but must be 'Rubicon.Data.DomainObjects.ObjectID'.", 
              propertyName, value.GetType ()));
    }

    return objectIDValue;
  }

  /// <summary>
  /// Gets the value of the <see cref="PropertyValue"/> specified by <i>propertyName</i> as a <see cref="System.Boolean"/>.
  /// </summary>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.InvalidCastException">The value cannot be casted to a <see cref="System.Boolean"/>.</exception>
  public bool GetBoolean (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (bool) this[propertyName];
  }

  /// <summary>
  /// Gets the value of the <see cref="PropertyValue"/> specified by <i>propertyName</i> as a <see cref="System.Byte"/>.
  /// </summary>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.InvalidCastException">The value cannot be casted to a <see cref="System.Byte"/>.</exception>
  public byte GetByte (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (byte) this[propertyName];
  }

  /// <summary>
  /// Gets the value of the <see cref="PropertyValue"/> specified by <i>propertyName</i> as an <see cref="System.DateTime"/>.
  /// </summary>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.InvalidCastException">The value cannot be casted to a <see cref="System.DateTime"/>.</exception>
  public DateTime GetDateTime (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (DateTime) this[propertyName];
  }

  /// <summary>
  /// Gets the value of the <see cref="PropertyValue"/> specified by <i>propertyName</i> as a <see cref="System.Decimal"/>.
  /// </summary>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.InvalidCastException">The value cannot be casted to a <see cref="System.Decimal"/>.</exception>
  public decimal GetDecimal (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (decimal) this[propertyName];
  }

  /// <summary>
  /// Gets the value of the <see cref="PropertyValue"/> specified by <i>propertyName</i> as a <see cref="System.Double"/>.
  /// </summary>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.InvalidCastException">The value cannot be casted to a <see cref="System.Double"/>.</exception>
  public double GetDouble (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (double) this[propertyName];
  }

  /// <summary>
  /// Gets the value of the <see cref="PropertyValue"/> specified by <i>propertyName</i> as a <see cref="System.Guid"/>.
  /// </summary>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.InvalidCastException">The value cannot be casted to a <see cref="System.Guid"/>.</exception>
  public Guid GetGuid (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (Guid) this[propertyName];
  }

  /// <summary>
  /// Gets the value of the <see cref="PropertyValue"/> specified by <i>propertyName</i> as a <see cref="System.Int16"/>.
  /// </summary>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.InvalidCastException">The value cannot be casted to a <see cref="System.Int16"/>.</exception>
  public short GetInt16 (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (short) this[propertyName];
  }

  /// <summary>
  /// Gets the value of the <see cref="PropertyValue"/> specified by <i>propertyName</i> as a <see cref="System.Int32"/>.
  /// </summary>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.InvalidCastException">The value cannot be casted to a <see cref="System.Int32"/>.</exception>
  public int GetInt32 (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (int) this[propertyName];
  }

  /// <summary>
  /// Gets the value of the <see cref="PropertyValue"/> specified by <i>propertyName</i> as a <see cref="System.Int64"/>.
  /// </summary>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.InvalidCastException">The value cannot be casted to a <see cref="System.Int64"/>.</exception>
  public long GetInt64 (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (long) this[propertyName];
  }

  /// <summary>
  /// Gets the value of the <see cref="PropertyValue"/> specified by <i>propertyName</i> as a <see cref="System.Single"/>.
  /// </summary>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.InvalidCastException">The value cannot be casted to a <see cref="System.Single"/>.</exception>
  public float GetSingle (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (float) this[propertyName];
  }

  /// <summary>
  /// Gets the value of the <see cref="PropertyValue"/> specified by <i>propertyName</i> as a <see cref="System.String"/>.
  /// </summary>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.InvalidCastException">The value cannot be casted to a <see cref="System.String"/>.</exception>
  public string GetString (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (string) this[propertyName];
  }

  /// <summary>
  /// Gets the value of the <see cref="PropertyValue"/> specified by <i>propertyName</i> as a <see cref="Rubicon.NullableValueTypes.NaBoolean"/>.
  /// </summary>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.InvalidCastException">The value cannot be casted to a <see cref="Rubicon.NullableValueTypes.NaBoolean"/>.</exception>
  public NaBoolean GetNaBoolean (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (NaBoolean) this[propertyName];
  }

  /// <summary>
  /// Gets the value of the <see cref="PropertyValue"/> specified by <i>propertyName</i> as a <see cref="Rubicon.NullableValueTypes.NaDateTime"/>.
  /// </summary>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.InvalidCastException">The value cannot be casted to a <see cref="Rubicon.NullableValueTypes.NaDateTime"/>.</exception>
  public NaDateTime GetNaDateTime (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (NaDateTime) this[propertyName];
  }

  /// <summary>
  /// Gets the value of the <see cref="PropertyValue"/> specified by <i>propertyName</i> as a <see cref="Rubicon.NullableValueTypes.NaDouble"/>.
  /// </summary>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.InvalidCastException">The value cannot be casted to a <see cref="Rubicon.NullableValueTypes.NaDouble"/>.</exception>
  public NaDouble GetNaDouble (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (NaDouble) this[propertyName];
  }

  /// <summary>
  /// Gets the value of the <see cref="PropertyValue"/> specified by <i>propertyName</i> as a <see cref="Rubicon.NullableValueTypes.NaInt32"/>.
  /// </summary>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.InvalidCastException">The value cannot be casted to a <see cref="Rubicon.NullableValueTypes.NaInt32"/>.</exception>
  public NaInt32 GetNaInt32 (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (NaInt32) this[propertyName];
  }

  /// <summary>
  /// Gets the value of the <see cref="PropertyValue"/> specified by <i>propertyName</i> as a <see cref="Rubicon.NullableValueTypes.NaByte"/>.
  /// </summary>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.InvalidCastException">The value cannot be casted to a <see cref="Rubicon.NullableValueTypes.NaByte"/>.</exception>
  public NaByte GetNaByte (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (NaByte) this[propertyName];
  }

  /// <summary>
  /// Gets the value of the <see cref="PropertyValue"/> specified by <i>propertyName</i> as a <see cref="Rubicon.NullableValueTypes.NaDecimal"/>.
  /// </summary>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.InvalidCastException">The value cannot be casted to a <see cref="Rubicon.NullableValueTypes.NaDecimal"/>.</exception>
  public NaDecimal GetNaDecimal (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (NaDecimal) this[propertyName];
  }

  /// <summary>
  /// Gets the value of the <see cref="PropertyValue"/> specified by <i>propertyName</i> as a <see cref="Rubicon.NullableValueTypes.NaGuid"/>.
  /// </summary>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.InvalidCastException">The value cannot be casted to a <see cref="Rubicon.NullableValueTypes.NaGuid"/>.</exception>
  public NaGuid GetNaGuid (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (NaGuid) this[propertyName];
  }

  /// <summary>
  /// Gets the value of the <see cref="PropertyValue"/> specified by <i>propertyName</i> as a <see cref="Rubicon.NullableValueTypes.NaInt16"/>.
  /// </summary>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.InvalidCastException">The value cannot be casted to a <see cref="Rubicon.NullableValueTypes.NaInt16"/>.</exception>
  public NaInt16 GetNaInt16 (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (NaInt16) this[propertyName];
  }

  /// <summary>
  /// Gets the value of the <see cref="PropertyValue"/> specified by <i>propertyName</i> as a <see cref="Rubicon.NullableValueTypes.NaInt64"/>.
  /// </summary>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.InvalidCastException">The value cannot be casted to a <see cref="Rubicon.NullableValueTypes.NaInt64"/>.</exception>
  public NaInt64 GetNaInt64 (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (NaInt64) this[propertyName];
  }

  /// <summary>
  /// Gets the value of the <see cref="PropertyValue"/> specified by <i>propertyName</i> as a <see cref="Rubicon.NullableValueTypes.NaSingle"/>.
  /// </summary>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.InvalidCastException">The value cannot be casted to a <see cref="Rubicon.NullableValueTypes.NaSingle"/>.</exception>
  public NaSingle GetNaSingle (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (NaSingle) this[propertyName];
  }

  /// <summary>
  /// Gets the value of the <see cref="PropertyValue"/> specified by <i>propertyName</i> as a byte array.
  /// </summary>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Utilities.ArgumentEmptyException"><i>propertyName</i> is an empty string.</exception>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="System.InvalidCastException">The value cannot be casted to a byte array.</exception>
  public byte[] GetBytes (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (byte[]) this[propertyName];
  }

  #endregion
}
}
