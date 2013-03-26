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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Relations
{
  [TestFixture]
  public class LoadingRelationsWithInvalidMandatoryRelationsTest : ClientTransactionBaseTest
  {
    [Test]
    public void LoadingMandatoryCollectionEndPoint_WithNoRelatedObjects_Throws ()
    {
      var order = DomainObjectIDs.OrderWithoutOrderItem.GetObject<Order>();

      Assert.That (
          () => order.OrderItems.EnsureDataComplete (), 
          Throws.TypeOf<PersistenceException> ().With.Message.EqualTo (
              "Collection for mandatory relation 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem:"
              + "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order->"
              + "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' (property: "
              + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems', "
              + "object: 'Order|f4016f41-f4e4-429e-b8d1-659c8c480a67|System.Guid') contains no items."));
    }

    [Test]
    public void LoadingMandatoryVirtualObjectEndPoint_WithNoRelatedObject_Throws ()
    {
      var partner = DomainObjectIDs.PartnerWithoutCeo.GetObject<Partner> ();

      Assert.That (
          () => partner.Ceo,
          Throws.TypeOf<PersistenceException> ().With.Message.EqualTo (
              "Cannot load related DataContainer of object 'Partner|a65b123a-6e17-498e-a28e-946217c0ae30|System.Guid' over mandatory relation "
              + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Ceo:Remotion.Data.UnitTests.DomainObjects.TestDomain.Ceo.Company->"
              + "Remotion.Data.UnitTests.DomainObjects.TestDomain.Company.Ceo'."));
    }

    [Test]
    public void LoadingMandatoryRealObjectEndPoint_WithNullValue_DoesNotThrow ()
    {
      // Note: This test documents current behavior, not necessarily desired behavior.
      OrderItem orderItemWithoutOrder = null;
      Assert.That (() => orderItemWithoutOrder = DomainObjectIDs.OrderItemWithoutOrder.GetObject<OrderItem>(), Throws.Nothing);

      Assert.That (orderItemWithoutOrder.Order, Is.Null);
    }
  }
}