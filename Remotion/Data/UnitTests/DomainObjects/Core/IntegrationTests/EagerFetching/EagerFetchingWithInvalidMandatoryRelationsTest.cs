﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.DomainObjects.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.EagerFetching
{
  [TestFixture]
  public class EagerFetchingWithInvalidMandatoryRelationsTest : ClientTransactionBaseTest
  {
    [Test]
    [Ignore ("TODO 3998")]
    public void FetchingMandatoryCollectionEndPoint_WithNoRelatedObjects_Throws ()
    {
      var query = QueryFactory.CreateLinqQuery<Order>().Where (o => o.ID == DomainObjectIDs.OrderWithoutOrderItem).FetchMany (o => o.OrderItems);

      Assert.That (
          () => query.ToArray(),
          Throws.TypeOf<PersistenceException> ().With.Message.EqualTo (
              "Collection for mandatory relation property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' "
              + "on object 'Order|f4016f41-f4e4-429e-b8d1-659c8c480a67|System.Guid' contains no items."));
    }

    [Test]
    [Ignore ("TODO 3998")]
    public void FetchingMandatoryVirtualObjectEndPoint_WithNoRelatedObject_Throws ()
    {
      var query = QueryFactory.CreateLinqQuery<Partner> ().Where (o => o.ID == DomainObjectIDs.PartnerWithoutCeo).FetchOne (p => p.Ceo);

      Assert.That (
          () => query.ToArray(),
          Throws.TypeOf<PersistenceException> ().With.Message.EqualTo (
              "Mandatory relation property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Company.Ceo' on object "
              + "'Partner|a65b123a-6e17-498e-a28e-946217c0ae30|System.Guid' contains no item."));
    }
    
    [Test]
    public void FetchingMandatoryRealObjectEndPoint_WithNullValue_DoesNotThrow ()
    {
      var query = QueryFactory.CreateLinqQuery<OrderItem> ().Where (o => o.ID == DomainObjectIDs.OrderItemWithoutOrder).FetchOne (oi => oi.Order);

      // Note: This test documents current behavior, not necessarily desired behavior.
      OrderItem orderItemWithoutOrder = null;
      Assert.That (() => orderItemWithoutOrder = query.ToArray().Single(), Throws.Nothing);

      Assert.That (orderItemWithoutOrder.Order, Is.Null);
    }
  }
}