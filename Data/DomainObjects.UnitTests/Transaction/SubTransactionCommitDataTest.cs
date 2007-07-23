using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Transaction
{
  [TestFixture]
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
      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.NewObject ();
      using (ClientTransactionMock.CreateSubTransaction().EnterScope ())
      {
        classWithAllDataTypes.Int32Property = 7;

        ClientTransactionScope.CurrentTransaction.Commit ();
      }

      Assert.AreEqual (7, classWithAllDataTypes.Int32Property);
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
      ClassWithAllDataTypes newClassWithAllDataTypes = ClassWithAllDataTypes.NewObject ();

      loadedOrder.OrderNumber = 5;
      newClassWithAllDataTypes.Int16Property = 7;

      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        loadedOrder.OrderNumber = 13;
        newClassWithAllDataTypes.Int16Property = 47;

        ClientTransactionScope.CurrentTransaction.Commit ();

        Assert.AreEqual (StateType.Unchanged, loadedOrder.State);
        Assert.AreEqual (StateType.Unchanged, newClassWithAllDataTypes.State);

        Assert.AreEqual (13, loadedOrder.OrderNumber);
        Assert.AreEqual (47, newClassWithAllDataTypes.Int16Property);
      }

      Assert.AreEqual (13, loadedOrder.OrderNumber);
      Assert.AreEqual (47, newClassWithAllDataTypes.Int16Property);

      Assert.AreEqual (StateType.Changed, loadedOrder.State);
      Assert.AreEqual (StateType.New, newClassWithAllDataTypes.State);
    }

    [Test]
    public void CommitSavesRelatedObjectsToParentTransaction ()
    {
      Order order = Order.NewObject ();
      Official official = Official.GetObject (DomainObjectIDs.Official1);
      order.Official = official;
      order.Customer = Customer.GetObject (DomainObjectIDs.Customer1);

      OrderItem orderItem = OrderItem.NewObject ();
      order.OrderItems.Add (orderItem);

      Assert.AreSame (official, order.Official);
      Assert.AreEqual (1, order.OrderItems.Count);
      Assert.IsTrue (order.OrderItems.ContainsObject (orderItem));
      Assert.IsNull (order.OrderTicket);

      OrderItem newOrderItem;
      OrderTicket newOrderTicket;

      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        newOrderItem = OrderItem.NewObject ();

        orderItem.Delete ();
        order.OrderItems.Add (newOrderItem);
        order.OrderItems.Add (OrderItem.NewObject ());

        newOrderTicket = OrderTicket.NewObject ();
        order.OrderTicket = newOrderTicket;

        Assert.AreSame (official, order.Official);
        Assert.AreEqual (2, order.OrderItems.Count);
        Assert.IsFalse (order.OrderItems.ContainsObject (orderItem));
        Assert.IsTrue (order.OrderItems.ContainsObject (newOrderItem));
        Assert.IsNotNull (order.OrderTicket);
        Assert.AreSame (newOrderTicket, order.OrderTicket);

        ClientTransactionScope.CurrentTransaction.Commit ();

        Assert.AreEqual (StateType.Unchanged, order.State);

        Assert.AreSame (official, order.Official);
        Assert.AreEqual (2, order.OrderItems.Count);
        Assert.IsFalse (order.OrderItems.ContainsObject (orderItem));
        Assert.IsTrue (order.OrderItems.ContainsObject (newOrderItem));
        Assert.IsNotNull (order.OrderTicket);
        Assert.AreSame (newOrderTicket, order.OrderTicket);
      }

      Assert.AreSame (official, order.Official);
      Assert.AreEqual (2, order.OrderItems.Count);
      Assert.IsFalse (order.OrderItems.ContainsObject (orderItem));
      Assert.IsTrue (order.OrderItems.ContainsObject (newOrderItem));
      Assert.IsNotNull (order.OrderTicket);
      Assert.AreSame (newOrderTicket, order.OrderTicket);
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
      Client newClient1 = Client.NewObject ();
      Client newClient2;

      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        newEmployee = Employee.NewObject ();
        computer.Employee = newEmployee;

        location1.Client = newClient1;

        newClient2 = Client.NewObject ();
        location2.Client = newClient2;

        Assert.IsNull (employee.Computer);
        Assert.AreSame (newEmployee, computer.Employee);
        Assert.AreSame (newClient1, location1.Client);
        Assert.AreSame (newClient2, location2.Client);

        ClientTransactionScope.CurrentTransaction.Commit ();

        Assert.IsNull (employee.Computer);
        Assert.AreSame (newEmployee, computer.Employee);
        Assert.AreSame (newClient1, location1.Client);
        Assert.AreSame (newClient2, location2.Client);
      }

      Assert.IsNull (employee.Computer);
      Assert.AreSame (newEmployee, computer.Employee);
      Assert.AreSame (newClient1, location1.Client);
      Assert.AreSame (newClient2, location2.Client);
    }
  }
}