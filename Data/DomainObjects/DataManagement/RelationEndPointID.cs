using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public class RelationEndPointID
{
  // types

  // static members and constants

  // member fields

  private ClassDefinition _classDefinition;
  private string _propertyName;
  private ObjectID _objectID;

  // construction and disposing

  public RelationEndPointID (ObjectID objectID, IRelationEndPointDefinition definition) 
      : this (objectID, definition.PropertyName)
  {
  }

  public RelationEndPointID (ObjectID objectID, string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    ArgumentUtility.CheckNotNull ("objectID", objectID);

    // TODO: Check if objectID.ClassID and propertyName match!

    _classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (objectID.ClassID);
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

  public ClassDefinition ClassDefinition
  {
    get { return _classDefinition; }
  }

  public string PropertyName
  {
    get { return _propertyName; }
  }

  public ObjectID ObjectID
  {
    get { return _objectID; }
  }

  public IRelationEndPointDefinition Definition
  {
    get { return _classDefinition.GetMandatoryRelationEndPointDefinition (_propertyName); }
  }

  public IRelationEndPointDefinition OppositeEndPointDefinition
  {
    get { return _classDefinition.GetOppositeEndPointDefinition (_propertyName); }
  }
}
}