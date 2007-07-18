using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Transaction
{
  [TestFixture]
  public class SubTransactionCommitRollbackTest : ClientTransactionBaseTest
  {
    private ClientTransaction _subTransaction;

    public override void SetUp ()
    {
      base.SetUp ();
      _subTransaction = ClientTransactionMock.CreateSubTransaction ();
    }

    [Test]
    public void ReturnToParentTransactionReturnsTrue ()
    {
      Assert.AreEqual (true, _subTransaction.ReturnToParentTransaction ());
    }

    [Test]
    public void ReturnToParentTransactionMakesParentWriteable ()
    {
      Assert.IsTrue (_subTransaction.ParentTransaction.IsReadOnly);
      _subTransaction.ReturnToParentTransaction ();
      Assert.IsFalse (_subTransaction.ParentTransaction.IsReadOnly);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The subtransaction can no longer be used because control has "
        + "returned to its parent transaction.")]
    public void ReturnToParentTransactionRendersSubTransactionUnusable ()
    {
      _subTransaction.ReturnToParentTransaction ();
      using (_subTransaction.EnterScope ())
      {
        Order.NewObject ();
      }
    }

    [Test]
    public void SubTransactionCanContinueToBeUsedAfterRollback ()
    {
      _subTransaction.Rollback ();
      using (_subTransaction.EnterScope ())
      {
        Order order = Order.NewObject ();
        Assert.IsNotNull (order);
      }
    }

    [Test]
    public void SubTransactionCanContinueToBeUsedAfterCommit ()
    {
      _subTransaction.Commit ();
      using (_subTransaction.EnterScope ())
      {
        Order order = Order.NewObject ();
        Assert.IsNotNull (order);
      }
    }
  }
}