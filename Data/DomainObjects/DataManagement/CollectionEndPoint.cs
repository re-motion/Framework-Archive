using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public class CollectionEndPoint : RelationEndPoint, ICollectionChangeDelegate
{
  // types

  // static members and constants

  // member fields

  private ICollectionEndPointChangeDelegate _changeDelegate = null;

  private DomainObjectCollection _originalOppositeDomainObjects;
  private DomainObjectCollection _oppositeDomainObjects;

  private RelationEndPoint _oldEndPoint;
  private RelationEndPoint _newEndPoint;

  // construction and disposing

  public CollectionEndPoint (
      ClientTransaction clientTransaction, 
      DomainObject domainObject, 
      VirtualRelationEndPointDefinition definition, 
      DomainObjectCollection oppositeDomainObjects) 
      : this (clientTransaction, domainObject.ID, definition, oppositeDomainObjects)
  {
  }

  public CollectionEndPoint (
      ClientTransaction clientTransaction,
      DataContainer dataContainer, 
      VirtualRelationEndPointDefinition definition, 
      DomainObjectCollection oppositeDomainObjects) 
      : this (clientTransaction, dataContainer.ID, definition, oppositeDomainObjects)
  {
  }

  public CollectionEndPoint (
      ClientTransaction clientTransaction,
      DomainObject domainObject, 
      string propertyName,
      DomainObjectCollection oppositeDomainObjects) 
      : this (clientTransaction, domainObject.ID, propertyName, oppositeDomainObjects)
  {
  }

  public CollectionEndPoint (
      ClientTransaction clientTransaction,
      DataContainer dataContainer, 
      string propertyName,
      DomainObjectCollection oppositeDomainObjects) 
      : this (clientTransaction, dataContainer.ID, propertyName, oppositeDomainObjects)
  {
  }

  public CollectionEndPoint (
      ClientTransaction clientTransaction,
      ObjectID objectID, 
      VirtualRelationEndPointDefinition definition, 
      DomainObjectCollection oppositeDomainObjects) 
      : this (clientTransaction, objectID, definition.PropertyName, oppositeDomainObjects)
  {
  }

  public CollectionEndPoint (
      ClientTransaction clientTransaction,
      ObjectID objectID, 
      string propertyName,
      DomainObjectCollection oppositeDomainObjects) 
      : this (clientTransaction, new RelationEndPointID (objectID, propertyName), oppositeDomainObjects)
  {
  }

  public CollectionEndPoint (
      ClientTransaction clientTransaction,
      RelationEndPointID id, 
      DomainObjectCollection oppositeDomainObjects) 
      : base (clientTransaction, id)
  {
    ArgumentUtility.CheckNotNull ("oppositeDomainObjects", oppositeDomainObjects);

    // TODO: Use DomainObjectCollection.Create instead to assure correct collection type => New overload necessary!
    _originalOppositeDomainObjects = new DomainObjectCollection (oppositeDomainObjects, true);
    _oppositeDomainObjects = oppositeDomainObjects;
    _oppositeDomainObjects.ChangeDelegate = this;
  }

  protected CollectionEndPoint (IRelationEndPointDefinition definition) : base (definition)
  {
  }

  // methods and properties

  public override void Commit ()
  {
    if (HasChanged)
    {
      // TODO: Use DomainObjectCollection.Create instead to assure correct collection type => New overload necessary!
      _originalOppositeDomainObjects = new DomainObjectCollection (_oppositeDomainObjects, true);
    }
  }

  public override void Rollback ()
  {
    if (HasChanged)
    {
      for (int i = _oppositeDomainObjects.Count - 1; i >= 0; i--)
        _oppositeDomainObjects.PerformRemove (_oppositeDomainObjects[i]);

      foreach (DomainObject domainObject in _originalOppositeDomainObjects)
        _oppositeDomainObjects.PerformAdd (domainObject);
    }
  }

  public override bool HasChanged
  {
    get { return !DomainObjectCollection.Compare (_oppositeDomainObjects, _originalOppositeDomainObjects); } 
  }

  public override void CheckMandatory ()
  {
    if (_oppositeDomainObjects.Count == 0)
    {
      throw CreateMandatoryRelationNotSetException (
          "Mandatory relation property '{0}' of domain object '{1}' contains no elements.", 
          PropertyName, 
          ObjectID);
    }
  }

  public DomainObjectCollection OriginalOppositeDomainObjects
  {
    get { return _originalOppositeDomainObjects; }
  }

  public DomainObjectCollection OppositeDomainObjects
  {
    get { return _oppositeDomainObjects; }
  }

  internal ICollectionEndPointChangeDelegate ChangeDelegate
  {
    set { _changeDelegate = value; }
  }

  public override bool BeginRelationChange (RelationEndPoint oldEndPoint, RelationEndPoint newEndPoint)
  {
    _oldEndPoint = oldEndPoint;
    _newEndPoint = newEndPoint;

    if (IsAddOperation () && !_oppositeDomainObjects.BeginAdd (_newEndPoint.GetDomainObject ()))
      return false;

    if (IsRemoveOperation () && !_oppositeDomainObjects.BeginRemove (_oldEndPoint.GetDomainObject ()))
      return false;

    return base.BeginRelationChange (oldEndPoint, newEndPoint);
  }

  public override void EndRelationChange ()
  {
    if (_oldEndPoint == null || _newEndPoint == null)
      throw new InvalidOperationException ("BeginRelationChange must be called before EndRelationChange.");

    if (IsAddOperation ())
      _oppositeDomainObjects.EndAdd (_newEndPoint.GetDomainObject ());

    if (IsRemoveOperation ())
      _oppositeDomainObjects.EndRemove (_oldEndPoint.GetDomainObject ());

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

  #region ICollectionChangeDelegate Members

  void ICollectionChangeDelegate.PerformAdd (DomainObjectCollection collection, DomainObject domainObject)
  {
    if (_changeDelegate == null)
      throw new DataManagementException ("Internal error: CollectionEndPoint must have an ILinkChangeDelegate registered.");

    _changeDelegate.PerformAdd (this, domainObject);
  }

  void ICollectionChangeDelegate.PerformRemove (DomainObjectCollection collection, DomainObject domainObject)
  {
    if (_changeDelegate == null)
      throw new DataManagementException ("Internal error: CollectionEndPoint must have an ILinkChangeDelegate registered.");

    _changeDelegate.PerformRemove (this, domainObject);
  }

  #endregion
}
}
