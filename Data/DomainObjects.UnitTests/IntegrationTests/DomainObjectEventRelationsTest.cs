using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.IntegrationTests
{
[TestFixture]
public class DomainObjectEventRelationsTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public DomainObjectEventRelationsTest ()
  {
  }

  // methods and properties

  [Test]
  public void OneToOneRelationChangeTest ()
  {
    Order order = Order.GetObject (DomainObjectIDs.Order1);
    OrderTicket orderTicket = order.OrderTicket;

    DomainObjectRelationCheckEventReceiver orderEventReceiver = new DomainObjectRelationCheckEventReceiver (order);
    DomainObjectRelationCheckEventReceiver orderTicketEventReceiver = new DomainObjectRelationCheckEventReceiver (orderTicket);

    orderTicket.Order = null;

    Assert.IsTrue (orderEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.IsTrue (orderTicketEventReceiver.HasRelationChangingEventBeenCalled);
    Assert.AreSame (orderTicket, orderEventReceiver.GetChangingRelatedDomainObject ("OrderTicket"));
    Assert.AreSame (order, orderTicketEventReceiver.GetChangingRelatedDomainObject ("Order"));

    Assert.IsTrue (orderEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.IsTrue (orderTicketEventReceiver.HasRelationChangedEventBeenCalled);
    Assert.AreSame (null, orderEventReceiver.GetChangedRelatedDomainObject ("OrderTicket"));
    Assert.AreSame (null, orderTicketEventReceiver.GetChangedRelatedDomainObject ("Order"));
  }

}
}
