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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Registers and unregisters end-points in/from a <see cref="RelationEndPointManager"/>.
  /// </summary>
  [Serializable]
  public class RelationEndPointRegistrationAgent : IRelationEndPointRegistrationAgent
  {
    private readonly IRelationEndPointProvider _endPointProvider;

    public RelationEndPointRegistrationAgent (IRelationEndPointProvider endPointProvider)
    {
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);
      _endPointProvider = endPointProvider;
    }

    public IRelationEndPointProvider EndPointProvider
    {
      get { return _endPointProvider; }
    }

    public void RegisterEndPoint (IRelationEndPoint endPoint, RelationEndPointMap map)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      if (map[endPoint.ID] != null)
      {
        var message = string.Format ("A relation end-point with ID '{0}' has already been registered.", endPoint.ID);
        throw new InvalidOperationException (message);
      }
      
      map.AddEndPoint (endPoint);

      var realObjectEndPoint = endPoint as IRealObjectEndPoint;
      if (realObjectEndPoint != null)
        RegisterOppositeForRealObjectEndPoint (realObjectEndPoint);
    }

    public void UnregisterEndPoint (IRelationEndPoint endPoint, RelationEndPointMap map)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      if (map[endPoint.ID] != endPoint)
      {
        var message = string.Format ("End-point '{0}' is not part of this map.\r\nParameter name: endPoint", endPoint.ID);
        throw new ArgumentException (message);
      }

      var realObjectEndPoint = endPoint as IRealObjectEndPoint;
      if (realObjectEndPoint != null)
        UnregisterOppositeForRealObjectEndPoint (realObjectEndPoint, map);

      map.RemoveEndPoint (endPoint.ID);
    }

    protected virtual IVirtualEndPoint RegisterOppositeForRealObjectEndPoint (IRealObjectEndPoint realObjectEndPoint)
    {
      var oppositeVirtualEndPointID = RelationEndPointID.CreateOpposite (realObjectEndPoint.Definition, realObjectEndPoint.OriginalOppositeObjectID);
      if (oppositeVirtualEndPointID.Definition.IsAnonymous)
      {
        realObjectEndPoint.MarkSynchronized ();
        return null;
      }

      var oppositeVirtualEndPoint = (IVirtualEndPoint) _endPointProvider.GetRelationEndPointWithMinimumLoading (oppositeVirtualEndPointID);
      oppositeVirtualEndPoint.RegisterOriginalOppositeEndPoint (realObjectEndPoint);
      return oppositeVirtualEndPoint;
    }

    private void UnregisterOppositeForRealObjectEndPoint (IRealObjectEndPoint realObjectEndPoint, RelationEndPointMap map)
    {
      ArgumentUtility.CheckNotNull ("realObjectEndPoint", realObjectEndPoint);

      var oppositeEndPointID = RelationEndPointID.CreateOpposite (realObjectEndPoint.Definition, realObjectEndPoint.OriginalOppositeObjectID);
      if (oppositeEndPointID.Definition.IsAnonymous)
      {
        realObjectEndPoint.ResetSyncState ();
        return;
      }
      
      var oppositeEndPoint = (IVirtualEndPoint) _endPointProvider.GetRelationEndPointWithoutLoading (oppositeEndPointID);
      if (oppositeEndPoint == null)
      {
        var message = string.Format (
            "Opposite end-point of '{0}' not found. When unregistering a non-virtual bidirectional end-point, the opposite end-point must exist.", 
            realObjectEndPoint.ID);
        throw new InvalidOperationException (message);
      }

      oppositeEndPoint.UnregisterOriginalOppositeEndPoint (realObjectEndPoint);
      if (oppositeEndPoint.CanBeCollected)
        map.RemoveEndPoint (oppositeEndPoint.ID);
    }
  }
}