using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
[TestFixture]  
public class RollbackDomainObjectTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public RollbackDomainObjectTest ()
  {
  }

  // methods and properties
  [Test]
  public void RollbackPropertyChange ()
  {
    Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);
    customer.Name = "Arthur Dent";

    Assert.AreEqual (StateType.Changed, customer.State);

    ClientTransaction.Current.Rollback ();
    
    Assert.AreEqual (StateType.Unchanged, customer.State);
    Assert.AreEqual ("Kunde 1", customer.Name);
  }

  [Test]
  public void RollbackOneToOneRelationChange ()
  {
    Order order = Order.GetObject (DomainObjectIDs.Order1);
    OrderTicket oldOrderTicket = order.OrderTicket;
    OrderTicket newOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);
    Order oldOrderOfNewOrderTicket = newOrderTicket.Order;    

    order.OrderTicket = newOrderTicket;
    oldOrderOfNewOrderTicket.OrderTicket = oldOrderTicket;
    
    ClientTransaction.Current.Rollback ();    

    Assert.AreSame (oldOrderTicket, order.OrderTicket);
    Assert.AreSame (order, oldOrderTicket.Order);
    Assert.AreSame (newOrderTicket, oldOrderOfNewOrderTicket.OrderTicket);
    Assert.AreSame (oldOrderOfNewOrderTicket, newOrderTicket.Order);
  }

  [Test]
  public void RollbackOneToManyRelationChange ()
  {
    Customer customer1 = Customer.GetObject (DomainObjectIDs.Customer1);
    Customer customer2 = Customer.GetObject (DomainObjectIDs.Customer2);

    Order order = customer1.Orders[DomainObjectIDs.Order1];

    order.Customer = customer2;

    ClientTransaction.Current.Rollback ();

    Assert.IsNotNull (customer1.Orders[order.ID]);
    Assert.IsNull (customer2.Orders[order.ID]);
    Assert.AreSame (customer1, order.Customer);

    Assert.AreEqual (2, customer1.Orders.Count);
    Assert.AreEqual (0, customer2.Orders.Count);
  }

  [Test]
  public void RollbackDeletion ()
  {
    Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
    computer.Delete ();

    ClientTransaction.Current.Rollback ();

    Computer computerAfterRollback = Computer.GetObject (DomainObjectIDs.Computer4);
    Assert.AreSame (computer, computerAfterRollback);
    Assert.AreEqual (StateType.Unchanged, computer.State);
  }

  [Test]
  public void RollbackDeletionAndPropertyChange ()
  {
    Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
    computer.SerialNumber = "1111111111111";

    Assert.AreEqual ("63457-kol-34", computer.DataContainer.PropertyValues["SerialNumber"].OriginalValue);
    Assert.AreEqual ("1111111111111", computer.DataContainer.PropertyValues["SerialNumber"].Value);

    computer.Delete ();
    ClientTransaction.Current.Rollback ();

    Assert.AreEqual ("63457-kol-34", computer.DataContainer.PropertyValues["SerialNumber"].OriginalValue);
    Assert.AreEqual ("63457-kol-34", computer.DataContainer.PropertyValues["SerialNumber"].Value);
    Assert.IsFalse (computer.DataContainer.PropertyValues["SerialNumber"].HasChanged);
  }

  [Test]
  public void RollbackDeletionWithRelationChange ()
  {
    Order order = Order.GetObject (DomainObjectIDs.Order1);

    OrderTicket oldOrderTicket = order.OrderTicket;
    DomainObjectCollection oldOrderItems = order.GetOriginalRelatedObjects ("OrderItems");
    Customer oldCustomer = order.Customer;
    Official oldOfficial = order.Official;

    order.Delete ();

    Assert.IsNull (order.OrderTicket);
    Assert.AreEqual (0, order.OrderItems.Count);
    Assert.IsNull (order.Customer);
    Assert.IsNull (order.Official);

    ClientTransaction.Current.Rollback ();

    Assert.AreSame (oldOrderTicket, order.OrderTicket);
    Assert.AreEqual (oldOrderItems.Count, order.OrderItems.Count);
    Assert.AreSame (oldOrderItems[DomainObjectIDs.OrderItem1], order.OrderItems[DomainObjectIDs.OrderItem1]);
    Assert.AreSame (oldOrderItems[DomainObjectIDs.OrderItem2], order.OrderItems[DomainObjectIDs.OrderItem2]);
    Assert.AreSame (oldCustomer, order.Customer);
    Assert.AreSame (oldOfficial, order.Official);
  }
}
}
