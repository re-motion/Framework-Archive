using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
[TestFixture]
public class RegisterInRelationEndPointMapTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  private RelationEndPointMap _endPoints;

  // construction and disposing

  public RegisterInRelationEndPointMapTest ()
  {
  }

  // methods and properties

  public override void SetUp ()
  {
    base.SetUp ();

    _endPoints = new RelationEndPointMap (ClientTransaction.Current);
  }

  [Test]
  public void DataContainerWithNoRelation ()
  {
    ObjectID id = new ObjectID (
        DatabaseTest.c_testDomainProviderID, "ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

    DataContainer container = TestDataContainerFactory.CreateClassWithAllDataTypesDataContainer ();
    _endPoints.Register (container);
    
    Assert.AreEqual (0, _endPoints.Count);
  }

  [Test]
  public void OrderTicket ()
  {
    DataContainer orderTicketContainer = TestDataContainerFactory.CreateOrderTicket1DataContainer ();
    _endPoints.Register (orderTicketContainer);
   
    Assert.AreEqual (2, _endPoints.Count, "Count");

    RelationEndPointID expectedEndPointIDForOrderTicket = new RelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");

    Assert.AreEqual (expectedEndPointIDForOrderTicket, 
        _endPoints[expectedEndPointIDForOrderTicket].ID, "RelationEndPointID for OrderTicket");
    
    Assert.AreEqual (
        DomainObjectIDs.Order1, 
        ((ObjectEndPoint) _endPoints[expectedEndPointIDForOrderTicket]).OppositeObjectID, 
        "OppositeObjectID for OrderTicket");

    RelationEndPointID expectedEndPointIDForOrder = new RelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");

    Assert.AreEqual (expectedEndPointIDForOrder, 
        _endPoints[expectedEndPointIDForOrder].ID, "RelationEndPointID for Order");
    
    Assert.AreEqual (
        DomainObjectIDs.OrderTicket1, 
        ((ObjectEndPoint) _endPoints[expectedEndPointIDForOrder]).OppositeObjectID, 
        "OppositeObjectID for Order");
  }

  [Test]
  public void VirtualEndPoint ()
  {
    DataContainer container = TestDataContainerFactory.CreateClassWithGuidKeyDataContainer ();
    _endPoints.Register (container);

    Assert.AreEqual (0, _endPoints.Count);
  }

  [Test]
  public void DerivedDataContainer ()
  {
    DataContainer distributorContainer = TestDataContainerFactory.CreateDistributor2DataContainer ();
    _endPoints.Register (distributorContainer);

    Assert.AreEqual (3, _endPoints.Count);
  }

  [Test]
  public void DataContainerWithOneToManyRelation ()
  {
    DataContainer orderContainer = TestDataContainerFactory.CreateOrder1DataContainer ();
    _endPoints.Register (orderContainer);

    Assert.AreEqual (2, _endPoints.Count, "Count");

    Assert.AreEqual (
        DomainObjectIDs.Customer1,
        ((ObjectEndPoint) _endPoints[new RelationEndPointID (DomainObjectIDs.Order1, "Customer")]).OppositeObjectID);

    Assert.AreEqual (DomainObjectIDs.Official1,
        ((ObjectEndPoint) _endPoints[new RelationEndPointID (DomainObjectIDs.Order1, "Official")]).OppositeObjectID);
  }
}
}
