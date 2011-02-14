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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.ObjectEndPointDataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Serialization;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.ObjectEndPointDataManagement
{
  [TestFixture]
  public class UnsynchronizedObjectEndPointSyncStateTest : StandardMappingTest
  {
    private IObjectEndPoint _endPointStub;
    private UnsynchronizedObjectEndPointSyncState _state;
    private IRelationEndPointDefinition _orderOrderTicketEndPointDefinition;

    private Action<ObjectID> _fakeSetter;

    public override void SetUp ()
    {
      base.SetUp ();

      _orderOrderTicketEndPointDefinition = GetRelationEndPointDefinition (typeof (Order), "OrderTicket");
      
      _endPointStub = MockRepository.GenerateStub<IObjectEndPoint> ();
      _endPointStub.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);
      _endPointStub.Stub (stub => stub.Definition).Return (_orderOrderTicketEndPointDefinition);
      
      _state = new UnsynchronizedObjectEndPointSyncState ();
      _fakeSetter = id => { };
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
      "The relation property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket' of object "
      + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be changed because it is "
      + "out of sync with the opposite property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order'. To make this change, "
      + "synchronize the two properties by calling the 'ClientTransactionSyncService.SynchronizeRelation' method.")]
    public void CreateDeleteCommand ()
    {
      _state.CreateDeleteCommand(_endPointStub, _fakeSetter);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
      "The relation property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket' of object "
      + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be changed because it is "
      + "out of sync with the opposite property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderTicket.Order'. To make this change, "
      + "synchronize the two properties by calling the 'ClientTransactionSyncService.SynchronizeRelation' method.")]
    public void CreateSetCommand ()
    {
      var relatedObject = DomainObjectMother.CreateFakeObject<OrderTicket> ();

      _state.CreateSetCommand (_endPointStub, relatedObject, _fakeSetter);
    }

    [Test]
    public void FlattenedSerializable ()
    {
      var state = new UnsynchronizedObjectEndPointSyncState ();

      var result = FlattenedSerializer.SerializeAndDeserialize (state);

      Assert.That (result, Is.Not.Null);
    }

    private IRelationEndPointDefinition GetRelationEndPointDefinition (Type classType, string shortPropertyName)
    {
      return Configuration.ClassDefinitions.GetMandatory (classType).GetRelationEndPointDefinition (classType.FullName + "." + shortPropertyName);
    }
  }
}