using System;
using System.Runtime.Serialization;

namespace Rubicon.Data.DomainObjects.Persistence
{
[Serializable]
public class ObjectNotFoundException : PersistenceException
{
  // types

  // static members and constants

  // member fields

  private ObjectID _id;

  // construction and disposing

  public ObjectNotFoundException () : base ("Object could not be found.") 
  {
  }

  public ObjectNotFoundException (string message) : base (message) 
  {
  }
  
  public ObjectNotFoundException (string message, Exception inner) : base (message, inner) 
  {
  }

  protected ObjectNotFoundException (SerializationInfo info, StreamingContext context) : base (info, context) 
  {
    _id = (ObjectID) info.GetValue ("ID", typeof (ObjectID));
  }

  public ObjectNotFoundException (ObjectID id) : this (string.Format ("Object '{0}' could not be found.", id), id)
  {
  }

  public ObjectNotFoundException (string message, ObjectID id) : base (message) 
  {
    ArgumentUtility.CheckNotNull ("id", id);

    _id = id;
  }

  // methods and properties

  public ObjectID ID
  {
    get { return _id; }
  }

  public override void GetObjectData (SerializationInfo info, StreamingContext context)
  {
    base.GetObjectData (info, context);

    info.AddValue ("ID", _id);
  }
}
}
