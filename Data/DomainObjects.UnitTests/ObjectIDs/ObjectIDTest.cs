using System;
using NUnit.Framework;

namespace Rubicon.Data.DomainObjects.UnitTests.ObjectIDs
{
[TestFixture]
public class ObjectIDTest
{
  // types

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
    ObjectID id = new ObjectID ("sqlserver1", "Order", "Arthur Dent");
    Assert.AreEqual ("sqlserver1|Order|Arthur Dent|System.String", id.ToString ());
  }

  [Test]
  public void SerializeInt32Value ()
  {
    ObjectID id = new ObjectID ("sqlserver1", "Order", 42);
    Assert.AreEqual ("sqlserver1|Order|42|System.Int32", id.ToString ());
  }

  [Test]
  public void SerializeGuidValue ()
  {
    ObjectID id = new ObjectID ("sqlserver1", "Order", new Guid ("{5D09030C-25C2-4735-B514-46333BD28AC8}"));
    Assert.AreEqual ("sqlserver1|Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid", id.ToString ());
  }

  [Test]
  [ExpectedException (typeof (ArgumentException), "Value cannot contain '&amp;pipe;'.\r\nParameter name: value")]
  public void EscapedDelimiterPlaceholderInValue ()
  {
    ObjectID id = new ObjectID ("sqlserver1", "Order", "Arthur|Dent &pipe; &amp;pipe; Zaphod Beeblebrox");
  }

  [Test]
  [ExpectedException (typeof (ArgumentException), "Type 'System.Double' is not supported.\r\nParameter name: value")]
  public void InvalidValueType ()
  {
    double value = 42.4242424242;
    ObjectID id = new ObjectID ("sqlserver2", "Order", value);
  }

  [Test]
  public void DeserializeStringValue ()
  {
    string idString = "sqlserver1|Order|Arthur Dent|System.String";
    ObjectID id = ObjectID.Parse (idString);

    Assert.AreEqual ("sqlserver1", id.StorageProviderID);
    Assert.AreEqual ("Order", id.ClassID);
    Assert.AreEqual (typeof (string), id.Value.GetType());
    Assert.AreEqual ("Arthur Dent", id.Value);
  }

  [Test]
  public void DeserializeInt32Value ()
  {
    string idString = "sqlserver1|Order|42|System.Int32";
    ObjectID id = ObjectID.Parse (idString);

    Assert.AreEqual ("sqlserver1", id.StorageProviderID);
    Assert.AreEqual ("Order", id.ClassID);
    Assert.AreEqual (typeof (int), id.Value.GetType());
    Assert.AreEqual (42, id.Value);
  }

  [Test]
  public void DeserializeGuidValue ()
  {
    string idString = "sqlserver1|Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid";
    ObjectID id = ObjectID.Parse (idString);

    Assert.AreEqual ("sqlserver1", id.StorageProviderID);
    Assert.AreEqual ("Order", id.ClassID);
    Assert.AreEqual (typeof (Guid), id.Value.GetType());
    Assert.AreEqual (new Guid("{5D09030C-25C2-4735-B514-46333BD28AC8}"), id.Value);
  }

  [Test]
  [ExpectedException (typeof (FormatException),
      "Serialized ObjectID 'sqlserver1|Order|5d09030c-25"
       + "c2-4735-b514-46333bd28ac8|System.Guid|Zaphod' is not correctly formatted.")]
  public void ObjectIDStringWithTooManyParts ()
  {
    string idString = "sqlserver1|Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid|Zaphod";
    ObjectID id = ObjectID.Parse (idString);    
  }

  [Test]
  [ExpectedException (typeof (FormatException), "Type 'System.Double' is not supported.")]
  public void ObjectIDStringWithInvalidValueType ()
  {
    string idString = "sqlserver1|Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Double";
    ObjectID id = ObjectID.Parse (idString);
  }

