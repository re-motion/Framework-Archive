using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public class CollectionEndPoint : RelationEndPoint, ICollectionChangeDelegate
{
  // types

  private enum RelationChangeOperation
  {
    Unspecified = -1,
    Add = 0,
    Remove = 1,
    Insert = 2,
    Replace = 3
  }

  // static members and constants

  // member fields

  private ICollectionEndPointChangeDelegate _changeDelegate = null;

  private DomainObjectCollection _originalOppositeDomainObjects;
  private DomainObjectCollection _oppositeDomainObjects;

  private RelationChangeOperation _relationChangeOperation = RelationChangeOperation.Unspecified;
  private RelationEndPoint _oldEndPoint = null;
  private RelationEndPoint _newEndPoint = null;
  private int _index = -1;

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

    _relationChangeOperation = RelationChangeOperation.Unspecified;
    _oldEndPoint = oldEndPoint;
    _newEndPoint = newEndPoint;
    _index = -1;

    if (IsRemoveOperation ())
    {
      _relationChangeOperation = RelationChangeOperation.Remove;

      if (!_oppositeDomainObjects.BeginRemove (_oldEndPoint.GetDomainObject ()))
        return false;
    }

    if (IsAddOperation ())
    {
      _relationChangeOperation = RelationChangeOperation.Add;

      if (!_oppositeDomainObjects.BeginAdd (_newEndPoint.GetDomainObject ()))
        return false;
    }

    return base.BeginRelationChange (oldEndPoint, newEndPoint);
  }

  public bool BeginInsert (
      RelationEndPoint oldEndPoint, 
      RelationEndPoint newEndPoint,
      int index)
  {
    ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
    ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);

    bool result = BeginRelationChange (oldEndPoint, newEndPoint);

    _relationChangeOperation = RelationChangeOperation.Insert;
    _index = index;
  
    return result;
  }

  public bool BeginReplace (RelationEndPoint oldEndPoint, RelationEndPoint newEndPoint)
  {
    ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
    ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);

    bool result = BeginRelationChange (oldEndPoint, newEndPoint);

    _relationChangeOperation = RelationChangeOperation.Replace;
    _index = _oppositeDomainObjects.IndexOf (oldEndPoint.GetDomainObject ());

    return result;
  }

  public override void PerformRelationChange ()
  {
    if (_relationChangeOperation == RelationChangeOperation.Unspecified || _oldEndPoint == null || _newEndPoint == null)
      throw new InvalidOperationException ("BeginRelationChange must be called before PerformRelationChange.");

    if (_relationChangeOperation == RelationChangeOperation.Remove)
    {
      _oppositeDomainObjects.PerformRemove (_oldEndPoint.GetDomainObject ());
      return;
    }

    if (_relationChangeOperation == RelationChangeOperation.Add)
    {
      _oppositeDomainObjects.PerformAdd (_newEndPoint.GetDomainObject ());
      return;
    }

    if (_relationChangeOperation == RelationChangeOperation.Insert)
    {
      _oppositeDomainObjects.PerformInsert (_index, _newEndPoint.GetDomainObject ());
      return;
    }

    if (_relationChangeOperation == RelationChangeOperation.Replace)
    {
      _oppositeDomainObjects.PerformRemove (_oldEndPoint.GetDomainObject ());
      _oppositeDomainObjects.PerformInsert (_index, _newEndPoint.GetDomainObject ());
    }
  }

  public override void PerformDelete ()
  {
    _oppositeDomainObjects.ClearCollection ();
  }

  public override void EndRelationChange ()
  {
    if (_relationChangeOperation == RelationChangeOperation.Unspecified || _oldEndPoint == null || _newEndPoint == null)
      throw new InvalidOperationException ("BeginRelationChange must be called before EndRelationChange.");

    if (_relationChangeOperation == RelationChangeOperation.Remove)
      _oppositeDomainObjects.EndRemove (_oldEndPoint.GetDomainObject ());

    if (_relationChangeOperation == RelationChangeOperation.Insert || _relationChangeOperation == RelationChangeOperation.Add)
      _oppositeDomainObjects.EndAdd (_newEndPoint.GetDomainObject ());

    _relationChangeOperation = RelationChangeOperation.Unspecified;
    _oldEndPoint = null;
    _newEndPoint = null;
    _index = -1;

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

  private bool IsAddOperation ()
  {
    return (!_newEndPoint.IsNull);
  }

  private bool IsRemoveOperation ()
  {
    return (!_oldEndPoint.IsNull);
  }

  #region ICollectionChangeDelegate Members

  void ICollectionChangeDelegate.PerformAdd (DomainObjectCollection collection, DomainObject domainObject)
  {
    if (_changeDelegate == null)
      throw new DataManagementException ("Internal error: CollectionEndPoint must have an ILinkChangeDelegate registered.");

    _changeDelegate.PerformAdd (this, domainObject);
  }

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
