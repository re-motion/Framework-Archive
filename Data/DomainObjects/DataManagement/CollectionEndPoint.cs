using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.Relations
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

  // TODO: New ctor parameter: domainObject (e.g. order for a orderItem collection)
  //       ctors with propertyName (see ObjectEndPoint)
  //       Make domainObjects optional and use ClientTx, if not provided
  

  public CollectionEndPoint (
      DomainObject domainObject, 
      IRelationEndPointDefinition definition, 
      DomainObjectCollection oppositeDomainObjects) 
      : this (domainObject.DataContainer, definition, oppositeDomainObjects)
  {
  }

  public CollectionEndPoint (
      DataContainer dataContainer, 
      IRelationEndPointDefinition definition, 
      DomainObjectCollection oppositeDomainObjects) 
      : this (dataContainer, definition.PropertyName, oppositeDomainObjects)
  {
  }

  public CollectionEndPoint (
      DomainObject domainObject, 
      string propertyName,
      DomainObjectCollection oppositeDomainObjects) 
      : this (domainObject.DataContainer, propertyName, oppositeDomainObjects)
  {
  }

  public CollectionEndPoint (
      DataContainer dataContainer, 
      string propertyName,
      DomainObjectCollection oppositeDomainObjects) 
      : base (dataContainer, propertyName)
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
