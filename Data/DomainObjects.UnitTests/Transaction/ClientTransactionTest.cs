using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Transaction
{
  [TestFixture]
  public class ClientTransactionTest : ClientTransactionBaseTest
  {
    private enum ApplicationDataKey
    {
      Key1 = 0
    }

    private ClientTransactionEventReceiver _eventReceiver;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      SetDatabaseModifyable ();
    }

    public override void SetUp ()
    {
      base.SetUp ();

      _eventReceiver = new ClientTransactionEventReceiver (ClientTransactionMock);
    }

    public override void TearDown ()
    {
      base.TearDown ();

      _eventReceiver.Unregister ();
    }

    [Test]
    public void DataContainerMapLookUp ()
    {
      DomainObject domainObject1 = ClientTransactionMock.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      Assert.AreEqual (1, _eventReceiver.LoadedDomainObjects.Count);

      DomainObjectCollection domainObjects = (DomainObjectCollection) _eventReceiver.LoadedDomainObjects[0];
      Assert.AreEqual (1, domainObjects.Count);
      Assert.AreSame (domainObject1, domainObjects[0]);
      _eventReceiver.Clear ();

      DomainObject domainObject2 = ClientTransactionMock.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      Assert.AreEqual (0, _eventReceiver.LoadedDomainObjects.Count);

      Assert.AreSame (domainObject1, domainObject2);
    }

    [Test]
    public void LoadingOfMultipleSimpleObjects ()
    {
      ObjectID id1 = DomainObjectIDs.ClassWithAllDataTypes1;
      ObjectID id2 = DomainObjectIDs.ClassWithAllDataTypes2;

      DomainObject domainObject1 = ClientTransactionMock.GetObject (id1);
      Assert.AreEqual (1, _eventReceiver.LoadedDomainObjects.Count);

      DomainObjectCollection domainObjects = (DomainObjectCollection) _eventReceiver.LoadedDomainObjects[0];
      Assert.AreEqual (1, domainObjects.Count);
      Assert.AreSame (domainObject1, domainObjects[0]);
      _eventReceiver.Clear ();

      DomainObject domainObject2 = ClientTransactionMock.GetObject (id2);
      Assert.AreEqual (1, _eventReceiver.LoadedDomainObjects.Count);

      domainObjects = (DomainObjectCollection) _eventReceiver.LoadedDomainObjects[0];
      Assert.AreEqual (1, domainObjects.Count);
      Assert.AreSame (domainObject2, domainObjects[0]);

      Assert.IsFalse (ReferenceEquals (domainObject1, domainObject2));
    }

    [Test]
    public void GetRelatedObjectForAlreadyLoadedObjects ()
    {
      DomainObject order = ClientTransactionMock.GetObject (DomainObjectIDs.Order1);
      DomainObject orderTicket = ClientTransactionMock.GetObject (DomainObjectIDs.OrderTicket1);

      _eventReceiver.Clear ();

      Assert.AreSame (orderTicket, ClientTransactionMock.GetRelatedObject (
          new RelationEndPointID (order.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket")));

      Assert.AreEqual (0, _eventReceiver.LoadedDomainObjects.Count);

      Assert.AreSame (order, ClientTransactionMock.GetRelatedObject (new RelationEndPointID (orderTicket.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order")));
      Assert.AreEqual (0, _eventReceiver.LoadedDomainObjects.Count);
    }

    [Test]
    public void GetRelatedObjectWithLazyLoad ()
    {
      DomainObject orderTicket = ClientTransactionMock.GetObject (DomainObjectIDs.OrderTicket1);
      _eventReceiver.Clear ();
      DomainObject order = ClientTransactionMock.GetRelatedObject (new RelationEndPointID (orderTicket.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"));

      Assert.IsNotNull (order);
      Assert.AreEqual (DomainObjectIDs.Order1, order.ID);
      Assert.AreEqual (1, _eventReceiver.LoadedDomainObjects.Count);

      DomainObjectCollection domainObjects = (DomainObjectCollection) _eventReceiver.LoadedDomainObjects[0];
      Assert.AreEqual (1, domainObjects.Count);
      Assert.AreSame (order, domainObjects[0]);
    }

    [Test]
    public void GetRelatedObjectOverVirtualEndPoint ()
    {
      DomainObject order = ClientTransactionMock.GetObject (DomainObjectIDs.Order1);
      _eventReceiver.Clear ();

      DomainObject orderTicket = ClientTransactionMock.GetRelatedObject (
          new RelationEndPointID (order.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));

      Assert.IsNotNull (orderTicket);
      Assert.AreEqual (DomainObjectIDs.OrderTicket1, orderTicket.ID);
      Assert.AreEqual (1, _eventReceiver.LoadedDomainObjects.Count);

      DomainObjectCollection domainObjects = (DomainObjectCollection) _eventReceiver.LoadedDomainObjects[0];
      Assert.AreEqual (1, domainObjects.Count);
      Assert.AreSame (orderTicket, domainObjects[0]);
    }

    [Test]
    public void GetOptionalRelatedObject ()
    {
      ObjectID id = new ObjectID ("ClassWithValidRelations", new Guid ("{6BE4FA61-E050-469c-9DBA-B47FFBB0F8AD}"));

      DomainObject classWithValidRelation = ClientTransactionMock.GetObject (id);
      _eventReceiver.Clear ();

      Assert.IsNull (ClientTransactionMock.GetRelatedObject (
          new RelationEndPointID (classWithValidRelation.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithValidRelations.ClassWithGuidKeyOptional")));

      Assert.AreEqual (0, _eventReceiver.LoadedDomainObjects.Count);
    }

    [Test]
    public void GetOptionalRelatedObjectOverVirtualEndPoint ()
    {
      ObjectID id = new ObjectID ("ClassWithGuidKey", new Guid ("{672C8754-C617-4b7a-890C-BFEF8AC86564}"));

      DomainObject classWithGuidKey = ClientTransactionMock.GetObject (id);
      _eventReceiver.Clear ();

      Assert.IsNull (ClientTransactionMock.GetRelatedObject (
          new RelationEndPointID (classWithGuidKey.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithValidRelationsOptional")));

      Assert.AreEqual (0, _eventReceiver.LoadedDomainObjects.Count);
    }

    [Test]
    public void GetOptionalRelatedObjectTwice ()
    {
      ObjectID id = new ObjectID ("ClassWithValidRelations", new Guid ("{6BE4FA61-E050-469c-9DBA-B47FFBB0F8AD}"));

      ClientTransactionMock clientTransactionMock = new ClientTransactionMock ();

      DomainObject classWithValidRelation = clientTransactionMock.GetObject (id);
      Assert.AreEqual (1, clientTransactionMock.NumberOfCallsToLoadDataContainer);
      Assert.AreEqual (0, clientTransactionMock.NumberOfCallsToLoadRelatedObject);

      Assert.IsNull (clientTransactionMock.GetRelatedObject (
          new RelationEndPointID (classWithValidRelation.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithValidRelations.ClassWithGuidKeyOptional")));

      Assert.AreEqual (1, clientTransactionMock.NumberOfCallsToLoadDataContainer);
      Assert.AreEqual (0, clientTransactionMock.NumberOfCallsToLoadRelatedObject);

      clientTransactionMock.GetRelatedObject (
          new RelationEndPointID (classWithValidRelation.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithValidRelations.ClassWithGuidKeyOptional"));

      Assert.AreEqual (1, clientTransactionMock.NumberOfCallsToLoadDataContainer);
      Assert.AreEqual (0, clientTransactionMock.NumberOfCallsToLoadRelatedObject);
    }

    [Test]
    public void GetOptionalRelatedObjectOverVirtualEndPointTwice ()
    {
      ObjectID id = new ObjectID ("ClassWithGuidKey", new Guid ("{672C8754-C617-4b7a-890C-BFEF8AC86564}"));

      ClientTransactionMock clientTransactionMock = new ClientTransactionMock ();

      DomainObject classWithGuidKey = clientTransactionMock.GetObject (id);
      Assert.AreEqual (1, clientTransactionMock.NumberOfCallsToLoadDataContainer);
      Assert.AreEqual (0, clientTransactionMock.NumberOfCallsToLoadRelatedObject);

      Assert.IsNull (clientTransactionMock.GetRelatedObject (
          new RelationEndPointID (classWithGuidKey.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithValidRelationsOptional")));

      Assert.AreEqual (1, clientTransactionMock.NumberOfCallsToLoadDataContainer);
      Assert.AreEqual (1, clientTransactionMock.NumberOfCallsToLoadRelatedObject);

      clientTransactionMock.GetRelatedObject (
          new RelationEndPointID (classWithGuidKey.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithValidRelationsOptional"));

      Assert.AreEqual (1, clientTransactionMock.NumberOfCallsToLoadDataContainer);
      Assert.AreEqual (1, clientTransactionMock.NumberOfCallsToLoadRelatedObject);
    }

    [Test]
    public void GetRelatedObjectWithInheritance ()
    {
      DomainObject expectedCeo = ClientTransactionMock.GetObject (DomainObjectIDs.Ceo6);
      DomainObject partner = ClientTransactionMock.GetObject (DomainObjectIDs.Partner1);

      DomainObject actualCeo = ClientTransactionMock.GetRelatedObject (new RelationEndPointID (partner.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo"));
      Assert.AreSame (expectedCeo, actualCeo);
    }

    [Test]
    public void GetRelatedObjects ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
      _eventReceiver.Clear ();

      DomainObjectCollection orders = ClientTransactionMock.GetRelatedObjects (
          new RelationEndPointID (customer.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));

      Assert.IsNotNull (orders);
      Assert.AreEqual (typeof (OrderCollection), orders.GetType (), "Type of collection");
      Assert.AreEqual (2, orders.Count);

      DomainObjectCollection domainObjects = (DomainObjectCollection) _eventReceiver.LoadedDomainObjects[0];
      Assert.AreEqual (2, domainObjects.Count);
    }

    [Test]
    public void GetRelatedObjectsTwice ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
      _eventReceiver.Clear ();

      DomainObjectCollection orders1 = ClientTransactionMock.GetRelatedObjects (
          new RelationEndPointID (customer.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));

      DomainObjectCollection orders2 = ClientTransactionMock.GetRelatedObjects (
          new RelationEndPointID (customer.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));

      Assert.IsTrue (ReferenceEquals (orders1, orders2));

      Assert.AreEqual (1, _eventReceiver.LoadedDomainObjects.Count);
      DomainObjectCollection domainObjects = (DomainObjectCollection) _eventReceiver.LoadedDomainObjects[0];
      Assert.AreEqual (2, domainObjects.Count);
    }

    [Test]
    public void GetRelatedObjectsWithAlreadyLoadedObject ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      _eventReceiver.Clear ();

      DomainObjectCollection orders = ClientTransactionMock.GetRelatedObjects (
          new RelationEndPointID (customer.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));

      Assert.AreSame (order, orders[DomainObjectIDs.Order1]);
      Assert.AreEqual (1, _eventReceiver.LoadedDomainObjects.Count);
    }

    [Test]
    public void LoadedEventDoesNotFireWithEmptyDomainObjectCollection ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer2);
      _eventReceiver.Clear ();

      DomainObjectCollection orders = ClientTransactionMock.GetRelatedObjects (new RelationEndPointID (customer.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));

      Assert.IsNotNull (orders);
      Assert.IsEmpty (orders);
      Assert.AreEqual (0, _eventReceiver.LoadedDomainObjects.Count);
    }

    [Test]
    public void GetRelatedObjectsWithLazyLoad ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      DomainObjectCollection orders = ClientTransactionMock.GetRelatedObjects (
          new RelationEndPointID (customer.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));

      Order order = Order.GetObject (DomainObjectIDs.Order1);

      Assert.IsTrue (ReferenceEquals (order, orders[DomainObjectIDs.Order1]));
    }

    [Test]
    public void GetRelatedObjectsAndNavigateBack ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      DomainObjectCollection orders = ClientTransactionMock.GetRelatedObjects (
          new RelationEndPointID (customer.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders"));

      Assert.AreSame (customer, ClientTransactionMock.GetRelatedObject (
          new RelationEndPointID (orders[0].ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer")));
    }

    [Test]
    public void GetRelatedObjectsWithInheritance ()
    {
      DomainObject industrialSector = ClientTransactionMock.GetObject (DomainObjectIDs.IndustrialSector2);
      DomainObject expectedPartner = ClientTransactionMock.GetObject (DomainObjectIDs.Partner2);

      DomainObjectCollection companies = ClientTransactionMock.GetRelatedObjects (
          new RelationEndPointID (industrialSector.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies"));

      Assert.AreSame (expectedPartner, companies[DomainObjectIDs.Partner2]);
    }

    [Test]
    [ExpectedException (typeof (DataManagementException))]
    public void SetRelatedObjectWithInvalidType ()
    {
      DomainObject order = ClientTransactionMock.GetObject (DomainObjectIDs.Order1);
      DomainObject customer = ClientTransactionMock.GetObject (DomainObjectIDs.Customer1);

      ClientTransactionMock.SetRelatedObject (new RelationEndPointID (order.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"), customer);
    }

    [Test]
    [ExpectedException (typeof (DataManagementException))]
    public void SetRelatedObjectWithBaseType ()
    {
      DomainObject person = ClientTransactionMock.GetObject (DomainObjectIDs.Person1);
      DomainObject company = ClientTransactionMock.GetObject (DomainObjectIDs.Company1);

      ClientTransactionMock.SetRelatedObject (new RelationEndPointID (person.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Person.AssociatedPartnerCompany"), company);
    }

    [Test]
    [ExpectedException (typeof (MandatoryRelationNotSetException),
       ExpectedMessage = "Mandatory relation property 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order' of domain object"
        + " 'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid' cannot be null.")]
    public void CommitWithMandatoryOneToOneRelationNotSet ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderTicket newOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);

      order.OrderTicket = newOrderTicket;

      ClientTransactionMock.Commit ();
    }

    [Test]
    public void CommitWithOptionalOneToOneRelationNotSet ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
      employee.Computer = null;

      ClientTransactionMock.Commit ();

      // expectation: no exception
    }

    [Test]
    [ExpectedException (typeof (MandatoryRelationNotSetException),
      ExpectedMessage = "Mandatory relation property 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies' of domain object"
       + " 'IndustrialSector|8565a077-ea01-4b5d-beaa-293dc484bddc|System.Guid' contains no items.")]
    public void CommitWithMandatoryOneToManyRelationNotSet ()
    {
      IndustrialSector industrialSector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector2);
      industrialSector.Companies.Clear ();

      ClientTransactionMock.Commit ();
    }

    [Test]
    public void CommitTwice ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderTicket oldOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      OrderTicket newOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);

      oldOrderTicket.Order = newOrderTicket.Order;
      order.OrderTicket = newOrderTicket;

      ClientTransactionMock.Commit ();

			object orderTimestamp = order.InternalDataContainer.Timestamp;
			object oldOrderTicketTimestamp = oldOrderTicket.InternalDataContainer.Timestamp;
			object newOrderTicketTimestamp = newOrderTicket.InternalDataContainer.Timestamp;

      ClientTransactionMock.Commit ();

			Assert.AreEqual (orderTimestamp, order.InternalDataContainer.Timestamp);
			Assert.AreEqual (oldOrderTicketTimestamp, oldOrderTicket.InternalDataContainer.Timestamp);
			Assert.AreEqual (newOrderTicketTimestamp, newOrderTicket.InternalDataContainer.Timestamp);
    }

    [Test]
    public void CommitTwiceWithChange ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderTicket oldOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      OrderTicket newOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);
      Order oldOrderOfNewOrderTicket = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);

      oldOrderTicket.Order = newOrderTicket.Order;
      order.OrderTicket = newOrderTicket;

      ClientTransactionMock.Commit ();

			object orderTimestamp = order.InternalDataContainer.Timestamp;
			object oldOrderTicketTimestamp = oldOrderTicket.InternalDataContainer.Timestamp;
			object newOrderTicketTimestamp = newOrderTicket.InternalDataContainer.Timestamp;
			object oldOrderOfNewOrderTicketTimestamp = oldOrderOfNewOrderTicket.InternalDataContainer.Timestamp;

      order.OrderTicket = oldOrderTicket;
      oldOrderOfNewOrderTicket.OrderTicket = newOrderTicket;

      ClientTransactionMock.Commit ();

			Assert.AreEqual (orderTimestamp, order.InternalDataContainer.Timestamp);
			Assert.IsFalse (oldOrderTicketTimestamp.Equals (oldOrderTicket.InternalDataContainer.Timestamp));
			Assert.IsFalse (newOrderTicketTimestamp.Equals (newOrderTicket.InternalDataContainer.Timestamp));
			Assert.AreEqual (oldOrderOfNewOrderTicketTimestamp, oldOrderOfNewOrderTicket.InternalDataContainer.Timestamp);
    }

    [Test]
    public void OppositeDomainObjectsTypeAfterCommit ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      customer.Orders.Add (Order.GetObject (DomainObjectIDs.Order2));
      ClientTransactionMock.Commit ();

      DomainObjectCollection originalOrders = customer.GetOriginalRelatedObjects ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");
      Assert.AreEqual (typeof (OrderCollection), originalOrders.GetType ());
      Assert.IsTrue (originalOrders.IsReadOnly);

      Assert.AreEqual (customer.Orders.RequiredItemType, originalOrders.RequiredItemType);
    }

    [Test]
    public void RollbackReadOnlyOppositeDomainObjects ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
      customer.Orders.Add (Order.GetObject (DomainObjectIDs.Order2));

      customer.Orders.SetIsReadOnly (true);
      ClientTransactionMock.Rollback ();

      Assert.IsTrue (customer.GetOriginalRelatedObjects ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders").IsReadOnly);
      Assert.IsTrue (customer.Orders.IsReadOnly);
    }

    [Test]
    [ExpectedException (typeof (ObjectNotFoundException))]
    public void CommitDeletedObject ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      computer.Delete ();
      ClientTransactionMock.Commit ();

      Computer.GetObject (DomainObjectIDs.Computer1);
    }

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException))]
    public void AccessDeletedObjectAfterCommit ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      computer.Delete ();
      ClientTransactionMock.Commit ();

      string serialNumber = computer.SerialNumber;
    }

    [Test]
    public void GetObjectByNewIndependentTransaction ()
    {
      ClientTransaction clientTransaction = new ClientTransaction ();
      Order order = Order.GetObject (DomainObjectIDs.Order1, clientTransaction);

			Assert.AreSame (clientTransaction, order.InternalDataContainer.ClientTransaction);
    }

    [Test]
    public void GetDeletedObjectByNewIndependentTransaction ()
    {
      ClientTransaction clientTransaction = new ClientTransaction ();
      Order order = Order.GetObject (DomainObjectIDs.Order1, clientTransaction);

      order.Delete ();

      order = Order.GetObject (DomainObjectIDs.Order1, clientTransaction, true);
      Assert.AreEqual (StateType.Deleted, order.State);
			Assert.AreSame (clientTransaction, order.InternalDataContainer.ClientTransaction);
    }

    [Test]
    public void CommitIndependentTransactions ()
    {
      ClientTransaction clientTransaction1 = new ClientTransaction ();
      ClientTransaction clientTransaction2 = new ClientTransaction ();

      Order order1 = Order.GetObject (DomainObjectIDs.Order1, clientTransaction1);
      order1.OrderNumber = 50;

      Order order2 = Order.GetObject (DomainObjectIDs.Order2, clientTransaction2);
      order2.OrderNumber = 60;

      clientTransaction1.Commit ();
      clientTransaction2.Commit ();

      ClientTransaction clientTransaction3 = new ClientTransaction ();

      Order changedOrder1 = Order.GetObject (DomainObjectIDs.Order1, clientTransaction3);
      Order changedOrder2 = Order.GetObject (DomainObjectIDs.Order2, clientTransaction3);

      Assert.IsFalse (ReferenceEquals (order1, changedOrder1));
      Assert.IsFalse (ReferenceEquals (order2, changedOrder2));

      Assert.AreEqual (50, changedOrder1.OrderNumber);
      Assert.AreEqual (60, changedOrder2.OrderNumber);
    }

    [Test]
    public void QueryManager ()
    {
      Assert.IsNotNull (ClientTransactionMock.QueryManager);
      Assert.AreSame (ClientTransactionMock, ClientTransactionMock.QueryManager.ClientTransaction);
    }

    [Test]
    public void AutoInitializationOfCurrent ()
    {
      ClientTransaction.SetCurrent (null);
      Assert.IsNotNull (ClientTransaction.Current);
    }

    [Test]
    public void HasCurrentTrue ()
    {
      Assert.IsTrue (ClientTransaction.HasCurrent);
    }

    [Test]
    public void HasCurrentFalse ()
    {
      ClientTransaction.SetCurrent (null);
      Assert.IsFalse (ClientTransaction.HasCurrent);
    }

    [Test]
    public void MandatoryRelationNotSetExceptionForOneToOneRelation ()
    {
      OrderTicket newOrderTicket = OrderTicket.NewObject ();

      try
      {
        ClientTransaction.Current.Commit ();
        Assert.Fail ("MandatoryRelationNotSetException was expected");
      }
      catch (MandatoryRelationNotSetException ex)
      {
        string expectedMessage = string.Format ("Mandatory relation property 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order' of domain object '{0}' cannot be null.", newOrderTicket.ID);
        Assert.AreEqual (expectedMessage, ex.Message);
        Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", ex.PropertyName);
        Assert.AreSame (newOrderTicket, ex.DomainObject);
      }
    }

    [Test]
    public void MandatoryRelationNotSetExceptionForOneToManyRelation ()
    {
      IndustrialSector newIndustrialSector = IndustrialSector.NewObject ();

      try
      {
        ClientTransaction.Current.Commit ();
        Assert.Fail ("MandatoryRelationNotSetException was expected");
      }
      catch (MandatoryRelationNotSetException ex)
      {
        string expectedMessage = string.Format ("Mandatory relation property 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies' of domain object '{0}' contains no items.", newIndustrialSector.ID);
        Assert.AreEqual (expectedMessage, ex.Message);
        Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies", ex.PropertyName);
        Assert.AreSame (newIndustrialSector, ex.DomainObject);
      }
    }

    [Test]
    public void HasChanged ()
    {
      Assert.IsFalse (ClientTransaction.Current.HasChanged ());
      Order order1 = Order.GetObject (DomainObjectIDs.Order1);
      order1.OrderNumber = order1.OrderNumber + 1;
      Assert.IsTrue (ClientTransaction.Current.HasChanged ());
    }

    [Test]
    public void ApplicationData ()
    {
      Assert.IsNotNull (ClientTransaction.Current.ApplicationData);
      Assert.IsAssignableFrom (typeof (Dictionary<Enum, object>), ClientTransaction.Current.ApplicationData);

      Assert.IsFalse (ClientTransaction.Current.ApplicationData.ContainsKey (ApplicationDataKey.Key1));
      ClientTransaction.Current.ApplicationData[ApplicationDataKey.Key1] = "TestData";
      Assert.AreEqual ("TestData", ClientTransaction.Current.ApplicationData[ApplicationDataKey.Key1]);
      ClientTransaction.Current.ApplicationData.Remove (ApplicationDataKey.Key1);
      Assert.IsFalse (ClientTransaction.Current.ApplicationData.ContainsKey (ApplicationDataKey.Key1));
    }
  }
}
