using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class CommitDomainObjectTest : ClientTransactionBaseTest
  {
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      SetDatabaseModifyable ();
    }

    [Test]
    public void CommitOneToManyRelation ()
    {
      Customer customer1 = DomainObject.GetObject<Customer> (DomainObjectIDs.Customer1);
      Customer customer2 = DomainObject.GetObject<Customer> (DomainObjectIDs.Customer2);
      Order order = customer1.Orders[DomainObjectIDs.Order1];

      customer2.Orders.Add (order);

      Assert.AreEqual (StateType.Changed, customer1.State);
      Assert.AreEqual (StateType.Changed, customer2.State);
      Assert.AreEqual (StateType.Changed, order.State);

      ClientTransactionMock.Commit ();

      Assert.AreEqual (StateType.Unchanged, customer1.State);
      Assert.AreEqual (StateType.Unchanged, customer2.State);
      Assert.AreEqual (StateType.Unchanged, order.State);
    }

    [Test]
    public void CommitOneToOneRelation ()
    {
      Order order = DomainObject.GetObject<Order> (DomainObjectIDs.Order1);
      OrderTicket oldOrderTicket = DomainObject.GetObject<OrderTicket> (DomainObjectIDs.OrderTicket1);
      OrderTicket newOrderTicket = DomainObject.GetObject<OrderTicket> (DomainObjectIDs.OrderTicket2);

      object orderTimestamp = order.DataContainer.Timestamp;
      object oldOrderTicketTimestamp = oldOrderTicket.DataContainer.Timestamp;
      object newOrderTicketTimestamp = newOrderTicket.DataContainer.Timestamp;

      oldOrderTicket.Order = newOrderTicket.Order;
      order.OrderTicket = newOrderTicket;

      ClientTransactionMock.Commit ();

      Assert.AreEqual (orderTimestamp, order.DataContainer.Timestamp);
      Assert.IsFalse (oldOrderTicketTimestamp.Equals (oldOrderTicket.DataContainer.Timestamp));
      Assert.IsFalse (newOrderTicketTimestamp.Equals (newOrderTicket.DataContainer.Timestamp));
    }

    [Test]
    public void CommitHierarchy ()
    {
      Employee supervisor1 = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee1);
      Employee supervisor2 = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee2);
      Employee subordinate = (Employee) supervisor1.Subordinates[DomainObjectIDs.Employee4];

      subordinate.Supervisor = supervisor2;

      ClientTransactionMock.Commit ();
      ReInitializeTransaction ();

      supervisor1 = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee1);
      supervisor2 = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee2);

      Assert.IsNull (supervisor1.Subordinates[DomainObjectIDs.Employee4]);
      Assert.IsNotNull (supervisor2.Subordinates[DomainObjectIDs.Employee4]);
    }

    [Test]
    public void CommitPolymorphicRelation ()
    {
      Ceo companyCeo = DomainObject.GetObject<Ceo> (DomainObjectIDs.Ceo1);
      Ceo distributorCeo = DomainObject.GetObject<Ceo> (DomainObjectIDs.Ceo10);
      Company company = companyCeo.Company;
      Distributor distributor = DomainObject.GetObject<Distributor> (DomainObjectIDs.Distributor1);

      distributor.Ceo = companyCeo;
      company.Ceo = distributorCeo;

      ClientTransactionMock.Commit ();
      ReInitializeTransaction ();

      companyCeo = DomainObject.GetObject<Ceo> (DomainObjectIDs.Ceo1);
      distributorCeo = DomainObject.GetObject<Ceo> (DomainObjectIDs.Ceo10);
      company = DomainObject.GetObject<Company> (DomainObjectIDs.Company1);
      distributor = DomainObject.GetObject<Distributor> (DomainObjectIDs.Distributor1);

      Assert.AreSame (companyCeo, distributor.Ceo);
      Assert.AreSame (distributor, companyCeo.Company);
      Assert.AreSame (distributorCeo, company.Ceo);
      Assert.AreSame (company, distributorCeo.Company);
    }

    [Test]
    public void CommitPropertyChange ()
    {
      Customer customer = DomainObject.GetObject<Customer> (DomainObjectIDs.Customer1);
      customer.Name = "Arthur Dent";

      ClientTransactionMock.Commit ();
      ReInitializeTransaction ();

      customer = DomainObject.GetObject<Customer> (DomainObjectIDs.Customer1);
      Assert.AreEqual ("Arthur Dent", customer.Name);
    }

    [Test]
    public void OriginalDomainObjectCollectionIsSameAfterCommit ()
    {
      Order order = DomainObject.GetObject<Order> (DomainObjectIDs.Order1);
      DomainObjectCollection originalOrderItems = order.GetOriginalRelatedObjects ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");
      OrderItem orderItem = OrderItem.NewObject (order);

      ClientTransactionMock.Commit ();

      Assert.AreSame (originalOrderItems, order.GetOriginalRelatedObjects ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"));
      Assert.IsTrue (order.GetOriginalRelatedObjects ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems").IsReadOnly);
    }
  }
}
