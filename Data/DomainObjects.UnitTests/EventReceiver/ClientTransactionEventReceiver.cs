using System;
using System.Collections;

namespace Rubicon.Data.DomainObjects.UnitTests.EventReceiver
{
public class ClientTransactionEventReceiver
{
  // types

  // static members and constants

  // member fields

  private ArrayList _loadedDomainObjects;
  private ClientTransaction _clientTransaction;

  // construction and disposing

  public ClientTransactionEventReceiver (ClientTransaction clientTransaction)
  {
    _loadedDomainObjects = new ArrayList ();
    _clientTransaction = clientTransaction;
    _clientTransaction.Loaded += new LoadedEventHandler (ClientTransaction_Loaded);
  }

  // methods and properties

  private void ClientTransaction_Loaded (object sender, LoadedEventArgs args)
  {
    _loadedDomainObjects.Add (args.LoadedDomainObject); 
  }

  public ArrayList LoadedDomainObjects 
  {
    get { return _loadedDomainObjects; }
  }

  public void Clear ()
  {
    _loadedDomainObjects = new ArrayList ();
  }

  public void Unregister ()
  {
    _clientTransaction.Loaded -= new LoadedEventHandler (ClientTransaction_Loaded);
  }
}
}
