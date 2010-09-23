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
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DomainImplementation
{
  /// <summary>
  /// Provides functionality for unloading the data that a <see cref="ClientTransaction"/> stores for <see cref="DomainObject"/> instances.
  /// </summary>
  public static class UnloadService
  {
    /// <summary>
    /// Unloads the unchanged collection end point indicated by the given <see cref="RelationEndPointID"/> in the specified
    /// <see cref="ClientTransaction"/>. If the end point has not been loaded or has already been unloaded, this method does nothing.
    /// </summary>
    /// <param name="clientTransaction">The client transaction to unload the data from.</param>
    /// <param name="endPointID">The end point ID. In order to retrieve this ID from a <see cref="DomainObjectCollection"/> representing a relation
    /// end point, specify the <see cref="DomainObjectCollection.AssociatedEndPointID"/>.</param>
    /// <param name="transactionMode">The <see cref="UnloadTransactionMode"/> to use. This can be used to specify whether the unload operation should 
    /// affect this transaction only or the whole transaction hierarchy, up to the root transaction.</param>
    /// <exception cref="InvalidOperationException">The given end point is not in unchanged state.</exception>
    /// <exception cref="ArgumentNullException">One of the arguments passed to this method is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The given <paramref name="endPointID"/> does not specify a collection end point.</exception>
    /// <remarks>
    /// With <see cref="UnloadTransactionMode.RecurseToRoot"/>, the unload operation is not atomic over the transaction hierarchy. It will start at
    /// the given transaction and try to unload here, then it will go over the parent transactions one by one. If the operation fails in any of the
    /// transactions, it will stop and throw an exception. At this point of time, the operation's results will be visible in all
    /// the transactions where it succeeded, but not in the one where it failed or those above.
    /// </remarks>
    public static void UnloadCollectionEndPoint (ClientTransaction clientTransaction, RelationEndPointID endPointID, UnloadTransactionMode transactionMode)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      var collectionEndPoint = CheckAndGetCollectionEndPoint(clientTransaction, endPointID);
      if (collectionEndPoint != null)
      {
        if (collectionEndPoint.HasChanged)
        {
          var message = string.Format ("The end point with ID '{0}' has been changed. Changed end points cannot be unloaded.", endPointID);
          throw new InvalidOperationException (message);
        }

        collectionEndPoint.Unload();
      }

      if (transactionMode == UnloadTransactionMode.RecurseToRoot && clientTransaction.ParentTransaction != null)
      {
        using (TransactionUnlocker.MakeWriteable (clientTransaction.ParentTransaction))
        {
          UnloadCollectionEndPoint (clientTransaction.ParentTransaction, endPointID, transactionMode);
        }
      }
    }

    /// <summary>
    /// Tries to unload the unchanged collection end point indicated by the given <see cref="RelationEndPointID"/> in the specified
    /// <see cref="ClientTransaction"/>, returning a value indicating whether the unload operation succeeded. If the end point has not been loaded or
    /// has already been unloaded, this method does nothing.
    /// </summary>
    /// <param name="clientTransaction">The client transaction to unload the data from.</param>
    /// <param name="endPointID">The end point ID. In order to retrieve this ID from a <see cref="DomainObjectCollection"/> representing a relation
    /// end point, specify the <see cref="DomainObjectCollection.AssociatedEndPointID"/>.</param>
    /// <param name="transactionMode">The <see cref="UnloadTransactionMode"/> to use. This can be used to specify whether the unload operation should
    /// affect this transaction only or the whole transaction hierarchy, up to the root transaction.</param>
    /// <returns><see langword="true" /> if the unload operation succeeded (in all transactions), or <see langword="false" /> if it did not succeed
    /// (in any transaction).</returns>
    /// <exception cref="ArgumentNullException">One of the arguments passed to this method is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The given <paramref name="endPointID"/> does not specify a collection end point.</exception>
    /// <remarks>
    /// With <see cref="UnloadTransactionMode.RecurseToRoot"/>, the unload operation is not atomic over the transaction hierarchy. It will start at
    /// the given transaction and try to unload here, then it will go over the parent transactions one by one. If the operation fails in any of the
    /// transactions, it will stop and return <see langword="false" />. At this point of time, the operation's results will be visible in all
    /// the transactions where it succeeded, but not in the one where it failed or those above.
    /// </remarks>
    public static bool TryUnloadCollectionEndPoint (ClientTransaction clientTransaction, RelationEndPointID endPointID, UnloadTransactionMode transactionMode)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      var collectionEndPoint = CheckAndGetCollectionEndPoint (clientTransaction, endPointID);
      if (collectionEndPoint != null)
      {
        if (collectionEndPoint.HasChanged)
          return false;

        collectionEndPoint.Unload ();
      }

      if (transactionMode == UnloadTransactionMode.RecurseToRoot && clientTransaction.ParentTransaction != null)
      {
        using (TransactionUnlocker.MakeWriteable (clientTransaction.ParentTransaction))
        {
          return TryUnloadCollectionEndPoint (clientTransaction.ParentTransaction, endPointID, transactionMode);
        }
      }

      return true;
    }

    /// <summary>
    /// Unloads the data held by the given <see cref="ClientTransaction"/> for the <see cref="DomainObject"/> with the specified 
    /// <paramref name="objectID"/>. The <see cref="DomainObject"/> reference and <see cref="DomainObjectCollection"/> instances held by the
    /// object are not removed, only the data is. The object can only be unloaded if it is in unchanged state.
    /// </summary>
    /// <param name="clientTransaction">The client transaction.</param>
    /// <param name="objectID">The object ID.</param>
    /// <param name="transactionMode">The <see cref="UnloadTransactionMode"/> to use. This can be used to specify whether the unload operation should 
    /// affect this transaction only or the whole transaction hierarchy, up to the root transaction.</param>
    /// <remarks>
    /// The method unloads the <see cref="DataContainer"/>, the collection end points the object is part of (but not
    /// the collection end points the object owns), the non-virtual end points owned by the object, their respective opposite virtual object 
    /// end-points, and the virtual object end points pointing to <see langword="null" />. This means that unloading an object will unload a relation 
    /// if and only if the object's <see cref="DataContainer"/> is holding the foreign key for the relation or if the relation points from the 
    /// unloaded object to <see langword="null" />.
    /// </remarks>
    /// <exception cref="InvalidOperationException">The object to be unloaded is not in unchanged state - or - the operation would affect an 
    /// opposite relation end-point that is not in unchanged state.</exception>
    /// <remarks>
    /// With <see cref="UnloadTransactionMode.RecurseToRoot"/>, the unload operation is not atomic over the transaction hierarchy. It will start at
    /// the given transaction and try to unload here, then it will go over the parent transactions one by one. If the operation fails in any of the
    /// transactions, it will stop and throw an exception. At this point of time, the operation's results will be visible in all
    /// the transactions where it succeeded, but not in the one where it failed or those above.
    /// </remarks>
    public static void UnloadData (ClientTransaction clientTransaction, ObjectID objectID, UnloadTransactionMode transactionMode)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      var command = clientTransaction.DataManager.CreateUnloadCommand (objectID);
      command.NotifyAndPerform ();

      if (transactionMode == UnloadTransactionMode.RecurseToRoot && clientTransaction.ParentTransaction != null)
      {
        using (TransactionUnlocker.MakeWriteable (clientTransaction.ParentTransaction))
        {
          UnloadData (clientTransaction.ParentTransaction, objectID, transactionMode);
        }
      }
    }

    /// <summary>
    /// Unloads the unchanged collection end point indicated by the given <see cref="RelationEndPointID"/> in the specified 
    /// <see cref="ClientTransaction"/> as well as the data items stored by it. If the end point has not been loaded or has already been unloaded, 
    /// this method does nothing.
    /// </summary>
    /// <param name="clientTransaction">The client transaction to unload the data from.</param>
    /// <param name="endPointID">The end point ID. In order to retrieve this ID from a <see cref="DomainObjectCollection"/> representing a relation
    /// end point, specify the <see cref="DomainObjectCollection.AssociatedEndPointID"/>.</param>
    /// <param name="transactionMode">The <see cref="UnloadTransactionMode"/> to use. This can be used to specify whether the unload operation should 
    /// affect this transaction only or the whole transaction hierarchy, up to the root transaction.</param>
    /// <exception cref="InvalidOperationException">The involved end points or one of the items it stores are not in unchanged state.</exception>
    /// <exception cref="ArgumentNullException">One of the arguments passed to this method is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The given <paramref name="endPointID"/> does not specify a collection end point.</exception>
    /// <remarks>
    /// With <see cref="UnloadTransactionMode.RecurseToRoot"/>, the unload operation is not atomic over the transaction hierarchy. It will start at
    /// the given transaction and try to unload here, then it will go over the parent transactions one by one. If the operation fails in any of the
    /// transactions, it will stop and return <see langword="false" />. At this point of time, the operation's results will be visible in all
    /// the transactions where it succeeded, but not in the one where it failed or those above.
    /// </remarks>
    public static void UnloadCollectionEndPointAndData (
        ClientTransaction clientTransaction, 
        RelationEndPointID endPointID, 
        UnloadTransactionMode transactionMode)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      var endPoint = CheckAndGetCollectionEndPoint (clientTransaction, endPointID);
      if (endPoint != null && endPoint.IsDataAvailable)
      {
        var unloadedIDs = endPoint.OppositeDomainObjects.Cast<DomainObject>().Select (obj => obj.ID).ToArray();
        var command = clientTransaction.DataManager.CreateUnloadCommand (unloadedIDs);
        command.NotifyAndPerform ();

        UnloadCollectionEndPoint (clientTransaction, endPointID, UnloadTransactionMode.ThisTransactionOnly); // needed in case unloadedIDs is empty
      }

      if (transactionMode == UnloadTransactionMode.RecurseToRoot && clientTransaction.ParentTransaction != null)
      {
        using (TransactionUnlocker.MakeWriteable (clientTransaction.ParentTransaction))
        {
          UnloadCollectionEndPointAndData (clientTransaction.ParentTransaction, endPointID, transactionMode);
        }
      }
    }

    private static CollectionEndPoint CheckAndGetCollectionEndPoint (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      if (endPointID.Definition.Cardinality != CardinalityType.Many)
      {
        var message = string.Format ("The given end point ID '{0}' does not denote a CollectionEndPoint.", endPointID);
        throw new ArgumentException (message, "endPointID");
      }

      return (CollectionEndPoint) clientTransaction.DataManager.RelationEndPointMap[endPointID];
    }
  }
}