using System;
using System.Collections;

namespace Rubicon.Data.DomainObjects.UnitTests.Transaction
{
public class ClientTransactionEventReceiver
{
  // types

  // static members and constants

  // member fields

  private DomainObjectCollection _committedDomainObjects;
  private ArrayList _loadedDomainObjects;
  private ClientTransaction _clientTransaction;

  // construction and disposing

  public ClientTransactionEventReceiver (ClientTransaction clientTransaction)
  {
    _loadedDomainObjects = new ArrayList ();
    _clientTransaction = clientTransaction;
    _clientTransaction.Loaded += new LoadedEventHandler (ClientTransaction_Loaded);
    _clientTransaction.Committed += new CommittedEventHandler (ClientTransaction_Committed);
  }

  // methods and properties

  private void ClientTransaction_Loaded (object sender, LoadedEventArgs args)
  {
    _loadedDomainObjects.Add (args.LoadedDomainObject); 
  }

  private void ClientTransaction_Committed (object sender, CommittedEventArgs args)
  {
    _committedDomainObjects = args.CommittedDomainObjects;
  }

  public ArrayList LoadedDomainObjects 
  {
    get { return _loadedDomainObjects; }
  }

  public DomainObjectCollection CommittedDomainObjects
  {
    get { return _committedDomainObjects; }
  }

  public void Clear ()
  {
    _loadedDomainObjects = new ArrayList ();
  }

  public void Unregister ()
  {
    _clientTransaction.Loaded -= new LoadedEventHandler (ClientTransaction_Loaded);
    _clientTransaction.Committed -= new CommittedEventHandler (ClientTransaction_Committed);
  }
}
}
