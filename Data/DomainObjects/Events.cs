using System;
using System.ComponentModel;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects
{

/// <summary>
/// Represents the method that will handle the <see cref="PropertyValue.Changing"/> event of the <see cref="PropertyValue"/> class.
/// </summary>
public delegate void ValueChangingEventHandler (object sender, ValueChangingEventArgs args);

/// <summary>
/// Represents the method that will handle a <b>PropertyChanging</b> event.
/// </summary>
public delegate void PropertyChangingEventHandler (object sender, PropertyChangingEventArgs args);
/// <summary>
/// Represents the method that will handle a <b>PropertyChanged</b> event.
/// </summary>
public delegate void PropertyChangedEventHandler (object sender, PropertyChangedEventArgs args);

/// <summary>
/// Represents the method that will handle a <b>RelationChanging</b> event.
/// </summary>
public delegate void RelationChangingEventHandler (object sender, RelationChangingEventArgs args);
/// <summary>
/// Represents the method that will handle a <b>RelationChanged</b> event.
/// </summary>
public delegate void RelationChangedEventHandler (object sender, RelationChangedEventArgs args);

/// <summary>
/// Represents the method that will handle the <see cref="DomainObject.Deleting"/> event of the <see cref="DomainObject"/>.
/// </summary>
public delegate void DeletingEventHandler (object sender, EventArgs args);

/// <summary>
/// Provides data for a <see cref="PropertyValue.Changing"/> event of the <see cref="PropertyValue"/> class.
/// </summary>
public class ValueChangingEventArgs : EventArgs
{
  private object _oldValue;
  private object _newValue;

  /// <summary>
  /// Initializes a new instance of the <b>ValueChangingEventArgs</b> class with the <see cref="Cancel"/> property set to false.
  /// </summary>
  /// <param name="oldValue">The old value.</param>
  /// <param name="newValue">The new value.</param>
  public ValueChangingEventArgs (object oldValue, object newValue)
  {
    _oldValue = oldValue;
    _newValue = newValue;
  }

  /// <summary>
  /// Gets the old value.
  /// </summary>
  public object OldValue
  {
    get { return _oldValue; }
  }

  /// <summary>
  /// Gets the new value.
  /// </summary>
  public object NewValue
  {
    get { return _newValue; }
  }
}

/// <summary>
/// Provides data for a <b>PropertyChanging</b> event.
/// </summary>
public class PropertyChangingEventArgs : ValueChangingEventArgs
{
  private PropertyValue _propertyValue;

  /// <summary>
  /// Initializes a new instance of the <b>ValueChangingEventArgs</b> class with the <see cref="Cancel"/> property set to false.
  /// </summary>
  /// <param name="propertyValue">The <see cref="PropertyValue"/> that is being changed.</param>
  /// <param name="oldValue">The old value.</param>
  /// <param name="newValue">The new value.</param>
  /// <exception cref="System.ArgumentNullException"><i>propertyValue</i> is a null reference.</exception>
  public PropertyChangingEventArgs (PropertyValue propertyValue, object oldValue, object newValue) 
      : base (oldValue, newValue)
  {
    ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);
    _propertyValue = propertyValue;
  }

  /// <summary>
  /// Gets the <see cref="Rubicon.Data.DomainObjects.PropertyValue"/> object that is being changed.
  /// </summary>
  public PropertyValue PropertyValue
  {
    get { return _propertyValue; }
  }
}

/// <summary>
/// Provides data for a <b>PropertyChanged</b> event.
/// </summary>
public class PropertyChangedEventArgs : EventArgs
{
  private PropertyValue _propertyValue;

  /// <summary>
  /// Initializes a new instance of the <b>ValueChangingEventArgs</b> class.
  /// </summary>
  /// <param name="propertyValue">The <see cref="PropertyValue"/> that has been changed.</param>
  /// <exception cref="System.ArgumentNullException"><i>propertyValue</i> is a null reference.</exception>
  public PropertyChangedEventArgs (PropertyValue propertyValue)
  {
    ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

    _propertyValue = propertyValue;
  }

  /// <summary>
  /// Gets the <see cref="Rubicon.Data.DomainObjects.PropertyValue"/> object that has been changed.
  /// </summary>
  public PropertyValue PropertyValue
  {
    get { return _propertyValue; }
  }
}

/// <summary>
/// Provides data for a <b>RelationChanging</b> event.
/// </summary>
public class RelationChangingEventArgs : EventArgs
{
  private string _propertyName;
  private DomainObject _oldRelatedObject;
  private DomainObject _newRelatedObject;

  /// <summary>
  /// Initializes a new instance of the <b>RelationChangingEventArgs</b> class with the <see cref="Cancel"/> property set to false.
  /// </summary>
  /// <param name="propertyName">The name of the property that is being changed due to the relation change.</param>
  /// <param name="oldRelatedObject">The old object that was related.</param>
  /// <param name="newRelatedObject">The new object that is related.</param>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  public RelationChangingEventArgs (
      string propertyName, 
      DomainObject oldRelatedObject, 
      DomainObject newRelatedObject) 
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    _propertyName = propertyName;
    _oldRelatedObject = oldRelatedObject;
    _newRelatedObject = newRelatedObject;
  }

  /// <summary>
  /// Gets the name of the <see cref="PropertyValue"/> that is being changed due to the relation change.
  /// </summary>
  public string PropertyName
  {
    get { return _propertyName; }
  }

  /// <summary>
  /// Gets the <see cref="DomainObject"/> that was related.
  /// </summary>
  public DomainObject OldRelatedObject
  {
    get { return _oldRelatedObject; }
  }

  /// <summary>
  /// Gets the <see cref="DomainObject"/> that is related.
  /// </summary>
  public DomainObject NewRelatedObject
  {
    get { return _newRelatedObject; }
  }
}

/// <summary>
/// Provides data for a <b>RelationChanged</b> event.
/// </summary>
public class RelationChangedEventArgs : EventArgs
{
  private string _propertyName;

  /// <summary>
  /// Initializes a new instance of the <b>RelationChangingEventArgs</b> class.
  /// </summary>
  /// <param name="propertyName">The name of the <see cref="PropertyValue"/> that is being changed due to the relation change.</param>
  /// <exception cref="System.ArgumentNullException"><i>propertyName</i> is a null reference.</exception>
  public RelationChangedEventArgs (string propertyName) 
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

    _propertyName = propertyName;
  }

  /// <summary>
  /// Gets the name of the <see cref="PropertyValue"/> that has been changed due to the relation change.
  /// </summary>
  public string PropertyName
  {
    get { return _propertyName; }
  }
}
}