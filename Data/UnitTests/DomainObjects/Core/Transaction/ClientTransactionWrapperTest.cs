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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transaction
{
  [TestFixture]
  public class ClientTransactionAsITransactionTest : ClientTransactionBaseTest
  {
    private ITransaction _transaction;

    public override void SetUp ()
    {
      base.SetUp();

      _transaction = ClientTransactionMock.ToITransation();
    }

    [Test]
    public void To_ClientTransaction ()
    {
      ClientTransactionMock actual = _transaction.To<ClientTransactionMock>();

      Assert.That (actual, Is.SameAs (ClientTransactionMock));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage =
        "Argument TTransaction is a Remotion.Data.DomainObjects.DomainObject, "
        + "which cannot be assigned to type Remotion.Data.DomainObjects.ClientTransaction.\r\nParameter name: TTransaction")]
    public void To_InvalidType ()
    {
      _transaction.To<DomainObject>();
    }

    [Test]
    public void CanCreateChild ()
    {
      Assert.IsTrue (_transaction.CanCreateChild);
    }

    [Test]
    public void CreateChild ()
    {
      ITransaction child = _transaction.CreateChild();
      Assert.IsNotNull (child);
      Assert.IsInstanceOfType (typeof (ClientTransactionWrapper), child);
      Assert.IsInstanceOfType (typeof (SubClientTransaction), ((ClientTransactionWrapper) child).WrappedInstance);
    }

    [Test]
    public void IsChild ()
    {
      ITransaction child = _transaction.CreateChild();
      Assert.IsTrue (child.IsChild);
      Assert.IsFalse (_transaction.IsChild);
      Assert.IsTrue (child.CreateChild().IsChild);
    }

    [Test]
    public void Parent ()
    {
      ITransaction child = _transaction.CreateChild();
      Assert.AreSame (((ClientTransactionWrapper) _transaction).WrappedInstance, ((ClientTransactionWrapper) child.Parent).WrappedInstance);
      Assert.AreSame (((ClientTransactionWrapper) child).WrappedInstance, ((ClientTransactionWrapper) child.CreateChild().Parent).WrappedInstance);
    }

    [Test]
    public void Release ()
    {
      ITransaction child = _transaction.CreateChild();
      Assert.IsTrue (((ClientTransactionWrapper) _transaction).WrappedInstance.IsReadOnly);
      Assert.IsFalse (((ClientTransactionWrapper) child).WrappedInstance.IsDiscarded);
      child.Release();
      Assert.IsFalse (((ClientTransactionWrapper) _transaction).WrappedInstance.IsReadOnly);
      Assert.IsTrue (((ClientTransactionWrapper) child).WrappedInstance.IsDiscarded);
    }

    [Test]
    public void EnterScope ()
    {
      ITransaction transaction = ClientTransaction.CreateRootTransaction().ToITransation();

      ClientTransactionScope.ResetActiveScope();
      Assert.That (ClientTransactionScope.ActiveScope, Is.Null);

      ITransactionScope transactionScope = transaction.EnterScope();

      Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (transactionScope));
      Assert.That (ClientTransactionScope.ActiveScope.ScopedTransaction, Is.SameAs (((ClientTransactionWrapper) transaction).WrappedInstance));
      Assert.That (ClientTransactionScope.ActiveScope.AutoRollbackBehavior, Is.EqualTo (AutoRollbackBehavior.None));
      ClientTransactionScope.ResetActiveScope();
    }

    [Test]
    public void RegisterObjects ()
    {
      ClientTransaction firstClientTransaction = ClientTransaction.CreateRootTransaction();

      DomainObject domainObject1;
      DomainObject domainObject2;
      using (firstClientTransaction.EnterNonDiscardingScope())
      {
        domainObject1 = RepositoryAccessor.GetObject (DomainObjectIDs.ClassWithAllDataTypes2, false);
        domainObject2 = RepositoryAccessor.GetObject (DomainObjectIDs.Partner1, false);
      }

      var secondClientTransaction = ClientTransactionMock;
      ITransaction secondTransaction = secondClientTransaction.ToITransation();
      Assert.That (domainObject1.CanBeUsedInTransaction (secondClientTransaction), Is.False);
      Assert.That (domainObject2.CanBeUsedInTransaction (secondClientTransaction), Is.False);

      secondTransaction.RegisterObjects (new object[] { null, domainObject1, 1, domainObject2 });

      Assert.That (domainObject1.CanBeUsedInTransaction (secondClientTransaction), Is.True);
      Assert.That (secondClientTransaction.DataManager.DataContainerMap.GetObjectWithoutLoading (domainObject1.ID, false), Is.Not.Null);

      Assert.That (domainObject2.CanBeUsedInTransaction (secondClientTransaction), Is.True);
      Assert.That (secondClientTransaction.DataManager.DataContainerMap.GetObjectWithoutLoading (domainObject2.ID, false), Is.Not.Null);
    }

    [Test]
    [ExpectedException (typeof (BulkLoadException),
        ExpectedMessage = "There were errors when loading a bulk of DomainObjects:\r\nObject 'Partner|",
        MatchType = MessageMatch.Contains)]
    public void RegisterObjects_WithNewObject ()
    {
      ClientTransaction firstClientTransaction = ClientTransaction.CreateRootTransaction();

      DomainObject domainObject1;
      DomainObject domainObject2;
      using (firstClientTransaction.EnterNonDiscardingScope())
      {
        domainObject1 = RepositoryAccessor.GetObject (DomainObjectIDs.ClassWithAllDataTypes2, false);
        domainObject2 = Partner.NewObject();
      }

      var secondClientTransaction = ClientTransactionMock;
      ITransaction secondTransaction = secondClientTransaction.ToITransation();

      secondTransaction.RegisterObjects (new object[] { null, domainObject1, 1, domainObject2 });
    }

    [Test]
    public void Reset_RootTransaction ()
    {
      ClientTransaction clientTransactionBefore = ClientTransaction.CreateRootTransaction();
      ITransaction transaction = clientTransactionBefore.ToITransation();
      Order order;
      bool addedCalled;
      bool loadedCalled;

      using (clientTransactionBefore.EnterNonDiscardingScope())
      {
        order = Order.GetObject (DomainObjectIDs.Order1);
        order.OrderNumber = 7;
        clientTransactionBefore.Rollback();

        addedCalled = false;
        order.OrderItems.Added += delegate { addedCalled = true; };

        loadedCalled = false;
        clientTransactionBefore.Loaded += delegate { loadedCalled = true; };

        transaction.Reset();
        Assert.That (clientTransactionBefore.IsDiscarded, Is.True);
      }

      ClientTransaction clientTransactionAfter = ((ClientTransactionWrapper) transaction).WrappedInstance;

      Assert.That (clientTransactionAfter, Is.Not.SameAs (clientTransactionBefore));

      using (clientTransactionAfter.EnterNonDiscardingScope())
      {
        Assert.That (order.CanBeUsedInTransaction (clientTransactionAfter), Is.True);
        Assert.That (order.OrderNumber, Is.EqualTo (1));

        Assert.That (addedCalled, Is.False);
        order.OrderItems.Add (OrderItem.NewObject());
        Assert.That (addedCalled, Is.True);

        loadedCalled = false;

        Order.GetObject (DomainObjectIDs.Order2);

        Assert.That (loadedCalled, Is.True);
      }
    }

    [Test]
    public void Reset_SubTransaction ()
    {
      ClientTransaction rootTransaction = ClientTransaction.CreateRootTransaction();
      ClientTransaction clientTransactionBefore = rootTransaction.CreateSubTransaction();
      ITransaction transaction = clientTransactionBefore.ToITransation();
      Order order;
      bool addedCalled;
      bool loadedCalled;

      using (clientTransactionBefore.EnterNonDiscardingScope())
      {
        order = Order.GetObject (DomainObjectIDs.Order1);
        order.OrderNumber = 7;
        clientTransactionBefore.Rollback();

        addedCalled = false;
        order.OrderItems.Added += delegate { addedCalled = true; };

        loadedCalled = false;
        clientTransactionBefore.Loaded += delegate { loadedCalled = true; };

        transaction.Reset();
        Assert.That (clientTransactionBefore.IsDiscarded, Is.True);
      }

      ClientTransaction clientTransactionAfter = ((ClientTransactionWrapper) transaction).WrappedInstance;

      Assert.That (clientTransactionAfter, Is.Not.SameAs (clientTransactionBefore));
      Assert.That (clientTransactionAfter.RootTransaction, Is.SameAs (rootTransaction));

      using (clientTransactionAfter.EnterNonDiscardingScope())
      {
        Assert.That (order.CanBeUsedInTransaction (clientTransactionAfter), Is.True);
        Assert.That (order.OrderNumber, Is.EqualTo (1));

        Assert.That (addedCalled, Is.False);
        order.OrderItems.Add (OrderItem.NewObject());
        Assert.That (addedCalled, Is.True);

        loadedCalled = false;

        Order.GetObject (DomainObjectIDs.Order2);

        Assert.That (loadedCalled, Is.True);
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "The transaction cannot be reset as it is read-only. The reason might be an open child transaction.")]
    public void Reset_RootTransaction_WithSubTransaction_Throws ()
    {
      ClientTransaction rootTransaction = ClientTransaction.CreateRootTransaction();
      rootTransaction.CreateSubTransaction();
      ITransaction transaction = rootTransaction.ToITransation ();
      transaction.Reset();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "The transaction cannot be reset as it is in a dirty state and needs to be committed or rolled back.")]
    public void Reset_RootTransaction_HasChanged_Throws ()
    {
      ClientTransaction rootTransaction = ClientTransaction.CreateRootTransaction();
      ITransaction transaction = rootTransaction.ToITransation();
      using (rootTransaction.EnterNonDiscardingScope ())
      {
        Order.NewObject();
        transaction.Reset();
      }
    }
  }
}