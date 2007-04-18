using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class DeleteDomainObjectWithLazyLoadTest : ClientTransactionBaseTest
  {
    [Test]
    public void DomainObjectWithOneToOneRelationAndNonVirtualProperty ()
    {
      OrderTicket orderTicket = DomainObject.GetObject<OrderTicket> (DomainObjectIDs.OrderTicket1);
      orderTicket.Delete ();

      Order order = DomainObject.GetObject<Order> (DomainObjectIDs.Order1);

      Assert.IsNull (orderTicket.Order);
      Assert.IsNull (order.OrderTicket);
      Assert.IsNull (orderTicket.DataContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"]);
      Assert.AreEqual (StateType.Changed, order.State);
      Assert.AreEqual (StateType.Unchanged, order.DataContainer.State);
    }

    [Test]
    public void DomainObjectWithOneToOneRelationAndNonVirtualNullProperty ()
    {
      Computer computerWithoutEmployee = DomainObject.GetObject<Computer> (DomainObjectIDs.Computer4);
      computerWithoutEmployee.Delete ();

      Assert.IsNull (computerWithoutEmployee.Employee);
      Assert.IsNull (computerWithoutEmployee.DataContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"]);
    }

    [Test]
    public void DomainObjectWithOneToOneRelationAndVirtualProperty ()
    {
      Order order = DomainObject.GetObject<Order> (DomainObjectIDs.Order1);
      order.Delete ();

      OrderTicket orderTicket = DomainObject.GetObject<OrderTicket> (DomainObjectIDs.OrderTicket1);

      Assert.IsNull (orderTicket.Order);
      Assert.IsNull (order.OrderTicket);
      Assert.IsNull (orderTicket.DataContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"]);
      Assert.AreEqual (StateType.Changed, orderTicket.DataContainer.State);
    }

    [Test]
    public void DomainObjectWithOneToOneRelationAndVirtualNullProperty ()
    {
      Employee employeeWithoutComputer = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee1);
      employeeWithoutComputer.Delete ();

      Assert.IsNull (employeeWithoutComputer.Computer);
    }

    [Test]
    public void DomainObjectWithOneToManyRelation ()
    {
      Employee supervisor = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee1);
      supervisor.Delete ();

      Employee subordinate1 = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee4);
      Employee subordinate2 = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee5);

      Assert.AreEqual (0, supervisor.Subordinates.Count);
      Assert.IsNull (subordinate1.Supervisor);
      Assert.IsNull (subordinate2.Supervisor);
      Assert.IsNull (subordinate1.DataContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor"]);
      Assert.IsNull (subordinate2.DataContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor"]);
      Assert.AreEqual (StateType.Changed, subordinate1.DataContainer.State);
      Assert.AreEqual (StateType.Changed, subordinate2.DataContainer.State);
    }

    [Test]
    public void DomainObjectWithEmptyOneToManyRelation ()
    {
      Employee supervisor = DomainObject.GetObject<Employee> (DomainObjectIDs.Employee3);
      supervisor.Delete ();

      Assert.AreEqual (0, supervisor.Subordinates.Count);
    }

    [Test]
    public void DomainObjectWithManyToOneRelation ()
    {
      OrderItem orderItem = DomainObject.GetObject<OrderItem> (DomainObjectIDs.OrderItem1);
      orderItem.Delete ();

      Order order = DomainObject.GetObject<Order> (DomainObjectIDs.Order1);

      Assert.IsNull (orderItem.Order);
      Assert.AreEqual (1, order.OrderItems.Count);
      Assert.IsFalse (order.OrderItems.Contains (orderItem.ID));
      Assert.IsNull (orderItem.DataContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order"]);
      Assert.AreEqual (StateType.Changed, order.State);
      Assert.AreEqual (StateType.Unchanged, order.DataContainer.State);
    }
  }
}
