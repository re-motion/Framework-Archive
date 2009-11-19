// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;
using System.Reflection;

namespace Remotion.Data.DomainObjects.DataManagement
{
  public class CollectionEndPoint : RelationEndPoint, ICollectionEndPoint
  {
    // types

    // static members and constants

    // member fields

    // this field is not serialized via IFlattenedSerializable.SerializeIntoFlatStructure
    private ICollectionEndPointChangeDelegate _changeDelegate = null;

    private readonly DomainObjectCollection _originalOppositeDomainObjectsContents;
    private DomainObjectCollection _originalOppositeDomainObjectsReference;
    private DomainObjectCollection _oppositeDomainObjects;

    private bool _hasBeenTouched;

    // construction and disposing

    public CollectionEndPoint (
        ClientTransaction clientTransaction,
        RelationEndPointID id,
        ICollectionEndPointChangeDelegate changeDelegate,
        IEnumerable<DomainObject> initialContents)
        : base (ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction), ArgumentUtility.CheckNotNull ("id", id))
    {
      ArgumentUtility.CheckNotNull ("changeDelegate", changeDelegate);

      var requiredItemType = id.OppositeEndPointDefinition.ClassDefinition.ClassType;
      var collectionType = id.Definition.PropertyType;

      var factory = new DomainObjectCollectionFactory ();

      var dataStore = new DomainObjectCollectionData (initialContents);
      _oppositeDomainObjects = factory.CreateCollection (collectionType, new EndPointDelegatingCollectionData (this, dataStore), requiredItemType);
      _oppositeDomainObjects.ChangeDelegate = this;

      _originalOppositeDomainObjectsContents = _oppositeDomainObjects.Clone (true);
      _originalOppositeDomainObjectsReference = _oppositeDomainObjects;
      
      _hasBeenTouched = false;
      _changeDelegate = changeDelegate;
    }

    protected CollectionEndPoint (IRelationEndPointDefinition definition)
        : base (definition)
    {
      _hasBeenTouched = false;
    }

    // methods and properties

    public void SetOppositeCollection (DomainObjectCollection oppositeDomainObjects)
    {
      ArgumentUtility.CheckNotNull ("oppositeDomainObjects", oppositeDomainObjects);

      if (oppositeDomainObjects.AssociatedEndPoint != this)
      {
        throw new ArgumentException (
            "The new opposite collection must have been prepared to delegate to this end point. Use ReplaceOppositeCollection instead.",
            "oppositeDomainObjects");
      }

      if (oppositeDomainObjects.GetType () != _oppositeDomainObjects.GetType ())
      {
        string message = string.Format (
            "The new opposite collection must have the same type as the old collection ('{0}'), but its type is '{1}'.",
            _oppositeDomainObjects.GetType (),
            oppositeDomainObjects.GetType ());
        throw new ArgumentException (message, "oppositeDomainObjects");
      }

      _oppositeDomainObjects = oppositeDomainObjects;
      Touch ();
    }

    public void ReplaceOppositeCollection (DomainObjectCollection oppositeDomainObjects)
    {
      ArgumentUtility.CheckNotNull ("oppositeDomainObjects", oppositeDomainObjects);

      var modification = oppositeDomainObjects.CreateAssociationModification (this);
      var bidirectionalModification = modification.CreateBidirectionalModification ();
      bidirectionalModification.ExecuteAllSteps ();
    }

    public override RelationEndPoint Clone (ClientTransaction clientTransaction)
    {
      var clone = new CollectionEndPoint (
          clientTransaction, 
          ID, 
          clientTransaction.DataManager.RelationEndPointMap, 
          _oppositeDomainObjects.Cast<DomainObject>());

      clone._oppositeDomainObjects.AssumeSameState (_oppositeDomainObjects);
      clone._originalOppositeDomainObjectsContents.AssumeSameState (_originalOppositeDomainObjectsContents);
      clone._hasBeenTouched = _hasBeenTouched;
      return clone;
    }

    protected internal override void TakeOverCommittedData (RelationEndPoint source)
    {
      var sourceCollectionEndPoint = ArgumentUtility.CheckNotNullAndType<CollectionEndPoint> ("source", source);
      Assertion.IsTrue (Definition == sourceCollectionEndPoint.Definition);
      
      _oppositeDomainObjects.TakeOverCommittedData (sourceCollectionEndPoint._oppositeDomainObjects);
      _hasBeenTouched |= sourceCollectionEndPoint._hasBeenTouched || HasChanged;
    }

