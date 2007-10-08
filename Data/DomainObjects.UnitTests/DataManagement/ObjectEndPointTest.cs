using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Utilities;
using Rubicon.Development.UnitTesting;

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
    public void HasChangedWithSameValueSet ()
    {
      Assert.IsFalse (_endPoint.HasChanged);
      _endPoint.OppositeObjectID = _oppositeObjectID;
      Assert.IsFalse (_endPoint.HasChanged);
    }

    [Test]
    public void HasBeenTouched ()
    {
      Assert.IsFalse (_endPoint.HasBeenTouched);

      _endPoint.OppositeObjectID = new ObjectID ("Order", Guid.NewGuid ());
      Assert.IsTrue (_endPoint.HasBeenTouched);

      _endPoint.OppositeObjectID = _oppositeObjectID;
      Assert.IsTrue (_endPoint.HasBeenTouched);
    }

    [Test]
    public void HasBeenTouchedWithInitializedWithNull ()
    {
      ObjectEndPoint endPoint = CreateObjectEndPoint (_endPointID, null);
      Assert.IsFalse (endPoint.HasBeenTouched);
    }

    [Test]
    public void HasBeenTouchedWithOldNullValue ()
    {
      ObjectEndPoint endPoint = CreateObjectEndPoint (_endPointID, null);
      endPoint.OppositeObjectID = new ObjectID ("Order", Guid.NewGuid ());

      Assert.IsTrue (endPoint.HasBeenTouched);
    }

    [Test]
    public void HasBeenTouchedWithNewNullValue ()
    {
      _endPoint.OppositeObjectID = null;

      Assert.IsTrue (_endPoint.HasBeenTouched);
    }

    [Test]
    public void HasBeenTouchedWithSameValueSet ()
    {
      Assert.IsFalse (_endPoint.HasBeenTouched);
      _endPoint.OppositeObjectID = _oppositeObjectID;
      Assert.IsTrue (_endPoint.HasBeenTouched);
    }

    [Test]
    public void HasBeenTouchedWithPerformRelationChange ()
    {
      Assert.IsFalse (_endPoint.HasBeenTouched);
      _endPoint.PerformRelationChange (new NullObjectEndPoint(_endPoint.OppositeEndPointDefinition));
      Assert.IsTrue (_endPoint.HasBeenTouched);
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
      Assert.AreEqual (expected.HasBeenTouched, actual.HasBeenTouched);
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
      Assert.IsFalse (endPoint.HasBeenTouched);
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
      Assert.IsTrue (endPoint.HasBeenTouched);
      Assert.AreEqual (id, endPoint.ID);
      Assert.AreEqual (computer.ID, endPoint.ObjectID);
      Assert.AreEqual (computer.Employee.ID, endPoint.OppositeObjectID);
      Assert.AreNotEqual (endPoint.OppositeObjectID, endPoint.OriginalOppositeObjectID);
      Assert.AreEqual (originalEmployee.ID, endPoint.OriginalOppositeObjectID);

      ObjectEndPoint clone = (ObjectEndPoint) endPoint.Clone ();

      Assert.IsNotNull (endPoint);

      CheckIfRelationEndPointsAreEqual (endPoint, clone);
    }

    [Test]
    public void Commit ()
    {
      ObjectID newOppositeID = new ObjectID ("Order", Guid.NewGuid ());
      _endPoint.OppositeObjectID = newOppositeID;

      Assert.IsTrue (_endPoint.HasBeenTouched);
      Assert.IsTrue (_endPoint.HasChanged);
      Assert.AreEqual (newOppositeID, _endPoint.OppositeObjectID);
      Assert.AreEqual (_oppositeObjectID, _endPoint.OriginalOppositeObjectID);

      _endPoint.Commit ();

      Assert.IsFalse (_endPoint.HasBeenTouched);
      Assert.IsFalse (_endPoint.HasChanged);
      Assert.AreEqual (newOppositeID, _endPoint.OppositeObjectID);
      Assert.AreEqual (newOppositeID, _endPoint.OriginalOppositeObjectID);
    }

    [Test]
    public void Rollback ()
    {
      ObjectID newOppositeID = new ObjectID ("Order", Guid.NewGuid ());
      _endPoint.OppositeObjectID = newOppositeID;

      Assert.IsTrue (_endPoint.HasBeenTouched);
      Assert.IsTrue (_endPoint.HasChanged);
      Assert.AreEqual (newOppositeID, _endPoint.OppositeObjectID);
      Assert.AreEqual (_oppositeObjectID, _endPoint.OriginalOppositeObjectID);

      _endPoint.Rollback ();

      Assert.IsFalse (_endPoint.HasBeenTouched);
      Assert.IsFalse (_endPoint.HasChanged);
      Assert.AreEqual (_oppositeObjectID, _endPoint.OppositeObjectID);
      Assert.AreEqual (_oppositeObjectID, _endPoint.OriginalOppositeObjectID);
    }

    [Test]
    public void TakeOverCommittedData_ChangedIntoUnchanged ()
    {
      ObjectEndPoint endPoint2 = CreateObjectEndPoint (_endPointID, DomainObjectIDs.Order2);

      _endPoint.OppositeObjectID = DomainObjectIDs.Order4;

      Assert.IsFalse (endPoint2.HasChanged);
      Assert.IsFalse (endPoint2.HasBeenTouched);
      Assert.AreEqual (DomainObjectIDs.Order2, endPoint2.OppositeObjectID);
      Assert.AreEqual (DomainObjectIDs.Order2, endPoint2.OriginalOppositeObjectID);

      PrivateInvoke.InvokeNonPublicMethod (endPoint2, "TakeOverCommittedData", _endPoint);

      Assert.IsTrue (endPoint2.HasChanged);
      Assert.IsTrue (endPoint2.HasBeenTouched);
      Assert.AreEqual (DomainObjectIDs.Order4, endPoint2.OppositeObjectID);
      Assert.AreEqual (DomainObjectIDs.Order2, endPoint2.OriginalOppositeObjectID);
    }

    [Test]
    public void TakeOverCommittedData_UnchangedIntoUnchanged ()
    {
      ObjectEndPoint endPoint2 = CreateObjectEndPoint (_endPointID, DomainObjectIDs.Order2);

      Assert.IsFalse (endPoint2.HasChanged);
      Assert.IsFalse (endPoint2.HasBeenTouched);
      Assert.AreEqual (DomainObjectIDs.Order2, endPoint2.OppositeObjectID);
      Assert.AreEqual (DomainObjectIDs.Order2, endPoint2.OriginalOppositeObjectID);

      PrivateInvoke.InvokeNonPublicMethod (endPoint2, "TakeOverCommittedData", _endPoint);

      Assert.IsTrue (endPoint2.HasChanged);
      Assert.IsTrue (endPoint2.HasBeenTouched);
      Assert.AreEqual (_oppositeObjectID, endPoint2.OppositeObjectID);
      Assert.AreEqual (DomainObjectIDs.Order2, endPoint2.OriginalOppositeObjectID);
    }

    [Test]
    public void TakeOverCommittedData_UnchangedIntoChanged ()
    {
      ObjectEndPoint endPoint2 = CreateObjectEndPoint (_endPointID, DomainObjectIDs.Order2);

      endPoint2.OppositeObjectID = DomainObjectIDs.Order3;

      Assert.IsTrue (endPoint2.HasChanged);
      Assert.IsTrue (endPoint2.HasBeenTouched);
      Assert.AreEqual (DomainObjectIDs.Order3, endPoint2.OppositeObjectID);
      Assert.AreEqual (DomainObjectIDs.Order2, endPoint2.OriginalOppositeObjectID);

      PrivateInvoke.InvokeNonPublicMethod (endPoint2, "TakeOverCommittedData", _endPoint);

      Assert.IsTrue (endPoint2.HasChanged);
      Assert.IsTrue (endPoint2.HasBeenTouched);
      Assert.AreEqual (_oppositeObjectID, endPoint2.OppositeObjectID);
      Assert.AreEqual (DomainObjectIDs.Order2, endPoint2.OriginalOppositeObjectID);
    }

    [Test]
    public void TakeOverCommittedData_ChangedIntoChanged ()
    {
      ObjectEndPoint endPoint2 = CreateObjectEndPoint (_endPointID, DomainObjectIDs.Order2);

      _endPoint.OppositeObjectID = DomainObjectIDs.Order3;
      endPoint2.OppositeObjectID = DomainObjectIDs.Order4;

      Assert.IsTrue (endPoint2.HasChanged);
      Assert.IsTrue (endPoint2.HasBeenTouched);
      Assert.AreEqual (DomainObjectIDs.Order4, endPoint2.OppositeObjectID);
      Assert.AreEqual (DomainObjectIDs.Order2, endPoint2.OriginalOppositeObjectID);

      PrivateInvoke.InvokeNonPublicMethod (endPoint2, "TakeOverCommittedData", _endPoint);

      Assert.IsTrue (endPoint2.HasChanged);
      Assert.IsTrue (endPoint2.HasBeenTouched);
      Assert.AreEqual (DomainObjectIDs.Order3, endPoint2.OppositeObjectID);
      Assert.AreEqual (DomainObjectIDs.Order2, endPoint2.OriginalOppositeObjectID);
    }

    [Test]
    public void TakeOverCommittedData_UnchangedIntoEqual ()
    {
      ObjectEndPoint endPoint2 = CreateObjectEndPoint (_endPointID, _endPoint.OppositeObjectID);

      Assert.IsFalse (endPoint2.HasChanged);
      Assert.IsFalse (endPoint2.HasBeenTouched);
      Assert.AreEqual (_endPoint.OppositeObjectID, endPoint2.OppositeObjectID);
      Assert.AreEqual (_endPoint.OppositeObjectID, endPoint2.OriginalOppositeObjectID);

      PrivateInvoke.InvokeNonPublicMethod (endPoint2, "TakeOverCommittedData", _endPoint);

      Assert.IsFalse (endPoint2.HasChanged);
      Assert.IsFalse (endPoint2.HasBeenTouched);
      Assert.AreEqual (_endPoint.OppositeObjectID, endPoint2.OppositeObjectID);
      Assert.AreEqual (_endPoint.OppositeObjectID, endPoint2.OriginalOppositeObjectID);
    }
  }
}
