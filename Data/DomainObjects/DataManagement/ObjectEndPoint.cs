using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public class ObjectEndPoint : RelationEndPoint, INullable
{
  // types

  // static members and constants

  // member fields

  private ObjectID _originalOppositeObjectID;
  private ObjectID _oppositeObjectID;
  private RelationEndPoint _newEndPoint;

  // construction and disposing

  public ObjectEndPoint (
      DomainObject domainObject, 
      IRelationEndPointDefinition definition, 
      ObjectID oppositeObjectID) 
      : this (domainObject.ID, definition, oppositeObjectID)
  {
  }

  public ObjectEndPoint (
      DataContainer dataContainer, 
      IRelationEndPointDefinition definition, 
      ObjectID oppositeObjectID) 
      : this (dataContainer.ID, definition, oppositeObjectID)
  {
  }

  public ObjectEndPoint (
      DomainObject domainObject, 
      string propertyName,
      ObjectID oppositeObjectID) 
      : this (domainObject.ID, propertyName, oppositeObjectID)
  {
  }

  public ObjectEndPoint (
      DataContainer dataContainer, 
      string propertyName,
      ObjectID oppositeObjectID) 
      : this (dataContainer.ID, propertyName, oppositeObjectID)
  {
  }

  public ObjectEndPoint (
      ObjectID objectID, 
      IRelationEndPointDefinition definition, 
      ObjectID oppositeObjectID) 
      : this (objectID, definition.PropertyName, oppositeObjectID)
  {
  }

  public ObjectEndPoint (
      ObjectID objectID, 
      string propertyName,
      ObjectID oppositeObjectID) 
      : this (new RelationEndPointID (objectID, propertyName), oppositeObjectID)
  {
  }

  public ObjectEndPoint (RelationEndPointID id, ObjectID oppositeObjectID) : base (id)
  {
    _oppositeObjectID = oppositeObjectID;
    _originalOppositeObjectID = oppositeObjectID;
  }

  protected ObjectEndPoint (IRelationEndPointDefinition definition) : base (definition)
  {
  }

  // methods and properties

  public override void Commit ()
  {
    if (HasChanged)
      _originalOppositeObjectID = _oppositeObjectID;
  }

  public override void Rollback ()
  {
    if (HasChanged)
      _oppositeObjectID = _originalOppositeObjectID;
  }

  public override bool HasChanged 
  {
    get
    {
      if (_oppositeObjectID == null && _originalOppositeObjectID == null) 
        return false;

      if (_oppositeObjectID == null) 
        return true;

      return !_oppositeObjectID.Equals (_originalOppositeObjectID);
    }
  }

  public override void CheckMandatory ()
  {
    if (_oppositeObjectID == null)
    {
      throw CreateMandatoryRelationNotSetException (
          "Mandatory relation property '{0}' of domain object '{1}' cannot be null.", PropertyName, ObjectID);
    }    
  }

  public virtual void SetOppositeEndPoint (RelationEndPoint endPoint)
  {
    ArgumentUtility.CheckNotNull ("endPoint", endPoint);

    if (!IsVirtual)
      DataContainer.PropertyValues[PropertyName].SetRelationValue (endPoint.ObjectID);
  }

  public override bool BeginRelationChange (RelationEndPoint oldEndPoint, RelationEndPoint newEndPoint)
  {
    _newEndPoint = newEndPoint;

    return base.BeginRelationChange (oldEndPoint, newEndPoint);
  }

  public override void EndRelationChange ()
  {
    if (_newEndPoint == null)
      throw new InvalidOperationException ("BeginRelationChange must be called before EndRelationChange.");

    base.EndRelationChange ();

    _oppositeObjectID = _newEndPoint.ObjectID;
  }

  public ObjectID OriginalOppositeObjectID
  {
    get { return _originalOppositeObjectID; }
  }

  public ObjectID OppositeObjectID
  {
    get { return _oppositeObjectID; }
    set { _oppositeObjectID = value; }
  }
}
}
