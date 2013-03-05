using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction.InactiveTransactions.ForbiddenOperations
{
  [TestFixture]
  public class VirtualCollectionRelationPropertyWriteTest : InactiveTransactionsTestBase
  {
    private Order _order1;
    private OrderItem _orderItem1;
    private OrderItem _orderItem2;
    private OrderItem _orderItem3;
    private OrderItem _orderItem4;

    public override void SetUp ()
    {
      base.SetUp ();

      _order1 = ExecuteInActiveSubTransaction (() => DomainObjectIDs.Order1.GetObject<Order> ());
      _orderItem1 = ExecuteInActiveSubTransaction (() => DomainObjectIDs.OrderItem1.GetObject<OrderItem>());
      _orderItem2 = ExecuteInActiveSubTransaction (() => DomainObjectIDs.OrderItem2.GetObject<OrderItem>());

      _orderItem3 = ExecuteInActiveSubTransaction (() => DomainObjectIDs.OrderItem3.GetObject<OrderItem>());
      _orderItem4 = ExecuteInActiveSubTransaction (() => DomainObjectIDs.OrderItem4.GetObject<OrderItem>());

      ExecuteInActiveSubTransaction (() => _order1.OrderItems.Add (_orderItem3));
      ExecuteInActiveSubTransaction (() => _orderItem4.Order.EnsureDataAvailable ());
    }

    [Test]
    public void RelationSetInInactiveRootTransaction_IsForbidden ()
    {
      CheckPropertyEquivalent (InactiveRootTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2 }, new[] { _orderItem1, _orderItem2 });
      CheckPropertyEquivalent (InactiveMiddleTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2 }, new[] { _orderItem1, _orderItem2 });
      CheckPropertyEquivalent (ActiveSubTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2, _orderItem3 }, new[] { _orderItem1, _orderItem2 });

      CheckForbidden (() => ExecuteInInactiveRootTransaction (() => _order1.OrderItems.Add (_orderItem4)), "RelationChanging");
      CheckForbidden (() => ExecuteInInactiveRootTransaction (() => _order1.OrderItems.Insert (0, _orderItem4)), "RelationChanging");
      CheckForbidden (() => ExecuteInInactiveRootTransaction (() => _order1.OrderItems.Remove (_orderItem1)), "RelationChanging");
      CheckForbidden (() => ExecuteInInactiveRootTransaction (() => _order1.OrderItems[0] = _orderItem4), "RelationChanging");
      CheckForbidden (() => ExecuteInInactiveRootTransaction (() => _order1.OrderItems.Clear ()), "RelationChanging");
      CheckForbidden (() => ExecuteInInactiveRootTransaction (() => _order1.OrderItems = new ObjectList<OrderItem>()), "RelationChanging");

      CheckPropertyEquivalent (InactiveRootTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2 }, new[] { _orderItem1, _orderItem2 });
      CheckPropertyEquivalent (InactiveMiddleTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2 }, new[] { _orderItem1, _orderItem2 });
      CheckPropertyEquivalent (ActiveSubTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2, _orderItem3 }, new[] { _orderItem1, _orderItem2 });
    }

    [Test]
    public void RelationSetInInactiveMiddleTransaction_IsForbidden ()
    {
      CheckPropertyEquivalent (InactiveRootTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2 }, new[] { _orderItem1, _orderItem2 });
      CheckPropertyEquivalent (InactiveMiddleTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2 }, new[] { _orderItem1, _orderItem2 });
      CheckPropertyEquivalent (ActiveSubTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2, _orderItem3 }, new[] { _orderItem1, _orderItem2 });

      CheckForbidden (() => ExecuteInInactiveMiddleTransaction (() => _order1.OrderItems.Add (_orderItem4)), "RelationChanging");
      CheckForbidden (() => ExecuteInInactiveMiddleTransaction (() => _order1.OrderItems.Insert (0, _orderItem4)), "RelationChanging");
      CheckForbidden (() => ExecuteInInactiveMiddleTransaction (() => _order1.OrderItems.Remove (_orderItem1)), "RelationChanging");
      CheckForbidden (() => ExecuteInInactiveMiddleTransaction (() => _order1.OrderItems[0] = _orderItem4), "RelationChanging");
      CheckForbidden (() => ExecuteInInactiveMiddleTransaction (() => _order1.OrderItems.Clear ()), "RelationChanging");
      CheckForbidden (() => ExecuteInInactiveMiddleTransaction (() => _order1.OrderItems = new ObjectList<OrderItem> ()), "RelationChanging");

      CheckPropertyEquivalent (InactiveRootTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2 }, new[] { _orderItem1, _orderItem2 });
      CheckPropertyEquivalent (InactiveMiddleTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2 }, new[] { _orderItem1, _orderItem2 });
      CheckPropertyEquivalent (ActiveSubTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2, _orderItem3 }, new[] { _orderItem1, _orderItem2 });
    }
  }
}