using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using NUnit.Framework.SyntaxHelpers;
using System.Collections.Generic;
using Rubicon.Collections;

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
    public void GetAllRelatedObjects_DoesNotContainDuplicates ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      List<DomainObject> relatedObjects = new List<DomainObject> (order.GetAllRelatedObjects ());
      Assert.That (relatedObjects, Is.EquivalentTo (new Set<DomainObject> (relatedObjects)));
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

    private Order GetGraph ()
    {
      Order root = Order.NewObject ();
      root.Official = Official.NewObject ();
      root.OrderTicket = OrderTicket.NewObject ();
      root.OrderItems.Add (OrderItem.NewObject ());
      root.OrderItems.Add (OrderItem.NewObject ());
      root.Customer = Customer.NewObject ();
      root.Customer.Ceo = Ceo.NewObject ();
      return root;
    }

    [Test]
    public void GetFlattenedRelatedObjectGraph_ContainsRoot ()
    {
      Order order = GetGraph();
      Set<DomainObject> graph = order.GetFlattenedRelatedObjectGraph();

      Assert.That (graph, List.Contains (order));
    }

    [Test]
    public void TraverseRelatedObjectGraph_ContainsRelatedObjects ()
    {
      Order order = GetGraph();
      Set<DomainObject> graph = order.GetFlattenedRelatedObjectGraph();

      foreach (DomainObject relatedObject in order.GetAllRelatedObjects())
        Assert.That (graph, List.Contains (relatedObject));
    }

    [Test]
    public void TraverseRelatedObjectGraph_ContainsIndirectRelatedObjects ()
    {
      Order order = GetGraph();
      Set<DomainObject> graph = order.GetFlattenedRelatedObjectGraph();

      Assert.That (graph, List.Contains (order.Customer.Ceo));
    }
  }
}