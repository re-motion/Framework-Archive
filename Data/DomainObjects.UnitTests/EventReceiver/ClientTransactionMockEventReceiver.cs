using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.EventReceiver
{
  public class ClientTransactionMockEventReceiver
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public ClientTransactionMockEventReceiver (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      clientTransaction.Loaded += new ClientTransactionEventHandler (Loaded);
      clientTransaction.Committing += new ClientTransactionEventHandler (Committing);
      clientTransaction.Committed += new ClientTransactionEventHandler (Committed);
    }

    // methods and properties

    public virtual void Loaded (object sender, ClientTransactionEventArgs args)
    {
    }

    public virtual void Committing (object sender, ClientTransactionEventArgs args)
    {
    }

    public virtual void Committed (object sender, ClientTransactionEventArgs args)
    {
    }
  }
}
