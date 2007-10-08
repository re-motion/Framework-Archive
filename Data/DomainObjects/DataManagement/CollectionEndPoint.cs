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

  private readonly DomainObjectCollection _originalOppositeDomainObjects;
  private readonly DomainObjectCollection _oppositeDomainObjects;

  private bool _hasBeenTouched;

  [NonSerialized]
  private CollectionEndPointChangeAgent _changeAgent;

  // construction and disposing

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
    _hasBeenTouched = false;
  }

  protected CollectionEndPoint (IRelationEndPointDefinition definition) : base (definition)
  {
    _hasBeenTouched = false;
  }

  // methods and properties

  public override RelationEndPoint Clone ()
  {
    CollectionEndPoint clone = new CollectionEndPoint (
        ClientTransaction, ID, DomainObjectCollection.Create (_oppositeDomainObjects.GetType(), _oppositeDomainObjects.RequiredItemType));
    clone.AssumeSameState (this);
    clone.ChangeDelegate = ChangeDelegate;

    return clone;
  }

  internal override void AssumeSameState (RelationEndPoint source)
  {
    Assertion.IsTrue (Definition == source.Definition);

    CollectionEndPoint sourceCollectionEndPoint = (CollectionEndPoint) source;

    _oppositeDomainObjects.AssumeSameState (sourceCollectionEndPoint._oppositeDomainObjects);
    _originalOppositeDomainObjects.AssumeSameState (sourceCollectionEndPoint._originalOppositeDomainObjects);
    _hasBeenTouched = sourceCollectionEndPoint._hasBeenTouched;
  }

  internal override void TakeOverCommittedData (RelationEndPoint source)
  {
    Assertion.IsTrue (Definition == source.Definition);

    CollectionEndPoint sourceCollectionEndPoint = (CollectionEndPoint) source;

    _oppositeDomainObjects.TakeOverCommittedData (sourceCollectionEndPoint._oppositeDomainObjects);
    _hasBeenTouched |= sourceCollectionEndPoint._hasBeenTouched || HasChanged;
  }

  internal override void RegisterWithMap (RelationEndPointMap map)
  {
    ChangeDelegate = map;
  }

  public override void Commit ()
  {
    if (HasChanged)
      _originalOppositeDomainObjects.Commit (_oppositeDomainObjects);

    _hasBeenTouched = false;
  }

  public override void Rollback ()
  {
    if (HasChanged)
      _oppositeDomainObjects.Rollback (_originalOppositeDomainObjects);

    _hasBeenTouched = false;
  }

  public override bool HasChanged
  {
    get { return !DomainObjectCollection.Compare (_oppositeDomainObjects, _originalOppositeDomainObjects, true); } 
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
    _hasBeenTouched = true;
  }

  public override void PerformDelete ()
  {
    _oppositeDomainObjects.PerformDelete ();
    _hasBeenTouched = true;
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
    _hasBeenTouched = true;
  }

  void ICollectionChangeDelegate.PerformReplace (DomainObjectCollection collection, DomainObject domainObject, int index)
  {
    if (_changeDelegate == null)
      throw new DataManagementException ("Internal error: CollectionEndPoint must have an ILinkChangeDelegate registered.");

    _changeDelegate.PerformReplace (this, domainObject, index);
    _hasBeenTouched = true;
  }

  void ICollectionChangeDelegate.PerformSelfReplace (DomainObjectCollection collection, DomainObject domainObject, int index)
  {
    if (_changeDelegate == null)
      throw new DataManagementException ("Internal error: CollectionEndPoint must have an ILinkChangeDelegate registered.");

    _changeDelegate.PerformSelfReplace (this, domainObject, index);
    _hasBeenTouched = true;
  }

  void ICollectionChangeDelegate.PerformRemove (DomainObjectCollection collection, DomainObject domainObject)
  {
    if (_changeDelegate == null)
      throw new DataManagementException ("Internal error: CollectionEndPoint must have an ILinkChangeDelegate registered.");

    _changeDelegate.PerformRemove (this, domainObject);
    _hasBeenTouched = true;
  }

  void ICollectionChangeDelegate.MarkAsTouched ()
  {
    _hasBeenTouched = true;
  }

  #endregion
}
}
