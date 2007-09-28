using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using NUnit.Framework.SyntaxHelpers;
using System.Collections.Generic;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class GraphTest : ClientTransactionBaseTest
  {
    [Test]
    public void GetAllRelatedObjects_DoesNotContainRoot ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      List<DomainObject> relatedObjects = new List<DomainObject>(order.GetAllRelatedObjects ());
      Assert.That (relatedObjects, List.Not.Contains (order));
    }

    [Test]
    public void GetAllRelatedObjects_DoesNotContainIndirectRelatedObjects ()
    {
      Ceo ceo = Ceo.GetObject (DomainObjectIDs.Ceo1);
      List<DomainObject> relatedObjects = new List<DomainObject> (ceo.GetAllRelatedObjects ());
      Assert.That (relatedObjects, List.Not.Contains (ceo.Company.IndustrialSector));
    }

    [Test]
    public void GetAllRelatedObjects_DoesNotContainNulls ()
    {
      Order order = Order.NewObject ();
      List<DomainObject> relatedObjects = new List<DomainObject> (order.GetAllRelatedObjects ());
      Assert.That (relatedObjects, List.Not.Contains (null));
    }

    [Test]
    public void GetAllRelatedObjects_ContainsSimpleRelatedObject ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      List<DomainObject> relatedObjects = new List<DomainObject> (order.GetAllRelatedObjects ());
      Assert.That (relatedObjects, List.Contains (order.Official));
      Assert.That (relatedObjects, List.Contains (order.OrderTicket));
    }

    [Test]
    public void GetAllRelatedObjects_ContainsSimpleRelatedObjectBothSides ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      List<DomainObject> relatedObjects = new List<DomainObject> (computer.GetAllRelatedObjects ());
      Assert.That (relatedObjects, List.Contains (computer.Employee));

      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);
      relatedObjects = new List<DomainObject> (employee.GetAllRelatedObjects ());
      Assert.That (relatedObjects, List.Contains (employee.Computer));

    }

    [Test]
    public void GetAllRelatedObjects_ContainsSimpleRelatedObjectUnidirectional ()
    {
      Client client = Client.GetObject (DomainObjectIDs.Client2);
      List<DomainObject> relatedObjects = new List<DomainObject> (client.GetAllRelatedObjects ());
      Assert.That (relatedObjects, List.Contains (client.ParentClient));
    }

    [Test]
    public void GetAllRelatedObjects_ContainsRelatedObjects ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      List<DomainObject> relatedObjects = new List<DomainObject> (order.GetAllRelatedObjects ());
      Assert.That (order.OrderItems, Is.SubsetOf (relatedObjects));
    }
  }
}