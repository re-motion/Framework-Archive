using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Relations;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Transaction
{
[TestFixture]
public class ClientTransactionTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields
  
  private ClientTransactionMock _clientTransactionMock;
  private ClientTransactionEventReceiver _eventReceiver;

  // construction and disposing

  public ClientTransactionTest ()
  {
  }

  // methods and properties

  public override void SetUp ()
  {
    base.SetUp ();

    _clientTransactionMock = new ClientTransactionMock ();
    ClientTransaction.SetCurrent (_clientTransactionMock);
    _eventReceiver = new ClientTransactionEventReceiver (_clientTransactionMock);
  }

  public override void TearDown ()
  {
    base.TearDown ();

    _eventReceiver.Unregister ();
  }

  [Test]
  public void DataContainerMapLookUp ()
  {
    ObjectID id = new ObjectID (c_testDomainProviderID, 
        "ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

    DomainObject domainObject1 = _clientTransactionMock.GetObject (id);
    Assert.AreEqual (1, _eventReceiver.LoadedDomainObjects.Count);
    Assert.AreSame (domainObject1, _eventReceiver.LoadedDomainObjects[0]);
    _eventReceiver.Clear ();

    DomainObject domainObject2 = _clientTransactionMock.GetObject (id);
    Assert.AreEqual (0, _eventReceiver.LoadedDomainObjects.Count);

    Assert.AreSame (domainObject1, domainObject2);
  }

  [Test]
  public void LoadingOfMultipleSimpleObjects ()
  {
    ObjectID id1 = new ObjectID (c_testDomainProviderID, 
        "ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

    ObjectID id2 = new ObjectID (c_testDomainProviderID, 
        "ClassWithAllDataTypes", new Guid ("{583EC716-8443-4b55-92BF-09F7C8768529}"));
    
    DomainObject domainObject1 = _clientTransactionMock.GetObject (id1);
    Assert.AreEqual (1, _eventReceiver.LoadedDomainObjects.Count);
    Assert.AreSame (domainObject1, _eventReceiver.LoadedDomainObjects[0]);
    _eventReceiver.Clear ();

    DomainObject domainObject2 = _clientTransactionMock.GetObject (id2);
    Assert.AreEqual (1, _eventReceiver.LoadedDomainObjects.Count);
    Assert.AreSame (domainObject2, _eventReceiver.LoadedDomainObjects[0]);
    
    Assert.IsFalse (object.ReferenceEquals (domainObject1, domainObject2));
  }

  [Test]
  public void GetRelatedObjectForAlreadyLoadedObjects ()
  {
    DomainObject order = _clientTransactionMock.GetObject (DomainObjectIDs.Order1);
    DomainObject orderTicket = _clientTransactionMock.GetObject (DomainObjectIDs.OrderTicket1);
    
    _eventReceiver.Clear ();

    Assert.AreSame (orderTicket, _clientTransactionMock.GetRelatedObject (
        new RelationEndPoint (order, "OrderTicket")));

    Assert.AreEqual (0, _eventReceiver.LoadedDomainObjects.Count);

    Assert.AreSame (order, _clientTransactionMock.GetRelatedObject (new RelationEndPoint (orderTicket, "Order")));
    Assert.AreEqual (0, _eventReceiver.LoadedDomainObjects.Count);
  }

  [Test]
  public void GetRelatedObjectWithLazyLoad ()
  {
    DomainObject orderTicket = _clientTransactionMock.GetObject (DomainObjectIDs.OrderTicket1);
    _eventReceiver.Clear();
    DomainObject order = _clientTransactionMock.GetRelatedObject (new RelationEndPoint (orderTicket, "Order"));

    Assert.IsNotNull (order);
    Assert.AreEqual (DomainObjectIDs.Order1, order.ID);
    Assert.AreEqual (1, _eventReceiver.LoadedDomainObjects.Count);
    Assert.AreSame (order, _eventReceiver.LoadedDomainObjects[0]);
  }

  [Test]
  public void GetRelatedObjectOverVirtualEndPoint ()
  {
    DomainObject order = _clientTransactionMock.GetObject (DomainObjectIDs.Order1);
    _eventReceiver.Clear();

    DomainObject orderTicket = _clientTransactionMock.GetRelatedObject (
        new RelationEndPoint (order, "OrderTicket"));

    Assert.IsNotNull (orderTicket);
    Assert.AreEqual (DomainObjectIDs.OrderTicket1, orderTicket.ID);
    Assert.AreEqual (1, _eventReceiver.LoadedDomainObjects.Count);
    Assert.AreSame (orderTicket, _eventReceiver.LoadedDomainObjects[0]);
  }

  [Test]
  public void GetOptionalRelatedObject ()
  {
    ObjectID id = new ObjectID (c_testDomainProviderID, "ClassWithValidRelations", 
        new Guid ("{6BE4FA61-E050-469c-9DBA-B47FFBB0F8AD}"));

    DomainObject classWithValidRelation = _clientTransactionMock.GetObject (id);
    _eventReceiver.Clear ();

    Assert.IsNull (_clientTransactionMock.GetRelatedObject (
        new RelationEndPoint (classWithValidRelation, "ClassWithGuidKeyOptional")));

    Assert.AreEqual (0, _eventReceiver.LoadedDomainObjects.Count);
  }

  [Test]
  public void GetOptionalRelatedObjectOverVirtualEndPoint ()
  {
    ObjectID id = new ObjectID (c_testDomainProviderID, "ClassWithGuidKey", 
        new Guid ("{672C8754-C617-4b7a-890C-BFEF8AC86564}"));

    DomainObject classWithGuidKey = _clientTransactionMock.GetObject (id);
    _eventReceiver.Clear ();

    Assert.IsNull (_clientTransactionMock.GetRelatedObject (
        new RelationEndPoint (classWithGuidKey, "ClassWithValidRelationsOptional")));

    Assert.AreEqual (0, _eventReceiver.LoadedDomainObjects.Count);
  }

  [Test]
  public void GetOptionalRelatedObjectTwice ()
  {
    ObjectID id = new ObjectID (c_testDomainProviderID, "ClassWithValidRelations", 
        new Guid ("{6BE4FA61-E050-469c-9DBA-B47FFBB0F8AD}"));

    ClientTransactionMock clientTransactionMock = new ClientTransactionMock ();

    DomainObject classWithValidRelation = clientTransactionMock.GetObject (id);
    Assert.AreEqual (1, clientTransactionMock.NumberOfCallsToLoadDataContainer);
    Assert.AreEqual (0, clientTransactionMock.NumberOfCallsToLoadRelatedObject);

    Assert.IsNull (clientTransactionMock.GetRelatedObject (
        new RelationEndPoint (classWithValidRelation, "ClassWithGuidKeyOptional")));

    Assert.AreEqual (1, clientTransactionMock.NumberOfCallsToLoadDataContainer);
    Assert.AreEqual (0, clientTransactionMock.NumberOfCallsToLoadRelatedObject);
    
    clientTransactionMock.GetRelatedObject (
        new RelationEndPoint (classWithValidRelation, "ClassWithGuidKeyOptional"));

    Assert.AreEqual (1, clientTransactionMock.NumberOfCallsToLoadDataContainer);
    Assert.AreEqual (0, clientTransactionMock.NumberOfCallsToLoadRelatedObject);     
  }

  [Test]
  public void GetOptionalRelatedObjectOverVirtualEndPointTwice ()
  {
    ObjectID id = new ObjectID (c_testDomainProviderID, "ClassWithGuidKey", 
        new Guid ("{672C8754-C617-4b7a-890C-BFEF8AC86564}"));

    ClientTransactionMock clientTransactionMock = new ClientTransactionMock ();

    DomainObject classWithGuidKey = clientTransactionMock.GetObject (id);
    Assert.AreEqual (1, clientTransactionMock.NumberOfCallsToLoadDataContainer);
    Assert.AreEqual (0, clientTransactionMock.NumberOfCallsToLoadRelatedObject);

    Assert.IsNull (clientTransactionMock.GetRelatedObject (
        new RelationEndPoint (classWithGuidKey, "ClassWithValidRelationsOptional")));

    Assert.AreEqual (1, clientTransactionMock.NumberOfCallsToLoadDataContainer);
    Assert.AreEqual (1, clientTransactionMock.NumberOfCallsToLoadRelatedObject);

    clientTransactionMock.GetRelatedObject (
        new RelationEndPoint (classWithGuidKey, "ClassWithValidRelationsOptional"));

    Assert.AreEqual (1, clientTransactionMock.NumberOfCallsToLoadDataContainer);
    Assert.AreEqual (1, clientTransactionMock.NumberOfCallsToLoadRelatedObject);
  }

  [Test]
  public void GetRelatedObjectWithInheritance ()
  {
    DomainObject expectedCeo = _clientTransactionMock.GetObject (DomainObjectIDs.Ceo6);
    DomainObject partner = _clientTransactionMock.GetObject (DomainObjectIDs.Partner1);

    DomainObject actualCeo = _clientTransactionMock.GetRelatedObject (new RelationEndPoint (partner, "Ceo"));
    Assert.AreSame (expectedCeo, actualCeo);
  }

  [Test]
  public void GetRelatedObjects ()
  {
    Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
    _eventReceiver.Clear ();

    DomainObjectCollection orders = _clientTransactionMock.GetRelatedObjects (
        new RelationEndPoint (customer, "Orders"));

    Assert.IsNotNull (orders);
    Assert.AreEqual (typeof (OrderCollection), orders.GetType (), "Type of collection");
    Assert.AreEqual (2, orders.Count);
    Assert.AreEqual (2, _eventReceiver.LoadedDomainObjects.Count);
  }

  [Test]
  public void GetRelatedObjectsTwice ()
  {
    Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
    _eventReceiver.Clear ();

    DomainObjectCollection orders1 = _clientTransactionMock.GetRelatedObjects (
        new RelationEndPoint (customer, "Orders"));

    DomainObjectCollection orders2 = _clientTransactionMock.GetRelatedObjects (
        new RelationEndPoint (customer, "Orders"));
    
    Assert.IsTrue (object.ReferenceEquals (orders1, orders2));
    Assert.AreEqual (2, _eventReceiver.LoadedDomainObjects.Count);
  }

  [Test]
  public void GetRelatedObjectsWithAlreadyLoadedObject ()
  {
    Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
    Order order = Order.GetObject (DomainObjectIDs.Order1);
    _eventReceiver.Clear ();

    DomainObjectCollection orders = _clientTransactionMock.GetRelatedObjects (
        new RelationEndPoint (customer, "Orders"));
    
    Assert.AreSame  (order, orders[DomainObjectIDs.Order1]);
    Assert.AreEqual (1, _eventReceiver.LoadedDomainObjects.Count);
  }

  [Test]
  public void GetRelatedObjectsWithObjectOverSingleObjectRelationLink ()
  {
    Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

    DomainObjectCollection orders = _clientTransactionMock.GetRelatedObjects (
        new RelationEndPoint (customer, "Orders"));

    Order order = Order.GetObject (DomainObjectIDs.Order1);

    Assert.IsTrue (object.ReferenceEquals (order, orders[DomainObjectIDs.Order1]));
  }

  [Test]
  public void GetRelatedObjectsWithObjectOverMultipleObjectRelationLink ()
  {
    Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

    DomainObjectCollection orders = _clientTransactionMock.GetRelatedObjects (
        new RelationEndPoint (customer, "Orders"));

    Assert.AreSame (customer, _clientTransactionMock.GetRelatedObject (
        new RelationEndPoint (orders[0], "Customer")));
  }

  [Test]
  public void GetRelatedObjectsWithInheritance ()
  {
    DomainObject industrialSector = _clientTransactionMock.GetObject (DomainObjectIDs.IndustrialSector2);
    DomainObject expectedPartner = _clientTransactionMock.GetObject (DomainObjectIDs.Partner2);

    DomainObjectCollection companies = _clientTransactionMock.GetRelatedObjects (
        new RelationEndPoint (industrialSector, "Companies"));

    Assert.AreSame (expectedPartner, companies[DomainObjectIDs.Partner2]);
  }

  [Test]
  [ExpectedException (typeof (MappingException))]
  public void SetRelatedObjectWithInvalidType ()
  {
    DomainObject order = _clientTransactionMock.GetObject (DomainObjectIDs.Order1);
    DomainObject customer = _clientTransactionMock.GetObject (DomainObjectIDs.Customer1);

    _clientTransactionMock.SetRelatedObject (new RelationEndPoint (order, "OrderTicket"), customer);
  }


  [Test]
  [ExpectedException (typeof (MappingException))]
  public void SetRelatedObjectWithBaseType ()
  {
    DomainObject person = _clientTransactionMock.GetObject (DomainObjectIDs.Person1);
    DomainObject company = _clientTransactionMock.GetObject (DomainObjectIDs.Company1);

    _clientTransactionMock.SetRelatedObject (new RelationEndPoint (person, "AssociatedPartnerCompany"), company);
  }

  [Test]
  [ExpectedException (typeof (MandatoryRelationNotSetException),
      "Mandatory relation property 'Order' of domain object"
      + " 'TestDomain|OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid' cannot be null.")]
  public void CommitWithMandatoryOneToOneRelationNotSet ()
  {
    Order order = Order.GetObject (DomainObjectIDs.Order1);
    OrderTicket newOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);

    order.OrderTicket = newOrderTicket;

    ClientTransaction.Current.Commit ();
  }

  [Test]
  public void CommitWithOptionalOneToOneRelationNotSet ()
  {
    Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
    employee.Computer = null;

    ClientTransaction.Current.Commit ();

    // expectation: no exception
  }

  [Test]
  [ExpectedException (typeof (MandatoryRelationNotSetException),
     "Mandatory relation property 'Companies' of domain object"
     + " 'TestDomain|IndustrialSector|8565a077-ea01-4b5d-beaa-293dc484bddc|System.Guid' contains no elements.")]
  public void CommitWithMandatoryOneToManyRelationNotSet ()
  {
    IndustrialSector industrialSector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector2);
    industrialSector.Companies.Clear ();

    ClientTransaction.Current.Commit ();
  }

  [Test]
  public void CommittedEvent ()
  {
    Order order = Order.GetObject (DomainObjectIDs.Order1);
    OrderTicket oldOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
    OrderTicket newOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);
    Order oldOrderOfNewOrderTicket = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);

    oldOrderTicket.Order = newOrderTicket.Order;
    order.OrderTicket = newOrderTicket;

    ClientTransactionEventReceiver eventReceiver = new ClientTransactionEventReceiver (_clientTransactionMock);
    ClientTransaction.Current.Commit ();    

    Assert.IsNotNull (eventReceiver.CommittedDomainObjects);
    Assert.IsTrue (eventReceiver.CommittedDomainObjects.IsReadOnly);
    Assert.AreEqual (4, eventReceiver.CommittedDomainObjects.Count);
    Assert.IsNotNull (eventReceiver.CommittedDomainObjects[order.ID]);
    Assert.IsNotNull (eventReceiver.CommittedDomainObjects[oldOrderTicket.ID]);
    Assert.IsNotNull (eventReceiver.CommittedDomainObjects[newOrderTicket.ID]);
    Assert.IsNotNull (eventReceiver.CommittedDomainObjects[oldOrderOfNewOrderTicket.ID]);
  }

  [Test]
  public void CommitTwice ()
  {
    Order order = Order.GetObject (DomainObjectIDs.Order1);
    OrderTicket oldOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
    OrderTicket newOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);

    oldOrderTicket.Order = newOrderTicket.Order;
    order.OrderTicket = newOrderTicket;
    
    ClientTransaction.Current.Commit ();    

    object orderTimestamp = order.DataContainer.Timestamp;
    object oldOrderTicketTimestamp = oldOrderTicket.DataContainer.Timestamp;
    object newOrderTicketTimestamp = newOrderTicket.DataContainer.Timestamp;

    ClientTransaction.Current.Commit ();    

    Assert.AreEqual (orderTimestamp, order.DataContainer.Timestamp);
    Assert.AreEqual (oldOrderTicketTimestamp, oldOrderTicket.DataContainer.Timestamp);
    Assert.AreEqual (newOrderTicketTimestamp, newOrderTicket.DataContainer.Timestamp);
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
    
    ClientTransaction.Current.Commit ();    

    object orderTimestamp = order.DataContainer.Timestamp;
    object oldOrderTicketTimestamp = oldOrderTicket.DataContainer.Timestamp;
    object newOrderTicketTimestamp = newOrderTicket.DataContainer.Timestamp;
    object oldOrderOfNewOrderTicketTimestamp = oldOrderOfNewOrderTicket.DataContainer.Timestamp;
 
    order.OrderTicket = oldOrderTicket;
    oldOrderOfNewOrderTicket.OrderTicket = newOrderTicket;

    ClientTransaction.Current.Commit ();

    Assert.AreEqual (orderTimestamp, order.DataContainer.Timestamp);
    Assert.IsFalse (oldOrderTicketTimestamp.Equals (oldOrderTicket.DataContainer.Timestamp));
    Assert.IsFalse (newOrderTicketTimestamp.Equals (newOrderTicket.DataContainer.Timestamp));
    Assert.AreEqual (oldOrderOfNewOrderTicketTimestamp, oldOrderOfNewOrderTicket.DataContainer.Timestamp);
  }
}
}
