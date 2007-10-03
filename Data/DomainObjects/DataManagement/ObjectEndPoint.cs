using System;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.DataManagement
{
[Serializable]
public class ObjectEndPoint : RelationEndPoint, INullObject
{
  // types

  // static members and constants

  // member fields

  private ObjectID _originalOppositeObjectID;
  private ObjectID _oppositeObjectID;
  private IEndPoint _newEndPoint;
  private bool _hasChanged;

  // construction and disposing

  public ObjectEndPoint (
      DomainObject domainObject, 
      IRelationEndPointDefinition definition, 
      ObjectID oppositeObjectID) 
      : this (domainObject.GetDataContainer().ClientTransaction, domainObject.ID, definition, oppositeObjectID)
  {
  }

  public ObjectEndPoint (
      DataContainer dataContainer, 
      IRelationEndPointDefinition definition, 
      ObjectID oppositeObjectID) 
      : this (dataContainer.ClientTransaction, dataContainer.ID, definition, oppositeObjectID)
  {
  }

  public ObjectEndPoint ( 
      DomainObject domainObject, 
      string propertyName,
      ObjectID oppositeObjectID) 
      : this (domainObject.GetDataContainer().ClientTransaction, domainObject.ID, propertyName, oppositeObjectID)
  {
  }

  public ObjectEndPoint (
      DataContainer dataContainer, 
      string propertyName,
      ObjectID oppositeObjectID) 
      : this (dataContainer.ClientTransaction, dataContainer.ID, propertyName, oppositeObjectID)
  {
  }

  public ObjectEndPoint (
      ClientTransaction clientTransaction,
      ObjectID objectID, 
      IRelationEndPointDefinition definition, 
      ObjectID oppositeObjectID) 
      : this (clientTransaction, objectID, definition.PropertyName, oppositeObjectID)
  {
  }

  public ObjectEndPoint (
      ClientTransaction clientTransaction,
      ObjectID objectID, 
      string propertyName,
      ObjectID oppositeObjectID) 
      : this (clientTransaction, new RelationEndPointID (objectID, propertyName), oppositeObjectID)
  {
  }

  public ObjectEndPoint (
      ClientTransaction clientTransaction, 
      RelationEndPointID id, 
      ObjectID oppositeObjectID) 
    : this (clientTransaction, id, oppositeObjectID, oppositeObjectID)
  {
  }

  private ObjectEndPoint (
      ClientTransaction clientTransaction,
      RelationEndPointID id,
      ObjectID oppositeObjectID,
      ObjectID originalOppositeObjectID) 
    : base (clientTransaction, id)
  {
    _oppositeObjectID = oppositeObjectID;
    _originalOppositeObjectID = originalOppositeObjectID;
    _hasChanged = false;
  }

  protected ObjectEndPoint (IRelationEndPointDefinition definition) : base (definition)
  {
    _hasChanged = false;
  }

  // methods and properties

  public override RelationEndPoint Clone ()
  {
    ObjectEndPoint clone = new ObjectEndPoint (ClientTransaction, ID, null);
    clone.AssumeSameState (this);
    return clone;
  }

  internal override void AssumeSameState (RelationEndPoint source)
  {
    Assertion.IsTrue (Definition == source.Definition);

    ObjectEndPoint sourceObjectEndPoint = (ObjectEndPoint)source;

    _oppositeObjectID = sourceObjectEndPoint._oppositeObjectID;
    _originalOppositeObjectID = sourceObjectEndPoint._originalOppositeObjectID;
    _hasChanged = sourceObjectEndPoint._hasChanged;
  }

  internal override void MergeData (RelationEndPoint source)
  {
    Assertion.IsTrue (Definition == source.Definition);

    ObjectEndPoint sourceObjectEndPoint = (ObjectEndPoint) source;

    _oppositeObjectID = sourceObjectEndPoint._oppositeObjectID;
    _hasChanged |= sourceObjectEndPoint._hasChanged;
  }

  internal override void RegisterWithMap (RelationEndPointMap map)
  {
    // nothing to do here
  }

  public override void Commit ()
  {
    if (HasChanged)
    {
      _originalOppositeObjectID = _oppositeObjectID;
      _hasChanged = false;
    }
  }

  public override void Rollback ()
  {
    if (HasChanged)
    {
      _oppositeObjectID = _originalOppositeObjectID;
      _hasChanged = false;
    }
  }

  public override bool HasChanged 
  {
    get
    {
      return _hasChanged;
    }
  }

  public override void CheckMandatory ()
  {
    if (_oppositeObjectID == null)
    {
      throw CreateMandatoryRelationNotSetException (
          GetDomainObject (),
          PropertyName, 
          "Mandatory relation property '{0}' of domain object '{1}' cannot be null.", PropertyName, ObjectID);
    }    
  }

  public override void BeginRelationChange (IEndPoint oldEndPoint, IEndPoint newEndPoint)
  {
    ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
    ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);

    base.BeginRelationChange (oldEndPoint, newEndPoint);
    _newEndPoint = newEndPoint;
  }

  public override void PerformRelationChange ()
  {
    if (_newEndPoint == null)
      throw new InvalidOperationException ("BeginRelationChange must be called before PerformRelationChange.");

    PerformRelationChange (_newEndPoint);
  }

  public virtual void PerformRelationChange (IEndPoint endPoint)
  {
    ArgumentUtility.CheckNotNull ("endPoint", endPoint);

    OppositeObjectID = endPoint.ObjectID;

    if (!IsVirtual)
    {
      DataContainer dataContainer = GetDataContainer ();
      dataContainer.PropertyValues[PropertyName].SetRelationValue (endPoint.ObjectID);
    }
  }

  public override void PerformDelete ()
  {
    PerformRelationChange (RelationEndPoint.CreateNullRelationEndPoint (OppositeEndPointDefinition));
  }

  public override void EndRelationChange ()
  {
    if (_newEndPoint == null)
      throw new InvalidOperationException ("BeginRelationChange must be called before EndRelationChange.");

    _newEndPoint = null;

    base.EndRelationChange ();
  }

  public ObjectID OriginalOppositeObjectID
  {
    get { return _originalOppositeObjectID; }
  }

  public ObjectID OppositeObjectID
  {
    get { return _oppositeObjectID; }
    set
    {
      _oppositeObjectID = value;
      _hasChanged = true;
    }
  }
}
}
