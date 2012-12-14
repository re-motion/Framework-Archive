using System;

using Rubicon.Data.DomainObjects.DataManagement;

namespace Rubicon.Data.DomainObjects.UnitTests.EventReceiver
{
public class PropertyValueEventReceiver : EventReceiverBase
{
  // types

  // static members and constants

  // member fields

  private bool _cancel;
  private bool _hasChangingEventBeenCalled;
  private bool _hasChangedEventBeenCalled;
  private object _oldValue;
  private object _newValue;

  // construction and disposing

  public PropertyValueEventReceiver (PropertyValue propertyValue) : this (propertyValue, false)
  {
  }

  public PropertyValueEventReceiver (PropertyValue propertyValue, bool cancel)
  {
    propertyValue.Changing += new ValueChangingEventHandler (PropertyValue_Changing);
    propertyValue.Changed += new EventHandler(PropertyValue_Changed);
    _cancel = cancel;
  }

  // methods and properties

  public bool HasChangingEventBeenCalled
  {
    get { return _hasChangingEventBeenCalled; }
  }

  public bool HasChangedEventBeenCalled
  {
    get { return _hasChangedEventBeenCalled; }
  }

  public object OldValue
  {
    get { return _oldValue; }
  }

  public object NewValue
  {
    get { return _newValue; }
  }

  private void PropertyValue_Changing(object sender, ValueChangingEventArgs e)
  {
    _hasChangingEventBeenCalled = true;
    _oldValue = e.OldValue;
    _newValue = e.NewValue;

    if (_cancel)
      CancelOperation ();
  }

  private void PropertyValue_Changed(object sender, EventArgs e)
  {
    _hasChangedEventBeenCalled = true;
  }
}
}
