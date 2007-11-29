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
  private bool _hasBeenTouched;

  // construction and disposing

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
    _hasBeenTouched = false;
  }

  protected ObjectEndPoint (IRelationEndPointDefinition definition) : base (definition)
  {
    _hasBeenTouched = false;
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
    _hasBeenTouched = sourceObjectEndPoint._hasBeenTouched;
  }

  internal override void TakeOverCommittedData (RelationEndPoint source)
  {
    Assertion.IsTrue (Definition == source.Definition);

    ObjectEndPoint sourceObjectEndPoint = (ObjectEndPoint) source;

    _oppositeObjectID = sourceObjectEndPoint._oppositeObjectID;
    _hasBeenTouched |= sourceObjectEndPoint._hasBeenTouched || HasChanged; // true if: we have been touched/source has been touched/we have changed
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
      _hasBeenTouched = false;
    }
  }

  public override void Rollback ()
  {
    if (HasChanged)
    {
      _oppositeObjectID = _originalOppositeObjectID;
      _hasBeenTouched = false;
    }
  }

  public override bool HasChanged 
  {
    get
    {
      return !object.Equals (_oppositeObjectID, _originalOppositeObjectID);
    }
  }

  public override bool HasBeenTouched
  {
    get { return _hasBeenTouched; }
  }

  protected internal override void Touch ()
  {
    _hasBeenTouched = true;
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

  public override RelationEndPointModification CreateModification (IEndPoint oldEndPoint, IEndPoint newEndPoint)
  {
    return new ObjectEndPointModification (this, oldEndPoint, newEndPoint);
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

  public virtual void PerformRelationChange (ObjectEndPointModification modification)
  {
    ArgumentUtility.CheckNotNull ("modification", modification);

    OppositeObjectID = modification.NewEndPoint.ObjectID;

    if (!IsVirtual)
    {
      DataContainer dataContainer = GetDataContainer ();
      dataContainer.PropertyValues[PropertyName].SetRelationValue (modification.NewEndPoint.ObjectID);
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
      _hasBeenTouched = true;
    }
  }
}
}
