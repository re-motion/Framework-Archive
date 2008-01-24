using System;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using NUnit.Framework.SyntaxHelpers;
using System.Collections.Generic;
using Rubicon.Collections;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class GraphTest : ClientTransactionBaseTest
  {
   

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
      Set<DomainObject> graph = order.GetGraphTraverser(FullGraphTraversalStrategy.Instance).GetFlattenedRelatedObjectGraph ();

      Assert.That (graph, List.Contains (order));
    }

    [Test]
    public void GetFlattenedRelatedObjectGraph_ContainsRelatedObjects ()
    {
      Order order = GetGraph();
      Set<DomainObject> graph = order.GetGraphTraverser(FullGraphTraversalStrategy.Instance).GetFlattenedRelatedObjectGraph ();

      foreach (DomainObject relatedObject in order.Properties.GetAllRelatedObjects())
        Assert.That (graph, List.Contains (relatedObject));
    }

    [Test]
    public void GetFlattenedRelatedObjectGraph_ContainsIndirectRelatedObjects ()
    {
      Order order = GetGraph();
      Set<DomainObject> graph = order.GetGraphTraverser(FullGraphTraversalStrategy.Instance).GetFlattenedRelatedObjectGraph ();

      Assert.That (graph, List.Contains (order.Customer.Ceo));
    }

    [Test]
    public void GetFlattenedRelatedObjectGraph_WithTraversalFilter_FollowLink ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Set<DomainObject> graph = order.GetGraphTraverser(new TestTraversalStrategy(true)).GetFlattenedRelatedObjectGraph ();

      Set<DomainObject> expected = new Set<DomainObject> (
          order,
          RepositoryAccessor.GetObject (DomainObjectIDs.OrderTicket1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.OrderItem1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.OrderItem2, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.Customer1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.Official1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.IndustrialSector1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.Partner1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.PartnerWithoutCeo, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.Supplier1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.Distributor2, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.Person1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.Person7, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.Person3, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.Person6, false));

      Assert.That (graph, Is.EquivalentTo(expected));
    }

    [Test]
    public void GetFlattenedRelatedObjectGraph_WithTraversalFilter_FollowLink_IncludeObject ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Set<DomainObject> graph = order.GetGraphTraverser(new TestTraversalStrategy (false)).GetFlattenedRelatedObjectGraph ();

      Set<DomainObject> expected = new Set<DomainObject> (
          order,
          RepositoryAccessor.GetObject (DomainObjectIDs.OrderTicket1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.OrderItem1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.OrderItem2, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.Customer1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.Official1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.IndustrialSector1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.Partner1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.PartnerWithoutCeo, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.Supplier1, false),
          RepositoryAccessor.GetObject (DomainObjectIDs.Distributor2, false));

      Assert.That (graph, Is.EquivalentTo (expected));
    }

    class TestTraversalStrategy : IGraphTraversalStrategy
    {
      private readonly bool _includePersons;

      public TestTraversalStrategy (bool includePersons)
      {
        _includePersons = includePersons;
      }

      public bool IncludeObject (DomainObject domainObject)
      {
        return _includePersons || !(domainObject is Person);
      }

      public bool FollowLink (DomainObject currentObject, PropertyAccessor linkProperty)
      {
        return !typeof (Ceo).IsAssignableFrom (linkProperty.PropertyType)
          && !typeof (Order).IsAssignableFrom (linkProperty.PropertyType)
          && !typeof (ObjectList<Order>).IsAssignableFrom (linkProperty.PropertyType);
      }
    }
  }
}