using System;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.EventReceiver
{
  public class PropertyValueContainerEventReceiver : EventReceiverBase
  {
    // types

    // static members and constants

    // member fields

    private bool _cancel;
    private PropertyValue _changingPropertyValue;
    private PropertyValue _changedPropertyValue;
    private object _changingOldValue;
    private object _changingNewValue;
    private object _changedOldValue;
    private object _changedNewValue;

    // construction and disposing

    public PropertyValueContainerEventReceiver (PropertyValueCollection propertyValueCollection, bool cancel)
    {
      _cancel = cancel;

      propertyValueCollection.PropertyChanging += new PropertyChangeEventHandler (
          PropertyValueContainer_PropertyChanging);

      propertyValueCollection.PropertyChanged += new PropertyChangeEventHandler (
          PropertyValueContainer_PropertyChanged);
    }

    public PropertyValueContainerEventReceiver (DataContainer dataContainer, bool cancel)
    {
      _cancel = cancel;

      dataContainer.PropertyChanging += new PropertyChangeEventHandler (PropertyValueContainer_PropertyChanging);
      dataContainer.PropertyChanged += new PropertyChangeEventHandler (PropertyValueContainer_PropertyChanged);
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

    public object ChangingOldValue
    {
      get { return _changingOldValue; }
    }

    public object ChangingNewValue
    {
      get { return _changingNewValue; }
    }

    public object ChangedOldValue
    {
      get { return _changedOldValue; }
    }

    public object ChangedNewValue
    {
      get { return _changedNewValue; }
    }

    private void PropertyValueContainer_PropertyChanging (object sender, PropertyChangeEventArgs args)
    {
      _changingPropertyValue = args.PropertyValue;
      _changingOldValue = args.OldValue;
      _changingNewValue = args.NewValue;

      if (_cancel)
        CancelOperation ();
    }

    private void PropertyValueContainer_PropertyChanged (object sender, PropertyChangeEventArgs args)
    {
      _changedPropertyValue = args.PropertyValue;
      _changedOldValue = args.OldValue;
      _changedNewValue = args.NewValue;
    }
  }
}
