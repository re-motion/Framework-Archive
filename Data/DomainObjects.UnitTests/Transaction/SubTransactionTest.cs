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
    public void CreateSubTransaction ()
    {
      ClientTransaction subTransaction = new ClientTransaction (ClientTransactionMock);
      Assert.IsNotNull (subTransaction);
      Assert.AreNotSame (ClientTransactionMock, subTransaction);
      Assert.AreSame (ClientTransactionMock, subTransaction.ParentTransaction);
    }

    [Test]
    public void CreatingSubTransactionSetsParentReadonly ()
    {
      Assert.IsFalse (ClientTransactionMock.IsReadOnly);
      ClientTransaction subTransaction = new ClientTransaction (ClientTransactionMock);
      Assert.IsTrue (ClientTransactionMock.IsReadOnly);
      Assert.IsFalse (subTransaction.IsReadOnly);
    }

    [Test]
    public void SubTransactionCanBeUsedToCreateAndLoadNewObjects ()
    {
      ClientTransaction subTransaction = new ClientTransaction (ClientTransactionMock);
      using (new ClientTransactionScope (subTransaction))
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
    [Ignore ("TODO: FS - SubTransactions")]
    public void SubTransactionHasSamePropertyValuesAsParent ()
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