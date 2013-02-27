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
  public class ExecuteInScopeTest : HierarchyBoundObjectsTestBase
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
    public void ExecuteInScope_ForLeafTransaction_AffectsCurrentTransaction ()
    {
      var subTransaction = _rootTransaction.CreateSubTransaction();

      subTransaction.ExecuteInScope (() => Assert.That (ClientTransaction.Current, Is.SameAs (subTransaction)));
      subTransaction.ExecuteInScope (
          () =>
          {
            Assert.That (ClientTransaction.Current, Is.SameAs (subTransaction));
            return 7;
          });
    }

    [Test]
    public void ExecuteInScope_AffectsNewObject ()
    {
      Assert.That (() => Order.NewObject (), Throws.InvalidOperationException.With.Message.EqualTo ("..."));

      var leafTransaction = _rootTransaction.CreateSubTransaction ();
      leafTransaction.ExecuteInScope (() =>
      {
        var instance = Order.NewObject ();
        Assert.That (instance.RootTransaction, Is.SameAs (_rootTransaction));
        Assert.That (instance.DefaultTransactionContext.ClientTransaction, Is.SameAs (leafTransaction));
      });
    }

    [Test]
    public void ExecuteInScope_AffectsGetObject ()
    {
      Assert.That (
          () => DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes> (),
          Throws.InvalidOperationException.With.Message.EqualTo ("..."));

      var clientTransaction = _rootTransaction.CreateSubTransaction ();
      clientTransaction.ExecuteInScope (
          () =>
          {
            var instance = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();
            Assert.That (instance.RootTransaction, Is.SameAs (_rootTransaction));
            Assert.That (instance.DefaultTransactionContext.ClientTransaction, Is.SameAs (clientTransaction));
          });
    }

    [Test]
    public void ExecuteInScope_AffectsQuery ()
    {
      var query = from cwadt in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes> () select cwadt;
      Assert.That (() => query.ToArray (), Throws.InvalidOperationException.With.Message.EqualTo ("..."));

      var clientTransaction = _rootTransaction.CreateSubTransaction ();
      clientTransaction.ExecuteInScope (
          () =>
          {
            var result = query.ToArray().First();
            Assert.That (result.RootTransaction, Is.SameAs (_rootTransaction));
            Assert.That (result.DefaultTransactionContext.ClientTransaction, Is.SameAs (clientTransaction));
          });
    }

    [Test]
    public void ExecuteInScope_ForOtherHierarchy_DoesNotAffectDefaultContext ()
    {
      _order1LoadedInRootTransaction.OrderNumber = 2;

      ClientTransaction.CreateRootTransaction ().ExecuteInScope (() =>
      {
        Assert.That (ClientTransaction.Current, Is.Not.SameAs (_rootTransaction));

        Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));
        Assert.That (_order1LoadedInRootTransaction.OrderNumber, Is.EqualTo (2));
        Assert.That (_order1LoadedInRootTransaction.State, Is.EqualTo (StateType.Changed));
      });
    }

    [Test]
    public void ExecuteInScope_ForNonLeafTransaction_Throws ()
    {
      _rootTransaction.CreateSubTransaction ();

      Assert.That (() => _rootTransaction.ExecuteInScope (() => { }), Throws.InvalidOperationException.With.Message.EqualTo ("..."));
      Assert.That (() => _rootTransaction.ExecuteInScope (() => 7), Throws.InvalidOperationException.With.Message.EqualTo ("..."));
    }

    [Test]
    public void ExecuteInScope_ForNonLeafTransaction_AllowedWithRightBehaviorFlag_AndInfluencesDefaultContext ()
    {
      var subTransaction = _rootTransaction.CreateSubTransaction();

      Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (subTransaction));
      Assert.That (subTransaction.ActiveTransaction, Is.SameAs (subTransaction));

      Assert.That (_order1LoadedInRootTransaction.RootTransaction, Is.SameAs (_rootTransaction));
      Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (subTransaction));

      _order1LoadedInRootTransaction.OrderNumber = 2;

      _rootTransaction.ExecuteInScope (
          () =>
          {
            Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (_rootTransaction));
            Assert.That (subTransaction.ActiveTransaction, Is.SameAs (_rootTransaction));

            Assert.That (_order1LoadedInRootTransaction.RootTransaction, Is.SameAs (_rootTransaction));
            Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));

            Assert.That (_order1LoadedInRootTransaction.OrderNumber, Is.EqualTo (1));
            Assert.That (_order1LoadedInRootTransaction.State, Is.EqualTo (StateType.Unchanged));

            Assert.That (() => _order1LoadedInRootTransaction.OrderNumber = 3, Throws.InvalidOperationException.With.Message.EqualTo ("..."));
          },
          inactiveTransactionBehavior: InactiveTransactionBehavior.MakeActive);

      Assert.That (_order1LoadedInRootTransaction.OrderNumber, Is.EqualTo (2));
    }

    [Test]
    public void ExecuteInScope_NestedForDifferentTransactionsInTheHierarchy ()
    {
      var middleTransaction = _rootTransaction.CreateSubTransaction ();
      var subTransaction = middleTransaction.CreateSubTransaction ();

      Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (subTransaction));
      Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (subTransaction));

      middleTransaction.ExecuteInScope (() =>
      {
        Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (middleTransaction));
        Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (middleTransaction));

        _rootTransaction.ExecuteInScope (
            () =>
            {
              Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (_rootTransaction));
              Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));

              subTransaction.ExecuteInScope (
                  () =>
                  {
                    Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (subTransaction));
                    Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (subTransaction));
                  },
                  inactiveTransactionBehavior: InactiveTransactionBehavior.MakeActive);

              Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (_rootTransaction));
              Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));
            },
            inactiveTransactionBehavior: InactiveTransactionBehavior.MakeActive);

        Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (middleTransaction));
        Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (middleTransaction));
      }, inactiveTransactionBehavior: InactiveTransactionBehavior.MakeActive);

      Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (subTransaction));
      Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (subTransaction));
    }
  }
}