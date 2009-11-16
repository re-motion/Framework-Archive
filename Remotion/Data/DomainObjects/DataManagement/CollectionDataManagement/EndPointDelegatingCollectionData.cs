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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement
{
  /// <summary>
  /// Implements the <see cref="IDomainObjectCollectionData"/> by forwarding all requests to an implementation of 
  /// <see cref="ICollectionEndPoint"/>.
  /// </summary>
  public class EndPointDelegatingCollectionData : IDomainObjectCollectionData
  {
    private readonly ICollectionEndPoint _collectionEndPoint;
    private readonly DomainObjectCollectionData _actualData;

    public EndPointDelegatingCollectionData (ICollectionEndPoint collectionEndPoint, DomainObjectCollectionData actualData)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("actualData", actualData);

      _collectionEndPoint = collectionEndPoint;
      _actualData = actualData;
    }

    public ICollectionEndPoint CollectionEndPoint
    {
      get { return _collectionEndPoint; }
    }

    public int Count
    {
      get { return _actualData.Count; }
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    public ICollectionEndPoint AssociatedEndPoint
    {
      get { return _collectionEndPoint; }
    }

    public IDomainObjectCollectionData GetUndecoratedDataStore ()
    {
      return _actualData;
    }

    public bool ContainsObjectID (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      return _actualData.ContainsObjectID (objectID);
    }

    public DomainObject GetObject (int index)
    {
      return _actualData.GetObject (index);
    }

    public DomainObject GetObject (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      return _actualData.GetObject (objectID);
    }

    public int IndexOf (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      return _actualData.IndexOf (objectID);
    }

    public IEnumerator<DomainObject> GetEnumerator ()
    {
      return _actualData.GetEnumerator ();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public void Clear ()
    {
      CheckNotDeleted (CollectionEndPoint.GetDomainObject ());

      // no need to check the inner objects for being deleted or for differing client transactions - we can rely on objects being part of an
      // endpoint fitting this transaction and not being deleted
      Assertion.DebugAssert (this.All (obj => 
          obj.TransactionContext[CollectionEndPoint.ClientTransaction].CanBeUsedInTransaction 
          && obj.TransactionContext[CollectionEndPoint.ClientTransaction].State != StateType.Deleted));

      for (int i = Count - 1; i >= 0; --i)
      {
        var removedObject = GetObject (i);

        CreateAndExecuteRemoveModification (removedObject);
      }

      CollectionEndPoint.Touch ();
    }

    public void Insert (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      CheckClientTransaction (domainObject, "Cannot insert DomainObject '{0}' into collection of property '{1}' of DomainObject '{2}'.");
      CheckNotDeleted (domainObject);
      CheckNotDeleted (CollectionEndPoint.GetDomainObject());

      var insertModification = CollectionEndPoint.CreateInsertModification (domainObject, index);
      var bidirectionalModification = insertModification.CreateBidirectionalModification ();
      bidirectionalModification.ExecuteAllSteps ();

      CollectionEndPoint.Touch ();
    }

    public void Remove (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      CheckClientTransaction (domainObject, "Cannot remove DomainObject '{0}' from collection of property '{1}' of DomainObject '{2}'.");
      CheckNotDeleted (domainObject);
      CheckNotDeleted (CollectionEndPoint.GetDomainObject());

      if (ContainsObjectID (domainObject.ID))
        CreateAndExecuteRemoveModification (domainObject);

      CollectionEndPoint.Touch ();
    }

    public void Remove (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);

      CheckNotDeleted (CollectionEndPoint.GetDomainObject ());

      var domainObject = GetObject (objectID);
      if (domainObject != null)
      {
        // we can rely on the fact that this object is not deleted, otherwise we wouldn't have got it
        Assertion.IsTrue (domainObject.TransactionContext[CollectionEndPoint.ClientTransaction].State != StateType.Deleted);

        CreateAndExecuteRemoveModification (domainObject);
      }

      CollectionEndPoint.Touch ();
    }

    public void Replace (int index, DomainObject newDomainObject)
    {
      ArgumentUtility.CheckNotNull ("newDomainObject", newDomainObject);

      CheckClientTransaction (newDomainObject, "Cannot put DomainObject '{0}' into the collection of property '{1}' of DomainObject '{2}'.");
      CheckNotDeleted (newDomainObject);
      CheckNotDeleted (CollectionEndPoint.GetDomainObject ());

      var replaceModification = CollectionEndPoint.CreateReplaceModification (index, newDomainObject);
      var bidirectionalModification = replaceModification.CreateBidirectionalModification ();
      bidirectionalModification.ExecuteAllSteps ();

      CollectionEndPoint.Touch ();
    }

    private void CreateAndExecuteRemoveModification (DomainObject domainObject)
    {
      var modification = CollectionEndPoint.CreateRemoveModification (domainObject);
      var bidirectionalModification = modification.CreateBidirectionalModification ();
      bidirectionalModification.ExecuteAllSteps ();
    }

    private void CheckClientTransaction (DomainObject domainObject, string exceptionFormatString)
    {
      if (!domainObject.TransactionContext[CollectionEndPoint.ClientTransaction].CanBeUsedInTransaction)
      {
        string transactionInfo = GetTransactionInfoForMismatchingClientTransactions (domainObject);

        var formattedMessage = string.Format (
            exceptionFormatString, 
            domainObject.ID, 
            CollectionEndPoint.Definition.PropertyName, 
            CollectionEndPoint.ObjectID);
        throw new ClientTransactionsDifferException (formattedMessage + " The objects do not belong to the same ClientTransaction." + transactionInfo);
      }
    }

    private string GetTransactionInfoForMismatchingClientTransactions (DomainObject otherDomainObject)
    {
      var transactionInfo = new StringBuilder ();

      var endPointObject = CollectionEndPoint.GetDomainObject ();
      if (otherDomainObject.HasBindingTransaction)
      {
        transactionInfo.AppendFormat (" The {0} object is bound to a BindingClientTransaction.", otherDomainObject.GetPublicDomainObjectType ().Name);
        if (endPointObject.HasBindingTransaction)
        {
          transactionInfo.AppendFormat (
              " The {0} object owning the collection is also bound, but to a different BindingClientTransaction.",
              endPointObject.GetPublicDomainObjectType ().Name);
        }
      }
      else if (endPointObject.HasBindingTransaction)
      {
        transactionInfo.AppendFormat (
            " The {0} object owning the collection is bound to a BindingClientTransaction.", 
            endPointObject.GetPublicDomainObjectType ().Name);
      }
      return transactionInfo.ToString();
    }

    private void CheckNotDeleted (DomainObject domainObject)
    {
      if (domainObject.TransactionContext[CollectionEndPoint.ClientTransaction].State == StateType.Deleted)
        throw new ObjectDeletedException (domainObject.ID);
    }
  }
}
