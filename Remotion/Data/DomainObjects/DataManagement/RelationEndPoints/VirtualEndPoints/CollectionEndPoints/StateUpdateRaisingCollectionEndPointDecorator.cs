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
using System.Collections.Generic;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Decorates another <see cref="ICollectionEndPoint"/>, raising <see cref="IVirtualEndPointStateUpdateListener"/> events whenever the 
  /// return value of the <see cref="HasChanged"/> property has possibly changed.
  /// </summary>
  /// <remarks>
  /// Because the <see cref="HasChanged"/> property of <see cref="ICollectionEndPoint"/> implementations can be expensive to determine, the 
  /// <see cref="StateUpdateRaisingCollectionEndPointDecorator"/> doesn't actually check the property.
  /// Events may also be raised even the the <see cref="HasChanged"/> property still returns the same value as before.
  /// </remarks>
  public class StateUpdateRaisingCollectionEndPointDecorator : ICollectionEndPoint
  {
    /// <summary>
    /// Using an instance of this class around a code block asserts that the change state before and after after the block is the same.
    /// </summary>
    private struct ConstantChangeStateAsserter : IDisposable
    {
      private readonly bool? _changeStateBefore;
      private readonly ICollectionEndPoint _innerEndPoint;

      public ConstantChangeStateAsserter (ICollectionEndPoint innerEndPoint)
      {
        _changeStateBefore = innerEndPoint.HasChangedFast;
        _innerEndPoint = innerEndPoint;
      }

      public void Dispose ()
      {
        Assertion.IsTrue (_changeStateBefore == _innerEndPoint.HasChangedFast);
      }
    }

    private readonly ICollectionEndPoint _innerEndPoint;
    private readonly IVirtualEndPointStateUpdateListener _listener;

    public StateUpdateRaisingCollectionEndPointDecorator (ICollectionEndPoint innerEndPoint, IVirtualEndPointStateUpdateListener listener)
    {
      ArgumentUtility.CheckNotNull ("innerEndPoint", innerEndPoint);
      ArgumentUtility.CheckNotNull ("listener", listener);

      _innerEndPoint = innerEndPoint;
      _listener = listener;
    }

    public IVirtualEndPointStateUpdateListener Listener
    {
      get { return _listener; }
    }

    public ICollectionEndPoint InnerEndPoint
    {
      get { return _innerEndPoint; }
    }

    public void SetDataFromSubTransaction (IRelationEndPoint source)
    {
      try
      {
        _innerEndPoint.SetDataFromSubTransaction (source);
      }
      finally
      {
        RaiseStateUpdated();
      }
    }

    public void Synchronize ()
    {
      try
      {
        _innerEndPoint.Synchronize();
      }
      finally
      {
        RaiseStateUpdated ();
      }
    }

    public void SynchronizeOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      try
      {
        _innerEndPoint.SynchronizeOppositeEndPoint (oppositeEndPoint);
      }
      finally
      {
        RaiseStateUpdated ();
      }
    }

    public void Commit ()
    {
      try
      {
        _innerEndPoint.Commit();
      }
      finally
      {
        RaiseStateUpdated ();
      }
    }

    public void Rollback ()
    {
      try
      {
        _innerEndPoint.Rollback();
      }
      finally
      {
        RaiseStateUpdated ();
      }
    }

    public void SortCurrentData (Comparison<DomainObject> comparison)
    {
      try
      {
        _innerEndPoint.SortCurrentData (comparison);
      }
      finally
      {
        RaiseStateUpdated ();
      }
    }

    public IDataManagementCommand CreateRemoveCommand (DomainObject removedRelatedObject)
    {
#if DEBUG
      using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
      {
        var command = _innerEndPoint.CreateRemoveCommand (removedRelatedObject);
        return CreateStateUpdateRaisingCommandDecorator (command);
      }
    }

    public IDataManagementCommand CreateDeleteCommand ()
    {
#if DEBUG
      using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
      {
        var command = _innerEndPoint.CreateDeleteCommand();
        return CreateStateUpdateRaisingCommandDecorator (command);
      }
    }

    public IDataManagementCommand CreateSetCollectionCommand (DomainObjectCollection newCollection)
    {
#if DEBUG
      using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
      {
        var command = _innerEndPoint.CreateSetCollectionCommand (newCollection);
        return CreateStateUpdateRaisingCommandDecorator (command);
      }
    }

    public IDataManagementCommand CreateInsertCommand (DomainObject insertedRelatedObject, int index)
    {
#if DEBUG
      using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
      {
        var command = _innerEndPoint.CreateInsertCommand (insertedRelatedObject, index);
        return CreateStateUpdateRaisingCommandDecorator (command);
      }
    }

    public IDataManagementCommand CreateAddCommand (DomainObject addedRelatedObject)
    {
#if DEBUG
      using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
      {
        var command = _innerEndPoint.CreateAddCommand (addedRelatedObject);
        return CreateStateUpdateRaisingCommandDecorator (command);
      }
    }

    public IDataManagementCommand CreateReplaceCommand (int index, DomainObject replacementObject)
    {
#if DEBUG
      using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
      {
        var command = _innerEndPoint.CreateReplaceCommand (index, replacementObject);
        return CreateStateUpdateRaisingCommandDecorator (command);
      }
    }

    #region Methods not affecting HasChanged

    public bool IsNull
    {
      get
      {
#if DEBUG
        using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
        {
          return _innerEndPoint.IsNull;
        }
      }
    }

    public RelationEndPointID ID
    {
      get
      {
#if DEBUG
        using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
        {
          return _innerEndPoint.ID;
        }
      }
    }

    public ClientTransaction ClientTransaction
    {
      get
      {
#if DEBUG
        using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
        {
          return _innerEndPoint.ClientTransaction;
        }
      }
    }

    public ObjectID ObjectID
    {
      get
      {
#if DEBUG
        using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
        {
          return _innerEndPoint.ObjectID;
        }
      }
    }

    public IRelationEndPointDefinition Definition
    {
      get
      {
#if DEBUG
        using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
        {
          return _innerEndPoint.Definition;
        }
      }
    }

    public RelationDefinition RelationDefinition
    {
      get
      {
#if DEBUG
        using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
        {
          return _innerEndPoint.RelationDefinition;
        }
      }
    }

    public bool? HasChangedFast
    {
      // No assertion on this property because the assertion uses this property...
      get { return _innerEndPoint.HasChangedFast; }
    }

    public bool HasChanged
    {
      get
      {
#if DEBUG
        using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
        {
          return _innerEndPoint.HasChanged;
        }
      }
    }

    public bool HasBeenTouched
    {
      get
      {
#if DEBUG
        using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
        {
          return _innerEndPoint.HasBeenTouched;
        }
      }
    }

    public DomainObject GetDomainObject ()
    {
#if DEBUG
      using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
      {
        return _innerEndPoint.GetDomainObject ();
      }
    }

    public DomainObject GetDomainObjectReference ()
    {
#if DEBUG
      using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
      {
        return _innerEndPoint.GetDomainObjectReference ();
      }
    }

    public bool IsDataComplete
    {
      get
      {
#if DEBUG
        using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
        {
          return _innerEndPoint.IsDataComplete;
        }
      }
    }

    public void EnsureDataComplete ()
    {
#if DEBUG
      using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
      {
        _innerEndPoint.EnsureDataComplete ();
      }
    }

    public bool? IsSynchronized
    {
      get
      {
#if DEBUG
        using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
        {
          return _innerEndPoint.IsSynchronized;
        }
      }
    }

    public void Touch ()
    {
#if DEBUG
      using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
      {
        _innerEndPoint.Touch ();
      }
    }

    public void ValidateMandatory ()
    {
#if DEBUG
      using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
      {
        _innerEndPoint.ValidateMandatory ();
      }
    }

    public IEnumerable<RelationEndPointID> GetOppositeRelationEndPointIDs ()
    {
#if DEBUG
      using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
      {
        return _innerEndPoint.GetOppositeRelationEndPointIDs ();
      }
    }

    public bool CanBeCollected
    {
      get
      {
#if DEBUG
        using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
        {
          return _innerEndPoint.CanBeCollected;
        }
      }
    }

    public bool CanBeMarkedIncomplete
    {
      get
      {
#if DEBUG
        using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
        {
          return _innerEndPoint.CanBeMarkedIncomplete;
        }
      }
    }

    public void MarkDataIncomplete ()
    {
#if DEBUG
      using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
      {
        _innerEndPoint.MarkDataIncomplete ();
      }
    }

    public void RegisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
