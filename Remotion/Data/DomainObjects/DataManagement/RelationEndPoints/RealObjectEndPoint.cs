// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.RealObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Represents an <see cref="ObjectEndPoint"/> that holds the foreign key in a relation. The foreign key is actually held by a 
  /// <see cref="PropertyValue"/> object, this end point implementation just delegates to the <see cref="PropertyValue"/>.
  /// </summary>
  public class RealObjectEndPoint : ObjectEndPoint, IRealObjectEndPoint
  {
    private static PropertyValue GetForeignKeyProperty (DataContainer foreignKeyDataContainer, string propertyName)
    {
      PropertyValue foreignKeyProperty;
      try
      {
        foreignKeyProperty = foreignKeyDataContainer.PropertyValues[propertyName];
      }
      catch (ArgumentException ex)
      {
        throw new ArgumentException ("The foreign key data container must be compatible with the end point definition.", "foreignKeyDataContainer", ex);
      }

      Assertion.IsTrue (foreignKeyProperty.Definition.IsObjectID, "The foreign key property must have a property type of ObjectID.");
      return foreignKeyProperty;
    }

    private readonly DataContainer _foreignKeyDataContainer;
    private readonly IRelationEndPointProvider _endPointProvider;
    private readonly IClientTransactionEventSink _transactionEventSink;
    private readonly PropertyValue _foreignKeyProperty;

    private IRealObjectEndPointSyncState _syncState; // keeps track of whether this end-point is synchronised with the opposite end point

    public RealObjectEndPoint (
        ClientTransaction clientTransaction, 
        RelationEndPointID id,
        DataContainer foreignKeyDataContainer,
        IRelationEndPointProvider endPointProvider,
        IClientTransactionEventSink transactionEventSink)
      : base (
          ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction),
          ArgumentUtility.CheckNotNull ("id", id))
    {
      ArgumentUtility.CheckNotNull ("foreignKeyDataContainer", foreignKeyDataContainer);
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull ("transactionEventSink", transactionEventSink);

      if (ID.Definition.IsVirtual)
        throw new ArgumentException ("End point ID must refer to a non-virtual end point.", "id");

      _foreignKeyDataContainer = foreignKeyDataContainer;
      _endPointProvider = endPointProvider;
      _transactionEventSink = transactionEventSink;

      _foreignKeyProperty = GetForeignKeyProperty (_foreignKeyDataContainer, PropertyName);
      _syncState = new UnknownRealObjectEndPointSyncState (_endPointProvider);
    }

    public DataContainer ForeignKeyDataContainer
    {
      get { return _foreignKeyDataContainer; }
    }

    public IRelationEndPointProvider EndPointProvider
    {
      get { return _endPointProvider; }
    }

    public IClientTransactionEventSink TransactionEventSink
    {
      get { return _transactionEventSink; }
    }

    public PropertyValue ForeignKeyProperty
    {
      get { return _foreignKeyProperty; }
    }

    public override ObjectID OppositeObjectID
    {
      get { return (ObjectID) ForeignKeyProperty.GetValueWithoutEvents (ValueAccess.Current); }
    }

    public override ObjectID OriginalOppositeObjectID
    {
      get { return (ObjectID) ForeignKeyProperty.GetValueWithoutEvents (ValueAccess.Original); }
    }

    public override bool HasChanged
    {
      get { return ForeignKeyProperty.HasChanged; }
    }

    public override bool HasBeenTouched
    {
      get { return ForeignKeyProperty.HasBeenTouched; }
    }

    public override bool IsDataComplete
    {
      get { return true; }
    }

    public override bool? IsSynchronized
    {
      get { return _syncState.IsSynchronized (this); }
    }

    public override DomainObject GetOppositeObject ()
    {
      if (OppositeObjectID == null)
        return null;
      else if (ClientTransaction.IsInvalid (OppositeObjectID))
        return ClientTransaction.GetInvalidObjectReference (OppositeObjectID);
      else
        return ClientTransaction.GetObject (OppositeObjectID, true);
    }

    public override DomainObject GetOriginalOppositeObject ()
    {
      if (OriginalOppositeObjectID == null)
        return null;

      return ClientTransaction.GetObject (OriginalOppositeObjectID, true);
    }

    public override void EnsureDataComplete ()
    {
      Assertion.IsTrue (IsDataComplete);
    }

    public override void Synchronize ()
    {
      var oppositeID = RelationEndPointID.CreateOpposite (Definition, OppositeObjectID);
      var oppositeEndPoint = (IVirtualEndPoint) _endPointProvider.GetRelationEndPointWithLazyLoad (oppositeID);
      _syncState.Synchronize (this, oppositeEndPoint);
    }

    public void MarkSynchronized ()
    {
      _syncState = new SynchronizedRealObjectEndPointSyncState (_endPointProvider, _transactionEventSink);
    }

    public void MarkUnsynchronized ()
    {
      _syncState = new UnsynchronizedRealObjectEndPointSyncState ();
    }

    public void ResetSyncState ()
    {
      _syncState = new UnknownRealObjectEndPointSyncState (_endPointProvider);
    }

    public override IDataManagementCommand CreateDeleteCommand ()
    {
      return _syncState.CreateDeleteCommand (this, () => SetOppositeObjectID (null));
    }

    public override IDataManagementCommand CreateSetCommand (DomainObject newRelatedObject)
    {
      return _syncState.CreateSetCommand (this, newRelatedObject, domainObject => SetOppositeObjectID (domainObject.GetSafeID()));
    }

    public override void Touch ()
    {
      ForeignKeyProperty.Touch ();
      Assertion.IsTrue (HasBeenTouched);
    }

    public override void Commit ()
    {
      ForeignKeyProperty.CommitState ();
      Assertion.IsFalse (HasBeenTouched);
      Assertion.IsFalse (HasChanged);
    }

    public override void Rollback ()
    {
      ForeignKeyProperty.RollbackState ();
      Assertion.IsFalse (HasBeenTouched);
      Assertion.IsFalse (HasChanged);
    }

    protected override void SetOppositeObjectDataFromSubTransaction (IObjectEndPoint sourceObjectEndPoint)
    {
      var sourceAsRealObjectEndPoint = ArgumentUtility.CheckNotNullAndType<RealObjectEndPoint> ("sourceObjectEndPoint", sourceObjectEndPoint);
      _foreignKeyProperty.SetDataFromSubTransaction (sourceAsRealObjectEndPoint.ForeignKeyProperty);
    }

    private void SetOppositeObjectID (ObjectID value)
    {
      _foreignKeyProperty.Value = value; // TODO 4608: This is with events, which is a little inconsistent to OppositeObjectID
    }


    #region Serialization
    protected RealObjectEndPoint (FlattenedDeserializationInfo info)
      : base (info)
    {
      _foreignKeyDataContainer = info.GetValueForHandle<DataContainer> ();
      _foreignKeyProperty = GetForeignKeyProperty (_foreignKeyDataContainer, PropertyName);
      _endPointProvider = info.GetValueForHandle<IRelationEndPointProvider> ();
      _transactionEventSink = info.GetValueForHandle<IClientTransactionEventSink> ();
      _syncState = info.GetValueForHandle<IRealObjectEndPointSyncState> ();
    }

    protected override void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      base.SerializeIntoFlatStructure (info);
      info.AddHandle (_foreignKeyDataContainer);
      info.AddHandle (_endPointProvider);
      info.AddHandle (_transactionEventSink);
      info.AddHandle (_syncState);
    }
    #endregion

  }
}