using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
[TestFixture]
public class DeleteNewDomainObjectTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  private Order _newOrder;
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
    _newOrderTicket = new OrderTicket (_newOrder);
  }

  [Test]
  public void DeleteNewWithRelatedObject ()
  {
    Assert.AreSame (_newOrder, _newOrderTicket.Order);
    Assert.AreSame (_newOrderTicket, _newOrder.OrderTicket);
 
    _newOrder.Delete ();
    
    Assert.IsNull (_newOrderTicket.Order);

    _newOrderTicket.Delete ();

    Assert.AreEqual (0, ClientTransactionMock.DataManager.DataContainerMap.Count);
    Assert.AreEqual (0, ClientTransactionMock.DataManager.RelationEndPointMap.Count);
  }
}
}
