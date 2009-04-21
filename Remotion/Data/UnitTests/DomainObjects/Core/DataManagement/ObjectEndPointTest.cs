// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
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

      _endPointID = new RelationEndPointID (DomainObjectIDs.OrderItem1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order");
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
      var newObjectID = new ObjectID ("Order", Guid.NewGuid ());
      _endPoint.OppositeObjectID = newObjectID;

      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (newObjectID));
      Assert.That (_endPoint.OriginalOppositeObjectID, Is.EqualTo (_oppositeObjectID));
      Assert.That (((OrderItem)_endPoint.GetDomainObject()).InternalDataContainer.PropertyValues[_endPoint.PropertyName].Value,
          Is.EqualTo (newObjectID));
    }

    [Test]
    public void GetOppositeObject ()
    {
      var oppositeObject = _endPoint.GetOppositeObject (true);
      Assert.That (oppositeObject, Is.SameAs (Order.GetObject (_endPoint.OppositeObjectID)));
    }

    [Test]
    public void GetOppositeObject_Deleted ()
    {
      var oppositeObject = (Order) _endPoint.GetOppositeObject (true);
      oppositeObject.Delete ();
      Assert.That (oppositeObject.State, Is.EqualTo (StateType.Deleted));

      Assert.That (_endPoint.GetOppositeObject (true), Is.SameAs (oppositeObject));
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void GetOppositeObject_Deleted_NoDeleted ()
    {
      var oppositeObject = (Order) _endPoint.GetOppositeObject (true);
      oppositeObject.Delete ();
      Assert.That (oppositeObject.State, Is.EqualTo (StateType.Deleted));

      _endPoint.GetOppositeObject (false);
    }

    [Test]
    public void GetOppositeObject_Discarded ()
    {
      var oppositeObject = Order.NewObject ();
      _endPoint.OppositeObjectID = oppositeObject.ID;

      oppositeObject.Delete ();
      Assert.That (oppositeObject.State, Is.EqualTo (StateType.Discarded));

      Assert.That (_endPoint.GetOppositeObject (true), Is.SameAs (oppositeObject));
    }

    [Test]
    [ExpectedException (typeof (ObjectNotFoundException))]
    public void GetOppositeObject_Discarded_NoDeleted ()
    {
      var oppositeObject = Order.NewObject ();
      _endPoint.OppositeObjectID = oppositeObject.ID;

      oppositeObject.Delete ();
      Assert.That (oppositeObject.State, Is.EqualTo (StateType.Discarded));

      _endPoint.GetOppositeObject (false);
    }

    [Test]
    public void GetOppositeObject_Null ()
    {
      _endPoint.OppositeObjectID = null;
      var oppositeObject = _endPoint.GetOppositeObject (false);
      Assert.That (oppositeObject, Is.Null);
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
      var removedRelatedObject = _endPoint.GetOppositeObject (false);
      Assert.IsFalse (_endPoint.HasBeenTouched);
      _endPoint.CreateRemoveModification (removedRelatedObject).Perform ();
      Assert.IsTrue (_endPoint.HasBeenTouched);
    }

    [Test]
    public void Touch_AlsoTouchesForeignKey ()
    {
      Assert.That (_endPoint.IsVirtual, Is.False);
      Assert.That (_endPoint.HasBeenTouched, Is.False);
      Assert.That (((OrderItem)_endPoint.GetDomainObject()).InternalDataContainer.PropertyValues[_endPoint.PropertyName].HasBeenTouched, Is.False);

      _endPoint.Touch();
      Assert.That (_endPoint.HasBeenTouched, Is.True);
      Assert.That (((OrderItem) _endPoint.GetDomainObject ()).InternalDataContainer.PropertyValues[_endPoint.PropertyName].HasBeenTouched, Is.True);
    }

    [Test]
    public void Touch_WorksIfNoForeignKey ()
    {
      var virtualEndPointID = new RelationEndPointID (DomainObjectIDs.Order1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket");
      var oppositeID = DomainObjectIDs.OrderTicket1;
      var virtualEndPoint = CreateObjectEndPoint (virtualEndPointID, oppositeID);

      Assert.That (virtualEndPoint.IsVirtual, Is.True);
      Assert.That (virtualEndPoint.HasBeenTouched, Is.False);
      Assert.That (((Order) virtualEndPoint.GetDomainObject ()).InternalDataContainer.PropertyValues.Contains (virtualEndPoint.PropertyName), Is.False);

      virtualEndPoint.Touch();
      Assert.That (virtualEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void PerformWithoutBegin ()
    {
      _endPoint.OppositeObjectID = DomainObjectIDs.Order1;
      Assert.IsNotNull (_endPoint.OppositeObjectID);
      _endPoint.CreateRemoveModification (Order.GetObject (DomainObjectIDs.Order1)).Perform();
      Assert.IsNull (_endPoint.OppositeObjectID);
    }

    [Test]
    public void GetEndPointDefinition ()
    {
      IRelationEndPointDefinition endPointDefinition = _endPoint.Definition;
      Assert.IsNotNull (endPointDefinition);

      Assert.AreSame (
        MappingConfiguration.Current.ClassDefinitions["OrderItem"],
        endPointDefinition.ClassDefinition);

      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", endPointDefinition.PropertyName);
    }

    [Test]
    public void GetOppositeEndPointDefinition ()
    {
      IRelationEndPointDefinition oppositeEndPointDefinition = _endPoint.OppositeEndPointDefinition;
      Assert.IsNotNull (oppositeEndPointDefinition);

      Assert.AreSame (
        MappingConfiguration.Current.ClassDefinitions["Order"],
        oppositeEndPointDefinition.ClassDefinition);

      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems", oppositeEndPointDefinition.PropertyName);
    }

    [Test]
    public void GetRelationDefinition ()
    {
      RelationDefinition relationDefinition = _endPoint.RelationDefinition;
      Assert.IsNotNull (relationDefinition);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", relationDefinition.ID);
    }

    [Test]
    public void IsVirtual ()
    {
      DataContainer orderContainer = TestDataContainerFactory.CreateOrder1DataContainer ();
      RelationEndPoint orderEndPoint = CreateObjectEndPoint (orderContainer, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket", DomainObjectIDs.OrderTicket1);

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
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order", _endPoint.ID.PropertyName);
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

      ObjectEndPoint clone = (ObjectEndPoint) endPoint.Clone (ClientTransactionMock);

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

      ObjectEndPoint clone = (ObjectEndPoint) endPoint.Clone (ClientTransactionMock);

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

    [Test]
    public void CreateSetModification_Same ()
    {
      var modification = _endPoint.CreateSetModification (_endPoint.GetOppositeObject (true));
      Assert.That (modification.GetType(), Is.EqualTo (typeof (ObjectEndPointSetSameModification)));
      Assert.That (modification.ModifiedEndPoint, Is.SameAs (_endPoint));
    }

    [Test]
    public void CreateSetModification_Unidirectional ()
    {
      var client = Client.GetObject (DomainObjectIDs.Client2);
      var parentClientEndPointDefinition = client.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (Client).FullName + ".ParentClient");
      var unidirectionalEndPoint = (ObjectEndPoint)
                                   ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (
                                       client, parentClientEndPointDefinition);
      Assert.That (unidirectionalEndPoint.OppositeEndPointDefinition.IsAnonymous, Is.True);
      var newClient = Client.NewObject ();

      var modification = unidirectionalEndPoint.CreateSetModification (newClient);
      Assert.That (modification.GetType (), Is.EqualTo (typeof (ObjectEndPointSetUnidirectionalModification)));
      Assert.That (modification.ModifiedEndPoint, Is.SameAs (unidirectionalEndPoint));
      Assert.That (modification.NewRelatedObject, Is.SameAs (newClient));
    }

    [Test]
    public void CreateSetModification_OneOne ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var orderTicketEndPointDefinition = order.ID.ClassDefinition.GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderTicket");
      var bidirectionalEndPoint = (ObjectEndPoint)
                                  ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (
                                      order, orderTicketEndPointDefinition);

      var newOrderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);

      var modification = bidirectionalEndPoint.CreateSetModification (newOrderTicket);
      Assert.That (modification.GetType (), Is.EqualTo (typeof (ObjectEndPointSetOneOneModification)));
      Assert.That (modification.ModifiedEndPoint, Is.SameAs (bidirectionalEndPoint));
      Assert.That (modification.NewRelatedObject, Is.SameAs (newOrderTicket));
    }

    [Test]
    public void CreateSetModification_OneMany ()
    {
      var orderItem = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      var orderEndPointDefinition = orderItem.ID.ClassDefinition.GetMandatoryRelationEndPointDefinition (typeof (OrderItem).FullName + ".Order");
      var bidirectionalEndPoint = (ObjectEndPoint)
                                  ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (
                                      orderItem, orderEndPointDefinition);

      // orderItem.Order = newOrder;
      var newOrder = Order.GetObject (DomainObjectIDs.Order2);

      var modification = bidirectionalEndPoint.CreateSetModification (newOrder);
      Assert.That (modification.GetType (), Is.EqualTo (typeof (ObjectEndPointSetOneManyModification)));
      Assert.That (modification.ModifiedEndPoint, Is.SameAs (bidirectionalEndPoint));
      Assert.That (modification.NewRelatedObject, Is.SameAs (newOrder));
    }

    [Test]
    public void CreateRemoveModification ()
    {
      var order = Order.GetObject (_endPoint.OppositeObjectID);
      var modification = _endPoint.CreateRemoveModification (order);
      Assert.That (modification, Is.InstanceOfType (typeof (ObjectEndPointSetOneManyModification)));
      Assert.That (modification.ModifiedEndPoint, Is.SameAs (_endPoint));
      Assert.That (modification.OldRelatedObject, Is.SameAs (order));
      Assert.That (modification.NewRelatedObject, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot remove object "
        + "'Order|90e26c86-611f-4735-8d1b-e1d0918515c2|System.Guid' from object end point "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order' - it currently holds object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'.")]
    public void CreateRemoveModification_InvalidID ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order4);
      _endPoint.CreateRemoveModification (order);
    }

    [Test]
    public void CreateSelfReplaceModification ()
    {
      var order = Order.GetObject (_endPoint.OppositeObjectID);
      var modification = _endPoint.CreateSelfReplaceModification (order);
      Assert.That (modification, Is.InstanceOfType (typeof (ObjectEndPointSetSameModification)));
      Assert.That (modification.ModifiedEndPoint, Is.SameAs (_endPoint));
      Assert.That (modification.OldRelatedObject, Is.SameAs (order));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot replace "
        + "'Order|90e26c86-611f-4735-8d1b-e1d0918515c2|System.Guid' from object end point "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order' - it currently holds object "
        + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'.")]
    public void CreateSelfReplaceModification_InvalidID ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order4);
      _endPoint.CreateSelfReplaceModification (order);
    }

  }
}