#if DEBUG
      using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
      {
        _innerEndPoint.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
      }
    }

    public void UnregisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
#if DEBUG
      using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
      {
        _innerEndPoint.UnregisterOriginalOppositeEndPoint (oppositeEndPoint);
      }
    }

    public void RegisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
#if DEBUG
      using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
      {
        _innerEndPoint.RegisterCurrentOppositeEndPoint (oppositeEndPoint);
      }
    }

    public void UnregisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
#if DEBUG
      using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
      {
        _innerEndPoint.UnregisterCurrentOppositeEndPoint (oppositeEndPoint);
      }
    }

    public ReadOnlyCollectionDataDecorator GetData ()
    {
#if DEBUG
      using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
      {
        return _innerEndPoint.GetData ();
      }
    }

    public ReadOnlyCollectionDataDecorator GetOriginalData ()
    {
#if DEBUG
      using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
      {
        return _innerEndPoint.GetOriginalData ();
      }
    }

    public DomainObjectCollection Collection
    {
      get
      {
#if DEBUG
        using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
        {
          return _innerEndPoint.Collection;
        }
      }
    }

    public DomainObjectCollection OriginalCollection
    {
      get
      {
#if DEBUG
        using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
        {
          return _innerEndPoint.OriginalCollection;
        }
      }
    }

    public IDomainObjectCollectionEventRaiser GetCollectionEventRaiser ()
    {
#if DEBUG
      using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
      {
        return _innerEndPoint.GetCollectionEventRaiser ();
      }
    }

    public DomainObjectCollection GetCollectionWithOriginalData ()
    {
#if DEBUG
      using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
      {
        return _innerEndPoint.GetCollectionWithOriginalData ();
      }
    }

    public void MarkDataComplete (DomainObject[] items)
    {
#if DEBUG
      using (new ConstantChangeStateAsserter (_innerEndPoint))
#endif
      {
        _innerEndPoint.MarkDataComplete (items);
      }
    }

    #endregion

    private void RaiseStateUpdated ()
    {
      _listener.StateUpdated (_innerEndPoint.HasChangedFast);
    }

    private IDataManagementCommand CreateStateUpdateRaisingCommandDecorator (IDataManagementCommand command)
    {
      return new VirtualEndPointStateUpdatedRaisingCommandDecorator (command, _listener, () => _innerEndPoint.HasChangedFast);
    }

    #region Serialization

    public StateUpdateRaisingCollectionEndPointDecorator (FlattenedDeserializationInfo info)
    {
      _innerEndPoint = info.GetValue<ICollectionEndPoint>();
      _listener = info.GetValueForHandle<IVirtualEndPointStateUpdateListener>();
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddValue (_innerEndPoint);
      info.AddHandle (_listener);
    }

    #endregion
  }
}