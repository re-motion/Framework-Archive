using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;

namespace Rubicon.Data.DomainObjects.Relations
{
public class ObjectEndPoint : RelationEndPoint
{
  // types

  // static members and constants

  // member fields

  private DataContainer _oppositeDataContainer;
  private RelationEndPoint _newEndPoint;

  // construction and disposing

  // TODO: New ctor parameter: destinationDomainObject
  //       Make destinationDomainObject optional and use ClientTx, if not provided

  public ObjectEndPoint (
      DomainObject domainObject, 
      IRelationEndPointDefinition definition, 
      DomainObject oppositeDomainObject) 
      : this (domainObject.DataContainer, definition, oppositeDomainObject.DataContainer)
  {
  }

  public ObjectEndPoint (
      DataContainer dataContainer, 
      IRelationEndPointDefinition definition, 
      DataContainer oppositeDataContainer) 
      : this (dataContainer, definition.PropertyName, oppositeDataContainer)
  {
  }

  public ObjectEndPoint (
      DomainObject domainObject, 
      string propertyName,
      DomainObject oppositeDomainObject) 
      : this (domainObject.DataContainer, propertyName, oppositeDomainObject.DataContainer)
  {
  }

  public ObjectEndPoint (
      DataContainer dataContainer, 
      string propertyName,
      DataContainer oppositeDataContainer) 
      : base (dataContainer, propertyName)
  {
    ArgumentUtility.CheckNotNull ("oppositeDataContainer", oppositeDataContainer);
    _oppositeDataContainer = oppositeDataContainer;
  }

  // methods and properties

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

    _oppositeDataContainer = _newEndPoint.DataContainer;
  }

  public DataContainer OppositeDataContainer
  {
    get { return _oppositeDataContainer; }
  }

  public DomainObject OppositeDomainObject 
  {
    get { return _oppositeDataContainer.DomainObject; }
  }
}
}
