using System;

namespace Rubicon.Data.DomainObjects.UnitTests.EventSequence
{
public class RelationChangeState : ChangeState
{
  // types

  // static members and constants

  // member fields

  private string _propertyName;
  private DomainObject _oldDomainObject;
  private DomainObject _newDomainObject;

  // construction and disposing

  public RelationChangeState (
      object sender, 
      string propertyName, 
      DomainObject oldDomainObject, 
      DomainObject newDomainObject)
      : this (sender, propertyName, oldDomainObject, newDomainObject, null)
  {
  }

  public RelationChangeState (
      object sender, 
      string propertyName, 
      DomainObject oldDomainObject, 
      DomainObject newDomainObject,
      string message)
      : base (sender, message)
  {
    ArgumentUtility.CheckNotNull ("propertyName", propertyName);

    _propertyName = propertyName;
    _oldDomainObject = oldDomainObject;
    _newDomainObject = newDomainObject;
  }

  // methods and properties

  public string PropertyName
  {
    get { return _propertyName; }
  }

  public DomainObject OldDomainObject
  {
    get { return _oldDomainObject; }
  }

  public DomainObject NewDomainObject
  {
    get { return _newDomainObject; }
  }

  public override bool Compare (object obj)
  {
    if (!base.Compare (obj))
      return false;

    RelationChangeState relationChangeState = obj as RelationChangeState;
    if (relationChangeState == null)
      return false;

    if (!Equals (_propertyName, relationChangeState.PropertyName))
      return false;

    if (!Equals (_oldDomainObject, relationChangeState.OldDomainObject))
      return false;

    return Equals (_newDomainObject, relationChangeState.NewDomainObject);
  }
}
}
