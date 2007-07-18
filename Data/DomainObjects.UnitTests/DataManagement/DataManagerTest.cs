using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
  [TestFixture]
  public class DataManagerTest : ClientTransactionBaseTest
  {
    private DataManager _dataManager;

    public override void SetUp ()
    {
      base.SetUp ();

      _dataManager = ClientTransactionMock.DataManager;
    }

    [Test]
    public void GetEmptyDomainObjectsFromStateTypeOverload ()
    {
      DomainObjectCollection domainObjects = _dataManager.GetDomainObjects (StateType.Unchanged);
      Assert.IsNotNull (domainObjects);
      Assert.AreEqual (0, domainObjects.Count);
    }

    [Test]
    public void GetUnchangedDomainObjectsFromStateTypeOverload ()
    {
      DataContainer container = TestDataContainerFactory.CreateOrder1DataContainer ();
      _dataManager.RegisterExistingDataContainer (container);

      DomainObjectCollection domainObjects = _dataManager.GetDomainObjects (StateType.Unchanged);
      Assert.IsNotNull (domainObjects);
      Assert.AreEqual (1, domainObjects.Count);
      Assert.AreSame (container.DomainObject, domainObjects[0]);
    }

    [Test]
    public void GetUnchangedDomainObjects ()
    {
      DataContainer container = TestDataContainerFactory.CreateOrder1DataContainer ();
      _dataManager.RegisterExistingDataContainer (container);

      DomainObjectCollection domainObjects = _dataManager.GetDomainObjects (new StateType[] { StateType.Unchanged });
      Assert.IsNotNull (domainObjects);
      Assert.AreEqual (1, domainObjects.Count);
      Assert.AreSame (container.DomainObject, domainObjects[0]);
    }

    [Test]
    public void GetChangedAndUnchangedDomainObjects ()
    {
      DataContainer container1 = TestDataContainerFactory.CreateOrder1DataContainer ();
      DataContainer container2 = TestDataContainerFactory.CreateOrder2DataContainer ();
      _dataManager.RegisterExistingDataContainer (container1);
      _dataManager.RegisterExistingDataContainer (container2);

      container1.SetValue (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber",
          (int) container1.GetValue ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber") + 1);

      DomainObjectCollection domainObjects = _dataManager.GetDomainObjects (new StateType[] { StateType.Changed });
      Assert.IsNotNull (domainObjects);
      Assert.AreEqual (1, domainObjects.Count);
      Assert.AreSame (container1.DomainObject, domainObjects[0]);

      domainObjects = _dataManager.GetDomainObjects (new StateType[] { StateType.Unchanged });
      Assert.IsNotNull (domainObjects);
      Assert.AreEqual (1, domainObjects.Count);
      Assert.AreSame (container2.DomainObject, domainObjects[0]);    
    }

    [Test]
    public void GetDeletedAndUnchangedDomainObjects ()
    {
      DataContainer container1 = TestDataContainerFactory.CreateClassWithAllDataTypesDataContainer ();
      DataContainer container2 = TestDataContainerFactory.CreateOrder2DataContainer ();
      _dataManager.RegisterExistingDataContainer (container1);
      _dataManager.RegisterExistingDataContainer (container2);

      _dataManager.Delete (container1.DomainObject);

      DomainObjectCollection domainObjects = _dataManager.GetDomainObjects (new StateType[] { StateType.Deleted });
      Assert.IsNotNull (domainObjects);
      Assert.AreEqual (1, domainObjects.Count);
      Assert.AreSame (container1.DomainObject, domainObjects[0]);

      domainObjects = _dataManager.GetDomainObjects (new StateType[] { StateType.Unchanged });
      Assert.IsNotNull (domainObjects);
      Assert.AreEqual (1, domainObjects.Count);
      Assert.AreSame (container2.DomainObject, domainObjects[0]);
    }

    [Test]
    public void GetNewAndUnchangedDomainObjects ()
    {
			DataContainer container1 = Order.NewObject ().InternalDataContainer;
      DataContainer container2 = TestDataContainerFactory.CreateOrder2DataContainer ();
      _dataManager.RegisterExistingDataContainer (container2);

      DomainObjectCollection domainObjects = _dataManager.GetDomainObjects (new StateType[] { StateType.New });
      Assert.IsNotNull (domainObjects);
      Assert.AreEqual (1, domainObjects.Count);
      Assert.AreSame (container1.DomainObject, domainObjects[0]);

      domainObjects = _dataManager.GetDomainObjects (new StateType[] { StateType.Unchanged });
      Assert.IsNotNull (domainObjects);
      Assert.AreEqual (1, domainObjects.Count);
      Assert.AreSame (container2.DomainObject, domainObjects[0]);
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
      container["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"] = 42;

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

      container2["Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.FileName"] = @"C:\NewFile.jpg";

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

      DataContainer orderWithoutOrderItemDataContainer = TestDataContainerFactory.CreateOrderWithoutOrderItemDataContainer ();

      _dataManager.RegisterExistingDataContainer (order1);
      _dataManager.RegisterExistingDataContainer (orderTicket1);
      _dataManager.RegisterExistingDataContainer (orderTicket2);
      _dataManager.RegisterExistingDataContainer (orderWithoutOrderItemDataContainer);

      RelationEndPointID order1EndPointID = new RelationEndPointID (order1.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");
      ClientTransactionMock.SetRelatedObject (order1EndPointID, orderTicket2.DomainObject);

      DomainObjectCollection changedObjects = _dataManager.GetChangedDomainObjects ();
      Assert.AreEqual (4, changedObjects.Count);
    }

    [Test]
    public void Commit ()
    {
      DataContainer container = TestDataContainerFactory.CreateOrder1DataContainer ();
      _dataManager.RegisterExistingDataContainer (container);
      container["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"] = 42;

      _dataManager.Commit ();

      Assert.AreEqual (0, _dataManager.GetChangedDomainObjects ().Count);
      Assert.AreEqual (42, container.PropertyValues["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].OriginalValue);
      Assert.AreEqual (42, container["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"]);
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

      RelationEndPointID order1EndPointID = new RelationEndPointID (order1.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");
      ClientTransactionMock.SetRelatedObject (order1EndPointID, orderTicket2.DomainObject);

      _dataManager.Commit ();

      Assert.AreEqual (0, _dataManager.GetChangedDomainObjects ().Count);
      Assert.AreSame (orderTicket2.DomainObject, ClientTransactionMock.GetRelatedObject (order1EndPointID));
      Assert.AreSame (order1.DomainObject, ClientTransactionMock.GetRelatedObject (new RelationEndPointID (orderTicket2.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order")));
      Assert.IsNull (ClientTransactionMock.GetRelatedObject (new RelationEndPointID (orderTicket1.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order")));
      Assert.IsNull (ClientTransactionMock.GetRelatedObject (new RelationEndPointID (orderWithoutOrderItemDataContainer.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket")));
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
      Assert.AreEqual (0, ClientTransactionMock.GetOriginalRelatedObjects (new RelationEndPointID (customer1.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders")).Count);
      Assert.AreEqual (0, customer1.Orders.Count);
    }

    [Test]
    public void RollbackDataContainerMap ()
    {
      Computer computer = Computer.NewObject ();
      ObjectID id = computer.ID;

			Assert.AreSame (computer.InternalDataContainer, _dataManager.DataContainerMap[id]);

      _dataManager.Rollback ();

      Assert.IsNull (_dataManager.DataContainerMap[id]);
    }

    [Test]
    public void RollbackObjectEndPoint ()
    {
      Computer computer = Computer.NewObject ();
      Employee employee = Employee.NewObject ();

      computer.Employee = employee;

      RelationEndPointID employeeEndPointID = new RelationEndPointID (employee.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Computer");
      RelationEndPointID computerEndPointID = new RelationEndPointID (computer.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee");

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
      Order order = Order.NewObject ();
      OrderItem orderItem = OrderItem.NewObject ();

      orderItem.Order = order;

      RelationEndPointID orderEndPointID = new RelationEndPointID (order.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");
      RelationEndPointID orderItemEndPointID = new RelationEndPointID (orderItem.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order");

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
       ExpectedMessage = "Domain object '.*' cannot be used in the current transaction as it was loaded or created in another transaction.",
       MatchType = MessageMatch.Regex)]
    public void DeleteWithOtherClientTransaction ()
    {
      ClientTransaction clientTransaction = ClientTransaction.NewTransaction();
      Order order1;
      using (new ClientTransactionScope ())
      {
        order1 = Order.GetObject (DomainObjectIDs.Order1);
      }
      _dataManager.Delete (order1);
    }

    [Test]
    public void IsDiscarded ()
    {
      OrderItem orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      Assert.IsFalse (_dataManager.IsDiscarded (orderItem1.ID));
      Assert.AreEqual (0, _dataManager.DiscardedObjectCount);
      _dataManager.MarkDiscarded (orderItem1.ID);
      Assert.IsTrue (_dataManager.IsDiscarded (orderItem1.ID));
      Assert.AreEqual (1, _dataManager.DiscardedObjectCount);
    }

    [Test]
    public void DeleteLoadedDiscardsOnCommit ()
    {
      using (ClientTransactionMock.EnterScope ())
      {
        SetDatabaseModifyable();

        OrderItem orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
        orderItem1.Delete();

        Assert.IsFalse (_dataManager.IsDiscarded (orderItem1.ID));

        ClientTransactionMock.Commit();

        Assert.IsTrue (_dataManager.IsDiscarded (orderItem1.ID));
      }
    }

    [Test]
    public void DeleteNewDiscardsImmediatelyOnCommit ()
    {
      using (ClientTransactionMock.EnterScope ())
      {
        OrderItem orderItem1 = OrderItem.NewObject();

        Assert.IsFalse (_dataManager.IsDiscarded (orderItem1.ID));

        orderItem1.Delete();

        Assert.IsTrue (_dataManager.IsDiscarded (orderItem1.ID));
      }
    }

    [Test]
    public void CopyFromEmpty ()
    {
      ClientTransactionMock emptyTransaction = new ClientTransactionMock ();
      _dataManager.CopyFrom (emptyTransaction.DataManager);

      Assert.AreEqual (0, _dataManager.DataContainerMap.Count);
      Assert.AreEqual (0, _dataManager.RelationEndPointMap.Count);
      Assert.AreEqual (0, _dataManager.DiscardedObjectCount);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Source cannot be the destination DataManager instance.",
        MatchType = MessageMatch.Contains)]
    public void CannotCopyFromSelf ()
    {
      _dataManager.CopyFrom (_dataManager);
    }

    [Test]
    public void CopyFromNonEmpty ()
    {
      ClientTransactionMock nonEmptyTransaction = new ClientTransactionMock ();
      using (nonEmptyTransaction .EnterScope ())
      {
        Order order = Order.GetObject (DomainObjectIDs.Order1);
        OrderItem item = order.OrderItems[0];
        Official official = order.Official;
        nonEmptyTransaction.DataManager.MarkDiscarded (DomainObjectIDs.Order2);
      }

      Assert.IsTrue (nonEmptyTransaction.DataManager.IsDiscarded (DomainObjectIDs.Order2));
      Assert.AreNotEqual (0, nonEmptyTransaction.DataManager.DiscardedObjectCount);
      Assert.AreNotEqual (0, nonEmptyTransaction.DataManager.DataContainerMap.Count);
      Assert.AreNotEqual (0, nonEmptyTransaction.DataManager.RelationEndPointMap.Count);

      _dataManager.CopyFrom (nonEmptyTransaction.DataManager);

      Assert.IsTrue (_dataManager.IsDiscarded (DomainObjectIDs.Order2));
      Assert.AreEqual (nonEmptyTransaction.DataManager.DiscardedObjectCount, _dataManager.DiscardedObjectCount);
      Assert.AreEqual (nonEmptyTransaction.DataManager.DataContainerMap.Count, _dataManager.DataContainerMap.Count);
      Assert.AreEqual (nonEmptyTransaction.DataManager.RelationEndPointMap.Count, _dataManager.RelationEndPointMap.Count);
    }
  }
}
