// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction
{
  [TestFixture]
  public class SubTransactionTest : ClientTransactionBaseTest
  {
    [Test]
    public void CreateSubTransaction ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      Assert.That (subTransaction, Is.Not.Null);
      Assert.That (ClientTransactionTestHelper.GetPersistenceStrategy (subTransaction), Is.TypeOf (typeof (SubPersistenceStrategy)));
    }

    [Test]
    public void CreateSubTransaction_OfSubTransaction ()
    {
      ClientTransaction subTransaction1 = ClientTransactionMock.CreateSubTransaction();
      ClientTransaction subTransaction2 = subTransaction1.CreateSubTransaction();
      Assert.That (ClientTransactionTestHelper.GetPersistenceStrategy (subTransaction2), Is.TypeOf (typeof (SubPersistenceStrategy)));
    }

    [Test]
    public void CreateSubTransaction_SetsParentReadonly ()
    {
      Assert.That (ClientTransactionMock.IsReadOnly, Is.False);
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      Assert.That (ClientTransactionMock.IsReadOnly, Is.True);
      Assert.That (subTransaction.IsReadOnly, Is.False);

      ClientTransaction subTransaction2 = subTransaction.CreateSubTransaction();
      Assert.That (subTransaction.IsReadOnly, Is.True);
      Assert.That (subTransaction2.IsReadOnly, Is.False);
    }

    [Test]
    public void ParentTransaction ()
    {
      ClientTransaction subTransaction1 = ClientTransactionMock.CreateSubTransaction ();
      Assert.That (subTransaction1.ParentTransaction, Is.SameAs (ClientTransactionMock));

      ClientTransaction subTransaction2 = subTransaction1.CreateSubTransaction ();
      Assert.That (subTransaction2.ParentTransaction, Is.SameAs (subTransaction1));
    }

    [Test]
    public void ActiveSubTansaction ()
    {
      ClientTransaction subTransaction1 = ClientTransactionMock.CreateSubTransaction ();
      Assert.That (ClientTransactionMock.SubTransaction, Is.SameAs (subTransaction1));

      ClientTransaction subTransaction2 = subTransaction1.CreateSubTransaction ();
      Assert.That (subTransaction1.SubTransaction, Is.SameAs (subTransaction2));
      Assert.That (subTransaction2.SubTransaction, Is.Null);

      subTransaction2.Discard();

      Assert.That (subTransaction1.SubTransaction, Is.Null);
      Assert.That (ClientTransactionMock.SubTransaction, Is.SameAs (subTransaction1));

      subTransaction1.Discard();
      Assert.That (ClientTransactionMock.SubTransaction, Is.Null);
    }

    [Test]
    public void RootTransaction ()
    {
      ClientTransaction subTransaction1 = ClientTransactionMock.CreateSubTransaction ();
      ClientTransaction subTransaction2 = subTransaction1.CreateSubTransaction ();

      Assert.That (ClientTransactionMock.RootTransaction, Is.SameAs (ClientTransactionMock));
      Assert.That (subTransaction1.RootTransaction, Is.SameAs (ClientTransactionMock));
      Assert.That (subTransaction2.RootTransaction, Is.SameAs (ClientTransactionMock));
    }

    [Test]
    public void LeafTransaction ()
    {
      ClientTransaction subTransaction1 = ClientTransactionMock.CreateSubTransaction ();
      ClientTransaction subTransaction2 = subTransaction1.CreateSubTransaction ();

      Assert.That (ClientTransactionMock.LeafTransaction, Is.SameAs (subTransaction2));
      Assert.That (subTransaction1.LeafTransaction, Is.SameAs (subTransaction2));
      Assert.That (subTransaction2.LeafTransaction, Is.SameAs (subTransaction2));

      subTransaction2.Discard();

      Assert.That (ClientTransactionMock.LeafTransaction, Is.SameAs (subTransaction1));
      Assert.That (subTransaction1.LeafTransaction, Is.SameAs (subTransaction1));

      subTransaction1.Discard ();

      Assert.That (ClientTransactionMock.LeafTransaction, Is.SameAs (ClientTransactionMock));
    }

    [Test]
    public void CreateEmptyTransactionOfSameType_ForSubTransaction ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      subTransaction.Discard();
      ClientTransaction newSubTransaction = subTransaction.CreateEmptyTransactionOfSameType();
      Assert.That (subTransaction.ParentTransaction, Is.SameAs (ClientTransactionMock));
      Assert.That (subTransaction.RootTransaction, Is.SameAs (ClientTransactionMock));
      Assert.That (newSubTransaction, Is.Not.SameAs (subTransaction));
      Assert.That (newSubTransaction.GetType(), Is.EqualTo (subTransaction.GetType()));
    }

    [Test]
    public void CreateEmptyTransactionOfSameType_ForRootTransaction ()
    {
      var rootTransaction = ClientTransaction.CreateRootTransaction ();
      ClientTransaction newRootTransaction = rootTransaction.CreateEmptyTransactionOfSameType ();
      ClientTransaction subTransaction = rootTransaction.CreateSubTransaction ();
      Assert.That (subTransaction.ParentTransaction, Is.SameAs (rootTransaction));
      Assert.That (subTransaction.RootTransaction, Is.SameAs (rootTransaction));
      Assert.That (newRootTransaction, Is.Not.SameAs (rootTransaction));
      Assert.That (newRootTransaction.GetType(), Is.EqualTo (rootTransaction.GetType()));
      Assert.That (
          ClientTransactionTestHelper.GetPersistenceStrategy (newRootTransaction).GetType(),
          Is.EqualTo (
              ClientTransactionTestHelper.GetPersistenceStrategy (rootTransaction).GetType()));
    }

    [Test]
    public void EnterDiscardingScopeEnablesDiscardBehavior ()
    {
      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (ClientTransactionScope.ActiveScope.AutoRollbackBehavior, Is.EqualTo (AutoRollbackBehavior.Discard));
      }
    }

    [Test]
    public void SubTransactionHasSameExtensions ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      Assert.That (subTransaction.Extensions, Is.SameAs (ClientTransactionMock.Extensions));
    }

    [Test]
    public void SubTransactionHasSameApplicationData ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      Assert.That (subTransaction.ApplicationData, Is.SameAs (ClientTransactionMock.ApplicationData));
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = "The operation cannot be executed because the "
                                                                                       +
                                                                                       "ClientTransaction is read-only. Offending transaction modification: SubTransactionCreating."
        )]
    public void NoTwoSubTransactionsAtSameTime ()
    {
      ClientTransactionMock.CreateSubTransaction();
      ClientTransactionMock.CreateSubTransaction();
    }

    [Test]
    public void SubTransaction_CanBeUsedToCreateAndLoadNewObjects ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope())
      {
        Assert.That (ClientTransactionScope.CurrentTransaction, Is.SameAs (subTransaction));
        Order order = Order.NewObject();
        Assert.That (subTransaction.IsEnlisted (order), Is.True);
        Assert.That (ClientTransactionMock.IsEnlisted (order), Is.True);

        order.OrderNumber = 4711;
        Assert.That (order.OrderNumber, Is.EqualTo (4711));

        OrderItem item = OrderItem.NewObject();
        order.OrderItems.Add (item);
        Assert.That (order.OrderItems.Contains (item.ID), Is.True);

        Ceo ceo = Ceo.GetObject (DomainObjectIDs.Ceo1);
        Assert.That (ceo, Is.Not.Null);
        Assert.That (subTransaction.IsEnlisted (ceo), Is.True);
        Assert.That (ClientTransactionMock.IsEnlisted (ceo), Is.True);

        Assert.That (Company.GetObject (DomainObjectIDs.Company1), Is.SameAs (ceo.Company));
      }
    }

    [Test]
    public void EnlistedObjects_SharedWithParentTransaction ()
    {
      var subTx = ClientTransactionMock.CreateSubTransaction ();

      var order = DomainObjectMother.CreateObjectInOtherTransaction<Order> ();
      Assert.That (subTx.IsEnlisted (order), Is.False);
      Assert.That (ClientTransactionMock.IsEnlisted (order), Is.False);

      subTx.EnlistDomainObject (order);
      Assert.That (subTx.IsEnlisted (order), Is.True);
      Assert.That (ClientTransactionMock.IsEnlisted (order), Is.True);
    }

    [Test]
    public void DomainObjects_CreatedInParent_CanBeUsedInSubTransactions ()
    {
      Order order = Order.NewObject();
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      Assert.That (ClientTransactionMock.IsEnlisted (order), Is.True);
      Assert.That (subTransaction.IsEnlisted (order), Is.True);
    }

    [Test]
    public void DomainObjects_CreatedInParent_NotLoadedYetInSubTransaction ()
    {
      Order order = Order.NewObject ();
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction ();
      Assert.That (order.TransactionContext[subTransaction].State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void DomainObjects_CreatedInSubTransaction_CanBeUsedInParent ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope())
      {
        Order order = Order.NewObject();
        Assert.That (subTransaction.IsEnlisted (order), Is.True);
        Assert.That (ClientTransactionMock.IsEnlisted (order), Is.True);
      }
    }

    [Test]
    public void DomainObjects_CreatedInSubTransaction_InvalidInParent ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        Order order = Order.NewObject ();
        Assert.That (order.TransactionContext[subTransaction].State, Is.EqualTo (StateType.New));
        Assert.That (order.TransactionContext[ClientTransactionMock].State, Is.EqualTo (StateType.Invalid));
      }
    }

    [Test]
    public void DomainObjects_CreatedInSubTransaction_CommitMakesValidInParent ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        var instance = ClassWithAllDataTypes.NewObject ();
        Assert.That (instance.TransactionContext[subTransaction].State, Is.EqualTo (StateType.New));
        Assert.That (instance.TransactionContext[ClientTransactionMock].State, Is.EqualTo (StateType.Invalid));
        subTransaction.Commit ();
        Assert.That (instance.TransactionContext[ClientTransactionMock].State, Is.EqualTo (StateType.New));
      }
    }

    [Test]
    public void DomainObjects_LoadedInParent_CanBeUsedInSubTransactions ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      Assert.That (ClientTransactionMock.IsEnlisted (order), Is.True);
      Assert.That (subTransaction.IsEnlisted (order), Is.True);
    }

    [Test]
    public void DomainObjects_LoadedInParent_NotLoadedYetInSubTransaction ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction ();
      Assert.That (order.TransactionContext[subTransaction].State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void DomainObjects_LoadedInSubTransaction_CanBeUsedInParent ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope())
      {
        Order order = Order.GetObject (DomainObjectIDs.Order1);
        Assert.That (subTransaction.IsEnlisted (order), Is.True);
        Assert.That (ClientTransactionMock.IsEnlisted (order), Is.True);
      }
    }

    [Test]
    public void SubTransaction_CanAccessObject_CreatedInParent ()
    {
      Order order = Order.NewObject();
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope())
      {
        order.OrderNumber = 5;
        order.OrderTicket = OrderTicket.NewObject();
      }
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void Parent_CannotAccessObject_CreatedInSubTransaction ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      Order order;
      using (subTransaction.EnterDiscardingScope())
      {
        order = Order.NewObject();
      }
      Dev.Null = order.OrderNumber;
    }

    [Test]
    public void SubTransaction_CanAccessObject_LoadedInParent ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope())
      {
        ++order.OrderNumber;
        Dev.Null = order.OrderTicket;
        order.OrderTicket = OrderTicket.NewObject();
      }
    }

    [Test]
    public void Parent_CanAccessObject_LoadedInSubTransaction ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      Order order;
      using (subTransaction.EnterDiscardingScope())
      {
        order = Order.GetObject (DomainObjectIDs.Order1);
      }
      Assert.That (order.OrderNumber, Is.EqualTo (1));
    }

    [Test]
    public void Parent_CanReloadObject_LoadedInSubTransaction_AndGetTheSameReference ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      Order order;
      using (subTransaction.EnterDiscardingScope())
      {
        order = Order.GetObject (DomainObjectIDs.Order1);
      }
      Assert.That (Order.GetObject (DomainObjectIDs.Order1), Is.SameAs (order));
    }

    [Test]
    public void Parent_CanReloadRelatedObject_LoadedInSubTransaction_AndGetTheSameReference ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      Order order;
      OrderTicket orderTicket;
      using (subTransaction.EnterDiscardingScope())
      {
        order = Order.GetObject (DomainObjectIDs.Order1);
        orderTicket = order.OrderTicket;
      }
      Assert.That (Order.GetObject (DomainObjectIDs.Order1), Is.SameAs (order));
      Assert.That (OrderTicket.GetObject (DomainObjectIDs.OrderTicket1), Is.SameAs (orderTicket));
    }

    [Test]
    public void Parent_CanReloadNullRelatedObject_LoadedInSubTransaction ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      Computer computer;
      Employee employee;
      using (subTransaction.EnterDiscardingScope())
      {
        computer = Computer.GetObject (DomainObjectIDs.Computer4);
        Assert.That (computer.Employee, Is.Null);
        employee = Employee.GetObject (DomainObjectIDs.Employee1);
        Assert.That (employee.Computer, Is.Null);
      }
      Assert.That (Computer.GetObject (DomainObjectIDs.Computer4).Employee, Is.Null);
      Assert.That (computer.Employee, Is.Null);
      Assert.That (Employee.GetObject (DomainObjectIDs.Employee1).Computer, Is.Null);
      Assert.That (employee.Computer, Is.Null);
    }

    [Test]
    public void Parent_CanReloadRelatedObjectCollection_LoadedInSubTransaction_AndGetTheSameReferences ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      Order order;
      var orderItems = new Set<OrderItem>();
      using (subTransaction.EnterDiscardingScope())
      {
        order = Order.GetObject (DomainObjectIDs.Order1);
        orderItems.Add (order.OrderItems[0]);
        orderItems.Add (order.OrderItems[1]);
      }
      Assert.That (Order.GetObject (DomainObjectIDs.Order1), Is.SameAs (order));
      Assert.That (orderItems.Contains (OrderItem.GetObject (DomainObjectIDs.OrderItem1)), Is.True);
      Assert.That (orderItems.Contains (OrderItem.GetObject (DomainObjectIDs.OrderItem1)), Is.True);
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void IndirectAccess_ToDeletedObject_InSubTransactionThrows ()
    {
      Client client = Client.GetObject (DomainObjectIDs.Client1);
      Location location = Location.GetObject (DomainObjectIDs.Location1);
      Assert.That (location.Client, Is.SameAs (client));

      client.Delete();

      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        Dev.Null = location.Client;
      }
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void NewUnidirectionalDelete_InRootTransaction_CausesThrowOnAccess ()
    {
      Location location = Location.GetObject (DomainObjectIDs.Location1);
      location.Client = Client.NewObject();
      location.Client.Delete();

      Dev.Null = location.Client;
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void IndirectAccess_ToDeletedNewObject_InSubTransactionThrows ()
    {
      Location location = Location.GetObject (DomainObjectIDs.Location1);
      location.Client = Client.NewObject();
      location.Client.Delete();

      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        Dev.Null = location.Client;
      }
    }

    [Test]
    public void ResettingDeletedNewUnidirectionalInRootTransactionWorks ()
    {
      Location location = Location.NewObject();
      location.Client = Client.NewObject();
      location.Client.Delete();
      location.Client = Client.NewObject();
    }

    [Test]
    public void ResettingDeletedNewUnidirectionalInSubTransactionWorks ()
    {
      Location location = Location.NewObject();
      location.Client = Client.NewObject();
      location.Client.Delete();

      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        location.Client = Client.NewObject();
      }
    }

    [Test]
    public void ResettingDeletedNewUnidirectionalInSubTransactionWorks2 ()
    {
      Customer location = Customer.NewObject();
      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        location.Orders = new OrderCollection();
      }
    }

    [Test]
    public void ResettingDeletedNewUnidirectionalInSubTransactionWorks3 ()
    {
      Location location = Location.NewObject();
      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        location.Client = Client.NewObject();
      }
    }

    [Test]
    public void ResettingDeletedLoadedUnidirectionalInRootTransactionWorks ()
    {
      Location location = Location.NewObject();
      location.Client = Client.GetObject (DomainObjectIDs.Client1);
      location.Client.Delete();
      location.Client = Client.NewObject();
    }

    [Test]
    public void ResettingDeletedLoadedUnidirectionalInSubTransactionWorks ()
    {
      Location location = Location.NewObject();
      location.Client = Client.GetObject (DomainObjectIDs.Client1);
      location.Client.Delete();

      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        location.Client = Client.NewObject();
      }
    }

    [Test]
    public void StateChangesInsideSubTransaction ()
    {
      Order newOrder = Order.NewObject();

      Assert.That (newOrder.State, Is.EqualTo (StateType.New));

      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        newOrder.OrderNumber = 7;

        Assert.That (newOrder.State, Is.EqualTo (StateType.Changed));
        Assert.That (
            newOrder.Properties[typeof (Order) + ".OrderNumber"].GetOriginalValue<int>(),
            Is.Not.EqualTo (
                newOrder.Properties[typeof (Order) + ".OrderNumber"].GetValue<int>()));
      }
    }

    [Test]
    public void SubTransactionHasSamePropertyValuessAsParent_ForPersistentProperties ()
    {
      Order newUnchangedOrder = Order.NewObject();
      int newUnchangedOrderNumber = newUnchangedOrder.OrderNumber;

      Order newChangedOrder = Order.NewObject();
      newChangedOrder.OrderNumber = 4711;

      Order loadedUnchangedOrder = Order.GetObject (DomainObjectIDs.Order1);
      int loadedUnchangedOrderNumber = loadedUnchangedOrder.OrderNumber;

      Order loadedChangedOrder = Order.GetObject (DomainObjectIDs.Order2);
      loadedChangedOrder.OrderNumber = 13;

      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (Order.GetObject (DomainObjectIDs.Order1), Is.SameAs (loadedUnchangedOrder));
        Assert.That (Order.GetObject (DomainObjectIDs.Order2), Is.SameAs (loadedChangedOrder));

        Assert.That (newUnchangedOrder.OrderNumber, Is.EqualTo (newUnchangedOrderNumber));
        Assert.That (newChangedOrder.OrderNumber, Is.EqualTo (4711));
        Assert.That (loadedUnchangedOrder.OrderNumber, Is.EqualTo (loadedUnchangedOrderNumber));
        Assert.That (loadedChangedOrder.OrderNumber, Is.EqualTo (13));
      }
    }

    [Test]
    public void SubTransactionHasSamePropertyValuessAsParent_ForTransactionProperties ()
    {
      OrderTicket newUnchangedOrderTicket = OrderTicket.NewObject();
      int newUnchangedInt32TransactionProperty = newUnchangedOrderTicket.Int32TransactionProperty;

      OrderTicket newChangedOrderTicket = OrderTicket.NewObject();
      newChangedOrderTicket.Int32TransactionProperty = 4711;

      OrderTicket loadedUnchangedOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      int loadedUnchangedInt32TransactionProperty = loadedUnchangedOrderTicket.Int32TransactionProperty;

      OrderTicket loadedChangedOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);
      loadedChangedOrderTicket.Int32TransactionProperty = 13;

      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (OrderTicket.GetObject (DomainObjectIDs.OrderTicket1), Is.SameAs (loadedUnchangedOrderTicket));
        Assert.That (OrderTicket.GetObject (DomainObjectIDs.OrderTicket2), Is.SameAs (loadedChangedOrderTicket));

        Assert.That (newUnchangedOrderTicket.Int32TransactionProperty, Is.EqualTo (newUnchangedInt32TransactionProperty));
        Assert.That (newChangedOrderTicket.Int32TransactionProperty, Is.EqualTo (4711));
        Assert.That (loadedUnchangedOrderTicket.Int32TransactionProperty, Is.EqualTo (loadedUnchangedInt32TransactionProperty));
        Assert.That (loadedChangedOrderTicket.Int32TransactionProperty, Is.EqualTo (13));
      }
    }

    [Test]
    public void PropertyValueChangedAreNotPropagatedToParent ()
    {
      Order newChangedOrder = Order.NewObject();
      newChangedOrder.OrderNumber = 4711;

      Order loadedChangedOrder = Order.GetObject (DomainObjectIDs.Order2);
      loadedChangedOrder.OrderNumber = 13;

      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        newChangedOrder.OrderNumber = 17;
        loadedChangedOrder.OrderNumber = 4;

        using (ClientTransactionMock.EnterDiscardingScope())
        {
          Assert.That (newChangedOrder.OrderNumber, Is.EqualTo (4711));
          Assert.That (loadedChangedOrder.OrderNumber, Is.EqualTo (13));
        }
      }
    }

    [Test]
    public void SubTransactionHasRelatedObjectCollectionEqualToParent ()
    {
      Order loadedOrder = Order.GetObject (DomainObjectIDs.Order1);
      ObjectList<OrderItem> loadedItems = loadedOrder.OrderItems;

      Assert.That (loadedOrder.OrderItems, Is.SameAs (loadedItems));

      Dev.Null = loadedOrder.OrderItems[0];
      OrderItem loadedItem2 = loadedOrder.OrderItems[1];
      OrderItem newItem1 = OrderItem.NewObject();
      OrderItem newItem2 = OrderItem.NewObject();
      newItem2.Product = "Baz, buy two get three for free";

      loadedOrder.OrderItems.Clear();
      loadedOrder.OrderItems.Add (loadedItem2);
      loadedOrder.OrderItems.Add (newItem1);
      loadedOrder.OrderItems.Add (newItem2);

      Order newOrder = Order.NewObject();
      OrderItem newItem3 = OrderItem.NewObject();
      newItem3.Product = "FooBar, the energy bar with extra Foo";
      newOrder.OrderItems.Add (newItem3);

      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (Order.GetObject (DomainObjectIDs.Order1), Is.SameAs (loadedOrder));
        Assert.That (loadedOrder.OrderItems, Is.Not.SameAs (loadedItems));

        Assert.That (loadedOrder.OrderItems.Count, Is.EqualTo (3));

        Assert.That (loadedOrder.OrderItems[0], Is.SameAs (loadedItem2));
        Assert.That (loadedOrder.OrderItems[1], Is.SameAs (newItem1));
        Assert.That (loadedOrder.OrderItems[2], Is.SameAs (newItem2));

        Assert.That (loadedItem2.Order, Is.SameAs (loadedOrder));
        Assert.That (newItem1.Order, Is.SameAs (loadedOrder));
        Assert.That (newItem2.Order, Is.SameAs (loadedOrder));

        Assert.That (loadedOrder.OrderItems[2].Product, Is.EqualTo ("Baz, buy two get three for free"));

        Assert.That (newOrder.OrderItems.Count, Is.EqualTo (1));
        Assert.That (newOrder.OrderItems[0], Is.SameAs (newItem3));
        Assert.That (newOrder.OrderItems[0].Product, Is.EqualTo ("FooBar, the energy bar with extra Foo"));
        Assert.That (newItem3.Order, Is.SameAs (newOrder));
      }
    }

    [Test]
    public void SortExpressionNotExecuted_WhenLoadingCollectionFromParent ()
    {
      var customer1 = Customer.GetObject (DomainObjectIDs.Customer1);
      var orders = customer1.Orders.Reverse().ToArray ();
      customer1.Orders.Clear ();
      customer1.Orders.AddRange (orders);

      Assert.That (customer1.Orders, Is.EqualTo (orders));
      
      var sortExpression = ((VirtualRelationEndPointDefinition) customer1.Orders.AssociatedEndPointID.Definition).GetSortExpression ();
      Assert.That (sortExpression, Is.Not.Null);

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (customer1.Orders, Is.EqualTo (orders), "This would not be equal if the sort expression was executed.");
        Assert.That (customer1.Properties[typeof (Customer).FullName + ".Orders"].HasChanged, Is.False);
      }
    }

    [Test]
    public void SubTransactionCanGetRelatedObjectCollectionEvenWhenObjectsHaveBeenDiscarded ()
    {
      Order loadedOrder = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        OrderItem orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
        orderItem1.Delete();
        ClientTransactionScope.CurrentTransaction.Commit();
        Assert.That (orderItem1.IsInvalid, Is.True);

        ObjectList<OrderItem> orderItems = loadedOrder.OrderItems;
        Assert.That (orderItems.Count, Is.EqualTo (1));
        Assert.That (orderItems[0].ID, Is.EqualTo (DomainObjectIDs.OrderItem2));
      }
    }

    [Test]
    public void RelatedObjectCollectionChangesAreNotPropagatedToParent ()
    {
      Order loadedOrder = Order.GetObject (DomainObjectIDs.Order1);

      Assert.That (loadedOrder.OrderItems.Count, Is.EqualTo (2));
      OrderItem loadedItem1 = loadedOrder.OrderItems[0];
      OrderItem loadedItem2 = loadedOrder.OrderItems[1];

      Order newOrder = Order.NewObject();

      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        loadedOrder.OrderItems.Clear();
        newOrder.OrderItems.Add (OrderItem.NewObject());

        using (ClientTransactionMock.EnterDiscardingScope())
        {
          Assert.That (loadedOrder.OrderItems.Count, Is.EqualTo (2));
          Assert.That (loadedOrder.OrderItems[0], Is.SameAs (loadedItem1));
          Assert.That (loadedOrder.OrderItems[1], Is.SameAs (loadedItem2));
          Assert.That (newOrder.OrderItems.Count, Is.EqualTo (0));
        }
      }
    }

    [Test]
    public void SubTransactionHasSameRelatedObjectAsParent1To1 ()
    {
      Computer loadedComputer = Computer.GetObject (DomainObjectIDs.Computer1);
      Employee loadedEmployee = Employee.GetObject (DomainObjectIDs.Employee1);
      Assert.That (loadedEmployee, Is.Not.SameAs (loadedComputer.Employee));
      loadedComputer.Employee = loadedEmployee;

      Assert.That (loadedComputer.Employee, Is.SameAs (loadedEmployee));
      Assert.That (loadedEmployee.Computer, Is.SameAs (loadedComputer));

      Computer newComputer = Computer.NewObject();
      Employee newEmployee = Employee.NewObject();
      newEmployee.Computer = newComputer;

      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (Computer.GetObject (DomainObjectIDs.Computer1), Is.SameAs (loadedComputer));
        Assert.That (Employee.GetObject (DomainObjectIDs.Employee1), Is.SameAs (loadedEmployee));

        Assert.That (loadedComputer.Employee, Is.SameAs (loadedEmployee));
        Assert.That (loadedEmployee.Computer, Is.SameAs (loadedComputer));

        Assert.That (newEmployee.Computer, Is.SameAs (newComputer));
        Assert.That (newComputer.Employee, Is.SameAs (newEmployee));
      }
    }

    [Test]
    public void RelatedObjectChangesAreNotPropagatedToParent ()
    {
      Computer loadedComputer = Computer.GetObject (DomainObjectIDs.Computer1);
      Employee loadedEmployee = Employee.GetObject (DomainObjectIDs.Employee3);

      Computer newComputer = Computer.NewObject();
      Employee newEmployee = Employee.NewObject();
      newEmployee.Computer = newComputer;

      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        loadedComputer.Employee = Employee.NewObject();
        loadedEmployee.Computer = Computer.NewObject();

        newComputer.Employee = Employee.NewObject();
        newEmployee.Computer = Computer.NewObject();

        using (ClientTransactionMock.EnterDiscardingScope())
        {
          Assert.That (loadedEmployee.Computer, Is.SameAs (loadedComputer));
          Assert.That (loadedComputer.Employee, Is.SameAs (loadedEmployee));

          Assert.That (newEmployee.Computer, Is.SameAs (newComputer));
          Assert.That (newComputer.Employee, Is.SameAs (newEmployee));
        }
      }
    }

    [Test]
    public void SubTransactionCreatingEvent ()
    {
      ClientTransaction subTransactionFromEvent = null;

      ClientTransactionMock.SubTransactionCreated += delegate (object sender, SubTransactionCreatedEventArgs args)
      {
        Assert.That (sender, Is.SameAs (ClientTransactionMock));
        Assert.That (args.SubTransaction, Is.Not.Null);
        subTransactionFromEvent = args.SubTransaction;
      };

      Assert.That (subTransactionFromEvent, Is.Null);
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      Assert.That (subTransactionFromEvent, Is.Not.Null);
      Assert.That (subTransactionFromEvent, Is.SameAs (subTransaction));
    }

    [Test]
    public void GetObjects_UnloadedObjects_PropagatedToParent ()
    {
      ClientTransaction parent = ClientTransaction.CreateRootTransaction();
      ClientTransaction subTransaction = parent.CreateSubTransaction();

      LifetimeService.GetObject (subTransaction, DomainObjectIDs.ClassWithAllDataTypes1, false); // preload ClassWithAllDataTypes

      var extensionMock = MockRepository.GenerateMock<IClientTransactionExtension>();
      parent.Extensions.Add ("mock", extensionMock);

      subTransaction.GetObjects<DomainObject> (
          DomainObjectIDs.Order1,
          DomainObjectIDs.ClassWithAllDataTypes1,
          // this has already been loaded
          DomainObjectIDs.Order2,
          DomainObjectIDs.OrderItem1);

      extensionMock.AssertWasCalled (mock => mock.ObjectsLoading (Arg.Is (parent), 
          Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.OrderItem1 })));
      extensionMock.AssertWasNotCalled (mock => mock.ObjectsLoading (Arg.Is (parent), 
          Arg<ReadOnlyCollection<ObjectID>>.List.ContainsAll(new[] { DomainObjectIDs.ClassWithAllDataTypes1 })));
    }

    [Test]
    public void TryGetObjects_UnloadedObjects_PropagatedToParent ()
    {
      ClientTransaction parent = ClientTransaction.CreateRootTransaction();
      ClientTransaction subTransaction = parent.CreateSubTransaction();

      LifetimeService.GetObject (subTransaction, DomainObjectIDs.ClassWithAllDataTypes1, false); // preload ClassWithAllDataTypes

      var extensionMock = MockRepository.GenerateMock<IClientTransactionExtension>();
      parent.Extensions.Add ("mock", extensionMock);

      subTransaction.TryGetObjects<DomainObject> (
          DomainObjectIDs.Order1,
          DomainObjectIDs.ClassWithAllDataTypes1, // this has already been loaded
          DomainObjectIDs.Order2,
          DomainObjectIDs.OrderItem1);

      extensionMock.AssertWasCalled (mock => mock.ObjectsLoading (Arg.Is (parent), 
          Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.OrderItem1 })));
      extensionMock.AssertWasNotCalled (mock => mock.ObjectsLoading (Arg.Is (parent), 
          Arg<ReadOnlyCollection<ObjectID>>.List.ContainsAll(new[] { DomainObjectIDs.ClassWithAllDataTypes1 })));
    }

    [Test]
    public void GetObjects_UnloadedObjects_Events ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope())
      {
        var listenerMock = MockRepository.GenerateMock<IClientTransactionListener>();
        PrivateInvoke.InvokeNonPublicMethod (subTransaction, "AddListener", listenerMock);

        var eventReceiver = new ClientTransactionEventReceiver (subTransaction);
        DomainObject[] objects = subTransaction.GetObjects<DomainObject> (
            DomainObjectIDs.Order1,
            DomainObjectIDs.Order2,
            DomainObjectIDs.OrderItem1);

        Assert.That (eventReceiver.LoadedDomainObjects.Count, Is.EqualTo (1));
        Assert.That (eventReceiver.LoadedDomainObjects[0], Is.EqualTo (objects));

        listenerMock.AssertWasCalled (mock => mock.ObjectsLoading (
            Arg.Is (subTransaction), 
            Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.OrderItem1 })));

        listenerMock.AssertWasCalled (mock => mock.ObjectsLoaded (
            Arg.Is (subTransaction), 
            Arg<ReadOnlyCollection<DomainObject>>.List.Equal (objects)));
      }
    }

    [Test]
    public void GetObjects_LoadedObjects ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope())
      {
        var expectedObjects = new object[]
                              {
                                  Order.GetObject (DomainObjectIDs.Order1), Order.GetObject (DomainObjectIDs.Order2),
                                  OrderItem.GetObject (DomainObjectIDs.OrderItem1)
                              };
        DomainObject[] objects = subTransaction.GetObjects<DomainObject> (
            DomainObjectIDs.Order1,
            DomainObjectIDs.Order2,
            DomainObjectIDs.OrderItem1);
        Assert.That (objects, Is.EqualTo (expectedObjects));
      }
    }

    [Test]
    public void GetObjects_LoadedObjects_Events ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope())
      {
        var eventReceiver = new ClientTransactionEventReceiver (subTransaction);
        Order.GetObject (DomainObjectIDs.Order1);
        Order.GetObject (DomainObjectIDs.Order2);
        OrderItem.GetObject (DomainObjectIDs.OrderItem1);

        eventReceiver.Clear();

        var listenerMock = MockRepository.GenerateMock<IClientTransactionListener>();
        PrivateInvoke.InvokeNonPublicMethod (subTransaction, "AddListener", listenerMock);

        subTransaction.GetObjects<DomainObject> (DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.OrderItem1);
        Assert.That (eventReceiver.LoadedDomainObjects, Is.Empty);

        listenerMock.AssertWasNotCalled (mock => mock.ObjectsLoading (
            Arg<ClientTransaction>.Is.Anything, 
            Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
        listenerMock.AssertWasNotCalled (mock => mock.ObjectsLoaded (
            Arg<ClientTransaction>.Is.Anything, 
            Arg<ReadOnlyCollection<DomainObject>>.Is.Anything));
      }
    }

    [Test]
    public void GetObjects_NewObjects ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope())
      {
        var expectedObjects = new DomainObject[] { Order.NewObject(), OrderItem.NewObject() };
        DomainObject[] objects = subTransaction.GetObjects<DomainObject> (expectedObjects[0].ID, expectedObjects[1].ID);
        Assert.That (objects, Is.EqualTo (expectedObjects));
      }
    }

    [Test]
    public void GetObjects_NewObjects_Events ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope())
      {
        var eventReceiver = new ClientTransactionEventReceiver (subTransaction);
        var expectedObjects = new DomainObject[] { Order.NewObject(), OrderItem.NewObject() };
        eventReceiver.Clear();

        var listenerMock = MockRepository.GenerateMock<IClientTransactionListener>();
        PrivateInvoke.InvokeNonPublicMethod (subTransaction, "AddListener", listenerMock);

        subTransaction.GetObjects<DomainObject> (expectedObjects[0].ID, expectedObjects[1].ID);
        Assert.That (eventReceiver.LoadedDomainObjects, Is.Empty);

        listenerMock.AssertWasNotCalled (mock => mock.ObjectsLoading (
            Arg<ClientTransaction>.Is.Anything, 
            Arg<ReadOnlyCollection<ObjectID>>.Is.Anything));
        listenerMock.AssertWasNotCalled (mock => mock.ObjectsLoaded (
            Arg<ClientTransaction>.Is.Anything, 
            Arg<ReadOnlyCollection<DomainObject>>.Is.Anything));
      }
    }

    [Test]
    [ExpectedException (typeof (BulkLoadException), ExpectedMessage = "There were errors when loading a bulk of DomainObjects:\r\n"
                                                                      +
                                                                      "Object 'Order|33333333-3333-3333-3333-333333333333|System.Guid' could not be found.\r\n"
        )]
    public void GetObjects_NotFound ()
    {
      var guid = new Guid ("33333333333333333333333333333333");
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope())
      {
        subTransaction.GetObjects<DomainObject> (new ObjectID (typeof (Order), guid));
      }
    }

    [Test]
    public void TryGetObjects_NotFound ()
    {
      var guid = new Guid ("33333333333333333333333333333333");
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope())
      {
        Order newObject = Order.NewObject();
        Order[] objects = subTransaction.TryGetObjects<Order> (
            DomainObjectIDs.Order1,
            newObject.ID,
            new ObjectID (typeof (Order), guid),
            DomainObjectIDs.Order2);
        var expectedObjects = new DomainObject[]
                              {
                                  Order.GetObject (DomainObjectIDs.Order1),
                                  newObject,
                                  null,
                                  Order.GetObject (DomainObjectIDs.Order2)
                              };
        Assert.That (objects, Is.EqualTo (expectedObjects));
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidCastException))]
    public void GetObjects_InvalidType ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope())
      {
        subTransaction.GetObjects<OrderItem> (DomainObjectIDs.Order1);
      }
    }

    [Test]
    public void GetObjects_Deleted ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope())
      {
        var order = Order.GetObject (DomainObjectIDs.Order1);
        order.Delete ();

        var result = subTransaction.GetObjects<Order> (DomainObjectIDs.Order1);

        Assert.That (result[0], Is.SameAs (order));
      }
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException),
        ExpectedMessage = "Object 'ClassWithAllDataTypes|3f647d79-0caf-4a53-baa7-a56831f8ce2d|System.Guid' is invalid in this transaction.")]
    public void GetObjects_Discarded ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope())
      {
        ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1).Delete();
        subTransaction.Commit();
        subTransaction.GetObjects<ClassWithAllDataTypes> (DomainObjectIDs.ClassWithAllDataTypes1);
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "The ClientTransactionMock cannot be made writeable twice. A common reason for this error is that a subtransaction is accessed while its "
        + "parent transaction is engaged in a load operation. During such an operation, the subtransaction cannot be used.")]
    public void Throws_WhenUsedWhileParentIsWriteable ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope())
      {
        Type unlockerType = typeof (ClientTransaction).Assembly.GetType ("Remotion.Data.DomainObjects.Infrastructure.TransactionUnlocker");
        object unlocker =
            Activator.CreateInstance (
                unlockerType, BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { ClientTransactionMock }, null);
        using ((IDisposable) unlocker)
        {
          Order.GetObject (DomainObjectIDs.Order1);
        }
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The ClientTransactionMock cannot be made writeable twice. A common "
                                                                              +
                                                                              ("reason for this error is that a subtransaction is accessed while its parent transaction is engaged in a load operation. During such an "
                                                                               + "operation, the subtransaction cannot be used."))]
    public void Throws_WhenUsedWhileParentIsWriteable_IntegrationTest ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope())
      {
        ClientTransactionMock.Loaded += delegate { subTransaction.GetObjects<Order> (DomainObjectIDs.Order1); };
        Order.GetObject (DomainObjectIDs.Order2);
      }
    }

    [Test]
    public void LoadRelatedDataContainers_MakesParentWritableWhileGettingItsContainers ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);

      // cause parent tx to require reload of data containers...
      UnloadService.UnloadCollectionEndPointAndData (ClientTransactionMock, order.OrderItems.AssociatedEndPointID);

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var relatedObjects = order.OrderItems.ToArray ();
        Assert.That (relatedObjects,
            Is.EquivalentTo (new[] { OrderItem.GetObject (DomainObjectIDs.OrderItem1), OrderItem.GetObject (DomainObjectIDs.OrderItem2) }));
      }
    }
  }
}
