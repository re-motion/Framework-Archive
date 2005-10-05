using System;
using System.Reflection;

using NUnit.Framework;

using Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes;
using Rubicon.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain;
using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.UnitTests.PropertyTypes
{
[TestFixture]
public class ReferencePropertyTest : DatabaseTest
{
	public ReferencePropertyTest ()
	{
	}

  private ClientTransaction _clientTransaction;
  private Order _order;
  private OrderTicket _orderTicket;

  [SetUp]
  public override void SetUp ()
  {
    base.SetUp ();

    _clientTransaction = new ClientTransaction ();
    _order = new Order (_clientTransaction);
    _orderTicket = new OrderTicket (_order, _clientTransaction);
  }

  [Test]
  public void SearchAvailableObjectsWithDomainObject ()
  {
    ReferenceProperty referenceProperty = new ReferenceProperty (GetOrderProperty (), true, null, false);

    IBusinessObjectWithIdentity[] businessObjects = referenceProperty.SearchAvailableObjects (_orderTicket, "AllOrders");
    
    Assert.IsNotNull (businessObjects);
    Assert.IsTrue (businessObjects.Length > 0);
    
    Order order = (Order) businessObjects[0];
    Assert.IsTrue (object.ReferenceEquals (_orderTicket.ClientTransaction, order.ClientTransaction));
    Assert.IsFalse (object.ReferenceEquals (ClientTransaction.Current, order.ClientTransaction));
  }

  [Test]
  public void SearchAvailableObjectsWithDifferentObject ()
  {
    ReferenceProperty referenceProperty = new ReferenceProperty (GetOrderProperty (), true, null, false);

    IBusinessObjectWithIdentity[] businessObjects = referenceProperty.SearchAvailableObjects (new BusinessObjectWithIdentity (), "AllOrders");
    
    Assert.IsNotNull (businessObjects);
    Assert.IsTrue (businessObjects.Length > 0);
    
    Order order = (Order) businessObjects[0];
    Assert.IsTrue (object.ReferenceEquals (ClientTransaction.Current, order.ClientTransaction));
  }

  [Test]
  public void SearchAvailableObjectsWithNull ()
  {
    ReferenceProperty referenceProperty = new ReferenceProperty (GetOrderProperty (), true, null, false);

    IBusinessObjectWithIdentity[] businessObjects = referenceProperty.SearchAvailableObjects (null, "AllOrders");
    
    Assert.IsNotNull (businessObjects);
    Assert.IsTrue (businessObjects.Length > 0);
    
    Order order = (Order) businessObjects[0];
    Assert.IsTrue (object.ReferenceEquals (ClientTransaction.Current, order.ClientTransaction));
  }

  private PropertyInfo GetOrderProperty ()
  {
    Type orderTicketType = _orderTicket.GetType ();
    return orderTicketType.GetProperty ("Order");
  }
}
}
