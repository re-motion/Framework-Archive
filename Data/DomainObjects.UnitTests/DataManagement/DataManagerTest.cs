using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Relations;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.Transaction;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
[TestFixture]
public class DataManagerTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  private ClientTransactionMock _clientTransaction;
  private DataManager _dataManager;

  // construction and disposing

  public DataManagerTest ()
  {
  }

  // methods and properties

  public override void SetUp ()
  {
    base.SetUp ();

    _clientTransaction = new ClientTransactionMock ();
    ClientTransaction.SetCurrent (_clientTransaction);

    _dataManager = _clientTransaction.DataManager;
  }

  [Test]
  public void GetEmptyChangedDomainObjects ()
  {
    Assert.AreEqual (0, _dataManager.GetChangedDomainObjects().Count);
  }

  [Test]
  public void GetChangedDomainObjects ()
  {
    DataContainer container = TestDataContainerFactory.CreateOrder1DataContainer ();
    _dataManager.Register (container);
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
    _dataManager.Register (container1);
    _dataManager.Register (container2);

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

    _dataManager.Register (order1);
    _dataManager.Register (orderTicket1);
    _dataManager.Register (orderTicket2);
    _dataManager.Register (orderWithoutOrderItemDataContainer);

    RelationEndPoint order1EndPoint = new RelationEndPoint (order1, "OrderTicket");
    _clientTransaction.SetRelatedObject (order1EndPoint, orderTicket2.DomainObject);

    DomainObjectCollection changedObjects = _dataManager.GetChangedDomainObjects ();
    Assert.AreEqual (4, changedObjects.Count);
  }

  [Test]
  public void Commit ()
  {
    DataContainer container = TestDataContainerFactory.CreateOrder1DataContainer ();
    _dataManager.Register (container);
    container["OrderNumber"] = 42;

    _dataManager.Commit ();

    Assert.AreEqual (0, _dataManager.GetChangedDomainObjects().Count);
    Assert.AreEqual (42, container.PropertyValues["OrderNumber"].OriginalValue);
    Assert.AreEqual (42, container["OrderNumber"]);
    Assert.AreEqual (StateType.Original, container.State);
  }

  [Test]
  public void CommitOneToOneRelationChange ()
  {
    DataContainer order1 = TestDataContainerFactory.CreateOrder1DataContainer ();
    DataContainer orderTicket1 = TestDataContainerFactory.CreateOrderTicket1DataContainer ();
    DataContainer orderTicket2 = TestDataContainerFactory.CreateOrderTicket2DataContainer ();

    DataContainer orderWithoutOrderItemDataContainer = 
        TestDataContainerFactory.CreateOrderWithoutOrderItemDataContainer ();

    _dataManager.Register (order1);
    _dataManager.Register (orderTicket1);
    _dataManager.Register (orderTicket2);
    _dataManager.Register (orderWithoutOrderItemDataContainer);

    RelationEndPoint order1EndPoint = new RelationEndPoint (order1, "OrderTicket");
    _clientTransaction.SetRelatedObject (order1EndPoint, orderTicket2.DomainObject);

    _dataManager.Commit ();

    Assert.AreEqual (0, _dataManager.GetChangedDomainObjects().Count);
    Assert.AreSame (orderTicket2.DomainObject, _clientTransaction.GetRelatedObject (order1EndPoint));
    Assert.AreSame (order1.DomainObject, _clientTransaction.GetRelatedObject (new RelationEndPoint (orderTicket2, "Order")));
    Assert.IsNull (_clientTransaction.GetRelatedObject (new RelationEndPoint (orderTicket1, "Order")));
    Assert.IsNull (_clientTransaction.GetRelatedObject (new RelationEndPoint (orderWithoutOrderItemDataContainer, "OrderTicket")));
    Assert.IsFalse (_dataManager.HasRelationChanged (order1));
    Assert.IsFalse (_dataManager.HasRelationChanged (orderWithoutOrderItemDataContainer));
    Assert.IsFalse (_dataManager.HasRelationChanged (orderTicket1));
    Assert.IsFalse (_dataManager.HasRelationChanged (orderTicket2));
  }

  [Test]
  public void CommitOneToManyRelationChange ()
  {
    Customer customer1 = Customer.GetObject (DomainObjectIDs.Customer1);
    Order order1 = Order.GetObject (DomainObjectIDs.Order1);
    customer1.Orders.Clear ();
    
    _dataManager.Commit ();

    Assert.AreEqual (0, _dataManager.GetChangedDomainObjects().Count);
    Assert.IsNull (order1.Customer);
    Assert.AreEqual (0, _clientTransaction.GetOriginalRelatedObjects(new RelationEndPoint (customer1, "Orders")).Count);
    Assert.AreEqual (0, customer1.Orders.Count);
  }

  [Test]
  public void RollbackDataContainerMap ()
  {
    Computer computer = new Computer ();
    ObjectID id = computer.ID;

    Assert.AreSame (computer.DataContainer, _dataManager.GetObject (id));

    _dataManager.Rollback ();
    
    Assert.IsNull (_dataManager.GetObject (id));
  }

  [Test]
  public void RollbackSingleObjectRelationLinkMap ()
  {
    Computer computer = new Computer ();
    Employee employee = new Employee ();

    computer.Employee = employee;

    RelationEndPoint computerEndPoint = new RelationEndPoint (computer, "Employee");
    RelationEndPoint employeeEndPoint = new RelationEndPoint (employee, "Computer");

    Assert.AreSame (computer.ID, _dataManager.GetSingleObjectRelationLink (employeeEndPoint).DestinationObjectID);
    Assert.AreSame (employee.ID, _dataManager.GetSingleObjectRelationLink (computerEndPoint).DestinationObjectID);

    _dataManager.Rollback ();

    Assert.IsNull (_dataManager.GetSingleObjectRelationLink (employeeEndPoint));
    Assert.IsNull (_dataManager.GetSingleObjectRelationLink (computerEndPoint));
  }

  [Test]
  public void RollbackMultipleObjectRelationLinkMap ()
  {
    Order order = new Order ();
    OrderItem orderItem = new OrderItem ();
  
    orderItem.Order = order;

    RelationEndPoint orderEndPoint = new RelationEndPoint (order, "OrderItems");
    RelationEndPoint orderItemEndPoint = new RelationEndPoint (orderItem, "Order");

    Assert.AreSame (order.ID, _dataManager.GetSingleObjectRelationLink (orderItemEndPoint).DestinationObjectID);

    DomainObjectCollection orderItems = 
        _dataManager.GetMultipleObjectsRelationLink (orderEndPoint).DestinationDomainObjects;

    Assert.AreEqual (1, orderItems.Count);
    Assert.IsNotNull (orderItems[orderItem.ID]);
    Assert.AreSame (order.ID, _dataManager.GetSingleObjectRelationLink (orderItemEndPoint).DestinationObjectID);

    _dataManager.Rollback ();

    Assert.IsNull (_dataManager.GetMultipleObjectsRelationLink (orderEndPoint));
    Assert.IsNull (_dataManager.GetSingleObjectRelationLink (orderItemEndPoint));
  }
}
}
