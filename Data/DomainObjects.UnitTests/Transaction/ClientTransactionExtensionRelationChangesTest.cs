using System;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.MockConstraints;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Mocks_Is = Rhino.Mocks.Constraints.Is;
using Mocks_List = Rhino.Mocks.Constraints.List;
using Mocks_Property = Rhino.Mocks.Constraints.Property;

namespace Rubicon.Data.DomainObjects.UnitTests.Transaction
{
  [TestFixture]
  public class ClientTransactionExtensionRelationChangesTest : ClientTransactionBaseTest
  {
    // types

    // static members and constants

    // member fields

    private Order _order1;
    private OrderTicket _orderTicket1;
    private Location _location1;
    private Client _client1;

    private DomainObjectMockEventReceiver _order1EventReceiver;
    private DomainObjectMockEventReceiver _orderTicket1EventReceiver;
    private DomainObjectMockEventReceiver _location1EventReceiver;
    private DomainObjectMockEventReceiver _client1EventReceiver;

    private MockRepository _mockRepository;
    private IClientTransactionExtension _extension;

    // construction and disposing

    public ClientTransactionExtensionRelationChangesTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _orderTicket1 = _order1.OrderTicket;
      _location1 = Location.GetObject (DomainObjectIDs.Location1);
      _client1 = _location1.Client;

      _mockRepository = new MockRepository ();

      _extension = _mockRepository.CreateMock<IClientTransactionExtension> ();
      _order1EventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (_order1);
      _orderTicket1EventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (_orderTicket1);
      _location1EventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (_location1);
      _client1EventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (_client1);

      ClientTransaction.Current.Extensions.Add ("Name", _extension);
    }

    [Test]
    public void OneToOneRelationFromVirtualEndPointWithSameObject ()
    {
      // no calls on the extension are expected

      _mockRepository.ReplayAll ();

      _order1.OrderTicket = _orderTicket1;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void OneToOneRelationFromVirtualEndPointWithNewNull ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension.RelationChanging (_order1, "OrderTicket", _orderTicket1, null);
        _extension.RelationChanging (_orderTicket1, "Order", _order1, null);

        _order1EventReceiver.RelationChanging (_order1, "OrderTicket", _orderTicket1, null);

        _orderTicket1EventReceiver.RelationChanging (_orderTicket1, "Order", _order1, null);

        _extension.RelationChanged (_order1, "OrderTicket");
        _extension.RelationChanged (_orderTicket1, "Order");

        _order1EventReceiver.RelationChanged (_order1, "OrderTicket");

        _orderTicket1EventReceiver.RelationChanged (_orderTicket1, "Order");
      }

      _mockRepository.ReplayAll ();

      _order1.OrderTicket = null;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void OneToOneRelationFromVirtualEndPointWithOldNull ()
    {
      Order order = new Order ();
      OrderTicket orderTicket = new OrderTicket ();

      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationChanging (order, "OrderTicket", null, orderTicket);
        _extension.RelationChanging (orderTicket, "Order", null, order);

        DomainObjectMockEventReceiver orderEventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (order);
        DomainObjectMockEventReceiver orderTicketEventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (orderTicket);

        orderEventReceiver.RelationChanging (order, "OrderTicket", null, orderTicket);

        orderTicketEventReceiver.RelationChanging (orderTicket, "Order", null, order);

        _extension.RelationChanged (order, "OrderTicket");
        _extension.RelationChanged (orderTicket, "Order");

        orderEventReceiver.RelationChanged (order, "OrderTicket");

        orderTicketEventReceiver.RelationChanged (orderTicket, "Order");
      }

      _mockRepository.ReplayAll ();

      order.OrderTicket = orderTicket;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void OneToOneRelationFromVirtualEndPointWithBothOldRelatedObjects ()
    {
      OrderTicket orderTicket3 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket3);
      Order oldOrderOfOrderTicket3 = orderTicket3.Order;

      DomainObjectMockEventReceiver orderTicket3EventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (orderTicket3);
      DomainObjectMockEventReceiver oldOrderOfOrderTicket3EventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (oldOrderOfOrderTicket3);
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationChanging (orderTicket3, "Order", oldOrderOfOrderTicket3, _order1);
        _extension.RelationChanging (oldOrderOfOrderTicket3, "OrderTicket", orderTicket3, null);
        _extension.RelationChanging (_order1, "OrderTicket", _orderTicket1, orderTicket3);
        _extension.RelationChanging (_orderTicket1, "Order", _order1, null);

