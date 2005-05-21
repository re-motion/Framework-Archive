using System;
using System.Collections;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
[TestFixture]
public class DeleteNewDomainObjectTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  private Order _newOrder;
  private DataContainer _newOrderContainer;
  private PropertyValueCollection _newOrderPropertyValues;
  private PropertyValue _orderNumberValue;
  private OrderTicket _newOrderTicket;

  // construction and disposing

  public DeleteNewDomainObjectTest ()
  {
  }

  // methods and properties

  public override void SetUp()
  {
    base.SetUp ();

    _newOrder = new Order ();
    _newOrderContainer = _newOrder.DataContainer;
    _newOrderPropertyValues = _newOrderContainer.PropertyValues;
    _orderNumberValue = _newOrderPropertyValues["OrderNumber"];
    _newOrderTicket = new OrderTicket (_newOrder);
  }

  [Test]
  public void RelatedObject ()
  {
    Assert.AreSame (_newOrder, _newOrderTicket.Order);
    Assert.AreSame (_newOrderTicket, _newOrder.OrderTicket);
 
    _newOrder.Delete ();
    
    Assert.IsNull (_newOrderTicket.Order);

    _newOrderTicket.Delete ();

    Assert.AreEqual (0, ClientTransactionMock.DataManager.DataContainerMap.Count);
    Assert.AreEqual (0, ClientTransactionMock.DataManager.RelationEndPointMap.Count);
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void DomainObjectID ()
  {
    _newOrder.Delete ();
    ObjectID id = _newOrder.ID;
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void DomainObjectState ()
  {
    _newOrder.Delete ();
    StateType state = _newOrder.State;
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void DomainObjectDataContainer ()
  {
    _newOrder.Delete ();
    DataContainer dataContainer = _newOrder.DataContainer;
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void DomainObjectDelete ()
  {
    _newOrder.Delete ();
    _newOrder.Delete ();
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void DomainObjectGetRelatedObject ()
  {
    _newOrder.Delete ();
    _newOrder.GetRelatedObject ("OrderTicket");
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void DomainObjectGetRelatedObjects ()
  {
    _newOrder.Delete ();
    _newOrder.GetRelatedObjects ("OrderItems");
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void DomainObjectGetOriginalRelatedObject ()
  {
    _newOrder.Delete ();
    _newOrder.GetRelatedObject ("OrderTicket");
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void DomainObjectGetOriginalRelatedObjects ()
  {
    _newOrder.Delete ();
    _newOrder.GetOriginalRelatedObjects ("OrderItems");
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void DomainObjectSetRelatedObject ()
  {
    _newOrder.Delete ();
    _newOrder.SetRelatedObject ("OrderTicket", _newOrderTicket);
  }

  [Test]
  public void DomainObjectIsDiscarded ()
  {
    Assert.IsFalse (_newOrder.IsDiscarded);

    _newOrder.Delete ();
    
    Assert.IsTrue (_newOrder.IsDiscarded);
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void DataContainerGetIndexer ()
  {
    _newOrder.Delete ();
    int orderNumber = (int) _newOrderContainer["OrderNumber"];
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void DataContainerSetIndexer ()
  {
    _newOrder.Delete ();
    _newOrderContainer["OrderNumber"] = 10;
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void DataContainerGetValue ()
  {
    _newOrder.Delete ();
    _newOrderContainer.GetValue ("OrderNumber");
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void DataContainerSetValue ()
  {
    _newOrder.Delete ();
    _newOrderContainer.SetValue ("OrderNumber", 10);
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void DataContainerDomainObject ()
  {
    _newOrder.Delete ();
    DomainObject domainObject = _newOrderContainer.DomainObject;
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void DataContainerID ()
  {
    _newOrder.Delete ();
    ObjectID id = _newOrderContainer.ID;
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void DataContainerClassDefinition ()
  {
    _newOrder.Delete ();
    ClassDefinition definition = _newOrderContainer.ClassDefinition;
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void DataContainerDomainObjectType ()
  {
    _newOrder.Delete ();
    Type type = _newOrderContainer.DomainObjectType;
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void DataContainerPropertyValues ()
  {
    _newOrder.Delete ();
    PropertyValueCollection propertyValues = _newOrderContainer.PropertyValues;
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void DataContainerState ()
  {
    _newOrder.Delete ();
    StateType state = _newOrderContainer.State;
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void DataContainerTimestamp ()
  {
    _newOrder.Delete ();
    object timestamp = _newOrderContainer.Timestamp;
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void DataContainerClientTransaction ()
  {
    _newOrder.Delete ();
    ClientTransaction clientTransaction = _newOrderContainer.ClientTransaction;
  }

  [Test]
  public void DataContainerIsDiscarded ()
  {
    DataContainer newDataContainer = _newOrder.DataContainer;
    Assert.IsFalse (newDataContainer.IsDiscarded);

    _newOrder.Delete ();
    
    Assert.IsTrue (newDataContainer.IsDiscarded);
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void PropertyValueCollectionContainsPropertyName ()
  {
    _newOrder.Delete ();
    bool contains = _newOrderPropertyValues.Contains ("OrderNumber");
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void PropertyValueCollectionContainsPropertyValue ()
  {
    PropertyValue propertyValue = _newOrderPropertyValues["OrderNumber"];

    _newOrder.Delete ();
    
    bool contains = _newOrderPropertyValues.Contains (propertyValue);
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void PropertyValueCollectionInt32Indexer ()
  {
    _newOrder.Delete ();
    PropertyValue propertyValue = _newOrderPropertyValues[0];   
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void PropertyValueCollectionStringIndexer ()
  {
    _newOrder.Delete ();
    PropertyValue propertyValue = _newOrderPropertyValues["OrderNumber"];   
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void PropertyValueCollectionAdd ()
  {
    PropertyValue propertyValue = _newOrderPropertyValues[0];

    _newOrder.Delete ();
    
    _newOrderPropertyValues.Add (propertyValue); 
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void PropertyValueCollectionCopyTo ()
  {
    _newOrder.Delete ();
    
    _newOrderPropertyValues.CopyTo (new object[0], 0);
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void PropertyValueCollectionCount ()
  {
    _newOrder.Delete ();
    
    int count = _newOrderPropertyValues.Count;
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void PropertyValueCollectionGetEnumerator ()
  {
    _newOrder.Delete ();
    
    IEnumerator enumerator = _newOrderPropertyValues.GetEnumerator (); 
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void PropertyValueCollectionIsReadOnly ()
  {
    _newOrder.Delete ();
    
    bool isReadOnly = _newOrderPropertyValues.IsReadOnly;
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void PropertyValueCollectionIsSynchronized ()
  {
    _newOrder.Delete ();
    
    bool isSynchronized = _newOrderPropertyValues.IsSynchronized;
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void PropertyValueCollectionSyncRoot ()
  {
    _newOrder.Delete ();
    
    object syncRoot = _newOrderPropertyValues.SyncRoot;
  }

  [Test]
  public void PropertyValueCollectionIsDiscarded ()
  {
    PropertyValueCollection propertyValueCollection = _newOrder.DataContainer.PropertyValues;
    Assert.IsFalse (propertyValueCollection.IsDiscarded);

    _newOrder.Delete ();
    
    Assert.IsTrue (propertyValueCollection.IsDiscarded);
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void PropertyValueDefinition ()
  {
    _newOrder.Delete ();
    
    PropertyDefinition definition = _orderNumberValue.Definition;
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void PropertyValueName ()
  {
    _newOrder.Delete ();
    
    string name = _orderNumberValue.Name;
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void PropertyValuePropertyType ()
  {
    _newOrder.Delete ();
    
    Type type = _orderNumberValue.PropertyType;
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void PropertyValueGetValue ()
  {
    _newOrder.Delete ();
    
    object value = _orderNumberValue.Value;
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void PropertyValueSetValue ()
  {
    _newOrder.Delete ();
    
    _orderNumberValue.Value = 10;
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void PropertyValueOriginalValue ()
  {
    _newOrder.Delete ();
    
    object originalValue = _orderNumberValue.OriginalValue;
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void PropertyValueIsNullable ()
  {
    _newOrder.Delete ();
    
    bool isNullable = _orderNumberValue.IsNullable;
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void PropertyValueMaxLength ()
  {
    _newOrder.Delete ();
    
    NaInt32 maxLength = _orderNumberValue.MaxLength;
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void PropertyValueHasChanged ()
  {
    _newOrder.Delete ();
    
    bool hasChanged = _orderNumberValue.HasChanged;
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void PropertyValueEquals ()
  {
    _newOrder.Delete ();
    
    _orderNumberValue.Equals (null);
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void PropertyValueGetHashCode ()
  {
    _newOrder.Delete ();
    
    _orderNumberValue.GetHashCode ();
  }

  [Test]
  public void PropertyValueIsDiscarded ()
  {
    PropertyValue propertyValue = _newOrder.DataContainer.PropertyValues[0];
    Assert.IsFalse (propertyValue.IsDiscarded);

    _newOrder.Delete ();
    
    Assert.IsTrue (propertyValue.IsDiscarded);
  }

  [Test]
  public void Events ()
  {
    SequenceEventReceiver eventReceiver = new SequenceEventReceiver (
        new DomainObject[] {_newOrder, _newOrderTicket},
        new DomainObjectCollection[] {_newOrder.OrderItems});

    _newOrder.Delete ();

    ChangeState[] expectedStates = new ChangeState[]
    {
      new ObjectDeletionState (_newOrder, "1. Deleting event of order"),
      new RelationChangeState (_newOrderTicket, "Order", _newOrder, null, "2. Relation changing event of orderTicket"),
      new ObjectDeletionState (_newOrder, "3. Deleted event of order"),
      new RelationChangeState (_newOrderTicket, "Order", null, null, "4. Relation changed event of orderTicket")
    };

    eventReceiver.Check (expectedStates);
  }

  [Test]
  public void DeleteFromManyToOneRelation ()
  {
    Customer newCustomer = new Customer ();

    _newOrder.Customer = newCustomer;
    
    ObjectID newOrderID = _newOrder.ID;

    _newOrder.Delete ();

    Assert.IsFalse (newCustomer.Orders.Contains (newOrderID));
  }

  [Test]
  public void DeleteFromOneToManyRelation ()
  {
    Customer newCustomer = new Customer ();

    _newOrder.Customer = newCustomer;
    
    ObjectID newCustomerID = newCustomer.ID;

    newCustomer.Delete ();

    Assert.IsNull (_newOrder.Customer);
  }

  [Test]
  public void DeleteNewObjectsInDomainObjectsCommittingEvent ()
  {
    _newOrder.Committing += new EventHandler (NewOrder_Committing);
    _newOrderTicket.Committing += new EventHandler (NewOrderTicket_Committing);
    ClientTransactionMock.Committing += new ClientTransactionEventHandler (ClientTransactionMock_Committing_DeleteNewObjectsInDomainObjectsCommittingEvent);

    ClientTransactionMock.Commit ();
  }

  [Test]
  public void DeleteNewObjectsInClientTransactionsCommittingEvent ()
  {
    ClientTransactionMock.Committing += new ClientTransactionEventHandler (ClientTransactionMock_Committing_DeleteNewObjectsInClientTransactionsCommittingEvent);
    ClientTransactionMock.Commit ();
  }

  private void NewOrder_Committing (object sender, EventArgs e)
  {
    ((Order) sender).Delete ();
    _newOrderTicket.Delete ();
  }

  private void ClientTransactionMock_Committing_DeleteNewObjectsInClientTransactionsCommittingEvent (object sender, ClientTransactionEventArgs args)
  {
    _newOrder.Delete ();
    _newOrderTicket.Delete ();
  }

  private void ClientTransactionMock_Committing_DeleteNewObjectsInDomainObjectsCommittingEvent (object sender, ClientTransactionEventArgs args)
  {
    Assert.AreEqual (0, args.DomainObjects.Count);
  }

  private void NewOrderTicket_Committing(object sender, EventArgs e)
  {
    Assert.Fail ("NewOrderTicket_Committing event should not be raised.");
  }
}
}
