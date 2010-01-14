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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Provides APIs for unloading the data that a <see cref="ClientTransaction"/> stores for <see cref="DomainObject"/> instances.
  /// </summary>
  public static class DomainObjectUnloader
  {
    /// <summary>
    /// Unloads the unchanged collection end point indicated by the given <see cref="RelationEndPointID"/> in the specified 
    /// <see cref="ClientTransaction"/>. If the end point has not been loaded or has already been unloaded, this method does nothing.
    /// </summary>
    /// <param name="clientTransaction">The client transaction to unload the data from.</param>
    /// <param name="endPointID">The end point ID. In order to retrieve this ID from a <see cref="DomainObjectCollection"/> representing a relation
    /// end point, specify the <see cref="IEndPoint.ID"/> of the <see cref="DomainObjectCollection.AssociatedEndPoint"/>.</param>
    /// <exception cref="InvalidOperationException">The given end point is not unchanged state.</exception>
    /// <exception cref="ArgumentNullException">One of the arguments passed to this method is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The given <paramref name="endPointID"/> does not specify a collection end point.</exception>
    public static void UnloadCollectionEndPoint (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      if (endPointID.Definition.Cardinality != CardinalityType.Many)
      {
        var message = string.Format ("The given end point ID '{0}' does not denote a CollectionEndPoint.", endPointID);
        throw new ArgumentException (message, "endPointID");
      }

      var collectionEndPoint = (CollectionEndPoint) clientTransaction.DataManager.RelationEndPointMap[endPointID];
      if (collectionEndPoint != null)
      {
        if (collectionEndPoint.HasChanged)
        {
          var message = string.Format ("The end point with ID '{0}' has been changed. Changed end points cannot be unloaded.", endPointID);
          throw new InvalidOperationException (message);
        }

        collectionEndPoint.Unload();
      }
    }
  }
}