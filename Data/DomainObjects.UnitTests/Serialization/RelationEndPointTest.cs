using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class RelationEndPointTest : ClientTransactionBaseTest
  {
    private RelationEndPoint _endPoint;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      RelationEndPointID id = new RelationEndPointID (DomainObjectIDs.Computer1, ReflectionUtility.GetPropertyName (typeof (Computer), "Employee"));
      _endPoint = new RelationEndPointStub (ClientTransactionMock, id);
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "Type 'Rubicon.Data.DomainObjects.DataManagemenet.RelationEndPoint' in Assembly "
        + ".* is not marked as serializable.", MatchType = MessageMatch.Regex)]
    [Ignore ("TODO: FS - after finishing flattened serializable implementation")]
    public void RelationEndPointIsNotSerializable ()
    {
      Serializer.SerializeAndDeserialize (_endPoint);
    }

    [Test]
    public void RelationEndPointIsFlattenedSerializable ()
    {
      RelationEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.IsNotNull (deserializedEndPoint);
      Assert.AreNotSame (_endPoint, deserializedEndPoint);
    }

    [Test]
    public void RelationEndPoint_Content ()
    {
      RelationEndPoint deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);
      Assert.AreSame (ClientTransactionMock, deserializedEndPoint.ClientTransaction);
      Assert.AreSame (_endPoint.Definition, deserializedEndPoint.Definition);
      Assert.AreEqual (_endPoint.ID, deserializedEndPoint.ID);
    }
  }
}