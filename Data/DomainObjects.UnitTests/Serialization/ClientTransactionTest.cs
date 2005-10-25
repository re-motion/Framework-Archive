using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.UnitTests.Serialization
{
[TestFixture]
public class ClientTransactionTest : SerializationBaseTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public ClientTransactionTest ()
  {
  }

  // methods and properties

  [Test]
  public void ObjectIDTest ()
  {
    ObjectID objectID = new ObjectID ("Company", Guid.NewGuid ());

    ObjectID deserializedObjectID = (ObjectID) SerializeAndDeserialize (objectID);

    Assert.AreEqual (objectID, deserializedObjectID);
    Assert.AreEqual (objectID.Value.GetType (), deserializedObjectID.Value.GetType ());
    Assert.AreSame (objectID.ClassDefinition, deserializedObjectID.ClassDefinition);
    Assert.AreSame (MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Company"), deserializedObjectID.ClassDefinition);
  }
}
}
