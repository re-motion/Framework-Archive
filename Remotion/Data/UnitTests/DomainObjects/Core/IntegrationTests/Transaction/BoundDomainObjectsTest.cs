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

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction
{
  [TestFixture]
  [Ignore ("TODO 5447")]
  public class BoundDomainObjectsTest : StandardMappingTest
  {
    private ClientTransaction _rootTransaction;

    public override void SetUp ()
    {
      base.SetUp ();

      _rootTransaction = ClientTransaction.CreateRootTransaction();
    }
    
    [Test]
    public void AccessingPropertiesAndState_AffectsAssociatedTransactionHierarchy ()
    {
      var order = DomainObjectIDs.Order1.GetObjectReference<Order>(_rootTransaction);
      Assert.That (order.RootTransaction, Is.SameAs (_rootTransaction));
      Assert.That (order.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));

      Assert.That (order.State, Is.EqualTo (StateType.NotLoadedYet));

      order.EnsureDataAvailable();

      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (order.Timestamp, Is.Not.Null);
      Assert.That (order.OrderNumber, Is.EqualTo (1));
      order.OrderNumber = 2;
      Assert.That (order.State, Is.EqualTo (StateType.Changed));
      Assert.That (order.OrderNumber, Is.EqualTo (2));
      Assert.That (order.IsInvalid, Is.False);
    }

    [Test]
    public void CurrentScope_DoesNotAffectPropertiesAndStateAccess ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order> (_rootTransaction);
      Assert.That (order.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));

      order.OrderNumber = 2;

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        Assert.That (order.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));

        Assert.That (order.OrderNumber, Is.EqualTo (2));
        Assert.That (order.State, Is.EqualTo (StateType.Changed));
      }
    }

    [Test]
    public void Execute_DoesNotAffectPropertiesAndStateAccess ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order> (_rootTransaction);
      Assert.That (order.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));

      order.OrderNumber = 2;

      ClientTransaction.CreateRootTransaction().Execute (
          () =>
          {
            Assert.That (order.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));

            Assert.That (order.OrderNumber, Is.EqualTo (2));
            Assert.That (order.State, Is.EqualTo (StateType.Changed));
          });
    }

    [Test]
    public void AccessingPropertiesAndState_UsesTheLeafTransaction ()
    {
      var subTransaction = _rootTransaction.CreateSubTransaction();

      var order = DomainObjectIDs.Order1.GetObject<Order> (_rootTransaction);
      Assert.That (order.RootTransaction, Is.SameAs (_rootTransaction));
      Assert.That (order.DefaultTransactionContext.ClientTransaction, Is.SameAs (subTransaction));

      Assert.That (order.State, Is.EqualTo (StateType.NotLoadedYet));

      order.OrderNumber = 2;

      Assert.That (order.State, Is.EqualTo (StateType.Changed));
      Assert.That (order.OrderNumber, Is.EqualTo (2));

      subTransaction.Discard();

      Assert.That (order.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));
      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (order.OrderNumber, Is.EqualTo (1));
    }

    [Test]
    public void OpeningScopeForNonLeafTransaction_Throws ()
    {
      _rootTransaction.CreateSubTransaction ();

      Assert.That (() => _rootTransaction.EnterDiscardingScope (), Throws.InvalidOperationException.With.Message.EqualTo ("..."));
      Assert.That (() => _rootTransaction.EnterNonDiscardingScope (), Throws.InvalidOperationException.With.Message.EqualTo ("..."));
      Assert.That (() => _rootTransaction.Execute (() => { }), Throws.InvalidOperationException.With.Message.EqualTo ("..."));
    }
  }
}