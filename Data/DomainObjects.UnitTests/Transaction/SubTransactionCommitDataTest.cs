using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;

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

    [Test]
    public void EndPointsAreCorrectFromBothSidesForCompletelyNewObjectGraphs ()
    {
      Order order;
      OrderItem newOrderItem;
      OrderTicket newOrderTicket;
      Official newOfficial;
      Customer newCustomer;
      Ceo newCeo;

      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        order = Order.NewObject ();
        
        newOrderTicket = OrderTicket.NewObject ();
        order.OrderTicket = newOrderTicket;
        
        newOrderItem = OrderItem.NewObject ();
        order.OrderItems.Add (newOrderItem);
        
        newOfficial = Official.NewObject ();
        order.Official = newOfficial;
        
        newCustomer = Customer.NewObject ();
        order.Customer = newCustomer;

        newCeo = Ceo.NewObject ();
        newCustomer.Ceo = newCeo;

        ClientTransactionScope.CurrentTransaction.Commit ();
      }

      Assert.AreSame (order, newOrderTicket.Order);
      Assert.AreSame (newOrderTicket, order.OrderTicket);

      Assert.AreSame (newOrderItem, order.OrderItems[0]);
      Assert.AreSame (order, newOrderItem.Order);

      Assert.AreSame (order, order.Official.Orders[0]);
      Assert.AreSame (newOfficial, order.Official);

      Assert.AreSame (order, order.Customer.Orders[0]);
      Assert.AreSame (newCustomer, order.Customer);

      Assert.AreSame (newCeo, newCustomer.Ceo);
      Assert.AreSame (newCustomer, newCeo.Company);
    }

    [Test]
    public void CommitObjectInSubTransactionAndReloadInParent ()
    {
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        Order orderInSub = Order.GetObject (DomainObjectIDs.Order1);
        Assert.AreNotEqual (4711, orderInSub.OrderNumber);
        orderInSub.OrderNumber = 4711;
        ClientTransactionScope.CurrentTransaction.Commit ();
      }

      Order orderInParent = Order.GetObject (DomainObjectIDs.Order1);
      Assert.AreEqual (4711, orderInParent.OrderNumber);
    }

    [Test]
    public void CommitObjectInSubTransactionAndReloadInNewSub ()
    {
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        Order orderInSub = Order.GetObject (DomainObjectIDs.Order1);
        Assert.AreNotEqual (4711, orderInSub.OrderNumber);
        orderInSub.OrderNumber = 4711;
        ClientTransactionScope.CurrentTransaction.Commit ();
      }

      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        Order orderInSub = Order.GetObject (DomainObjectIDs.Order1);
        Assert.AreEqual (4711, orderInSub.OrderNumber);
      }
    }

    [Test]
    public void ObjectValuesCanBeChangedInParentAndChildSubTransactions ()
    {
      SetDatabaseModifyable ();

      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      Assert.AreNotEqual (7, cwadt.Int32Property);
      Assert.AreNotEqual (8, cwadt.Int16Property);

      using (ClientTransaction.Current.CreateSubTransaction ().EnterScope ())
      {
        cwadt.Int32Property = 7;
        using (ClientTransaction.Current.CreateSubTransaction ().EnterScope ())
        {
          Assert.AreEqual (7, cwadt.Int32Property);
          cwadt.Int16Property = 8;
          ClientTransaction.Current.Commit ();
        }
        Assert.AreEqual (7, cwadt.Int32Property);
        Assert.AreEqual (8, cwadt.Int16Property);
        ClientTransaction.Current.Commit ();
      }
      Assert.AreEqual (7, cwadt.Int32Property);
      Assert.AreEqual (8, cwadt.Int16Property);
      ClientTransactionMock.Commit ();

      using (ClientTransaction.NewTransaction ().EnterScope ())
      {
        ClientTransaction.Current.EnlistDomainObject (cwadt);
        Assert.AreEqual (7, cwadt.Int32Property);
        Assert.AreEqual (8, cwadt.Int16Property);
      }
    }

    [Test]
    public void HasBeenMarkedChangedHandling_WithNestedSubTransactions ()
    {
      SetDatabaseModifyable ();

      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      Assert.AreEqual (StateType.Unchanged, cwadt.InternalDataContainer.State);

      using (ClientTransaction.Current.CreateSubTransaction ().EnterScope ())
      {
        Assert.AreEqual (StateType.Unchanged, cwadt.InternalDataContainer.State);

        using (ClientTransaction.Current.CreateSubTransaction ().EnterScope ())
        {
          cwadt.MarkAsChanged ();
          Assert.AreEqual (StateType.Changed, cwadt.InternalDataContainer.State);

          using (ClientTransaction.Current.CreateSubTransaction ().EnterScope ())
          {
            Assert.AreEqual (StateType.Unchanged, cwadt.InternalDataContainer.State);
            ++cwadt.Int32Property;
            Assert.AreEqual (StateType.Changed, cwadt.InternalDataContainer.State);
            ClientTransaction.Current.Commit ();
            Assert.AreEqual (StateType.Unchanged, cwadt.InternalDataContainer.State);
          }

          ClientTransaction.Current.Commit ();

          Assert.AreEqual (StateType.Unchanged, cwadt.InternalDataContainer.State);
        }

        Assert.AreEqual (StateType.Changed, cwadt.InternalDataContainer.State);
        ClientTransaction.Current.Commit ();
        Assert.AreEqual (StateType.Unchanged, cwadt.InternalDataContainer.State);
      }

      Assert.AreEqual (StateType.Changed, cwadt.InternalDataContainer.State);

      ClientTransactionMock.Commit ();

      Assert.AreEqual (StateType.Unchanged, cwadt.InternalDataContainer.State);
    }

    [Test]
    public void PropertyValue_HasChangedHandling_WithNestedSubTransactions ()
    {
      SetDatabaseModifyable ();

      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].HasChanged);
      Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].HasChanged);
      Assert.AreEqual (32767, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].OriginalValue);
      Assert.AreEqual (2147483647, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].OriginalValue);

      using (ClientTransaction.Current.CreateSubTransaction ().EnterScope ())
      {
        cwadt.Int32Property = 7;
        Assert.IsTrue (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].HasChanged);
        Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].HasChanged);
        Assert.AreEqual (32767, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].OriginalValue);
        Assert.AreEqual (2147483647, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].OriginalValue);

        using (ClientTransaction.Current.CreateSubTransaction ().EnterScope ())
        {
          Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].HasChanged);
          Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].HasChanged);
          Assert.AreEqual (32767, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].OriginalValue);
          Assert.AreEqual (7, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].OriginalValue);

          cwadt.Int16Property = 8;

          Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].HasChanged);
          Assert.IsTrue (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].HasChanged);

          ClientTransaction.Current.Commit ();

          Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].HasChanged);
          Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].HasChanged);
          Assert.AreEqual (8, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].OriginalValue);
          Assert.AreEqual (7, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].OriginalValue);
        }

        Assert.IsTrue (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].HasChanged);
        Assert.IsTrue (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].HasChanged);
        Assert.AreEqual (32767, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].OriginalValue);
        Assert.AreEqual (2147483647, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].OriginalValue);

        ClientTransaction.Current.Commit ();

        Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].HasChanged);
        Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].HasChanged);
        Assert.AreEqual (8, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].OriginalValue);
        Assert.AreEqual (7, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].OriginalValue);
      }

      Assert.IsTrue (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].HasChanged);
      Assert.IsTrue (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].HasChanged);
      Assert.AreEqual (32767, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].OriginalValue);
      Assert.AreEqual (2147483647, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].OriginalValue);

      ClientTransactionMock.Commit ();

      Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].HasChanged);
      Assert.IsFalse (cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].HasChanged);
      Assert.AreEqual (8, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int16Property"].OriginalValue);
      Assert.AreEqual (7, cwadt.InternalDataContainer.PropertyValues[typeof (ClassWithAllDataTypes).FullName + ".Int32Property"].OriginalValue);
    }

    [Test]
    public void ObjectEndPoint_HasChangedHandling_WithNestedSubTransactions ()
    {
      SetDatabaseModifyable ();

      OrderTicket orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      Order oldOrder = orderTicket.Order;
      
      Order newOrder = Order.GetObject (DomainObjectIDs.Order2);
      OrderTicket oldOrderTicket = newOrder.OrderTicket;

      RelationEndPointID propertyID = new RelationEndPointID (orderTicket.ID, typeof (OrderTicket).FullName + ".Order");

      using (ClientTransaction.Current.CreateSubTransaction ().EnterScope ())
      {
        orderTicket.Order = newOrder;
        oldOrder.OrderTicket = oldOrderTicket;
        Assert.IsTrue (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
        Assert.AreEqual (oldOrder.ID, ((ObjectEndPoint)GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);

        using (ClientTransaction.Current.CreateSubTransaction ().EnterScope ())
        {
          Assert.AreEqual (newOrder, orderTicket.Order);

          Assert.IsFalse (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
          Assert.AreEqual (newOrder.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);

          orderTicket.Order = null;
          orderTicket.Order = newOrder;

          Assert.IsTrue (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
          Assert.AreEqual (newOrder.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);

          ClientTransaction.Current.Commit ();
          Assert.IsFalse (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
          Assert.AreEqual (newOrder.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);
        }

        Assert.IsTrue (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
        Assert.AreEqual (oldOrder.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);

        ClientTransaction.Current.Commit ();
        Assert.IsFalse (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
        Assert.AreEqual (newOrder.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);
      }
      Assert.IsTrue (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
      Assert.AreEqual (oldOrder.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);

      ClientTransaction.Current.Commit ();

      Assert.IsFalse (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
      Assert.AreEqual (newOrder.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);
    }

    [Test]
    public void VirtualObjectEndPoint_HasChangedHandling_WithNestedSubTransactions ()
    {
      SetDatabaseModifyable ();

      OrderTicket newOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      Order oldOrder = newOrderTicket.Order;

      Order newOrder = Order.GetObject (DomainObjectIDs.Order2);
      OrderTicket oldOrderTicket = newOrder.OrderTicket;

      RelationEndPointID propertyID = new RelationEndPointID (newOrder.ID, typeof (Order).FullName + ".OrderTicket");

      using (ClientTransaction.Current.CreateSubTransaction ().EnterScope ())
      {
        newOrder.OrderTicket = newOrderTicket;
        oldOrderTicket.Order = oldOrder;

        Assert.IsTrue (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
        Assert.AreEqual (oldOrderTicket.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);

        using (ClientTransaction.Current.CreateSubTransaction ().EnterScope ())
        {
          Assert.AreEqual (newOrderTicket, newOrder.OrderTicket);

          Assert.IsFalse (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
          Assert.AreEqual (newOrderTicket.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);

          newOrder.OrderTicket = null;
          newOrder.OrderTicket = newOrderTicket;

          Assert.IsTrue (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
          Assert.AreEqual (newOrderTicket.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);

          ClientTransaction.Current.Commit ();
          Assert.IsFalse (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
          Assert.AreEqual (newOrderTicket.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);
        }

        Assert.IsTrue (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
        Assert.AreEqual (oldOrderTicket.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);

        ClientTransaction.Current.Commit ();
        Assert.IsFalse (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
        Assert.AreEqual (newOrderTicket.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);
      }
      Assert.IsTrue (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
      Assert.AreEqual (oldOrderTicket.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);

      ClientTransaction.Current.Commit ();

      Assert.IsFalse (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
      Assert.AreEqual (newOrderTicket.ID, ((ObjectEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeObjectID);
    }

    [Test]
    public void CollectionEndPoint_HasChangedHandling_WithNestedSubTransactions ()
    {
      SetDatabaseModifyable ();

      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem newItem = OrderItem.NewObject ();
      OrderItem firstItem = order.OrderItems[0];

      RelationEndPointID propertyID = new RelationEndPointID (order.ID, typeof (Order).FullName + ".OrderItems");

      using (ClientTransaction.Current.CreateSubTransaction ().EnterScope ())
      {
        order.OrderItems.Add (newItem);

        Assert.IsTrue (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
        Assert.IsFalse (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (newItem));
        Assert.IsTrue (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (firstItem));

        using (ClientTransaction.Current.CreateSubTransaction ().EnterScope ())
        {
          Assert.IsTrue (order.OrderItems.ContainsObject (newItem));

          Assert.IsFalse (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
          Assert.IsTrue (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (newItem));
          Assert.IsTrue (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (firstItem));

          order.OrderItems[0].Delete ();
          Assert.IsTrue (order.OrderItems.ContainsObject (newItem));

          Assert.IsTrue (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
          Assert.IsTrue (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (newItem));
          Assert.IsTrue (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (firstItem));

          ClientTransaction.Current.Commit ();

          Assert.IsFalse (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
          Assert.IsTrue (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (newItem));
          Assert.IsFalse (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (firstItem));
        }

        Assert.IsTrue (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
        Assert.IsFalse (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (newItem));
        Assert.IsTrue (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (firstItem));

        ClientTransaction.Current.Commit ();
        Assert.IsFalse (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
        Assert.IsTrue (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (newItem));
        Assert.IsFalse (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (firstItem));
      }
      Assert.IsTrue (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
      Assert.IsFalse (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (newItem));
      Assert.IsTrue (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (firstItem));

      ClientTransaction.Current.Commit ();

      Assert.IsFalse (GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID].HasChanged);
      Assert.IsTrue (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (newItem));
      Assert.IsFalse (((CollectionEndPoint) GetDataManager (ClientTransaction.Current).RelationEndPointMap[propertyID]).OriginalOppositeDomainObjects.ContainsObject (firstItem));
    }

    private DataManager GetDataManager (ClientTransaction transaction)
    {
      return (DataManager) PrivateInvoke.GetNonPublicProperty (transaction, "DataManager");
    }
  }
}