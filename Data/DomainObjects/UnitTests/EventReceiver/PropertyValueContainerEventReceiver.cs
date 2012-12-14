using System;

using Rubicon.Data.DomainObjects.DataManagement;

namespace Rubicon.Data.DomainObjects.UnitTests.EventReceiver
{
public class PropertyValueContainerEventReceiver : EventReceiverBase
{
  // types

  // static members and constants

  // member fields

  private bool _cancel;
  private PropertyValue _changingPropertyValue;
  private PropertyValue _changedPropertyValue;
  private object _oldValue;
  private object _newValue;

  // construction and disposing

  public PropertyValueContainerEventReceiver (PropertyValueCollection propertyValueCollection, bool cancel)
  {
    _cancel = cancel;

    propertyValueCollection.PropertyChanging += new PropertyChangingEventHandler(
        PropertyValueContainer_PropertyChanging);

    propertyValueCollection.PropertyChanged += new PropertyChangedEventHandler(
        PropertyValueContainer_PropertyChanged);
  }

  public PropertyValueContainerEventReceiver (DataContainer dataContainer, bool cancel)
  {
    _cancel = cancel;

    dataContainer.PropertyChanging += new PropertyChangingEventHandler(PropertyValueContainer_PropertyChanging);
    dataContainer.PropertyChanged += new PropertyChangedEventHandler(PropertyValueContainer_PropertyChanged);
  }

  // methods and properties

  public PropertyValue ChangingPropertyValue
  {
    get { return _changingPropertyValue; }
  }

  public PropertyValue ChangedPropertyValue
  {
    get { return _changedPropertyValue; }
  }

  public object OldValue
  {
    get { return _oldValue; }
  }

  public object NewValue
  {
    get { return _newValue; }
  }

  private void PropertyValueContainer_PropertyChanging(object sender, PropertyChangingEventArgs args)
  {
    _changingPropertyValue = args.PropertyValue;
    _oldValue = args.OldValue;
    _newValue = args.NewValue;

    if (_cancel)
      CancelOperation ();
  }

  private void PropertyValueContainer_PropertyChanged(object sender, PropertyChangedEventArgs args)
  {
    _changedPropertyValue = args.PropertyValue;
  }
}
}
