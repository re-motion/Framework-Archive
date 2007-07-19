using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.DataManagement;
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

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException), ExpectedMessage = "Object 'Order.*' is already discarded.", MatchType = MessageMatch.Regex)]
    public void RollbackResetsNewedObjects ()
    {
      using (_subTransaction.EnterScope ())
      {
        Order order = Order.NewObject();
        _subTransaction.Rollback();
        int i = order.OrderNumber;
      }
    }

    [Test]
    public void RollbackResetsLoadedObjects ()
    {
      using (_subTransaction.EnterScope ())
      {
        Order order = Order.GetObject (DomainObjectIDs.Order1);
        order.OrderNumber = 5;

        _subTransaction.Rollback ();

        Assert.AreNotEqual (5, order.OrderNumber);
      }
    }

    [Test]
    public void RollbackResetsPropertyValuesToThoseOfParentTransaction ()
    {
      _subTransaction.ReturnToParentTransaction ();

      Order loadedOrder = Order.GetObject (DomainObjectIDs.Order1);
      Order newOrder = Order.NewObject ();

      loadedOrder.OrderNumber = 5;
      newOrder.OrderNumber = 7;

      using (ClientTransactionMock.CreateSubTransaction().EnterScope ())
      {
        loadedOrder.OrderNumber = 13;
        newOrder.OrderNumber = 47;

        ClientTransactionScope.CurrentTransaction.Rollback ();

        Assert.AreEqual (5, loadedOrder.OrderNumber);
        Assert.AreEqual (7, newOrder.OrderNumber);
      }
    }

    [Test]
    public void RollbackResetsRelatedObjectsToThoseOfParentTransaction ()
    {
      _subTransaction.ReturnToParentTransaction ();

      Order newOrder = Order.NewObject ();
      OrderItem orderItem = OrderItem.NewObject ();
      newOrder.OrderItems.Add (orderItem);
      Assert.AreEqual (1, newOrder.OrderItems.Count);
      Assert.IsTrue (newOrder.OrderItems.ContainsObject (orderItem));

      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        newOrder.OrderItems.Clear ();
        newOrder.OrderItems.Add (OrderItem.NewObject ());
        newOrder.OrderItems.Add (OrderItem.NewObject ());
        Assert.AreEqual (2, newOrder.OrderItems.Count);
        Assert.IsFalse (newOrder.OrderItems.ContainsObject (orderItem));

        ClientTransactionScope.CurrentTransaction.Rollback ();

        Assert.AreEqual (1, newOrder.OrderItems.Count);
        Assert.IsTrue (newOrder.OrderItems.ContainsObject (orderItem));
      }
    }

    [Test]
    public void RollbackResetsRelatedObjectToThatOfParentTransaction ()
    {
      _subTransaction.ReturnToParentTransaction ();

      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      Employee employee = computer.Employee;
      Location location = Location.NewObject ();
      Client client = Client.NewObject ();
      location.Client = client;

      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        computer.Employee = Employee.NewObject ();
        location.Client = null;
        Assert.IsNull (employee.Computer);

        ClientTransactionScope.CurrentTransaction.Rollback ();

        Assert.AreSame (employee, computer.Employee);
        Assert.AreSame (computer, employee.Computer);
        Assert.AreSame (client, location.Client);
      }
    }

    [Ignore ("TODO: FS - ClientTransactions")]
    public void Commit ()
    {
      Assert.Fail ();
    }
  }
}