using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.Relations
{
public class CollectionEndPoint : RelationEndPoint
{
  // types

  // static members and constants

  // member fields

  private DomainObjectCollection _domainObjects;
  private ObjectEndPoint _oldRelatedEndPoint;
  private ObjectEndPoint _newRelatedEndPoint;

  // construction and disposing

  // TODO: New ctor parameter: domainObject (e.g. order for a orderItem collection)
  //       ctors with propertyName (see ObjectEndPoint)
  //       Make domainObjects optional and use ClientTx, if not provided
  public CollectionEndPoint (DomainObjectCollection domainObjects, VirtualRelationEndPointDefinition definition)
  {
    ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

    base.Initialize (definition);
    _domainObjects = domainObjects;
  }

  // methods and properties

  public DomainObjectCollection DomainObjects 
  {
    get { return _domainObjects; }
  }

  public override bool BeginRelationChange (ObjectEndPoint oldRelatedEndPoint, ObjectEndPoint newRelatedEndPoint)
  {
    _oldRelatedEndPoint = oldRelatedEndPoint;
    _newRelatedEndPoint = newRelatedEndPoint;

    if (_oldRelatedEndPoint.IsNull && !_newRelatedEndPoint.IsNull)
      return _domainObjects.BeginAdd (_newRelatedEndPoint.DomainObject);

    if (!_oldRelatedEndPoint.IsNull && _newRelatedEndPoint.IsNull)
      return _domainObjects.BeginRemove (_oldRelatedEndPoint.DomainObject);

    return true;
  }

  public override void EndRelationChange ()
  {
    if (_oldRelatedEndPoint.IsNull && !_newRelatedEndPoint.IsNull)
      _domainObjects.EndAdd (_newRelatedEndPoint.DomainObject);

    if (!_oldRelatedEndPoint.IsNull && _newRelatedEndPoint.IsNull)
      _domainObjects.EndRemove (_oldRelatedEndPoint.DomainObject);
  }

}
}
