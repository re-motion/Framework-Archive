using System;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.EventSequence
{
public class ChangeState
{
  // types

  // static members and constants

  // member fields

  private object _sender;
  private string _message;

  // construction and disposing

  public ChangeState (object sender) : this (sender, null)
  {
  }

  public ChangeState (object sender, string message)
  {
    ArgumentUtility.CheckNotNull ("sender", sender);

    _sender = sender;
    _message = message;
  }

  // methods and properties

  public object Sender
  {
    get { return _sender; }
  }

  public string Message
  {
    get { return _message; }
  }

  public virtual bool Compare (object obj)
  {
    ChangeState changeState = obj as ChangeState;
    if (changeState == null)
      return false;

    return object.ReferenceEquals (_sender, changeState.Sender);
  }

  protected bool Compare (object object1, object object2)
  {
    if (object1 == null && object2 == null)
      return true;

    if (object1 != null)
      return object1.Equals (object2);
    else
      return object2.Equals (object1);
  }
}
}
