using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.DataManagement;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
[TestFixture]
public class RelationLinkIDTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public RelationLinkIDTest ()
  {
  }

  // methods and properties

  [Test]
  public void Initialize ()
  {
    ObjectID id = new ObjectID ("StorageProviderID", "ClassID", Guid.NewGuid ());
    RelationLinkID linkID = new RelationLinkID (id, "PropertyName");

    Assert.AreEqual ("PropertyName", linkID.PropertyName);
    Assert.AreEqual (id, linkID.ObjectID);
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void InitializeWithInvalidPropertyName ()
  {
    ObjectID id = new ObjectID ("StorageProviderID", "ClassID", Guid.NewGuid ());
    RelationLinkID linkID = new RelationLinkID (id, null);
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void InitializeWithInvalidObjectID ()
  {
    RelationLinkID linkID = new RelationLinkID (null, "PropertyName");
  }

  [Test]
  public void HashCode ()
  {
    ObjectID id = new ObjectID ("StorageProviderID", "ClassID", Guid.NewGuid ());
    RelationLinkID linkID = new RelationLinkID (id, "PropertyName");
    
    int expectedHashCode = id.GetHashCode () ^ "PropertyName".GetHashCode ();
    Assert.AreEqual (expectedHashCode, linkID.GetHashCode ());
  }

  [Test]
  public void TestEquals ()
  {
    Guid guid = Guid.NewGuid ();

    ObjectID id1 = new ObjectID ("StorageProviderID", "ClassID", guid);
    RelationLinkID linkID1 = new RelationLinkID (id1, "PropertyName");
  
    ObjectID id2 = new ObjectID ("StorageProviderID", "ClassID", guid);
    RelationLinkID linkID2 = new RelationLinkID (id2, "PropertyName");

    Assert.IsTrue (linkID1.Equals (linkID2));
  }

  [Test]
  public void TestToString ()
  {
    ObjectID id = new ObjectID ("StorageProviderID", "ClassID", Guid.NewGuid ());
    RelationLinkID linkID = new RelationLinkID (id, "PropertyName");
    
    string expected = id.ToString () + "/PropertyName";
    Assert.AreEqual (expected, linkID.ToString ());
  }
}
}