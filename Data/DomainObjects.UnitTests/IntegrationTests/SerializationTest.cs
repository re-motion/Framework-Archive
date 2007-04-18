using System;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.Transaction;

namespace Rubicon.Data.DomainObjects.UnitTests.IntegrationTests
{
  [TestFixture]
  public class SerializationTest : SerializationBaseTest
  {
    [Test]
    public void ObjectsFromPartnerClassDefinition ()
    {
      ClassDefinition companyClassDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Company");
      ClassDefinition supplierClassDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Supplier");
      ClassDefinition partnerClassDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Partner");
      PropertyDefinition partnerContactPersonPropertyDefinition = partnerClassDefinition.GetMandatoryPropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson");
      RelationDefinition partnerToPersonRelationDefinition = partnerClassDefinition.GetMandatoryRelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson");

      object[] partnerObjects = new object[] {
        partnerClassDefinition,
        partnerClassDefinition.BaseClass,
        partnerClassDefinition.DerivedClasses,
        partnerClassDefinition.MyPropertyDefinitions,
        partnerClassDefinition.MyRelationDefinitions,
        supplierClassDefinition,
        partnerContactPersonPropertyDefinition,
        partnerToPersonRelationDefinition,
        partnerToPersonRelationDefinition.EndPointDefinitions
    };

      object[] deserializedPartnerObjects = (object[]) SerializeAndDeserialize (partnerObjects);

      Assert.AreEqual (partnerObjects.Length, deserializedPartnerObjects.Length);
      Assert.AreSame (partnerClassDefinition, deserializedPartnerObjects[0]);
      Assert.AreSame (companyClassDefinition, deserializedPartnerObjects[1]);

      ClassDefinitionCollection deserializedDerivedClasses = (ClassDefinitionCollection) deserializedPartnerObjects[2];
      Assert.IsFalse (object.ReferenceEquals (partnerClassDefinition.DerivedClasses, deserializedDerivedClasses));
      Assert.AreEqual (partnerClassDefinition.DerivedClasses.Count, deserializedDerivedClasses.Count);
      for (int i = 0; i < partnerClassDefinition.DerivedClasses.Count; i++)
        Assert.AreSame (partnerClassDefinition.DerivedClasses[i], deserializedDerivedClasses[i]);

      PropertyDefinitionCollection deserializedPropertyDefinitions = (PropertyDefinitionCollection) deserializedPartnerObjects[3];
      Assert.IsFalse (object.ReferenceEquals (partnerClassDefinition.MyPropertyDefinitions, deserializedPropertyDefinitions));
      Assert.AreEqual (partnerClassDefinition.MyPropertyDefinitions.Count, deserializedPropertyDefinitions.Count);
      for (int i = 0; i < partnerClassDefinition.MyPropertyDefinitions.Count; i++)
        Assert.AreSame (partnerClassDefinition.MyPropertyDefinitions[i], deserializedPropertyDefinitions[i]);

      RelationDefinitionCollection deserializedRelationDefinitions = (RelationDefinitionCollection) deserializedPartnerObjects[4];
      Assert.IsFalse (object.ReferenceEquals (partnerClassDefinition.MyRelationDefinitions, deserializedRelationDefinitions));
      Assert.AreEqual (partnerClassDefinition.MyRelationDefinitions.Count, deserializedRelationDefinitions.Count);
      for (int i = 0; i < partnerClassDefinition.MyRelationDefinitions.Count; i++)
        Assert.AreSame (partnerClassDefinition.MyRelationDefinitions[i], deserializedRelationDefinitions[i]);

      Assert.AreSame (supplierClassDefinition, deserializedPartnerObjects[5]);

      Assert.AreSame (partnerContactPersonPropertyDefinition, deserializedPartnerObjects[6]);

      Assert.AreSame (partnerToPersonRelationDefinition, deserializedPartnerObjects[7]);

      IRelationEndPointDefinition[] deserializedRelationEndPoints = (IRelationEndPointDefinition[]) deserializedPartnerObjects[8];
      Assert.IsFalse (object.ReferenceEquals (partnerToPersonRelationDefinition.EndPointDefinitions, deserializedRelationEndPoints));
      Assert.AreEqual (partnerToPersonRelationDefinition.EndPointDefinitions.Length, deserializedRelationEndPoints.Length);
      for (int i = 0; i < partnerToPersonRelationDefinition.EndPointDefinitions.Length; i++)
        Assert.AreSame (partnerToPersonRelationDefinition.EndPointDefinitions[i], deserializedRelationEndPoints[i]);
    }

