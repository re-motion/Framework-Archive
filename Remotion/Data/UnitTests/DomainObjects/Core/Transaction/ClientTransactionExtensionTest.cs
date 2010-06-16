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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using Mocks_Is = Rhino.Mocks.Constraints.Is;
using Mocks_List = Rhino.Mocks.Constraints.List;
using Mocks_Property = Rhino.Mocks.Constraints.Property;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transaction
{
  [TestFixture]
  public class ClientTransactionExtensionTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private IClientTransactionExtension _extensionMock;
    private ClientTransactionMock _newTransaction;

    private Order _order1;
    private Computer _computerWithoutRelatedObjects;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();
      SetDatabaseModifyable();
    }

    public override void SetUp ()
    {
      base.SetUp();

      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _computerWithoutRelatedObjects = Computer.GetObject (DomainObjectIDs.Computer5);

      _newTransaction = new ClientTransactionMock();
      _newTransaction.EnlistDomainObjects (_order1);
      _order1.TransactionContext[_newTransaction].EnsureDataAvailable();
      _newTransaction.EnlistDomainObjects (_computerWithoutRelatedObjects);
      _computerWithoutRelatedObjects.TransactionContext[_newTransaction].EnsureDataAvailable ();

      _mockRepository = new MockRepository ();
      _extensionMock = _mockRepository.StrictMock<IClientTransactionExtension> ();

      ClientTransactionMock.Extensions.Add ("TestExtension", _extensionMock);
      _newTransaction.Extensions.Add ("TestExtension", _extensionMock);
    }

    [Test]
    public void Extensions ()
    {
      Assert.That (ClientTransactionMock.Extensions, List.Contains (_extensionMock));
    }

    [Test]
    public void NewObjectCreation ()
    {
      _extensionMock.Expect (mock => mock.NewObjectCreating (ClientTransactionMock, typeof (Order)));

      _mockRepository.ReplayAll();
      
      Order.NewObject();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ObjectLoading ()
    {
      using (_mockRepository.Ordered())
      {
        _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (ClientTransactionMock), 
            Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order2 })));
        _extensionMock.Expect (mock => mock.ObjectsLoaded (
            Arg.Is (ClientTransactionMock), 
            Arg<ReadOnlyCollection<DomainObject>>.Matches (loadedObjects => loadedObjects.Count == 1 && loadedObjects[0].ID == DomainObjectIDs.Order2)));
      }

      _mockRepository.ReplayAll();

      Dev.Null = Order.GetObject (DomainObjectIDs.Order2);
      Dev.Null = Order.GetObject (DomainObjectIDs.Order2);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ObjectLoadingWithRelatedObjects1Side ()
    {
      TestObjectLoadingWithRelatedObjects (
          delegate
          {
            Order order = Order.GetObject (DomainObjectIDs.Order2);
            int orderItemCount = order.OrderItems.Count;
            Assert.AreEqual (1, orderItemCount);
          },
          DomainObjectIDs.Order2,
          true,
          true,
          new[] { DomainObjectIDs.OrderItem3 });
    }

    [Test]
    public void ObjectLoadingWithRelatedObjectsNSide ()
    {
      TestObjectLoadingWithRelatedObjects (
          delegate
          {
            OrderItem orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem3);
            Order order = orderItem.Order;
            Assert.IsNotNull (order);
          },
          DomainObjectIDs.OrderItem3,
          false,
          true,
          new[] { DomainObjectIDs.Order2 });
    }

    [Test]
    public void ObjectLoadingWithRelatedObjects1To1RealSide ()
    {
      TestObjectLoadingWithRelatedObjects (
          delegate
          {
            Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
            Employee employee = computer.Employee;
            Assert.IsNotNull (employee);
          },
          DomainObjectIDs.Computer1,
          false,
          true,
          new[] { DomainObjectIDs.Employee3 });
    }

    [Test]
    public void ObjectLoadingWithRelatedObjects1To1VirtualSide ()
    {
      TestObjectLoadingWithRelatedObjects (
          delegate
          {
            Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
            Computer computer = employee.Computer;
            Assert.IsNotNull (computer);
          },
          DomainObjectIDs.Employee3,
          false,
          true,
          new[] { DomainObjectIDs.Computer1 });
    }

    [Test]
    public void EmptyObjectLoadingWithRelatedObjects1Side ()
    {
      TestObjectLoadingWithRelatedObjects (
          delegate
          {
            Official official = Official.GetObject (DomainObjectIDs.Official2);
            int count = official.Orders.Count;
            Assert.AreEqual (0, count);
          },
          DomainObjectIDs.Official2,
          true,
          false,
          new ObjectID[] { });
    }

    [Test]
    public void NullObjectLoadingWithRelatedObjectsNSide ()
    {
      TestObjectLoadingWithRelatedObjects (
          delegate
          {
            Client client = Client.GetObject (DomainObjectIDs.Client1);
            Client parent = client.ParentClient;
            Assert.IsNull (parent);
          },
          DomainObjectIDs.Client1,
          false,
          false,
          new ObjectID[] { });
    }

    [Test]
    public void NullObjectLoadingWithRelatedObjects1To1RealSide ()
    {
      TestObjectLoadingWithRelatedObjects (
          delegate
          {
            Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
            Employee employee = computer.Employee;
            Assert.IsNull (employee);
          },
          DomainObjectIDs.Computer4,
          false,
          false,
          new ObjectID[] { });
    }

    [Test]
    public void NullObjectLoadingWithRelatedObjects1To1VirtualSide ()
    {
      TestObjectLoadingWithRelatedObjects (
          delegate
          {
            Employee employee = Employee.GetObject (DomainObjectIDs.Employee7);
            Computer computer = employee.Computer;
            Assert.IsNull (computer);
          },
          DomainObjectIDs.Employee7,
          false,
          false,
          new ObjectID[] { });
    }

    [Test]
    public void ObjectsLoaded ()
    {
      _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_newTransaction),
            Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.ClassWithAllDataTypes1 })));
      _extensionMock.Expect (mock => mock.ObjectsLoaded (
            Arg.Is (_newTransaction),
            Arg<ReadOnlyCollection<DomainObject>>.Matches (collection => collection.Count == 1)));

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope())
      {
        ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ObjectsLoadedWithRelations ()
    {
      _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_newTransaction),
            Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order2 })));
      _extensionMock.Expect (mock => mock.ObjectsLoaded (
            Arg.Is (_newTransaction),
            Arg<ReadOnlyCollection<DomainObject>>.Matches (collection => collection.Count == 1)));

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope())
      {
        Order.GetObject (DomainObjectIDs.Order2);
      }
      _mockRepository.VerifyAll();
    }

    [Test]
    public void ObjectsLoadedWithEvents ()
    {
      var clientTransactionEventReceiver =
          _mockRepository.StrictMock<ClientTransactionMockEventReceiver> (_newTransaction);

      using (_mockRepository.Ordered())
      {
        _extensionMock.Expect (
            mock => mock.ObjectsLoading (
                        Arg.Is (_newTransaction),
                        Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.ClassWithAllDataTypes1 })));
        _extensionMock.Expect (
            mock => mock.ObjectsLoaded (
                        Arg.Is (_newTransaction),
                        Arg<ReadOnlyCollection<DomainObject>>.Matches (collection => collection.Count == 1)));

        clientTransactionEventReceiver.Loaded (null, null);
        LastCall.Constraints (
            Mocks_Is.Same (_newTransaction),
            Mocks_Property.ValueConstraint ("DomainObjects", Mocks_Property.Value ("Count", 1)));
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope())
      {
        ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ObjectDelete ()
    {
      var eventReceiverMock = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (_computerWithoutRelatedObjects);
      _extensionMock.BackToRecord();

      using (_mockRepository.Ordered())
      {
        _extensionMock.Expect (mock => mock.ObjectDeleting (ClientTransactionMock, _computerWithoutRelatedObjects));
        eventReceiverMock.Expect (mock => mock.Deleting (_computerWithoutRelatedObjects, EventArgs.Empty));
        eventReceiverMock.Expect (mock => mock.Deleted (_computerWithoutRelatedObjects, EventArgs.Empty));
        _extensionMock.Expect (mock => mock.ObjectDeleted (ClientTransactionMock, _computerWithoutRelatedObjects));
      }

      _mockRepository.ReplayAll();

      _computerWithoutRelatedObjects.Delete ();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ObjectDeleteWithOldRelatedObjects ()
    {
      OrderItem orderItem1;
      OrderItem orderItem2;
      OrderTicket orderTicket;
      Official official;
      Customer customer;
      OrderCollection customerOrders;
      DomainObjectCollection officialOrders;
      using (_newTransaction.EnterNonDiscardingScope())
      {
        orderItem1 = _order1.OrderItems[0];
        orderItem2 = _order1.OrderItems[1];
        orderTicket = _order1.OrderTicket;
        official = _order1.Official;
        customer = _order1.Customer;
        customerOrders = customer.Orders;
        officialOrders = official.Orders;
        Dev.Null = orderTicket.Order; // preload
      }

      var order1MockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (_order1);
      var orderItem1MockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (orderItem1);
      var orderItem2MockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (orderItem2);
      var orderTicketMockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (orderTicket);
      var officialMockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (official);
      var customerMockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (customer);

      var customerOrdersMockEventReceiver =
          _mockRepository.StrictMock<DomainObjectCollectionMockEventReceiver> (customerOrders);

      var officialOrdersMockEventReceiver =
          _mockRepository.StrictMock<DomainObjectCollectionMockEventReceiver> (officialOrders);

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.ObjectDeleting (_newTransaction, _order1);

        using (_mockRepository.Unordered())
        {
          _extensionMock.RelationChanging (_newTransaction, customer, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders", _order1, null);
          _extensionMock.RelationChanging (
              _newTransaction, orderTicket, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", _order1, null);
          _extensionMock.RelationChanging (_newTransaction, orderItem1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", _order1, null);
          _extensionMock.RelationChanging (_newTransaction, orderItem2, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", _order1, null);
          _extensionMock.RelationChanging (_newTransaction, official, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Official.Orders", _order1, null);
        }

        order1MockEventReceiver.Deleting (_order1, EventArgs.Empty);

        using (_mockRepository.Unordered())
        {
          customerMockEventReceiver.RelationChanging (customer, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders", _order1, null);
          customerOrdersMockEventReceiver.Removing (customerOrders, _order1);
          orderTicketMockEventReceiver.RelationChanging (
              orderTicket, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order", _order1, null);
          orderItem1MockEventReceiver.RelationChanging (orderItem1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", _order1, null);
          orderItem2MockEventReceiver.RelationChanging (orderItem2, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", _order1, null);
          officialMockEventReceiver.RelationChanging (official, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Official.Orders", _order1, null);
          officialOrdersMockEventReceiver.Removing (officialOrders, _order1);
          LastCall.IgnoreArguments().Constraints (Mocks_Is.Same (officialOrders), Mocks_Property.Value ("DomainObject", _order1));
        }

        using (_mockRepository.Unordered ())
        {
          customerMockEventReceiver.RelationChanged (customer, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders");
          customerOrdersMockEventReceiver.Removed (customerOrders, _order1);
          orderTicketMockEventReceiver.RelationChanged (orderTicket, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");
          orderItem1MockEventReceiver.RelationChanged (orderItem1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");
          orderItem2MockEventReceiver.RelationChanged (orderItem2, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");
          officialMockEventReceiver.RelationChanged (official, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Official.Orders");
          officialOrdersMockEventReceiver.Removed (officialOrders, _order1);
        }

        order1MockEventReceiver.Deleted (_order1, EventArgs.Empty);

        using (_mockRepository.Unordered())
        {
          _extensionMock.RelationChanged (_newTransaction, customer, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders");
          _extensionMock.RelationChanged (_newTransaction, orderTicket, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");
          _extensionMock.RelationChanged (_newTransaction, orderItem1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");
          _extensionMock.RelationChanged (_newTransaction, orderItem2, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");
          _extensionMock.RelationChanged (_newTransaction, official, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Official.Orders");
        }

        _extensionMock.ObjectDeleted (_newTransaction, _order1);
      }

      _mockRepository.ReplayAll();
      using (_newTransaction.EnterNonDiscardingScope())
      {
        _order1.Delete();
      }
      _mockRepository.VerifyAll();
    }

    [Test]
    public void RelationChangesWithUnidirectionalRelationshipWhenResettingDeletedLoaded ()
    {
      Location location;
      Client deletedClient;
      Client newClient;


      using (_newTransaction.EnterNonDiscardingScope())
      {
        location = Location.GetObject (DomainObjectIDs.Location1);
        deletedClient = location.Client;
        deletedClient.Delete();
        newClient = Client.NewObject();
      }

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationChanging (_newTransaction, location, typeof (Location) + ".Client", deletedClient, newClient);
        _extensionMock.RelationChanged (_newTransaction, location, typeof (Location) + ".Client");
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope())
      {
        location.Client = newClient;
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void RelationChangesWithUnidirectionalRelationshipWhenResettingNewLoaded ()
    {
      Location location;
      Client deletedClient;
      Client newClient;


      using (_newTransaction.EnterNonDiscardingScope())
      {
        location = Location.GetObject (DomainObjectIDs.Location1);
        location.Client = Client.NewObject();

        deletedClient = location.Client;
        location.Client.Delete();

        newClient = Client.NewObject();
      }

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationChanging (_newTransaction, location, typeof (Location) + ".Client", deletedClient, newClient);
        _extensionMock.RelationChanged (_newTransaction, location, typeof (Location) + ".Client");
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope())
      {
        location.Client = newClient;
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ObjectDeleteTwice ()
    {
      Computer computer;

      using (_newTransaction.EnterNonDiscardingScope())
      {
        computer = Computer.GetObject (DomainObjectIDs.Computer4);
      }
      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.ObjectDeleting (_newTransaction, computer);
        _extensionMock.ObjectDeleted (_newTransaction, computer);
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope())
      {
        computer.Delete();
        computer.Delete();
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void PropertyRead ()
    {
      int orderNumber = _order1.OrderNumber;
      _mockRepository.BackToRecord (_extensionMock);

      DataContainer order1DC = _order1.GetInternalDataContainerForTransaction (_newTransaction);


      using (_mockRepository.Ordered())
      {
        _extensionMock.PropertyValueReading (
            _newTransaction,
            order1DC,
            _order1.InternalDataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            ValueAccess.Current);
        _extensionMock.PropertyValueRead (
            _newTransaction,
            order1DC,
            _order1.InternalDataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            orderNumber,
            ValueAccess.Current);
        _extensionMock.PropertyValueReading (
            _newTransaction,
            order1DC,
            _order1.InternalDataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            ValueAccess.Original);
        _extensionMock.PropertyValueRead (
            _newTransaction,
            order1DC,
            _order1.InternalDataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            orderNumber,
            ValueAccess.Original);
      }

      _mockRepository.ReplayAll();
      using (_newTransaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.OrderNumber;
        Dev.Null =
            (int) _order1.InternalDataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].OriginalValue;
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void PropertyReadWithoutDataContainer ()
    {
      ClassDefinition orderClass = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order));
      PropertyDefinition orderNumberDefinition =
          orderClass.MyPropertyDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"];

      var propertyValue = new PropertyValue (orderNumberDefinition);
      var propertyValueCollection = new PropertyValueCollection();
      propertyValueCollection.Add (propertyValue);

      Dev.Null = (int) propertyValue.Value;

      // Expectation: no exception
    }

    [Test]
    public void ReadObjectIDProperty ()
    {
      PropertyValue customerPropertyValue =
          _order1.InternalDataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"];
      var customerID =
          (ObjectID) _order1.InternalDataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"].Value;

      DataContainer order1DC = _order1.GetInternalDataContainerForTransaction (_newTransaction);

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.PropertyValueReading (_newTransaction, order1DC, customerPropertyValue, ValueAccess.Current);
        _extensionMock.PropertyValueRead (_newTransaction, order1DC, customerPropertyValue, customerID, ValueAccess.Current);
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.InternalDataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"].Value;
      }
      _mockRepository.VerifyAll();
    }

    [Test]
    public void PropertySetToSameValue ()
    {
      int orderNumber = _order1.OrderNumber;

      _mockRepository.BackToRecord (_extensionMock);
      // Note: No method call on the extension is expected.
      _mockRepository.ReplayAll();

      _order1.OrderNumber = orderNumber;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ChangeAndReadProperty ()
    {
      int oldOrderNumber = _order1.OrderNumber;
      int newOrderNumber = oldOrderNumber + 1;

      DataContainer order1DC = _order1.GetInternalDataContainerForTransaction (_newTransaction);

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.PropertyValueChanging (
            _newTransaction,
            order1DC,
            order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            oldOrderNumber,
            newOrderNumber);
        _extensionMock.PropertyValueChanged (
            _newTransaction,
            order1DC,
            order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            oldOrderNumber,
            newOrderNumber);

        _extensionMock.PropertyValueReading (
            _newTransaction,
            order1DC,
            order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            ValueAccess.Current);
        _extensionMock.PropertyValueRead (
            _newTransaction,
            order1DC,
            order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            newOrderNumber,
            ValueAccess.Current);
        _extensionMock.PropertyValueReading (
            _newTransaction,
            order1DC,
            order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            ValueAccess.Original);
        _extensionMock.PropertyValueRead (
            _newTransaction,
            order1DC,
            order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            oldOrderNumber,
            ValueAccess.Original);
      }

      _mockRepository.ReplayAll();
      using (_newTransaction.EnterNonDiscardingScope())
      {
        _order1.OrderNumber = newOrderNumber;
        Dev.Null = _order1.OrderNumber;
        Dev.Null =
            (int) _order1.InternalDataContainer.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"].OriginalValue;
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void PropertyChange ()
    {
      int oldOrderNumber = _order1.OrderNumber;
      _mockRepository.BackToRecord (_extensionMock);

      DataContainer order1DC = _order1.GetInternalDataContainerForTransaction (_newTransaction);


      using (_mockRepository.Ordered())
      {
        _extensionMock.PropertyValueChanging (
            _newTransaction,
            order1DC,
            order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            oldOrderNumber,
            oldOrderNumber + 1);
        _extensionMock.PropertyValueChanged (
            _newTransaction,
            order1DC,
            order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            oldOrderNumber,
            oldOrderNumber + 1);
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope())
      {
        _order1.OrderNumber = oldOrderNumber + 1;
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void PropertyChangeWithEvents ()
    {
      int oldOrderNumber = _order1.OrderNumber;
      _mockRepository.BackToRecord (_extensionMock);

      DataContainer order1DC = _order1.GetInternalDataContainerForTransaction (_newTransaction);

      var domainObjectMockEventReceiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (_order1);
      var propertyValueCollectionMockEventReceiver =
          _mockRepository.StrictMock<PropertyValueCollectionMockEventReceiver> (order1DC.PropertyValues);


      using (_mockRepository.Ordered())
      {
        // "Changing" notifications

        _extensionMock.PropertyValueChanging (
            _newTransaction,
            order1DC,
            order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            oldOrderNumber,
            oldOrderNumber + 1);

        domainObjectMockEventReceiver.PropertyChanging (null, null);
        LastCall.IgnoreArguments();

        propertyValueCollectionMockEventReceiver.PropertyChanging (null, null);
        LastCall.IgnoreArguments();

        // "Changed" notifications

        propertyValueCollectionMockEventReceiver.PropertyChanged (null, null);
        LastCall.IgnoreArguments();

        domainObjectMockEventReceiver.PropertyChanged (null, null);
        LastCall.IgnoreArguments();

        _extensionMock.PropertyValueChanged (
            _newTransaction,
            order1DC,
            order1DC.PropertyValues["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"],
            oldOrderNumber,
            oldOrderNumber + 1);
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope())
      {
        _order1.OrderNumber = oldOrderNumber + 1;
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void LoadRelatedDataContainerForEndPoint ()
    {
      OrderTicket orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);

      _mockRepository.BackToRecord (_extensionMock);

      //Note: no reading notification must be performed

      _mockRepository.ReplayAll();

      using (var persistanceManager = new PersistenceManager(NullPersistenceListener.Instance))
      {
        ClassDefinition orderTicketDefinition = MappingConfiguration.Current.ClassDefinitions[typeof (OrderTicket)];
        IRelationEndPointDefinition orderEndPointDefinition =
            orderTicketDefinition.GetRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order");
        persistanceManager.LoadRelatedDataContainer (
            orderTicket.InternalDataContainer, new RelationEndPointID (orderTicket.ID, orderEndPointDefinition));
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void LoadRelatedDataContainerForVirtualEndPoint ()
    {
      //Note: no reading notification must be performed
      _mockRepository.ReplayAll();

      using (var persistenceManager = new PersistenceManager(NullPersistenceListener.Instance))
      {
        ClassDefinition orderDefinition = MappingConfiguration.Current.ClassDefinitions[typeof (Order)];
        IRelationEndPointDefinition orderTicketEndPointDefinition =
            orderDefinition.GetRelationEndPointDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
        persistenceManager.LoadRelatedDataContainer (
            _order1.InternalDataContainer, new RelationEndPointID (_order1.ID, orderTicketEndPointDefinition));
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetRelatedObject ()
    {
      OrderTicket orderTicket;

      using (_newTransaction.EnterNonDiscardingScope())
      {
        orderTicket = _order1.OrderTicket;
      }

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationReading (
            _newTransaction, _order1, _order1.Properties[typeof (Order), "OrderTicket"].PropertyData.RelationEndPointDefinition, ValueAccess.Current);
        _extensionMock.RelationRead (
            _newTransaction, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", orderTicket, ValueAccess.Current);
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.OrderTicket;
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetOriginalRelatedObject ()
    {
      OrderTicket originalOrderTicket;
      using (_newTransaction.EnterNonDiscardingScope())
      {
        originalOrderTicket =
            (OrderTicket) _order1.GetOriginalRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
      }
      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationReading (
            _newTransaction, _order1, _order1.Properties[typeof (Order), "OrderTicket"].PropertyData.RelationEndPointDefinition, ValueAccess.Original);
        _extensionMock.RelationRead (
            _newTransaction, _order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", originalOrderTicket, ValueAccess.Original);
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.GetOriginalRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetRelatedObjects ()
    {
      DomainObjectCollection orderItems;
      using (_newTransaction.EnterNonDiscardingScope())
      {
        orderItems = _order1.OrderItems;
      }
      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationReading (
            _newTransaction, _order1, _order1.Properties[typeof(Order),"OrderItems"].PropertyData.RelationEndPointDefinition, ValueAccess.Current);
        _extensionMock.RelationRead (null, null, null, (ReadOnlyDomainObjectCollectionAdapter<DomainObject>) null, ValueAccess.Current);
        LastCall.Constraints (
            Mocks_Is.Same (_newTransaction),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"),
            Mocks_Property.Value ("Count", 2) & Mocks_List.IsIn (orderItems[0]) & Mocks_List.IsIn (orderItems[1]),
            Mocks_Is.Equal (ValueAccess.Current));
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.OrderItems;
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetOriginalRelatedObjects ()
    {
      DomainObjectCollection originalOrderItems;
      using (_newTransaction.EnterNonDiscardingScope())
      {
        originalOrderItems = _order1.GetOriginalRelatedObjects ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");
      }
      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationReading (
            _newTransaction, _order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition, ValueAccess.Original);
        _extensionMock.RelationRead (null, null, null, (ReadOnlyDomainObjectCollectionAdapter<DomainObject>) null, ValueAccess.Original);

        LastCall.Constraints (
            Mocks_Is.Same (_newTransaction),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"),
            Mocks_Property.Value ("Count", 2) & Mocks_List.IsIn (originalOrderItems[0]) & Mocks_List.IsIn (originalOrderItems[1]),
            Mocks_Is.Equal (ValueAccess.Original));
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.GetOriginalRelatedObjects ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetRelatedObjectWithLazyLoad ()
    {
      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationReading (
            _newTransaction, _order1, _order1.Properties[typeof(Order),"OrderTicket"].PropertyData.RelationEndPointDefinition, ValueAccess.Current);

        _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_newTransaction), 
            Arg<ReadOnlyCollection<ObjectID>>.Matches (list => list.Count == 1)));
        
        _extensionMock.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 1));

        _extensionMock.RelationRead (null, null, null, (DomainObject) null, ValueAccess.Current);
        LastCall.Constraints (
            Mocks_Is.Same (_newTransaction),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"),
            Mocks_Is.NotNull(),
            Mocks_Is.Equal (ValueAccess.Current));
      }
      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.OrderTicket;
      }
      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetRelatedObjectsWithLazyLoad ()
    {
      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationReading (
            _newTransaction, _order1, _order1.Properties[typeof(Order),"OrderItems"].PropertyData.RelationEndPointDefinition, ValueAccess.Current);

        _extensionMock.ObjectsLoading (_newTransaction, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 2));

        _extensionMock.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 2));
        _extensionMock.RelationRead (null, null, null, (ReadOnlyDomainObjectCollectionAdapter<DomainObject>) null, ValueAccess.Current);
        LastCall.Constraints (
            Mocks_Is.Same (_newTransaction),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"),
            Mocks_Is.NotNull(),
            Mocks_Is.Equal (ValueAccess.Current));
      }
      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.OrderItems;
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetOriginalRelatedObjectWithLazyLoad ()
    {
      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationReading (
            _newTransaction, _order1, _order1.Properties[typeof (Order), "OrderTicket"].PropertyData.RelationEndPointDefinition, ValueAccess.Original);

        _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_newTransaction),
            Arg<ReadOnlyCollection<ObjectID>>.Matches (list => list.Count == 1)));

        _extensionMock.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 1));
        _extensionMock.RelationRead (null, null, null, (DomainObject) null, ValueAccess.Current);
        LastCall.Constraints (
            Mocks_Is.Same (_newTransaction),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket"),
            Mocks_Is.NotNull(),
            Mocks_Is.Equal (ValueAccess.Original));
      }
      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.GetOriginalRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetOriginalRelatedObjectsWithLazyLoad ()
    {
      using (_mockRepository.Ordered())
      {
        _extensionMock.RelationReading (
            _newTransaction, _order1, _order1.Properties[typeof (Order), "OrderItems"].PropertyData.RelationEndPointDefinition, ValueAccess.Original);

        _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_newTransaction),
            Arg<ReadOnlyCollection<ObjectID>>.Matches (list => list.Count == 2)));

        _extensionMock.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 2));
        _extensionMock.RelationRead (null, null, null, (ReadOnlyDomainObjectCollectionAdapter<DomainObject>) null, ValueAccess.Current);
        LastCall.Constraints (
            Mocks_Is.Same (_newTransaction),
            Mocks_Is.Same (_order1),
            Mocks_Is.Equal ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"),
            Mocks_Is.NotNull(),
            Mocks_Is.Equal (ValueAccess.Original));
      }
      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope())
      {
        Dev.Null = _order1.GetOriginalRelatedObjects ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems");
      }
      _mockRepository.VerifyAll();
    }

    [Test]
    public void FilterQueryResult ()
    {
      IQuery query = QueryFactory.CreateQueryFromConfiguration ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer1);

      using (_newTransaction.EnterNonDiscardingScope())
      {
        ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
      }

      _mockRepository.BackToRecord (_extensionMock);

      QueryResult<DomainObject> newQueryResult = TestQueryFactory.CreateTestQueryResult<DomainObject>();
      _extensionMock
          .Expect (
          mock => mock.FilterQueryResult (Arg.Is (_newTransaction), Arg<QueryResult<DomainObject>>.Matches (qr => qr.Count == 2 && qr.Query == query)))
          .Return (newQueryResult);

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope())
      {
        QueryResult<DomainObject> finalResult = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
        Assert.That (finalResult, Is.SameAs (newQueryResult));
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void FilterQueryResultWithLoad ()
    {
      IQuery query = QueryFactory.CreateQueryFromConfiguration ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer4);

      QueryResult<DomainObject> newQueryResult = TestQueryFactory.CreateTestQueryResult<DomainObject>();

      using (_mockRepository.Ordered())
      {
        _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_newTransaction),
            Arg<ReadOnlyCollection<ObjectID>>.Matches (list => list.Count == 2)));

        _extensionMock.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 2));
        _extensionMock
            .Expect (
            mock =>
            mock.FilterQueryResult (Arg.Is (_newTransaction), Arg<QueryResult<DomainObject>>.Matches (qr => qr.Count == 2 && qr.Query == query)))
            .Return (newQueryResult);
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope())
      {
        QueryResult<DomainObject> finalQueryResult = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
        Assert.That (finalQueryResult, Is.SameAs (newQueryResult));
      }
      _mockRepository.VerifyAll();
    }

    [Test]
    public void FilterQueryResultWithFiltering ()
    {
      IQuery query = QueryFactory.CreateQueryFromConfiguration ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer4);

      var filteringExtension = _mockRepository.StrictMock<ClientTransactionExtensionWithQueryFiltering>();
      _newTransaction.Extensions.Add ("FilteringExtension", filteringExtension);

      var lastExtension = _mockRepository.StrictMock<IClientTransactionExtension>();
      _newTransaction.Extensions.Add ("LastExtension", lastExtension);

      QueryResult<DomainObject> newQueryResult1 = TestQueryFactory.CreateTestQueryResult<DomainObject> (query, new[] { _order1 });
      QueryResult<DomainObject> newQueryResult2 = TestQueryFactory.CreateTestQueryResult<DomainObject> (query);

      using (_mockRepository.Ordered())
      {
        _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_newTransaction),
            Arg<ReadOnlyCollection<ObjectID>>.Matches (list => list.Count == 2)));
        filteringExtension.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_newTransaction),
            Arg<ReadOnlyCollection<ObjectID>>.Matches (list => list.Count == 2)));
        lastExtension.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_newTransaction),
            Arg<ReadOnlyCollection<ObjectID>>.Matches (list => list.Count == 2)));

        _extensionMock.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 2));
        filteringExtension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 2));
        lastExtension.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 2));

        _extensionMock
            .Expect (
            mock =>
            mock.FilterQueryResult (Arg.Is (_newTransaction), Arg<QueryResult<DomainObject>>.Matches (qr => qr.Count == 2 && qr.Query == query)))
            .Return (newQueryResult1);
        filteringExtension
            .Expect (mock => mock.FilterQueryResult (_newTransaction, newQueryResult1))
            .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
        lastExtension
            .Expect (
            mock =>
            mock.FilterQueryResult (Arg.Is (_newTransaction), Arg<QueryResult<DomainObject>>.Matches (qr => qr.Count == 0 && qr.Query == query)))
            .Return (newQueryResult2);
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope())
      {
        QueryResult<DomainObject> finalQueryResult = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
        Assert.That (finalQueryResult, Is.SameAs (newQueryResult2));
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void CommitWithChangedPropertyValue ()
    {
      Computer computer;
      using (_newTransaction.EnterNonDiscardingScope())
      {
        computer = Computer.GetObject (DomainObjectIDs.Computer4);
        computer.SerialNumber = "newSerialNumber";
      }

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.Committing (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 1) & Mocks_List.IsIn (computer));
        _extensionMock.Committed (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 1) & Mocks_List.IsIn (computer));
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope())
      {
        ClientTransactionScope.CurrentTransaction.Commit();
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void CommitWithChangedRelationValue ()
    {
      Computer computer;
      Employee employee;
      using (_newTransaction.EnterNonDiscardingScope())
      {
        computer = Computer.GetObject (DomainObjectIDs.Computer4);
        employee = Employee.GetObject (DomainObjectIDs.Employee1);
        computer.Employee = employee;
      }
      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.Committing (null, null);
        LastCall.Constraints (
            Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 2) & Mocks_List.IsIn (computer) & Mocks_List.IsIn (employee));
        _extensionMock.Committed (null, null);
        LastCall.Constraints (
            Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 2) & Mocks_List.IsIn (computer) & Mocks_List.IsIn (employee));
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope())
      {
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      _mockRepository.VerifyAll();
    }

    [Test]
    public void CommitWithChangedRelationValueWithClassIDColumn ()
    {
      Customer oldCustomer;
      Customer newCustomer;
      using (_newTransaction.EnterNonDiscardingScope())
      {
        oldCustomer = _order1.Customer;
        newCustomer = Customer.GetObject (DomainObjectIDs.Customer2);
        _order1.Customer = newCustomer;
      }
      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.Committing (null, null);
        LastCall.Constraints (
            Mocks_Is.Same (_newTransaction),
            Mocks_Property.Value ("Count", 3) & Mocks_List.IsIn (_order1) & Mocks_List.IsIn (newCustomer) & Mocks_List.IsIn (oldCustomer));
        _extensionMock.Committed (null, null);
        LastCall.Constraints (
            Mocks_Is.Same (_newTransaction),
            Mocks_Property.Value ("Count", 3) & Mocks_List.IsIn (_order1) & Mocks_List.IsIn (newCustomer) & Mocks_List.IsIn (oldCustomer));
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope())
      {
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      _mockRepository.VerifyAll();
    }

    [Test]
    public void CommitWithEvents ()
    {
      SetDatabaseModifyable();

      Computer computer;
      using (_newTransaction.EnterNonDiscardingScope())
      {
        computer = Computer.GetObject (DomainObjectIDs.Computer4);
        computer.SerialNumber = "newSerialNumber";
      }
      _mockRepository.BackToRecord (_extensionMock);

      var clientTransactionMockEventReceiver =
          _mockRepository.StrictMock<ClientTransactionMockEventReceiver> (_newTransaction);

      var computerEventReveiver = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (computer);

      using (_mockRepository.Ordered())
      {
        computerEventReveiver.Committing (computer, EventArgs.Empty);

        _extensionMock.Committing (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 1) & Mocks_List.IsIn (computer));

        clientTransactionMockEventReceiver.Committing (null, null);
        LastCall.Constraints (
            Mocks_Is.Same (_newTransaction),
            Mocks_Property.ValueConstraint ("DomainObjects", Mocks_Property.Value ("Count", 1)));

        computerEventReveiver.Committed (computer, EventArgs.Empty);

        clientTransactionMockEventReceiver.Committed (null, null);
        LastCall.Constraints (
            Mocks_Is.Same (_newTransaction),
            Mocks_Property.ValueConstraint ("DomainObjects", Mocks_Property.Value ("Count", 1)));

        _extensionMock.Committed (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("Count", 1) & Mocks_List.IsIn (computer));
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope())
      {
        ClientTransactionScope.CurrentTransaction.Commit();
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void StorageProviderGetFieldValue ()
    {
      int originalOrderNumber = _order1.OrderNumber;
      _order1.OrderNumber = originalOrderNumber + 1;

      _mockRepository.BackToRecord (_extensionMock);

      using (var storageProviderManager = new StorageProviderManager(NullPersistenceListener.Instance))
      {
        using (var storageProvider =
            (UnitTestStorageProviderStub) storageProviderManager.GetMandatory (c_unitTestStorageProviderStubID))
        {
          _mockRepository.ReplayAll();

          Assert.AreEqual (
              originalOrderNumber + 1,
              storageProvider.GetFieldValue (
                  _order1.InternalDataContainer, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber", ValueAccess.Current));
          Assert.AreEqual (
              originalOrderNumber,
              storageProvider.GetFieldValue (
                  _order1.InternalDataContainer, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber", ValueAccess.Original));

          _mockRepository.VerifyAll();
        }
      }
    }

    [Test]
    public void Rollback ()
    {
      Computer computer;
      using (_newTransaction.EnterNonDiscardingScope())
      {
        computer = Computer.GetObject (DomainObjectIDs.Computer4);
        computer.SerialNumber = "newSerialNumber";
      }

      _mockRepository.BackToRecord (_extensionMock);

      using (_mockRepository.Ordered())
      {
        _extensionMock.RollingBack (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_List.IsIn (computer));

        _extensionMock.RolledBack (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_List.IsIn (computer));
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope())
      {
        ClientTransactionScope.CurrentTransaction.Rollback();
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void SubTransactions ()
    {
      using (_mockRepository.Ordered())
      {
        _extensionMock.SubTransactionCreating (_newTransaction);
        _extensionMock.SubTransactionCreated (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_Property.Value ("ParentTransaction", _newTransaction));
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope())
      {
        ClientTransactionScope.CurrentTransaction.CreateSubTransaction();
      }
    }

    [Test]
    public void GetObjects ()
    {
      using (_mockRepository.Ordered())
      {
        _extensionMock.Expect (mock => mock.ObjectsLoading (
            Arg.Is (_newTransaction),
            Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order2, DomainObjectIDs.Order3 })));

        _extensionMock.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (_newTransaction), Mocks_List.Count (Mocks_Is.Equal (2)));
      }

      _mockRepository.ReplayAll();

      using (_newTransaction.EnterNonDiscardingScope())
      {
        _newTransaction.GetObjects<DomainObject> (DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3);
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void UnloadData ()
    {
      using (_mockRepository.Ordered ())
      {
        _extensionMock
            .Expect (mock => mock.ObjectsUnloading (
                        Arg.Is (ClientTransactionMock),
                        Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { _order1 })))
            .WhenCalled (mi => Assert.That (ClientTransactionMock.DataManager.DataContainerMap[_order1.ID] != null));
        _extensionMock
            .Expect (mock => mock.ObjectsUnloaded (
                        Arg.Is (ClientTransactionMock),
                        Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { _order1 })))
            .WhenCalled (mi => Assert.That (ClientTransactionMock.DataManager.DataContainerMap[_order1.ID] == null));
      }

      _mockRepository.ReplayAll ();

      UnloadService.UnloadData (ClientTransactionMock, _order1.ID, UnloadTransactionMode.ThisTransactionOnly);

      _mockRepository.VerifyAll ();
    }

    private void RecordObjectLoadingCalls (
    ClientTransaction transaction,
    ObjectID expectedMainObjectID,
    bool expectingCollection,
    bool expectLoadedEvent,
    IEnumerable<ObjectID> expectedRelatedObjectIDs)
    {
      using (_mockRepository.Ordered ())
      {
        // loading of main object
        _extensionMock.ObjectsLoading (Arg.Is (transaction), Arg<ReadOnlyCollection<ObjectID>>.List.Equal (new[] { expectedMainObjectID }));
        _extensionMock.ObjectsLoaded (null, null);
        LastCall.Constraints (Mocks_Is.Same (transaction), Mocks_Is.NotNull ());

        // accessing relation property

        _extensionMock.RelationReading (null, null, (IRelationEndPointDefinition) null, ValueAccess.Current);
        LastCall.IgnoreArguments ();

        if (expectedRelatedObjectIDs.Any())
          _extensionMock.ObjectsLoading (Arg.Is (transaction), Arg<ReadOnlyCollection<ObjectID>>.List.Equal (expectedRelatedObjectIDs));

        if (expectLoadedEvent)
        {
          _extensionMock.ObjectsLoaded (transaction, null);
          LastCall.IgnoreArguments ();
        }

        if (expectingCollection)
          _extensionMock.RelationRead (transaction, null, null, (ReadOnlyDomainObjectCollectionAdapter<DomainObject>) null, ValueAccess.Current);
        else
          _extensionMock.RelationRead (transaction, null, null, (DomainObject) null, ValueAccess.Current);

        LastCall.IgnoreArguments ();

        // loading of main object a second time

        // accessing relation property a second time

        _extensionMock.RelationReading (transaction, null, (IRelationEndPointDefinition) null, ValueAccess.Current);
        LastCall.IgnoreArguments ();

        if (expectingCollection)
          _extensionMock.RelationRead (transaction, null, null, (ReadOnlyDomainObjectCollectionAdapter<DomainObject>) null, ValueAccess.Current);
        else
          _extensionMock.RelationRead (transaction, null, null, (DomainObject) null, ValueAccess.Current);
        LastCall.IgnoreArguments ();
      }
    }

    private void TestObjectLoadingWithRelatedObjects (
        Action accessCode,
        ObjectID expectedMainObjectID,
        bool expectCollection,
        bool expectLoadedEvent,
        IEnumerable<ObjectID> expectedRelatedIDs)
    {
      _mockRepository.BackToRecordAll ();
      RecordObjectLoadingCalls (_newTransaction, expectedMainObjectID, expectCollection, expectLoadedEvent, expectedRelatedIDs);

      using (_newTransaction.EnterNonDiscardingScope ())
      {
        _mockRepository.ReplayAll ();

        accessCode ();
        accessCode ();

        _mockRepository.VerifyAll ();
      }
    }
  }
}
