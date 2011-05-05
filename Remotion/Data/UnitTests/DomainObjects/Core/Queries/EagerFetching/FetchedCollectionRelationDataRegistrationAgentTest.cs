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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Queries.EagerFetching
{
  [TestFixture]
  public class FetchedCollectionRelationDataRegistrationAgentTest : StandardMappingTest
  {
    private FetchedCollectionRelationDataRegistrationAgent _agent;

    private Order _originatingOrder1;
    private Order _originatingOrder2;

    private OrderItem _fetchedOrderItem1;
    private OrderItem _fetchedOrderItem2;
    private OrderItem _fetchedOrderItem3;

    private DataContainer _fetchedOrderItemDataContainer1;
    private DataContainer _fetchedOrderItemDataContainer2;
    private DataContainer _fetchedOrderItemDataContainer3;

    private IDataManager _dataManagerMock;

    private IRelationEndPointDefinition _endPointDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _agent = new FetchedCollectionRelationDataRegistrationAgent();

      _originatingOrder1 = DomainObjectMother.CreateFakeObject<Order> ();
      _originatingOrder2 = DomainObjectMother.CreateFakeObject<Order> ();

      _fetchedOrderItem1 = DomainObjectMother.CreateFakeObject<OrderItem> ();
      _fetchedOrderItem2 = DomainObjectMother.CreateFakeObject<OrderItem> ();
      _fetchedOrderItem3 = DomainObjectMother.CreateFakeObject<OrderItem> ();

      _fetchedOrderItemDataContainer1 = CreateFetchedOrderItemDataContainer (_fetchedOrderItem1, _originatingOrder1.ID);
      _fetchedOrderItemDataContainer2 = CreateFetchedOrderItemDataContainer (_fetchedOrderItem2, _originatingOrder2.ID);
      _fetchedOrderItemDataContainer3 = CreateFetchedOrderItemDataContainer (_fetchedOrderItem3, _originatingOrder1.ID);

      _dataManagerMock = MockRepository.GenerateStrictMock<IDataManager>();

      _endPointDefinition = GetEndPointDefinition (typeof (Order), "OrderItems");
    }

    [Test]
    public void GroupAndRegisterRelatedObjects ()
    {
      _dataManagerMock.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderItem1.ID)).Return (_fetchedOrderItemDataContainer1);
      _dataManagerMock.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderItem2.ID)).Return (_fetchedOrderItemDataContainer2);
      _dataManagerMock.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderItem3.ID)).Return (_fetchedOrderItemDataContainer3);

      ExpectMarkCollectionEndPointComplete (_originatingOrder1.ID, _endPointDefinition, true, _fetchedOrderItem1, _fetchedOrderItem3);
      ExpectMarkCollectionEndPointComplete (_originatingOrder2.ID, _endPointDefinition, true, _fetchedOrderItem2);
      _dataManagerMock.Replay ();

      _agent.GroupAndRegisterRelatedObjects(
          _endPointDefinition,
          new[] { _originatingOrder1, _originatingOrder2 },
          new[] { _fetchedOrderItem1, _fetchedOrderItem2, _fetchedOrderItem3 },
          _dataManagerMock);

      _dataManagerMock.VerifyAllExpectations ();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithNullOriginalObject ()
    {
      _dataManagerMock.Replay ();

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new DomainObject[] { null },
          new DomainObject[0], 
          _dataManagerMock);

      _dataManagerMock.VerifyAllExpectations ();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithNullRelatedObject ()
    {
      ExpectMarkCollectionEndPointComplete (_originatingOrder1.ID, _endPointDefinition, true);
      _dataManagerMock.Replay ();

      _agent.GroupAndRegisterRelatedObjects (
        _endPointDefinition,
          new[] { _originatingOrder1 },
        new DomainObject[] { null }, 
        _dataManagerMock);

      _dataManagerMock.VerifyAllExpectations ();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithRelatedObjectPointingToNull ()
    {
      var dataContainerPointingToNull = CreateFetchedOrderItemDataContainer (_fetchedOrderItem1, null);
      _dataManagerMock.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderItem1.ID)).Return (dataContainerPointingToNull);

      ExpectMarkCollectionEndPointComplete (_originatingOrder1.ID, _endPointDefinition, true);
      _dataManagerMock.Replay ();

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new[] { _originatingOrder1 },
          new[] { _fetchedOrderItem1 },
          _dataManagerMock);

      _dataManagerMock.VerifyAllExpectations ();
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WithEndPointAlreadyComplete ()
    {

      _dataManagerMock.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderItem1.ID)).Return (_fetchedOrderItemDataContainer1);
      _dataManagerMock.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderItem2.ID)).Return (_fetchedOrderItemDataContainer2);
      _dataManagerMock.Stub (stub => stub.GetDataContainerWithoutLoading (_fetchedOrderItem3.ID)).Return (_fetchedOrderItemDataContainer3);

      ExpectMarkCollectionEndPointComplete (_originatingOrder1.ID, _endPointDefinition, false, _fetchedOrderItem1, _fetchedOrderItem3);
      ExpectMarkCollectionEndPointComplete (_originatingOrder2.ID, _endPointDefinition, true, _fetchedOrderItem2);
      _dataManagerMock.Replay ();

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new[] { _originatingOrder1, _originatingOrder2 },
          new[] { _fetchedOrderItem1, _fetchedOrderItem2, _fetchedOrderItem3 },
          _dataManagerMock);

      _dataManagerMock.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot register relation end-point "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' for domain object "
        + "'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid'. The end-point belongs to an object of class 'Order' but the domain object "
        + "has class 'OrderItem'.")]
    public void GroupAndRegisterRelatedObjects_InvalidOriginalObject ()
    {
      _dataManagerMock.Replay();

      var invalidOriginalObject = DomainObjectMother.CreateFakeObject<OrderItem> (DomainObjectIDs.OrderItem1);
      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new[] { invalidOriginalObject },
          new DomainObject[0], 
          _dataManagerMock);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Cannot associate object 'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid' with the relation end-point "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems'. An object of type "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem' was expected.")]
    public void GroupAndRegisterRelatedObjects_InvalidRelatedObject ()
    {
      _dataManagerMock.Replay();

      var invalidFetchedObject = DomainObjectMother.CreateFakeObject<OrderTicket> (DomainObjectIDs.OrderTicket1);

      _agent.GroupAndRegisterRelatedObjects (
          _endPointDefinition,
          new[] { _originatingOrder1, _originatingOrder2 }, 
          new[] { invalidFetchedObject },
          _dataManagerMock);
    }

    [Test]
    public void GroupAndRegisterRelatedObjects_WrongCardinality ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (Order), "OrderTicket");

      Assert.That (
          () => _agent.GroupAndRegisterRelatedObjects (endPointDefinition, new[] { _originatingOrder1 }, new[] { _fetchedOrderItem1 }, _dataManagerMock), 
          Throws.ArgumentException.With.Message.EqualTo (
              "Only collection-valued relations can be handled by this registration agent.\r\nParameter name: relationEndPointDefinition"));
    }

    [Test]
    public void Serialization ()
    {
      Serializer.SerializeAndDeserialize (_agent);
    }

    private DataContainer CreateFetchedOrderItemDataContainer (OrderItem fetchedOrderItem, ObjectID originatingOrderID)
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (fetchedOrderItem.ID, "Order");
      var dataContainer = RelationEndPointTestHelper.CreateExistingForeignKeyDataContainer (endPointID, originatingOrderID);
      return dataContainer;
    }

    private void ExpectMarkCollectionEndPointComplete (ObjectID objectID, IRelationEndPointDefinition endPointDefinition, bool result, params DomainObject[] items)
    {
      var relationEndPointID = RelationEndPointID.Create (objectID, endPointDefinition);
      _dataManagerMock.Expect (mock => mock.TrySetCollectionEndPointData (relationEndPointID, items)).Return (result);
    }
  }
}