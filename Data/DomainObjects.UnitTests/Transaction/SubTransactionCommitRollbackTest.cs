using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Transaction
{
  [TestFixture]
  public class SubTransactionCommitRollbackTest : ClientTransactionBaseTest
  {
    [Test]
    public void SubTransactionCanContinueToBeUsedAfterRollback ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction ();
      subTransaction.Rollback ();
      using (subTransaction.EnterScope ())
      {
        Order order = Order.NewObject ();
        Assert.IsNotNull (order);
      }
    }

    [Test]
    public void SubTransactionCanContinueToBeUsedAfterCommit ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction ();
      subTransaction.Commit ();
      using (subTransaction.EnterScope ())
      {
        Order order = Order.NewObject ();
        Assert.IsNotNull (order);
      }
    }
  }
}