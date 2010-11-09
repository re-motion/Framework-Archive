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
using Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;
using System.Reflection;

namespace Remotion.Data.DomainObjects.DataManagement
{
  public class CollectionEndPoint : RelationEndPoint, ICollectionEndPoint
  {
    private readonly ICollectionEndPointChangeDetectionStrategy _changeDetectionStrategy;
    private readonly ICollectionEndPointDataKeeper _dataKeeper; // stores the data kept by _oppositeDomainObjects and the original data for rollback

    private DomainObjectCollection _oppositeDomainObjects; // points to _dataKeeper by using EndPointDelegatingCollectionData as its data strategy
    private DomainObjectCollection _originalCollectionReference; // keeps the original reference of the _oppositeDomainObjects for rollback

    private bool _hasBeenTouched;

    public CollectionEndPoint (
        ClientTransaction clientTransaction,
        RelationEndPointID id,
        ICollectionEndPointChangeDetectionStrategy changeDetectionStrategy,
        IEnumerable<DomainObject> initialContentsOrNull)
        : base (ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction), ArgumentUtility.CheckNotNull ("id", id))
    {
      ArgumentUtility.CheckNotNull ("changeDetectionStrategy", changeDetectionStrategy);
      
      _dataKeeper = new LazyLoadingCollectionEndPointDataKeeper (clientTransaction, id, initialContentsOrNull);

      var collectionType = id.Definition.PropertyType;
      var dataStrategy = CreateDelegatingCollectionData ();
      _oppositeDomainObjects = DomainObjectCollectionFactory.Instance.CreateCollection (collectionType, dataStrategy);

      _originalCollectionReference = _oppositeDomainObjects;
      
      _hasBeenTouched = false;
      _changeDetectionStrategy = changeDetectionStrategy;
    }

