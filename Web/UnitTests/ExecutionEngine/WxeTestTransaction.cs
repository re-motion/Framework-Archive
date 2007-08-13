using System.Collections.Generic;
using NUnit.Framework;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{
  public class WxeTestTransaction : WxeTransactionBase<TestTransaction>
  {
    private readonly Stack<TestTransaction> _previousTransactionsStack = new Stack<TestTransaction> ();

    public WxeTestTransaction ()
        : base (null, false, true)
    {
    }

    protected override TestTransaction CurrentTransaction
    {
      get { return TestTransaction.Current; }
    }

    protected override void SetCurrentTransaction (TestTransaction transaction)
    {
      _previousTransactionsStack.Push (TestTransaction.Current);
      TestTransaction.Current = transaction;
    }

    protected override void SetPreviousCurrentTransaction (TestTransaction previousTransaction)
    {
      TestTransaction storedPreviousTransaction = _previousTransactionsStack.Pop ();
      Assert.AreSame (storedPreviousTransaction, previousTransaction);
      TestTransaction.Current = previousTransaction;
    }

    protected override TestTransaction CreateRootTransaction ()
    {
      return new TestTransaction ();
    }
  }
}