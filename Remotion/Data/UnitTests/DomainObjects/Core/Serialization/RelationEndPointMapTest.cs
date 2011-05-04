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
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Serialization
{
  [TestFixture]
  public class RelationEndPointMapTest : ClientTransactionBaseTest
  {
    private RelationEndPointMap _relationEndPointMap;

    public override void SetUp ()
    {
      base.SetUp ();
      _relationEndPointMap = (RelationEndPointMap) ClientTransactionMock.DataManager.RelationEndPointMap;
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = 
        "Type 'Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.RelationEndPointMap' in Assembly "
        + ".* is not marked as serializable.", MatchType = MessageMatch.Regex)]
    public void RelationEndPointMapIsNotSerializable ()
    {
      Serializer.SerializeAndDeserialize (_relationEndPointMap);
    }

    [Test]
    public void RelationEndPointMapIsFlattenedSerializable ()
    {
      RelationEndPointMap deserializedMap = FlattenedSerializer.SerializeAndDeserialize (_relationEndPointMap);
      Assert.That (deserializedMap, Is.Not.Null);
      Assert.That (deserializedMap, Is.Not.SameAs (_relationEndPointMap));
    }

    [Test]
    public void RelationEndPointMap_Content ()
    {
      Dev.Null = Order.GetObject (DomainObjectIDs.Order1).OrderItems;
      Assert.That (_relationEndPointMap.Count, Is.EqualTo (7));

      var deserializedMap = (RelationEndPointMap) Serializer.SerializeAndDeserialize (ClientTransactionMock.DataManager).RelationEndPointMap;

      Assert.That (deserializedMap.ClientTransaction, Is.Not.Null);
      Assert.That (deserializedMap.ClientTransaction, Is.InstanceOf (typeof (ClientTransactionMock)));
      Assert.That (deserializedMap.ClientTransaction, Is.Not.SameAs (ClientTransactionMock));

      var deserializedDataManager = ClientTransactionTestHelper.GetDataManager (deserializedMap.ClientTransaction);

      var deserializedAgent = (RelationEndPointRegistrationAgent) RelationEndPointMapTestHelper.GetRegistrationAgent (deserializedMap);
      Assert.That (deserializedAgent, Is.Not.Null);
      Assert.That (deserializedAgent.EndPointProvider, Is.SameAs (deserializedMap.EndPointProvider));
      Assert.That (deserializedAgent.ClientTransaction, Is.SameAs (deserializedMap.ClientTransaction));
      Assert.That (PrivateInvoke.GetNonPublicField (deserializedAgent, "_relationEndPoints"), Is.SameAs (RelationEndPointMapTestHelper.GetMap (deserializedMap)));
      Assert.That (deserializedMap.LazyLoader, Is.SameAs (deserializedDataManager));
      Assert.That (deserializedMap.EndPointProvider, Is.SameAs (deserializedDataManager));
      Assert.That (deserializedMap.CollectionEndPointDataKeeperFactory, Is.TypeOf (_relationEndPointMap.CollectionEndPointDataKeeperFactory.GetType ()));
      Assert.That (deserializedMap.VirtualObjectEndPointDataKeeperFactory, Is.TypeOf (_relationEndPointMap.VirtualObjectEndPointDataKeeperFactory.GetType ()));

      Assert.That (deserializedMap.Count, Is.EqualTo (7));

      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, ReflectionMappingHelper.GetPropertyName (typeof (Order), "OrderItems"));
      var endPoint = (CollectionEndPoint) deserializedMap.GetRelationEndPointWithoutLoading (endPointID);

      Assert.That (endPoint.ClientTransaction, Is.SameAs (deserializedMap.ClientTransaction));
    }
  }
}
