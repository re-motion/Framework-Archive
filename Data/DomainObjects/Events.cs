using System;
using System.ComponentModel;

using Rubicon.Data.DomainObjects.DataManagement;

namespace Rubicon.Data.DomainObjects
{

public delegate void ValueChangingEventHandler (object sender, ValueChangingEventArgs args);

public delegate void PropertyChangingEventHandler (object sender, PropertyChangingEventArgs args);
public delegate void PropertyChangedEventHandler (object sender, PropertyChangedEventArgs args);

public delegate void RelationChangingEventHandler (object sender, RelationChangingEventArgs args);
public delegate void RelationChangedEventHandler (object sender, RelationChangedEventArgs args);

public delegate void DeletingEventHandler (object sender, DeletingEventArgs args);

public class ValueChangingEventArgs : CancelEventArgs
{
  private object _oldValue;
  private object _newValue;

  public ValueChangingEventArgs (object oldValue, object newValue) : this (oldValue, newValue, false)
  {
  }

  public ValueChangingEventArgs (object oldValue, object newValue, bool cancel) : base (cancel)
  {
    _oldValue = oldValue;
    _newValue = newValue;
  }

  public object OldValue
  {
    get { return _oldValue; }
  }

  public object NewValue
  {
    get { return _newValue; }
  }
}

public class PropertyChangingEventArgs : ValueChangingEventArgs
{
  private PropertyValue _propertyValue;

  public PropertyChangingEventArgs (PropertyValue propertyValue, object oldValue, object newValue) 
      : this (propertyValue, oldValue, newValue, false)
  {
  }

  public PropertyChangingEventArgs (PropertyValue propertyValue, object oldValue, object newValue, bool cancel) 
      : base (oldValue, newValue, cancel)
  {
    ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

    _propertyValue = propertyValue;
  }

  public PropertyValue PropertyValue
  {
    get { return _propertyValue; }
  }
}

public class PropertyChangedEventArgs : EventArgs
{
  private PropertyValue _propertyValue;

  public PropertyChangedEventArgs (PropertyValue propertyValue)
  {
    ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

    _propertyValue = propertyValue;
  }

  public PropertyValue PropertyValue
  {
    get { return _propertyValue; }
  }
}

public class RelationChangingEventArgs : CancelEventArgs
{
  private string _propertyName;
  private DomainObject _oldRelatedObject;
  private DomainObject _newRelatedObject;

  public RelationChangingEventArgs (
      string propertyName, 
      DomainObject oldRelatedObject, 
      DomainObject newRelatedObject) 
      : this (propertyName, oldRelatedObject, newRelatedObject, false)
  {
  }

  public RelationChangingEventArgs (
      string propertyName, 
      DomainObject oldRelatedObject, 
      DomainObject newRelatedObject,     
      bool cancel) 
      : base (cancel)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    _propertyName = propertyName;
    _oldRelatedObject = oldRelatedObject;
    _newRelatedObject = newRelatedObject;
  }

  public string PropertyName
  {
    get { return _propertyName; }
  }

  public DomainObject OldRelatedObject
  {
    get { return _oldRelatedObject; }
  }

  public DomainObject NewRelatedObject
  {
    get { return _newRelatedObject; }
  }
}

public class RelationChangedEventArgs : EventArgs
{
  private string _propertyName;

  public RelationChangedEventArgs (string propertyName) 
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    _propertyName = propertyName;
  }

  public string PropertyName
  {
    get { return _propertyName; }
  }
}

public class DeletingEventArgs : CancelEventArgs
{

  public DeletingEventArgs () : this (false)
  {
  }

  public DeletingEventArgs (bool cancel) : base (cancel)
  {
  }
}
}