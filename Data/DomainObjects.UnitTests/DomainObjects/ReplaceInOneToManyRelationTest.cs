using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.EventSequence;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
[TestFixture]
public class ReplaceInOneToManyRelationTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  private Customer _customer;
  private Order _oldOrder;
  private Order _newOrder;

  // construction and disposing

  public ReplaceInOneToManyRelationTest ()
  {
  }

  // methods and properties

  public override void SetUp()
  {
    base.SetUp ();

    _customer = Customer.GetObject (DomainObjectIDs.Customer1);
    _oldOrder = Order.GetObject (DomainObjectIDs.Order1);
    _newOrder = Order.GetObject (DomainObjectIDs.Order2);
  }

// TODO: Implement this!
//  [Test]
//  public void Events ()
//  {
//    DomainObject[] domainObjectEventSources = new DomainObject[] {_customer, _oldOrder, _newOrder};
//    DomainObjectCollection[] collectionEventSources = new DomainObjectCollection[] {_customer.Orders}; 
//    SequenceEventReceiver eventReceiver = new SequenceEventReceiver (domainObjectEventSources, collectionEventSources);
//           
//    _customer.Orders[0] = _newOrder;
//
//    ChangeState[] expectedChangeStates = new ChangeState[]
//    {
//      new RelationChangeState (_oldOrder, "Customer", _customer, null, "1. Changing event of old order from old customer to null"),
//      new RelationChangeState (_newOrder, "Customer", null, _customer, "2. Changing event of new order from null to new customer"),
//      new CollectionChangeState (_customer.Orders, _oldOrder, "3. Removing event of old order from orders"),
//      new CollectionChangeState (_customer.Orders, _newOrder, "4. Adding event of new order to orders"),
//      new RelationChangeState (_customer, "Orders", _oldOrder, _newOrder, "5. Changing event of customer"),
//
//      new RelationChangeState (_oldOrder, "Customer", null, null, "6. Changed event of old order from old customer to null"),
//      new RelationChangeState (_newOrder, "Customer", null, null, "7. Changed event of new order from null to new customer"),
//      new CollectionChangeState (_customer.Orders, _oldOrder, "8. Removed event of old order from orders"),
//      new CollectionChangeState (_customer.Orders, _newOrder, "9. Added event of new order to orders"),
//      new RelationChangeState (_customer, "Orders", null, null, "10. Changed event of customer"),
//    };      
//
//    eventReceiver.Compare (expectedChangeStates);
//
//    Assert.AreEqual (StateType.Changed, _customer.State);
//    Assert.AreEqual (StateType.Changed, _oldOrder.State);
//    Assert.AreEqual (StateType.Changed, _newOrder.State);
//
//    Assert.AreSame (_newOrder, _customer.Orders[0]);
//    Assert.AreSame (_customer, _newOrder.Customer);
//
//    Assert.IsFalse (_customer.Orders.Contains (_oldOrder));
//    Assert.IsNull (_oldOrder.Customer);
//  }
}
}
