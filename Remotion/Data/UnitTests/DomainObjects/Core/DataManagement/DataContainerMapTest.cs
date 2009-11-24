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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class DataContainerMapTest : ClientTransactionBaseTest
  {
    // types

    // static members and constants

    // member fields

    private DataContainerMap _map;
    private DataContainer _newOrder;
    private DataContainer _existingOrder;

    // construction and disposing

    public DataContainerMapTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _map = new DataContainerMap (ClientTransactionMock);
      _newOrder = CreateNewOrderDataContainer ();
      _existingOrder = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;
    }

    [Test]
    public void DeleteNewDataContainer ()
    {
      _map.Register (_newOrder);
      Assert.AreEqual (1, _map.Count);

      _map.PerformDelete (_newOrder);
      Assert.AreEqual (0, _map.Count);
    }

    [Test]
    public void RemoveDeletedDataContainerInCommit ()
    {
      _map.Register (_existingOrder);
      Assert.AreEqual (1, _map.Count);

      Order order = (Order) _existingOrder.DomainObject;
      order.Delete();
      _map.Commit();

      Assert.AreEqual (0, _map.Count);
    }

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException))]
    public void AccessDeletedDataContainerAfterCommit ()
    {
      _map.Register (_existingOrder);
      Assert.AreEqual (1, _map.Count);

      Order order = (Order) _existingOrder.DomainObject;
      order.Delete ();
      _map.Commit ();

      Dev.Null = _existingOrder.Timestamp;
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException))]
    public void GetByInvalidState ()
    {
      _map.GetByState ((StateType) 1000);
    }

    [Test]
    public void RollbackForDeletedObject ()
    {
      using (ClientTransactionMock.EnterDiscardingScope ())
      {
        _map.Register (_existingOrder);

        Order order = (Order) _existingOrder.DomainObject;
        order.Delete();
        Assert.AreEqual (StateType.Deleted, _existingOrder.State);

        _map.Rollback();

        _existingOrder = _map[_existingOrder.ID];
        Assert.IsNotNull (_existingOrder);
        Assert.AreEqual (StateType.Unchanged, _existingOrder.State);
      }
    }

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException))]
    public void RollbackForNewObject ()
    {
      _map.Register (_newOrder);

      _map.Rollback ();

      Dev.Null = _newOrder.Timestamp;
    }

    [Test]
    public void Rollback_SingleDeletedObject ()
    {
      using (ClientTransactionMock.EnterDiscardingScope ())
      {
        _map.Register (_existingOrder);

        Order order = (Order) _existingOrder.DomainObject;
        order.Delete ();
        Assert.AreEqual (StateType.Deleted, _existingOrder.State);

        _map.Rollback (_existingOrder);

        _existingOrder = _map[_existingOrder.ID];
        Assert.IsNotNull (_existingOrder);
        Assert.AreEqual (StateType.Unchanged, _existingOrder.State);
      }
    }

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException))]
    public void Rollback_SingleNewObject ()
    {
      _map.Register (_newOrder);

      _map.Rollback (_newOrder);

      Dev.Null = _newOrder.Timestamp;
    }
    
    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException),
        ExpectedMessage = "Cannot remove DataContainer '.*' from DataContainerMap, because it belongs to a different ClientTransaction.",
        MatchType = MessageMatch.Regex)]
    public void PerformDeleteWithOtherClientTransaction ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        Order order1 = Order.GetObject (DomainObjectIDs.Order1);

        _map.PerformDelete (order1.InternalDataContainer);
      }
    }

    private DataContainer CreateNewOrderDataContainer ()
    {
      Order order = Order.NewObject ();
      order.OrderNumber = 10;
      order.DeliveryDate = new DateTime (2006, 1, 1);
      order.Official = Official.GetObject (DomainObjectIDs.Official1);
      order.Customer = Customer.GetObject (DomainObjectIDs.Customer1);

      return order.InternalDataContainer;
    }

    [Test]
    public void GetObjectWithoutLoading_LoadedObject ()
    {
      ClassWithAllDataTypes loadedOrder = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      Assert.AreSame (loadedOrder, ClientTransactionMock.DataManager.DataContainerMap.GetObjectWithoutLoading (DomainObjectIDs.ClassWithAllDataTypes1, false));
    }

    [Test]
    public void GetObjectWithoutLoading_NonLoadedObject ()
    {
      Assert.IsNull (ClientTransactionMock.DataManager.DataContainerMap.GetObjectWithoutLoading (DomainObjectIDs.ClassWithAllDataTypes1, false));
    }

    [Test]
    public void GetObjectWithoutLoading_IncludeDeletedTrue ()
    {
      Order deletedOrder = Order.GetObject (DomainObjectIDs.Order1);
      deletedOrder.Delete ();
      Assert.AreSame (deletedOrder, ClientTransactionMock.DataManager.DataContainerMap.GetObjectWithoutLoading (DomainObjectIDs.Order1, true));
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void GetObjectWithoutLoading_IncludeDeletedFalse ()
    {
      Order deletedOrder = Order.GetObject (DomainObjectIDs.Order1);
      deletedOrder.Delete ();
      ClientTransactionMock.DataManager.DataContainerMap.GetObjectWithoutLoading (DomainObjectIDs.Order1, false);
    }
  }
}
