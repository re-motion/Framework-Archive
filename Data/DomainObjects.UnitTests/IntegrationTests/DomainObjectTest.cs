using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

using Rubicon.Data.DomainObjects.UnitTests.EventSequence;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
[TestFixture]
public class IntegrationTest: ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public IntegrationTest ()
  {
  }

  // methods and properties

// TODO: Reactivate this test
//  [Test]
//  public void NewOfficialTest ()
//  {
//    new Official ();
//  }
//

// TODO: Reactivate this test
//  // The same column should not be referenced twice !!!
//  [Test]
//  public void AddPropertyValueWithExistingColumnTest ()
//  {
//    Employee employee = new Employee ();
//
//    PropertyDefinition propertyDefinition = new PropertyDefinition (
//      "Name1", "Name", typeof (string), true, 10);
//    PropertyValueCollection propertyValues = employee.DataContainer.PropertyValues;
//    PropertyValue propertyValue = new PropertyValue (propertyDefinition);
//
//    Assert.IsFalse (propertyValues.Contains ("Name1"));
//    Assert.AreEqual ("Name", propertyValues["Name"].Definition.ColumnName);
//  
//    propertyValues.Add (propertyValue);
//
//    // should be forbidden
//    Assert.Fail ();
//  }
//


// TODO: Reactivate this test
//  [Test]
//  public void DiscardSinglePropertyValueTest ()
//  {
//    Employee employee = new Employee ();
//
//    PropertyValue propertyValue = employee.DataContainer.PropertyValues["Name"];
//
//    //should not be valid
//    propertyValue.Discard ();
//
//    employee.Name = "newName";
//    //expectation: no exception
//  }

