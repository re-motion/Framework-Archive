using System;
using System.Runtime.Serialization;

namespace Rubicon.Data.DomainObjects.DataManagement
{
[Serializable]
public class ObjectDiscardedException : DomainObjectException
{
  // types

  // static members and constants

  // member fields

  private ObjectID _id;

  // construction and disposing

  public ObjectDiscardedException () : this ("Object is already discarded.") 
  {
  }

  public ObjectDiscardedException (string message) : base (message) 
  {
  }
  
  public ObjectDiscardedException (string message, Exception inner) : base (message, inner) 
  {
  }

  protected ObjectDiscardedException (SerializationInfo info, StreamingContext context) : base (info, context) 
  {
    _id = (ObjectID) info.GetValue ("ID", typeof (ObjectID));
  }

  public ObjectDiscardedException (ObjectID id) : this (string.Format ("Object '{0}' is already discarded.", id), id)
  {
  }

  public ObjectDiscardedException (string message, ObjectID id) : base (message) 
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
