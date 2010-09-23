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

      var collectionEndPoint = CheckAndGetCollectionEndPoint (clientTransaction, endPointID);
      if (collectionEndPoint != null)
      {
        if (!CanUnloadCollectionEndPoint (collectionEndPoint))
        {
          var message = string.Format ("The end point with ID '{0}' has been changed. Changed end points cannot be unloaded.", endPointID);
          throw new InvalidOperationException (message);
        }

        collectionEndPoint.Unload();
      }

      ProcessTransactionHierarchy (clientTransaction, transactionMode, tx =>
      {
        UnloadCollectionEndPoint (tx, endPointID, transactionMode);
        return true;
      });
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
        if (!CanUnloadCollectionEndPoint (collectionEndPoint))
          return false;

        collectionEndPoint.Unload ();
      }

      return ProcessTransactionHierarchy (clientTransaction, transactionMode, tx => TryUnloadCollectionEndPoint (tx, endPointID, transactionMode));
    }

    /// <summary>
    /// Unloads the data held by the given <see cref="ClientTransaction"/> for the <see cref="DomainObject"/> with the specified 
    /// <paramref name="objectID"/>. The <see cref="DomainObject"/> reference 
    /// and <see cref="DomainObjectCollection"/> instances held by the object are not removed, only the data is. The object can only be unloaded if 
    /// it is in unchanged state and no relation end-points would remain inconsistent.
    /// </summary>
    /// <param name="clientTransaction">The client transaction.</param>
    /// <param name="objectID">The object ID.</param>
    /// <param name="transactionMode">The <see cref="UnloadTransactionMode"/> to use. This can be used to specify whether the unload operation should 
    /// affect this transaction only or the whole transaction hierarchy, up to the root transaction.</param>
    /// <exception cref="InvalidOperationException">The object to be unloaded is not in unchanged state - or - the operation would affect an 
    /// opposite relation end-point that is not in unchanged state.</exception>
    /// <remarks>
    /// <para>
    /// The method unloads the <see cref="DataContainer"/>, the collection end points the object is part of (but not
    /// the collection end points the object owns), the non-virtual end points owned by the object, their respective opposite virtual object 
    /// end-points, and the virtual object end points pointing to <see langword="null" />. This means that unloading an object will unload a relation 
    /// if and only if the object's <see cref="DataContainer"/> is holding the foreign key for the relation or if the relation points from the 
    /// unloaded object to <see langword="null" />.
    /// </para>
    /// <para>
    /// With <see cref="UnloadTransactionMode.RecurseToRoot"/>, the unload operation is not atomic over the transaction hierarchy. It will start at
    /// the given transaction and try to unload here, then it will go over the parent transactions one by one. If the operation fails in any of the
    /// transactions, it will stop and throw an exception. At this point of time, the operation's results will be visible in all
    /// the transactions where it succeeded, but not in the one where it failed or those above.
    /// </para>
    /// </remarks>
    public static void UnloadData (ClientTransaction clientTransaction, ObjectID objectID, UnloadTransactionMode transactionMode)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      var command = clientTransaction.DataManager.CreateUnloadCommand (objectID);
      command.NotifyAndPerform ();

      ProcessTransactionHierarchy (clientTransaction, transactionMode, tx =>
      {
        UnloadData (tx, objectID, transactionMode);
        return true;
      });
    }

    /// <summary>
    /// Unloads the data held by the given <see cref="ClientTransaction"/> for the <see cref="DomainObject"/> with the specified
    /// <paramref name="objectID"/>, returning a value indicating whether the unload operation succeeded. The <see cref="DomainObject"/> reference
    /// and <see cref="DomainObjectCollection"/> instances held by the object are not removed, only the data is. The object can only be unloaded if
    /// it is in unchanged state and no relation end-points would remain inconsistent.
    /// </summary>
    /// <param name="clientTransaction">The client transaction.</param>
    /// <param name="objectID">The object ID.</param>
    /// <param name="transactionMode">The <see cref="UnloadTransactionMode"/> to use. This can be used to specify whether the unload operation should
    /// affect this transaction only or the whole transaction hierarchy, up to the root transaction.</param>
    /// <returns><see langword="true" /> if the unload operation succeeded (in all transactions), or <see langword="false" /> if it did not succeed
    /// (in any transaction).</returns>
    /// <remarks>
    /// 	<para>
    /// The method unloads the <see cref="DataContainer"/>, the collection end points the object is part of (but not
    /// the collection end points the object owns), the non-virtual end points owned by the object, their respective opposite virtual object
    /// end-points, and the virtual object end points pointing to <see langword="null"/>. This means that unloading an object will unload a relation
    /// if and only if the object's <see cref="DataContainer"/> is holding the foreign key for the relation or if the relation points from the
    /// unloaded object to <see langword="null"/>.
    /// </para>
    /// 	<para>
    /// With <see cref="UnloadTransactionMode.RecurseToRoot"/>, the unload operation is not atomic over the transaction hierarchy. It will start at
    /// the given transaction and try to unload here, then it will go over the parent transactions one by one. If the operation fails in any of the
    /// transactions, it will stop and throw an exception. At this point of time, the operation's results will be visible in all
    /// the transactions where it succeeded, but not in the one where it failed or those above.
    /// </para>
    /// </remarks>
    public static bool TryUnloadData (ClientTransaction clientTransaction, ObjectID objectID, UnloadTransactionMode transactionMode)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      var command = clientTransaction.DataManager.CreateUnloadCommand (objectID);
      if (!command.CanUnload)
        return false;
      
      command.NotifyAndPerform ();

      return ProcessTransactionHierarchy (clientTransaction, transactionMode, tx => TryUnloadData (tx, objectID, transactionMode));
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

      ProcessTransactionHierarchy (clientTransaction, transactionMode, tx =>
      {
        UnloadCollectionEndPointAndData (tx, endPointID, transactionMode);
        return true;
      });
    }

    /// <summary>
    /// Unloads the unchanged collection end point indicated by the given <see cref="RelationEndPointID"/> in the specified
    /// <see cref="ClientTransaction"/> as well as the data items stored by it, returning a value indicating whether the unload operation succeeded. 
    /// If the end point has not been loaded or has already been unloaded, this method returns <see langword="true" /> and does nothing.
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
    /// transactions, it will stop and return <see langword="false"/>. At this point of time, the operation's results will be visible in all
    /// the transactions where it succeeded, but not in the one where it failed or those above.
    /// </remarks>
    public static bool TryUnloadCollectionEndPointAndData (
        ClientTransaction clientTransaction,
        RelationEndPointID endPointID,
        UnloadTransactionMode transactionMode)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);

      var endPoint = CheckAndGetCollectionEndPoint (clientTransaction, endPointID);
      if (endPoint != null && endPoint.IsDataAvailable)
      {
        if (!CanUnloadCollectionEndPoint (endPoint))
          return false;

        var unloadedIDs = endPoint.OppositeDomainObjects.Cast<DomainObject> ().Select (obj => obj.ID).ToArray ();
        var command = clientTransaction.DataManager.CreateUnloadCommand (unloadedIDs);
        if (!command.CanUnload)
          return false;

        command.NotifyAndPerform ();
        
        // Still unload the end point, in case the unloadedIDs is empty (if it isn't, the data unload will have unloaded the end point anyway)
        var result = TryUnloadCollectionEndPoint (clientTransaction, endPointID, UnloadTransactionMode.ThisTransactionOnly);
        Assertion.IsTrue (result, "We checked above...");
      }

      return ProcessTransactionHierarchy (
          clientTransaction, transactionMode, tx => TryUnloadCollectionEndPointAndData (tx, endPointID, transactionMode));
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

    private static bool CanUnloadCollectionEndPoint (CollectionEndPoint collectionEndPoint)
    {
      return !collectionEndPoint.HasChanged;
    }

    private static bool ProcessTransactionHierarchy (
        ClientTransaction clientTransaction,
        UnloadTransactionMode transactionMode,
        Func<ClientTransaction, bool> operation)
    {
      if (transactionMode == UnloadTransactionMode.RecurseToRoot && clientTransaction.ParentTransaction != null)
      {
        using (TransactionUnlocker.MakeWriteable (clientTransaction.ParentTransaction))
        {
          return operation (clientTransaction.ParentTransaction);
        }
      }
      else
      {
        return true;
      }
    }
  }
}