    public override void Commit ()
    {
      if (HasChanged)
      {
        _originalOppositeDomainObjectsContents.Commit (_oppositeDomainObjects);
        _originalOppositeDomainObjectsReference = _oppositeDomainObjects;
      }

      _hasBeenTouched = false;
    }

    public override void Rollback ()
    {
      if (HasChanged)
      {
        _oppositeDomainObjects.ChangeDelegate = null;
        _oppositeDomainObjects = _originalOppositeDomainObjectsReference;
        _oppositeDomainObjects.ChangeDelegate = this;
        _oppositeDomainObjects.Rollback (_originalOppositeDomainObjectsContents);
      }

      _hasBeenTouched = false;
    }

    public override bool HasChanged
    {
      get
      {
        return OppositeDomainObjects != OriginalOppositeDomainObjectsReference
               || ClientTransaction.HasCollectionEndPointDataChanged (OppositeDomainObjects, OriginalOppositeDomainObjectsContents);
      }
    }

    public override bool HasBeenTouched
    {
      get { return _hasBeenTouched; }
    }

    public override void Touch ()
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

    public EndPointDelegatingCollectionData CreateDelegatingCollectionData (IDomainObjectCollectionData actualDataStore)
    {
      return new EndPointDelegatingCollectionData (this, actualDataStore);
    }

    public override IRelationEndPointModification CreateRemoveModification (DomainObject removedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("removedRelatedObject", removedRelatedObject);
      return new CollectionEndPointRemoveModification (this, removedRelatedObject, _oppositeDomainObjects._data.GetUndecoratedDataStore ());
    }

    public override IRelationEndPointModification CreateSelfReplaceModification (DomainObject selfReplaceRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("selfReplaceRelatedObject", selfReplaceRelatedObject);
      return new CollectionEndPointSelfReplaceModification (this, selfReplaceRelatedObject, _oppositeDomainObjects._data.GetUndecoratedDataStore ());
    }

    public virtual IRelationEndPointModification CreateInsertModification (DomainObject insertedRelatedObject, int index)
    {
      ArgumentUtility.CheckNotNull ("insertedRelatedObject", insertedRelatedObject);
      return new CollectionEndPointInsertModification (this, index, insertedRelatedObject, _oppositeDomainObjects._data.GetUndecoratedDataStore ());
    }

    public virtual IRelationEndPointModification CreateAddModification (DomainObject addedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("addedRelatedObject", addedRelatedObject);
      return CreateInsertModification (addedRelatedObject, OppositeDomainObjects.Count);
    }

    public virtual IRelationEndPointModification CreateReplaceModification (int index, DomainObject replacementObject)
    {
      var replacedObject = OppositeDomainObjects[index];
      if (replacedObject == replacementObject)
        return new CollectionEndPointSelfReplaceModification (this, replacedObject, _oppositeDomainObjects._data.GetUndecoratedDataStore());
      else
        return new CollectionEndPointReplaceModification (this, replacedObject, index, replacementObject, _oppositeDomainObjects._data.GetUndecoratedDataStore ());
    }

    public override void PerformDelete ()
    {
      Assertion.IsFalse (_oppositeDomainObjects.IsReadOnly);

      ((IDomainObjectCollectionEventRaiser) _oppositeDomainObjects).BeginDelete ();
      _oppositeDomainObjects._data.GetUndecoratedDataStore ().Clear ();
      _hasBeenTouched = true;
      ((IDomainObjectCollectionEventRaiser) _oppositeDomainObjects).EndDelete ();
    }

    public DomainObjectCollection OriginalOppositeDomainObjectsContents
    {
      get { return _originalOppositeDomainObjectsContents; }
    }

    public DomainObjectCollection OriginalOppositeDomainObjectsReference
    {
      get { return _originalOppositeDomainObjectsReference; }
    }

    public DomainObjectCollection OppositeDomainObjects
    {
      get { return _oppositeDomainObjects; }
    }

    public ICollectionEndPointChangeDelegate ChangeDelegate
    {
      get { return _changeDelegate; }
    }

