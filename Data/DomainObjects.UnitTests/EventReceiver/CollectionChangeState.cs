using System;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.EventSequence
{
public class CollectionChangeState : ChangeState
{
  // types

  // static members and constants

  // member fields

  private DomainObject _domainObject;

  // construction and disposing

  public CollectionChangeState (object sender, DomainObject domainObject) 
      : this (sender, domainObject, null)
  {
  }

  public CollectionChangeState (object sender, DomainObject domainObject, string message) 
      : base (sender, message)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);

    _domainObject = domainObject;
  }

  // methods and properties

  public DomainObject DomainObject
  {
    get { return _domainObject; }
  }

  public override bool Compare (object obj)
  {
    if (!base.Compare (obj))
      return false;

    CollectionChangeState collectionChangeState = obj as CollectionChangeState;
    if (collectionChangeState == null)
      return false;

    return Object.ReferenceEquals (_domainObject, collectionChangeState.DomainObject);
  }
}
}
