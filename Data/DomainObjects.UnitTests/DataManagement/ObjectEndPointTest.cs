using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
  [TestFixture]
  public class ObjectEndPointTest : RelationEndPointBaseTest
  {
    private RelationEndPointID _endPointID;
    private ObjectEndPoint _endPoint;
    private ObjectID _oppositeObjectID;

    public override void SetUp ()
    {
      base.SetUp ();

      _endPointID = new RelationEndPointID (DomainObjectIDs.OrderItem1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order");
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
      ObjectID id = new ObjectID ("Order", Guid.NewGuid ());
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
      ObjectID newObjectID = new ObjectID ("Order", Guid.NewGuid ());
      _endPoint.OppositeObjectID = newObjectID;

      Assert.AreSame (newObjectID, _endPoint.OppositeObjectID);
      Assert.AreEqual (_oppositeObjectID, _endPoint.OriginalOppositeObjectID);
    }

    [Test]
    public void HasChanged ()
    {
      Assert.IsFalse (_endPoint.HasChanged);

      _endPoint.OppositeObjectID = new ObjectID ("Order", Guid.NewGuid ());
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
      endPoint.OppositeObjectID = new ObjectID ("Order", Guid.NewGuid ());

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
        MappingConfiguration.Current.ClassDefinitions["OrderItem"],
        endPointDefinition.ClassDefinition);

      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", endPointDefinition.PropertyName);
    }

    [Test]
    public void GetOppositeEndPointDefinition ()
    {
      IRelationEndPointDefinition oppositeEndPointDefinition = _endPoint.OppositeEndPointDefinition;
      Assert.IsNotNull (oppositeEndPointDefinition);

      Assert.AreSame (
        MappingConfiguration.Current.ClassDefinitions["Order"],
        oppositeEndPointDefinition.ClassDefinition);

      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems", oppositeEndPointDefinition.PropertyName);
    }

    [Test]
    public void GetRelationDefinition ()
    {
      RelationDefinition relationDefinition = _endPoint.RelationDefinition;
      Assert.IsNotNull (relationDefinition);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", relationDefinition.ID);
    }

    [Test]
    public void IsVirtual ()
    {
      DataContainer orderContainer = TestDataContainerFactory.CreateOrder1DataContainer ();
      RelationEndPoint orderEndPoint = CreateObjectEndPoint (orderContainer, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", DomainObjectIDs.OrderTicket1);

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
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", _endPoint.ID.PropertyName);
      Assert.AreEqual (DomainObjectIDs.OrderItem1, _endPoint.ID.ObjectID);
    }

    private void CheckIfRelationEndPointsAreEqual (ObjectEndPoint expected, ObjectEndPoint actual)
    {
      ArgumentUtility.CheckNotNull ("expected", expected);
      ArgumentUtility.CheckNotNull ("actual", actual);

      Assert.AreNotSame (expected, actual);

      Assert.AreSame (expected.ClientTransaction, actual.ClientTransaction);
      Assert.AreSame (expected.Definition, actual.Definition);
      Assert.AreEqual (expected.HasChanged, actual.HasChanged);
      Assert.AreEqual (expected.ID, actual.ID);
      Assert.AreEqual (expected.ObjectID, actual.ObjectID);
      Assert.AreEqual (expected.OppositeObjectID, actual.OppositeObjectID);
      Assert.AreEqual (expected.OriginalOppositeObjectID, actual.OriginalOppositeObjectID);
    }

    [Test]
    public void CloneUnchanged ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      RelationEndPointID id = new RelationEndPointID (order.ID, typeof (Order) + ".Official");

      ObjectEndPoint endPoint = (ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[id];
      Assert.IsNotNull (endPoint);

      Assert.AreSame (ClientTransactionMock, endPoint.ClientTransaction);
      Assert.IsNotNull (endPoint.Definition);
      Assert.IsFalse (endPoint.HasChanged);
      Assert.AreEqual (id, endPoint.ID);
      Assert.AreEqual (order.ID, endPoint.ObjectID);
      Assert.AreEqual (order.Official.ID, endPoint.OppositeObjectID);
      Assert.AreEqual (endPoint.OppositeObjectID, endPoint.OriginalOppositeObjectID);
      Assert.AreEqual (order.Official.ID, endPoint.OriginalOppositeObjectID);

      ObjectEndPoint clone = (ObjectEndPoint) endPoint.Clone ();

      Assert.IsNotNull (endPoint);

      CheckIfRelationEndPointsAreEqual (endPoint, clone);
    }

    [Test]
    public void CloneChanged ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
      Employee originalEmployee = computer.Employee;
      computer.Employee = Employee.NewObject ();

      RelationEndPointID id = new RelationEndPointID (computer.ID, typeof (Computer) + ".Employee");

      ObjectEndPoint endPoint = (ObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[id];
      Assert.IsNotNull (endPoint);

      Assert.AreSame (ClientTransactionMock, endPoint.ClientTransaction);
      Assert.IsNotNull (endPoint.Definition);
      Assert.IsTrue (endPoint.HasChanged);
      Assert.AreEqual (id, endPoint.ID);
      Assert.AreEqual (computer.ID, endPoint.ObjectID);
      Assert.AreEqual (computer.Employee.ID, endPoint.OppositeObjectID);
      Assert.AreNotEqual (endPoint.OppositeObjectID, endPoint.OriginalOppositeObjectID);
      Assert.AreEqual (originalEmployee.ID, endPoint.OriginalOppositeObjectID);

      ObjectEndPoint clone = (ObjectEndPoint) endPoint.Clone ();

      Assert.IsNotNull (endPoint);

      CheckIfRelationEndPointsAreEqual (endPoint, clone);
    }
  }
}
