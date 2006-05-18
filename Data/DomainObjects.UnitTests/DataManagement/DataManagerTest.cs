using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
  [TestFixture]
  public class DataManagerTest : ClientTransactionBaseTest
  {
    // types

    // static members and constants

    // member fields

    private DataManager _dataManager;

    // construction and disposing

    public DataManagerTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _dataManager = ClientTransactionMock.DataManager;
    }

    [Test]
    public void GetEmptyChangedDomainObjects ()
    {
      Assert.AreEqual (0, _dataManager.GetChangedDomainObjects ().Count);
    }

    [Test]
    public void GetChangedDomainObjects ()
    {
      DataContainer container = TestDataContainerFactory.CreateOrder1DataContainer ();
      _dataManager.RegisterExistingDataContainer (container);
      container["OrderNumber"] = 42;

      DomainObjectCollection changedObjects = _dataManager.GetChangedDomainObjects ();
      Assert.AreEqual (1, changedObjects.Count);
      Assert.AreEqual (container.ID, changedObjects[0].ID);
    }

    [Test]
    public void GetChangedDomainObjectsForMultipleObjects ()
    {
      DataContainer container1 = TestDataContainerFactory.CreateOrder1DataContainer ();
      DataContainer container2 = TestDataContainerFactory.CreateOrderTicket1DataContainer ();
      _dataManager.RegisterExistingDataContainer (container1);
      _dataManager.RegisterExistingDataContainer (container2);

      container2["FileName"] = @"C:\NewFile.jpg";

      DomainObjectCollection changedObjects = _dataManager.GetChangedDomainObjects ();
      Assert.AreEqual (1, changedObjects.Count);
      Assert.AreEqual (container2.ID, changedObjects[0].ID);
    }

    [Test]
    public void GetChangedDomainObjectsForRelationChange ()
    {
      DataContainer order1 = TestDataContainerFactory.CreateOrder1DataContainer ();
      DataContainer orderTicket1 = TestDataContainerFactory.CreateOrderTicket1DataContainer ();
      DataContainer orderTicket2 = TestDataContainerFactory.CreateOrderTicket2DataContainer ();

      DataContainer orderWithoutOrderItemDataContainer =
          TestDataContainerFactory.CreateOrderWithoutOrderItemDataContainer ();

      _dataManager.RegisterExistingDataContainer (order1);
      _dataManager.RegisterExistingDataContainer (orderTicket1);
      _dataManager.RegisterExistingDataContainer (orderTicket2);
      _dataManager.RegisterExistingDataContainer (orderWithoutOrderItemDataContainer);

      RelationEndPointID order1EndPointID = new RelationEndPointID (order1.ID, "OrderTicket");
      ClientTransactionMock.SetRelatedObject (order1EndPointID, orderTicket2.DomainObject);

      DomainObjectCollection changedObjects = _dataManager.GetChangedDomainObjects ();
      Assert.AreEqual (4, changedObjects.Count);
    }

    [Test]
    public void Commit ()
    {
      DataContainer container = TestDataContainerFactory.CreateOrder1DataContainer ();
      _dataManager.RegisterExistingDataContainer (container);
      container["OrderNumber"] = 42;

      _dataManager.Commit ();

      Assert.AreEqual (0, _dataManager.GetChangedDomainObjects ().Count);
      Assert.AreEqual (42, container.PropertyValues["OrderNumber"].OriginalValue);
      Assert.AreEqual (42, container["OrderNumber"]);
      Assert.AreEqual (StateType.Unchanged, container.State);
    }

    [Test]
    public void CommitOneToOneRelationChange ()
    {
      DataContainer order1 = TestDataContainerFactory.CreateOrder1DataContainer ();
      DataContainer orderTicket1 = TestDataContainerFactory.CreateOrderTicket1DataContainer ();
      DataContainer orderTicket2 = TestDataContainerFactory.CreateOrderTicket2DataContainer ();

      DataContainer orderWithoutOrderItemDataContainer =
          TestDataContainerFactory.CreateOrderWithoutOrderItemDataContainer ();

      _dataManager.RegisterExistingDataContainer (order1);
      _dataManager.RegisterExistingDataContainer (orderTicket1);
      _dataManager.RegisterExistingDataContainer (orderTicket2);
      _dataManager.RegisterExistingDataContainer (orderWithoutOrderItemDataContainer);

      RelationEndPointID order1EndPointID = new RelationEndPointID (order1.ID, "OrderTicket");
      ClientTransactionMock.SetRelatedObject (order1EndPointID, orderTicket2.DomainObject);

      _dataManager.Commit ();

      Assert.AreEqual (0, _dataManager.GetChangedDomainObjects ().Count);
      Assert.AreSame (orderTicket2.DomainObject, ClientTransactionMock.GetRelatedObject (order1EndPointID));
      Assert.AreSame (order1.DomainObject, ClientTransactionMock.GetRelatedObject (new RelationEndPointID (orderTicket2.ID, "Order")));
      Assert.IsNull (ClientTransactionMock.GetRelatedObject (new RelationEndPointID (orderTicket1.ID, "Order")));
      Assert.IsNull (ClientTransactionMock.GetRelatedObject (new RelationEndPointID (orderWithoutOrderItemDataContainer.ID, "OrderTicket")));
      Assert.IsFalse (_dataManager.RelationEndPointMap.HasRelationChanged (order1));
      Assert.IsFalse (_dataManager.RelationEndPointMap.HasRelationChanged (orderWithoutOrderItemDataContainer));
      Assert.IsFalse (_dataManager.RelationEndPointMap.HasRelationChanged (orderTicket1));
      Assert.IsFalse (_dataManager.RelationEndPointMap.HasRelationChanged (orderTicket2));
    }

    [Test]
    public void CommitOneToManyRelationChange ()
    {
      Customer customer1 = Customer.GetObject (DomainObjectIDs.Customer1);
      Order order1 = Order.GetObject (DomainObjectIDs.Order1);
      customer1.Orders.Clear ();

      _dataManager.Commit ();

      Assert.AreEqual (0, _dataManager.GetChangedDomainObjects ().Count);
      Assert.IsNull (order1.Customer);
      Assert.AreEqual (0, ClientTransactionMock.GetOriginalRelatedObjects (new RelationEndPointID (customer1.ID, "Orders")).Count);
      Assert.AreEqual (0, customer1.Orders.Count);
    }

    [Test]
    public void RollbackDataContainerMap ()
    {
      Computer computer = new Computer ();
      ObjectID id = computer.ID;

      Assert.AreSame (computer.DataContainer, _dataManager.DataContainerMap[id]);

      _dataManager.Rollback ();

      Assert.IsNull (_dataManager.DataContainerMap[id]);
    }

    [Test]
    public void RollbackObjectEndPoint ()
    {
      Computer computer = new Computer ();
      Employee employee = new Employee ();

      computer.Employee = employee;

      RelationEndPointID employeeEndPointID = new RelationEndPointID (employee.ID, "Computer");
      RelationEndPointID computerEndPointID = new RelationEndPointID (computer.ID, "Employee");

      ObjectEndPoint employeeEndPoint = (ObjectEndPoint) _dataManager.RelationEndPointMap[employeeEndPointID];
      ObjectEndPoint computerEndPoint = (ObjectEndPoint) _dataManager.RelationEndPointMap[computerEndPointID];

      Assert.AreSame (computer.ID, employeeEndPoint.OppositeObjectID);
      Assert.AreSame (employee.ID, computerEndPoint.OppositeObjectID);

      _dataManager.Rollback ();

      Assert.IsNull (_dataManager.RelationEndPointMap[employeeEndPointID]);
      Assert.IsNull (_dataManager.RelationEndPointMap[computerEndPointID]);
    }

    [Test]
    public void RollbackCollectionEndPoint ()
    {
      Order order = new Order ();
      OrderItem orderItem = new OrderItem ();

      orderItem.Order = order;

      RelationEndPointID orderEndPointID = new RelationEndPointID (order.ID, "OrderItems");
      RelationEndPointID orderItemEndPointID = new RelationEndPointID (orderItem.ID, "Order");

      ObjectEndPoint orderItemEndPoint = (ObjectEndPoint) _dataManager.RelationEndPointMap[orderItemEndPointID];
      Assert.AreEqual (order.ID, orderItemEndPoint.OppositeObjectID);

      CollectionEndPoint orderEndPoint = (CollectionEndPoint) _dataManager.RelationEndPointMap[orderEndPointID];

      Assert.AreEqual (1, orderEndPoint.OppositeDomainObjects.Count);
      Assert.IsNotNull (orderEndPoint.OppositeDomainObjects[orderItem.ID]);

      _dataManager.Rollback ();

      orderItemEndPoint = (ObjectEndPoint) _dataManager.RelationEndPointMap[orderItemEndPointID];
      orderEndPoint = (CollectionEndPoint) _dataManager.RelationEndPointMap[orderEndPointID];

      Assert.IsNull (orderEndPoint);
      Assert.IsNull (orderItemEndPoint);
    }

    [Test]
    public void GetChangedDataContainersForCommitWithDeletedObject ()
    {
      OrderItem orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      orderItem1.Delete ();

      _dataManager.GetChangedDataContainersForCommit ();

      // expectation: no exception
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException),
        "Cannot delete DomainObject 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid', because it belongs to a different ClientTransaction.")]
    public void DeleteWithOtherClientTransaction ()
    {
      ClientTransaction clientTransaction = new ClientTransaction ();
      Order order1 = (Order) clientTransaction.GetObject (DomainObjectIDs.Order1);

      _dataManager.Delete (order1);
    }
  }
}
