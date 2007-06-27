using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.Transaction
{
  [TestFixture]
  public class ClientTransactionScopeTest
  {
    private ClientTransactionScope _outermostScope;

    [SetUp]
    public void SetUp ()
    {
      _outermostScope = new ClientTransactionScope ();
    }

    [TearDown]
    public void TearDown ()
    {
      _outermostScope.Dispose ();
    }

    [Test]
    public void ScopeSetsAndResetsCurrentTransaction ()
    {
      ClientTransaction clientTransaction = new ClientTransaction ();
      Assert.AreNotSame (clientTransaction, ClientTransactionScope.CurrentTransaction);
      using (new ClientTransactionScope (clientTransaction))
      {
        Assert.AreSame (clientTransaction, ClientTransactionScope.CurrentTransaction);
      }
      Assert.AreNotSame (clientTransaction, ClientTransactionScope.CurrentTransaction);
    }

    [Test]
    public void ScopeSetsNullTransaction ()
    {
      using (new ClientTransactionScope (new ClientTransaction ()))
      {
        Assert.IsTrue (ClientTransactionScope.HasCurrentTransaction);
        using (new ClientTransactionScope (null))
        {
          Assert.IsFalse (ClientTransactionScope.HasCurrentTransaction);
        }
        Assert.IsTrue (ClientTransactionScope.HasCurrentTransaction);
      }
    }

    [Test]
    public void NestedScopes ()
    {
      ClientTransaction clientTransaction1 = new ClientTransaction ();
      ClientTransaction clientTransaction2 = new ClientTransaction ();
      ClientTransaction original = ClientTransactionScope.CurrentTransaction;
      
      Assert.AreNotSame (clientTransaction1, original);
      Assert.AreNotSame (clientTransaction2, original);
      Assert.IsNotNull (original);

      using (new ClientTransactionScope (clientTransaction1))
      {
        Assert.AreSame (clientTransaction1, ClientTransactionScope.CurrentTransaction);
        using (new ClientTransactionScope (clientTransaction2))
        {
          Assert.AreSame (clientTransaction2, ClientTransactionScope.CurrentTransaction);
        }
        Assert.AreSame (clientTransaction1, ClientTransactionScope.CurrentTransaction);
      }
      Assert.AreSame (original, ClientTransactionScope.CurrentTransaction);
    }

    [Test]
    public void LeavesEmptyTransaction ()
    {
      using (new ClientTransactionScope (null))
      {
        Assert.IsFalse (ClientTransactionScope.HasCurrentTransaction);
        using (new ClientTransactionScope (new ClientTransaction()))
        {
          Assert.IsTrue (ClientTransactionScope.HasCurrentTransaction);
        }
        Assert.IsFalse (ClientTransactionScope.HasCurrentTransaction);
      }
    }

    [Test]
    public void ScopeCreatesTransactionWithDefaultCtor ()
    {
      ClientTransaction original = ClientTransactionScope.CurrentTransaction;
      using (new ClientTransactionScope ())
      {
        Assert.IsNotNull (ClientTransactionScope.CurrentTransaction);
        Assert.AreNotSame (original, ClientTransactionScope.CurrentTransaction);
      }
      Assert.AreSame (original, ClientTransactionScope.CurrentTransaction);
    }

    [Test]
    public void ScopeHasTransactionProperty ()
    {
      ClientTransaction outerTransaction = new ClientTransaction ();
      ClientTransaction innerTransaction = new ClientTransaction ();
      using (ClientTransactionScope outer = new ClientTransactionScope (outerTransaction))
      {
        using (ClientTransactionScope inner = new ClientTransactionScope (innerTransaction))
        {
          Assert.AreSame (innerTransaction, inner.ScopedTransaction);
          Assert.AreSame (outerTransaction, outer.ScopedTransaction);
        }
      }
    }

    [Test]
    public void ScopeHasAutoRollbackBehavior ()
    {
      using (ClientTransactionScope scope = new ClientTransactionScope ())
      {
        Assert.AreEqual (AutoRollbackBehavior.Rollback, scope.AutoRollbackBehavior);
        scope.AutoRollbackBehavior = AutoRollbackBehavior.None;
        Assert.AreEqual (AutoRollbackBehavior.None, scope.AutoRollbackBehavior);
      }

      using (ClientTransactionScope scope = new ClientTransactionScope (new ClientTransaction()))
      {
        Assert.AreEqual (AutoRollbackBehavior.None, scope.AutoRollbackBehavior);
      }

      using (ClientTransactionScope scope = new ClientTransactionScope (AutoRollbackBehavior.None))
      {
        Assert.AreEqual (AutoRollbackBehavior.None, scope.AutoRollbackBehavior);
      }

      using (ClientTransactionScope scope = new ClientTransactionScope (new ClientTransaction(), AutoRollbackBehavior.Rollback))
      {
        Assert.AreEqual (AutoRollbackBehavior.Rollback, scope.AutoRollbackBehavior);
      }
    }

    class TransactionEventCounter
    {
      public int Rollbacks = 0;
      public int Commits = 0;

      public TransactionEventCounter (ClientTransaction clientTransaction)
      {
        clientTransaction.RolledBack += new ClientTransactionEventHandler (ClientTransaction_RolledBack);
        clientTransaction.Committed += new ClientTransactionEventHandler (ClientTransaction_Committed);
      }

      void ClientTransaction_RolledBack (object sender, ClientTransactionEventArgs args)
      {
        ++Rollbacks;
      }

      void ClientTransaction_Committed (object sender, ClientTransactionEventArgs args)
      {
        ++Commits;
      }
    }

    [Test]
    public void NoAutoRollbackWhenNoneBehavior ()
    {
      ClientTransactionMock mock = new ClientTransactionMock();
      TransactionEventCounter eventCounter = new TransactionEventCounter(mock);

      using (ClientTransactionScope scope = new ClientTransactionScope (mock, AutoRollbackBehavior.None))
      {
        Order order = Order.GetObject (new DomainObjectIDs ().Order1);
        order.OrderNumber = 0xbadf00d;
        order.OrderTicket = OrderTicket.NewObject ();
        order.OrderItems.Add (OrderItem.NewObject ());
      }

      Assert.AreEqual (0, eventCounter.Rollbacks);

      using (ClientTransactionScope scope = new ClientTransactionScope (mock, AutoRollbackBehavior.None))
      {
      }

      Assert.AreEqual (0, eventCounter.Rollbacks);

      using (ClientTransactionScope scope = new ClientTransactionScope (mock, AutoRollbackBehavior.None))
      {
        Order order = Order.GetObject (new DomainObjectIDs ().Order1);
        order.OrderNumber = 0xbadf00d;
        
        scope.ScopedTransaction.Rollback ();
      }

      Assert.AreEqual (1, eventCounter.Rollbacks);
    }

    [Test]
    public void AutoRollbackWhenRollbackBehavior ()
    {
      ClientTransactionMock mock = new ClientTransactionMock ();
      TransactionEventCounter eventCounter = new TransactionEventCounter (mock);

      using (ClientTransactionScope scope = new ClientTransactionScope (mock, AutoRollbackBehavior.Rollback))
      {
        Order order = Order.GetObject (new DomainObjectIDs ().Order1);
        order.OrderNumber = 0xbadf00d;
      }

      Assert.AreEqual (1, eventCounter.Rollbacks);
      eventCounter.Rollbacks = 0;

      using (ClientTransactionScope scope = new ClientTransactionScope (mock, AutoRollbackBehavior.Rollback))
      {
        Order order = Order.GetObject (new DomainObjectIDs ().Order1);
        order.OrderTicket = OrderTicket.NewObject ();
      }

      Assert.AreEqual (1, eventCounter.Rollbacks);
      eventCounter.Rollbacks = 0;

      using (ClientTransactionScope scope = new ClientTransactionScope (mock, AutoRollbackBehavior.Rollback))
      {
        Order order = Order.GetObject (new DomainObjectIDs ().Order1);
        order.OrderItems.Add (OrderItem.NewObject ());
      }

      Assert.AreEqual (1, eventCounter.Rollbacks);
      eventCounter.Rollbacks = 0;

      using (ClientTransactionScope scope = new ClientTransactionScope (mock, AutoRollbackBehavior.Rollback))
      {
      }

      Assert.AreEqual (0, eventCounter.Rollbacks);

      using (ClientTransactionScope scope = new ClientTransactionScope (mock, AutoRollbackBehavior.Rollback))
      {
        Order order = Order.GetObject (new DomainObjectIDs ().Order1);
        order.OrderNumber = 0xbadf00d;
        scope.ScopedTransaction.Rollback ();
      }

      Assert.AreEqual (1, eventCounter.Rollbacks);
      eventCounter.Rollbacks = 0;

      using (ClientTransactionScope scope = new ClientTransactionScope (mock, AutoRollbackBehavior.Rollback))
      {
        Order order = Order.GetObject (new DomainObjectIDs ().Order1);
        order.OrderNumber = 0xbadf00d;
        scope.ScopedTransaction.Rollback ();

        order.OrderNumber = 0xbadf00d;
      }

      Assert.AreEqual (2, eventCounter.Rollbacks);
    }

    [Test]
    public void CommitAndRollbackOnScope ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      TransactionEventCounter eventCounter = new TransactionEventCounter (transaction);
      using (ClientTransactionScope scope = new ClientTransactionScope (transaction))
      {
        Assert.AreEqual (0, eventCounter.Commits);
        Assert.AreEqual (0, eventCounter.Rollbacks);

        scope.Commit ();

        Assert.AreEqual (1, eventCounter.Commits);
        Assert.AreEqual (0, eventCounter.Rollbacks);

        scope.Rollback ();

        Assert.AreEqual (1, eventCounter.Commits);
        Assert.AreEqual (1, eventCounter.Rollbacks);

        transaction.Commit ();

        Assert.AreEqual (2, eventCounter.Commits);
        Assert.AreEqual (1, eventCounter.Rollbacks);

        transaction.Rollback ();

        Assert.AreEqual (2, eventCounter.Commits);
        Assert.AreEqual (2, eventCounter.Rollbacks);
      }
    }
  }
}
