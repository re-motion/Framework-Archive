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
using System.Collections;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Utilities;
using System.Collections.Generic;

namespace Remotion.Data.DomainObjects.DataManagement
{
  // TODO 3658: Inject event sink rather than ClientTransaction
  public class DataContainerMap : IFlattenedSerializable, IDataContainerMapReadOnlyView
  {
    // types

    // static members and constants

    // member fields

    private readonly ClientTransaction _clientTransaction;
    private readonly IClientTransactionEventSink _transactionEventSink;
    private readonly DataContainerCollection _dataContainers;

    // construction and disposing

    public DataContainerMap (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      _clientTransaction = clientTransaction;
      _transactionEventSink = clientTransaction.ListenerManager;
      _dataContainers = new DataContainerCollection();
    }

    // methods and properties

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public IClientTransactionEventSink TransactionEventSink
    {
      get { return _transactionEventSink; }
    }

    public DataContainer this [ObjectID id]
    {
      get { return _dataContainers[id]; }
    }

    public int Count
    {
      get { return _dataContainers.Count; }
    }

    public void CommitAllDataContainers ()
    {
      foreach (DataContainer dataContainer in _dataContainers)
        dataContainer.CommitState();
    }

    public void RollbackAllDataContainers ()
    {
      foreach (DataContainer dataContainer in _dataContainers)
        dataContainer.RollbackState();
    }

    public void Register (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      _transactionEventSink.RaiseEvent ((tx, l) => l.DataContainerMapRegistering (tx, dataContainer));
      _dataContainers.Add (dataContainer);
    }

    public void Remove (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      var dataContainer = this[id];
      if (dataContainer == null)
      {
        var message = string.Format ("Data container '{0}' is not part of this map.", id);
        throw new ArgumentException (message, "id");
      }

      _transactionEventSink.RaiseEvent ((tx, l) => l.DataContainerMapUnregistering (tx, dataContainer));
      _dataContainers.Remove (dataContainer);
    }

    public IEnumerator<DataContainer> GetEnumerator ()
    {
      return _dataContainers.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    #region Serialization

    protected DataContainerMap (FlattenedDeserializationInfo info)
        : this (info.GetValueForHandle<ClientTransaction>())
    {
      var dataContainerCount = info.GetValue<int>();
      for (int i = 0; i < dataContainerCount; ++i)
        _dataContainers.Add (info.GetValueForHandle<DataContainer>());
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddHandle (_clientTransaction);

      info.AddValue (_dataContainers.Count);
      foreach (DataContainer dataContainer in _dataContainers)
        info.AddHandle (dataContainer);
    }

    #endregion
  }
}