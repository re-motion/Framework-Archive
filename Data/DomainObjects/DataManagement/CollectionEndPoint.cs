using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public class CollectionEndPoint : RelationEndPoint
{
  // types

  // static members and constants

  // member fields

  private DomainObjectCollection _oppositeDomainObjects;
  private RelationEndPoint _oldEndPoint;
  private RelationEndPoint _newEndPoint;

  // construction and disposing

  public CollectionEndPoint (
      DomainObject domainObject, 
      VirtualRelationEndPointDefinition definition, 
      DomainObjectCollection oppositeDomainObjects) 
      : this (domainObject.ID, definition, oppositeDomainObjects)
  {
  }

  public CollectionEndPoint (
      DataContainer dataContainer, 
      VirtualRelationEndPointDefinition definition, 
      DomainObjectCollection oppositeDomainObjects) 
      : this (dataContainer.ID, definition, oppositeDomainObjects)
  {
  }

  public CollectionEndPoint (
      DomainObject domainObject, 
      string propertyName,
      DomainObjectCollection oppositeDomainObjects) 
      : this (domainObject.ID, propertyName, oppositeDomainObjects)
  {
  }

  public CollectionEndPoint (
      DataContainer dataContainer, 
      string propertyName,
      DomainObjectCollection oppositeDomainObjects) 
      : this (dataContainer.ID, propertyName, oppositeDomainObjects)
  {
  }

  public CollectionEndPoint (
      ObjectID objectID, 
      VirtualRelationEndPointDefinition definition, 
      DomainObjectCollection oppositeDomainObjects) 
      : this (objectID, definition.PropertyName, oppositeDomainObjects)
  {
  }

  public CollectionEndPoint (
      ObjectID objectID, 
      string propertyName,
      DomainObjectCollection oppositeDomainObjects) 
      : this (new RelationEndPointID (objectID, propertyName), oppositeDomainObjects)
  {
  }

  public CollectionEndPoint (RelationEndPointID id, DomainObjectCollection oppositeDomainObjects) : base (id)
  {
    ArgumentUtility.CheckNotNull ("oppositeDomainObjects", oppositeDomainObjects);
    _oppositeDomainObjects = oppositeDomainObjects;
  }

  // methods and properties

  public DomainObjectCollection OppositeDomainObjects 
  {
    get { return _oppositeDomainObjects; }
  }

  public override bool BeginRelationChange (RelationEndPoint oldEndPoint, RelationEndPoint newEndPoint)
  {
    _oldEndPoint = oldEndPoint;
    _newEndPoint = newEndPoint;

    if (IsAddOperation () && !_oppositeDomainObjects.BeginAdd (_newEndPoint.DomainObject))
      return false;

    if (IsRemoveOperation () && !_oppositeDomainObjects.BeginRemove (_oldEndPoint.DomainObject))
      return false;

    return base.BeginRelationChange (oldEndPoint, newEndPoint);
  }

  public override void EndRelationChange ()
  {
    if (_oldEndPoint == null || _newEndPoint == null)
      throw new InvalidOperationException ("BeginRelationChange must be called before EndRelationChange.");

    if (IsAddOperation ())
      _oppositeDomainObjects.EndAdd (_newEndPoint.DomainObject);

    if (IsRemoveOperation ())
      _oppositeDomainObjects.EndRemove (_oldEndPoint.DomainObject);

    base.EndRelationChange ();
  }

  private bool IsAddOperation ()
  {
    return (_oldEndPoint.IsNull && !_newEndPoint.IsNull);
  }

  private bool IsRemoveOperation ()
  {
    return (!_oldEndPoint.IsNull && _newEndPoint.IsNull);
  }
}
}
