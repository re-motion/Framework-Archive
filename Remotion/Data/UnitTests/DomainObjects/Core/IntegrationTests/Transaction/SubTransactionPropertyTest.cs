// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction
{
  [TestFixture]
  public class SubTransactionPropertyTest : ClientTransactionBaseTest
  {
    [Test]
    public void StateChangesInsideSubTransaction ()
    {
      Order newOrder = Order.NewObject ();

      Assert.That (newOrder.State, Is.EqualTo (StateType.New));

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (newOrder.State, Is.EqualTo (StateType.NotLoadedYet));

        newOrder.OrderNumber = 7;

        Assert.That (newOrder.State, Is.EqualTo (StateType.Changed));
        Assert.That (
            newOrder.Properties[typeof (Order) + ".OrderNumber"].GetOriginalValue<int> (),
            Is.Not.EqualTo (
                newOrder.Properties[typeof (Order) + ".OrderNumber"].GetValue<int> ()));
      }
    }

    [Test]
    public void SubTransactionHasSamePropertyValuessAsParent_ForPersistentProperties ()
    {
      Order newUnchangedOrder = Order.NewObject ();
      int newUnchangedOrderNumber = newUnchangedOrder.OrderNumber;

      Order newChangedOrder = Order.NewObject ();
      newChangedOrder.OrderNumber = 4711;

      Order loadedUnchangedOrder = Order.GetObject (DomainObjectIDs.Order1);
      int loadedUnchangedOrderNumber = loadedUnchangedOrder.OrderNumber;

      Order loadedChangedOrder = Order.GetObject (DomainObjectIDs.Order2);
      loadedChangedOrder.OrderNumber = 13;

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (Order.GetObject (DomainObjectIDs.Order1), Is.SameAs (loadedUnchangedOrder));
        Assert.That (Order.GetObject (DomainObjectIDs.Order2), Is.SameAs (loadedChangedOrder));

        Assert.That (newUnchangedOrder.OrderNumber, Is.EqualTo (newUnchangedOrderNumber));
        Assert.That (newChangedOrder.OrderNumber, Is.EqualTo (4711));
        Assert.That (loadedUnchangedOrder.OrderNumber, Is.EqualTo (loadedUnchangedOrderNumber));
        Assert.That (loadedChangedOrder.OrderNumber, Is.EqualTo (13));
      }
    }

    [Test]
    public void SubTransactionHasSamePropertyValuessAsParent_ForTransactionProperties ()
    {
      OrderTicket newUnchangedOrderTicket = OrderTicket.NewObject ();
      int newUnchangedInt32TransactionProperty = newUnchangedOrderTicket.Int32TransactionProperty;

      OrderTicket newChangedOrderTicket = OrderTicket.NewObject ();
      newChangedOrderTicket.Int32TransactionProperty = 4711;

      OrderTicket loadedUnchangedOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      int loadedUnchangedInt32TransactionProperty = loadedUnchangedOrderTicket.Int32TransactionProperty;

      OrderTicket loadedChangedOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);
      loadedChangedOrderTicket.Int32TransactionProperty = 13;

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (OrderTicket.GetObject (DomainObjectIDs.OrderTicket1), Is.SameAs (loadedUnchangedOrderTicket));
        Assert.That (OrderTicket.GetObject (DomainObjectIDs.OrderTicket2), Is.SameAs (loadedChangedOrderTicket));

        Assert.That (newUnchangedOrderTicket.Int32TransactionProperty, Is.EqualTo (newUnchangedInt32TransactionProperty));
        Assert.That (newChangedOrderTicket.Int32TransactionProperty, Is.EqualTo (4711));
        Assert.That (loadedUnchangedOrderTicket.Int32TransactionProperty, Is.EqualTo (loadedUnchangedInt32TransactionProperty));
        Assert.That (loadedChangedOrderTicket.Int32TransactionProperty, Is.EqualTo (13));
      }
    }

    [Test]
    public void PropertyValueChangedAreNotPropagatedToParent ()
    {
      Order newChangedOrder = Order.NewObject ();
      newChangedOrder.OrderNumber = 4711;

      Order loadedChangedOrder = Order.GetObject (DomainObjectIDs.Order2);
      loadedChangedOrder.OrderNumber = 13;

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        newChangedOrder.OrderNumber = 17;
        loadedChangedOrder.OrderNumber = 4;

        using (ClientTransactionMock.EnterDiscardingScope ())
        {
          Assert.That (newChangedOrder.OrderNumber, Is.EqualTo (4711));
          Assert.That (loadedChangedOrder.OrderNumber, Is.EqualTo (13));
        }
      }
    }
  }
}