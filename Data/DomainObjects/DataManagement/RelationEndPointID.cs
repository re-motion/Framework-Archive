using System;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public class RelationEndPointID
{
  // types

  // static members and constants

  // member fields

  private string _propertyName;
  private ObjectID _objectID;

  // construction and disposing

  public RelationEndPointID (ObjectID objectID, string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    ArgumentUtility.CheckNotNull ("objectID", objectID);

    _objectID = objectID;
    _propertyName = propertyName;
  }

  // methods and properties

  public override int GetHashCode ()
  {
    return _objectID.GetHashCode () ^ _propertyName.GetHashCode ();
  }

  public override bool Equals (object obj)
  {
    RelationEndPointID endPointID = obj as RelationEndPointID;
    if (endPointID == null)
      return false;

    return this._objectID.Equals (endPointID.ObjectID)
        && this._propertyName.Equals (endPointID.PropertyName);
  }

  public override string ToString ()
  {
    return string.Format ("{0}/{1}", _objectID.ToString (), _propertyName);
  }

  public string PropertyName
  {
    get { return _propertyName; }
  }

  public ObjectID ObjectID
  {
    get { return _objectID; }
  }
}
}