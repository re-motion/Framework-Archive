using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.DataManagement;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
[TestFixture]
public class RelationEndPointIDTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public RelationEndPointIDTest ()
  {
  }

  // methods and properties

  [Test]
  public void Initialize ()
  {
    ObjectID id = new ObjectID ("StorageProviderID", "ClassID", Guid.NewGuid ());
    RelationEndPointID endPointID = new RelationEndPointID (id, "PropertyName");

    Assert.AreEqual ("PropertyName", endPointID.PropertyName);
    Assert.AreEqual (id, endPointID.ObjectID);
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void InitializeWithInvalidPropertyName ()
  {
    ObjectID id = new ObjectID ("StorageProviderID", "ClassID", Guid.NewGuid ());
    RelationEndPointID endPointID = new RelationEndPointID (id, null);
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void InitializeWithInvalidObjectID ()
  {
    RelationEndPointID endPointID = new RelationEndPointID (null, "PropertyName");
  }

  [Test]
  public void HashCode ()
  {
    ObjectID id = new ObjectID ("StorageProviderID", "ClassID", Guid.NewGuid ());
    RelationEndPointID endPointID = new RelationEndPointID (id, "PropertyName");
    
    int expectedHashCode = id.GetHashCode () ^ "PropertyName".GetHashCode ();
    Assert.AreEqual (expectedHashCode, endPointID.GetHashCode ());
  }

  [Test]
  public void TestEquals ()
  {
    Guid guid = Guid.NewGuid ();

    ObjectID id1 = new ObjectID ("StorageProviderID", "ClassID", guid);
    RelationEndPointID endPointID1 = new RelationEndPointID (id1, "PropertyName");
  
    ObjectID id2 = new ObjectID ("StorageProviderID", "ClassID", guid);
    RelationEndPointID endPointID2 = new RelationEndPointID (id2, "PropertyName");

    Assert.IsTrue (endPointID1.Equals (endPointID2));
  }

  [Test]
  public void TestToString ()
  {
    ObjectID id = new ObjectID ("StorageProviderID", "ClassID", Guid.NewGuid ());
    RelationEndPointID endPointID = new RelationEndPointID (id, "PropertyName");
    
    string expected = id.ToString () + "/PropertyName";
    Assert.AreEqual (expected, endPointID.ToString ());
  }
}
}