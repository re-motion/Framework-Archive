using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public class RelationEndPointID
{
  // types

  // static members and constants

  // member fields

  private IRelationEndPointDefinition _definition;
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

    ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (objectID.ClassID);
    _definition = classDefinition.GetMandatoryRelationEndPointDefinition (propertyName);
    _objectID = objectID;
  }

  // methods and properties

  public override int GetHashCode ()
  {
    return _objectID.GetHashCode () ^ PropertyName.GetHashCode ();
  }

  public override bool Equals (object obj)
  {
    RelationEndPointID endPointID = obj as RelationEndPointID;
    if (endPointID == null)
      return false;

    return this._objectID.Equals (endPointID.ObjectID)
        && this.PropertyName.Equals (endPointID.PropertyName);
  }

  public override string ToString ()
  {
    return string.Format ("{0}/{1}", _objectID.ToString (), PropertyName);
  }

  public IRelationEndPointDefinition Definition
  {
    get { return _definition; }
  }

  public string PropertyName
  {
    get { return _definition.PropertyName; }
  }

  public ClassDefinition ClassDefinition
  {
    get { return _definition.ClassDefinition; }
  }

  public IRelationEndPointDefinition OppositeEndPointDefinition
  {
    get { return ClassDefinition.GetOppositeEndPointDefinition (PropertyName); }
  }

  public RelationDefinition RelationDefinition
  {
    get { return ClassDefinition.GetRelationDefinition (PropertyName); }
  }

  public bool IsVirtual
  {
    get { return _definition.IsVirtual; }
  }

  public ObjectID ObjectID
  {
    get { return _objectID; }
  }
}
}