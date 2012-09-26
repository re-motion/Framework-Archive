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
  public class PropertyValueWriteTest : InactiveTransactionsTestBase
  {
    private Order _order1;

    public override void SetUp ()
    {
      base.SetUp ();

      _order1 = ActiveSubTransaction.Execute (() => Order.GetObject (DomainObjectIDs.Order1));
      ActiveSubTransaction.Execute (() => _order1.OrderNumber = 13);
    }

    [Test]
    public void PropertyValueSetInInactiveRootTransaction_IsForbidden ()
    {
      CheckProperty (InactiveRootTransaction, _order1, o => o.OrderNumber, 1, 1);
      CheckProperty (InactiveMiddleTransaction, _order1, o => o.OrderNumber, 1, 1);
      CheckProperty (ActiveSubTransaction, _order1, o => o.OrderNumber, 13, 1);

      CheckForbidden (() => InactiveRootTransaction.Execute (() => _order1.OrderNumber = 27), "PropertyValueChanging");

      CheckProperty (InactiveRootTransaction, _order1, o => o.OrderNumber, 1, 1);
      CheckProperty (InactiveMiddleTransaction, _order1, o => o.OrderNumber, 1, 1);
      CheckProperty (ActiveSubTransaction, _order1, o => o.OrderNumber, 13, 1);
    }

    [Test]
    public void PropertyValueSetInInactiveMiddleTransaction_IsForbidden ()
    {
      CheckProperty (InactiveRootTransaction, _order1, o => o.OrderNumber, 1, 1);
      CheckProperty (InactiveMiddleTransaction, _order1, o => o.OrderNumber, 1, 1);
      CheckProperty (ActiveSubTransaction, _order1, o => o.OrderNumber, 13, 1);

      CheckForbidden (() => InactiveMiddleTransaction.Execute (() => _order1.OrderNumber = 27), "PropertyValueChanging");

      CheckProperty (InactiveRootTransaction, _order1, o => o.OrderNumber, 1, 1);
      CheckProperty (InactiveMiddleTransaction, _order1, o => o.OrderNumber, 1, 1);
      CheckProperty (ActiveSubTransaction, _order1, o => o.OrderNumber, 13, 1);
    }
  }
}