    // No loading
    public DomainObjectCollection OppositeDomainObjects
    {
      get { return _oppositeDomainObjects; }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        if (_oppositeDomainObjects.GetType () != value.GetType ())
          throw new ArgumentTypeException ("value", _oppositeDomainObjects.GetType (), value.GetType ());

        if (!value.IsAssociatedWith (this))
        {
          throw new ArgumentException (
              "The new opposite collection must have been prepared to delegate to this end point. Use SetOppositeCollectionAndNotify instead.",
              "value");
        }

        _oppositeDomainObjects = value;
        Touch ();

        RaiseStateUpdateNotification (HasChanged);
      }
    }

    // Causes collection to be loaded
    public DomainObjectCollection OriginalOppositeDomainObjectsContents
    {
      get 
      { 
        var collectionType = Definition.PropertyType;
        return DomainObjectCollectionFactory.Instance.CreateCollection (collectionType, _dataKeeper.OriginalCollectionData);
      }
    }

    // No loading
    public DomainObjectCollection OriginalCollectionReference
    {
      get { return _originalCollectionReference; }
    }

    public ICollectionEndPointChangeDetectionStrategy ChangeDetectionStrategy
    {
      get { return _changeDetectionStrategy; }
    }

    public override bool IsDataAvailable
    {
      get { return _dataKeeper.IsDataAvailable; }
    }

    // No loading
    public override bool HasChanged
    {
      get { return OriginalCollectionReference != OppositeDomainObjects || _dataKeeper.HasDataChanged (_changeDetectionStrategy); }
    }

    // No loading
    public override bool HasBeenTouched
    {
      get { return _hasBeenTouched; }
    }

    // No loading
    public void Unload ()
    {
      if (IsDataAvailable)
      {
        ClientTransaction.TransactionEventSink.RelationEndPointUnloading (ClientTransaction, this);
        _dataKeeper.Unload();
      }
    }

    // Causes collection to be loaded
    public override void EnsureDataAvailable ()
    {
      _dataKeeper.EnsureDataAvailable ();
    }

    // Causes collection to be loaded // TODO: Very implicit
    public void SetOppositeCollectionAndNotify (DomainObjectCollection oppositeDomainObjects)
    {
      ArgumentUtility.CheckNotNull ("oppositeDomainObjects", oppositeDomainObjects);

      if (!oppositeDomainObjects.IsAssociatedWith (null) && !oppositeDomainObjects.IsAssociatedWith (this))
        throw new ArgumentException ("The given collection is already associated with an end point.", "oppositeDomainObjects");

      RelationEndPointValueChecker.CheckNotDeleted (this, this.GetDomainObject ());

      var command = ((IAssociatableDomainObjectCollection) oppositeDomainObjects).CreateAssociationCommand (this);
      var bidirectionalModification = command.ExpandToAllRelatedObjects ();
      bidirectionalModification.NotifyAndPerform ();
    }

    // Causes collection to be loaded
    public override void SetValueFrom (RelationEndPoint source)
    {
      var sourceCollectionEndPoint = ArgumentUtility.CheckNotNullAndType<CollectionEndPoint> ("source", source);
      if (Definition != sourceCollectionEndPoint.Definition)
      {
        var message = string.Format (
            "Cannot set this end point's value from '{0}'; the end points do not have the same end point definition.", 
            source.ID);
        throw new ArgumentException (message, "source");
      }

      _dataKeeper.CollectionData.ReplaceContents (sourceCollectionEndPoint._dataKeeper.CollectionData);

      if (sourceCollectionEndPoint.HasBeenTouched || HasChanged)
        Touch ();
    }

    // No loading
    public override void Commit ()
    {
      if (HasChanged)
      {
        _dataKeeper.CommitOriginalContents ();
        _originalCollectionReference = _oppositeDomainObjects;
      }

      _hasBeenTouched = false;
    }

    // No loading
    public override void Rollback ()
    {
      if (HasChanged)
      {
        var oppositeObjectsReferenceBeforeRollback = _oppositeDomainObjects;

        if (_originalCollectionReference != _oppositeDomainObjects)
        {
          var command = ((IAssociatableDomainObjectCollection) _originalCollectionReference).CreateAssociationCommand (this);
          command.Perform(); // no notifications, no bidirectional changes, we only change the collections' associations
        }

        _oppositeDomainObjects = _originalCollectionReference;

        Assertion.IsTrue (_oppositeDomainObjects.IsAssociatedWith (this));
        Assertion.IsTrue (
            _oppositeDomainObjects == oppositeObjectsReferenceBeforeRollback 
            || !oppositeObjectsReferenceBeforeRollback.IsAssociatedWith (this));

        _oppositeDomainObjects.ReplaceItemsWithoutNotifications (OriginalOppositeDomainObjectsContents.Cast<DomainObject>());
      }

      _hasBeenTouched = false;
    }

    // No loading
    public override void Touch ()
    {
      _hasBeenTouched = true;
    }

    // Causes collection to be loaded // TODO: Bug!
    public override void CheckMandatory ()
    {
      if (_oppositeDomainObjects.Count == 0)
      {
        throw CreateMandatoryRelationNotSetException (
            this.GetDomainObject(),
            PropertyName,
            "Mandatory relation property '{0}' of domain object '{1}' contains no items.",
            PropertyName,
            ObjectID);
      }
    }

    // No loading
    public IDomainObjectCollectionData CreateDelegatingCollectionData ()
    {
      var requiredItemType = Definition.GetOppositeEndPointDefinition().ClassDefinition.ClassType;
      var dataStrategy = new ModificationCheckingCollectionDataDecorator (requiredItemType, new EndPointDelegatingCollectionData (this, _dataKeeper));

      return dataStrategy;
    }

    // Causes collection to be loaded
    public override IDataManagementCommand CreateRemoveCommand (DomainObject removedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("removedRelatedObject", removedRelatedObject);
      return new CollectionEndPointRemoveCommand (this, removedRelatedObject, _dataKeeper.CollectionData); // indirect EnsureDataAvailable
    }

    // Causes collection to be loaded // TODO: Collection is only loaded from within the command, this probably should be done earlier
    public override IDataManagementCommand CreateDeleteCommand ()
    {
      return new AdHocCommand
          {
            BeginHandler = () => ((IDomainObjectCollectionEventRaiser) _oppositeDomainObjects).BeginDelete (),
            PerformHandler = () => { _dataKeeper.CollectionData.Clear (); Touch (); }, // indirect EnsureDataAvailable (IN THE command!)
            EndHandler = () => ((IDomainObjectCollectionEventRaiser) _oppositeDomainObjects).EndDelete ()
          };
    }

    // Causes collection to be loaded
    public virtual IDataManagementCommand CreateInsertCommand (DomainObject insertedRelatedObject, int index)
    {
      ArgumentUtility.CheckNotNull ("insertedRelatedObject", insertedRelatedObject);
      return new CollectionEndPointInsertCommand (this, index, insertedRelatedObject, _dataKeeper.CollectionData); // indirect EnsureDataAvailable
    }

    // Causes collection to be loaded
    public virtual IDataManagementCommand CreateAddCommand (DomainObject addedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("addedRelatedObject", addedRelatedObject);
      return CreateInsertCommand (addedRelatedObject, OppositeDomainObjects.Count); // indirect EnsureDataAvailable
    }

    // Causes collection to be loaded
    public virtual IDataManagementCommand CreateReplaceCommand (int index, DomainObject replacementObject)
    {
      var replacedObject = OppositeDomainObjects[index];
      if (replacedObject == replacementObject)
        return new CollectionEndPointReplaceSameCommand (this, replacedObject, _dataKeeper.CollectionData); // indirect EnsureDataAvailable
      else
        return new CollectionEndPointReplaceCommand (this, replacedObject, index, replacementObject, _dataKeeper.CollectionData); // indirect EnsureDataAvailable
    }

    // Causes collection to be loaded
    public override IEnumerable<RelationEndPoint> GetOppositeRelationEndPoints (IDataManager dataManager)
    {
      var oppositeEndPointDefinition = Definition.GetOppositeEndPointDefinition ();

      Assertion.IsFalse (oppositeEndPointDefinition.IsAnonymous);

      return from oppositeDomainObject in OppositeDomainObjects.Cast<DomainObject> ()
             let oppositeEndPointID = new RelationEndPointID (oppositeDomainObject.ID, oppositeEndPointDefinition)
             select dataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (oppositeEndPointID);
    }

    private void RaiseStateUpdateNotification (bool hasChanged)
    {
      ClientTransaction.TransactionEventSink.VirtualRelationEndPointStateUpdated (ClientTransaction, ID, hasChanged);
    }

    #region Serialization

    protected CollectionEndPoint (FlattenedDeserializationInfo info)
        : base (info)
    {
      _oppositeDomainObjects = info.GetValueForHandle<DomainObjectCollection>();
      _originalCollectionReference = info.GetValueForHandle<DomainObjectCollection>();
      _hasBeenTouched = info.GetBoolValue();
      _dataKeeper = info.GetValue<ICollectionEndPointDataKeeper> ();
      _changeDetectionStrategy = info.GetValueForHandle<ICollectionEndPointChangeDetectionStrategy> ();

      FixupAssociatedEndPoint (_oppositeDomainObjects);
    }

    protected override void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddHandle (_oppositeDomainObjects);
      info.AddHandle (_originalCollectionReference);
      info.AddBoolValue (_hasBeenTouched);
      info.AddValue (_dataKeeper);
      info.AddHandle (_changeDetectionStrategy);
    }

    private void FixupAssociatedEndPoint (DomainObjectCollection collection)
    {
      // The reason we need to do a fix up for associated collections is that:
      // - CollectionEndPoint is not serializable; it is /flattened/ serializable (for performance reasons); this means no reference to it can be held
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

      var dataField = typeof (DomainObjectCollection).GetField ("_dataStrategy", BindingFlags.NonPublic | BindingFlags.Instance);
      var decorator = dataField.GetValue (collection);

      var wrappedDataField = typeof (DomainObjectCollectionDataDecoratorBase).GetField ("_wrappedData", BindingFlags.NonPublic | BindingFlags.Instance);
      var endPointDelegatingData = (EndPointDelegatingCollectionData) wrappedDataField.GetValue (decorator);

      var associatedEndPointField = typeof (EndPointDelegatingCollectionData).GetField ("_associatedEndPoint", BindingFlags.NonPublic | BindingFlags.Instance);
      associatedEndPointField.SetValue (endPointDelegatingData, this);

      var endPointDataField = typeof (EndPointDelegatingCollectionData).GetField ("_endPointDataKeeper", BindingFlags.NonPublic | BindingFlags.Instance);
      endPointDataField.SetValue (endPointDelegatingData, _dataKeeper);
    }

    #endregion
  }
}
