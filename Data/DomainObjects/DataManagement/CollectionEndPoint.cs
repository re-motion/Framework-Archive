using System;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

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

  private CollectionEndPointChangeWorker _changeWorker;

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

    _originalOppositeDomainObjects = oppositeDomainObjects.Clone (true);
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
      _originalOppositeDomainObjects = _oppositeDomainObjects.Clone (true);
  }

  public override void Rollback ()
  {
    if (HasChanged)
      _oppositeDomainObjects = _originalOppositeDomainObjects.Clone (_oppositeDomainObjects.IsReadOnly);
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

  public override bool BeginRelationChange (RelationEndPoint oldEndPoint, RelationEndPoint newEndPoint)
  {
    ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
    ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);

    if (oldEndPoint.IsNull && newEndPoint.IsNull)
      throw new ArgumentException ("Both endPoints cannot be NullEndPoints.", "oldEndPoint, newEndPoint");

    if (!oldEndPoint.IsNull && !newEndPoint.IsNull)
      throw new ArgumentException ("One endPoint must be a NullEndPoint.", "oldEndPoint, newEndPoint");

    if (!oldEndPoint.IsNull && newEndPoint.IsNull)
       _changeWorker = CollectionEndPointChangeWorker.CreateForRemove (_oppositeDomainObjects, oldEndPoint, newEndPoint);

    if (!newEndPoint.IsNull && oldEndPoint.IsNull)
      _changeWorker = CollectionEndPointChangeWorker.CreateForAdd (_oppositeDomainObjects, oldEndPoint, newEndPoint);

    return BeginRelationChange ();
  }

  public virtual bool BeginInsert (
      RelationEndPoint oldEndPoint, 
      RelationEndPoint newEndPoint,
      int index)
  {
    ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
    ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);

    _changeWorker = CollectionEndPointChangeWorker.CreateForInsert (_oppositeDomainObjects, oldEndPoint, newEndPoint, index);

    return BeginRelationChange ();
  }

  public virtual bool BeginReplace (RelationEndPoint oldEndPoint, RelationEndPoint newEndPoint)
  {
    ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
    ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);

    _changeWorker = CollectionEndPointChangeWorker.CreateForReplace (
        _oppositeDomainObjects, oldEndPoint, newEndPoint, _oppositeDomainObjects.IndexOf (oldEndPoint.GetDomainObject ()));

    return BeginRelationChange ();
  }

  public override void PerformRelationChange ()
  {
    if (_changeWorker == null)
      throw new InvalidOperationException ("BeginRelationChange must be called before PerformRelationChange.");

    _changeWorker.PerformRelationChange ();
  }

  public override void PerformDelete ()
  {
    _oppositeDomainObjects.ClearCollection ();
  }

  public override void EndRelationChange ()
  {
    if (_changeWorker == null)
      throw new InvalidOperationException ("BeginRelationChange must be called before EndRelationChange.");

    _changeWorker.EndRelationChange ();
    _changeWorker = null;

    base.EndRelationChange ();
  }

  public DomainObjectCollection OriginalOppositeDomainObjects
  {
    get { return _originalOppositeDomainObjects; }
  }

  public DomainObjectCollection OppositeDomainObjects
  {
    get { return _oppositeDomainObjects; }
  }

  public ICollectionEndPointChangeDelegate ChangeDelegate
  {
    set { _changeDelegate = value; }
  }

  private bool BeginRelationChange ()
  {
    if (!_changeWorker.BeginRelationChange ())
      return false;

    return base.BeginRelationChange (_changeWorker.OldEndPoint, _changeWorker.NewEndPoint);
  }

  #region ICollectionChangeDelegate Members

  void ICollectionChangeDelegate.PerformInsert (DomainObjectCollection collection, DomainObject domainObject, int index)
  {
    if (_changeDelegate == null)
      throw new DataManagementException ("Internal error: CollectionEndPoint must have an ILinkChangeDelegate registered.");

    _changeDelegate.PerformInsert (this, domainObject, index);
  }

  void ICollectionChangeDelegate.PerformReplace (DomainObjectCollection collection, DomainObject domainObject, int index)
  {
    if (_changeDelegate == null)
      throw new DataManagementException ("Internal error: CollectionEndPoint must have an ILinkChangeDelegate registered.");

    _changeDelegate.PerformReplace (this, domainObject, index);
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
