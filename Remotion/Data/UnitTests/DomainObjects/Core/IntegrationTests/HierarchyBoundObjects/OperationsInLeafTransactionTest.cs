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
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Reflection;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.HierarchyBoundObjects
{
  [TestFixture]
  [Ignore ("TODO 5447")]
  public class OperationsInLeafTransactionTest : HierarchyBoundObjectsTestBase
  {
    private ClientTransaction _rootTransaction;
    private ClientTransaction _middleTransaction;
    private ClientTransaction _leafTransaction;
    private Order _order1LoadedInMiddleTransaction;

    public override void SetUp ()
    {
      base.SetUp ();

      _rootTransaction = ClientTransaction.CreateRootTransaction();
      _middleTransaction = _rootTransaction.CreateSubTransaction ();
      _leafTransaction = _middleTransaction.CreateSubTransaction ();

      _order1LoadedInMiddleTransaction = DomainObjectIDs.Order1.GetObject<Order> (_middleTransaction);
    }

    [Test]
    public void DefaultTransactionContext_IsActiveLeafTransaction ()
    {
      Assert.That (_order1LoadedInMiddleTransaction.RootTransaction, Is.SameAs (_rootTransaction));
      Assert.That (_order1LoadedInMiddleTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (_leafTransaction));
    }

    [Test]
    public void AccessingPropertiesAndState_AffectsActiveLeafTransaction ()
    {
      Assert.That (_order1LoadedInMiddleTransaction.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (_order1LoadedInMiddleTransaction.OrderNumber, Is.EqualTo (1));
      Assert.That (_order1LoadedInMiddleTransaction.OrderItems, Has.Length.EqualTo (2));

      Assert.That (_leafTransaction.HasChanged (), Is.False);

      _order1LoadedInMiddleTransaction.OrderNumber = 2;
      _order1LoadedInMiddleTransaction.OrderItems.Clear();
      
      Assert.That (_order1LoadedInMiddleTransaction.State, Is.EqualTo (StateType.Changed));
      Assert.That (_order1LoadedInMiddleTransaction.OrderNumber, Is.EqualTo (2));
      Assert.That (_order1LoadedInMiddleTransaction.OrderItems, Is.Empty);

      Assert.That (_rootTransaction.HasChanged (), Is.False);
      Assert.That (_middleTransaction.HasChanged (), Is.False);
      Assert.That (_leafTransaction.HasChanged (), Is.True);
      Assert.That (GetStateFromTransaction (_order1LoadedInMiddleTransaction, _leafTransaction), Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void PropertyIndexer_AffectsActiveLeafTransaction ()
    {
      Assert.That (_order1LoadedInMiddleTransaction.Properties[typeof (Order), "OrderNumber"].GetValue<int> (), Is.EqualTo (1));
      Assert.That (GetStateFromTransaction (_order1LoadedInMiddleTransaction, _leafTransaction), Is.EqualTo (StateType.Unchanged));

      _order1LoadedInMiddleTransaction.Properties[typeof (Order), "OrderNumber"].SetValue (2);

      Assert.That (_order1LoadedInMiddleTransaction.Properties[typeof (Order), "OrderNumber"].GetValue<int> (), Is.EqualTo (2));
      Assert.That (GetStateFromTransaction (_order1LoadedInMiddleTransaction, _leafTransaction), Is.EqualTo (StateType.Changed));
    }

    // Note: No Timestamp check here - the value is always the same as in the root tx, so we can't test that it is retrieved from the leaf tx.

    [Test]
    public void EnsureDataAvailable_AffectsActiveLeafTransaction ()
    {
      var order = DomainObjectIDs.Order1.GetObjectReference<Order> (_middleTransaction);

      Assert.That (GetStateFromTransaction (order, _leafTransaction), Is.EqualTo (StateType.NotLoadedYet));

      order.EnsureDataAvailable ();

      Assert.That (GetStateFromTransaction (order, _leafTransaction), Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void TryEnsureDataAvailable_AffectsActiveLeafTransaction ()
    {
      var order = DomainObjectIDs.Order1.GetObjectReference<Order> (_middleTransaction);

      Assert.That (GetStateFromTransaction (order, _leafTransaction), Is.EqualTo (StateType.NotLoadedYet));

      order.TryEnsureDataAvailable ();

      Assert.That (GetStateFromTransaction (order, _leafTransaction), Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void Delete_AffectsActiveLeafTransaction ()
    {
      Assert.That (GetStateFromTransaction (_order1LoadedInMiddleTransaction, _leafTransaction), Is.EqualTo (StateType.Unchanged));

      _order1LoadedInMiddleTransaction.Delete ();

      Assert.That (GetStateFromTransaction (_order1LoadedInMiddleTransaction, _leafTransaction), Is.EqualTo (StateType.Deleted));
    }

    [Test]
    public void IsInvalid_AffectsActiveLeafTransaction ()
    {
      var order = (Order) LifetimeService.NewObject (_leafTransaction, typeof (Order), ParamList.Empty);

      Assert.That (GetStateFromTransaction (order, _leafTransaction), Is.EqualTo (StateType.New));
      Assert.That (order.IsInvalid, Is.False);

      order.Delete ();

      Assert.That (GetStateFromTransaction (order, _leafTransaction), Is.EqualTo (StateType.Invalid));
      Assert.That (order.IsInvalid, Is.True);
    }

    [Test]
    public void RegisterForCommit_AffectsActiveLeafTransaction ()
    {
      Assert.That (GetStateFromTransaction (_order1LoadedInMiddleTransaction, _leafTransaction), Is.EqualTo (StateType.Unchanged));

      _order1LoadedInMiddleTransaction.RegisterForCommit ();

      Assert.That (GetStateFromTransaction (_order1LoadedInMiddleTransaction, _leafTransaction), Is.EqualTo (StateType.Changed));
    }
  }
}