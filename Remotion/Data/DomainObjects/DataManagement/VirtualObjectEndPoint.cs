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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints;
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Represents an <see cref="ObjectEndPoint"/> that does not hold the foreign key in a relation. The <see cref="VirtualObjectEndPoint"/> is
  /// constructed by the <see cref="RelationEndPointMap"/> as an in-memory representation of the opposite of the <see cref="RealObjectEndPoint"/> 
  /// holding the foreign key.
  /// </summary>
  public class VirtualObjectEndPoint : ObjectEndPoint, IVirtualObjectEndPoint
  {
    private ObjectID _originalOppositeObjectID;
    private ObjectID _oppositeObjectID;
    private readonly IVirtualEndPointDataKeeperFactory<IVirtualObjectEndPointDataKeeper> _dataKeeperFactory;
    private bool _hasBeenTouched;

    public VirtualObjectEndPoint (
        ClientTransaction clientTransaction,
        RelationEndPointID id,
        ObjectID oppositeObjectID,
        IRelationEndPointLazyLoader lazyLoader,
        IRelationEndPointProvider endPointProvider,
        IVirtualEndPointDataKeeperFactory<IVirtualObjectEndPointDataKeeper> dataKeeperFactory)
        : base (
            ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction),
            ArgumentUtility.CheckNotNull ("id", id),
            ArgumentUtility.CheckNotNull ("lazyLoader", lazyLoader),
            ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider))
    {
      ArgumentUtility.CheckNotNull ("dataKeeperFactory", dataKeeperFactory);

      if (!ID.Definition.IsVirtual)
        throw new ArgumentException ("End point ID must refer to a virtual end point.", "id");

      _oppositeObjectID = oppositeObjectID;
      _originalOppositeObjectID = oppositeObjectID;
      _hasBeenTouched = false;

      _dataKeeperFactory = dataKeeperFactory;
    }

    public IVirtualEndPointDataKeeperFactory<IVirtualObjectEndPointDataKeeper> DataKeeperFactory
    {
      get { return _dataKeeperFactory; }
    }

    public override ObjectID OppositeObjectID
    {
      get { return _oppositeObjectID; }
    }

    ObjectID IVirtualEndPoint<ObjectID>.GetData ()
    {
      return OppositeObjectID;
    }

    public override ObjectID OriginalOppositeObjectID
    {
      get { return _originalOppositeObjectID; }
    }

    ObjectID IVirtualEndPoint<ObjectID>.GetOriginalData ()
    {
      return OriginalOppositeObjectID;
    }

    public override bool HasChanged
    {
      get { return !Equals (_oppositeObjectID, _originalOppositeObjectID); }
    }

    public override bool HasBeenTouched
    {
      get { return _hasBeenTouched; }
    }

    public override bool IsSynchronized
    {
      get { return true; }
    }

    public override void Synchronize ()
    {
      // TODO 3818
    }

    public void SynchronizeOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      throw new InvalidOperationException (
          "In the current implementation, ObjectEndPoints in a 1:1 relation should always be in-sync with each other.");
    }

    public void MarkDataComplete (DomainObject item)
    {
      // TODO 3818
    }

    public void MarkDataIncomplete ()
    {
      // TODO 3818
      throw new NotImplementedException ();
    }

    public void RegisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      // TODO 3818
      throw new NotImplementedException ();
    }

    public void UnregisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      // TODO 3818
      throw new NotImplementedException ();
    }

    public void RegisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      // TODO 3818
      throw new NotImplementedException ();
    }

    public void UnregisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      // TODO 3818
      throw new NotImplementedException ();
    }

    public override IDataManagementCommand CreateSetCommand (DomainObject newRelatedObject)
    {
      var newRelatedObjectID = newRelatedObject != null ? newRelatedObject.ID : null;
      if (OppositeObjectID == newRelatedObjectID)
        return new ObjectEndPointSetSameCommand (this, SetOppositeObjectID);
      else
        return new ObjectEndPointSetOneOneCommand (this, newRelatedObject, SetOppositeObjectID);
    }

    public override IDataManagementCommand CreateDeleteCommand ()
    {
      return new ObjectEndPointDeleteCommand (this, SetOppositeObjectID);
    }

    public override void Touch ()
    {
      _hasBeenTouched = true;
    }

    public override void Commit ()
    {
      _originalOppositeObjectID = _oppositeObjectID;
      RaiseStateUpdateNotification (false);

      _hasBeenTouched = false;
    }

    public override void Rollback ()
    {
      _oppositeObjectID = _originalOppositeObjectID;
      RaiseStateUpdateNotification (false);

      _hasBeenTouched = false;
    }

    protected override void SetOppositeObjectIDValueFrom (IObjectEndPoint sourceObjectEndPoint)
    {
      ArgumentUtility.CheckNotNull ("sourceObjectEndPoint", sourceObjectEndPoint);
      _oppositeObjectID = sourceObjectEndPoint.OppositeObjectID;
    }

    private void RaiseStateUpdateNotification (bool newChangedState)
    {
      ClientTransaction.TransactionEventSink.VirtualRelationEndPointStateUpdated (ClientTransaction, ID, newChangedState);
    }

    // TODO 3818: Remove
    private void SetOppositeObjectID (ObjectID value)
    {
      _oppositeObjectID = value;
      RaiseStateUpdateNotification (HasChanged);
    }

    #region Serialization

    protected VirtualObjectEndPoint (FlattenedDeserializationInfo info)
        : base (info)
    {
      _hasBeenTouched = info.GetBoolValue();
      _oppositeObjectID = info.GetValueForHandle<ObjectID>();
      _originalOppositeObjectID = _hasBeenTouched ? info.GetValueForHandle<ObjectID>() : _oppositeObjectID;
      _dataKeeperFactory = info.GetValueForHandle<IVirtualEndPointDataKeeperFactory<IVirtualObjectEndPointDataKeeper>>();
    }

    protected override void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      base.SerializeIntoFlatStructure (info);

      info.AddBoolValue (_hasBeenTouched);
      info.AddHandle (_oppositeObjectID);
      if (_hasBeenTouched)
        info.AddHandle (_originalOppositeObjectID);
      info.AddHandle (_dataKeeperFactory);
    }

    #endregion
  }
}