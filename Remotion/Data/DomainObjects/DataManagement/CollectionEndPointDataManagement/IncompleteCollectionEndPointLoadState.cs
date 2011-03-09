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
using System.Collections.ObjectModel;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement
{
  /// <summary>
  /// Represents the state of a <see cref="CollectionEndPoint"/> where not all of its data is available (ie., the end-point has not been (lazily) 
  /// loaded, or it has been unloaded).
  /// </summary>
  public class IncompleteCollectionEndPointLoadState : ICollectionEndPointLoadState
  {
    private readonly ICollectionEndPointDataKeeper _dataKeeper;
    private readonly IRelationEndPointLazyLoader _lazyLoader;
    private readonly HashSet<IObjectEndPoint> _oppositeEndPoints;

    public IncompleteCollectionEndPointLoadState (ICollectionEndPointDataKeeper dataKeeper, IRelationEndPointLazyLoader lazyLoader)
    {
      ArgumentUtility.CheckNotNull ("dataKeeper", dataKeeper);
      ArgumentUtility.CheckNotNull ("lazyLoader", lazyLoader);

      _dataKeeper = dataKeeper;
      _lazyLoader = lazyLoader;
      _oppositeEndPoints = new HashSet<IObjectEndPoint>();
    }

    public ICollectionEndPointDataKeeper DataKeeper
    {
      get { return _dataKeeper; }
    }

    public IRelationEndPointLazyLoader LazyLoader
    {
      get { return _lazyLoader; }
    }

    public IObjectEndPoint[] OppositeEndPoints
    {
      get { return _oppositeEndPoints.ToArray(); }
    }

    public bool IsDataComplete ()
    {
      return false;
    }

    public void EnsureDataComplete (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      _lazyLoader.LoadLazyCollectionEndPoint (collectionEndPoint);
    }

    public void MarkDataComplete (ICollectionEndPoint collectionEndPoint, DomainObject[] items, Action<ICollectionEndPointDataKeeper> stateSetter)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("items", items);
      ArgumentUtility.CheckNotNull ("stateSetter", stateSetter);

      foreach (var oppositeEndPoint in _oppositeEndPoints)
        _dataKeeper.RegisterOriginalOppositeEndPoint (oppositeEndPoint);
      
      _dataKeeper.SortCurrentAndOriginalData();
      stateSetter (_dataKeeper);
    }

    public void MarkDataIncomplete (ICollectionEndPoint collectionEndPoint, Action<ICollectionEndPointDataKeeper> stateSetter)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("stateSetter", stateSetter);

      throw new InvalidOperationException ("The data is already incomplete.");
    }

    public IDomainObjectCollectionData GetCollectionData (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      
      collectionEndPoint.EnsureDataComplete ();
      return collectionEndPoint.GetCollectionData();
    }

    public DomainObjectCollection GetCollectionWithOriginalData (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      
      collectionEndPoint.EnsureDataComplete ();
      return collectionEndPoint.GetCollectionWithOriginalData();
    }

    public IEnumerable<RelationEndPointID> GetOppositeRelationEndPointIDs (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      
      collectionEndPoint.EnsureDataComplete ();
      return collectionEndPoint.GetOppositeRelationEndPointIDs ();
    }

    public void RegisterOriginalOppositeEndPoint (ICollectionEndPoint collectionEndPoint, IObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      if (_oppositeEndPoints.Contains (oppositeEndPoint))
        throw new InvalidOperationException ("The opposite end point has already been registered.");

      if (_dataKeeper.ContainsOriginalOppositeEndPoint (oppositeEndPoint))
        throw new InvalidOperationException ("The opposite end point has already been registered.");

      _oppositeEndPoints.Add (oppositeEndPoint);
    }

    public void UnregisterOriginalOppositeEndPoint (ICollectionEndPoint collectionEndPoint, IObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      if (!_oppositeEndPoints.Remove (oppositeEndPoint))
        _dataKeeper.UnregisterOriginalOppositeEndPoint (oppositeEndPoint);
    }

    public ReadOnlyCollection<IObjectEndPoint> GetUnsynchronizedOppositeEndPoints ()
    {
      return Array.AsReadOnly (new IObjectEndPoint[0]);
    }

    public void SynchronizeOppositeEndPoint (IObjectEndPoint oppositeEndPoint)
    {
      ArgumentUtility.CheckNotNull ("oppositeEndPoint", oppositeEndPoint);

      var message = string.Format (
          "Cannot synchronize opposite end-point '{0}' - the end-point is not in the list of unsynchronized end-points.", 
          oppositeEndPoint.ID);
      throw new InvalidOperationException (message);
    }

    public IDataManagementCommand CreateSetCollectionCommand (ICollectionEndPoint collectionEndPoint, DomainObjectCollection newCollection, Action<DomainObjectCollection> collectionSetter)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("newCollection", newCollection);

      collectionEndPoint.EnsureDataComplete ();
      return collectionEndPoint.CreateSetCollectionCommand (newCollection);
    }

    public IDataManagementCommand CreateRemoveCommand (ICollectionEndPoint collectionEndPoint, DomainObject removedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("removedRelatedObject", removedRelatedObject);

      collectionEndPoint.EnsureDataComplete ();
      return collectionEndPoint.CreateRemoveCommand (removedRelatedObject);
    }

    public IDataManagementCommand CreateDeleteCommand (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      
      collectionEndPoint.EnsureDataComplete ();
      return collectionEndPoint.CreateDeleteCommand ();
    }

    public IDataManagementCommand CreateInsertCommand (ICollectionEndPoint collectionEndPoint, DomainObject insertedRelatedObject, int index)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("insertedRelatedObject", insertedRelatedObject);
      
      collectionEndPoint.EnsureDataComplete ();
      return collectionEndPoint.CreateInsertCommand (insertedRelatedObject, index);
    }

    public IDataManagementCommand CreateAddCommand (ICollectionEndPoint collectionEndPoint, DomainObject addedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("addedRelatedObject", addedRelatedObject);
      
      collectionEndPoint.EnsureDataComplete ();
      return collectionEndPoint.CreateAddCommand (addedRelatedObject);
    }

    public IDataManagementCommand CreateReplaceCommand (ICollectionEndPoint collectionEndPoint, int index, DomainObject replacementObject)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("replacementObject", replacementObject);

      collectionEndPoint.EnsureDataComplete ();
      return collectionEndPoint.CreateReplaceCommand (index, replacementObject);
    }

    public void SetValueFrom (ICollectionEndPoint collectionEndPoint, ICollectionEndPoint sourceEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("sourceEndPoint", sourceEndPoint);

      collectionEndPoint.EnsureDataComplete ();
      collectionEndPoint.SetValueFrom (sourceEndPoint);
    }

    public void CheckMandatory (ICollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);

      collectionEndPoint.EnsureDataComplete ();
      collectionEndPoint.CheckMandatory ();
    }

    public bool HasChanged (ICollectionEndPointChangeDetectionStrategy changeDetectionStrategy)
    {
      ArgumentUtility.CheckNotNull ("changeDetectionStrategy", changeDetectionStrategy);

      return _dataKeeper.HasDataChanged (changeDetectionStrategy);
    }

    public void Commit ()
    {
      _dataKeeper.Commit ();
    }

    #region Serialization

    public IncompleteCollectionEndPointLoadState (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      _lazyLoader = info.GetValueForHandle<IRelationEndPointLazyLoader>();
      _dataKeeper = info.GetValueForHandle<ICollectionEndPointDataKeeper>();
      
      _oppositeEndPoints = new HashSet<IObjectEndPoint>();
      info.FillCollection (_oppositeEndPoints);
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      info.AddHandle (_lazyLoader);
      info.AddHandle (_dataKeeper);
      info.AddCollection (_oppositeEndPoints);
    }

    #endregion
  }
}