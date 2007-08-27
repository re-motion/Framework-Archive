using System;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Temporarily makes a read-only transaction writeable.
  /// </summary>
  internal struct TransactionUnlocker : IDisposable
  {
    public static TransactionUnlocker MakeWriteable (ClientTransaction transaction)
    {
      return new TransactionUnlocker (transaction);
    }

    private ClientTransaction _transaction;

    private TransactionUnlocker (ClientTransaction transaction)
    {
      Assertion.IsTrue (transaction.IsReadOnly);
      transaction.IsReadOnly = false;
      _transaction = transaction;
    }

    public void Dispose ()
    {
      if (_transaction != null)
      {
        Assertion.IsFalse (_transaction.IsReadOnly);
        _transaction.IsReadOnly = true;
        _transaction = null;
      }
    }
  }
}