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

  // construction and disposing

  public ObjectEndPoint (
      ClientTransaction clientTransaction,
      DomainObject domainObject, 
      IRelationEndPointDefinition definition, 
      ObjectID oppositeObjectID) 
      : this (clientTransaction, domainObject.ID, definition, oppositeObjectID)
  {
  }

  public ObjectEndPoint (
      ClientTransaction clientTransaction,
      DataContainer dataContainer, 
      IRelationEndPointDefinition definition, 
      ObjectID oppositeObjectID) 
      : this (clientTransaction, dataContainer.ID, definition, oppositeObjectID)
  {
  }

  public ObjectEndPoint ( 
      ClientTransaction clientTransaction,
      DomainObject domainObject, 
      string propertyName,
      ObjectID oppositeObjectID) 
      : this (clientTransaction, domainObject.ID, propertyName, oppositeObjectID)
  {
  }

  public ObjectEndPoint (
      ClientTransaction clientTransaction,
      DataContainer dataContainer, 
      string propertyName,
      ObjectID oppositeObjectID) 
      : this (clientTransaction, dataContainer.ID, propertyName, oppositeObjectID)
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
      : base (clientTransaction, id)
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

    _oppositeObjectID = endPoint.ObjectID;

    if (!IsVirtual)
    {
      DataContainer dataContainer = GetDataContainer ();
      dataContainer.PropertyValues[PropertyName].SetRelationValue (endPoint.ObjectID);
    }
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

  #region ICloneable Members

  public override object Clone ()
  {
    ObjectEndPoint newObjectEndPoint = new ObjectEndPoint (this.ClientTransaction, this.ID, this.OppositeObjectID);
    newObjectEndPoint._originalOppositeObjectID = this._originalOppositeObjectID;

    return newObjectEndPoint;
  }

  #endregion
}
}
