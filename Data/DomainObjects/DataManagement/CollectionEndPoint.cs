using System;
using System.Reflection;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Reflection;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.DataManagement
{
  [Serializable]
  public class CollectionEndPoint : RelationEndPoint, ICollectionChangeDelegate
  {
    // types

    // static members and constants

    private static DomainObjectCollection CloneDomainObjectCollection (DomainObjectCollection domainObjects, bool makeReadOnly)
    {
      ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

      return domainObjects.Clone (makeReadOnly);
    }

    // member fields

    // this field is not serialized via IFlattenedSerializable.SerializeIntoFlatStructure
    private ICollectionEndPointChangeDelegate _changeDelegate = null;

    private readonly DomainObjectCollection _originalOppositeDomainObjects;
    private readonly DomainObjectCollection _oppositeDomainObjects;

    private bool _hasBeenTouched;

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

    protected CollectionEndPoint (IRelationEndPointDefinition definition)
        : base (definition)
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

    protected internal override void AssumeSameState (RelationEndPoint source)
    {
      Assertion.IsTrue (Definition == source.Definition);

      CollectionEndPoint sourceCollectionEndPoint = (CollectionEndPoint) source;

      _oppositeDomainObjects.AssumeSameState (sourceCollectionEndPoint._oppositeDomainObjects);
      _originalOppositeDomainObjects.AssumeSameState (sourceCollectionEndPoint._originalOppositeDomainObjects);
      _hasBeenTouched = sourceCollectionEndPoint._hasBeenTouched;
    }

    protected internal override void TakeOverCommittedData (RelationEndPoint source)
    {
      Assertion.IsTrue (Definition == source.Definition);

      CollectionEndPoint sourceCollectionEndPoint = (CollectionEndPoint) source;

      _oppositeDomainObjects.TakeOverCommittedData (sourceCollectionEndPoint._oppositeDomainObjects);
      _hasBeenTouched |= sourceCollectionEndPoint._hasBeenTouched || HasChanged;
    }

    protected internal override void RegisterWithMap (RelationEndPointMap map)
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
      get { return ClientTransaction.HasCollectionEndPointChanged (this); }
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
            GetDomainObject(),
            PropertyName,
            "Mandatory relation property '{0}' of domain object '{1}' contains no items.",
            PropertyName,
            ObjectID);
      }
    }

    public override RelationEndPointModification CreateModification (IEndPoint oldEndPoint, IEndPoint newEndPoint)
    {
      return new CollectionEndPointModification (
          this,
          CollectionEndPointChangeAgent.CreateForAddOrRemove (_oppositeDomainObjects, oldEndPoint, newEndPoint));
    }

    public virtual RelationEndPointModification CreateInsertModification (IEndPoint oldEndPoint, IEndPoint newEndPoint, int index)
    {
      return new CollectionEndPointModification (
          this,
          CollectionEndPointChangeAgent.CreateForInsert (_oppositeDomainObjects, oldEndPoint, newEndPoint, index));
    }

    public virtual RelationEndPointModification CreateReplaceModification (IEndPoint oldEndPoint, IEndPoint newEndPoint)
    {
      return new CollectionEndPointModification (
          this,
          CollectionEndPointChangeAgent.CreateForReplace (
              _oppositeDomainObjects, oldEndPoint, newEndPoint, _oppositeDomainObjects.IndexOf (oldEndPoint.GetDomainObject())));
    }

    public virtual void PerformRelationChange (CollectionEndPointModification modification)
    {
      ArgumentUtility.CheckNotNull ("modification", modification);

      modification.ChangeAgent.PerformRelationChange();
      _hasBeenTouched = true;
    }

    public override void PerformDelete ()
    {
      _oppositeDomainObjects.PerformDelete();
      _hasBeenTouched = true;
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

    #region Serialization

    protected CollectionEndPoint (FlattenedDeserializationInfo info)
        : base (info)
    {
      Type collectionType = info.GetValueForHandle<Type>();
      _originalOppositeDomainObjects = (DomainObjectCollection) 
          TypesafeActivator.CreateInstance (collectionType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).With();
      _originalOppositeDomainObjects.DeserializeFromFlatStructure (info);
      _originalOppositeDomainObjects.ChangeDelegate = this;

      _oppositeDomainObjects = (DomainObjectCollection)
          TypesafeActivator.CreateInstance (collectionType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).With ();
      _oppositeDomainObjects.DeserializeFromFlatStructure (info);
      _oppositeDomainObjects.ChangeDelegate = this;

      _hasBeenTouched = info.GetValue<bool> ();
    }

    protected override void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddHandle (_originalOppositeDomainObjects.GetType());
      _originalOppositeDomainObjects.SerializeIntoFlatStructure (info);
      _oppositeDomainObjects.SerializeIntoFlatStructure (info);
      info.AddValue (_hasBeenTouched);
    }

    #endregion
  }
}