    [Test]
    public void RelationDefinitionsFromClientClassDefinition ()
    {
      ClassDefinition clientClassDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Client");
      RelationDefinition parentClientToChildClientRelationDefinition = clientClassDefinition.GetMandatoryRelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Client.ParentClient");
      ClassDefinition locationClassDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Location");
      RelationDefinition clientToLocationRelationDefinition = locationClassDefinition.GetMandatoryRelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Location.Client");

      object[] clientObjects = new object[] {
        clientClassDefinition,
        clientClassDefinition.DerivedClasses,
        clientClassDefinition.MyRelationDefinitions,
        parentClientToChildClientRelationDefinition,
        parentClientToChildClientRelationDefinition.EndPointDefinitions[0],
        parentClientToChildClientRelationDefinition.EndPointDefinitions[1],
        clientToLocationRelationDefinition.EndPointDefinitions[0],
        clientToLocationRelationDefinition.EndPointDefinitions[1],
    };

      object[] deserializedClientObjects = (object[]) SerializeAndDeserialize (clientObjects);

      Assert.AreEqual (clientObjects.Length, deserializedClientObjects.Length);
      Assert.AreSame (clientClassDefinition, deserializedClientObjects[0]);

      ClassDefinitionCollection deserializedDerivedClasses = (ClassDefinitionCollection) deserializedClientObjects[1];
      Assert.IsFalse (object.ReferenceEquals (clientClassDefinition.DerivedClasses, deserializedDerivedClasses));
      Assert.AreEqual (0, deserializedDerivedClasses.Count);

      RelationDefinitionCollection deserializedRelationDefinitions = (RelationDefinitionCollection) deserializedClientObjects[2];
      Assert.IsFalse (object.ReferenceEquals (clientClassDefinition.MyRelationDefinitions, deserializedRelationDefinitions));
      Assert.AreEqual (clientClassDefinition.MyRelationDefinitions.Count, deserializedRelationDefinitions.Count);
      for (int i = 0; i < clientClassDefinition.MyRelationDefinitions.Count; i++)
        Assert.AreSame (clientClassDefinition.MyRelationDefinitions[i], deserializedRelationDefinitions[i]);

      RelationDefinition deserializedParentClientToChildClientRelationDefinition = (RelationDefinition) deserializedClientObjects[3];
      Assert.AreSame (parentClientToChildClientRelationDefinition, deserializedParentClientToChildClientRelationDefinition);

      IRelationEndPointDefinition deserializedParentClientToChildClientFirstEndPoint = (IRelationEndPointDefinition) deserializedClientObjects[4];
      Assert.AreSame (parentClientToChildClientRelationDefinition.EndPointDefinitions[0], deserializedParentClientToChildClientFirstEndPoint);

      IRelationEndPointDefinition deserializedParentClientToChildClientSecondEndPoint = (IRelationEndPointDefinition) deserializedClientObjects[5];
      Assert.AreSame (parentClientToChildClientRelationDefinition.EndPointDefinitions[1], deserializedParentClientToChildClientSecondEndPoint);

      IRelationEndPointDefinition deserializedClientToLocationFirstEndPoint = (IRelationEndPointDefinition) deserializedClientObjects[6];
      Assert.AreSame (clientToLocationRelationDefinition.EndPointDefinitions[0], deserializedClientToLocationFirstEndPoint);

      IRelationEndPointDefinition deserializedClientToLocationSecondEndPoint = (IRelationEndPointDefinition) deserializedClientObjects[7];
      Assert.AreSame (clientToLocationRelationDefinition.EndPointDefinitions[1], deserializedClientToLocationSecondEndPoint);
    }

    [Test]
    public void Extensions ()
    {
      ClientTransactionExtensionWithQueryFiltering extension = new ClientTransactionExtensionWithQueryFiltering ();
      ClientTransaction.Current.Extensions.Add ("Name", extension);

      ClientTransaction deserializedClientTransaction = (ClientTransaction) SerializeAndDeserialize (ClientTransaction.Current);

      Assert.IsNotNull (deserializedClientTransaction);
      Assert.IsNotNull (deserializedClientTransaction.Extensions);
      Assert.AreEqual (1, deserializedClientTransaction.Extensions.Count);
      Assert.IsInstanceOfType (typeof (ClientTransactionExtensionWithQueryFiltering), deserializedClientTransaction.Extensions[0]);
    }


