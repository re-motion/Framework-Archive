using System;
using System.Collections;

namespace Rubicon.Data.DomainObjects.UnitTests.EventReceiver
{
public class ClientTransactionEventReceiver
{
  // types

  // static members and constants

  // member fields

  private ClientTransaction _clientTransaction;
  private ArrayList _loadedDomainObjects;
  private ArrayList _committingDomainObjects;
  private ArrayList _committedDomainObjects;

  // construction and disposing

  public ClientTransactionEventReceiver (ClientTransaction clientTransaction)
  {
    _loadedDomainObjects = new ArrayList ();
    _committingDomainObjects = new ArrayList ();
    _committedDomainObjects = new ArrayList ();
    _clientTransaction = clientTransaction;

    _clientTransaction.Loaded += new LoadedEventHandler (ClientTransaction_Loaded);
    _clientTransaction.Committing += new CommitEventHandler (ClientTransaction_Committing);
    _clientTransaction.Committed += new CommitEventHandler (ClientTransaction_Committed);
  }

  // methods and properties

  public void Clear ()
  {
    _loadedDomainObjects = new ArrayList ();
    _committingDomainObjects = new ArrayList ();
    _committedDomainObjects = new ArrayList ();
  }

  public void Unregister ()
  {
    _clientTransaction.Loaded -= new LoadedEventHandler (ClientTransaction_Loaded);
    _clientTransaction.Committing -= new CommitEventHandler (ClientTransaction_Committing);
    _clientTransaction.Committed -= new CommitEventHandler (ClientTransaction_Committed);
  }

  private void ClientTransaction_Loaded (object sender, LoadedEventArgs args)
  {
    _loadedDomainObjects.Add (args.LoadedDomainObject); 
  }

  private void ClientTransaction_Committing (object sender, CommitEventArgs args)
  {
    _committingDomainObjects.Add (args.DomainObjects);
  }

  private void ClientTransaction_Committed (object sender, CommitEventArgs args)
  {
    _committedDomainObjects.Add (args.DomainObjects);
  }

  public ArrayList LoadedDomainObjects 
  {
    get { return _loadedDomainObjects; }
  }

  public ArrayList CommittingDomainObjects
  {
    get { return _committingDomainObjects; }
  }

  public ArrayList CommittedDomainObjects
  {
    get { return _committedDomainObjects; }
  }
}
}
