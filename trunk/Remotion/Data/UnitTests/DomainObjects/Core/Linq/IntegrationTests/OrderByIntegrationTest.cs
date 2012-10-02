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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq.IntegrationTests
{
  [TestFixture]
  public class OrderByIntegrationTest : IntegrationTestBase
  {
    [Test]
    public void QueryWithSimpleOrderBy ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<Order>()
          orderby o.OrderNumber
          select o;
      CheckQueryResult (
          query,
          DomainObjectIDs.Order1,
          DomainObjectIDs.OrderWithoutOrderItem,
          DomainObjectIDs.Order2,
          DomainObjectIDs.Order3,
          DomainObjectIDs.Order4,
          DomainObjectIDs.InvalidOrder);
    }

    [Test]
    public void AutomaticOrderByHandlingInSubStatements_InFromClause_WithoutTopExpression ()
    {
      CheckQueryResult (
          from o in QueryFactory.CreateLinqQuery<Order>()
          from i in
              (from si in QueryFactory.CreateLinqQuery<OrderItem>() where si.Order == o orderby si.Product select si)
          select i,
          DomainObjectIDs.OrderItem1,
          DomainObjectIDs.OrderItem2,
          DomainObjectIDs.OrderItem3,
          DomainObjectIDs.OrderItem4,
          DomainObjectIDs.OrderItem5
          );
    }

    [Test]
    public void AutomaticOrderByHandlingInSubStatements_InFromClause_WithTopExpression ()
    {
      CheckQueryResult (
          from o in QueryFactory.CreateLinqQuery<Order>()
          from c in
              (from sc in QueryFactory.CreateLinqQuery<OrderItem>() where sc.Order == o orderby sc.Product select sc).Take (10)
          select c,
          DomainObjectIDs.OrderItem1,
          DomainObjectIDs.OrderItem2,
          DomainObjectIDs.OrderItem3,
          DomainObjectIDs.OrderItem4,
          DomainObjectIDs.OrderItem5
          );
    }

    [Test]
    public void AutomaticOrderByHandlingInSubStatements_InWhereClause_WithTopExpression ()
    {
      CheckQueryResult (
          from o in QueryFactory.CreateLinqQuery<Order>()
          where
              (from so in QueryFactory.CreateLinqQuery<Order>() orderby so.OrderNumber where so.ID == DomainObjectIDs.Order1 select so.OrderNumber).
                  Single() != null
          select o,
          DomainObjectIDs.Order1,
          DomainObjectIDs.Order2,
          DomainObjectIDs.Order3,
          DomainObjectIDs.Order4,
          DomainObjectIDs.OrderWithoutOrderItem,
          DomainObjectIDs.InvalidOrder);
    }

    [Test]
    public void AutomaticOrderByHandlingInSubStatements_InWhereClause_WithoutTopExpression ()
    {
      CheckQueryResult (
          from o in QueryFactory.CreateLinqQuery<Order>()
          where (from so in QueryFactory.CreateLinqQuery<Order> () orderby so.OrderNumber where so.ID == DomainObjectIDs.Order1 select so.OrderNumber).
                  Count() > 0
          select o,
          DomainObjectIDs.Order1,
          DomainObjectIDs.OrderWithoutOrderItem,
          DomainObjectIDs.Order2,
          DomainObjectIDs.Order3,
          DomainObjectIDs.Order4,
          DomainObjectIDs.InvalidOrder);
    }
  }
}