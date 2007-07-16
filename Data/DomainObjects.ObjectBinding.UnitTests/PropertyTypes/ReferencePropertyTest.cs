using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
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

  private ClientTransactionScope _transactionScope;
  private ClientTransaction _clientTransaction;
  private Order _order;
  private OrderTicket _orderTicket;
  private IBusinessObjectClass _orderTicketBusinessObjectClass;

  [SetUp]
  public override void SetUp ()
  {
    base.SetUp ();

    _transactionScope = new ClientTransactionScope ();
    _clientTransaction = _transactionScope.ScopedTransaction;

    _order = new Order();
    _orderTicket = new OrderTicket (_order);
    _orderTicketBusinessObjectClass = new DomainObjectClass (MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderTicket)));
  }

  public override void TearDown ()
  {
    _transactionScope.Leave ();
    base.TearDown ();
  }

  [Test]
  public void SearchAvailableObjectsWithDomainObject ()
  {
    ReferenceProperty referenceProperty = new ReferenceProperty (_orderTicketBusinessObjectClass, GetOrderProperty(), true, null, false);

    IBusinessObject[] businessObjects = referenceProperty.SearchAvailableObjects (_orderTicket, true, "AllOrders");

    Assert.IsNotNull (businessObjects);
    Assert.IsTrue (businessObjects.Length > 0);

    Order order = (Order) businessObjects[0];
    Assert.AreSame (_orderTicket.InitialClientTransaction, order.InitialClientTransaction);
    Assert.IsTrue (order.CanBeUsedInTransaction (_clientTransaction));
  }

  [Test]
  public void SearchAvailableObjectsUsesCurrentTransaction ()
  {
    using (new ClientTransactionScope ())
    {
      ReferenceProperty referenceProperty = new ReferenceProperty (_orderTicketBusinessObjectClass, GetOrderProperty(), true, null, false);

      IBusinessObject[] businessObjects = referenceProperty.SearchAvailableObjects (_orderTicket, true, "AllOrders");

      Assert.IsNotNull (businessObjects);
      Assert.IsTrue (businessObjects.Length > 0);

      Order order = (Order) businessObjects[0];
      Assert.AreNotSame (_orderTicket.InitialClientTransaction, order.InitialClientTransaction);
      Assert.AreSame (ClientTransactionScope.CurrentTransaction, order.InitialClientTransaction);
      Assert.IsFalse (order.CanBeUsedInTransaction (_clientTransaction));
    }
  }

  [Test]
  public void SearchAvailableObjectsWithDifferentObject ()
  {
    ReferenceProperty referenceProperty = new ReferenceProperty (_orderTicketBusinessObjectClass, GetOrderProperty (), true, null, false);

    IBusinessObject[] businessObjects = referenceProperty.SearchAvailableObjects (new BusinessObjectWithIdentity (), true, "AllOrders");
    
    Assert.IsNotNull (businessObjects);
    Assert.IsTrue (businessObjects.Length > 0);
    
    Order order = (Order) businessObjects[0];

    Assert.AreSame (ClientTransactionScope.CurrentTransaction, order.InitialClientTransaction);
    Assert.IsTrue (order.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
  }

  [Test]
  public void SearchAvailableObjectsWithNull ()
  {
    ReferenceProperty referenceProperty = new ReferenceProperty (_orderTicketBusinessObjectClass, GetOrderProperty (), true, null, false);

    IBusinessObject[] businessObjects = referenceProperty.SearchAvailableObjects (null, true, "AllOrders");
    
    Assert.IsNotNull (businessObjects);
    Assert.IsTrue (businessObjects.Length > 0);
    
    Order order = (Order) businessObjects[0];

    Assert.AreSame (ClientTransactionScope.CurrentTransaction, order.InitialClientTransaction);
    Assert.IsTrue (order.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
  }

  private PropertyInfo GetOrderProperty ()
  {
    Type orderTicketType = _orderTicket.GetType ();
    return orderTicketType.GetProperty ("Order");
  }
}
}