        orderTicket3EventReceiver.RelationChanging (orderTicket3, "Order", oldOrderOfOrderTicket3, _order1);

        oldOrderOfOrderTicket3EventReceiver.RelationChanging (oldOrderOfOrderTicket3, "OrderTicket", orderTicket3, null);

        _order1EventReceiver.RelationChanging (_order1, "OrderTicket", _orderTicket1, orderTicket3);

        _orderTicket1EventReceiver.RelationChanging (_orderTicket1, "Order", _order1, null);

        _extension.RelationChanged (orderTicket3, "Order");
        _extension.RelationChanged (oldOrderOfOrderTicket3, "OrderTicket");
        _extension.RelationChanged (_order1, "OrderTicket");
        _extension.RelationChanged (_orderTicket1, "Order");

        orderTicket3EventReceiver.RelationChanged (orderTicket3, "Order");

        oldOrderOfOrderTicket3EventReceiver.RelationChanged (oldOrderOfOrderTicket3, "OrderTicket");

        _order1EventReceiver.RelationChanged (_order1, "OrderTicket");

        _orderTicket1EventReceiver.RelationChanged (_orderTicket1, "Order");
      }

      _mockRepository.ReplayAll ();

      orderTicket3.Order = _order1;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void OneToOneRelationFromEndPointWithSameObject ()
    {
      // no calls on the extension are expected

      _mockRepository.ReplayAll ();

      _orderTicket1.Order = _order1;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void OneToOneRelationFromEndPointWithBothOldRelatedObjects ()
    {
      OrderTicket orderTicket3 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket3);
      Order oldOrderOfOrderTicket3 = orderTicket3.Order;

      DomainObjectMockEventReceiver orderTicket3EventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (orderTicket3);
      DomainObjectMockEventReceiver oldOrderOfOrderTicket3EventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (oldOrderOfOrderTicket3);
      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationChanging (_order1, "OrderTicket", _orderTicket1, orderTicket3);
        _extension.RelationChanging (_orderTicket1, "Order", _order1, null);
        _extension.RelationChanging (orderTicket3, "Order", oldOrderOfOrderTicket3, _order1);
        _extension.RelationChanging (oldOrderOfOrderTicket3, "OrderTicket", orderTicket3, null);

        _order1EventReceiver.RelationChanging (_order1, "OrderTicket", _orderTicket1, orderTicket3);

        _orderTicket1EventReceiver.RelationChanging (_orderTicket1, "Order", _order1, null);

        orderTicket3EventReceiver.RelationChanging (orderTicket3, "Order", oldOrderOfOrderTicket3, _order1);

        oldOrderOfOrderTicket3EventReceiver.RelationChanging (oldOrderOfOrderTicket3, "OrderTicket", orderTicket3, null);

        _extension.RelationChanged (_order1, "OrderTicket");
        _extension.RelationChanged (_orderTicket1, "Order");
        _extension.RelationChanged (orderTicket3, "Order");
        _extension.RelationChanged (oldOrderOfOrderTicket3, "OrderTicket");

        _order1EventReceiver.RelationChanged (_order1, "OrderTicket");

        _orderTicket1EventReceiver.RelationChanged (_orderTicket1, "Order");

        orderTicket3EventReceiver.RelationChanged (orderTicket3, "Order");

        oldOrderOfOrderTicket3EventReceiver.RelationChanged (oldOrderOfOrderTicket3, "OrderTicket");
      }

      _mockRepository.ReplayAll ();

      _order1.OrderTicket = orderTicket3;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void UnidirectionalRelationWithSameObject ()
    {
      // no calls on the extension are expected

      _mockRepository.ReplayAll ();

      _location1.Client = _client1;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void UnidirectionalRelationWithNewNull ()
    {
      using (_mockRepository.Ordered ())
      {
        _extension.RelationChanging (_location1, "Client", _client1, null);

        _location1EventReceiver.RelationChanging (_location1, "Client", _client1, null);

        _extension.RelationChanged (_location1, "Client");

        _location1EventReceiver.RelationChanged (_location1, "Client");
      }

      _mockRepository.ReplayAll ();

      _location1.Client = null;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void UnidirectionalRelationWithOldNull ()
    {
      Location newLocation = new Location ();

      DomainObjectMockEventReceiver newLocationEventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (newLocation);

      _mockRepository.BackToRecord (_extension);
      using (_mockRepository.Ordered ())
      {
        _extension.RelationChanging (newLocation, "Client", null, _client1);

        newLocationEventReceiver.RelationChanging (newLocation, "Client", null, _client1);

        _extension.RelationChanged (newLocation, "Client");

        newLocationEventReceiver.RelationChanged (newLocation, "Client");
      }

      _mockRepository.ReplayAll ();

      newLocation.Client = _client1;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void UnidirectionalRelationWithOldRelatedObject ()
    {
      Client newClient = new Client ();
      DomainObjectMockEventReceiver newClientEventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (newClient);

      _mockRepository.BackToRecord (_extension);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationChanging (_location1, "Client", _client1, newClient);

        _location1EventReceiver.RelationChanging (_location1, "Client", _client1, newClient);

        _extension.RelationChanged (_location1, "Client");

        _location1EventReceiver.RelationChanged (_location1, "Client");
      }

      _mockRepository.ReplayAll ();

      _location1.Client = newClient;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void RemoveFromOneToManyRelation ()
    {
      DomainObjectCollection preloadedOrderItems = _order1.OrderItems;

      Assert.Greater (preloadedOrderItems.Count, 0);
      OrderItem orderItem = (OrderItem) preloadedOrderItems[0];

      _mockRepository.BackToRecord (_extension);
      DomainObjectMockEventReceiver orderItemEventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (orderItem);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationReading (_order1, "OrderItems", ValueAccess.Current);
        _extension.RelationRead (null, null, (DomainObjectCollection) null, ValueAccess.Current);
        LastCall.Constraints (Mocks_Is.Same (_order1), Mocks_Is.Equal ("OrderItems"), Mocks_Property.Value ("Count", preloadedOrderItems.Count) & new ContainsConstraint (preloadedOrderItems), Mocks_Is.Equal (ValueAccess.Current));

        _extension.RelationChanging (orderItem, "Order", _order1, null);
        _extension.RelationChanging (_order1, "OrderItems", orderItem, null);

        orderItemEventReceiver.RelationChanging (orderItem, "Order", _order1, null);

        _order1EventReceiver.RelationChanging (_order1, "OrderItems", orderItem, null);

        _extension.RelationChanged (orderItem, "Order");
        _extension.RelationChanged (_order1, "OrderItems");

        orderItemEventReceiver.RelationChanged (orderItem, "Order");

        _order1EventReceiver.RelationChanged (_order1, "OrderItems");
      }

      _mockRepository.ReplayAll ();

      _order1.OrderItems.Remove (orderItem);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void AddToOneToManyRelation ()
    {
      DomainObjectCollection preloadedOrderItems = _order1.OrderItems;
      OrderItem orderItem = new OrderItem ();

      _mockRepository.BackToRecord (_extension);
      DomainObjectMockEventReceiver orderItemEventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (orderItem);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationReading (_order1, "OrderItems", ValueAccess.Current);
        _extension.RelationRead (null, null, (DomainObjectCollection) null, ValueAccess.Current);
        LastCall.Constraints (Mocks_Is.Same (_order1), Mocks_Is.Equal ("OrderItems"), Mocks_Property.Value ("Count", preloadedOrderItems.Count) & new ContainsConstraint (preloadedOrderItems), Mocks_Is.Equal (ValueAccess.Current));

        _extension.RelationChanging (orderItem, "Order", null, _order1);
        _extension.RelationChanging (_order1, "OrderItems", null, orderItem);

        orderItemEventReceiver.RelationChanging (orderItem, "Order", null, _order1);

        _order1EventReceiver.RelationChanging (_order1, "OrderItems", null, orderItem);

        _extension.RelationChanged (orderItem, "Order");
        _extension.RelationChanged (_order1, "OrderItems");

        orderItemEventReceiver.RelationChanged (orderItem, "Order");

        _order1EventReceiver.RelationChanged (_order1, "OrderItems");
      }

      _mockRepository.ReplayAll ();

      _order1.OrderItems.Add (orderItem);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void AddToOneToManyRelationWithOldRelatedObject ()
    {
      DomainObjectCollection preloadedOrderItemsOfOrder1 = _order1.OrderItems;
      OrderItem newOrderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem3);
      Order oldOrderOfNewOrderItem = newOrderItem.Order;
      DomainObjectCollection preloadedOrderItems2 = oldOrderOfNewOrderItem.OrderItems;

      _mockRepository.BackToRecord (_extension);
      DomainObjectMockEventReceiver newOrderItemEventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (newOrderItem);
      DomainObjectMockEventReceiver oldOrderOfNewOrderItemEventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (oldOrderOfNewOrderItem);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationReading (_order1, "OrderItems", ValueAccess.Current);
        _extension.RelationRead (null, null, (DomainObjectCollection) null, ValueAccess.Current);
        LastCall.Constraints (Mocks_Is.Same (_order1), Mocks_Is.Equal ("OrderItems"), Mocks_Property.Value ("Count", preloadedOrderItemsOfOrder1.Count) & new ContainsConstraint (preloadedOrderItemsOfOrder1), Mocks_Is.Equal (ValueAccess.Current));

        _extension.RelationChanging (newOrderItem, "Order", oldOrderOfNewOrderItem, _order1);
        _extension.RelationChanging (_order1, "OrderItems", null, newOrderItem);
        _extension.RelationChanging (oldOrderOfNewOrderItem, "OrderItems", newOrderItem, null);

        newOrderItemEventReceiver.RelationChanging (newOrderItem, "Order", oldOrderOfNewOrderItem, _order1);

        _order1EventReceiver.RelationChanging (_order1, "OrderItems", null, newOrderItem);

        oldOrderOfNewOrderItemEventReceiver.RelationChanging (oldOrderOfNewOrderItem, "OrderItems", newOrderItem, null);

        _extension.RelationChanged (newOrderItem, "Order");
        _extension.RelationChanged (_order1, "OrderItems");
        _extension.RelationChanged (oldOrderOfNewOrderItem, "OrderItems");

        newOrderItemEventReceiver.RelationChanged (newOrderItem, "Order");

        _order1EventReceiver.RelationChanged (_order1, "OrderItems");

        oldOrderOfNewOrderItemEventReceiver.RelationChanged (oldOrderOfNewOrderItem, "OrderItems");
      }

      _mockRepository.ReplayAll ();

      _order1.OrderItems.Add (newOrderItem);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ReplaceInOneToManyRelationWithSameObject ()
    {
      DomainObjectCollection orderItems = _order1.OrderItems;
      OrderItem oldOrderItem = (OrderItem) _order1.OrderItems[0];

      _mockRepository.BackToRecord (_extension);

      // no calls on the extension are expected

      _mockRepository.ReplayAll ();

      orderItems[0] = oldOrderItem;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ReplaceInOneToManyRelation ()
    {
      Assert.Greater (_order1.OrderItems.Count, 0);
      OrderItem oldOrderItem = (OrderItem) _order1.OrderItems[0];

      DomainObjectCollection preloadedOrderItems = _order1.OrderItems;
      OrderItem newOrderItem = new OrderItem ();

      _mockRepository.BackToRecord (_extension);
      DomainObjectMockEventReceiver oldOrderItemEventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (oldOrderItem);
      DomainObjectMockEventReceiver newOrderItemEventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (newOrderItem);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationReading (_order1, "OrderItems", ValueAccess.Current);
        _extension.RelationRead (null, null, (DomainObjectCollection) null, ValueAccess.Current);
        LastCall.Constraints (Mocks_Is.Same (_order1), Mocks_Is.Equal ("OrderItems"), Mocks_Property.Value ("Count", preloadedOrderItems.Count) & new ContainsConstraint (preloadedOrderItems), Mocks_Is.Equal (ValueAccess.Current));

        _extension.RelationChanging (oldOrderItem, "Order", _order1, null);
        _extension.RelationChanging (newOrderItem, "Order", null, _order1);
        _extension.RelationChanging (_order1, "OrderItems", oldOrderItem, newOrderItem);

        oldOrderItemEventReceiver.RelationChanging (oldOrderItem, "Order", _order1, null);

        newOrderItemEventReceiver.RelationChanging (newOrderItem, "Order", null, _order1);

        _order1EventReceiver.RelationChanging (_order1, "OrderItems", oldOrderItem, newOrderItem);

        _extension.RelationChanged (oldOrderItem, "Order");
        _extension.RelationChanged (newOrderItem, "Order");
        _extension.RelationChanged (_order1, "OrderItems");

        oldOrderItemEventReceiver.RelationChanged (oldOrderItem, "Order");

        newOrderItemEventReceiver.RelationChanged (newOrderItem, "Order");

        _order1EventReceiver.RelationChanged (_order1, "OrderItems");
      }

      _mockRepository.ReplayAll ();

      _order1.OrderItems[0] = newOrderItem;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ReplaceInOneToManyRelationWithOldRelatedObject ()
    {
      Assert.Greater (_order1.OrderItems.Count, 0);
      OrderItem oldOrderItem = (OrderItem) _order1.OrderItems[0];

      DomainObjectCollection preloadedOrderItemsOfOrder1 = _order1.OrderItems;
      OrderItem newOrderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem3);
      Order oldOrderOfNewOrderItem = newOrderItem.Order;
      DomainObjectCollection preloadedOrderItems2 = oldOrderOfNewOrderItem.OrderItems;

      _mockRepository.BackToRecord (_extension);
      DomainObjectMockEventReceiver oldOrderItemEventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (oldOrderItem);
      DomainObjectMockEventReceiver newOrderItemEventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (newOrderItem);
      DomainObjectMockEventReceiver oldOrderOfNewOrderItemEventReceiver = _mockRepository.CreateMock<DomainObjectMockEventReceiver> (oldOrderOfNewOrderItem);

      using (_mockRepository.Ordered ())
      {
        _extension.RelationReading (_order1, "OrderItems", ValueAccess.Current);
        _extension.RelationRead (null, null, (DomainObjectCollection) null, ValueAccess.Current);
        LastCall.Constraints (Mocks_Is.Same (_order1), Mocks_Is.Equal ("OrderItems"), Mocks_Property.Value ("Count", preloadedOrderItemsOfOrder1.Count) & new ContainsConstraint (preloadedOrderItemsOfOrder1), Mocks_Is.Equal (ValueAccess.Current));

        _extension.RelationChanging (oldOrderItem, "Order", _order1, null);
        _extension.RelationChanging (newOrderItem, "Order", oldOrderOfNewOrderItem, _order1);
        _extension.RelationChanging (_order1, "OrderItems", oldOrderItem, newOrderItem);
        _extension.RelationChanging (oldOrderOfNewOrderItem, "OrderItems", newOrderItem, null);

        oldOrderItemEventReceiver.RelationChanging (oldOrderItem, "Order", _order1, null);

        newOrderItemEventReceiver.RelationChanging (newOrderItem, "Order", oldOrderOfNewOrderItem, _order1);

        _order1EventReceiver.RelationChanging (_order1, "OrderItems", oldOrderItem, newOrderItem);

        oldOrderOfNewOrderItemEventReceiver.RelationChanging (oldOrderOfNewOrderItem, "OrderItems", newOrderItem, null);

        _extension.RelationChanged (oldOrderItem, "Order");
        _extension.RelationChanged (newOrderItem, "Order");
        _extension.RelationChanged (_order1, "OrderItems");
        _extension.RelationChanged (oldOrderOfNewOrderItem, "OrderItems");

        oldOrderItemEventReceiver.RelationChanged (oldOrderItem, "Order");

        newOrderItemEventReceiver.RelationChanged (newOrderItem, "Order");

        _order1EventReceiver.RelationChanged (_order1, "OrderItems");

        oldOrderOfNewOrderItemEventReceiver.RelationChanged (oldOrderOfNewOrderItem, "OrderItems");
      }

      _mockRepository.ReplayAll ();

      _order1.OrderItems[0] = newOrderItem;

      _mockRepository.VerifyAll ();
    }
  }
}
