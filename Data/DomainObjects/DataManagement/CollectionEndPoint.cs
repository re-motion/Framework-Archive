using System;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.DataManagement
{
[Serializable]
public class CollectionEndPoint : RelationEndPoint, ICollectionChangeDelegate
{
  // types

  // static members and constants

  // only used for cloning, never returned to the outside
  private static readonly DomainObjectCollection s_emptyCollectionForCloning = new DomainObjectCollection ();

  private static DomainObjectCollection CloneDomainObjectCollection (DomainObjectCollection domainObjects, bool makeReadOnly)
  {
    ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

    return domainObjects.Clone (makeReadOnly);
  }

  // member fields

  private ICollectionEndPointChangeDelegate _changeDelegate = null;

  private DomainObjectCollection _originalOppositeDomainObjects;
  private DomainObjectCollection _oppositeDomainObjects;
  // TODO: private bool _hasBeenTouched;

  [NonSerialized]
  private CollectionEndPointChangeAgent _changeAgent;

  // construction and disposing

  public CollectionEndPoint (
      DomainObject domainObject, 
      VirtualRelationEndPointDefinition definition, 
      DomainObjectCollection oppositeDomainObjects) 
      : this (domainObject.GetDataContainer().ClientTransaction, domainObject.ID, definition, oppositeDomainObjects)
  {
  }

  public CollectionEndPoint (
      DataContainer dataContainer, 
      VirtualRelationEndPointDefinition definition, 
      DomainObjectCollection oppositeDomainObjects) 
      : this (dataContainer.ClientTransaction, dataContainer.ID, definition, oppositeDomainObjects)
  {
  }

  public CollectionEndPoint (
      DomainObject domainObject, 
      string propertyName,
      DomainObjectCollection oppositeDomainObjects) 
      : this (domainObject.GetDataContainer().ClientTransaction, domainObject.ID, propertyName, oppositeDomainObjects)
  {
  }

  public CollectionEndPoint (
      DataContainer dataContainer, 
      string propertyName,
      DomainObjectCollection oppositeDomainObjects) 
      : this (dataContainer.ClientTransaction, dataContainer.ID, propertyName, oppositeDomainObjects)
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
    : this (clientTransaction, id, oppositeDomainObjects, CloneDomainObjectCollection (oppositeDomainObjects, true))
  {
  }

  private CollectionEndPoint (
      ClientTransaction clientTransaction,
      RelationEndPointID id, 
      DomainObjectCollection oppositeDomainObjects,
      DomainObjectCollection originalOppositeDomainObjects) 
      : base (clientTransaction, id)
  {
    ArgumentUtility.CheckNotNull ("oppositeDomainObjects", oppositeDomainObjects);
    ArgumentUtility.CheckNotNull ("originalOppositeDomainObjects", originalOppositeDomainObjects);

    _originalOppositeDomainObjects = originalOppositeDomainObjects;
    _oppositeDomainObjects = oppositeDomainObjects;
    _oppositeDomainObjects.ChangeDelegate = this;
    // TODO: _hasBeenTouched = false;
  }

  protected CollectionEndPoint (IRelationEndPointDefinition definition) : base (definition)
  {
    // TODO: _hasBeenTouched = false;
  }

  // methods and properties

  public override RelationEndPoint Clone ()
  {
    CollectionEndPoint clone = new CollectionEndPoint (ClientTransaction, ID, s_emptyCollectionForCloning);
    clone.AssumeSameState (this);
    clone.ChangeDelegate = ChangeDelegate;

    return clone;
  }

  internal override void AssumeSameState (RelationEndPoint source)
  {
    Assertion.IsTrue (Definition == source.Definition);

    CollectionEndPoint sourceCollectionEndPoint = (CollectionEndPoint) source;

    _oppositeDomainObjects = sourceCollectionEndPoint._oppositeDomainObjects.Clone();
    _oppositeDomainObjects.ChangeDelegate = this;
    _originalOppositeDomainObjects = sourceCollectionEndPoint._originalOppositeDomainObjects.Clone();
    // TODO: _hasBeenTouched = sourceCollectionEndPoint._hasBeenTouched;
  }

  internal override void MergeData (RelationEndPoint source)
  {
    Assertion.IsTrue (Definition == source.Definition);

    CollectionEndPoint sourceCollectionEndPoint = (CollectionEndPoint) source;

    _oppositeDomainObjects = sourceCollectionEndPoint._oppositeDomainObjects.Clone ();
    _oppositeDomainObjects.ChangeDelegate = this;
    // TODO: _hasBeenTouched |= sourceCollectionEndPoint._hasBeenTouched;
  }

  internal override void RegisterWithMap (RelationEndPointMap map)
  {
    ChangeDelegate = map;
  }

  public override void Commit ()
  {
    if (HasChanged)
    {
      _originalOppositeDomainObjects.Commit (_oppositeDomainObjects);
      // TODO: _hasBeenTouched = false;
    }
  }

  public override void Rollback ()
  {
    if (HasChanged)
    {
      _oppositeDomainObjects.Rollback (_originalOppositeDomainObjects);
      // TODO: _hasBeenTouched = false;
    }
  }

  public override bool HasChanged
  {
    get { return !DomainObjectCollection.Compare (_oppositeDomainObjects, _originalOppositeDomainObjects, true); } 
  }

  public override void CheckMandatory ()
  {
    if (_oppositeDomainObjects.Count == 0)
    {
      throw CreateMandatoryRelationNotSetException (
          GetDomainObject (),
          PropertyName,
          "Mandatory relation property '{0}' of domain object '{1}' contains no items.", 
          PropertyName, 
          ObjectID);
    }
  }

  public override void BeginRelationChange (IEndPoint oldEndPoint, IEndPoint newEndPoint)
  {
    ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
    ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);

    if (oldEndPoint.IsNull && newEndPoint.IsNull)
      throw new ArgumentException ("Both endPoints cannot be NullEndPoints.", "oldEndPoint, newEndPoint");

    if (!oldEndPoint.IsNull && !newEndPoint.IsNull)
      throw new ArgumentException ("One endPoint must be a NullEndPoint.", "oldEndPoint, newEndPoint");

    if (!oldEndPoint.IsNull && newEndPoint.IsNull)
       _changeAgent = CollectionEndPointChangeAgent.CreateForRemove (_oppositeDomainObjects, oldEndPoint, newEndPoint);

    if (!newEndPoint.IsNull && oldEndPoint.IsNull)
      _changeAgent = CollectionEndPointChangeAgent.CreateForAdd (_oppositeDomainObjects, oldEndPoint, newEndPoint);

    BeginRelationChange ();
  }

  public virtual void BeginInsert (IEndPoint oldEndPoint, IEndPoint newEndPoint, int index)
  {
    ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
    ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);

    _changeAgent = CollectionEndPointChangeAgent.CreateForInsert (_oppositeDomainObjects, oldEndPoint, newEndPoint, index);

    BeginRelationChange ();
  }

  public virtual void BeginReplace (IEndPoint oldEndPoint, IEndPoint newEndPoint)
  {
    ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
    ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);

    _changeAgent = CollectionEndPointChangeAgent.CreateForReplace (
        _oppositeDomainObjects, oldEndPoint, newEndPoint, _oppositeDomainObjects.IndexOf (oldEndPoint.GetDomainObject ()));

    BeginRelationChange ();
  }

  public override void PerformRelationChange ()
  {
    if (_changeAgent == null)
      throw new InvalidOperationException ("BeginRelationChange must be called before PerformRelationChange.");

    _changeAgent.PerformRelationChange ();
    // TODO: _hasBeenTouched = true;
  }

  public override void PerformDelete ()
  {
    _oppositeDomainObjects.PerformDelete ();
    // TODO: _hasBeenTouched = true;
  }

  public override void EndRelationChange ()
  {
    if (_changeAgent == null)
      throw new InvalidOperationException ("BeginRelationChange must be called before EndRelationChange.");

    _changeAgent.EndRelationChange ();
    _changeAgent = null;

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
    get { return _changeDelegate; }
    set { _changeDelegate = value; }
  }

  private void BeginRelationChange ()
  {
    _changeAgent.BeginRelationChange ();
    base.BeginRelationChange (_changeAgent.OldEndPoint, _changeAgent.NewEndPoint);
  }

  #region ICollectionChangeDelegate Members

  void ICollectionChangeDelegate.PerformInsert (DomainObjectCollection collection, DomainObject domainObject, int index)
  {
    if (_changeDelegate == null)
      throw new DataManagementException ("Internal error: CollectionEndPoint must have an ILinkChangeDelegate registered.");

    _changeDelegate.PerformInsert (this, domainObject, index);
    // TODO: _hasBeenTouched = true;
  }

  void ICollectionChangeDelegate.PerformReplace (DomainObjectCollection collection, DomainObject domainObject, int index)
  {
    if (_changeDelegate == null)
      throw new DataManagementException ("Internal error: CollectionEndPoint must have an ILinkChangeDelegate registered.");

    _changeDelegate.PerformReplace (this, domainObject, index);
    // TODO: _hasBeenTouched = true;
  }

  void ICollectionChangeDelegate.PerformRemove (DomainObjectCollection collection, DomainObject domainObject)
  {
    if (_changeDelegate == null)
      throw new DataManagementException ("Internal error: CollectionEndPoint must have an ILinkChangeDelegate registered.");

    _changeDelegate.PerformRemove (this, domainObject);
    // TODO: _hasBeenTouched = true;
  }

  #endregion
}
}
