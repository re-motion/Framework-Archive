using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
[TestFixture]
public class RelationEndPointIDTest
{
  // types

  // static members and constants

  // member fields

  private ObjectID _objectID;
  private string _propertyName;
  private RelationEndPointID _endPointID;

  // construction and disposing

  public RelationEndPointIDTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void SetUp ()
  {
    _objectID = DomainObjectIDs.Order1;
    _propertyName = "OrderTicket";
    _endPointID = new RelationEndPointID (_objectID, _propertyName);
  }

  [Test]
  public void Initialize ()
  {
    Assert.AreEqual (_propertyName, _endPointID.PropertyName);
    Assert.AreEqual (_objectID, _endPointID.ObjectID);
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void InitializeWithInvalidPropertyName ()
  {
    RelationEndPointID endPointID = new RelationEndPointID (_objectID, (string) null);
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void InitializeWithInvalidObjectID ()
  {
    RelationEndPointID endPointID = new RelationEndPointID (null, _propertyName);
  }


  [Test]
  [ExpectedException (typeof (MappingException))]
  public void InitializeWithInvalidClassID ()
  {
    ObjectID objectIDWithInvalidClass = new ObjectID (
        DatabaseTest.c_testDomainProviderID, "InvalidClassID", Guid.NewGuid ());

    RelationEndPointID invalidEndPointID = new RelationEndPointID (objectIDWithInvalidClass, "PropertyName");
  }

  [Test]
  public void HashCode ()
  {
    int expectedHashCode = _objectID.GetHashCode () ^ _propertyName.GetHashCode ();
    Assert.AreEqual (expectedHashCode, _endPointID.GetHashCode ());
  }

  [Test]
  public void TestEquals ()
  {
    RelationEndPointID endPointID2 = new RelationEndPointID (_objectID, _propertyName);

    Assert.IsTrue (_endPointID.Equals (endPointID2));
  }

  [Test]
  public void TestEqualsForObjectID ()
  {
    RelationEndPointID endPointID2 = new RelationEndPointID (ObjectID.Parse (_objectID.ToString ()), _propertyName);
    RelationEndPointID endPointID3 = new RelationEndPointID (DomainObjectIDs.Order2, _propertyName);

    Assert.IsTrue (_endPointID.Equals (endPointID2));
    Assert.IsTrue (endPointID2.Equals (_endPointID));
    Assert.IsFalse (_endPointID.Equals (endPointID3));
    Assert.IsFalse (endPointID3.Equals (_endPointID));
    Assert.IsFalse (endPointID2.Equals (endPointID3));
    Assert.IsFalse (endPointID3.Equals (endPointID2));
  }

  [Test]
  public void TestEqualsWithOtherType ()
  {
    Assert.IsFalse (_endPointID.Equals (new RelationEndPointIDTest ()));
  }

  [Test]
  public void TestEqualsWithNull ()
  {
    Assert.IsFalse (_endPointID.Equals (null));
  }

  [Test]
  public void TestToString ()
  {
    string expected = _objectID.ToString () + "/" + _propertyName;
    Assert.AreEqual (expected, _endPointID.ToString ());
  }
}
}