  [Test]
  public void HashCode ()
  {
    ObjectID id1 = new ObjectID ("sqlserver1", "Order", 42);
    ObjectID id2 = new ObjectID ("sqlserver1", "Order", 42);
    ObjectID id3 = new ObjectID ("sqlserver1", "Order", 41);

    Assert.IsTrue (id1.GetHashCode() == id2.GetHashCode());
    Assert.IsFalse (id1.GetHashCode() == id3.GetHashCode());
    Assert.IsFalse (id2.GetHashCode() == id3.GetHashCode());
  }

  [Test]
  public void TestEqualsForStorageProvider ()
  {
    ObjectID id1 = new ObjectID ("sqlserver1", "Order", 42);
    ObjectID id2 = new ObjectID ("sqlserver1", "Order", 42);
    ObjectID id3 = new ObjectID ("sqlserver2", "Order", 42);

    Assert.IsTrue (id1.Equals (id2));
    Assert.IsFalse (id1.Equals (id3));
    Assert.IsFalse (id2.Equals (id3));
    Assert.IsTrue (id2.Equals (id1));
    Assert.IsFalse (id3.Equals (id1));
    Assert.IsFalse (id3.Equals (id2));
  }

  [Test]
  public void TestEqualsForClassID ()
  {
    ObjectID id1 = new ObjectID ("sqlserver1", "Order", 42);
    ObjectID id2 = new ObjectID ("sqlserver1", "Order", 42);
    ObjectID id3 = new ObjectID ("sqlserver1", "Customer", 42);

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
    ObjectID id1 = new ObjectID ("sqlserver1", "Order", 42);
    ObjectID id2 = new ObjectID ("sqlserver1", "Order", 42);
    ObjectID id3 = new ObjectID ("sqlserver1", "Order", 41);

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
    ObjectID id = new ObjectID ("sqlserver1", "Order", 42);
    Assert.IsFalse (id.Equals (new ObjectIDTest ()));
    Assert.IsFalse (id.Equals (42));
  }

  [Test]
  public void EqualsWithNull ()
  {
    ObjectID id = new ObjectID ("sqlserver1", "Order", 42);
    Assert.IsFalse (id.Equals (null));
  }

  [Test]
  public void EqualityOperatorTrue ()
  {
    ObjectID id1 = new ObjectID ("sqlserver1", "Order", 42);
    ObjectID id2 = new ObjectID ("sqlserver1", "Order", 42);

    Assert.IsTrue (id1 == id2);
    Assert.IsFalse (id1 != id2);
  }

  [Test]
  public void EqualityOperatorFalse ()
  {
    ObjectID id1 = new ObjectID ("sqlserver1", "Order", 42);
    ObjectID id2 = new ObjectID ("sqlserver2", "Customer", 1);

    Assert.IsFalse (id1 == id2);
    Assert.IsTrue (id1 != id2);
  }

  [Test]
  public void EqualityOperatorForSameObject ()
  {
    ObjectID id = new ObjectID ("sqlserver1", "Order", 42);

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
    ObjectID id2 = new ObjectID ("sqlserver1", "Order", 42);

    Assert.IsFalse (null == id2);
    Assert.IsTrue (null != id2);
  }

  [Test]
  public void EqualityOperatorID2Null ()
  {
    ObjectID id1 = new ObjectID ("sqlserver1", "Order", 42);

    Assert.IsFalse (id1 == null);
    Assert.IsTrue (id1 != null);
  }

  [Test]
  public void StaticEquals ()
  {
    ObjectID id1 = new ObjectID ("sqlserver1", "Order", 42);
    ObjectID id2 = new ObjectID ("sqlserver1", "Order", 42);

    Assert.IsTrue (ObjectID.Equals (id1, id2));
  }

  [Test]
  public void StaticNotEquals ()
  {
    ObjectID id1 = new ObjectID ("sqlserver1", "Order", 42);
    ObjectID id2 = new ObjectID ("sqlserver2", "Customer", 1);

    Assert.IsFalse (ObjectID.Equals (id1, id2));
  }
}
}