// TODO: Reactivate this test
//  [Test]
//  public void RelationEventTest ()
//  {
//    Customer newCustomer1 = new Customer ();
//    Customer newCustomer2 = new Customer ();
//    Official official2 = Official.GetObject (DomainObjectIDs.Official2);
//    Ceo newCeo1 = new Ceo ();
//    Ceo newCeo2 = new Ceo ();
//    Order newOrder1 = new Order ();
//    Order newOrder2 = new Order ();
//
//    OrderItem newOrderItem1 = new OrderItem ();
//    OrderItem newOrderItem2 = new OrderItem ();
//
//    SequenceEventReceiver eventReceiver = new SequenceEventReceiver (
//      new DomainObject[] { newCustomer1, newCustomer2, official2, newCeo1, newCeo2, newOrder1,  
//                            newOrder2, newOrderItem1, newOrderItem2 },
//      new DomainObjectCollection[] { newCustomer1.Orders, newCustomer2.Orders, official2.Orders, newOrder1.OrderItems, 
//                                      newOrder2.OrderItems } );
//    //1
//    newCeo1.Company = newCustomer1;
//    //2
//    newCeo2.Company = newCustomer1;
//    //3
//    newCeo1.Company = newCustomer2;
//    //4
//    newCeo1.Company = null;
//
//    //5
//    newCustomer1.Orders.Add (newOrder1);
//    //6
//    newCustomer1.Orders.Add (newOrder2);
//    //7
//    newCustomer1.Orders.Remove (newOrder2);
//    
//    //8
//    newOrderItem1.Order = newOrder1;
//    //9
//    newOrderItem2.Order = newOrder1;
//    //10
//    newOrderItem1.Order = null;
//    //11
//    newOrderItem1.Order = newOrder2;
//
//    //12
//    newOrder1.Official = official2;
//
//    //13
//    OrderTicket newOrderTicket1 = new OrderTicket (newOrder1);
//
//    ChangeState[] expectedChangeStates = new ChangeState[]
//    { 
//      new RelationChangeState (newCeo1, "Company", null, newCustomer1, "1: 1. Changing event of newCeo from null to newCustomer1"),
//      new RelationChangeState (newCustomer1, "Ceo", null, newCeo1, "1: 2. Changing event of newCustomer1 from null to newCeo1"),
//      new RelationChangeState (newCeo1, "Company", null, null, "1: 3. Changed event of newCeo from null to newCustomer1"),
//      new RelationChangeState (newCustomer1, "Ceo", null, null, "1: 4. Changed event of newCustomer1 from null to newCeo1"),
//
//      new RelationChangeState (newCeo2, "Company", null, newCustomer1, "2: 1. Changing event of newCeo2 from null to newCustomer1"),
//      new RelationChangeState (newCustomer1, "Ceo", newCeo1, newCeo2, "2: 2. Changing event of newCustomer1 from newCeo1 to newCeo2"),
//      new RelationChangeState (newCeo1, "Company", newCustomer1, null, "2: 3. Changing event of newCeo1 from newCustomer1 to null"),
//      new RelationChangeState (newCeo2, "Company", null, null, "2: 4. Changed event of newCeo2 from null to newCustomer1"),
//      new RelationChangeState (newCustomer1, "Ceo", null, null, "2: 5. Changed event of newCustomer1 from newCeo1 to newCeo2"),
//      new RelationChangeState (newCeo1, "Company", null, null, "2: 6. Changed event of newCeo1 from newCustomer1 to null"),
//
//      new RelationChangeState (newCeo1, "Company", null, newCustomer2, "3: 1. Changing event of newCeo from null to newCustomer1"),
//      new RelationChangeState (newCustomer2, "Ceo", null, newCeo1, "3: 2. Changing event of newCustomer2 from null to newCeo1"),
//      new RelationChangeState (newCeo1, "Company", null, null, "3: 3. Changed event of newCeo from null to newCustomer1"),
//      new RelationChangeState (newCustomer2, "Ceo", null, null, "3: 4. Changed event of newCustomer2 from null to newCeo1"),
//
//      new RelationChangeState (newCeo1, "Company", newCustomer2, null, "4: 1. Changing event of newCeo from newCustomer1 to null"),
//      new RelationChangeState (newCustomer2, "Ceo", newCeo1, null, "4: 2. Changing event of newCustomer2 from newCeo1 to null"),
//      new RelationChangeState (newCeo1, "Company", null, null, "4: 3. Changed event of newCeo from newCustomer1 to null"),
//      new RelationChangeState (newCustomer2, "Ceo", null, null, "4: 4. Changed event of newCustomer2 from newCeo1 to null"),
//
//      new RelationChangeState (newOrder1, "Customer", null, newCustomer1, "5: 1. Changing event of newOrder1 from null to newCustomer1"),
//      new CollectionChangeState (newCustomer1.Orders, newOrder1, "5: 2. Adding of newOrder1 to newCustomer1"),
//      new RelationChangeState (newCustomer1, "Orders", null, newOrder1, "5: 3. Changing event of newCustomer1 from null to newOrder1"),
//      new RelationChangeState (newOrder1, "Customer", null, null, "5: 4. Changed event of newOrder1 from null to newCustomer1"),
//      new CollectionChangeState (newCustomer1.Orders, newOrder1, "5: 5. Added of newOrder1 to newCustomer1"),
//      new RelationChangeState (newCustomer1, "Orders", null, null, "5: 6. Changed event of newCustomer1 from null to newOrder1"),
//
//      new RelationChangeState (newOrder2, "Customer", null, newCustomer1, "6: 1. Changing event of newOrder2 from null to newCustomer1"),
//      new CollectionChangeState (newCustomer1.Orders, newOrder2, "6: 2. Adding of newOrder2 to newCustomer1"),
//      new RelationChangeState (newCustomer1, "Orders", null, newOrder2, "6: 3. Changing event of newCustomer1 from null to newOrder2"),
//      new RelationChangeState (newOrder2, "Customer", null, null, "6: 4. Changed event of newOrder2 from null to newCustomer1"),
//      new CollectionChangeState (newCustomer1.Orders, newOrder2, "6: 5. Added of newOrder2 to newCustomer1"),
//      new RelationChangeState (newCustomer1, "Orders", null, null, "6: 6. Changed event of newCustomer1 from null to newOrder2"),
//
//      new RelationChangeState (newOrder2, "Customer", newCustomer1, null, "7: 1. Changing event of newOrder2 from newCustomer1 to null"),
//      new CollectionChangeState (newCustomer1.Orders, newOrder2, "7: 2. Removing of newOrder2 from newCustomer1"),
//      new RelationChangeState (newCustomer1, "Orders", newOrder2, null, "7: 3. Changing event of newCustomer1 from newOrder2 to null"),
//      new RelationChangeState (newOrder2, "Customer", null, null, "7: 4. Changed event of newOrder2 from newCustomer1 to null"),
//      new CollectionChangeState (newCustomer1.Orders, newOrder2, "7: 5. Removed of newOrder2 from newCustomer1"),
//      new RelationChangeState (newCustomer1, "Orders", null, null, "7: 6. Changed event of newCustomer1 from newOrder2 to null"),
//
//      new RelationChangeState (newOrderItem1, "Order", null, newOrder1, "8: 1. Changing event of newOrderItem1 from null to newOrder1"),
//      new CollectionChangeState (newOrder1.OrderItems, newOrderItem1, "8: 2. Adding of newOrderItem1 to newOrder1"),
//      new RelationChangeState (newOrder1, "OrderItems", null, newOrderItem1, "8: 3. Changing event of newOrder1 from null to newOrderItem1"),
//      new RelationChangeState (newOrderItem1, "Order", null, null, "8: 4. Changed event of newOrderItem1 from null to newOrder1"),
//      new CollectionChangeState (newOrder1.OrderItems, newOrderItem1, "8: 5. Added of newOrderItem1 to newOrder1"),
//      new RelationChangeState (newOrder1, "OrderItems", null, null, "8: 6. Changed event of newOrder1 from null to newOrderItem1"),
//
//      new RelationChangeState (newOrderItem2, "Order", null, newOrder1, "9: 1. Changing event of newOrderItem2 from null to newOrder1"),
//      new CollectionChangeState (newOrder1.OrderItems, newOrderItem2, "9: 2. Adding of newOrderItem2 to newOrder1"),
//      new RelationChangeState (newOrder1, "OrderItems", null, newOrderItem2, "9: 3. Changing event of newOrder1 from null to newOrderItem2"),
//      new RelationChangeState (newOrderItem2, "Order", null, null, "9: 4. Changed event of newOrderItem2 from null to newOrder1"),
//      new CollectionChangeState (newOrder1.OrderItems, newOrderItem2, "9: 5. Added of newOrderItem2 to newOrder1"),
//      new RelationChangeState (newOrder1, "OrderItems", null, null, "9: 6. Changed event of newOrder1 from null to newOrderItem2"),
//
//      new RelationChangeState (newOrderItem1, "Order", newOrder1, null, "10: 1. Changing event of newOrderItem1 from newOrder1 to null"),
//      new CollectionChangeState (newOrder1.OrderItems, newOrderItem1, "10: 2. Removing of newOrderItem1 from newOrder1"),
//      new RelationChangeState (newOrder1, "OrderItems", newOrderItem1, null, "10: 3. Changing event of newOrder1 from newOrderItem1 to null"),
//      new RelationChangeState (newOrderItem1, "Order", null, null, "10: 4. Changed event of newOrderItem2 from newOrder1 to null"),
//      new CollectionChangeState (newOrder1.OrderItems, newOrderItem1, "10: 5. Removed of newOrderItem1 from newOrder1"),
//      new RelationChangeState (newOrder1, "OrderItems", null, null, "10: 6. Changed event of newOrder1 from newOrderItem1 to null"),
//
//      new RelationChangeState (newOrderItem1, "Order", null, newOrder2, "11: 1. Changing event of newOrderItem1 from null to newOrder2"),
//      new CollectionChangeState (newOrder2.OrderItems, newOrderItem1, "11: 2. Adding of newOrderItem1 to newOrder2"),
//      new RelationChangeState (newOrder2, "OrderItems", null, newOrderItem1, "11: 3. Changing event of newOrder2 from null to newOrderItem1"),
//      new RelationChangeState (newOrderItem1, "Order", null, null, "11: 4. Changed event of newOrderItem2 from null to newOrder2"),
//      new CollectionChangeState (newOrder2.OrderItems, newOrderItem1, "11: 5. Adding of newOrderItem1 to newOrder2"),
//      new RelationChangeState (newOrder2, "OrderItems", null, null, "11: 6. Changed event of newOrder2 from null to newOrderItem1"),
//
//      new RelationChangeState (newOrder1, "Official", null, official2, "12: 1. Changing event of newOrder1 from null to official2"),
//      new CollectionChangeState (official2.Orders, newOrder1, "12: 2. Adding of newOrder1 to official2"),
//      new RelationChangeState (official2, "Orders", null, newOrder1, "12: 3. Changing event of official2 from null to newOrder1"),
//      new RelationChangeState (newOrder1, "Official", null, null, "12: 4. Changed event of newOrder1 from null to official2"),
//      new CollectionChangeState (official2.Orders, newOrder1, "12: 5. Adding of newOrder1 to official2"),
//      new RelationChangeState (official2, "Orders", null, null, "12: 6. Changed event of official2 from null to newOrder1"),
//
//      new RelationChangeState (newOrder1, "OrderTicket", null, newOrderTicket1, "13: 1. Changing event of newOrder1 from null to newOrderTicket1"),
//      new RelationChangeState (newOrder1, "OrderTicket", null, null, "13: 2. Changed event of newOrder1 from null to newOrderTicket1")
//    };      
//
//    eventReceiver.Compare (expectedChangeStates);
//    
//    eventReceiver = new SequenceEventReceiver (
//      new DomainObject[] { newCustomer1, newOrderTicket1, newOrder2, newOrder1, newOrderItem1 },
//      new DomainObjectCollection[] { newOrder2.OrderItems, newCustomer1.Orders } );
//
//    //14
//    newOrderTicket1.Order = newOrder2;
//    //15
//    newOrder2.Delete ();
//    //16
//    newOrderTicket1.Order = newOrder1;
//
//    expectedChangeStates = new ChangeState[]
//    { 
//      new RelationChangeState (newOrderTicket1, "Order", newOrder1, newOrder2, "14: 1. Changing event of newOrderTicket1 from newOrder1 to newOrder2"),
//      new RelationChangeState (newOrder1, "OrderTicket", newOrderTicket1, null, "14: 2. Changing event of newOrder1 from newOrderTicket1 to null"),
//      new RelationChangeState (newOrder2, "OrderTicket", null, newOrderTicket1, "14: 3. Changing event of newOrder1 from null to newOrderTicket1"),
//      new RelationChangeState (newOrderTicket1, "Order", null, null, "14: 4. Changed event of newOrderTicket1 from newOrder1 to newOrder2"),
//      new RelationChangeState (newOrder1, "OrderTicket", null, null, "14: 5. Changed event of newOrder1 from newOrderTicket1 to null"),
//      new RelationChangeState (newOrder2, "OrderTicket", null, null, "14: 6. Changed event of newOrder1 from null to newOrderTicket1"),
//
//      new ObjectDeletionState (newOrder2, "15: 1. Deleting event of newOrder2"),
//      new RelationChangeState (newOrderTicket1, "Order", newOrder2, null, "15: 2. Changing event of newOrderTicket1 from newOrder2 to null"),
//      new RelationChangeState (newOrderItem1, "Order", newOrder2, null, "15: 3. Changing event of newOrderItem1 from newOrder2 to null"),
//      new ObjectDeletionState (newOrder2, "15: 4. Deleted event of newOrder2"),
//      new RelationChangeState (newOrderTicket1, "Order", null, null, "15: 5. Changed event of newOrderTicket1 from newOrder2 to null"),
//      new RelationChangeState (newOrderItem1, "Order", null, null, "15: 6. Changed event of newOrderItem1 from newOrder2 to null"),
//
//      // Todo: Check why are these events not firing?
//      //        new CollectionChangeState (newCustomer1.Orders, newOrder2, "15: ?. Removed of newOrder2 from newCustomer1"),
//      //        new RelationChangeState (newCustomer1, "Orders", null, null, "15: ?. Changed event of newCustomer1 from null to newCeo1"),
//      //        new CollectionChangeState (newCustomer1.Orders, newOrder2, "15: ?. Removing of newOrder2 from newCustomer1"),
//      //        new RelationChangeState (newCustomer1, "Orders", null, newOrder2, "15: ?. Changing event of newCustomer1 from null to newCeo1"),
//
//      new RelationChangeState (newOrderTicket1, "Order", null, newOrder1, "16: 1. Changing event of newOrderTicket1 from null to newOrder1"),
//      new RelationChangeState (newOrder1, "OrderTicket", null, newOrderTicket1, "16: 2. Changing event of newOrder1 from null to newOrderTicket1"),
//      new RelationChangeState (newOrderTicket1, "Order", null, null, "16: 3. Changed event of newOrderTicket1 from null to newOrder1"),
//      new RelationChangeState (newOrder1, "OrderTicket", null, null, "16: 4. Changed event of newOrder1 from null to newOrderTicket1"),
//    };
//
//    eventReceiver.Compare (expectedChangeStates);
//
//    // assertion: no exception
//  }

  [Test]
  public void SetValuesAndAccessOriginalValuesTest ()
  {
    OrderItem orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);

    DataContainer dataContainer = orderItem.DataContainer;

    dataContainer.SetValue ("Product", "newProduct");

    Assert.IsFalse (dataContainer.PropertyValues["Product"].OriginalValue.ToString () == "newProduct");
    Assert.AreEqual ("newProduct", orderItem.Product);

    ClientTransactionMock.Commit ();
    orderItem.Product = "newProduct2";

    Assert.IsTrue (dataContainer.PropertyValues["Product"].OriginalValue.ToString () == "newProduct");
    Assert.AreEqual ("newProduct2", orderItem.Product);
  }

  [Test]
  [ExpectedException(typeof(MandatoryRelationNotSetException))]
  public void NewCustomerAndCEOTest ()
  {
    IndustrialSector industrialSector = new IndustrialSector ();
    Customer customer = new Customer ();
    customer.Ceo = new Ceo ();

    industrialSector.Companies.Add (customer);

    Order order1 = new Order ();
    new OrderTicket (order1);

    //getting an SQL Exception without this line
    order1.DeliveryDate = DateTime.Now; 

    OrderItem orderItem = new OrderItem ();
    order1.OrderItems.Add (orderItem);
    order1.Official = Official.GetObject (DomainObjectIDs.Official2);
    customer.Orders.Add (order1);
  
    ClientTransactionMock.Commit ();
    //assertion: no exception

    customer.Delete ();
    ClientTransaction.Current.Commit ();
  }

  [Test]
  [ExpectedException (typeof (StorageProviderException))]
  public void AddInvalidPropertyValueTest ()
  {
    Employee employee = new Employee ();
    //Why is this method public?
    PropertyDefinition propertyDefinition = new PropertyDefinition (
      "testproperty", "testproperty", typeof (string), true, 10);
    PropertyValueCollection propertyValues = employee.DataContainer.PropertyValues;

    Assert.IsFalse (propertyValues.Contains ("testproperty"));

    propertyValues.Add (new PropertyValue (propertyDefinition));
  
    Assert.IsTrue (propertyValues.Contains ("testproperty"));
    Assert.IsNotNull (propertyValues["testproperty"]);

    ClientTransactionMock.Commit ();
  }

  [Test]
  [ExpectedException (typeof (ArgumentException))]
  public void AddPropertyValueWithExistingNameTest ()
  {
    Employee employee = new Employee ();

    PropertyDefinition propertyDefinition = new PropertyDefinition (
      "Name", "Name", typeof (string), true, 10);
    PropertyValueCollection propertyValues = employee.DataContainer.PropertyValues;

    Assert.IsTrue (propertyValues.Contains ("Name"));

    propertyValues.Add (new PropertyValue (propertyDefinition));
  }
}
}
