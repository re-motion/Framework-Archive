using System;
using System.Collections.Specialized;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
[TestFixture]
public class DomainObjectCollectionTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public DomainObjectCollectionTest ()
  {
  }

  // methods and properties

  [Test]
  public void ExactType ()
  {
    ObjectID id1 = new ObjectID (c_testDomainProviderID, "ClassWithAllDataTypes", 
        new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

    ObjectID id2 = new ObjectID (c_testDomainProviderID, "ClassWithAllDataTypes", 
        new Guid ("{583EC716-8443-4b55-92BF-09F7C8768529}"));

    ClassWithAllDataTypes c1 = ClassWithAllDataTypes.GetObject (id1);
    ClassWithAllDataTypes c2 = ClassWithAllDataTypes.GetObject (id2);

    DomainObjectCollection domainObjectCollection = new DomainObjectCollection (typeof (ClassWithAllDataTypes));
    domainObjectCollection.Add (c1);
    domainObjectCollection.Add (c2);
    
    Assert.AreEqual (2, domainObjectCollection.Count, "Count");
    CheckKeys (new ObjectID[] {id1, id2}, domainObjectCollection);
    Assert.AreSame (c1, domainObjectCollection[id1], "ClassWithAllDataTypes1");
    Assert.AreSame (c2, domainObjectCollection[id2], "ClassWithAllDataTypes2");
  }

  [Test]
  public void DerivedType ()
  {
    Company company = Company.GetObject (DomainObjectIDs.Company1);
    Partner partner = Partner.GetObject (DomainObjectIDs.Partner1);
    Distributor distributor = Distributor.GetObject (DomainObjectIDs.Distributor2);

    DomainObjectCollection domainObjectCollection = new DomainObjectCollection (typeof (Company));
    domainObjectCollection.Add (company);
    domainObjectCollection.Add (partner);
    domainObjectCollection.Add (distributor);

    Assert.AreEqual (3, domainObjectCollection.Count, "Count");

    CheckKeys (new ObjectID[] {DomainObjectIDs.Company1, DomainObjectIDs.Partner1, DomainObjectIDs.Distributor2},
        domainObjectCollection);

    Assert.AreSame (company, domainObjectCollection[DomainObjectIDs.Company1], "Company");
    Assert.AreSame (partner, domainObjectCollection[DomainObjectIDs.Partner1], "Partner");
    Assert.AreSame (distributor, domainObjectCollection[DomainObjectIDs.Distributor2], "Distributor");
  }

  [Test]
  [ExpectedException (typeof (ArgumentException), 
      "Values of type 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.Distributor' cannot be added to this collection. " 
      + "Values must be of type 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer' "
      + "or derived from 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer'.")]
  public void InvalidDerivedType ()
  {
    Distributor distributor = Distributor.GetObject (DomainObjectIDs.Distributor2);

    DomainObjectCollection customerCollection = new DomainObjectCollection (typeof (Customer));
    customerCollection.Add (distributor);
  }

  [Test]
  public void CopyConstructorWithDerivedType ()
  {
    Company company = Company.GetObject (DomainObjectIDs.Company1);
    Partner partner = Partner.GetObject (DomainObjectIDs.Partner1);
    Distributor distributor = Distributor.GetObject (DomainObjectIDs.Distributor2);

    DomainObjectCollection domainObjectCollection1 = new DomainObjectCollection (typeof (Company));
    domainObjectCollection1.Add (company);
    domainObjectCollection1.Add (partner);
    domainObjectCollection1.Add (distributor);

    DomainObjectCollection domainObjectCollection2 = new DomainObjectCollection (
        domainObjectCollection1, true);

    Assert.AreEqual (3, domainObjectCollection2.Count, "Count");

    CheckKeys (new ObjectID[] {DomainObjectIDs.Company1, DomainObjectIDs.Partner1, DomainObjectIDs.Distributor2},
        domainObjectCollection2);

    Assert.AreSame (company, domainObjectCollection2[DomainObjectIDs.Company1], "Company");
    Assert.AreSame (partner, domainObjectCollection2[DomainObjectIDs.Partner1], "Partner");
    Assert.AreSame (distributor, domainObjectCollection2[DomainObjectIDs.Distributor2], "Distributor");
  }

  private void CheckKeys (ObjectID[] expectedKeys, DomainObjectCollection collection)
  {
    Assert.AreEqual (expectedKeys.Length, collection.Count, "DomainObjectCollection.Count");
    foreach (ObjectID expectedKey in expectedKeys)
      Assert.IsTrue (collection.Contains (expectedKey), string.Format ("Key {0}", expectedKey));
  }

  [Test]
  public void Contains ()
  {
    Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
    DomainObjectCollection customers = new DomainObjectCollection ();

    customers.Add (customer);

    Assert.IsTrue (customers.Contains (customer.ID));
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void ContainsWithNull ()
  {
    DomainObjectCollection customers = new DomainObjectCollection ();
    
    customers.Contains ((ObjectID) null);
  }

  [Test]
  public void ContainsWithDomainObject ()
  {
    Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
    DomainObjectCollection customers = new DomainObjectCollection ();

    customers.Add (customer);

    Assert.IsTrue (customers.Contains (customer));
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void ContainsWithNullDomainObject ()
  {
    DomainObjectCollection customers = new DomainObjectCollection ();
    
    customers.Contains ((DomainObject) null);
  }

  [Test]
  public void CloneRequiredItemType ()
  {
    DomainObjectCollection collection = new DomainObjectCollection (typeof (Order));
    DomainObjectCollection clone = (DomainObjectCollection) collection.Clone ();

    Assert.IsNotNull (clone, "Clone does not exist");
    Assert.AreEqual (typeof (Order), clone.RequiredItemType, "Required item type does not match.");
  }

  [Test]
  public void CloneAllItems ()
  {
    DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));

    Customer customer1 = Customer.GetObject (DomainObjectIDs.Customer1);
    Customer customer2 = Customer.GetObject (DomainObjectIDs.Customer2);

    collection.Add (customer1);
    collection.Add (customer2);

    DomainObjectCollection readOnlyCollection = new DomainObjectCollection (collection, true);

    DomainObjectCollection clone = (DomainObjectCollection) readOnlyCollection.Clone ();

    Assert.IsNotNull (clone, "Clone does not exist");
    Assert.AreEqual (2, clone.Count, "Item count of clone");
    Assert.AreSame (customer1, clone[customer1.ID], "Customer1");
    Assert.AreSame (customer2, clone[customer2.ID], "Customer2");
    Assert.AreEqual (true, clone.IsReadOnly, "IsReadOnly");
  }

  [Test]
  public void Remove ()
  {
    DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));

    Customer customer1 = Customer.GetObject (DomainObjectIDs.Customer1);
    Customer customer2 = Customer.GetObject (DomainObjectIDs.Customer2);

    collection.Add (customer1);
    collection.Add (customer2);

    collection.Remove (customer2.ID);

    Assert.AreEqual (1, collection.Count, "Item count");
    Assert.AreSame (customer1, collection[customer1.ID], "Customer 1");
  }

  [Test]
  public void RemoveOverNumericIndex ()
  {
    DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));
    Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
    collection.Add (customer);

    Assert.AreEqual (1, collection.Count);
    Assert.AreSame (customer, collection[0]);

    collection.Remove (0);
    
    Assert.AreEqual (0, collection.Count);
  }

  [Test]
  public void RemoveObjectNotInCollection ()
  {
    DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));
    Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
    
    collection.Remove (customer);

    // expectation: no exception
  }

  [Test]
  public void Clear ()
  {
    DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));

    Customer customer1 = Customer.GetObject (DomainObjectIDs.Customer1);
    Customer customer2 = Customer.GetObject (DomainObjectIDs.Customer2);

    collection.Add (customer1);
    collection.Add (customer2);

    collection.Clear ();

    Assert.AreEqual (0, collection.Count, "Item count");
  }

  [Test]
  public void AddEvents ()
  {
    DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));
    
    DomainObjectCollectionEventReceiver eventReceiver = new DomainObjectCollectionEventReceiver (
        collection, false);

    Customer customer1 = Customer.GetObject (DomainObjectIDs.Customer1);
    collection.Add (customer1);

    Assert.AreEqual (1, collection.Count);
    Assert.AreEqual (true, eventReceiver.HasAddingEventBeenCalled);
    Assert.AreEqual (true, eventReceiver.HasAddedEventBeenCalled);
    Assert.AreSame (customer1, eventReceiver.AddingDomainObject);
    Assert.AreSame (customer1, eventReceiver.AddedDomainObject);
  }

  [Test]
  public void CancelAddEvents ()
  {
    DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));
    
    DomainObjectCollectionEventReceiver eventReceiver = new DomainObjectCollectionEventReceiver (
        collection, true);

    Customer customer1 = Customer.GetObject (DomainObjectIDs.Customer1);
    collection.Add (customer1);

    Assert.AreEqual (0, collection.Count);
    Assert.AreEqual (true, eventReceiver.HasAddingEventBeenCalled);
    Assert.AreEqual (false, eventReceiver.HasAddedEventBeenCalled);
    Assert.AreSame (customer1, eventReceiver.AddingDomainObject);
    Assert.IsNull (eventReceiver.AddedDomainObject);
  }

  [Test]
  public void RemoveEvents ()
  {
    DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));
    
    Customer customer1 = Customer.GetObject (DomainObjectIDs.Customer1);
    collection.Add (customer1);
    collection.Add (Customer.GetObject (DomainObjectIDs.Customer2));

    DomainObjectCollectionEventReceiver eventReceiver = new DomainObjectCollectionEventReceiver (
        collection, false);

    collection.Remove (customer1.ID);

    Assert.AreEqual (1, collection.Count);
    Assert.AreEqual (true, eventReceiver.HasRemovingEventBeenCalled);
    Assert.AreEqual (true, eventReceiver.HasRemovedEventBeenCalled);
    Assert.AreEqual (1, eventReceiver.RemovingDomainObjects.Count);
    Assert.AreEqual (1, eventReceiver.RemovedDomainObjects.Count);
    Assert.AreSame (customer1, eventReceiver.RemovingDomainObjects[0]);
    Assert.AreSame (customer1, eventReceiver.RemovedDomainObjects[0]);
  }

  [Test]
  public void CancelRemoveEvents ()
  {
    DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));
    
    Customer customer1 = Customer.GetObject (DomainObjectIDs.Customer1);
    collection.Add (customer1);
    collection.Add (Customer.GetObject (DomainObjectIDs.Customer2));

    DomainObjectCollectionEventReceiver eventReceiver = new DomainObjectCollectionEventReceiver (
        collection, true);

    collection.Remove (customer1.ID);

    Assert.AreEqual (2, collection.Count);
    Assert.AreEqual (true, eventReceiver.HasRemovingEventBeenCalled);
    Assert.AreEqual (false, eventReceiver.HasRemovedEventBeenCalled);
    Assert.AreEqual (1, eventReceiver.RemovingDomainObjects.Count);
    Assert.AreEqual (0, eventReceiver.RemovedDomainObjects.Count);
    Assert.AreSame (customer1, eventReceiver.RemovingDomainObjects[0]);
  }

  [Test]
  public void ClearEvents ()
  {
    DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));
    
    Customer customer1 = Customer.GetObject (DomainObjectIDs.Customer1);
    Customer customer2 = Customer.GetObject (DomainObjectIDs.Customer2);
    collection.Add (customer1);
    collection.Add (customer2);

    DomainObjectCollectionEventReceiver eventReceiver = new DomainObjectCollectionEventReceiver (
        collection, false);

    collection.Clear ();

    Assert.AreEqual (0, collection.Count);
    Assert.AreEqual (true, eventReceiver.HasRemovingEventBeenCalled);
    Assert.AreEqual (true, eventReceiver.HasRemovedEventBeenCalled);
    Assert.AreEqual (2, eventReceiver.RemovingDomainObjects.Count);
    Assert.AreEqual (2, eventReceiver.RemovedDomainObjects.Count);
    Assert.AreSame (customer1, eventReceiver.RemovingDomainObjects[customer1.ID]);
    Assert.AreSame (customer2, eventReceiver.RemovingDomainObjects[customer2.ID]);
    Assert.AreSame (customer1, eventReceiver.RemovedDomainObjects[customer1.ID]);
    Assert.AreSame (customer2, eventReceiver.RemovedDomainObjects[customer2.ID]);
  }

  [Test]
  public void CancelClearEvents ()
  {
    DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));
    
    Customer customer1 = Customer.GetObject (DomainObjectIDs.Customer1);
    Customer customer2 = Customer.GetObject (DomainObjectIDs.Customer2);
    collection.Add (customer1);
    collection.Add (customer2);

    DomainObjectCollectionEventReceiver eventReceiver = new DomainObjectCollectionEventReceiver (
        collection, true);

    collection.Clear ();

    Assert.AreEqual (2, collection.Count);
    Assert.AreEqual (true, eventReceiver.HasRemovingEventBeenCalled);
    Assert.AreEqual (false, eventReceiver.HasRemovedEventBeenCalled);
    Assert.AreEqual (2, eventReceiver.RemovingDomainObjects.Count);
    Assert.AreEqual (0, eventReceiver.RemovedDomainObjects.Count);
  }

  [Test]
  public void CompareTrue ()
  {
    DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));
    
    Assert.IsTrue (DomainObjectCollection.Compare (collection, collection));
  }

  [Test]
  public void CompareFalse ()
  {
    DomainObjectCollection collection1 = new DomainObjectCollection (typeof (Customer));
    DomainObjectCollection collection2 = new DomainObjectCollection (typeof (Customer));

    collection1.Add (Customer.GetObject (DomainObjectIDs.Customer1));

    Assert.IsFalse (DomainObjectCollection.Compare (collection1, collection2));
  }

  [Test]
  public void CompareFalseWithSameCount ()
  {
    DomainObjectCollection collection1 = new DomainObjectCollection (typeof (Customer));
    DomainObjectCollection collection2 = new DomainObjectCollection (typeof (Customer));

    collection1.Add (Customer.GetObject (DomainObjectIDs.Customer1));
    collection2.Add (Customer.GetObject (DomainObjectIDs.Customer2));

    Assert.IsFalse (DomainObjectCollection.Compare (collection1, collection2));
  }

  [Test]
  public void CompareFalseWithDifferentOrder ()
  {
    DomainObjectCollection collection1 = new DomainObjectCollection (typeof (Customer));
    collection1.Add (Customer.GetObject (DomainObjectIDs.Customer1));
    collection1.Add (Customer.GetObject (DomainObjectIDs.Customer2));

    DomainObjectCollection collection2 = new DomainObjectCollection (typeof (Customer));
    collection2.Add (Customer.GetObject (DomainObjectIDs.Customer2));
    collection2.Add (Customer.GetObject (DomainObjectIDs.Customer1));

    Assert.IsFalse (DomainObjectCollection.Compare (collection1, collection2));
  }

  [Test]
  public void CompareTrueWithSameOrder ()
  {
    DomainObjectCollection collection1 = new DomainObjectCollection (typeof (Customer));
    collection1.Add (Customer.GetObject (DomainObjectIDs.Customer1));
    collection1.Add (Customer.GetObject (DomainObjectIDs.Customer2));

    DomainObjectCollection collection2 = new DomainObjectCollection (typeof (Customer));
    collection2.Add (Customer.GetObject (DomainObjectIDs.Customer1));
    collection2.Add (Customer.GetObject (DomainObjectIDs.Customer2));

    Assert.IsTrue (DomainObjectCollection.Compare (collection1, collection2));
  }

  [Test]
  public void CompareFalseWithCollection1Null ()
  {
    DomainObjectCollection collection2 = new DomainObjectCollection (typeof (Customer));

    Assert.IsFalse (DomainObjectCollection.Compare (null, collection2));
  }

  [Test]
  public void CompareFalseWithCollection2Null ()
  {
    DomainObjectCollection collection1 = new DomainObjectCollection (typeof (Customer));

    Assert.IsFalse (DomainObjectCollection.Compare (collection1, null));
  }

  [Test]
  public void CompareTrueWithBothCollectionsNull ()
  {
    Assert.IsTrue (DomainObjectCollection.Compare (null, null));
  }

  [Test]
  public void RemoveObjectNotInCollectionWithEvents ()
  {
    DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));
    DomainObjectCollectionEventReceiver eventReceiver = new DomainObjectCollectionEventReceiver (collection);
    Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
    
    collection.Remove (customer);

    Assert.IsFalse (eventReceiver.HasRemovingEventBeenCalled);
    Assert.IsFalse (eventReceiver.HasRemovedEventBeenCalled);
    Assert.AreEqual (0, eventReceiver.RemovingDomainObjects.Count);
    Assert.AreEqual (0, eventReceiver.RemovedDomainObjects.Count);
  }

  [Test]
  public void RemoveNullObjectIDWithEvents ()
  {
    DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));
    DomainObjectCollectionEventReceiver eventReceiver = new DomainObjectCollectionEventReceiver (collection);
  
    try
    {
      collection.Remove ((ObjectID) null);
    }
    catch (ArgumentNullException)
    {
    }

    Assert.IsFalse (eventReceiver.HasRemovingEventBeenCalled);
    Assert.IsFalse (eventReceiver.HasRemovedEventBeenCalled);
    Assert.AreEqual (0, eventReceiver.RemovingDomainObjects.Count);
    Assert.AreEqual (0, eventReceiver.RemovedDomainObjects.Count);
  }

  [Test]
  public void RemoveNullDomainObjectWithEvents ()
  {
    DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));
    DomainObjectCollectionEventReceiver eventReceiver = new DomainObjectCollectionEventReceiver (collection);
    
    try
    {
      collection.Remove ((DomainObject) null);
    }
    catch (ArgumentNullException)
    {
    }

    Assert.IsFalse (eventReceiver.HasRemovingEventBeenCalled);
    Assert.IsFalse (eventReceiver.HasRemovedEventBeenCalled);
    Assert.AreEqual (0, eventReceiver.RemovingDomainObjects.Count);
    Assert.AreEqual (0, eventReceiver.RemovedDomainObjects.Count);
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void IndexerWithNullObjectID ()
  {
    DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));

    DomainObject domainObject = collection[null];
  }

  [Test]
  [ExpectedException (typeof (InvalidOperationException))]
  public void ChangeCollectionDuringEnumeration ()
  {
    Order order = Order.GetObject (DomainObjectIDs.Order1);

    foreach (OrderItem item in order.OrderItems)
      order.OrderItems.Remove (item);
  }

  [Test]
  [ExpectedException (typeof (NotSupportedException))]
  public void RemoveFromReadOnlyCollection ()
  {
    Order order = Order.GetObject (DomainObjectIDs.Order1);
    DomainObjectCollection readOnlyCollection = new DomainObjectCollection (order.OrderItems, true);

    readOnlyCollection.Remove (DomainObjectIDs.OrderItem1);
  }

  [Test]
  [ExpectedException (typeof (NotSupportedException))]
  public void AddToReadOnlyCollection ()
  {
    Order order = Order.GetObject (DomainObjectIDs.Order1);
    DomainObjectCollection readOnlyCollection = new DomainObjectCollection (order.OrderItems, true);

    readOnlyCollection.Add (OrderItem.GetObject (DomainObjectIDs.OrderItem3));
  }

  [Test]
  [ExpectedException (typeof (NotSupportedException))]
  public void ClearReadOnlyCollection ()
  {
    Order order = Order.GetObject (DomainObjectIDs.Order1);
    DomainObjectCollection readOnlyCollection = new DomainObjectCollection (order.OrderItems, true);

    readOnlyCollection.Clear ();
  }

  [Test]
  public void CopyToArray ()
  {
    Order order = Order.GetObject (DomainObjectIDs.Order1);
    OrderItem[] items = new OrderItem [order.OrderItems.Count];

    order.OrderItems.CopyTo (items, 0);

    Assert.AreSame (order.OrderItems[0], items[0]);
    Assert.AreSame (order.OrderItems[1], items[1]);
  }

  [Test]
  public void CopyToArrayWithIndex ()
  {
    Order order = Order.GetObject (DomainObjectIDs.Order1);
    OrderItem[] items = new OrderItem [order.OrderItems.Count + 1];

    OrderItem otherItem = OrderItem.GetObject (DomainObjectIDs.OrderItem3);
    items[0] = otherItem;

    order.OrderItems.CopyTo (items, 1);

    Assert.AreSame (otherItem, items[0]);
    Assert.AreSame (order.OrderItems[0], items[1]);
    Assert.AreSame (order.OrderItems[1], items[2]);
  }

  [Test]
  public void Clone ()
  {
    DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));
    
    Customer customer1 = Customer.GetObject (DomainObjectIDs.Customer1);
    Customer customer2 = Customer.GetObject (DomainObjectIDs.Customer2);
    collection.Add (customer1);
    collection.Add (customer2);

    ICloneable cloneableCollection = (ICloneable) collection;
    DomainObjectCollection clonedCollection = (DomainObjectCollection) cloneableCollection.Clone ();

    Assert.IsNotNull (clonedCollection);
    Assert.AreEqual (collection.Count, clonedCollection.Count);
    Assert.AreEqual (collection.IsReadOnly, clonedCollection.IsReadOnly);
    Assert.AreEqual (collection.RequiredItemType, clonedCollection.RequiredItemType);
    Assert.AreSame (collection[0], clonedCollection[0]);
    Assert.AreSame (collection[1], clonedCollection[1]);
  }
}
}