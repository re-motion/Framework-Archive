using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.Infrastructure;

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
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
        + "ClientTransaction is read-only. Offending transaction modification: SubTransactionCreating.")]
    public void NoTwoSubTransactionsAtSameTime ()
    {
      ClientTransaction subTransaction1 = new ClientTransaction (ClientTransactionMock);
      ClientTransaction subTransaction2 = new ClientTransaction (ClientTransactionMock);
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
    public void SubTransactionHasSamePropertyValuessAsParent ()
    {
      Order newUnchangedOrder = Order.NewObject ();
      int newUnchangedOrderNumber = newUnchangedOrder.OrderNumber;

      Order newChangedOrder = Order.NewObject ();
      newChangedOrder.OrderNumber = 4711;

      Order loadedUnchangedOrder = Order.GetObject (DomainObjectIDs.Order1);
      int loadedUnchangedOrderNumber = loadedUnchangedOrder.OrderNumber;

      Order loadedChangedOrder = Order.GetObject (DomainObjectIDs.Order2);
      loadedChangedOrder.OrderNumber = 13;

      using (new ClientTransaction (ClientTransactionMock).EnterScope ())
      {
        Assert.AreSame (loadedUnchangedOrder, Order.GetObject (DomainObjectIDs.Order1));
        Assert.AreSame (loadedChangedOrder, Order.GetObject (DomainObjectIDs.Order2));

        Assert.AreEqual (newUnchangedOrderNumber, newUnchangedOrder.OrderNumber);
        Assert.AreEqual (4711, newChangedOrder.OrderNumber);
        Assert.AreEqual (loadedUnchangedOrderNumber, loadedUnchangedOrder.OrderNumber);
        Assert.AreEqual (13, loadedChangedOrder.OrderNumber);
      }
    }

    [Test]
    public void PropertyValueChangedAreNotPropagatedToParent ()
    {
      Order newChangedOrder = Order.NewObject ();
      newChangedOrder.OrderNumber = 4711;

      Order loadedChangedOrder = Order.GetObject (DomainObjectIDs.Order2);
      loadedChangedOrder.OrderNumber = 13;

      using (new ClientTransaction (ClientTransactionMock).EnterScope ())
      {
        newChangedOrder.OrderNumber = 17;
        loadedChangedOrder.OrderNumber = 4;

        using (ClientTransactionMock.EnterScope ())
        {
          Assert.AreEqual (4711, newChangedOrder.OrderNumber);
          Assert.AreEqual (13, loadedChangedOrder.OrderNumber);
        }
      }
    }

    [Test]
    public void SubTransactionHasSameRelatedObjectCollectionAsParent ()
    {
      Order loadedOrder = Order.GetObject (DomainObjectIDs.Order1);
      ObjectList<OrderItem> loadedItems = loadedOrder.OrderItems;

      Assert.AreSame (loadedItems, loadedOrder.OrderItems);

      OrderItem loadedItem1 = loadedOrder.OrderItems[0];
      OrderItem loadedItem2 = loadedOrder.OrderItems[1];
      OrderItem newItem1 = OrderItem.NewObject ();
      OrderItem newItem2 = OrderItem.NewObject ();
      newItem2.Product = "Baz, buy two get three for free";

      loadedOrder.OrderItems.Clear ();
      loadedOrder.OrderItems.Add (loadedItem2);
      loadedOrder.OrderItems.Add (newItem1);
      loadedOrder.OrderItems.Add (newItem2);

      Order newOrder = Order.NewObject ();
      OrderItem newItem3 = OrderItem.NewObject();
      newItem3.Product = "FooBar, the energy bar with the extra Foo";
      newOrder.OrderItems.Add (newItem3);

      using (new ClientTransaction (ClientTransactionMock).EnterScope())
      {
        Assert.AreSame (loadedOrder, Order.GetObject (DomainObjectIDs.Order1));
        Assert.AreNotSame (loadedItems, loadedOrder.OrderItems);

        Assert.AreEqual (3, loadedOrder.OrderItems.Count);

        Assert.AreSame (loadedItem2, loadedOrder.OrderItems[0]);
        Assert.AreSame (newItem1, loadedOrder.OrderItems[1]);
        Assert.AreSame (newItem2, loadedOrder.OrderItems[2]);

        Assert.AreSame (loadedOrder, loadedItem2.Order);
        Assert.AreSame (loadedOrder, newItem1.Order);
        Assert.AreSame (loadedOrder, newItem2.Order);

        Assert.AreEqual ("Baz, buy two get three for free", loadedOrder.OrderItems[2].Product);

        Assert.AreEqual (1, newOrder.OrderItems.Count);
        Assert.AreSame (newItem3, newOrder.OrderItems[0]);
        Assert.AreEqual ("FooBar, the energy bar with the extra Foo", newOrder.OrderItems[0].Product);
        Assert.AreSame (newOrder, newItem3.Order);
      }
    }

    [Test]
    public void RelatedObjectCollectionChangesAreNotPropagatedToParent ()
    {
      Order loadedOrder = Order.GetObject (DomainObjectIDs.Order1);

      Assert.AreEqual (2, loadedOrder.OrderItems.Count);
      OrderItem loadedItem1 = loadedOrder.OrderItems[0];
      OrderItem loadedItem2 = loadedOrder.OrderItems[1];

      Order newOrder = Order.NewObject ();

      using (new ClientTransaction (ClientTransactionMock).EnterScope ())
      {
        loadedOrder.OrderItems.Clear ();
        newOrder.OrderItems.Add (OrderItem.NewObject ());

        using (ClientTransactionMock.EnterScope ())
        {
          Assert.AreEqual (2, loadedOrder.OrderItems.Count);
          Assert.AreSame (loadedItem1, loadedOrder.OrderItems[0]);
          Assert.AreSame (loadedItem2, loadedOrder.OrderItems[1]);
          Assert.AreEqual (0, newOrder.OrderItems.Count);
        }
      }
    }

    [Test]
    public void SubTransactionHasSameRelatedObjectAsParent1To1 ()
    {
      Computer loadedComputer = Computer.GetObject (DomainObjectIDs.Computer1);
      Employee loadedEmployee = Employee.GetObject (DomainObjectIDs.Employee1);
      Assert.AreNotSame (loadedComputer.Employee, loadedEmployee);
      loadedComputer.Employee = loadedEmployee;

      Assert.AreSame (loadedEmployee, loadedComputer.Employee);
      Assert.AreSame (loadedComputer, loadedEmployee.Computer);

      Computer newComputer = Computer.NewObject ();
      Employee newEmployee = Employee.NewObject ();
      newEmployee.Computer = newComputer;

      using (new ClientTransaction (ClientTransactionMock).EnterScope ())
      {
        Assert.AreSame (loadedComputer, Computer.GetObject (DomainObjectIDs.Computer1));
        Assert.AreSame (loadedEmployee, Employee.GetObject (DomainObjectIDs.Employee1));

        Assert.AreSame (loadedEmployee, loadedComputer.Employee);
        Assert.AreSame (loadedComputer, loadedEmployee.Computer);

        Assert.AreSame (newComputer, newEmployee.Computer);
        Assert.AreSame (newEmployee, newComputer.Employee);
      }
    }

    [Test]
    public void RelatedObjectChangesAreNotPropagatedToParent ()
    {
      Computer loadedComputer = Computer.GetObject (DomainObjectIDs.Computer1);
      Employee loadedEmployee = Employee.GetObject (DomainObjectIDs.Employee3);

      Computer newComputer = Computer.NewObject ();
      Employee newEmployee = Employee.NewObject ();
      newEmployee.Computer = newComputer;

      using (new ClientTransaction (ClientTransactionMock).EnterScope ())
      {
        loadedComputer.Employee = Employee.NewObject ();
        loadedEmployee.Computer = Computer.NewObject ();

        newComputer.Employee = Employee.NewObject ();
        newEmployee.Computer = Computer.NewObject ();

        using (ClientTransactionMock.EnterScope ())
        {
          Assert.AreSame (loadedComputer, loadedEmployee.Computer);
          Assert.AreSame (loadedEmployee, loadedComputer.Employee);

          Assert.AreSame (newComputer, newEmployee.Computer);
          Assert.AreSame (newEmployee, newComputer.Employee);
        }
      }
    }
  }
}