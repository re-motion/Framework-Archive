using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.Serialization
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
          new RelationEndPointID (DomainObjectIDs.Order1, ReflectionUtility.GetPropertyName (typeof (Order), "OrderItems"))];
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "Type 'Rubicon.Data.DomainObjects.DataManagement.CollectionEndPoint' in Assembly "
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
      Assert.AreSame (deserializedEndPoint, PrivateInvoke.GetNonPublicField (deserializedEndPoint.OppositeDomainObjects, "_changeDelegate"));

      Assert.AreEqual (2, deserializedEndPoint.OriginalOppositeDomainObjects.Count);
      Assert.IsTrue (deserializedEndPoint.OriginalOppositeDomainObjects.Contains (DomainObjectIDs.OrderItem1));
      Assert.IsTrue (deserializedEndPoint.OriginalOppositeDomainObjects.Contains (DomainObjectIDs.OrderItem2));
      Assert.IsTrue (deserializedEndPoint.OriginalOppositeDomainObjects.IsReadOnly);
      Assert.AreSame (deserializedEndPoint, PrivateInvoke.GetNonPublicField (deserializedEndPoint.OriginalOppositeDomainObjects, "_changeDelegate"));

      Assert.IsNull (deserializedEndPoint.ChangeDelegate);
    }

    [Test]
    public void CollectionEndPoint_Untouched_Content ()
    {
      CollectionEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.IsFalse (deserializedEndPoint.HasBeenTouched);
    }
  }
}