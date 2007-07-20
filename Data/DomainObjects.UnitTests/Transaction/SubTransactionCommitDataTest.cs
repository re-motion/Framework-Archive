using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Transaction
{
  [TestFixture]
  [Ignore ("TODO: FS - SubTransactions Commit")]
  public class SubTransactionCommitDataTest : ClientTransactionBaseTest
  {
    [Test]
    public void CommitPropagatesChangesToLoadedObjectsToParentTransaction ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransactionMock.CreateSubTransaction().EnterScope ())
      {
        order.OrderNumber = 5;

        ClientTransactionScope.CurrentTransaction.Commit ();

        Assert.AreEqual (5, order.OrderNumber);
      }

      Assert.IsNotNull (order);
      Assert.AreEqual (5, order.OrderNumber);
    }

    [Test]
    public void CommitPropagatesChangesToNewObjectsToParentTransaction ()
    {
      Order order = Order.NewObject ();
      using (ClientTransactionMock.CreateSubTransaction().EnterScope ())
      {
        order.OrderNumber = 7;

        ClientTransactionScope.CurrentTransaction.Commit ();
      }

      Assert.AreEqual (7, order.OrderNumber);
    }

    [Test]
    public void CommitLeavesUnchangedObjectsLoadedInSub ()
    {
      Order order;
      using (ClientTransactionMock.CreateSubTransaction().EnterScope ())
      {
        order = Order.GetObject (DomainObjectIDs.Order1);

        ClientTransactionScope.CurrentTransaction.Commit ();
        Assert.AreEqual (1, order.OrderNumber);
      }

      Assert.AreEqual (1, order.OrderNumber);
    }

    [Test]
    public void CommitLeavesUnchangedObjectsLoadedInRoot ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransactionMock.CreateSubTransaction().EnterScope ())
      {
        ClientTransactionScope.CurrentTransaction.Commit ();
        Assert.AreEqual (1, order.OrderNumber);
      }

      Assert.AreEqual (1, order.OrderNumber);
    }

    [Test]
    public void CommitLeavesUnchangedNewObjects ()
    {
      Order order = Order.NewObject ();
      using (ClientTransactionMock.CreateSubTransaction().EnterScope ())
      {
        ClientTransactionScope.CurrentTransaction.Commit ();
        Assert.AreEqual (0, order.OrderNumber);
      }

      Assert.AreEqual (0, order.OrderNumber);
    }

    [Test]
    public void CommitSavesPropertyValuesToParentTransaction ()
    {
      Order loadedOrder = Order.GetObject (DomainObjectIDs.Order1);
      Order newOrder = Order.NewObject ();

      loadedOrder.OrderNumber = 5;
      newOrder.OrderNumber = 7;

      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        loadedOrder.OrderNumber = 13;
        newOrder.OrderNumber = 47;

        ClientTransactionScope.CurrentTransaction.Commit ();

        Assert.AreEqual (StateType.Unchanged, loadedOrder.State);
        Assert.AreEqual (StateType.Unchanged, newOrder.State);

        Assert.AreEqual (13, loadedOrder.OrderNumber);
        Assert.AreEqual (47, newOrder.OrderNumber);
      }

      Assert.AreEqual (13, loadedOrder.OrderNumber);
      Assert.AreEqual (47, newOrder.OrderNumber);

      Assert.AreEqual (StateType.Changed, loadedOrder.State);
      Assert.AreEqual (StateType.Changed, newOrder.State);
    }

    [Test]
    public void CommitSavesRelatedObjectsToParentTransaction ()
    {
      Order newOrder = Order.NewObject ();
      OrderItem orderItem = OrderItem.NewObject ();
      newOrder.OrderItems.Add (orderItem);

      Assert.AreEqual (1, newOrder.OrderItems.Count);
      Assert.IsTrue (newOrder.OrderItems.ContainsObject (orderItem));

      OrderItem newOrderItem;

      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        newOrder.OrderItems.Clear ();
        newOrderItem = OrderItem.NewObject ();
        newOrder.OrderItems.Add (newOrderItem);
        newOrder.OrderItems.Add (OrderItem.NewObject ());

        Assert.AreEqual (2, newOrder.OrderItems.Count);
        Assert.IsFalse (newOrder.OrderItems.ContainsObject (orderItem));
        Assert.IsTrue (newOrder.OrderItems.ContainsObject (newOrderItem));

        ClientTransactionScope.CurrentTransaction.Commit ();

        Assert.AreEqual (StateType.Unchanged, newOrder.State);

        Assert.AreEqual (2, newOrder.OrderItems.Count);
        Assert.IsFalse (newOrder.OrderItems.ContainsObject (orderItem));
        Assert.IsTrue (newOrder.OrderItems.ContainsObject (newOrderItem));
      }

      Assert.AreEqual (2, newOrder.OrderItems.Count);
      Assert.IsFalse (newOrder.OrderItems.ContainsObject (orderItem));
      Assert.IsTrue (newOrder.OrderItems.ContainsObject (newOrderItem));
    }

    [Test]
    public void CommitSavesRelatedObjectToParentTransaction ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      Employee employee = computer.Employee;
      Location location1 = Location.NewObject ();
      Location location2 = Location.NewObject ();

      Client client = Client.NewObject ();
      location1.Client = client;

      Employee newEmployee;
      Client newClient = Client.NewObject ();

      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        newEmployee = Employee.NewObject ();
        computer.Employee = newEmployee;

        location1.Client = null;

        newClient = Client.NewObject ();
        location2.Client = newClient;

        Assert.IsNull (employee.Computer);
        Assert.AreSame (newEmployee, computer.Employee);
        Assert.IsNull (location1.Client);
        Assert.AreSame (newClient, location2.Client);

        ClientTransactionScope.CurrentTransaction.Rollback ();

        Assert.IsNull (employee.Computer);
        Assert.AreSame (newEmployee, computer.Employee);
        Assert.IsNull (location1.Client);
        Assert.AreSame (newClient, location2.Client);
      }

      Assert.IsNull (employee.Computer);
      Assert.AreSame (newEmployee, computer.Employee);
      Assert.IsNull (location1.Client);
      Assert.AreSame (newClient, location2.Client);
    }

    [Test]
    public void SubCommitDoesNotCommitParent ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        order.OrderNumber = 5;
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
      Assert.AreEqual (5, order.OrderNumber);
      ClientTransactionMock.Rollback ();
      Assert.AreEqual (1, order.OrderNumber);
    }
  }
}