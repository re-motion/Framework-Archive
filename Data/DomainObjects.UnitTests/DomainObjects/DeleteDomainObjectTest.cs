using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
[TestFixture]
public class DeleteDomainObjectTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  Order _order;
  OrderTicket _orderTicket;

  // construction and disposing

  public DeleteDomainObjectTest ()
  {
  }

  // methods and properties

  public override void SetUp()
  {
    base.SetUp ();

    _order = Order.GetObject (DomainObjectIDs.Order2);
    _orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
  }

  [Test]
  public void Delete ()
  {
    _orderTicket.Delete ();

    Assert.AreEqual (StateType.Deleted, _orderTicket.State);
    Assert.AreEqual (StateType.Deleted, _orderTicket.DataContainer.State);
  }

  [Test]
  public void DeleteTwice ()
  {
    _orderTicket.Delete ();

    SequenceEventReceiver eventReceiver = new SequenceEventReceiver (_orderTicket);
    _orderTicket.Delete ();

    Assert.AreEqual (0, eventReceiver.Count);
  }

  [Test]
  [ExpectedException (typeof (ObjectDeletedException))]
  public void GetObject ()
  {
    _orderTicket.Delete ();

    OrderTicket.GetObject (_orderTicket.ID);
  }

  [Test]
  public void GetObjectAndIncludeDeleted ()
  {
    _orderTicket.Delete ();

    Assert.IsNotNull (OrderTicket.GetObject (_orderTicket.ID, true));
  }

  [Test]
  [ExpectedException (typeof (ObjectDeletedException))]      
  public void ModifyDeletedObject ()
  {
    PropertyValue propertyValue = _order.DataContainer.PropertyValues["OrderNumber"];

    _order.Delete ();

    propertyValue.Value = 10;
  }

  [Test]
  public void AccessDeletedObject ()
  {
    _order.Delete ();

    Assert.AreEqual (DomainObjectIDs.Order2, _order.ID);
    Assert.AreEqual (3, _order.OrderNumber);
    Assert.AreEqual (new DateTime (2005, 3, 1), _order.DeliveryDate);
    Assert.IsNotNull (_order.DataContainer.Timestamp);
    Assert.IsNotNull (_order.DataContainer.PropertyValues["OrderNumber"]);
  }

  [Test]
  public void CascadedDelete ()
  {
    Employee supervisor = Employee.GetObject (DomainObjectIDs.Employee1);
    supervisor.DeleteWithSubordinates ();

    DomainObject deletedSubordinate4 = TestDomainBase.GetObject (DomainObjectIDs.Employee4, true);
    DomainObject deletedSubordinate5 = TestDomainBase.GetObject (DomainObjectIDs.Employee5, true);

    Assert.AreEqual (StateType.Deleted, supervisor.State);
    Assert.AreEqual (StateType.Deleted, deletedSubordinate4.State);
    Assert.AreEqual (StateType.Deleted, deletedSubordinate5.State);

    ClientTransactionMock.Commit ();
    ReInitializeTransaction ();

    CheckIfObjectIsDeleted (DomainObjectIDs.Employee1);
    CheckIfObjectIsDeleted (DomainObjectIDs.Employee4);
    CheckIfObjectIsDeleted (DomainObjectIDs.Employee5);
  }
}
}
