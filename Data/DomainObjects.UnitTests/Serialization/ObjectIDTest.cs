using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class ObjectIDTest : StandardMappingTest
  {
    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "Type 'Rubicon.Data.DomainObjects.ObjectID' in Assembly "
        + "'Rubicon.Data.DomainObjects, Version=1.7.65.202, Culture=neutral, PublicKeyToken=ad97c3e83e217fcd' is not marked as serializable.")]
    [Ignore ("TODO: FS - after finishing flattened serializable implementation")]
    public void ObjectIDIsNotSerializable ()
    {
      Serializer.SerializeAndDeserialize (DomainObjectIDs.Order1);
    }

    [Test]
    public void ObjectIDIsFlattenedSerializable ()
    {
      DomainObjectSerializationInfo info = new DomainObjectSerializationInfo ();
      ObjectID id = DomainObjectIDs.Order1;
      ((IFlattenedSerializable) id).SerializeIntoFlatStructure (info);

      DomainObjectDeserializationInfo deserializationInfo = new DomainObjectDeserializationInfo (info.GetData ());
      ObjectID deserializedID = ObjectID.DeserializeFromFlatStructure (deserializationInfo);

      Assert.IsNotNull (deserializedID);
    }

    [Test]
    public void DeserializedContent_Value ()
    {
      DomainObjectSerializationInfo info = new DomainObjectSerializationInfo();
      ObjectID id = DomainObjectIDs.Order1;
      ((IFlattenedSerializable)id).SerializeIntoFlatStructure (info);

      DomainObjectDeserializationInfo deserializationInfo = new DomainObjectDeserializationInfo (info.GetData());
      ObjectID deserializedID = ObjectID.DeserializeFromFlatStructure (deserializationInfo);
      
      Assert.AreEqual (id.Value, deserializedID.Value);
    }

    [Test]
    public void DeserializedContent_ClassDefinition ()
    {
      DomainObjectSerializationInfo info = new DomainObjectSerializationInfo ();
      ObjectID id = DomainObjectIDs.Order1;
      ((IFlattenedSerializable) id).SerializeIntoFlatStructure (info);

      DomainObjectDeserializationInfo deserializationInfo = new DomainObjectDeserializationInfo (info.GetData ());
      ObjectID deserializedID = ObjectID.DeserializeFromFlatStructure (deserializationInfo);

      Assert.AreEqual (id.ClassDefinition, deserializedID.ClassDefinition);
    }
  }
}