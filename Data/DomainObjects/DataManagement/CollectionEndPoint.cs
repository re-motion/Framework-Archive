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

  // member fields

  private ICollectionEndPointChangeDelegate _changeDelegate = null;

  private DomainObjectCollection _originalOppositeDomainObjects;
  private DomainObjectCollection _oppositeDomainObjects;

  [NonSerialized]
  private CollectionEndPointChangeAgent _changeAgent;

  // construction and disposing

  public CollectionEndPoint (
      DomainObject domainObject, 
      VirtualRelationEndPointDefinition definition, 
      DomainObjectCollection oppositeDomainObjects) 
      : this (domainObject.DataContainer.ClientTransaction, domainObject.ID, definition, oppositeDomainObjects)
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
      : this (domainObject.DataContainer.ClientTransaction, domainObject.ID, propertyName, oppositeDomainObjects)
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
      _originalOppositeDomainObjects.Commit (_oppositeDomainObjects);
  }

  public override void Rollback ()
  {
    if (HasChanged)
      _oppositeDomainObjects.Rollback (_originalOppositeDomainObjects);
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
  }

  public override void PerformDelete ()
  {
    _oppositeDomainObjects.PerformDelete ();
  }

  public override void EndRelationChange ()
  {
    if (_changeAgent == null)
      throw new InvalidOperationException ("BeginRelationChange must be called before EndRelationChange.");

    base.EndRelationChange ();

    _changeAgent.EndRelationChange ();
    _changeAgent = null;
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

  private void BeginRelationChange ()
  {
    base.BeginRelationChange (_changeAgent.OldEndPoint, _changeAgent.NewEndPoint);
    _changeAgent.BeginRelationChange ();
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
