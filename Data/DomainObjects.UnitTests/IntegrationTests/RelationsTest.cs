using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.EventReceiver;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.IntegrationTests
{
  [TestFixture]
  public class RelationsTest : ClientTransactionBaseTest
  {
    [Test]
    public void OneToOneRelationChangeTest ()
    {
      Order order = DomainObject.GetObject<Order> (DomainObjectIDs.Order1);
      OrderTicket orderTicket = order.OrderTicket;

      DomainObjectRelationCheckEventReceiver orderEventReceiver = new DomainObjectRelationCheckEventReceiver (order);
      DomainObjectRelationCheckEventReceiver orderTicketEventReceiver = new DomainObjectRelationCheckEventReceiver (orderTicket);

      orderTicket.Order = null;

      Assert.IsTrue (orderEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsTrue (orderTicketEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.AreSame (orderTicket, orderEventReceiver.GetChangingRelatedDomainObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));
      Assert.AreSame (order, orderTicketEventReceiver.GetChangingRelatedDomainObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"));

      Assert.IsTrue (orderEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.IsTrue (orderTicketEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreSame (null, orderEventReceiver.GetChangedRelatedDomainObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));
      Assert.AreSame (null, orderTicketEventReceiver.GetChangedRelatedDomainObject ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"));
    }

  }
}
