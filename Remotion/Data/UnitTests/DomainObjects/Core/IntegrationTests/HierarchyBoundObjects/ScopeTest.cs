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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.HierarchyBoundObjects
{
  [TestFixture]
  [Ignore ("TODO 5447")]
  public class ScopeTest : HierarchyBoundObjectsTestBase
  {
    private ClientTransaction _rootTransaction;
    private Order _order1LoadedInRootTransaction;

    public override void SetUp ()
    {
      base.SetUp ();

      _rootTransaction = ClientTransaction.CreateRootTransaction();
      _order1LoadedInRootTransaction = DomainObjectIDs.Order1.GetObject<Order> (_rootTransaction);
    }

    [Test]
    public void OpeningScopeForLeafTransaction_AffectsCurrentTransactiion ()
    {
      using (_rootTransaction.CreateSubTransaction ().EnterNonDiscardingScope ())
      {
        Assert.That (ClientTransaction.Current, Is.SameAs (_rootTransaction.LeafTransaction));
        ClientTransaction.Current.Discard ();
      }

      using (_rootTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (ClientTransaction.Current, Is.SameAs (_rootTransaction.LeafTransaction));
      }

      using (_rootTransaction.CreateSubTransaction ().EnterScope (AutoRollbackBehavior.Rollback))
      {
        Assert.That (ClientTransaction.Current, Is.SameAs (_rootTransaction.LeafTransaction));
      }
    }

    [Test]
    public void Scope_AffectsNewObject ()
    {
      Assert.That (() => Order.NewObject (), Throws.InvalidOperationException.With.Message.EqualTo ("..."));

      var leafTransaction = _rootTransaction.CreateSubTransaction ();
      using (leafTransaction.EnterDiscardingScope ())
      {
        var instance = Order.NewObject ();
        Assert.That (instance.RootTransaction, Is.SameAs (_rootTransaction));
        Assert.That (instance.DefaultTransactionContext.ClientTransaction, Is.SameAs (leafTransaction));
      }
    }

    [Test]
    public void Scope_AffectsGetObject ()
    {
      Assert.That (
          () => DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes> (),
          Throws.InvalidOperationException.With.Message.EqualTo ("..."));

      var clientTransaction = _rootTransaction.CreateSubTransaction ();
      using (clientTransaction.EnterNonDiscardingScope ())
      {
        var instance = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes> ();
        Assert.That (instance.RootTransaction, Is.SameAs (_rootTransaction));
        Assert.That (instance.DefaultTransactionContext.ClientTransaction, Is.SameAs (clientTransaction));
      }
    }

    [Test]
    public void Scope_AffectsQuery ()
    {
      var query = from cwadt in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes> () select cwadt;
      Assert.That (() => query.ToArray (), Throws.InvalidOperationException.With.Message.EqualTo ("..."));

      var clientTransaction = _rootTransaction.CreateSubTransaction ();
      using (clientTransaction.EnterNonDiscardingScope ())
      {
        var result = query.ToArray ().First ();
        Assert.That (result.RootTransaction, Is.SameAs (_rootTransaction));
        Assert.That (result.DefaultTransactionContext.ClientTransaction, Is.SameAs (clientTransaction));
      }
    }

    [Test]
    public void ScopeForOtherHierarchy_DoesNotAffectDefaultContext ()
    {
      _order1LoadedInRootTransaction.OrderNumber = 2;

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        Assert.That (ClientTransaction.Current, Is.Not.SameAs (_rootTransaction));

        Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));
        Assert.That (_order1LoadedInRootTransaction.OrderNumber, Is.EqualTo (2));
        Assert.That (_order1LoadedInRootTransaction.State, Is.EqualTo (StateType.Changed));
      }

      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));
        Assert.That (_order1LoadedInRootTransaction.OrderNumber, Is.EqualTo (2));
        Assert.That (_order1LoadedInRootTransaction.State, Is.EqualTo (StateType.Changed));
      }

      using (ClientTransaction.CreateRootTransaction ().EnterScope (AutoRollbackBehavior.Rollback))
      {
        Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));
        Assert.That (_order1LoadedInRootTransaction.OrderNumber, Is.EqualTo (2));
        Assert.That (_order1LoadedInRootTransaction.State, Is.EqualTo (StateType.Changed));
      }
    }

    [Test]
    public void OpeningScopeForNonLeafTransaction_Throws ()
    {
      _rootTransaction.CreateSubTransaction ();

      Assert.That (() => _rootTransaction.EnterDiscardingScope (), Throws.InvalidOperationException.With.Message.EqualTo ("..."));
      Assert.That (() => _rootTransaction.EnterNonDiscardingScope (), Throws.InvalidOperationException.With.Message.EqualTo ("..."));
    }

    [Test]
    public void OpeningScopeForNonLeafTransaction_AllowedWithRightBehaviorFlag_AndInfluencesDefaultContext ()
    {
      var subTransaction = _rootTransaction.CreateSubTransaction();

      Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (subTransaction));
      Assert.That (subTransaction.ActiveTransaction, Is.SameAs (subTransaction));

      Assert.That (_order1LoadedInRootTransaction.RootTransaction, Is.SameAs (_rootTransaction));
      Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (subTransaction));

      _order1LoadedInRootTransaction.OrderNumber = 2;

      using (_rootTransaction.EnterNonDiscardingScope (inactiveTransactionBehavior: InactiveTransactionBehavior.MakeActive))
      {
        Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (_rootTransaction));
        Assert.That (subTransaction.ActiveTransaction, Is.SameAs (_rootTransaction));

        Assert.That (_order1LoadedInRootTransaction.RootTransaction, Is.SameAs (_rootTransaction));
        Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));

        Assert.That (_order1LoadedInRootTransaction.OrderNumber, Is.EqualTo (1));
        Assert.That (_order1LoadedInRootTransaction.State, Is.EqualTo (StateType.Unchanged));

        Assert.That (() => _order1LoadedInRootTransaction.OrderNumber = 3, Throws.InvalidOperationException.With.Message.EqualTo ("..."));
      }

      Assert.That (_order1LoadedInRootTransaction.OrderNumber, Is.EqualTo (2));
    }

    [Test]
    public void Scope_NestedForDifferentTransactionsInTheHierarchy ()
    {
      var middleTransaction = _rootTransaction.CreateSubTransaction ();
      var subTransaction = middleTransaction.CreateSubTransaction ();

      Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (subTransaction));
      Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (subTransaction));

      using (middleTransaction.EnterNonDiscardingScope (inactiveTransactionBehavior: InactiveTransactionBehavior.MakeActive))
      {
        Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (middleTransaction));
        Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (middleTransaction));

        using (_rootTransaction.EnterNonDiscardingScope (inactiveTransactionBehavior: InactiveTransactionBehavior.MakeActive))
        {
          Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (_rootTransaction));
          Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));

          using (subTransaction.EnterNonDiscardingScope (inactiveTransactionBehavior: InactiveTransactionBehavior.MakeActive))
          {
            Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (subTransaction));
            Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (subTransaction));
          }

          Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (_rootTransaction));
          Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));
        }

        Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (middleTransaction));
        Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (middleTransaction));
      }

      Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (subTransaction));
      Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (subTransaction));
    }
  }
}