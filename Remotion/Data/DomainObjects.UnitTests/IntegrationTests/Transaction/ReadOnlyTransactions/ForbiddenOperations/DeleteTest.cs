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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction.ReadOnlyTransactions.ForbiddenOperations
{
  [TestFixture]
  public class DeleteTest : ReadOnlyTransactionsTestBase
  {
    private ClassWithAllDataTypes _loadedClassWithAllDataTypes;
    private Order _orderNewInRootTransaction;

    public override void SetUp ()
    {
      base.SetUp ();

      _loadedClassWithAllDataTypes = ExecuteInWriteableSubTransaction (() => DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes> ());
    }

    protected override void InitializeReadOnlyRootTransaction ()
    {
      base.InitializeReadOnlyRootTransaction ();
      _orderNewInRootTransaction = Order.NewObject();
    }

    [Test]
    public void DeleteInReadOnlyRootTransaction_IsForbidden_Loaded ()
    {
      CheckState (ReadOnlyRootTransaction, _loadedClassWithAllDataTypes, StateType.Unchanged);
      CheckState (ReadOnlyMiddleTransaction, _loadedClassWithAllDataTypes, StateType.Unchanged);
      CheckState (WriteableSubTransaction, _loadedClassWithAllDataTypes, StateType.Unchanged);

      CheckForbidden (() => ExecuteInReadOnlyRootTransaction (() => _loadedClassWithAllDataTypes.Delete()), "ObjectDeleting");

      CheckState (ReadOnlyRootTransaction, _loadedClassWithAllDataTypes, StateType.Unchanged);
      CheckState (ReadOnlyMiddleTransaction, _loadedClassWithAllDataTypes, StateType.Unchanged);
      CheckState (WriteableSubTransaction, _loadedClassWithAllDataTypes, StateType.Unchanged);
    }

    [Test]
    public void DeleteInReadOnlyMiddleTransaction_IsForbidden_Loaded ()
    {
      CheckState (ReadOnlyRootTransaction, _loadedClassWithAllDataTypes, StateType.Unchanged);
      CheckState (ReadOnlyMiddleTransaction, _loadedClassWithAllDataTypes, StateType.Unchanged);
      CheckState (WriteableSubTransaction, _loadedClassWithAllDataTypes, StateType.Unchanged);

      CheckForbidden (() => ExecuteInReadOnlyMiddleTransaction (() => _loadedClassWithAllDataTypes.Delete()), "ObjectDeleting");

      CheckState (ReadOnlyRootTransaction, _loadedClassWithAllDataTypes, StateType.Unchanged);
      CheckState (ReadOnlyMiddleTransaction, _loadedClassWithAllDataTypes, StateType.Unchanged);
      CheckState (WriteableSubTransaction, _loadedClassWithAllDataTypes, StateType.Unchanged);
    }

    [Test]
    public void DeleteInReadOnlyRootTransaction_IsForbidden_New ()
    {
      CheckState (ReadOnlyRootTransaction, _orderNewInRootTransaction, StateType.New);
      CheckState (ReadOnlyMiddleTransaction, _orderNewInRootTransaction, StateType.NotLoadedYet);
      CheckState (WriteableSubTransaction, _orderNewInRootTransaction, StateType.NotLoadedYet);

      CheckForbidden (() => ExecuteInReadOnlyRootTransaction (() => _orderNewInRootTransaction.Delete()), "ObjectDeleting");

      CheckState (ReadOnlyRootTransaction, _orderNewInRootTransaction, StateType.New);
      CheckState (ReadOnlyMiddleTransaction, _orderNewInRootTransaction, StateType.NotLoadedYet);
      CheckState (WriteableSubTransaction, _orderNewInRootTransaction, StateType.NotLoadedYet);
    }

    [Test]
    public void DeleteInReadOnlyMiddleTransaction_IsForbidden_New ()
    {
      CheckState (ReadOnlyRootTransaction, _orderNewInRootTransaction, StateType.New);
      CheckState (ReadOnlyMiddleTransaction, _orderNewInRootTransaction, StateType.NotLoadedYet);
      CheckState (WriteableSubTransaction, _orderNewInRootTransaction, StateType.NotLoadedYet);

      CheckForbidden (() => ExecuteInReadOnlyMiddleTransaction (() => _orderNewInRootTransaction.Delete()), "ObjectDeleting");

      CheckState (ReadOnlyRootTransaction, _orderNewInRootTransaction, StateType.New);
      CheckState (ReadOnlyMiddleTransaction, _orderNewInRootTransaction, StateType.Unchanged);
      CheckState (WriteableSubTransaction, _orderNewInRootTransaction, StateType.NotLoadedYet);
    }
  }
}