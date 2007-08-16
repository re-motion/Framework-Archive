using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
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
      ClientTransactionScope.ResetActiveScope ();
      _outermostScope = new ClientTransactionScope ();
    }

    [TearDown]
    public void TearDown ()
    {
      if (ClientTransactionScope.ActiveScope != null)
      {
        Assert.AreSame (_outermostScope, ClientTransactionScope.ActiveScope);
        _outermostScope.Leave();
      }
    }

    [Test]
    public void ScopeSetsAndResetsCurrentTransaction ()
    {
      ClientTransaction clientTransaction = ClientTransaction.NewTransaction();
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
      using (new ClientTransactionScope (ClientTransaction.NewTransaction()))
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
    public void ActiveScope ()
    {
      _outermostScope.Leave();
      Assert.IsNull (ClientTransactionScope.ActiveScope);
      using (ClientTransactionScope scope = new ClientTransactionScope ())
      {
        Assert.IsNotNull (ClientTransactionScope.ActiveScope);
        Assert.AreSame (scope, ClientTransactionScope.ActiveScope);
      }
    }

    [Test]
    public void NestedScopes ()
    {
      ClientTransaction clientTransaction1 = ClientTransaction.NewTransaction();
      ClientTransaction clientTransaction2 = ClientTransaction.NewTransaction();
      ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;
      ClientTransaction original = ClientTransactionScope.CurrentTransaction;
      
      Assert.AreNotSame (clientTransaction1, original);
      Assert.AreNotSame (clientTransaction2, original);
      Assert.IsNotNull (original);

      using (ClientTransactionScope scope1 = new ClientTransactionScope (clientTransaction1))
      {
        Assert.AreSame (clientTransaction1, ClientTransactionScope.CurrentTransaction);
        Assert.AreSame (scope1, ClientTransactionScope.ActiveScope);

        using (ClientTransactionScope scope2 = new ClientTransactionScope (clientTransaction2))
        {
          Assert.AreSame (scope2, ClientTransactionScope.ActiveScope);
          Assert.AreSame (clientTransaction2, ClientTransactionScope.CurrentTransaction);
        }
        Assert.AreSame (scope1, ClientTransactionScope.ActiveScope);
        Assert.AreSame (clientTransaction1, ClientTransactionScope.CurrentTransaction);
      }
      Assert.AreSame (originalScope, ClientTransactionScope.ActiveScope);
      Assert.AreSame (original, ClientTransactionScope.CurrentTransaction);
    }

    [Test]
    public void LeavesEmptyTransaction ()
    {
      using (new ClientTransactionScope (null))
      {
        Assert.IsFalse (ClientTransactionScope.HasCurrentTransaction);
        using (new ClientTransactionScope (ClientTransaction.NewTransaction()))
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
      ClientTransaction outerTransaction = ClientTransaction.NewTransaction();
      ClientTransaction innerTransaction = ClientTransaction.NewTransaction();
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

      using (ClientTransactionScope scope = new ClientTransactionScope (ClientTransaction.NewTransaction()))
      {
        Assert.AreEqual (AutoRollbackBehavior.None, scope.AutoRollbackBehavior);
      }

      using (ClientTransactionScope scope = new ClientTransactionScope (AutoRollbackBehavior.None))
      {
        Assert.AreEqual (AutoRollbackBehavior.None, scope.AutoRollbackBehavior);
      }

      using (ClientTransactionScope scope = new ClientTransactionScope (ClientTransaction.NewTransaction(), AutoRollbackBehavior.Rollback))
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
      ClientTransaction transaction = ClientTransaction.NewTransaction();
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

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The ClientTransactionScope has already been left.")]
    public void LeaveTwiceThrows ()
    {
      ClientTransactionScope scope = new ClientTransactionScope ();
      scope.Leave ();
      scope.Leave ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The ClientTransactionScope has already been left.")]
    public void LeaveAndDisposeThrows ()
    {
      using (ClientTransactionScope scope = new ClientTransactionScope ())
      {
        scope.Leave();
      }
    }

    [Test]
    public void NoAutoEnlistingByDefault ()
    {
      Order order = Order.GetObject (new DomainObjectIDs ().Order1);
      Assert.IsTrue (order.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
      using (ClientTransactionScope scope = new ClientTransactionScope ())
      {
        Assert.IsFalse (scope.AutoEnlistDomainObjects);
        Assert.IsFalse (order.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
      }
    }

    [Test]
    public void AutoEnlistingIfRequested ()
    {
      Order order1 = Order.GetObject (new DomainObjectIDs ().Order1);
      Order order2 = Order.GetObject (new DomainObjectIDs ().Order2);
      
      Assert.IsTrue (order1.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
      ClientTransaction clientTransaction = ClientTransaction.NewTransaction();

      using (ClientTransactionScope scope = clientTransaction.EnterScope())
      {
        scope.AutoEnlistDomainObjects = true;
        Assert.IsTrue (order1.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
        scope.AutoEnlistDomainObjects = false;
        Assert.IsTrue (order1.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
        Assert.IsFalse (order2.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
      }

      using (ClientTransactionScope scope = clientTransaction.EnterScope ())
      {
        Assert.IsFalse (scope.AutoEnlistDomainObjects);
        Assert.IsFalse (order2.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
        scope.AutoEnlistDomainObjects = true;
        Assert.IsTrue (order2.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
        scope.AutoEnlistDomainObjects = false;
        Assert.IsTrue (order2.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
      }
    }

    [Test]
    public void ResetScope ()
    {
      ClientTransactionScope scope = new ClientTransactionScope ();
      Assert.IsNotNull (ClientTransactionScope.ActiveScope);
      Assert.IsTrue (ClientTransactionScope.HasCurrentTransaction);
      ClientTransactionScope.ResetActiveScope ();
      Assert.IsNull (ClientTransactionScope.ActiveScope);
      Assert.IsFalse (ClientTransactionScope.HasCurrentTransaction);
    }

    [Test]
    public void AutoReturnToParentBehavior ()
    {
      MockRepository mockRepository = new MockRepository();

      ClientTransaction subTransaction =
          mockRepository.CreateMock<ClientTransaction> (new Dictionary<Enum, object>(), new ClientTransactionExtensionCollection());

      Expect.Call (subTransaction.ReturnToParentTransaction ()).Return (true);

      mockRepository.ReplayAll ();

      using (new ClientTransactionScope (subTransaction, AutoRollbackBehavior.ReturnToParent))
      {
      }

      mockRepository.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "This ClientTransactionScope is not the active scope. Leave the active scope before leaving this one.")]
    public void LeaveNonActiveScopeThrows ()
    {
      try
      {
        using (new ClientTransactionScope (AutoRollbackBehavior.Rollback))
        {
          new ClientTransactionScope ();
        }
      }
      finally
      {
        ClientTransactionScope.ResetActiveScope (); // for TearDown
      }
    }
  }
}
