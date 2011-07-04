﻿using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints
{
  /// <summary>
  /// Defines common logic for <see cref="IVirtualEndPoint"/> implementations in complete state, ie., when lazy loading has completed.
  /// </summary>
  /// <typeparam name="TEndPoint">The type of the end point whose state is managed by this class.</typeparam>
  /// <typeparam name="TData">The type of data held by the <typeparamref name="TDataKeeper"/>.</typeparam>
  /// <typeparam name="TDataKeeper">The type of data keeper holding the data for the end-point.</typeparam>
  public abstract class CompleteVirtualEndPointLoadStateBase<TEndPoint, TData, TDataKeeper> 
      : IVirtualEndPointLoadState<TEndPoint, TData, TDataKeeper>
      where TEndPoint : IVirtualEndPoint<TData>
      where TDataKeeper : IVirtualEndPointDataKeeper
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (CompleteVirtualEndPointLoadStateBase<TEndPoint, TData, TDataKeeper>));

    private readonly TDataKeeper _dataKeeper;
    private readonly IRelationEndPointProvider _endPointProvider;
    private readonly ClientTransaction _clientTransaction;

    private readonly Dictionary<ObjectID, IRealObjectEndPoint> _unsynchronizedOppositeEndPoints;

    protected CompleteVirtualEndPointLoadStateBase (
        TDataKeeper dataKeeper,
        IRelationEndPointProvider endPointProvider,
        ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("dataKeeper", dataKeeper);
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      _dataKeeper = dataKeeper;
      _endPointProvider = endPointProvider;
      _clientTransaction = clientTransaction;

      _unsynchronizedOppositeEndPoints = new Dictionary<ObjectID, IRealObjectEndPoint> ();
    }

    public abstract TData GetData (TEndPoint endPoint);
    public abstract TData GetOriginalData (TEndPoint endPoint);
    public abstract void SetDataFromSubTransaction (TEndPoint endPoint, IVirtualEndPointLoadState<TEndPoint, TData, TDataKeeper> sourceLoadState);

    protected abstract IEnumerable<IRealObjectEndPoint> GetOriginalOppositeEndPoints ();
    protected abstract IEnumerable<DomainObject> GetOriginalItemsWithoutEndPoints ();

    public static ILog Log
    {
      get { return s_log; }
    }

    public TDataKeeper DataKeeper
    {
      get { return _dataKeeper; }
    }

    public IRelationEndPointProvider EndPointProvider
    {
      get { return _endPointProvider; }
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public ICollection<IRealObjectEndPoint> UnsynchronizedOppositeEndPoints
    {
      get { return _unsynchronizedOppositeEndPoints.Values; }
    }

    public bool IsDataComplete ()
    {
      return true;
    }

    public void EnsureDataComplete (TEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      // Data is already complete
    }

    public bool CanDataBeMarkedIncomplete (TEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      return !HasChanged();
    }

    public virtual void MarkDataIncomplete (TEndPoint endPoint, Action stateSetter)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("stateSetter", stateSetter);

      if (HasChanged ())
      {
        var message = string.Format ("Cannot mark virtual end-point '{0}' incomplete because it has been changed.", endPoint.ID);
        throw new InvalidOperationException (message);
      }

      _clientTransaction.TransactionEventSink.RelationEndPointUnloading (_clientTransaction, endPoint);

      stateSetter ();

      var allOppositeEndPoints = UnsynchronizedOppositeEndPoints.Concat (GetOriginalOppositeEndPoints());
      foreach (var oppositeEndPoint in allOppositeEndPoints)
        endPoint.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
    }

    public bool CanEndPointBeCollected (TEndPoint endPoint)
    {
      return false;
    }

    public void RegisterOriginalOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      if (_dataKeeper.ContainsOriginalObjectID (oppositeEndPoint.ObjectID))
      {
        if (s_log.IsInfoEnabled)
        {
          s_log.InfoFormat (
              "RealObjectEndPoint '{0}' is registered for already loaded virtual end-point '{1}'. "
              + "The query result contained the item, so the ObjectEndPoint is marked as synchronzed.",
              oppositeEndPoint.ID,
              endPoint.ID);
        }

        _dataKeeper.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
        oppositeEndPoint.MarkSynchronized ();
      }
      else
      {
        if (s_log.IsWarnEnabled)
        {
          s_log.WarnFormat (
              "ObjectEndPoint '{0}' is registered for already loaded virtual end-point '{1}'. "
              + "The query result did not contain the item, so the ObjectEndPoint is out-of-sync.",
              oppositeEndPoint.ID,
              endPoint.ID);
        }

        _unsynchronizedOppositeEndPoints.Add (oppositeEndPoint.ObjectID, oppositeEndPoint);
        oppositeEndPoint.MarkUnsynchronized ();
      }
    }

    public void UnregisterOriginalOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      if (_unsynchronizedOppositeEndPoints.ContainsKey (oppositeEndPoint.ObjectID))
      {
        if (s_log.IsDebugEnabled)
        {
          s_log.DebugFormat (
              "Unsynchronized ObjectEndPoint '{0}' is unregistered from virtual end-point '{1}'.",
              oppositeEndPoint.ID,
              endPoint.ID);
        }

        _unsynchronizedOppositeEndPoints.Remove (oppositeEndPoint.ObjectID);
      }
      else
      {
        if (s_log.IsInfoEnabled)
        {
          s_log.InfoFormat (
              "ObjectEndPoint '{0}' is unregistered from virtual end-point '{1}'. The virtual end-point is transitioned to incomplete state.",
              oppositeEndPoint.ID,
              endPoint.ID);
        }

        endPoint.MarkDataIncomplete ();
        endPoint.UnregisterOriginalOppositeEndPoint (oppositeEndPoint);
      }
    }

    public void RegisterCurrentOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _dataKeeper.RegisterCurrentOppositeEndPoint (oppositeEndPoint);
    }

    public void UnregisterCurrentOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      _dataKeeper.UnregisterCurrentOppositeEndPoint (oppositeEndPoint);
    }

    public bool IsSynchronized (TEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      return !GetOriginalItemsWithoutEndPoints ().Any();
    }

    bool? IVirtualEndPointLoadState<TEndPoint, TData, TDataKeeper>.IsSynchronized (TEndPoint endPoint)
    {
      return IsSynchronized (endPoint);
    }

    public virtual void Synchronize (TEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      if (Log.IsDebugEnabled)
        Log.DebugFormat ("End-point '{0}' is being synchronized.", endPoint.ID);

      foreach (var item in GetOriginalItemsWithoutEndPoints ())
        DataKeeper.UnregisterOriginalItemWithoutEndPoint (item);
    }

    public virtual void SynchronizeOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      if (s_log.IsDebugEnabled)
        s_log.DebugFormat ("ObjectEndPoint '{0}' is being marked as synchronized.", oppositeEndPoint.ID);

      if (!_unsynchronizedOppositeEndPoints.Remove (oppositeEndPoint.ObjectID))
      {
        var message = string.Format (
            "Cannot synchronize opposite end-point '{0}' - the end-point is not in the list of unsynchronized end-points.",
            oppositeEndPoint.ID);
        throw new InvalidOperationException (message);
      }

      _dataKeeper.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
      oppositeEndPoint.MarkSynchronized ();
    }

    public bool HasChanged ()
    {
      return _dataKeeper.HasDataChanged ();
    }

    public virtual void Commit (TEndPoint endPoint)
    {
      _dataKeeper.Commit ();
    }

    public virtual void Rollback (TEndPoint endPoint)
    {
      _dataKeeper.Rollback ();
    }

    protected void MarkDataComplete (TEndPoint endPoint, IEnumerable<DomainObject> data, Action<TDataKeeper> stateSetter)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      ArgumentUtility.CheckNotNull ("data", data);
      ArgumentUtility.CheckNotNull ("stateSetter", stateSetter);

      throw new InvalidOperationException ("The data is already complete.");
    }

    protected bool ContainsUnsynchronizedOppositeEndPoint (ObjectID objectID)
    {
      return _unsynchronizedOppositeEndPoints.ContainsKey (objectID);
    }

    #region Serialization

    protected CompleteVirtualEndPointLoadStateBase (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      _dataKeeper = info.GetValueForHandle<TDataKeeper> ();
      _endPointProvider = info.GetValueForHandle<IRelationEndPointProvider> ();
      _clientTransaction = info.GetValueForHandle<ClientTransaction> ();
      var unsynchronizedOppositeEndPoints = new List<IRealObjectEndPoint> ();
      info.FillCollection (unsynchronizedOppositeEndPoints);
      _unsynchronizedOppositeEndPoints = unsynchronizedOppositeEndPoints.ToDictionary (ep => ep.ObjectID);
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      SerializeIntoFlatStructure(info);
    }

    protected virtual void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      info.AddHandle (_dataKeeper);
      info.AddHandle (_endPointProvider);
      info.AddHandle (_clientTransaction);
      info.AddCollection (_unsynchronizedOppositeEndPoints.Values);
    }

    #endregion
  }
}