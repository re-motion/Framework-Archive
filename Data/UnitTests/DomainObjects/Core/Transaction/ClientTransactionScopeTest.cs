/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transaction
{
  [TestFixture]
  public class ClientTransactionScopeTest
  {
    private ClientTransactionScope _outermostScope;

    [SetUp]
    public void SetUp ()
    {
      ClientTransactionScope.ResetActiveScope ();
      _outermostScope = ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope();
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
      ClientTransaction clientTransaction = ClientTransaction.CreateRootTransaction();
      Assert.AreNotSame (clientTransaction, ClientTransactionScope.CurrentTransaction);
      using (clientTransaction.EnterNonDiscardingScope ())
      {
        Assert.AreSame (clientTransaction, ClientTransactionScope.CurrentTransaction);
      }
      Assert.AreNotSame (clientTransaction, ClientTransactionScope.CurrentTransaction);
    }

    [Test]
    public void EnterNullScopeSetsNullTransaction ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        Assert.IsTrue (ClientTransactionScope.HasCurrentTransaction);
        using (ClientTransactionScope.EnterNullScope ())
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
      using (ClientTransactionScope scope = ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        Assert.IsNotNull (ClientTransactionScope.ActiveScope);
        Assert.AreSame (scope, ClientTransactionScope.ActiveScope);
      }
    }

    [Test]
    public void NestedScopes ()
    {
      ClientTransaction clientTransaction1 = ClientTransaction.CreateRootTransaction();
      ClientTransaction clientTransaction2 = ClientTransaction.CreateRootTransaction();
      ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;
      ClientTransaction original = ClientTransactionScope.CurrentTransaction;
      
      Assert.AreNotSame (clientTransaction1, original);
      Assert.AreNotSame (clientTransaction2, original);
      Assert.IsNotNull (original);

      using (ClientTransactionScope scope1 = clientTransaction1.EnterNonDiscardingScope ())
      {
        Assert.AreSame (clientTransaction1, ClientTransactionScope.CurrentTransaction);
        Assert.AreSame (scope1, ClientTransactionScope.ActiveScope);

        using (ClientTransactionScope scope2 = clientTransaction2.EnterNonDiscardingScope ())
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
      using (ClientTransactionScope.EnterNullScope ())
      {
        Assert.IsFalse (ClientTransactionScope.HasCurrentTransaction);
        using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
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
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        Assert.IsNotNull (ClientTransactionScope.CurrentTransaction);
        Assert.AreNotSame (original, ClientTransactionScope.CurrentTransaction);
      }
      Assert.AreSame (original, ClientTransactionScope.CurrentTransaction);
    }

    [Test]
    public void ScopeHasTransactionProperty ()
    {
      ClientTransaction outerTransaction = ClientTransaction.CreateRootTransaction();
      ClientTransaction innerTransaction = ClientTransaction.CreateRootTransaction();
      using (ClientTransactionScope outer = outerTransaction.EnterNonDiscardingScope ())
      {
        using (ClientTransactionScope inner = innerTransaction.EnterNonDiscardingScope ())
        {
          Assert.AreSame (innerTransaction, inner.ScopedTransaction);
          Assert.AreSame (outerTransaction, outer.ScopedTransaction);
        }
      }
    }

    [Test]
    public void ScopeHasTransactionProperty_FromITransactionScopeInterface ()
    {
      ClientTransaction outerTransaction = ClientTransaction.CreateRootTransaction ();
      ClientTransaction innerTransaction = ClientTransaction.CreateRootTransaction ();
      using (ClientTransactionScope outer = outerTransaction.EnterNonDiscardingScope ())
      {
        using (ClientTransactionScope inner = innerTransaction.EnterNonDiscardingScope ())
        {
          Assert.AreSame (innerTransaction, ((ITransactionScope)inner).ScopedTransaction);
          Assert.AreSame (outerTransaction, ((ITransactionScope)outer).ScopedTransaction);
        }
      }
    }

    [Test]
    public void ScopeHasAutoRollbackBehavior ()
    {
      using (ClientTransactionScope scope = ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        Assert.AreEqual (AutoRollbackBehavior.Discard, scope.AutoRollbackBehavior);
        scope.AutoRollbackBehavior = AutoRollbackBehavior.None;
        Assert.AreEqual (AutoRollbackBehavior.None, scope.AutoRollbackBehavior);
      }

      using (ClientTransactionScope scope = ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        Assert.AreEqual (AutoRollbackBehavior.Discard, scope.AutoRollbackBehavior);
      }

      using (ClientTransactionScope scope = ClientTransaction.CreateRootTransaction().EnterScope (AutoRollbackBehavior.None))
      {
        Assert.AreEqual (AutoRollbackBehavior.None, scope.AutoRollbackBehavior);
      }

      using (ClientTransactionScope scope = ClientTransaction.CreateRootTransaction ().EnterScope (AutoRollbackBehavior.Rollback))
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

      using (ClientTransactionScope scope = mock.EnterScope (AutoRollbackBehavior.None))
      {
        Order order = Order.GetObject (new DomainObjectIDs ().Order1);
        order.OrderNumber = 0xbadf00d;
        order.OrderTicket = OrderTicket.NewObject ();
        order.OrderItems.Add (OrderItem.NewObject ());
      }

      Assert.AreEqual (0, eventCounter.Rollbacks);

      using (ClientTransactionScope scope = mock.EnterScope (AutoRollbackBehavior.None))
      {
      }

      Assert.AreEqual (0, eventCounter.Rollbacks);

      using (ClientTransactionScope scope = mock.EnterScope (AutoRollbackBehavior.None))
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

      using (ClientTransactionScope scope = mock.EnterScope (AutoRollbackBehavior.Rollback))
      {
        Order order = Order.GetObject (new DomainObjectIDs ().Order1);
        order.OrderNumber = 0xbadf00d;
      }

      Assert.AreEqual (1, eventCounter.Rollbacks);
      eventCounter.Rollbacks = 0;

      using (ClientTransactionScope scope = mock.EnterScope (AutoRollbackBehavior.Rollback))
      {
        Order order = Order.GetObject (new DomainObjectIDs ().Order1);
        order.OrderTicket = OrderTicket.NewObject ();
      }

      Assert.AreEqual (1, eventCounter.Rollbacks);
      eventCounter.Rollbacks = 0;

      using (ClientTransactionScope scope = mock.EnterScope (AutoRollbackBehavior.Rollback))
      {
        Order order = Order.GetObject (new DomainObjectIDs ().Order1);
        order.OrderItems.Add (OrderItem.NewObject ());
      }

      Assert.AreEqual (1, eventCounter.Rollbacks);
      eventCounter.Rollbacks = 0;

      using (ClientTransactionScope scope = mock.EnterScope (AutoRollbackBehavior.Rollback))
      {
      }

      Assert.AreEqual (0, eventCounter.Rollbacks);

      using (ClientTransactionScope scope = mock.EnterScope (AutoRollbackBehavior.Rollback))
      {
        Order order = Order.GetObject (new DomainObjectIDs ().Order1);
        order.OrderNumber = 0xbadf00d;
        scope.ScopedTransaction.Rollback ();
      }

      Assert.AreEqual (1, eventCounter.Rollbacks);
      eventCounter.Rollbacks = 0;

      using (ClientTransactionScope scope = mock.EnterScope (AutoRollbackBehavior.Rollback))
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
      ClientTransaction transaction = ClientTransaction.CreateRootTransaction();
      TransactionEventCounter eventCounter = new TransactionEventCounter (transaction);
      using (ClientTransactionScope scope = transaction.EnterNonDiscardingScope ())
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
      ClientTransactionScope scope = ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ();
      scope.Leave ();
      scope.Leave ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The ClientTransactionScope has already been left.")]
    public void LeaveAndDisposeThrows ()
    {
      using (ClientTransactionScope scope = ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        scope.Leave();
      }
    }

    [Test]
    public void NoAutoEnlistingByDefault ()
    {
      Order order = Order.GetObject (new DomainObjectIDs ().Order1);
      Assert.IsTrue (order.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
      using (ClientTransactionScope scope = ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
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
      ClientTransaction clientTransaction = ClientTransaction.CreateRootTransaction();

      using (ClientTransactionScope scope = clientTransaction.EnterNonDiscardingScope ())
      {
        scope.AutoEnlistDomainObjects = true;
        Assert.IsTrue (order1.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
        scope.AutoEnlistDomainObjects = false;
        Assert.IsTrue (order1.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
        Assert.IsFalse (order2.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
      }

      using (ClientTransactionScope scope = clientTransaction.EnterNonDiscardingScope ())
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
    public void InitialAutoEnlistValueIsInherited ()
    {
      ClientTransaction clientTransaction = ClientTransaction.CreateRootTransaction ();

      using (ClientTransactionScope scope1 = clientTransaction.EnterNonDiscardingScope ())
      {
        Assert.IsFalse (scope1.AutoEnlistDomainObjects);
        scope1.AutoEnlistDomainObjects = true;
        Assert.IsTrue (scope1.AutoEnlistDomainObjects);

        using (ClientTransactionScope scope2 = clientTransaction.EnterNonDiscardingScope ())
        {
          Assert.IsTrue (scope2.AutoEnlistDomainObjects);
          
          scope1.AutoEnlistDomainObjects = false;
          Assert.IsTrue (scope2.AutoEnlistDomainObjects);
          scope1.AutoEnlistDomainObjects = true;
          Assert.IsTrue (scope2.AutoEnlistDomainObjects);

          scope2.AutoEnlistDomainObjects = false;
          Assert.IsFalse (scope2.AutoEnlistDomainObjects);
          Assert.IsTrue (scope1.AutoEnlistDomainObjects);
        }

        Assert.IsTrue (scope1.AutoEnlistDomainObjects);
      }
    }

    [Test]
    public void ResetScope ()
    {
      ClientTransactionScope scope = ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ();
      Assert.IsNotNull (ClientTransactionScope.ActiveScope);
      Assert.IsTrue (ClientTransactionScope.HasCurrentTransaction);
      ClientTransactionScope.ResetActiveScope ();
      Assert.IsNull (ClientTransactionScope.ActiveScope);
      Assert.IsFalse (ClientTransactionScope.HasCurrentTransaction);
    }

    [Test]
    public void AutoDiscardBehavior ()
    {
      MockRepository mockRepository = new MockRepository();

      ClientTransaction subTransaction =
          mockRepository.StrictMock<ClientTransaction> (new Dictionary<Enum, object>(), new ClientTransactionExtensionCollection());

      Expect.Call (subTransaction.EnterScope(AutoRollbackBehavior.Discard))
          .Return ((ClientTransactionScope) PrivateInvoke.CreateInstanceNonPublicCtor (typeof (ClientTransactionScope), subTransaction, AutoRollbackBehavior.Discard));
      Expect.Call (subTransaction.Discard ()).Return (true);

      mockRepository.ReplayAll ();

      using (subTransaction.EnterScope (AutoRollbackBehavior.Discard))
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
        using (ClientTransaction.CreateRootTransaction().EnterScope (AutoRollbackBehavior.Rollback))
        {
          ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope();
        }
      }
      finally
      {
        ClientTransactionScope.ResetActiveScope (); // for TearDown
      }
    }

    [Test]
    public void name ()
    {
ClientTransaction tx1 = ClientTransaction.CreateRootTransaction();
using (tx1.EnterDiscardingScope ())
{

  ClientTransaction tx2 = ClientTransaction.CreateRootTransaction();
  using (tx2.EnterDiscardingScope ())
  {


  }

}
    }
  }
}
