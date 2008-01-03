using System;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class ObjectIDTest : StandardMappingTest
  {
    [Test]
    public void DeserializedContent_Value ()
    {
      ObjectID id = DomainObjectIDs.Order1;
      ObjectID deserielizedID = Serializer.SerializeAndDeserialize (id);
      Assert.AreEqual (id.Value, deserielizedID.Value);
    }

    [Test]
    public void DeserializedContent_ClassDefinition ()
    {
      ObjectID id = DomainObjectIDs.Order1;
      ObjectID deserielizedID = Serializer.SerializeAndDeserialize (id);
      Assert.AreSame (id.ClassDefinition, deserielizedID.ClassDefinition);
    }
  }
}