using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Transaction
{
  [TestFixture]
  public class SubTransactionTest : ClientTransactionBaseTest
  {
    [Test]
    public void NullParentTransaction ()
    {
      Assert.IsNull (ClientTransactionMock.ParentTransaction);
    }

    [Test]
    public void SameRootTransaction ()
    {
      Assert.AreSame (ClientTransactionMock, ClientTransactionMock.RootTransaction);
    }

    [Test]
    public void CreateSubTransaction ()
    {
      ClientTransaction subTransaction = new ClientTransaction (ClientTransactionMock);
      Assert.AreSame (ClientTransactionMock, subTransaction.ParentTransaction);
      Assert.AreSame (ClientTransactionMock, subTransaction.RootTransaction);
    }

    [Test]
    public void CreateSubTransactionInSubTransaction ()
    {
      ClientTransaction subTransaction1 = new ClientTransaction (ClientTransactionMock);
      ClientTransaction subTransaction2 = new ClientTransaction (subTransaction1);
      Assert.AreSame (subTransaction1, subTransaction2.ParentTransaction);
      Assert.AreSame (ClientTransactionMock, subTransaction2.RootTransaction);
    }

    [Test]
    public void CreatingSubTransactionSetsParentReadonly ()
    {
      Assert.IsFalse (ClientTransactionMock.IsReadOnly);
      ClientTransaction subTransaction = new ClientTransaction (ClientTransactionMock);
      Assert.IsTrue (ClientTransactionMock.IsReadOnly);
      Assert.IsFalse (subTransaction.IsReadOnly);

      ClientTransaction subTransaction2 = new ClientTransaction (subTransaction);
      Assert.IsTrue (subTransaction.IsReadOnly);
      Assert.IsFalse (subTransaction2.IsReadOnly);
    }

    [Test]
    public void SubTransactionCanBeUsedToCreateAndLoadNewObjects ()
    {
      ClientTransaction subTransaction = new ClientTransaction (ClientTransactionMock);
      using (subTransaction.EnterScope())
      {
        Assert.AreSame (subTransaction, ClientTransactionScope.CurrentTransaction);
        Order order = Order.NewObject ();
        Assert.AreSame (subTransaction, order.InitialClientTransaction);
        Assert.IsTrue (order.CanBeUsedInTransaction (subTransaction));
        Assert.IsFalse (order.CanBeUsedInTransaction (ClientTransactionMock));

        order.OrderNumber = 4711;
        Assert.AreEqual (4711, order.OrderNumber);

        OrderItem item = OrderItem.NewObject ();
        order.OrderItems.Add (item);
        Assert.IsTrue (order.OrderItems.Contains (item.ID));

        Ceo ceo = Ceo.GetObject (DomainObjectIDs.Ceo1);
        Assert.IsNotNull (ceo);
        Assert.AreSame (subTransaction, ceo.InitialClientTransaction);
        Assert.IsTrue (ceo.CanBeUsedInTransaction (subTransaction));
        Assert.IsFalse (ceo.CanBeUsedInTransaction (ClientTransactionMock));

        Assert.AreSame (ceo.Company, Company.GetObject (DomainObjectIDs.Company1));
      }
    }

    [Test]
    public void DomainObjectsCreatedInParentCanBeUsedInSubTransactions ()
    {
      Order order = Order.NewObject ();
      ClientTransaction subTransaction = new ClientTransaction (ClientTransactionMock);
      Assert.IsTrue (order.CanBeUsedInTransaction (ClientTransactionMock));
      Assert.IsTrue (order.CanBeUsedInTransaction (subTransaction));
    }

    [Test]
    public void DomainObjectsCreatedInSubTransactionCannotBeUsedInParent ()
    {
      ClientTransaction subTransaction = new ClientTransaction (ClientTransactionMock);
      using (subTransaction.EnterScope ())
      {
        Order order = Order.NewObject();
        Assert.IsTrue (order.CanBeUsedInTransaction (subTransaction));
        Assert.IsFalse (order.CanBeUsedInTransaction (ClientTransactionMock));
      }
    }

    [Test]
    public void DomainObjectsLoadedInParentCanBeUsedInSubTransactions ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ClientTransaction subTransaction = new ClientTransaction (ClientTransactionMock);
      Assert.IsTrue (order.CanBeUsedInTransaction (ClientTransactionMock));
      Assert.IsTrue (order.CanBeUsedInTransaction (subTransaction));
    }

    [Test]
    public void DomainObjectsLoadedInSubTransactionCannotBeUsedInParent ()
    {
      ClientTransaction subTransaction = new ClientTransaction (ClientTransactionMock);
      using (subTransaction.EnterScope ())
      {
        Order order = Order.GetObject (DomainObjectIDs.Order1);
        Assert.IsTrue (order.CanBeUsedInTransaction (subTransaction));
        Assert.IsFalse (order.CanBeUsedInTransaction (ClientTransactionMock));
      }
    }

    [Test]
    public void SubTransactionCanAccessObjectNewedInParent ()
    {
      Order order = Order.NewObject ();
      ClientTransaction subTransaction = new ClientTransaction (ClientTransactionMock);
      using (subTransaction.EnterScope ())
      {
        order.OrderNumber = 5;
        order.OrderTicket = OrderTicket.NewObject ();
      }
    }

    [Test]
    public void SubTransactionCanAccessObjectLoadedInParent ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ClientTransaction subTransaction = new ClientTransaction (ClientTransactionMock);
      using (subTransaction.EnterScope ())
      {
        ++order.OrderNumber;
        OrderTicket oldTicket = order.OrderTicket;
        order.OrderTicket = OrderTicket.NewObject ();
      }
    }

    [Test]
    [Ignore ("TODO: FS - SubTransactions")]
    public void SubTransactionHasSamePropertyValuessAsParent ()
    {
      Assert.Fail ();
    }

    [Test]
    [Ignore ("TODO: FS - SubTransactions")]
    public void SubTransactionHasSameRelatedObjectsAsParent ()
    {
      Assert.Fail ();
    }

    [Test]
    [Ignore ("TODO: FS - SubTransactions")]
    public void SubTransactionDataChangesDoNotPropagateToParent ()
    {
      Assert.Fail ();
    }
  }
}