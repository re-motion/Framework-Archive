﻿using System;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;

namespace Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints
{
  /// <summary>
  /// Represents the lazy-loading state of an <see cref="IVirtualEndPoint"/> and implements accessor methods for that end-point.
  /// </summary>
  /// <typeparam name="TEndPoint">The type of the end point whose state is managed by this instance.</typeparam>
  /// <typeparam name="TData">The type of data held by the <typeparamref name="TDataKeeper"/>.</typeparam>
  /// <typeparam name="TDataKeeper">The type of data keeper holding the data for the end-point.</typeparam>
  public interface IVirtualEndPointLoadState<TEndPoint, TData, TDataKeeper> : IFlattenedSerializable
      where TEndPoint : IVirtualEndPoint<TData>
      where TDataKeeper : IVirtualEndPointDataKeeper
  {
    bool IsDataComplete ();
    void EnsureDataComplete (TEndPoint endPoint);

    void MarkDataIncomplete (TEndPoint endPoint, Action<TDataKeeper> stateSetter);

    TData GetData (TEndPoint endPoint);
    TData GetOriginalData (TEndPoint endPoint);

    void RegisterOriginalOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint);
    void UnregisterOriginalOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint);

    void RegisterCurrentOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint);
    void UnregisterCurrentOppositeEndPoint (TEndPoint endPoint, IRealObjectEndPoint oppositeEndPoint);

    bool IsSynchronized (TEndPoint endPoint);
    void Synchronize (TEndPoint endPoint);

    void SynchronizeOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint);

    void SetValueFrom (TEndPoint endPoint, TEndPoint sourceEndPoint);

    bool HasChanged ();

    void Commit ();
    void Rollback ();
  }
}