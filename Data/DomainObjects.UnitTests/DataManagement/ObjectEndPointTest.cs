using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
[TestFixture]
public class ObjectEndPointTest : RelationEndPointBaseTest
{
  // types

  // static members and constants

  // member fields

  private RelationEndPointID _endPointID;
  private ObjectEndPoint _endPoint;
  private ObjectID _oppositeObjectID;

  // construction and disposing

  public ObjectEndPointTest ()
  {
  }

  // methods and properties

  public override void SetUp ()
  {
    _endPointID = new RelationEndPointID (DomainObjectIDs.OrderItem1, "Order");
    _oppositeObjectID = DomainObjectIDs.Order1;
  
    _endPoint = CreateObjectEndPoint (_endPointID, _oppositeObjectID);    
  }

  [Test]
  public void Initialize ()
  {
    Assert.AreEqual (_endPointID, _endPoint.ID);
    Assert.AreEqual (_oppositeObjectID, _endPoint.OriginalOppositeObjectID);
    Assert.AreEqual (_oppositeObjectID, _endPoint.OppositeObjectID);
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void InitializeWithInvalidRelationEndPointID ()
  {
    ObjectID id = new ObjectID ("StorageProverID", "ClassID", Guid.NewGuid ());
    ObjectEndPoint endPoint = CreateObjectEndPoint (null, id);
  }

  [Test]
  public void InitializeWithNullObjectID ()
  {
    ObjectEndPoint endPoint = CreateObjectEndPoint (_endPointID, null);

    Assert.IsNull (endPoint.OriginalOppositeObjectID);
    Assert.IsNull (endPoint.OppositeObjectID);
  }

  [Test]
  public void ChangeOppositeObjectID ()
  {
    ObjectID newObjectID = new ObjectID ("StorageProverID", "ClassID", Guid.NewGuid ());
    _endPoint.OppositeObjectID = newObjectID;

    Assert.AreSame (newObjectID, _endPoint.OppositeObjectID);
    Assert.AreEqual (_oppositeObjectID, _endPoint.OriginalOppositeObjectID);
  }

  [Test]
  public void HasChanged ()
  {
    Assert.IsFalse (_endPoint.HasChanged);

    _endPoint.OppositeObjectID = new ObjectID ("StorageProverID", "ClassID", Guid.NewGuid ());
    Assert.IsTrue (_endPoint.HasChanged);

    _endPoint.OppositeObjectID = _oppositeObjectID;
    Assert.IsFalse (_endPoint.HasChanged);
  }

  [Test]
  public void HasChangedWithInitializedWithNull ()
  {
    ObjectEndPoint endPoint = CreateObjectEndPoint (_endPointID, null);

    Assert.IsFalse (endPoint.HasChanged);
  }

  [Test]
  public void HasChangedWithOldNullValue ()
  {
    ObjectEndPoint endPoint = CreateObjectEndPoint (_endPointID, null);
    endPoint.OppositeObjectID = new ObjectID ("StorageProverID", "ClassID", Guid.NewGuid ());

    Assert.IsTrue (endPoint.HasChanged);
  }

  [Test]
  public void HasChangedWithNewNullValue ()
  {
    _endPoint.OppositeObjectID = null;

    Assert.IsTrue (_endPoint.HasChanged);
  }

  [Test]
  public void GetEndPointDefinition ()
  {
    IRelationEndPointDefinition endPointDefinition = _endPoint.Definition;
    Assert.IsNotNull (endPointDefinition);

    Assert.AreSame (
      MappingConfiguration.Current.ClassDefinitions.GetByClassID ("OrderItem"), 
      endPointDefinition.ClassDefinition);

    Assert.AreEqual ("Order", endPointDefinition.PropertyName);
  }

  [Test]
  public void GetOppositeEndPointDefinition ()
  {
    IRelationEndPointDefinition oppositeEndPointDefinition = _endPoint.OppositeEndPointDefinition;
    Assert.IsNotNull (oppositeEndPointDefinition);

    Assert.AreSame (
      MappingConfiguration.Current.ClassDefinitions.GetByClassID ("Order"),
      oppositeEndPointDefinition.ClassDefinition);

    Assert.AreEqual ("OrderItems", oppositeEndPointDefinition.PropertyName);
  }

  [Test]
  public void GetRelationDefinition ()
  {
    RelationDefinition relationDefinition = _endPoint.RelationDefinition;
    Assert.IsNotNull (relationDefinition);
    Assert.AreEqual ("OrderToOrderItem", relationDefinition.ID);
  }

  [Test]
  public void IsVirtual ()
  {
    DataContainer orderContainer = TestDataContainerFactory.CreateOrder1DataContainer ();
    RelationEndPoint orderEndPoint = CreateObjectEndPoint (orderContainer, "OrderTicket", DomainObjectIDs.OrderTicket1);

    Assert.AreEqual (true, orderEndPoint.IsVirtual);
  }

  [Test]
  public void IsNotVirtual ()
  {
    Assert.AreEqual (false, _endPoint.IsVirtual);
  }

  [Test]
  public void ID ()
  {
    Assert.IsNotNull (_endPoint.ID);
    Assert.AreEqual ("Order", _endPoint.ID.PropertyName);
    Assert.AreEqual (DomainObjectIDs.OrderItem1, _endPoint.ID.ObjectID);
  }
}
}
