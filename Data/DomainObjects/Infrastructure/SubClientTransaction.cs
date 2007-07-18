using System;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Represents a transaction that is part of a bigger parent transaction. Any changes made within this subtransaction are not visible in
  /// the parent transaction until the subtransaction is committed, and a commit operation will only commit the changes to the parent transaction, 
  /// not to any storage providers.
  /// </summary>
  /// <remarks>The parent transaction cannot be modified while a subtransaction is active.</remarks>
  [Serializable]
  public class SubClientTransaction : ClientTransaction
  {
    private readonly ClientTransaction _parentTransaction;

    public SubClientTransaction (ClientTransaction parentTransaction)
        : base (ArgumentUtility.CheckNotNull("parentTransaction", parentTransaction).ApplicationData,
            ArgumentUtility.CheckNotNull("parentTransaction", parentTransaction).Extensions)
    {
      ArgumentUtility.CheckNotNull ("parentTransaction", parentTransaction);

      parentTransaction.TransactionEventSink.SubTransactionCreating ();
      parentTransaction.IsReadOnly = true;
      _parentTransaction = parentTransaction;

      DataManager.CopyFrom (_parentTransaction.DataManager);
      parentTransaction.TransactionEventSink.SubTransactionCreated (this);
    }

    public override ClientTransaction ParentTransaction
    {
      get { return _parentTransaction; }
    }

    public override ClientTransaction RootTransaction
    {
      get { return ParentTransaction.RootTransaction; }
    }

    public override bool ReturnToParentTransaction ()
    {
      throw new NotImplementedException();
    }
  }
}