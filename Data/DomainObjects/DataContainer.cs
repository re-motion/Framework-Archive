using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects
{
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

  public static DataContainer CreateNew (ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    DataContainer newDataContainer = new DataContainer (id);
    newDataContainer._state = DataContainerStateType.New;
    
    foreach (PropertyDefinition propertyDefinition in newDataContainer.ClassDefinition.GetAllPropertyDefinitions ())
      newDataContainer.PropertyValues.Add (new PropertyValue (propertyDefinition));

    return newDataContainer;
  }

  public static DataContainer CreateForExisting (ObjectID id, object timestamp)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    DataContainer dataContainer = new DataContainer (id, timestamp);
    dataContainer._state = DataContainerStateType.Existing;
    return dataContainer;
  }

  // member fields

  public event PropertyChangingEventHandler PropertyChanging;
  public event PropertyChangedEventHandler PropertyChanged;

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

  public object GetValue (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return this[propertyName];
  }

  public void SetValue (string propertyName, object value)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    this[propertyName] = value;
  }

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

  public ObjectID ID
  {
    get 
    { 
      CheckDiscarded ();
      return _id; 
    }
  }

  public ClassDefinition ClassDefinition
  {
    get 
    { 
      CheckDiscarded ();
      return _classDefinition; 
    }
  }

  public Type DomainObjectType
  {
    get 
    { 
      CheckDiscarded ();
      return _classDefinition.ClassType; 
    }
  }

  public PropertyValueCollection PropertyValues
  {
    get 
    {
      CheckDiscarded ();
      return _propertyValues; 
    }
  }

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

  public object Timestamp
  {
    get 
    {
      CheckDiscarded ();
      return _timestamp; 
    }
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

  protected virtual void OnPropertyChanging (PropertyChangingEventArgs args)
  {
    if (PropertyChanging != null)
      PropertyChanging (this, args);
  }

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
    {
      _isDiscarded = true;
      _propertyValues.Discard ();
    }

    _state = DataContainerStateType.Deleted;
  }

  internal void SetTimestamp (object timestamp)
  {
    ArgumentUtility.CheckNotNull ("timestamp", timestamp);

    _timestamp = timestamp;
  }

  internal void Commit ()
  {
    foreach (PropertyValue propertyValue in _propertyValues)
      propertyValue.Commit ();

    _state = DataContainerStateType.Existing;
  }

  internal void Rollback ()
  {
    foreach (PropertyValue propertyValue in _propertyValues)
      propertyValue.Rollback (); 
  }

  internal void SetDomainObject (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    
    _domainObject = domainObject;
  }

  internal bool IsDiscarded
  {
    get { return _isDiscarded; }
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

  private void CheckDiscarded ()
  {
    if (_isDiscarded)
      throw new ObjectDiscardedException (_id);
  }

  #region Typed property accessors

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

  public bool GetBoolean (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (bool) this[propertyName];
  }

  public byte GetByte (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (byte) this[propertyName];
  }

  public char GetChar (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (char) this[propertyName];
  }

  public DateTime GetDateTime (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (DateTime) this[propertyName];
  }

  public decimal GetDecimal (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (decimal) this[propertyName];
  }

  public double GetDouble (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (double) this[propertyName];
  }

  public Guid GetGuid (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (Guid) this[propertyName];
  }

  public short GetInt16 (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (short) this[propertyName];
  }

  public int GetInt32 (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (int) this[propertyName];
  }

  public long GetInt64 (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (long) this[propertyName];
  }

  public float GetSingle (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (float) this[propertyName];
  }

  public string GetString (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (string) this[propertyName];
  }

  public NaBoolean GetNaBoolean (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (NaBoolean) this[propertyName];
  }

  public NaDateTime GetNaDateTime (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (NaDateTime) this[propertyName];
  }

  public NaDouble GetNaDouble (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (NaDouble) this[propertyName];
  }

  public NaInt32 GetNaInt32 (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    CheckDiscarded ();

    return (NaInt32) this[propertyName];
  }

  #endregion
}
}
