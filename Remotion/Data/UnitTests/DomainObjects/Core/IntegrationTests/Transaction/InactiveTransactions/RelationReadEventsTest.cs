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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.RhinoMocks.UnitTesting;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using Remotion.Data.UnitTests.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction.InactiveTransactions
{
  [TestFixture]
  public class RelationReadEventsTest : InactiveTransactionsTestBase
  {
    private Order _loadedOrder;
    private Customer _relatedCustomer;
    private OrderTicket _relatedOrderTicket;
    private OrderItem _relatedOrderItem1;
    private OrderItem _relatedOrderItem2;

    public override void SetUp ()
    {
      base.SetUp();

      _loadedOrder = ExecuteInActiveSubTransaction (() => DomainObjectIDs.Order1.GetObject<Order> ());
      _relatedCustomer = ExecuteInActiveSubTransaction (() => DomainObjectIDs.Customer1.GetObject<Customer> ());
      _relatedOrderTicket = ExecuteInActiveSubTransaction (() => DomainObjectIDs.OrderTicket1.GetObject<OrderTicket> ());
      _relatedOrderItem1 = ExecuteInActiveSubTransaction (() => DomainObjectIDs.OrderItem1.GetObject<OrderItem>());
      _relatedOrderItem2 = ExecuteInActiveSubTransaction (() => DomainObjectIDs.OrderItem2.GetObject<OrderItem>());

      InstallListenerMock();
      InstallExtensionMock();
    }


    [Test]
    public void RelationReadEvents_OnlyRaisedInActiveSub_NonVirtualEndPoint ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (Order), "Customer");
      ExpectRelationReadEvents (ActiveSubTransaction, _loadedOrder, endPointDefinition, _relatedCustomer);

      Dev.Null = ExecuteInActiveSubTransaction (() => _loadedOrder.Customer);

      AssertNoRelationReadEvents (InactiveMiddleTransaction);
      AssertNoRelationReadEvents (InactiveRootTransaction);

      ListenerDynamicMock.VerifyAllExpectations();
      ExtensionStrictMock.VerifyAllExpectations();
    }

    [Test]
    public void RelationReadEvents_OnlyRaisedInActiveSub_VirtualEndPoint_One ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (Order), "OrderTicket");
      ExpectRelationReadEvents (ActiveSubTransaction, _loadedOrder, endPointDefinition, _relatedOrderTicket);

      Dev.Null = ExecuteInActiveSubTransaction (() => _loadedOrder.OrderTicket);

      AssertNoRelationReadEvents (InactiveMiddleTransaction);
      AssertNoRelationReadEvents (InactiveRootTransaction);

      ListenerDynamicMock.VerifyAllExpectations ();
      ExtensionStrictMock.VerifyAllExpectations ();
    }

    [Test]
    public void RelationReadEvents_OnlyRaisedInActiveSub_VirtualEndPoint_Many ()
    {
      var endPointDefinition = GetEndPointDefinition (typeof (Order), "OrderItems");
      ExpectRelationReadEvents (ActiveSubTransaction, _loadedOrder, endPointDefinition, new[] { _relatedOrderItem1, _relatedOrderItem2 });

      ExecuteInActiveSubTransaction (() => _loadedOrder.OrderItems.EnsureDataComplete());

      AssertNoRelationReadEvents (InactiveMiddleTransaction);
      AssertNoRelationReadEvents (InactiveRootTransaction);

      ListenerDynamicMock.VerifyAllExpectations ();
      ExtensionStrictMock.VerifyAllExpectations ();
    }

    private void ExpectRelationReadEvents (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition endPointDefinition,
        DomainObject relatedDomainObject)
    {
      using (ListenerDynamicMock.GetMockRepository().Ordered())
      {
        ListenerDynamicMock.Expect (mock => mock.RelationReading (clientTransaction, domainObject, endPointDefinition, ValueAccess.Current));
        ListenerDynamicMock.Expect (
            mock => mock.RelationRead (clientTransaction, domainObject, endPointDefinition, relatedDomainObject, ValueAccess.Current));
      }

      using (ExtensionStrictMock.GetMockRepository().Ordered())
      {
        ExtensionStrictMock.Expect (mock => mock.RelationReading (clientTransaction, domainObject, endPointDefinition, ValueAccess.Current));
        ExtensionStrictMock.Expect (
            mock => mock.RelationRead (clientTransaction, domainObject, endPointDefinition, relatedDomainObject, ValueAccess.Current));
      }
    }

    private void ExpectRelationReadEvents (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition endPointDefinition,
        DomainObject[] relatedDomainObjects)
    {
      using (ListenerDynamicMock.GetMockRepository ().Ordered ())
      {
        ListenerDynamicMock.Expect (mock => mock.RelationReading (clientTransaction, domainObject, endPointDefinition, ValueAccess.Current));
        ListenerDynamicMock.Expect (
            mock => mock.RelationRead (
                Arg.Is (clientTransaction),
                Arg.Is (domainObject),
                Arg.Is (endPointDefinition),
                Arg<ReadOnlyDomainObjectCollectionAdapter<DomainObject>>.List.Equivalent (relatedDomainObjects),
                Arg.Is (ValueAccess.Current)));
      }

      using (ExtensionStrictMock.GetMockRepository ().Ordered ())
      {
        ExtensionStrictMock.Expect (mock => mock.RelationReading (clientTransaction, domainObject, endPointDefinition, ValueAccess.Current));
        ExtensionStrictMock.Expect (
            mock => mock.RelationRead (
                Arg.Is (clientTransaction),
                Arg.Is (domainObject),
                Arg.Is (endPointDefinition),
                Arg<ReadOnlyDomainObjectCollectionAdapter<DomainObject>>.List.Equivalent (relatedDomainObjects),
                Arg.Is (ValueAccess.Current)));
      }
    }

    private void AssertNoRelationReadEvents (ClientTransaction clientTransaction)
    {
      ListenerDynamicMock.AssertWasNotCalled (
          mock => mock.RelationReading (
              Arg.Is (clientTransaction),
              Arg<DomainObject>.Is.Anything,
              Arg<IRelationEndPointDefinition>.Is.Anything,
              Arg<ValueAccess>.Is.Anything));
      ListenerDynamicMock.AssertWasNotCalled (
          mock => mock.RelationRead (
              Arg.Is (clientTransaction),
              Arg<DomainObject>.Is.Anything,
              Arg<IRelationEndPointDefinition>.Is.Anything,
              Arg<DomainObject>.Is.Anything,
              Arg<ValueAccess>.Is.Anything));
      ListenerDynamicMock.AssertWasNotCalled (
          mock => mock.RelationRead (
              Arg.Is (clientTransaction),
              Arg<DomainObject>.Is.Anything,
              Arg<IRelationEndPointDefinition>.Is.Anything,
              Arg<ReadOnlyDomainObjectCollectionAdapter<DomainObject>>.Is.Anything,
              Arg<ValueAccess>.Is.Anything));

      ExtensionStrictMock.AssertWasNotCalled (
          mock => mock.RelationReading (
              Arg.Is (clientTransaction),
              Arg<DomainObject>.Is.Anything,
              Arg<IRelationEndPointDefinition>.Is.Anything,
              Arg<ValueAccess>.Is.Anything));
      ExtensionStrictMock.AssertWasNotCalled (
          mock => mock.RelationRead (
              Arg.Is (clientTransaction),
              Arg<DomainObject>.Is.Anything,
              Arg<IRelationEndPointDefinition>.Is.Anything,
              Arg<DomainObject>.Is.Anything,
              Arg<ValueAccess>.Is.Anything));
      ExtensionStrictMock.AssertWasNotCalled (
          mock => mock.RelationRead (
              Arg.Is (clientTransaction),
              Arg<DomainObject>.Is.Anything,
              Arg<IRelationEndPointDefinition>.Is.Anything,
              Arg<ReadOnlyDomainObjectCollectionAdapter<DomainObject>>.Is.Anything,
              Arg<ValueAccess>.Is.Anything));
    }
  }
}