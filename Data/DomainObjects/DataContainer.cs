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
    New = 1
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
      return _propertyValues[propertyName].Value; 
    }
    set 
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      _propertyValues[propertyName].Value = value; 
    }
  }

  public RelationEndPointID[] RelationEndPointIDs
  {
    get
    {
      if (_relationEndPointIDs != null)
        return _relationEndPointIDs;

      IRelationEndPointDefinition[] endPointDefinitions = _classDefinition.GetAllRelationEndPointDefinitions ();
      _relationEndPointIDs = new RelationEndPointID[endPointDefinitions.Length];

      for (int i = 0; i < endPointDefinitions.Length; i++)
        _relationEndPointIDs[i] = new RelationEndPointID (_id, endPointDefinitions[i].PropertyName);

      return _relationEndPointIDs;
    }
  }

  public DomainObject DomainObject
  {
    get 
    {
      if (_domainObject == null)
        _domainObject = DomainObject.Create (this);

      return _domainObject; 
    }
  }

  public ObjectID ID
  {
    get { return _id; }
  }

  public ClassDefinition ClassDefinition
  {
    get { return _classDefinition; }
  }

  public Type DomainObjectType
  {
    get { return _classDefinition.ClassType; }
  }

  public PropertyValueCollection PropertyValues
  {
    get { return _propertyValues; }
  }

  public StateType State
  {
    get 
    { 
      if (_state == DataContainerStateType.Existing)
        return GetStateForPropertyValues ();

      return StateType.New;
    }
  }

  public object Timestamp
  {
    get { return _timestamp; }
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

  private StateType GetStateForPropertyValues ()
  {
    foreach (PropertyValue propertyValue in _propertyValues)
    {
      if (propertyValue.HasChanged)
        return StateType.Changed;
    }

    return StateType.Original;
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

  private void PropertyValues_PropertyChanging (object sender, PropertyChangingEventArgs args)
  {
    OnPropertyChanging (args);
  }

  private void PropertyValues_PropertyChanged (object sender, PropertyChangedEventArgs args)
  {
    OnPropertyChanged (args);
  }

  #region Typed property accessors

  public ObjectID GetObjectID (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

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

    return (bool) this[propertyName];
  }

  public byte GetByte (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    return (byte) this[propertyName];
  }

  public char GetChar (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    return (char) this[propertyName];
  }

  public DateTime GetDateTime (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    return (DateTime) this[propertyName];
  }

  public decimal GetDecimal (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    return (decimal) this[propertyName];
  }

  public double GetDouble (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    return (double) this[propertyName];
  }

  public Guid GetGuid (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    return (Guid) this[propertyName];
  }

  public short GetInt16 (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    return (short) this[propertyName];
  }

  public int GetInt32 (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    return (int) this[propertyName];
  }

  public long GetInt64 (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    return (long) this[propertyName];
  }

  public float GetSingle (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    return (float) this[propertyName];
  }

  public string GetString (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    return (string) this[propertyName];
  }

  public NaBoolean GetNaBoolean (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    return (NaBoolean) this[propertyName];
  }

  public NaDateTime GetNaDateTime (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    return (NaDateTime) this[propertyName];
  }

  public NaDouble GetNaDouble (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    return (NaDouble) this[propertyName];
  }

  public NaInt32 GetNaInt32 (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    return (NaInt32) this[propertyName];
  }

  #endregion
}
}
