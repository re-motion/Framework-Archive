using NUnit.Framework;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{
  public class TestTransactedFunctionWithNestedFunction : WxeTransactedFunctionBase<TestTransaction>
  {
    private readonly TestTransaction _transactionBeforeExecution;
    private TestTransaction _transactionInExecution;

    protected override WxeTransactionBase<TestTransaction> CreateWxeTransaction ()
    {
      return new WxeTestTransaction ();
    }

    public new TestTransaction Transaction
    {
      get { return base.Transaction; }
    }

    public TestTransactedFunctionWithNestedFunction (TestTransaction transactionBefore, WxeTransactedFunctionBase<TestTransaction> nestedFunction)
    {
      _transactionBeforeExecution = transactionBefore;
      
      Add (new WxeMethodStep (CheckBeforeNestedFunction));
      Add (nestedFunction);
      Add (new WxeMethodStep (CheckAfterNestedFunction));
    }

    private void CheckBeforeNestedFunction ()
    {
      Assert.AreNotSame (_transactionBeforeExecution, TestTransaction.Current);
      _transactionInExecution = TestTransaction.Current;
    }

    private void CheckAfterNestedFunction ()
    {
      Assert.AreSame (_transactionInExecution, TestTransaction.Current);
    }
  }
}