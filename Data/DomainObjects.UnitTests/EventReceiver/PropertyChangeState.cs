using System;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.EventSequence
{
public class PropertyChangeState : ChangeState
{
  // types

  // static members and constants

  // member fields

  private PropertyValue _propertyValue;
  private object _oldValue;
  private object _newValue;

  // construction and disposing

  public PropertyChangeState (
      object sender, 
      PropertyValue propertyValue, 
      object oldValue, 
      object newValue)
      : this (sender, propertyValue, oldValue, newValue, null)
  {
  }

  public PropertyChangeState (
      object sender, 
      PropertyValue propertyValue, 
      object oldValue, 
      object newValue, 
      string message)
      : base (sender, message)
  {
    ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

    _propertyValue = propertyValue;
    _oldValue = oldValue;
    _newValue = newValue;
  }

  // methods and properties

  public PropertyValue PropertyValue
  {
    get { return _propertyValue; }
  }

  public object OldValue
  {
    get { return _oldValue; }
  }

  public object NewValue
  {
    get { return _newValue; }
  }

  public override bool Compare (object obj)
  {
    if (!base.Compare (obj))
      return false;

    PropertyChangeState propertyChangeState = obj as PropertyChangeState;
    if (propertyChangeState == null)
      return false;

    if (!Equals (_propertyValue.Name, propertyChangeState.PropertyValue.Name))
      return false;

    if (!Equals (_oldValue, propertyChangeState.OldValue))
      return false;

    return Equals (_newValue, propertyChangeState.NewValue);
  }
}
}
