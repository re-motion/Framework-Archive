// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 

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

      _order1 = ActiveSubTransaction.ExecuteInScope (() => DomainObjectIDs.Order1.GetObject<Order> ());
      _orderItem1 = ActiveSubTransaction.ExecuteInScope (() => DomainObjectIDs.OrderItem1.GetObject<OrderItem>());
      _orderItem2 = ActiveSubTransaction.ExecuteInScope (() => DomainObjectIDs.OrderItem2.GetObject<OrderItem>());

      _orderItem3 = ActiveSubTransaction.ExecuteInScope (() => DomainObjectIDs.OrderItem3.GetObject<OrderItem>());
      _orderItem4 = ActiveSubTransaction.ExecuteInScope (() => DomainObjectIDs.OrderItem4.GetObject<OrderItem>());

      ActiveSubTransaction.ExecuteInScope (() => _order1.OrderItems.Add (_orderItem3));
      ActiveSubTransaction.ExecuteInScope (() => _orderItem4.Order.EnsureDataAvailable ());
    }

    [Test]
    public void RelationSetInInactiveRootTransaction_IsForbidden ()
    {
      CheckPropertyEquivalent (InactiveRootTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2 }, new[] { _orderItem1, _orderItem2 });
      CheckPropertyEquivalent (InactiveMiddleTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2 }, new[] { _orderItem1, _orderItem2 });
      CheckPropertyEquivalent (ActiveSubTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2, _orderItem3 }, new[] { _orderItem1, _orderItem2 });

      CheckForbidden (() => InactiveRootTransaction.ExecuteInScope (() => _order1.OrderItems.Add (_orderItem4)), "RelationChanging");
      CheckForbidden (() => InactiveRootTransaction.ExecuteInScope (() => _order1.OrderItems.Insert (0, _orderItem4)), "RelationChanging");
      CheckForbidden (() => InactiveRootTransaction.ExecuteInScope (() => _order1.OrderItems.Remove (_orderItem1)), "RelationChanging");
      CheckForbidden (() => InactiveRootTransaction.ExecuteInScope (() => _order1.OrderItems[0] = _orderItem4), "RelationChanging");
      CheckForbidden (() => InactiveRootTransaction.ExecuteInScope (() => _order1.OrderItems.Clear ()), "RelationChanging");
      CheckForbidden (() => InactiveRootTransaction.ExecuteInScope (() => _order1.OrderItems = new ObjectList<OrderItem>()), "RelationChanging");

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

      CheckForbidden (() => InactiveMiddleTransaction.ExecuteInScope (() => _order1.OrderItems.Add (_orderItem4)), "RelationChanging");
      CheckForbidden (() => InactiveMiddleTransaction.ExecuteInScope (() => _order1.OrderItems.Insert (0, _orderItem4)), "RelationChanging");
      CheckForbidden (() => InactiveMiddleTransaction.ExecuteInScope (() => _order1.OrderItems.Remove (_orderItem1)), "RelationChanging");
      CheckForbidden (() => InactiveMiddleTransaction.ExecuteInScope (() => _order1.OrderItems[0] = _orderItem4), "RelationChanging");
      CheckForbidden (() => InactiveMiddleTransaction.ExecuteInScope (() => _order1.OrderItems.Clear ()), "RelationChanging");
      CheckForbidden (() => InactiveMiddleTransaction.ExecuteInScope (() => _order1.OrderItems = new ObjectList<OrderItem> ()), "RelationChanging");

      CheckPropertyEquivalent (InactiveRootTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2 }, new[] { _orderItem1, _orderItem2 });
      CheckPropertyEquivalent (InactiveMiddleTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2 }, new[] { _orderItem1, _orderItem2 });
      CheckPropertyEquivalent (ActiveSubTransaction, _order1, o => o.OrderItems, new[] { _orderItem1, _orderItem2, _orderItem3 }, new[] { _orderItem1, _orderItem2 });
    }
  }
}