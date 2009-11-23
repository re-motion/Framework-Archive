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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Serialization
{
  [TestFixture]
  public class CollectionEndPointTest : ClientTransactionBaseTest
  {
    private CollectionEndPoint _endPoint;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      Dev.Null = Order.GetObject (DomainObjectIDs.Order1).OrderItems;
      _endPoint = (CollectionEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[
          new RelationEndPointID (DomainObjectIDs.Order1, MappingConfiguration.Current.NameResolver.GetPropertyName (typeof (Order), "OrderItems"))];
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "Type 'Remotion.Data.DomainObjects.DataManagement.CollectionEndPoint' in Assembly "
        + ".* is not marked as serializable.", MatchType = MessageMatch.Regex)]
    public void CollectionEndPointIsNotSerializable ()
    {
      Serializer.SerializeAndDeserialize (_endPoint);
    }

    [Test]
    public void CollectionEndPointIsFlattenedSerializable ()
    {
      CollectionEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.IsNotNull (deserializedEndPoint);
      Assert.AreNotSame (_endPoint, deserializedEndPoint);
    }

    [Test]
    public void CollectionEndPoint_Touched_Content ()
    {
      _endPoint.OppositeDomainObjects.Add (OrderItem.GetObject (DomainObjectIDs.OrderItem5));
      CollectionEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.AreSame (_endPoint.Definition, deserializedEndPoint.Definition);
      Assert.IsTrue (deserializedEndPoint.HasBeenTouched);

      Assert.AreEqual (3, deserializedEndPoint.OppositeDomainObjects.Count);
      Assert.IsTrue (deserializedEndPoint.OppositeDomainObjects.Contains (DomainObjectIDs.OrderItem1));
      Assert.IsTrue (deserializedEndPoint.OppositeDomainObjects.Contains (DomainObjectIDs.OrderItem2));
      Assert.IsTrue (deserializedEndPoint.OppositeDomainObjects.Contains (DomainObjectIDs.OrderItem5));
      Assert.IsFalse (deserializedEndPoint.OppositeDomainObjects.IsReadOnly);

      Assert.AreEqual (2, deserializedEndPoint.OriginalOppositeDomainObjectsContents.Count);
      Assert.IsTrue (deserializedEndPoint.OriginalOppositeDomainObjectsContents.Contains (DomainObjectIDs.OrderItem1));
      Assert.IsTrue (deserializedEndPoint.OriginalOppositeDomainObjectsContents.Contains (DomainObjectIDs.OrderItem2));
      Assert.IsTrue (deserializedEndPoint.OriginalOppositeDomainObjectsContents.IsReadOnly);
    }

    [Test]
    public void CollectionEndPoint_Untouched_Content ()
    {
      CollectionEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.IsFalse (deserializedEndPoint.HasBeenTouched);
    }

    [Test]
    public void CollectionEndPoint_DataStore ()
    {
      CollectionEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);

      var dataStore = deserializedEndPoint.CreateDelegatingCollectionData ().GetUndecoratedDataStore ();
      Assert.That (dataStore, Is.Not.Null);

      DomainObjectCollectionDataTestHelper.CheckAssociatedCollectionStrategy (
          deserializedEndPoint.OppositeDomainObjects, 
          typeof (OrderItem), 
          deserializedEndPoint, 
          dataStore);
    }

    [Test]
    public void CollectionEndPoint_ClientTransaction ()
    {
      CollectionEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.IsNotNull (deserializedEndPoint.ClientTransaction);
    }

    [Test]
    public void CollectionEndPoint_ReplacedCollection ()
    {
      var newOpposites = _endPoint.OppositeDomainObjects.Clone();
      _endPoint.ReplaceOppositeCollection (newOpposites);
      CollectionEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.That (deserializedEndPoint.HasChanged, Is.True);

      var deserializedNewOpposites = deserializedEndPoint.OppositeDomainObjects;
      deserializedEndPoint.Rollback ();
      
      Assert.That (deserializedEndPoint.HasChanged, Is.False);
      var deserializedOldOpposites = deserializedEndPoint.OppositeDomainObjects;
      Assert.That (deserializedOldOpposites, Is.Not.SameAs (deserializedNewOpposites));
      Assert.That (deserializedOldOpposites, Is.Not.Null);
    }

    [Test]
    public void CollectionEndPoint_ReplacedCollection_ReferenceEqualityWithOtherCollection ()
    {
      var industrialSector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1);
      var oldOpposites = industrialSector.Companies;
      var newOpposites = industrialSector.Companies.Clone ();
      industrialSector.Companies = newOpposites;

      var tuple = Tuple.NewTuple (ClientTransactionMock, industrialSector, oldOpposites, newOpposites);
      var deserializedTuple = Serializer.SerializeAndDeserialize (tuple);
      using (deserializedTuple.A.EnterDiscardingScope())
      {
        Assert.That (deserializedTuple.B.Companies, Is.SameAs (deserializedTuple.D));
        ClientTransaction.Current.Rollback();
        Assert.That (deserializedTuple.B.Companies, Is.SameAs (deserializedTuple.C));
      }
    }

    [Test]
    public void CollectionEndPoint_AssociatedEndPoint_OfOppositeCollection ()
    {
      var industrialSector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1);
      var oldOpposites = industrialSector.Companies;
      var newOpposites = industrialSector.Companies.Clone ();
      industrialSector.Companies = newOpposites;

      var tuple = Tuple.NewTuple (ClientTransactionMock, industrialSector, oldOpposites, newOpposites);
      var deserializedTuple = Serializer.SerializeAndDeserialize (tuple);
      using (deserializedTuple.A.EnterDiscardingScope ())
      {
        var propertyName = Configuration.NameResolver.GetPropertyName (typeof (IndustrialSector), "Companies");
        var endPointID = new RelationEndPointID (industrialSector.ID, propertyName);
        var endPoint = ((ClientTransactionMock)ClientTransaction.Current).DataManager.RelationEndPointMap[endPointID];
        Assert.That (deserializedTuple.B.Companies.AssociatedEndPoint, Is.SameAs (endPoint));

        ClientTransaction.Current.Rollback ();
        Assert.That (deserializedTuple.B.Companies.AssociatedEndPoint, Is.SameAs (endPoint));
      }
    }

    [Test]
    public void Serialization_IntegrationWithRelationEndPointMap ()
    {
      var deserializedTransactionMock = Serializer.SerializeAndDeserialize (ClientTransactionMock);
      var deserializedMap = deserializedTransactionMock.DataManager.RelationEndPointMap;
      var deserializedCollectionEndPoint = (CollectionEndPoint) deserializedMap.GetRelationEndPointWithLazyLoad (_endPoint.ID);
      Assert.That (deserializedCollectionEndPoint, Is.Not.Null);
    }
  }
}
