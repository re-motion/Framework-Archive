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
using System.Collections;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Deleting
{
  [TestFixture]
  public class DeleteNewDomainObjectTest : ClientTransactionBaseTest
  {
    private Order _newOrder;
    private DataContainer _newOrderContainer;
    private OrderTicket _newOrderTicket;

    private PropertyDefinition _orderNumberProperty;

    public override void SetUp ()
    {
      base.SetUp ();

      _newOrder = Order.NewObject ();
      _newOrderContainer = _newOrder.InternalDataContainer;
      _newOrderTicket = OrderTicket.NewObject (_newOrder);

      _orderNumberProperty = GetPropertyDefinition (typeof (Order), "OrderNumber");
    }

    [Test]
    public void RelatedObject ()
    {
      Assert.AreSame (_newOrder, _newOrderTicket.Order);
      Assert.AreSame (_newOrderTicket, _newOrder.OrderTicket);

      _newOrder.Delete ();

      Assert.IsNull (_newOrderTicket.Order);

      _newOrderTicket.Delete ();

      Assert.AreEqual (0, TestableClientTransaction.DataManager.DataContainers.Count);
      Assert.AreEqual (0, TestableClientTransaction.DataManager.RelationEndPoints.Count);
    }

    [Test]
    public void DomainObjectID ()
    {
      ObjectID oldID = _newOrder.ID;
      _newOrder.Delete ();
      ObjectID newID = _newOrder.ID;
      Assert.AreEqual (oldID, newID);
    }

    [Test]
    public void DomainObjectState ()
    {
      _newOrder.Delete ();
      StateType state = _newOrder.State;
      Assert.AreEqual (StateType.Invalid, state);
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void DomainObjectDataContainer ()
    {
      _newOrder.Delete ();
      DataContainer dataContainer = _newOrder.InternalDataContainer;
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void DomainObjectDelete ()
    {
      _newOrder.Delete ();
      _newOrder.Delete ();
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void DomainObjectGetRelatedObject ()
    {
      _newOrder.Delete ();
      _newOrder.GetRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void DomainObjectGetRelatedObjects ()
    {
      _newOrder.Delete ();
      _newOrder.GetRelatedObjects ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void DomainObjectGetOriginalRelatedObject ()
    {
      _newOrder.Delete ();
      _newOrder.GetRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void DomainObjectGetOriginalRelatedObjects ()
    {
      _newOrder.Delete ();
      _newOrder.GetOriginalRelatedObjects ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void DomainObjectSetRelatedObject ()
    {
      _newOrder.Delete ();
      _newOrder.SetRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", _newOrderTicket);
    }

    [Test]
    public void DomainObjectIsDiscarded ()
    {
      Assert.IsFalse (_newOrder.IsInvalid);

      _newOrder.Delete ();

      Assert.IsTrue (_newOrder.IsInvalid);
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void DomainObjectGetPropertyValue ()
    {
      _newOrder.Delete ();
      Dev.Null = _newOrder.Properties[typeof (Order), "OrderNumber"].GetValueWithoutTypeCheck();
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void DomainObjectSetPropertyValue ()
    {
      _newOrder.Delete ();
      _newOrder.Properties[typeof (Order), "OrderNumber"].SetValueWithoutTypeCheck (10);
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void DataContainerGetValue ()
    {
      _newOrder.Delete ();
      _newOrderContainer.GetValue (GetPropertyDefinition (typeof (Order), "OrderNumber"));
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void DataContainerSetValue ()
    {
      _newOrder.Delete ();
      _newOrderContainer.SetValue (GetPropertyDefinition (typeof (Order), "OrderNumber"), 10);
    }

    [Test]
    public void DataContainerDomainObject ()
    {
      _newOrder.Delete ();
      Assert.AreSame (_newOrder, _newOrderContainer.DomainObject);
    }

    [Test]
    public void DataContainerID ()
    {
      _newOrder.Delete ();
      Assert.AreSame (_newOrder.ID, _newOrderContainer.ID);
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void DataContainerClassDefinition ()
    {
      _newOrder.Delete ();
      ClassDefinition definition = _newOrderContainer.ClassDefinition;
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void DataContainerDomainObjectType ()
    {
      _newOrder.Delete ();
      Type type = _newOrderContainer.DomainObjectType;
    }

    [Test]
    public void DataContainerState ()
    {
      _newOrder.Delete ();
      StateType state = _newOrderContainer.State;
      Assert.AreEqual (StateType.Invalid, state);
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void DataContainerTimestamp ()
    {
      _newOrder.Delete ();
      object timestamp = _newOrderContainer.Timestamp;
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void DataContainerClientTransaction ()
    {
      _newOrder.Delete ();
      ClientTransaction clientTransaction = _newOrderContainer.ClientTransaction;
    }

    [Test]
    public void DataContainerIsDiscarded ()
    {
      DataContainer newDataContainer = _newOrder.InternalDataContainer;
      Assert.IsFalse (newDataContainer.IsDiscarded);

      _newOrder.Delete ();

      Assert.IsTrue (newDataContainer.IsDiscarded);
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void PropertyValueGetValue ()
    {
      _newOrder.Delete ();

      Dev.Null = _newOrderContainer.GetValue (_orderNumberProperty);
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void PropertyValueSetValue ()
    {
      _newOrder.Delete ();

      _newOrderContainer.SetValue (_orderNumberProperty, 10);
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void PropertyValueOriginalValue ()
    {
      _newOrder.Delete ();

      Dev.Null = _newOrderContainer.GetValue (_orderNumberProperty, ValueAccess.Original);
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void PropertyValueHasChanged ()
    {
      _newOrder.Delete ();

      bool hasChanged = _newOrderContainer.HasValueChanged (_orderNumberProperty);
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void PropertyValueHasBeenTouched ()
    {
      _newOrder.Delete ();

      bool hasBeenTouched = _newOrderContainer.HasValueBeenTouched (_orderNumberProperty);
    }

    [Test]
    public void Events ()
    {
      var orderItemsCollection = _newOrder.OrderItems;
      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (
          new DomainObject[] { _newOrder, _newOrderTicket },
          new DomainObjectCollection[] { orderItemsCollection });

      _newOrder.Delete ();

      ChangeState[] expectedStates = new ChangeState[]
    {
      new ObjectDeletionState (_newOrder, "1. Deleting event of order"),
      new CollectionDeletionState (orderItemsCollection, "2. Deleting of order.OrderItems"),
      new RelationChangeState (_newOrderTicket, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", _newOrder, null, "3. Relation changing event of orderTicket"),
      new RelationChangeState (_newOrderTicket, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", null, null, "4. Relation changed event of orderTicket"),
      new CollectionDeletionState (orderItemsCollection, "5. Deleted of order.OrderItems"),
      new ObjectDeletionState (_newOrder, "6. Deleted event of order")
    };

      eventReceiver.Check (expectedStates);
    }

    [Test]
    public void DeleteFromManyToOneRelation ()
    {
      Customer newCustomer = Customer.NewObject ();

      _newOrder.Customer = newCustomer;

      ObjectID newOrderID = _newOrder.ID;

      _newOrder.Delete ();

      Assert.IsFalse (newCustomer.Orders.Contains (newOrderID));
    }

    [Test]
    public void DeleteFromOneToManyRelation ()
    {
      Customer newCustomer = Customer.NewObject ();

      _newOrder.Customer = newCustomer;

      ObjectID newCustomerID = newCustomer.ID;

      newCustomer.Delete ();

      Assert.IsNull (_newOrder.Customer);
    }

    [Test]
    public void DeleteNewObjectsInDomainObjectsCommittingEvent ()
    {
      _newOrder.Committing += new EventHandler (NewOrder_Committing);
      _newOrderTicket.Committing += new EventHandler (NewOrderTicket_Committing);
      TestableClientTransaction.Committing += ClientTransactionMock_Committing_DeleteNewObjectsInDomainObjectsCommittingEvent;

      TestableClientTransaction.Commit ();
    }

    [Test]
    public void DeleteNewObjectsInClientTransactionsCommittingEvent ()
    {
      TestableClientTransaction.Committing += ClientTransactionMock_Committing_DeleteNewObjectsInClientTransactionsCommittingEvent;
      TestableClientTransaction.Commit ();
    }

    private void NewOrder_Committing (object sender, EventArgs e)
    {
      ((Order) sender).Delete ();
      _newOrderTicket.Delete ();
    }

    private void ClientTransactionMock_Committing_DeleteNewObjectsInClientTransactionsCommittingEvent (object sender, ClientTransactionEventArgs args)
    {
      _newOrder.Delete ();
      _newOrderTicket.Delete ();
    }

    private void ClientTransactionMock_Committing_DeleteNewObjectsInDomainObjectsCommittingEvent (object sender, ClientTransactionEventArgs args)
    {
      Assert.AreEqual (0, args.DomainObjects.Count);
    }

    private void NewOrderTicket_Committing (object sender, EventArgs e)
    {
      Assert.Fail ("NewOrderTicket_Committing event should not be raised.");
    }
  }
}
