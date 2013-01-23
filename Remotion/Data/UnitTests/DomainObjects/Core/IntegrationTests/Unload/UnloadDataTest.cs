// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Synchronization;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Unload
{
  [TestFixture]
  public class UnloadDataTest : UnloadTestBase
  {
    [Test]
    public void UnloadData_OrderTicket ()
    {
      var orderTicket1 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      var order = orderTicket1.Order;
      order.EnsureDataAvailable ();

      Assert.That (orderTicket1.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));

      UnloadService.UnloadData (TestableClientTransaction, orderTicket1.ID);

      CheckDataContainerExists (orderTicket1, false);
      CheckDataContainerExists (order, true);

      CheckEndPointExists (orderTicket1, "Order", false);
      CheckEndPointExists (order, "OrderTicket", false);

      Assert.That (orderTicket1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void UnloadData_OrderTicket_ReloadData ()
    {
      var orderTicket1 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      var order = orderTicket1.Order;
      order.EnsureDataAvailable ();

      Assert.That (orderTicket1.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));

      UnloadService.UnloadData (TestableClientTransaction, orderTicket1.ID);

      CheckDataContainerExists (orderTicket1, false);
      CheckDataContainerExists (order, true);

      // Data reload

      CheckDataContainerExists (orderTicket1, false);

      Assert.That (orderTicket1.FileName, Is.EqualTo ("C:\\order1.png"));

      CheckDataContainerExists (orderTicket1, true);
      CheckDataContainerExists (order, true);
    }

    [Test]
    public void UnloadData_OrderTicket_ReloadRelation_OneToOne_FromRealSide ()
    {
      var orderTicket1 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      var order = orderTicket1.Order;
      order.EnsureDataAvailable ();

      Assert.That (orderTicket1.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));

      UnloadService.UnloadData (TestableClientTransaction, orderTicket1.ID);

      CheckEndPointExists (orderTicket1, "Order", false);
      CheckEndPointExists (order, "OrderTicket", false);

      // 1:1 relation reload from real side

      Assert.That (orderTicket1.Order, Is.SameAs (order));

      CheckEndPointExists (orderTicket1, "Order", true);
      CheckVirtualEndPointExistsAndComplete (order, "OrderTicket", true, true);
    }

    [Test]
    public void UnloadData_OrderTicket_ReloadRelation_OneToOne_FromVirtualSide ()
    {
      var orderTicket1 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      var order = orderTicket1.Order;
      order.EnsureDataAvailable ();

      Assert.That (orderTicket1.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));

      UnloadService.UnloadData (TestableClientTransaction, orderTicket1.ID);

      CheckEndPointExists (orderTicket1, "Order", false);
      CheckEndPointExists (order, "OrderTicket", false);

      // 1:1 relation reload from virtual side

      Assert.That (order.OrderTicket, Is.SameAs (orderTicket1));

      CheckEndPointExists (orderTicket1, "Order", true);
      CheckVirtualEndPointExistsAndComplete (order, "OrderTicket", true, true);
    }

    [Test]
    public void UnloadData_Computer_WithUnsynchronizedOppositeEndPoint ()
    {
      SetDatabaseModifyable();

      var computer1 = Computer.GetObject (DomainObjectIDs.Computer1);
      var employee = computer1.Employee;
      employee.EnsureDataAvailable ();

      var unsynchronizedComputerID =
          RelationInconcsistenciesTestHelper.CreateObjectAndSetRelationInOtherTransaction<Computer, Employee> (employee.ID, (ot, o) => ot.Employee = o);
      var unsynchronizedComputer = Computer.GetObject (unsynchronizedComputerID);

      Assert.That (computer1.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (unsynchronizedComputer.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (employee.State, Is.EqualTo (StateType.Unchanged));

      CheckEndPointExists (computer1, "Employee", true);
      CheckEndPointExists (unsynchronizedComputer, "Employee", true);
      CheckVirtualEndPointExistsAndComplete (employee, "Computer", true, true);

      UnloadService.UnloadData (TestableClientTransaction, computer1.ID);

      CheckDataContainerExists (computer1, false);
      CheckDataContainerExists (unsynchronizedComputer, true);
      CheckDataContainerExists (employee, true);

      CheckEndPointExists (computer1, "Employee", false);
      CheckEndPointExists (unsynchronizedComputer, "Employee", true);
      CheckVirtualEndPointExistsAndComplete (employee, "Computer", true, false);

      Assert.That (computer1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (unsynchronizedComputer.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (employee.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void UnloadData_Order ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var orderItems = order1.OrderItems;
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];
      var orderTicket = order1.OrderTicket;
      var customer = order1.Customer;
      var customerOrders = customer.Orders;
      customerOrders.EnsureDataComplete ();

      customer.EnsureDataAvailable ();

      Assert.That (order1.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItems.IsDataComplete, Is.True);
      Assert.That (orderItemA.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItemB.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderTicket.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (customer.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (customerOrders.IsDataComplete, Is.True);

      UnloadService.UnloadData (TestableClientTransaction, order1.ID);

      CheckDataContainerExists (order1, false);
      CheckDataContainerExists (orderItemA, true);
      CheckDataContainerExists (orderItemB, true);
      CheckDataContainerExists (orderTicket, true);
      CheckDataContainerExists (customer, true);

      CheckEndPointExists (orderTicket, "Order", true);
      CheckEndPointExists (order1, "OrderTicket", true);
      CheckEndPointExists (orderItemA, "Order", true);
      CheckEndPointExists (orderItemB, "Order", true);
      CheckVirtualEndPointExistsAndComplete (order1, "OrderItems", true, true);
      CheckEndPointExists (order1, "Customer", false);
      CheckVirtualEndPointExistsAndComplete (customer, "Orders", true, false);

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItems.IsDataComplete, Is.True);
      Assert.That (orderItemA.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItemB.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderTicket.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (customer.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (customerOrders.IsDataComplete, Is.False);
    }

    [Test]
    public void UnloadData_Order_RelationAccess_OneToMany_FromRealSide ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];

      UnloadService.UnloadData (TestableClientTransaction, order1.ID);

      CheckDataContainerExists (order1, false);
      CheckDataContainerExists (orderItemA, true);
      CheckDataContainerExists (orderItemB, true);

      CheckEndPointExists (orderItemA, "Order", true);
      CheckEndPointExists (orderItemB, "Order", true);
      CheckVirtualEndPointExistsAndComplete (order1, "OrderItems", true, true);

      Assert.That (orderItemA.Order, Is.SameAs (order1));
      Assert.That (orderItemB.Order, Is.SameAs (order1));

      CheckDataContainerExists (order1, true); // Relation access reloads object, although this is not really necessary
      CheckDataContainerExists (orderItemA, true);
      CheckDataContainerExists (orderItemB, true);

      CheckEndPointExists (orderItemA, "Order", true);
      CheckEndPointExists (orderItemB, "Order", true);
      CheckVirtualEndPointExistsAndComplete (order1, "OrderItems", true, true);
    }

    [Test]
    public void UnloadData_Order_RelationAccess_OneToMany_FromVirtualSide ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];

      UnloadService.UnloadData (TestableClientTransaction, order1.ID);

      CheckDataContainerExists (order1, false);
      CheckDataContainerExists (orderItemA, true);
      CheckDataContainerExists (orderItemB, true);

      CheckEndPointExists (orderItemA, "Order", true);
      CheckEndPointExists (orderItemB, "Order", true);
      CheckVirtualEndPointExistsAndComplete (order1, "OrderItems", true, true);

      Assert.That (order1.OrderItems, Is.EqualTo (new[] { orderItemA, orderItemB }));

      CheckDataContainerExists (order1, true); // Relation access reloads object, although this is not really necessary
      CheckDataContainerExists (orderItemA, true);
      CheckDataContainerExists (orderItemB, true);

      CheckEndPointExists (orderItemA, "Order", true);
      CheckEndPointExists (orderItemB, "Order", true);
      CheckVirtualEndPointExistsAndComplete (order1, "OrderItems", true, true);
    }

    [Test]
    public void UnloadData_OrderItem ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var orderItems = order1.OrderItems;
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];

      Assert.That (order1.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItems.IsDataComplete, Is.True);
      Assert.That (orderItemA.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItemB.State, Is.EqualTo (StateType.Unchanged));

      UnloadService.UnloadData (TestableClientTransaction, orderItemA.ID);

      CheckDataContainerExists (order1, true);
      CheckDataContainerExists (orderItemA, false);
      CheckDataContainerExists (orderItemB, true);

      CheckEndPointExists (orderItemA, "Order", false);
      CheckEndPointExists (orderItemB, "Order", true);
      CheckVirtualEndPointExistsAndComplete (order1, "OrderItems", true, false);

      Assert.That (order1.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderItems.IsDataComplete, Is.False);
      Assert.That (orderItemA.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItemB.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void UnloadData_OrderItem_AfterCommit ()
    {
      SetDatabaseModifyable();

      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var orderItems = order1.OrderItems;
      var newOrderItem = OrderItem.NewObject();
      orderItems.Add (newOrderItem);

      TestableClientTransaction.Commit();

      Assert.That (newOrderItem.Order, Is.SameAs (order1));
      Assert.That (newOrderItem.State, Is.EqualTo (StateType.Unchanged));

      UnloadService.UnloadData (TestableClientTransaction, newOrderItem.ID);

      Assert.That (newOrderItem.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (orderItems.IsDataComplete, Is.False);
    }

    [Test]
    public void UnloadData_OrderItem_ReloadRelation_OneToMany_FromRealSide ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];

      UnloadService.UnloadData (TestableClientTransaction, orderItemA.ID);

      CheckDataContainerExists (order1, true);
      CheckDataContainerExists (orderItemA, false);
      CheckDataContainerExists (orderItemB, true);

      CheckEndPointExists (orderItemA, "Order", false);
      CheckEndPointExists (orderItemB, "Order", true);
      CheckVirtualEndPointExistsAndComplete (order1, "OrderItems", true, false);

      Assert.That (orderItemA.Order, Is.SameAs (order1));

      CheckDataContainerExists (order1, true);
      CheckDataContainerExists (orderItemA, true);
      CheckDataContainerExists (orderItemB, true);

      CheckEndPointExists (orderItemA, "Order", true);
      CheckEndPointExists (orderItemB, "Order", true);
      CheckVirtualEndPointExistsAndComplete (order1, "OrderItems", true, false);
    }

    [Test]
    public void UnloadData_OrderItem_ReloadRelation_OneToMany_FromVirtualSide ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];

      UnloadService.UnloadData (TestableClientTransaction, orderItemA.ID);

      CheckDataContainerExists (order1, true);
      CheckDataContainerExists (orderItemA, false);
      CheckDataContainerExists (orderItemB, true);

      CheckEndPointExists (orderItemA, "Order", false);
      CheckEndPointExists (orderItemB, "Order", true);
      CheckVirtualEndPointExistsAndComplete (order1, "OrderItems", true, false);

      Assert.That (order1.OrderItems, Is.EquivalentTo (new[] { orderItemA, orderItemB }));

      CheckDataContainerExists (order1, true);
      CheckDataContainerExists (orderItemA, true);
      CheckDataContainerExists (orderItemB, true);

      CheckEndPointExists (orderItemA, "Order", true);
      CheckEndPointExists (orderItemB, "Order", true);
      CheckVirtualEndPointExistsAndComplete (order1, "OrderItems", true, true);
    }

    [Test]
    public void UnloadData_ReloadChanges_PropertyValue ()
    {
      SetDatabaseModifyable();
      var order1 = Order.GetObject (DomainObjectIDs.Order1);

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var orderInOtherTx = Order.GetObject (order1.ID);
        orderInOtherTx.OrderNumber = 4711;
        ClientTransaction.Current.Commit ();
      }

      Assert.That (order1.OrderNumber, Is.EqualTo (1));

      UnloadService.UnloadData (TestableClientTransaction, order1.ID);

      Assert.That (order1.OrderNumber, Is.EqualTo (4711));
    }

    [Test]
    public void UnloadData_ReloadChanges_ForeignKey ()
    {
      SetDatabaseModifyable ();
      var computer1 = Computer.GetObject (DomainObjectIDs.Computer1);

      IObjectID<DomainObject> newEmployeeID;
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var computerInOtherTx = Computer.GetObject (computer1.ID);
        computerInOtherTx.Employee = Employee.NewObject ();
        newEmployeeID = computerInOtherTx.Employee.ID;
        ClientTransaction.Current.Commit ();
      }

      Assert.That (computer1.Employee, Is.SameAs (Employee.GetObject (DomainObjectIDs.Employee3)));

      UnloadService.UnloadData (TestableClientTransaction, computer1.ID);

      Assert.That (computer1.Employee, Is.SameAs (Employee.GetObject (newEmployeeID)));
    }

    [Test]
    public void UnloadData_AlreadyUnloaded ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      UnloadService.UnloadData (TestableClientTransaction, order1.ID);
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (TestableClientTransaction.GetEnlistedDomainObject (DomainObjectIDs.Order1), Is.SameAs (order1));

      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (TestableClientTransaction);

      UnloadService.UnloadData (TestableClientTransaction, order1.ID);

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void UnloadData_NonLoadedObject ()
    {
      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (TestableClientTransaction);

      UnloadService.UnloadData (TestableClientTransaction, DomainObjectIDs.Order1);

      Assert.That (TestableClientTransaction.GetEnlistedDomainObject (DomainObjectIDs.Order1), Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The state of the following DataContainers prohibits that they be unloaded; only unchanged DataContainers can be unloaded: "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' (Changed).")]
    public void UnloadData_Changed ()
    {
      ++Order.GetObject (DomainObjectIDs.Order1).OrderNumber;
      UnloadService.UnloadData (TestableClientTransaction, DomainObjectIDs.Order1);
    }

    [Test]
    public void UnloadData_ChangedVirtualEndPoint ()
    {
      var newTicket = OrderTicket.NewObject ();

      var domainObject = Order.GetObject (DomainObjectIDs.Order1);
      domainObject.OrderTicket = newTicket;

      UnloadService.UnloadData (TestableClientTransaction, domainObject.ID);

      Assert.That (domainObject.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (domainObject.OrderTicket, Is.SameAs (newTicket));
    }

    [Test]
    public void UnloadData_ChangedVirtualNullEndPoint ()
    {
      var domainObject = Employee.GetObject (DomainObjectIDs.Employee3);

      domainObject.Computer = null;
      
      UnloadService.UnloadData (TestableClientTransaction, domainObject.ID);

      Assert.That (domainObject.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (domainObject.Computer, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The relations of object 'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid' cannot be unloaded.\r\n"
        + "The opposite relation property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' of relation end-point "
        + "'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid/Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order' has "
        + "changed. Non-virtual end-points that are part of changed relations cannot be unloaded.")]
    public void UnloadData_ChangedCollection ()
    {
      OrderItem.GetObject (DomainObjectIDs.OrderItem1).Order.OrderItems.Add (OrderItem.NewObject ());
      Assert.That (TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.OrderItem1].State, Is.EqualTo (StateType.Unchanged));
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      Assert.That (TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (endPointID).HasChanged, Is.True);

      UnloadService.UnloadData (TestableClientTransaction, DomainObjectIDs.OrderItem1);
    }

    [Test]
    public void ReadingValueProperties_ReloadsObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      UnloadService.UnloadData (TestableClientTransaction, order1.ID);
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (TestableClientTransaction);

      Dev.Null = order1.OrderNumber;

      AssertObjectWasLoaded(listenerMock, order1);
      Assert.That (order1.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void WritingValueProperties_ReloadsObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      UnloadService.UnloadData (TestableClientTransaction, order1.ID);
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (TestableClientTransaction);

      order1.OrderNumber = 4711;

      AssertObjectWasLoaded (listenerMock, order1);
      Assert.That (order1.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void ReadingStateProperties_DoesNotReloadObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      UnloadService.UnloadData (TestableClientTransaction, order1.ID);
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      EnsureTransactionThrowsOnLoad ();

      Dev.Null = order1.ID;
      Dev.Null = order1.IsInvalid;
      Dev.Null = order1.State;
      try
      {
        Dev.Null = order1.GetBindingTransaction ();
        Assert.Fail ("Expected InvalidOperationException");
      }
      catch (InvalidOperationException)
      {
      }

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void ReadingTimestamp_ReloadsObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      UnloadService.UnloadData (TestableClientTransaction, order1.ID);
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (TestableClientTransaction);

      Dev.Null = order1.Timestamp;

      AssertObjectWasLoaded (listenerMock, order1);
      Assert.That (order1.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void RegisterForCommit_ReloadsObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      UnloadService.UnloadData (TestableClientTransaction, order1.ID);
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (TestableClientTransaction);

      order1.RegisterForCommit();

      AssertObjectWasLoaded (listenerMock, order1);
      Assert.That (order1.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void EnsureDataAvailable_ReloadsObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      UnloadService.UnloadData (TestableClientTransaction, order1.ID);
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (TestableClientTransaction);

      order1.EnsureDataAvailable ();

      AssertObjectWasLoaded (listenerMock, order1);
      Assert.That (order1.State, Is.EqualTo (StateType.Unchanged));
    }
    
    [Test]
    public void ReadingPropertyAccessor_DoesNotReloadObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      UnloadService.UnloadData (TestableClientTransaction, order1.ID);
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      EnsureTransactionThrowsOnLoad ();

      order1.PreparePropertyAccess (typeof (Order).FullName + ".OrderNumber");
      Dev.Null = order1.CurrentProperty;
      order1.PropertyAccessFinished ();

      Dev.Null = order1.Properties;

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void ReadingTransactionContext_DoesNotReloadObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      UnloadService.UnloadData (TestableClientTransaction, order1.ID);
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      EnsureTransactionThrowsOnLoad ();

      Dev.Null = order1.DefaultTransactionContext;
      Dev.Null = order1.TransactionContext;

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void ReadingCollectionEndPoint_DoesNotReloadObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var customer = order1.Customer;
      var customerOrders = customer.Orders;
      customerOrders.EnsureDataComplete();

      UnloadService.UnloadData (TestableClientTransaction, order1.ID);

      EnsureTransactionThrowsOnLoad ();

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (customerOrders.IsDataComplete, Is.False);

      Assert.That (customer.Orders, Is.SameAs (customerOrders)); // does not reload the object or the relation

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (customerOrders.IsDataComplete, Is.False);
    }

    [Test]
    public void ChangingCollectionEndPoint_ReloadsCollectionAndObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var customer = order1.Customer;
      var customerOrders = customer.Orders;

      UnloadService.UnloadData (TestableClientTransaction, order1.ID);

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (customerOrders.IsDataComplete, Is.False);

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (TestableClientTransaction);

      customer.Orders.Add (Order.NewObject ()); // reloads the relation contents and thus the object

      AssertObjectWasLoadedAmongOthers(listenerMock, order1);
      
      Assert.That (order1.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (customerOrders.IsDataComplete, Is.True);
    }

    [Test]
    [Ignore ("TODO 2264")]
    public void ReadingVirtualRelationEndPoints_DoesNotReloadObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var orderItems = order1.OrderItems;
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];
      var orderTicket = order1.OrderTicket;

      UnloadService.UnloadData (TestableClientTransaction, order1.ID);

      EnsureTransactionThrowsOnLoad ();

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      Assert.That (order1.OrderTicket, Is.SameAs (orderTicket)); // does not reload the object
      Assert.That (orderTicket.Order, Is.SameAs (order1)); // does not reload the object
      Assert.That (order1.OrderItems, Is.SameAs (orderItems)); // does not reload the object
      Assert.That (order1.OrderItems, Is.EquivalentTo (new[] { orderItemA, orderItemB })); // does not reload the object
      Assert.That (orderItemA.Order, Is.SameAs (order1)); // does not reload the object
      Assert.That (orderItemB.Order, Is.SameAs (order1)); // does not reload the object

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    [Ignore ("TODO 2264")]
    public void ReadingOriginalVirtualRelationEndPoints_DoesNotReloadObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];
      var orderTicket = order1.OrderTicket;

      UnloadService.UnloadData (TestableClientTransaction, order1.ID);

      EnsureTransactionThrowsOnLoad ();

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      Assert.That (order1.Properties.Find ("OrderTicket").GetOriginalValueWithoutTypeCheck(), Is.SameAs (orderTicket)); // does not reload the object
      Assert.That (orderTicket.Properties.Find ("Order").GetOriginalValueWithoutTypeCheck (), Is.SameAs (order1)); // does not reload the object
      Assert.That (order1.Properties.Find ("OrderItems").GetOriginalValueWithoutTypeCheck (), Is.EquivalentTo (new[] { orderItemA, orderItemB })); // does not reload the object
      Assert.That (orderItemA.Properties.Find ("Order").GetOriginalValueWithoutTypeCheck (), Is.SameAs (order1)); // does not reload the object
      Assert.That (orderItemB.Properties.Find ("Order").GetOriginalValueWithoutTypeCheck (), Is.SameAs (order1)); // does not reload the object

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    [Ignore ("TODO 2263")]
    public void ChangingVirtualRelationEndPoints_DoesNotReloadObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var orderItemA = order1.OrderItems[0];
      var orderItemB = order1.OrderItems[1];
      var orderTicket = order1.OrderTicket;

      UnloadService.UnloadData (TestableClientTransaction, order1.ID);

      EnsureTransactionThrowsOnLoad ();

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      order1.OrderTicket = OrderTicket.NewObject(); // does not reload the object
      Assert.That (orderTicket.Order, Is.Null);

      order1.OrderItems.Add (OrderItem.NewObject ()); // does not reload the object
      order1.OrderItems = new ObjectList<OrderItem> (new[] { orderItemA }); // does not reload the object
      Assert.That (orderItemA.Order, Is.SameAs (order1));
      Assert.That (orderItemB.Order, Is.Null);

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    [Ignore ("TODO 2263")]
    public void ChangingRealRelationEndPoints_DoesNotReloadOppositeObjects ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var orderItemA = order1.OrderItems[0];
      var orderTicket = order1.OrderTicket;

      UnloadService.UnloadData (TestableClientTransaction, order1.ID);

      EnsureTransactionThrowsOnLoad ();

      orderTicket.Order = Order.NewObject ();
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      orderItemA.Order = Order.NewObject ();
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void ReadingRealRelationEndPoints_ReloadsObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var customer = order1.Customer;

      UnloadService.UnloadData (TestableClientTransaction, order1.ID);
      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (TestableClientTransaction);
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      Assert.That (order1.Customer, Is.SameAs (customer)); // reloads the object because the foreign key is stored in order1

      AssertObjectWasLoaded (listenerMock, order1);

      Assert.That (order1.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void ChangingRealRelationEndPoints_ReloadsObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);

      UnloadService.UnloadData (TestableClientTransaction, order1.ID);
      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (TestableClientTransaction);
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      order1.Customer = Customer.NewObject (); // reloads the object because the foreign key is stored in order1

      AssertObjectWasLoaded (listenerMock, order1);
      Assert.That (order1.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void ReadingOppositeCollectionEndPoints_ReloadsObject ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var customer = order1.Customer;

      UnloadService.UnloadData (TestableClientTransaction, order1.ID);
      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (TestableClientTransaction);
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      Assert.That (customer.Orders, Has.Member (order1)); // enumerating reloads the relation contents because the foreign key is stored in order1

      AssertObjectWasLoadedAmongOthers (listenerMock, order1);
      Assert.That (order1.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    [Ignore ("TODO 2263")]
    public void AddingToCollectionEndPoint_DoesntReloadOtherItems ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var customer = order1.Customer;
      Console.WriteLine (customer.State);

      UnloadService.UnloadData (TestableClientTransaction, order1.ID);
      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));

      Console.WriteLine (customer.State);
      EnsureTransactionThrowsOnLoad ();
      
      customer.Orders.Add (Order.NewObject()); // does not reload order1 because that object's foreign key is not involved

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
    }
    
    [Test]
    public void AddingToCollectionEndPoint_ReloadsObjectBeingAdded ()
    {
      var customer = Customer.GetObject (DomainObjectIDs.Customer1);
      var order2 = Order.GetObject (DomainObjectIDs.Order2);

      UnloadService.UnloadData (TestableClientTransaction, order2.ID);
      Assert.That (order2.State, Is.EqualTo (StateType.NotLoadedYet));

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (TestableClientTransaction);

      customer.Orders.Add (order2); // reloads order2 because order2's foreign key is changed

      AssertObjectWasLoaded (listenerMock, order2);
      Assert.That (order2.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void Commit_DoesNotReloadObjectOrCollection ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var customerOrders = order1.Customer.Orders;

      UnloadService.UnloadData (TestableClientTransaction, order1.ID);

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (customerOrders.IsDataComplete, Is.False);

      EnsureTransactionThrowsOnLoad();

      TestableClientTransaction.Commit();

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (customerOrders.IsDataComplete, Is.False);
    }

    [Test]
    public void Rollback_DoesNotReloadObjectOrCollection ()
    {
      var order1 = Order.GetObject (DomainObjectIDs.Order1);
      var customerOrders = order1.Customer.Orders;

      UnloadService.UnloadData (TestableClientTransaction, order1.ID);

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (customerOrders.IsDataComplete, Is.False);

      EnsureTransactionThrowsOnLoad ();

      TestableClientTransaction.Rollback();

      Assert.That (order1.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (customerOrders.IsDataComplete, Is.False);
    }
  }
}