using System;
using System.Collections;

using Rubicon.Data.DomainObjects.DataManagement;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
public class DomainObjectEventReceiver
{
  // types

  // static members and constants

  // member fields

  private bool _cancel;
  private bool _hasChangingEventBeenCalled;
  private bool _hasChangedEventBeenCalled;
  private PropertyValue _changingPropertyValue;
  private PropertyValue _changedPropertyValue;
  private object _oldValue;
  private object _newValue;

  private bool _hasRelationChangingEventBeenCalled;
  private bool _hasRelationChangedEventBeenCalled;
  private string _changingRelationPropertyName;
  private string _changedRelationPropertyName;
  private DomainObject _oldRelatedObject;
  private DomainObject _newRelatedObject;

  // construction and disposing

  public DomainObjectEventReceiver (DomainObject domainObject) : this (domainObject, false)
  {
  }

  public DomainObjectEventReceiver (DomainObject domainObject, bool cancel)
  {
    _cancel = cancel;

    domainObject.PropertyChanging += new PropertyChangingEventHandler (DomainObject_PropertyChanging);
    domainObject.PropertyChanged += new PropertyChangedEventHandler (DomainObject_PropertyChanged);
    domainObject.RelationChanging += new RelationChangingEventHandler (DomainObject_RelationChanging);
    domainObject.RelationChanged += new RelationChangedEventHandler (DomainObject_RelationChanged);
  }

  // methods and properties

  public bool Cancel
  {
    get { return _cancel; }
    set { _cancel = value; }
  }

  public bool HasChangingEventBeenCalled
  {
    get { return _hasChangingEventBeenCalled; }
  }

  public bool HasChangedEventBeenCalled
  {
    get { return _hasChangedEventBeenCalled; }
  }

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

  public bool HasRelationChangingEventBeenCalled
  {
    get { return _hasRelationChangingEventBeenCalled; }
  }

  public bool HasRelationChangedEventBeenCalled
  {
    get { return _hasRelationChangedEventBeenCalled; }
  }

  public string ChangingRelationPropertyName
  {
    get { return _changingRelationPropertyName; }
  }

  public string ChangedRelationPropertyName
  {
    get { return _changedRelationPropertyName; }
  }

  public DomainObject OldRelatedObject
  {
    get { return _oldRelatedObject; }
  }

  public DomainObject NewRelatedObject
  {
    get { return _newRelatedObject; }
  }

  private void DomainObject_PropertyChanging(object sender, PropertyChangingEventArgs args)
  {
    _hasChangingEventBeenCalled = true;
    _changingPropertyValue = args.PropertyValue;
    _oldValue = args.OldValue;
    _newValue = args.NewValue;
    args.Cancel = _cancel;
  }

  private void DomainObject_PropertyChanged(object sender, PropertyChangedEventArgs args)
  {
    _hasChangedEventBeenCalled = true;
    _changedPropertyValue = args.PropertyValue;
  }

  private void DomainObject_RelationChanging (object sender, RelationChangingEventArgs args)
  {
    _hasRelationChangingEventBeenCalled = true;
    _changingRelationPropertyName = args.PropertyName;
    _oldRelatedObject = args.OldRelatedObject;
    _newRelatedObject = args.NewRelatedObject;
    args.Cancel = _cancel;
  }

  private void DomainObject_RelationChanged (object sender, RelationChangedEventArgs args)
  {
    _hasRelationChangedEventBeenCalled = true;
    _changedRelationPropertyName = args.PropertyName;
  }
}
}