    [Test]
    [Ignore ("TODO: FS: Proxies are not serializable.")]
    public void EventsAfterDeserializationWithRegisteredEvents ()
    {
      Customer newCustomer1 = Customer.NewObject ();
      newCustomer1.Name = "NewCustomer1";

      Customer newCustomer2 = Customer.NewObject ();
      newCustomer2.Name = "NewCustomer2";

      Official official2 = Official.GetObject (DomainObjectIDs.Official2);
      Ceo newCeo1 = Ceo.NewObject ();
      Ceo newCeo2 = Ceo.NewObject ();
      Order newOrder1 = Order.NewObject ();
      newOrder1.DeliveryDate = new DateTime (2006, 1, 1);

      Order newOrder2 = Order.NewObject ();
      newOrder2.DeliveryDate = new DateTime (2006, 2, 2);

      OrderItem newOrderItem1 = OrderItem.NewObject ();
      OrderItem newOrderItem2 = OrderItem.NewObject ();

      DomainObject[] domainObjects = new DomainObject[] 
    { 
      newCustomer1, 
      newCustomer2, 
      official2, 
      newCeo1, 
      newCeo2, 
      newOrder1,  
      newOrder2, 
      newOrderItem1, 
      newOrderItem2 
    };

      DomainObjectCollection[] collections = new DomainObjectCollection[] 
    { 
      newCustomer1.Orders, 
      newCustomer2.Orders, 
      official2.Orders, 
      newOrder1.OrderItems,
      newOrder2.OrderItems 
    };

      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (domainObjects, collections);

      object[] deserializedObjects = (object[]) SerializeAndDeserialize (new object[] { domainObjects, collections, ClientTransaction.Current, eventReceiver });

      Assert.AreEqual (4, deserializedObjects.Length);

      DomainObject[] deserializedDomainObjects = (DomainObject[]) deserializedObjects[0];
      DomainObjectCollection[] deserializedCollections = (DomainObjectCollection[]) deserializedObjects[1];
      ClientTransaction deserializedClientTransaction = (ClientTransaction) deserializedObjects[2];
      ClientTransaction.SetCurrent (deserializedClientTransaction);
      SequenceEventReceiver deserializedEventReceiver = (SequenceEventReceiver) deserializedObjects[3];

      Assert.AreEqual (9, deserializedDomainObjects.Length);
      Assert.AreEqual (5, deserializedCollections.Length);

      Customer desNewCustomer1 = (Customer) deserializedDomainObjects[0];
      Customer desNewCustomer2 = (Customer) deserializedDomainObjects[1];
      Official desOfficial2 = (Official) deserializedDomainObjects[2];
      Ceo desNewCeo1 = (Ceo) deserializedDomainObjects[3];
      Ceo desNewCeo2 = (Ceo) deserializedDomainObjects[4];
      Order desNewOrder1 = (Order) deserializedDomainObjects[5];
      Order desNewOrder2 = (Order) deserializedDomainObjects[6];
      OrderItem desNewOrderItem1 = (OrderItem) deserializedDomainObjects[7];
      OrderItem desNewOrderItem2 = (OrderItem) deserializedDomainObjects[8];

      //1
      desNewCeo1.Company = desNewCustomer1;
      //2
      desNewCeo2.Company = desNewCustomer1;
      //3
      desNewCeo1.Company = desNewCustomer2;
      //4
      desNewCeo1.Company = null;

      //5
      desNewCustomer1.Orders.Add (desNewOrder1);
      //6
      desNewCustomer1.Orders.Add (desNewOrder2);
      //7
      desNewCustomer1.Orders.Remove (desNewOrder2);

      //8
      desNewOrderItem1.Order = desNewOrder1;
      //9
      desNewOrderItem2.Order = desNewOrder1;
      //10
      desNewOrderItem1.Order = null;
      //11
      desNewOrderItem1.Order = desNewOrder2;

      //12
      desNewOrder1.Official = desOfficial2;

      //13
      OrderTicket desNewOrderTicket1 = OrderTicket.NewObject (desNewOrder1);

      ChangeState[] expectedChangeStates = new ChangeState[]
    { 
      new RelationChangeState (desNewCeo1, "Company", null, desNewCustomer1, "1: 1. Changing event of newCeo from null to newCustomer1"),
      new RelationChangeState (desNewCustomer1, "Ceo", null, desNewCeo1, "1: 2. Changing event of newCustomer1 from null to newCeo1"),
      new RelationChangeState (desNewCeo1, "Company", null, null, "1: 3. Changed event of newCeo from null to newCustomer1"),
      new RelationChangeState (desNewCustomer1, "Ceo", null, null, "1: 4. Changed event of newCustomer1 from null to newCeo1"),

      new RelationChangeState (desNewCeo2, "Company", null, desNewCustomer1, "2: 1. Changing event of newCeo2 from null to newCustomer1"),
      new RelationChangeState (desNewCustomer1, "Ceo", desNewCeo1, desNewCeo2, "2: 2. Changing event of newCustomer1 from newCeo1 to newCeo2"),
      new RelationChangeState (desNewCeo1, "Company", desNewCustomer1, null, "2: 3. Changing event of newCeo1 from newCustomer1 to null"),
      new RelationChangeState (desNewCeo2, "Company", null, null, "2: 4. Changed event of newCeo2 from null to newCustomer1"),
      new RelationChangeState (desNewCustomer1, "Ceo", null, null, "2: 5. Changed event of newCustomer1 from newCeo1 to newCeo2"),
      new RelationChangeState (desNewCeo1, "Company", null, null, "2: 6. Changed event of newCeo1 from newCustomer1 to null"),

      new RelationChangeState (desNewCeo1, "Company", null, desNewCustomer2, "3: 1. Changing event of newCeo from null to newCustomer1"),
      new RelationChangeState (desNewCustomer2, "Ceo", null, desNewCeo1, "3: 2. Changing event of newCustomer2 from null to newCeo1"),
      new RelationChangeState (desNewCeo1, "Company", null, null, "3: 3. Changed event of newCeo from null to newCustomer1"),
      new RelationChangeState (desNewCustomer2, "Ceo", null, null, "3: 4. Changed event of newCustomer2 from null to newCeo1"),

      new RelationChangeState (desNewCeo1, "Company", desNewCustomer2, null, "4: 1. Changing event of newCeo from newCustomer1 to null"),
      new RelationChangeState (desNewCustomer2, "Ceo", desNewCeo1, null, "4: 2. Changing event of newCustomer2 from newCeo1 to null"),
      new RelationChangeState (desNewCeo1, "Company", null, null, "4: 3. Changed event of newCeo from newCustomer1 to null"),
      new RelationChangeState (desNewCustomer2, "Ceo", null, null, "4: 4. Changed event of newCustomer2 from newCeo1 to null"),

      new RelationChangeState (desNewOrder1, "Customer", null, desNewCustomer1, "5: 1. Changing event of newOrder1 from null to newCustomer1"),
      new CollectionChangeState (desNewCustomer1.Orders, desNewOrder1, "5: 2. Adding of newOrder1 to newCustomer1"),
      new RelationChangeState (desNewCustomer1, "Orders", null, desNewOrder1, "5: 3. Changing event of newCustomer1 from null to newOrder1"),
      new RelationChangeState (desNewOrder1, "Customer", null, null, "5: 4. Changed event of newOrder1 from null to newCustomer1"),
      new CollectionChangeState (desNewCustomer1.Orders, desNewOrder1, "5: 5. Added of newOrder1 to newCustomer1"),
      new RelationChangeState (desNewCustomer1, "Orders", null, null, "5: 6. Changed event of newCustomer1 from null to newOrder1"),

      new RelationChangeState (desNewOrder2, "Customer", null, desNewCustomer1, "6: 1. Changing event of newOrder2 from null to newCustomer1"),
      new CollectionChangeState (desNewCustomer1.Orders, desNewOrder2, "6: 2. Adding of newOrder2 to newCustomer1"),
      new RelationChangeState (desNewCustomer1, "Orders", null, desNewOrder2, "6: 3. Changing event of newCustomer1 from null to newOrder2"),
      new RelationChangeState (desNewOrder2, "Customer", null, null, "6: 4. Changed event of newOrder2 from null to newCustomer1"),
      new CollectionChangeState (desNewCustomer1.Orders, desNewOrder2, "6: 5. Added of newOrder2 to newCustomer1"),
      new RelationChangeState (desNewCustomer1, "Orders", null, null, "6: 6. Changed event of newCustomer1 from null to newOrder2"),

      new RelationChangeState (desNewOrder2, "Customer", desNewCustomer1, null, "7: 1. Changing event of newOrder2 from newCustomer1 to null"),
      new CollectionChangeState (desNewCustomer1.Orders, desNewOrder2, "7: 2. Removing of newOrder2 from newCustomer1"),
      new RelationChangeState (desNewCustomer1, "Orders", desNewOrder2, null, "7: 3. Changing event of newCustomer1 from newOrder2 to null"),
      new RelationChangeState (desNewOrder2, "Customer", null, null, "7: 4. Changed event of newOrder2 from newCustomer1 to null"),
      new CollectionChangeState (desNewCustomer1.Orders, desNewOrder2, "7: 5. Removed of newOrder2 from newCustomer1"),
      new RelationChangeState (desNewCustomer1, "Orders", null, null, "7: 6. Changed event of newCustomer1 from newOrder2 to null"),

      new RelationChangeState (desNewOrderItem1, "Order", null, desNewOrder1, "8: 1. Changing event of newOrderItem1 from null to newOrder1"),
      new CollectionChangeState (desNewOrder1.OrderItems, desNewOrderItem1, "8: 2. Adding of newOrderItem1 to newOrder1"),
      new RelationChangeState (desNewOrder1, "OrderItems", null, desNewOrderItem1, "8: 3. Changing event of newOrder1 from null to newOrderItem1"),
      new RelationChangeState (desNewOrderItem1, "Order", null, null, "8: 4. Changed event of newOrderItem1 from null to newOrder1"),
      new CollectionChangeState (desNewOrder1.OrderItems, desNewOrderItem1, "8: 5. Added of newOrderItem1 to newOrder1"),
      new RelationChangeState (desNewOrder1, "OrderItems", null, null, "8: 6. Changed event of newOrder1 from null to newOrderItem1"),

      new RelationChangeState (desNewOrderItem2, "Order", null, desNewOrder1, "9: 1. Changing event of newOrderItem2 from null to newOrder1"),
      new CollectionChangeState (desNewOrder1.OrderItems, desNewOrderItem2, "9: 2. Adding of newOrderItem2 to newOrder1"),
      new RelationChangeState (desNewOrder1, "OrderItems", null, desNewOrderItem2, "9: 3. Changing event of newOrder1 from null to newOrderItem2"),
      new RelationChangeState (desNewOrderItem2, "Order", null, null, "9: 4. Changed event of newOrderItem2 from null to newOrder1"),
      new CollectionChangeState (desNewOrder1.OrderItems, desNewOrderItem2, "9: 5. Added of newOrderItem2 to newOrder1"),
      new RelationChangeState (desNewOrder1, "OrderItems", null, null, "9: 6. Changed event of newOrder1 from null to newOrderItem2"),

      new RelationChangeState (desNewOrderItem1, "Order", desNewOrder1, null, "10: 1. Changing event of newOrderItem1 from newOrder1 to null"),
      new CollectionChangeState (desNewOrder1.OrderItems, desNewOrderItem1, "10: 2. Removing of newOrderItem1 from newOrder1"),
      new RelationChangeState (desNewOrder1, "OrderItems", desNewOrderItem1, null, "10: 3. Changing event of newOrder1 from newOrderItem1 to null"),
      new RelationChangeState (desNewOrderItem1, "Order", null, null, "10: 4. Changed event of newOrderItem2 from newOrder1 to null"),
      new CollectionChangeState (desNewOrder1.OrderItems, desNewOrderItem1, "10: 5. Removed of newOrderItem1 from newOrder1"),
      new RelationChangeState (desNewOrder1, "OrderItems", null, null, "10: 6. Changed event of newOrder1 from newOrderItem1 to null"),

      new RelationChangeState (desNewOrderItem1, "Order", null, desNewOrder2, "11: 1. Changing event of newOrderItem1 from null to newOrder2"),
      new CollectionChangeState (desNewOrder2.OrderItems, desNewOrderItem1, "11: 2. Adding of newOrderItem1 to newOrder2"),
      new RelationChangeState (desNewOrder2, "OrderItems", null, desNewOrderItem1, "11: 3. Changing event of newOrder2 from null to newOrderItem1"),
      new RelationChangeState (desNewOrderItem1, "Order", null, null, "11: 4. Changed event of newOrderItem2 from null to newOrder2"),
      new CollectionChangeState (desNewOrder2.OrderItems, desNewOrderItem1, "11: 5. Adding of newOrderItem1 to newOrder2"),
      new RelationChangeState (desNewOrder2, "OrderItems", null, null, "11: 6. Changed event of newOrder2 from null to newOrderItem1"),

      new RelationChangeState (desNewOrder1, "Official", null, desOfficial2, "12: 1. Changing event of newOrder1 from null to official2"),
      new CollectionChangeState (desOfficial2.Orders, desNewOrder1, "12: 2. Adding of newOrder1 to official2"),
      new RelationChangeState (desOfficial2, "Orders", null, desNewOrder1, "12: 3. Changing event of official2 from null to newOrder1"),
      new RelationChangeState (desNewOrder1, "Official", null, null, "12: 4. Changed event of newOrder1 from null to official2"),
      new CollectionChangeState (desOfficial2.Orders, desNewOrder1, "12: 5. Adding of newOrder1 to official2"),
      new RelationChangeState (desOfficial2, "Orders", null, null, "12: 6. Changed event of official2 from null to newOrder1"),

      new RelationChangeState (desNewOrder1, "OrderTicket", null, desNewOrderTicket1, "13: 1. Changing event of newOrder1 from null to newOrderTicket1"),
      new RelationChangeState (desNewOrder1, "OrderTicket", null, null, "13: 2. Changed event of newOrder1 from null to newOrderTicket1")
    };

      deserializedEventReceiver.Check (expectedChangeStates);
      deserializedEventReceiver.Unregister ();

      eventReceiver = new SequenceEventReceiver (
          new DomainObject[] { desNewCustomer1, desNewOrderTicket1, desNewOrder2, desNewOrder1, desNewOrderItem1 },
          new DomainObjectCollection[] { desNewOrder2.OrderItems, desNewCustomer1.Orders });

      //14
      desNewOrderTicket1.Order = desNewOrder2;


      expectedChangeStates = new ChangeState[]
    { 
      new RelationChangeState (desNewOrderTicket1, "Order", desNewOrder1, desNewOrder2, "14: 1. Changing event of newOrderTicket1 from newOrder1 to newOrder2"),
      new RelationChangeState (desNewOrder1, "OrderTicket", desNewOrderTicket1, null, "14: 2. Changing event of newOrder1 from newOrderTicket1 to null"),
      new RelationChangeState (desNewOrder2, "OrderTicket", null, desNewOrderTicket1, "14: 3. Changing event of newOrder1 from null to newOrderTicket1"),
      new RelationChangeState (desNewOrderTicket1, "Order", null, null, "14: 4. Changed event of newOrderTicket1 from newOrder1 to newOrder2"),
      new RelationChangeState (desNewOrder1, "OrderTicket", null, null, "14: 5. Changed event of newOrder1 from newOrderTicket1 to null"),
      new RelationChangeState (desNewOrder2, "OrderTicket", null, null, "14: 6. Changed event of newOrder1 from null to newOrderTicket1"),
    };

      eventReceiver.Check (expectedChangeStates);
      eventReceiver.Unregister ();

      //15a
      eventReceiver = new SequenceEventReceiver (
          new DomainObject[] { desNewCustomer1, desNewOrderTicket1, desNewOrder2, desNewOrder1, desNewOrderItem1 },
          new DomainObjectCollection[] { desNewOrder2.OrderItems, desNewCustomer1.Orders });

      desNewOrder2.Customer = desNewCustomer1;

      expectedChangeStates = new ChangeState[]
    { 
      new RelationChangeState (desNewOrder2, "Customer", null, desNewCustomer1, "15a: 1. Changing event of newOrder2 from null to newCustomer1.Orders"),
      new CollectionChangeState (desNewCustomer1.Orders, desNewOrder2, "15a: 2. Adding of newOrder2 to newCustomer1"),
      new RelationChangeState (desNewCustomer1, "Orders", null, desNewOrder2, "15a: 3. Changing event of newCustomer1 from null to newOrder2"),
      new RelationChangeState (desNewOrder2, "Customer", null, null, "15a: 4. Changed event of newOrder2 from null to newCustomer1.Orders"),
      new CollectionChangeState (desNewCustomer1.Orders, desNewOrder2, "15a: 5. Added of newOrder2 to newCustomer1"),
      new RelationChangeState (desNewCustomer1, "Orders", null, null, "15a: 6. Changed event of newCustomer2 from null to newOrder2"),
    };

      eventReceiver.Check (expectedChangeStates);
      eventReceiver.Unregister ();

      //15b
      eventReceiver = new SequenceEventReceiver (
          new DomainObject[] { desNewCustomer1, desNewCustomer2, desNewOrderTicket1, desNewOrder2, desNewOrder1, desNewOrderItem1 },
          new DomainObjectCollection[] { desNewOrder2.OrderItems, desNewCustomer1.Orders, desNewCustomer2.Orders });

      desNewOrder2.Customer = desNewCustomer2;

      expectedChangeStates = new ChangeState[]
    { 
      new RelationChangeState (desNewOrder2, "Customer", desNewCustomer1, desNewCustomer2, "15b: 1. Changing event of newOrder2 from null to newCustomer2.Orders"),
      new CollectionChangeState (desNewCustomer2.Orders, desNewOrder2, "15b: 4. Adding of newOrder2 to newCustomer2"),
      new RelationChangeState (desNewCustomer2, "Orders", null, desNewOrder2, "15b: 5. Changing event of newCustomer2 from null to newOrder2"),
      new CollectionChangeState (desNewCustomer1.Orders, desNewOrder2, "15b: 2. Removing of newOrder2 from newCustomer1"),
      new RelationChangeState (desNewCustomer1, "Orders", desNewOrder2, null, "15b: 3. Changing event of newCustomer1 from newOrder2 to null"),
      new RelationChangeState (desNewOrder2, "Customer", null, null, "15b: 6. Changed event of newOrder2 from null to newCustomer2.Orders"),
      new CollectionChangeState (desNewCustomer2.Orders, desNewOrder2, "15b: 9. Added of newOrder2 to newCustomer2"),
      new RelationChangeState (desNewCustomer2, "Orders", null, null, "15b: 10. Changed event of newCustomer2 from null to newOrder2"),
      new CollectionChangeState (desNewCustomer1.Orders, desNewOrder2, "15b: 7. Removed of newOrder2 from newCustomer1"),
      new RelationChangeState (desNewCustomer1, "Orders", null, null, "15b: 8. Changed event of newCustomer1 from newOrder2 to null"),
    };

      eventReceiver.Check (expectedChangeStates);
      eventReceiver.Unregister ();

      //16
      eventReceiver = new SequenceEventReceiver (
          new DomainObject[] { desNewCustomer1, desNewCustomer2, desNewOrderTicket1, desNewOrder2, desNewOrder1, desNewOrderItem1 },
          new DomainObjectCollection[] { desNewOrder2.OrderItems, desNewCustomer1.Orders, desNewCustomer2.Orders });

      desNewOrder2.Delete ();

      expectedChangeStates = new ChangeState[]
    { 
      new ObjectDeletionState (desNewOrder2, "16: 1. Deleting event of newOrder2"),
      new CollectionChangeState (desNewCustomer2.Orders, desNewOrder2, "16: 2. Removing of newOrder2 from newCustomer2"),
      new RelationChangeState (desNewCustomer2, "Orders", desNewOrder2, null, "16: 3. Changing event of newCustomer2 from newOrder2 to null"),
      new RelationChangeState (desNewOrderTicket1, "Order", desNewOrder2, null, "16: 4. Changing event of newOrderTicket1 from newOrder2 to null"),
      new RelationChangeState (desNewOrderItem1, "Order", desNewOrder2, null, "16: 5. Changing event of newOrderItem1 from newOrder2 to null"),

      new CollectionChangeState (desNewCustomer2.Orders, desNewOrder2, "16: 7. Removed of newOrder2 from newCustomer2"),
      new RelationChangeState (desNewCustomer2, "Orders", null, null, "16: 8. Changed event of newCustomer2 from newOrder2 to null"),
      new RelationChangeState (desNewOrderTicket1, "Order", null, null, "16: 9. Changed event of newOrderTicket1 from newOrder2 to null"),
      new RelationChangeState (desNewOrderItem1, "Order", null, null, "16: 10. Changed event of newOrderItem1 from newOrder2 to null"),
      new ObjectDeletionState (desNewOrder2, "16: 6. Deleted event of newOrder2")
    };

      eventReceiver.Check (expectedChangeStates);
      eventReceiver.Unregister ();

      //17
      eventReceiver = new SequenceEventReceiver (
          new DomainObject[] { desNewCustomer1, desNewCustomer2, desNewOrderTicket1, desNewOrder1, desNewOrderItem1 },
          new DomainObjectCollection[] { desNewCustomer1.Orders, desNewCustomer2.Orders });

      desNewOrderTicket1.Order = desNewOrder1;

      expectedChangeStates = new ChangeState[]
    { 
      new RelationChangeState (desNewOrderTicket1, "Order", null, desNewOrder1, "17: 1. Changing event of newOrderTicket1 from null to newOrder1"),
      new RelationChangeState (desNewOrder1, "OrderTicket", null, desNewOrderTicket1, "17: 2. Changing event of newOrder1 from null to newOrderTicket1"),
      new RelationChangeState (desNewOrderTicket1, "Order", null, null, "17: 3. Changed event of newOrderTicket1 from null to newOrder1"),
      new RelationChangeState (desNewOrder1, "OrderTicket", null, null, "17: 4. Changed event of newOrder1 from null to newOrderTicket1"),
    };

      eventReceiver.Check (expectedChangeStates);
      eventReceiver.Unregister ();

      //cleanup for commit
      desNewCustomer2.Delete ();
      desNewCeo1.Delete ();
      desNewOrderItem1.Delete ();

      ClientTransaction.Current.Commit ();
    }

