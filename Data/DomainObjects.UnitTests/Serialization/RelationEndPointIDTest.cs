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
  public class RelationEndPointIDTest : StandardMappingTest
  {
    private RelationEndPointID _id;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _id = new RelationEndPointID (DomainObjectIDs.Computer1, ReflectionUtility.GetPropertyName (typeof (Computer), "Employee"));
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "Type 'Rubicon.Data.DomainObjects.DataManagemenet.RelationEndPointID' in Assembly "
        + ".* is not marked as serializable.", MatchType = MessageMatch.Regex)]
    [Ignore ("TODO: FS - after finishing flattened serializable implementation")]
    public void RelationEndPointIDIsNotSerializable ()
    {
      Serializer.SerializeAndDeserialize (_id);
    }

    [Test]
    public void RelationEndPointIDIsFlattenedSerializable ()
    {
      RelationEndPointID deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_id);
      Assert.IsNotNull (deserializedEndPoint);
      Assert.AreNotSame (_id, deserializedEndPoint);
    }

    [Test]
    public void RelationEndPointID_Content ()
    {
      RelationEndPointID deserializedID = FlattenedSerializer.SerializeAndDeserialize (_id);
      Assert.AreEqual (DomainObjectIDs.Computer1, deserializedID.ObjectID);
      Assert.AreSame (_id.Definition, deserializedID.Definition);
    }
  }
}