using System;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects
{
/// <summary>
/// Represents the method that will handle the <see cref="PropertyValue.Changing"/> event of the <see cref="PropertyValue"/> class.
/// </summary>
public delegate void ValueChangeEventHandler (object sender, ValueChangeEventArgs args);

/// <summary>
/// Represents the method that will handle a <b>PropertyChanging</b> event.
/// </summary>
public delegate void PropertyChangeEventHandler (object sender, PropertyChangeEventArgs args);

/// <summary>
/// Represents the method that will handle a <b>RelationChanging</b> event.
/// </summary>
public delegate void RelationChangingEventHandler (object sender, RelationChangingEventArgs args);
/// <summary>
/// Represents the method that will handle a <b>RelationChanged</b> event.
/// </summary>
public delegate void RelationChangedEventHandler (object sender, RelationChangedEventArgs args);

/// <summary>
/// Represents the method that will handle <see cref="ClientTransaction"/> events.
/// </summary>
public delegate void ClientTransactionEventHandler (object sender, ClientTransactionEventArgs args);

/// <summary>
/// Provides data for a <see cref="PropertyValue.Changing"/> event of the <see cref="PropertyValue"/> class.
/// </summary>
[Serializable]
public class ValueChangeEventArgs : EventArgs
{
  private object _oldValue;
  private object _newValue;

  /// <summary>
  /// Initializes a new instance of the <b>ValueChangingEventArgs</b>.
  /// </summary>
  /// <param name="oldValue">The old value.</param>
  /// <param name="newValue">The new value.</param>
  public ValueChangeEventArgs (object oldValue, object newValue)
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
[Serializable]
public class PropertyChangeEventArgs : ValueChangeEventArgs
{
  private PropertyValue _propertyValue;

  /// <summary>
  /// Initializes a new instance of the <b>ValueChangingEventArgs</b> class.
  /// </summary>
  /// <param name="propertyValue">The <see cref="PropertyValue"/> that is being changed. Must not be <see langword="null"/>.</param>
  /// <param name="oldValue">The old value.</param>
  /// <param name="newValue">The new value.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="propertyValue"/> is <see langword="null"/>.</exception>
  public PropertyChangeEventArgs (PropertyValue propertyValue, object oldValue, object newValue) 
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
/// Provides data for a <b>RelationChanging</b> event.
/// </summary>
[Serializable]
public class RelationChangingEventArgs : EventArgs
{
  private string _propertyName;
  private DomainObject _oldRelatedObject;
  private DomainObject _newRelatedObject;

  /// <summary>
  /// Initializes a new instance of the <b>RelationChangingEventArgs</b> class.
  /// </summary>
  /// <param name="propertyName">The name of the property that is being changed due to the relation change. Must not be <see langword="null"/>.</param>
  /// <param name="oldRelatedObject">The old object that was related.</param>
  /// <param name="newRelatedObject">The new object that is related.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
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
[Serializable]
public class RelationChangedEventArgs : EventArgs
{
  private string _propertyName;

  /// <summary>
  /// Initializes a new instance of the <b>RelationChangingEventArgs</b> class.
  /// </summary>
  /// <param name="propertyName">The name of the <see cref="PropertyValue"/> that is being changed due to the relation change. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
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

/// <summary>
/// Provides data for <see cref="ClientTransaction"/> events.
/// </summary>
[Serializable]
public class ClientTransactionEventArgs : EventArgs
{
  private DomainObjectCollection _domainObjects;

  /// <summary>
  /// Initializes a new instance of the <b>ClientTransactionEventArgs</b> class.
  /// </summary>
  /// <param name="domainObjects">The <see cref="DomainObject"/>s affected by the <see cref="ClientTransaction"/> operation. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="domainObjects"/> is <see langword="null"/>.</exception>
  public ClientTransactionEventArgs (DomainObjectCollection domainObjects)
  {
    ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);
    _domainObjects = domainObjects;
  }

  /// <summary>
  /// Gets the <see cref="DomainObject"/>s affected by the <see cref="ClientTransaction.Commit"/> operation.
  /// </summary>
  public DomainObjectCollection DomainObjects
  {
    get { return _domainObjects; }
  }
}
}