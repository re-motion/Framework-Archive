using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.ObjectIDs
{
[TestFixture]
public class ObjectIDTest
{
  // types

  private class InvalidDomainObject : DomainObject
  {
  }

  // static members and constants

  // member fields

  // construction and disposing

  public ObjectIDTest ()
  {
  }

  // methods and properties

  [Test]
  public void SerializeStringValue ()
  {
    ObjectID id = new ObjectID ("Order", "Arthur Dent");
    Assert.AreEqual ("Order|Arthur Dent|System.String", id.ToString ());
  }

  [Test]
  public void SerializeInt32Value ()
  {
    ObjectID id = new ObjectID ("Order", 42);
    Assert.AreEqual ("Order|42|System.Int32", id.ToString ());
  }

  [Test]
  public void SerializeGuidValue ()
  {
    ObjectID id = new ObjectID ("Order", new Guid ("{5D09030C-25C2-4735-B514-46333BD28AC8}"));
    Assert.AreEqual ("Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid", id.ToString ());
  }

  [Test]
  [ExpectedException (typeof (ArgumentException), "Value cannot contain '&amp;pipe;'.\r\nParameter name: value")]
  public void EscapedDelimiterPlaceholderInValue ()
  {
    ObjectID id = new ObjectID ("Order", "Arthur|Dent &pipe; &amp;pipe; Zaphod Beeblebrox");
  }

  [Test]
  public void DeserializeStringValue ()
  {
    string idString = "Order|Arthur Dent|System.String";
    ObjectID id = ObjectID.Parse (idString);

    Assert.AreEqual ("TestDomain", id.StorageProviderID);
    Assert.AreEqual ("Order", id.ClassID);
    Assert.AreEqual (typeof (string), id.Value.GetType());
    Assert.AreEqual ("Arthur Dent", id.Value);
  }

  [Test]
  public void DeserializeInt32Value ()
  {
    string idString = "Order|42|System.Int32";
    ObjectID id = ObjectID.Parse (idString);

    Assert.AreEqual ("TestDomain", id.StorageProviderID);
    Assert.AreEqual ("Order", id.ClassID);
    Assert.AreEqual (typeof (int), id.Value.GetType());
    Assert.AreEqual (42, id.Value);
  }

  [Test]
  public void DeserializeGuidValue ()
  {
    string idString = "Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid";
    ObjectID id = ObjectID.Parse (idString);

    Assert.AreEqual ("TestDomain", id.StorageProviderID);
    Assert.AreEqual ("Order", id.ClassID);
    Assert.AreEqual (typeof (Guid), id.Value.GetType());
    Assert.AreEqual (new Guid("{5D09030C-25C2-4735-B514-46333BD28AC8}"), id.Value);
  }

  [Test]
  [ExpectedException (typeof (FormatException),
      "Serialized ObjectID 'Order|5d09030c-25"
       + "c2-4735-b514-46333bd28ac8|System.Guid|Zaphod' is not correctly formatted.")]
  public void ObjectIDStringWithTooManyParts ()
  {
    string idString = "Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid|Zaphod";
    ObjectID id = ObjectID.Parse (idString);    
  }

  [Test]
  [ExpectedException (typeof (FormatException), "Type 'System.Double' is not supported.")]
  public void ObjectIDStringWithInvalidValueType ()
  {
    string idString = "Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Double";
    ObjectID id = ObjectID.Parse (idString);
  }

  [Test]
  public void HashCode ()
  {
    ObjectID id1 = new ObjectID ("Order", 42);
    ObjectID id2 = new ObjectID ("Order", 42);
    ObjectID id3 = new ObjectID ("Order", 41);

    Assert.IsTrue (id1.GetHashCode() == id2.GetHashCode());
    Assert.IsFalse (id1.GetHashCode() == id3.GetHashCode());
    Assert.IsFalse (id2.GetHashCode() == id3.GetHashCode());
  }

  [Test]
  public void TestEqualsForClassID ()
  {
    ObjectID id1 = new ObjectID ("Order", 42);
    ObjectID id2 = new ObjectID ("Order", 42);
    ObjectID id3 = new ObjectID ("Customer", 42);

    Assert.IsTrue (id1.Equals (id2));
    Assert.IsFalse (id1.Equals (id3));
    Assert.IsFalse (id2.Equals (id3));
    Assert.IsTrue (id2.Equals (id1));
    Assert.IsFalse (id3.Equals (id1));
    Assert.IsFalse (id3.Equals (id2));
  }

  [Test]
  public void TestEqualsForValue ()
  {
    ObjectID id1 = new ObjectID ("Order", 42);
    ObjectID id2 = new ObjectID ("Order", 42);
    ObjectID id3 = new ObjectID ("Order", 41);

    Assert.IsTrue (id1.Equals (id2));
    Assert.IsFalse (id1.Equals (id3));
    Assert.IsFalse (id2.Equals (id3));
    Assert.IsTrue (id2.Equals (id1));
    Assert.IsFalse (id3.Equals (id1));
    Assert.IsFalse (id3.Equals (id2));
  }

  [Test]
  public void EqualsWithOtherType ()
  {
    ObjectID id = new ObjectID ("Order", 42);
    Assert.IsFalse (id.Equals (new ObjectIDTest ()));
    Assert.IsFalse (id.Equals (42));
  }

  [Test]
  public void EqualsWithNull ()
  {
    ObjectID id = new ObjectID ("Order", 42);
    Assert.IsFalse (id.Equals (null));
  }

  [Test]
  public void EqualityOperatorTrue ()
  {
    ObjectID id1 = new ObjectID ("Order", 42);
    ObjectID id2 = new ObjectID ("Order", 42);

    Assert.IsTrue (id1 == id2);
    Assert.IsFalse (id1 != id2);
  }

  [Test]
  public void EqualityOperatorFalse ()
  {
    ObjectID id1 = new ObjectID ("Order", 42);
    ObjectID id2 = new ObjectID ("Customer", 1);

    Assert.IsFalse (id1 == id2);
    Assert.IsTrue (id1 != id2);
  }

  [Test]
  public void EqualityOperatorForSameObject ()
  {
    ObjectID id = new ObjectID ("Order", 42);

    Assert.IsTrue (id == id);
    Assert.IsFalse (id != id);
  }

  [Test]
  public void EqualityOperatorWithBothNull ()
  {
    Assert.IsTrue ((ObjectID) null == (ObjectID) null);
    Assert.IsFalse ((ObjectID) null != (ObjectID) null);

  }
 
  [Test]
  public void EqualityOperatorID1Null ()
  {
    ObjectID id2 = new ObjectID ("Order", 42);

    Assert.IsFalse (null == id2);
    Assert.IsTrue (null != id2);
  }

  [Test]
  public void EqualityOperatorID2Null ()
  {
    ObjectID id1 = new ObjectID ("Order", 42);

    Assert.IsFalse (id1 == null);
    Assert.IsTrue (id1 != null);
  }

  [Test]
  public void StaticEquals ()
  {
    ObjectID id1 = new ObjectID ("Order", 42);
    ObjectID id2 = new ObjectID ("Order", 42);

    Assert.IsTrue (ObjectID.Equals (id1, id2));
  }

  [Test]
  public void StaticNotEquals ()
  {
    ObjectID id1 = new ObjectID ("Order", 42);
    ObjectID id2 = new ObjectID ("Customer", 1);

    Assert.IsFalse (ObjectID.Equals (id1, id2));
  }

  [Test]
  public void InitializeWithClassID ()
  {
    Guid value = new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C36}");
    ObjectID id = new ObjectID ("Order", value);

    Assert.AreEqual ("TestDomain", id.StorageProviderID);
    Assert.AreEqual ("Order", id.ClassID);
    Assert.AreEqual (value, id.Value);
  }

  [Test]
  public void InitializeWithClassType ()
  {
    Guid value = new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C36}");
    ObjectID id = new ObjectID (typeof (Order), value);

    Assert.AreEqual ("TestDomain", id.StorageProviderID);
    Assert.AreEqual ("Order", id.ClassID);
    Assert.AreEqual (value, id.Value);
  }

  [Test]
  public void InitializeWithClassDefinition ()
  {
    ClassDefinition orderDefinition = MappingConfiguration.Current.ClassDefinitions["Order"];
    Guid value = new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C36}");

    ObjectID id = new ObjectID (orderDefinition, value);

    Assert.AreEqual (orderDefinition.StorageProviderID, id.StorageProviderID);
    Assert.AreEqual (orderDefinition.ID, id.ClassID);
    Assert.AreEqual (value, id.Value);
  }

  [Test]
  [ExpectedException (typeof (MappingException))]
  public void InitializeWithUnknownClassDefinitionID ()
  {
    ClassDefinition unknownDefinition = new ClassDefinition ("UnknownClass", "UnknownTable", typeof (Order), "TestDomain");
    Guid value = new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C36}");

    ObjectID id = new ObjectID (unknownDefinition, value);
  }

  [Test]
  [ExpectedException (typeof (MappingException))]
  public void InitializeWithUnknownClassDefinitionType ()
  {
    ClassDefinition unknownDefinition = new ClassDefinition ("Order", "Order", typeof (InvalidDomainObject), "TestDomain");
    Guid value = new Guid ("{5682F032-2F0B-494b-A31C-C97F02B89C36}");

    ObjectID id = new ObjectID (unknownDefinition, value);
  }

  [Test]
  [ExpectedException (typeof (ArgumentEmptyException))]
  public void InitializeWithEmptyGuid ()
  {
    ObjectID id = new ObjectID (MappingConfiguration.Current.ClassDefinitions["Order"], Guid.Empty);
  }

  [Test]
  [ExpectedException (typeof (ArgumentEmptyException))]
  public void InitializeWithEmptyString ()
  {
    ObjectID id = new ObjectID (MappingConfiguration.Current.ClassDefinitions["Order"], string.Empty);
  }

  [Test]
  public void Serialization ()
  {
    using (MemoryStream memoryStream = new MemoryStream ())
    {
      BinaryFormatter formatter = new BinaryFormatter();
      formatter.Serialize (memoryStream, DomainObjectIDs.Order1);
      memoryStream.Seek (0, SeekOrigin.Begin);

      formatter = new BinaryFormatter();

      ObjectID id = (ObjectID) formatter.Deserialize (memoryStream);
      
      Assert.AreEqual (DomainObjectIDs.Order1, id);
    }    
  }

  [Test]
  [ExpectedException (typeof (ArgumentException), 
      "The ClassID 'Order' and the ClassType 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer'"
      + " do not refer to the same ClassDefinition in the mapping configuration.\r\nParameter name: classDefinition")]
  public void InitializeWithInvalidClassDefinition ()
  {
    ClassDefinition invalidDefinition = new ClassDefinition ("Order", "Order", typeof (Customer), "TestDomain");
    ObjectID id = new ObjectID (invalidDefinition, Guid.NewGuid ());
  }

  [Test]
  [ExpectedException (typeof (ArgumentException), 
      "The provided ClassDefinition 'Order' is not the same reference as the ClassDefinition found in the mapping configuration.\r\nParameter name: classDefinition")]
  public void InitializeWithClassDefinitionNotPartOfMappingConfiguration ()
  {
    ClassDefinition invalidDefinition = new ClassDefinition ("Order", "Order", typeof (Order), "TestDomain");
    ObjectID id = new ObjectID (invalidDefinition, Guid.NewGuid ());
  }
}
}