    [Test]
    [Ignore ("TODO: FS: Proxies are not serializable.")]
    public void BidirectionalRelationsIncludingHierarchyOfObjects ()
    {
      Employee employee1 = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee1);
      Employee employee2 = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee2);
      Employee employee3 = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee3);
      Employee employee4 = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee4);
      Employee employee5 = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee5);
      Employee employee6 = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee6);
      Employee employee7 = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee7);

      DomainObjectCollection employee1Subordinates = employee1.Subordinates;
      Employee employee1Supervisor = employee1.Supervisor;
      Computer employee1Computer = employee1.Computer;
      DomainObjectCollection employee2Subordinates = employee2.Subordinates;
      Employee employee2Supervisor = employee2.Supervisor;
      Computer employee2Computer = employee2.Computer;
      DomainObjectCollection employee3Subordinates = employee3.Subordinates;
      Employee employee3Supervisor = employee3.Supervisor;
      Computer employee3Computer = employee3.Computer;
      DomainObjectCollection employee4Subordinates = employee4.Subordinates;
      Employee employee4Supervisor = employee4.Supervisor;
      Computer employee4Computer = employee4.Computer;
      DomainObjectCollection employee5Subordinates = employee5.Subordinates;
      Employee employee5Supervisor = employee5.Supervisor;
      Computer employee5Computer = employee5.Computer;
      DomainObjectCollection employee6Subordinates = employee6.Subordinates;
      Employee employee6Supervisor = employee6.Supervisor;
      Computer employee6Computer = employee6.Computer;
      DomainObjectCollection employee7Subordinates = employee7.Subordinates;
      Employee employee7Supervisor = employee1.Supervisor;
      Computer employee7Computer = employee7.Computer;

      Employee[] employees = new Employee[] { employee1, employee2, employee3, employee4, employee5, employee6, employee7 };

      Employee[] deserializedEmployees = (Employee[]) SerializeAndDeserialize (employees);

      Employee deserializedEmployee1 = deserializedEmployees[0];
      Employee deserializedEmployee2 = deserializedEmployees[1];
      Employee deserializedEmployee3 = deserializedEmployees[2];
      Employee deserializedEmployee4 = deserializedEmployees[3];
      Employee deserializedEmployee5 = deserializedEmployees[4];
      Employee deserializedEmployee6 = deserializedEmployees[5];
      Employee deserializedEmployee7 = deserializedEmployees[6];

      DomainObjectCollection deserializedEmployee1Subordinates = deserializedEmployee1.Subordinates;
      Employee deserializedEmployee1Supervisor = deserializedEmployee1.Supervisor;
      Computer deserializedEmployee1Computer = deserializedEmployee1.Computer;
      DomainObjectCollection deserializedEmployee2Subordinates = deserializedEmployee2.Subordinates;
      Employee deserializedEmployee2Supervisor = deserializedEmployee2.Supervisor;
      Computer deserializedEmployee2Computer = deserializedEmployee2.Computer;
      DomainObjectCollection deserializedEmployee3Subordinates = deserializedEmployee3.Subordinates;
      Employee deserializedEmployee3Supervisor = deserializedEmployee3.Supervisor;
      Computer deserializedEmployee3Computer = deserializedEmployee3.Computer;
      DomainObjectCollection deserializedEmployee4Subordinates = deserializedEmployee4.Subordinates;
      Employee deserializedEmployee4Supervisor = deserializedEmployee4.Supervisor;
      Computer deserializedEmployee4Computer = deserializedEmployee4.Computer;
      DomainObjectCollection deserializedEmployee5Subordinates = deserializedEmployee5.Subordinates;
      Employee deserializedEmployee5Supervisor = deserializedEmployee5.Supervisor;
      Computer deserializedEmployee5Computer = deserializedEmployee5.Computer;
      DomainObjectCollection deserializedEmployee6Subordinates = deserializedEmployee6.Subordinates;
      Employee deserializedEmployee6Supervisor = deserializedEmployee6.Supervisor;
      Computer deserializedEmployee6Computer = deserializedEmployee6.Computer;
      DomainObjectCollection deserializedEmployee7Subordinates = deserializedEmployee7.Subordinates;
      Employee deserializedEmployee7Supervisor = deserializedEmployee1.Supervisor;
      Computer deserializedEmployee7Computer = deserializedEmployee7.Computer;

      Assert.AreEqual (employee1Subordinates.Count, deserializedEmployee1Subordinates.Count);
      AreEqual (employee1Supervisor, deserializedEmployee1Supervisor);
      AreEqual (employee1Computer, deserializedEmployee1Computer);
      Assert.AreEqual (employee2Subordinates.Count, deserializedEmployee2Subordinates.Count);
      AreEqual (employee2Supervisor, deserializedEmployee2Supervisor);
      AreEqual (employee2Computer, deserializedEmployee2Computer);
      Assert.AreEqual (employee3Subordinates.Count, deserializedEmployee3Subordinates.Count);
      AreEqual (employee3Supervisor, deserializedEmployee3Supervisor);
      AreEqual (employee3Computer, deserializedEmployee3Computer);
      Assert.AreEqual (employee4Subordinates.Count, deserializedEmployee4Subordinates.Count);
      AreEqual (employee4Supervisor, deserializedEmployee4Supervisor);
      AreEqual (employee4Computer, deserializedEmployee4Computer);
      Assert.AreEqual (employee5Subordinates.Count, deserializedEmployee5Subordinates.Count);
      AreEqual (employee5Supervisor, deserializedEmployee5Supervisor);
      AreEqual (employee5Computer, deserializedEmployee5Computer);
      Assert.AreEqual (employee6Subordinates.Count, deserializedEmployee6Subordinates.Count);
      AreEqual (employee6Supervisor, deserializedEmployee6Supervisor);
      AreEqual (employee6Computer, deserializedEmployee6Computer);
      Assert.AreEqual (employee7Subordinates.Count, deserializedEmployee7Subordinates.Count);
      AreEqual (employee7Supervisor, deserializedEmployee7Supervisor);
      AreEqual (employee7Computer, deserializedEmployee7Computer);
    }

    [Test]
    [Ignore ("TODO: FS: Proxies are not serializable.")]
    public void UnidirectionalRelation ()
    {
      Location location1 = DomainObject.GetObject<Location> (DomainObjectIDs.Location1);
      Client location1Client = location1.Client;
      Location location2 = DomainObject.GetObject<Location> (DomainObjectIDs.Location2);
      Client location2Client = location2.Client;
      Location location3 = DomainObject.GetObject<Location> (DomainObjectIDs.Location3);
      Client location3Client = location3.Client;

      Location[] locations = new Location[] { location1, location2, location3 };

      Location[] deserializedLocations = (Location[]) SerializeAndDeserialize (locations);

      Location deserializedLocation1 = deserializedLocations[0];
      Location deserializedLocation2 = deserializedLocations[1];
      Location deserializedLocation3 = deserializedLocations[2];

      Assert.AreEqual (location1.ID, deserializedLocation1.ID);
      AreEqual (location1.Client, deserializedLocation1.Client);
      Assert.AreEqual (location2.ID, deserializedLocation2.ID);
      AreEqual (location2.Client, deserializedLocation2.Client);
      Assert.AreEqual (location3.ID, deserializedLocation3.ID);
      AreEqual (location3.Client, deserializedLocation3.Client);
    }

    private void AreEqual (DomainObject expected, DomainObject actual)
    {
      if (expected == null && actual == null)
        return;
      if (expected == null || actual == null)
        Assert.Fail ("One reference is null.");

      Assert.AreEqual (expected.ID, actual.ID);
    }

    private void BackToRecord (MockRepository mockRepository, params object[] objects)
    {
      foreach (object obj in objects)
        mockRepository.BackToRecord (obj);
    }
  }
}
