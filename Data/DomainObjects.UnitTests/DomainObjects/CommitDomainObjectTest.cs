using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
[TestFixture]
public class CommitDomainObjectTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public CommitDomainObjectTest ()
  {
  }

  // methods and properties

  [Test]
  public void CommittedEvent ()
  {
    Order order = Order.GetObject (DomainObjectIDs.Order1);
    OrderTicket oldOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
    OrderTicket newOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);
    Order oldOrderOfNewOrderTicket = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);

    DomainObjectEventReceiver orderEventReceiver = new DomainObjectEventReceiver (order);
    DomainObjectEventReceiver oldOrderTicketEventReceiver = new DomainObjectEventReceiver (oldOrderTicket);
    DomainObjectEventReceiver newOrderTicketEventReceiver = new DomainObjectEventReceiver (newOrderTicket);
    
    DomainObjectEventReceiver oldOrderOfNewOrderTicketEventReceiver = 
        new DomainObjectEventReceiver (oldOrderOfNewOrderTicket);

    oldOrderTicket.Order = newOrderTicket.Order;
    order.OrderTicket = newOrderTicket;

    ClientTransaction.Current.Commit ();    

    Assert.IsTrue (orderEventReceiver.HasCommittedEventBeenCalled);
    Assert.IsTrue (oldOrderTicketEventReceiver.HasCommittedEventBeenCalled);
    Assert.IsTrue (newOrderTicketEventReceiver.HasCommittedEventBeenCalled);
    Assert.IsTrue (oldOrderOfNewOrderTicketEventReceiver.HasCommittedEventBeenCalled);
  }

  [Test]
  public void CommitOneToManyRelation ()
  {
    Customer customer1 = Customer.GetObject (DomainObjectIDs.Customer1);
    Customer customer2 = Customer.GetObject (DomainObjectIDs.Customer2);
    Order order = customer1.Orders[DomainObjectIDs.Order1];

    customer2.Orders.Add (order);

    Assert.AreEqual (StateType.Changed, customer1.State);
    Assert.AreEqual (StateType.Changed, customer2.State);
    Assert.AreEqual (StateType.Changed, order.State);

    ClientTransaction.Current.Commit ();

    Assert.AreEqual (StateType.Original, customer1.State);
    Assert.AreEqual (StateType.Original, customer2.State);
    Assert.AreEqual (StateType.Original, order.State);
  }

  [Test]
  public void CommitOneToOneRelation ()
  {
    Order order = Order.GetObject (DomainObjectIDs.Order1);
    OrderTicket oldOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
    OrderTicket newOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);
    
    object orderTimestamp = order.DataContainer.Timestamp;
    object oldOrderTicketTimestamp = oldOrderTicket.DataContainer.Timestamp;
    object newOrderTicketTimestamp = newOrderTicket.DataContainer.Timestamp;

    oldOrderTicket.Order = newOrderTicket.Order;
    order.OrderTicket = newOrderTicket;
    
    ClientTransaction.Current.Commit ();    

    Assert.AreEqual (orderTimestamp, order.DataContainer.Timestamp);
    Assert.IsFalse (oldOrderTicketTimestamp.Equals (oldOrderTicket.DataContainer.Timestamp));
    Assert.IsFalse (newOrderTicketTimestamp.Equals (newOrderTicket.DataContainer.Timestamp));
  }

  [Test]
  public void CommitHierarchy ()
  {
    Employee supervisor1 = Employee.GetObject (DomainObjectIDs.Employee1);
    Employee supervisor2 = Employee.GetObject (DomainObjectIDs.Employee2);
    Employee subordinate = (Employee) supervisor1.Subordinates[DomainObjectIDs.Employee4];
 
    subordinate.Supervisor = supervisor2;

    ClientTransaction.Current.Commit ();
    ClientTransaction.SetCurrent (null);

    supervisor1 = Employee.GetObject (DomainObjectIDs.Employee1);
    supervisor2 = Employee.GetObject (DomainObjectIDs.Employee2);
    
    Assert.IsNull (supervisor1.Subordinates[DomainObjectIDs.Employee4]);
    Assert.IsNotNull (supervisor2.Subordinates[DomainObjectIDs.Employee4]);
  }

  [Test]
  public void CommitPolymorphicRelation ()
  {
    Ceo companyCeo = Ceo.GetObject (DomainObjectIDs.Ceo1);
    Ceo distributorCeo = Ceo.GetObject (DomainObjectIDs.Ceo10);
    Company company = companyCeo.Company;
    Distributor distributor = Distributor.GetObject (DomainObjectIDs.Distributor1);

    distributor.Ceo = companyCeo;
    company.Ceo = distributorCeo;

    ClientTransaction.Current.Commit ();
    ClientTransaction.SetCurrent (null);

    companyCeo = Ceo.GetObject (DomainObjectIDs.Ceo1);
    distributorCeo = Ceo.GetObject (DomainObjectIDs.Ceo10);
    company = Company.GetObject (DomainObjectIDs.Company1);
    distributor = Distributor.GetObject (DomainObjectIDs.Distributor1);

    Assert.AreSame (companyCeo, distributor.Ceo);
    Assert.AreSame (distributor, companyCeo.Company);
    Assert.AreSame (distributorCeo, company.Ceo);
    Assert.AreSame (company, distributorCeo.Company);
  }

  [Test]
  public void CommitPropertyChange ()
  {
    Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
    customer.Name = "Arthur Dent";

    ClientTransaction.Current.Commit ();
    ClientTransaction.SetCurrent (null);

    customer = Customer.GetObject (DomainObjectIDs.Customer1);
    Assert.AreEqual ("Arthur Dent", customer.Name);
  }
}
}
