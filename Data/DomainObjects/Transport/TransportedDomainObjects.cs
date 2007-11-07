using System;
using System.Collections.Generic;
using Rubicon.Data.DomainObjects.Transport;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Transport
{
  /// <summary>
  /// Represents the data transported via a <see cref="DomainObjectTransporter"/> object on the target system.
  /// </summary>
  public struct TransportedDomainObjects
  {
    private ClientTransaction _dataTransaction;
    private List<DomainObject> _transportedObjects;

    public TransportedDomainObjects (ClientTransaction dataTransaction, List<DomainObject> transportedObjects)
    {
      ArgumentUtility.CheckNotNull ("dataTransaction", dataTransaction);

      _dataTransaction = dataTransaction;
      _transportedObjects = transportedObjects;
    }

    public ClientTransaction DataTransaction
    {
      get { return _dataTransaction; }
    }

    public IEnumerable<DomainObject> TransportedObjects
    {
      get { return _transportedObjects; }
    }

    public void FinishTransport ()
    {
      FinishTransport (delegate { return true; });
    }

    public void FinishTransport (Func<DomainObject, bool> filter)
    {
      ArgumentUtility.CheckNotNull ("filter", filter);

      if (DataTransaction == null)
        throw new InvalidOperationException ("FinishTransport can only be called once.");

      DataTransaction.AddListener (new TransportFinishTransactionListener (DataTransaction, filter));
      DataTransaction.Commit ();

      _dataTransaction = null;
      _transportedObjects = null;
    }
  }
}