    #region ICollectionChangeDelegate Members

    void ICollectionChangeDelegate.PerformInsert (DomainObjectCollection collection, DomainObject domainObject, int index)
    {
      ChangeDelegate.PerformInsert (this, domainObject, index);
      _hasBeenTouched = true;
    }

    void ICollectionChangeDelegate.PerformReplace (DomainObjectCollection collection, DomainObject newDomainObject, int index)
    {
      ChangeDelegate.PerformReplace (this, index, newDomainObject);
      _hasBeenTouched = true;
    }

    void ICollectionChangeDelegate.PerformRemove (DomainObjectCollection collection, DomainObject domainObject)
    {
      ChangeDelegate.PerformRemove (this, domainObject);
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
      _originalOppositeDomainObjectsContents = info.GetValueForHandle<DomainObjectCollection>();
      _originalOppositeDomainObjectsContents.ChangeDelegate = this;
      _oppositeDomainObjects = info.GetValueForHandle<DomainObjectCollection>();
      _oppositeDomainObjects.ChangeDelegate = this;
      _originalOppositeDomainObjectsReference = info.GetValueForHandle<DomainObjectCollection>();
      _hasBeenTouched = info.GetBoolValue();
      _changeDelegate = info.GetValueForHandle<ICollectionEndPointChangeDelegate>();

      // if _changeDelegate == null, we didn't serialize the back-reference to the RelationEndPointMap, so get it from the ClientTransaction
      // only do that after deserialization has finished
      if (_changeDelegate == null)
        info.DeserializationFinished += ((sender, args) => _changeDelegate = ClientTransaction.DataManager.RelationEndPointMap);

      FixupAssociatedEndPoint (_oppositeDomainObjects);
    }

    protected override void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddHandle (_originalOppositeDomainObjectsContents);
      info.AddHandle (_oppositeDomainObjects);
      info.AddHandle (_originalOppositeDomainObjectsReference);
      info.AddBoolValue (_hasBeenTouched);

      // cannot serialize back-references, therefore, we only add the ChangeDelegate if it is not the (default) back-reference to 
      // ClientTransaction.DataManager.RelationEndPointMap
      info.AddHandle (_changeDelegate == ClientTransaction.DataManager.RelationEndPointMap ? null : _changeDelegate);
    }

    private void FixupAssociatedEndPoint (DomainObjectCollection collection)
    {
      // The reason we need to do a fix up for associated collections is that:
      // - CollectionEndPoint is not serializable; it is /flattened/ serializable (for performance reasons); this means no reference to it can be
      //   by a serializable object.
      // - DomainObjectCollection is serializable, not flattened serializable.
      // - Therefore, EndPointDelegatingCollectionData can only be serializable, not flattened serializable.
      // - Therefore, EndPointDelegatingCollectionData's back-reference to CollectionEndPoint cannot be serialized. (It is marked as [NonSerializable].)
      // - Therefore, it needs to be fixed up manually when the end point is restored.

      // Fixups could be avoided if DomainObjectCollection and all IDomainObjectCollectionData implementations were made flattened serializable, 
      // but that would be complex and it would impose the details of flattened serialization to re-store's users. Won't happen.
      // Fixups could also be avoided if the end points stop being flattened serializable. For that, however, they must lose any references they 
      // currently have to RelationEndPointMap. Will probably happen in the future.
      // If it doesn't happen, fixups can be made prettier by adding an IAssociatedEndPointFixup interface to DomainObjectCollection and all 
      // IDomainObjectCollectionData implementors.

      var dataField = typeof (DomainObjectCollection).GetField ("_data", BindingFlags.NonPublic | BindingFlags.Instance);
      var decorator = dataField.GetValue (collection);

      var wrappedDataField = typeof (ArgumentCheckingCollectionDataDecorator).GetField ("_wrappedData", BindingFlags.NonPublic | BindingFlags.Instance);
      var endPointDelegatingData = (EndPointDelegatingCollectionData) wrappedDataField.GetValue (decorator);

      var associatedEndPointField = typeof (EndPointDelegatingCollectionData).GetField ("_associatedEndPoint", BindingFlags.NonPublic | BindingFlags.Instance);
      associatedEndPointField.SetValue (endPointDelegatingData, this);
    }

    #endregion
  }
}
