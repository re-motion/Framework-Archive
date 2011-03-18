// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class ObjectEndPointSetOneOneCommandTest : ObjectEndPointSetCommandTestBase
  {
    private Order _domainObject;
    private OrderTicket _oldRelatedObject;
    private OrderTicket _newRelatedObject;

    private RelationEndPointID _endPointID;
    private ObjectEndPoint _endPoint;
    
    private Action<ObjectID> _oppositeObjectIDSetter;
    
    private ObjectEndPointSetCommand _command;

    public override void SetUp ()
    {
      base.SetUp ();

      _domainObject = Order.GetObject (DomainObjectIDs.Order1);
      _oldRelatedObject = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
      _newRelatedObject = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);

      _endPointID = RelationEndPointID.Create (_domainObject, o => o.OrderTicket);
      _endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (_endPointID, _oldRelatedObject.ID);

      _oppositeObjectIDSetter = id => ObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, id);

      _command = new ObjectEndPointSetOneOneCommand (_endPoint, _newRelatedObject, _oppositeObjectIDSetter);
    }

    [Test]
    public void Initialization ()
    {
      Assert.AreSame (_endPoint, _command.ModifiedEndPoint);
      Assert.AreSame (_oldRelatedObject, _command.OldRelatedObject);
      Assert.AreSame (_newRelatedObject, _command.NewRelatedObject);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Modified end point is null, a NullEndPointModificationCommand is needed.\r\n"
                                                                      + "Parameter name: modifiedEndPoint")]
    public void Initialization_FromNullEndPoint ()
    {
      var endPoint = new NullObjectEndPoint (ClientTransactionMock, _endPointID.Definition);
      new ObjectEndPointSetOneOneCommand (endPoint, _newRelatedObject, _oppositeObjectIDSetter);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "EndPoint 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Client.ParentClient' "
        + "is from a unidirectional relation - use a ObjectEndPointSetUnidirectionalCommand instead.\r\nParameter name: modifiedEndPoint")]
    public void Initialization_Unidirectional ()
    {
      var definition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Client))
          .GetMandatoryRelationEndPointDefinition (typeof (Client).FullName + ".ParentClient");
      var client = Client.GetObject (DomainObjectIDs.Client1);
      var id = RelationEndPointID.Create (client.ID, definition);
      var endPoint = (IObjectEndPoint)
          ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (id);
      new ObjectEndPointSetOneOneCommand (endPoint, Client.NewObject (), mi => { });
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "EndPoint 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order' "
        + "is from a 1:n relation - use a ObjectEndPointSetOneManyCommand instead.\r\nParameter name: modifiedEndPoint")]
    public void Initialization_Bidirectional_OneMany ()
    {
      var definition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem))
          .GetMandatoryRelationEndPointDefinition (typeof (OrderItem).FullName + ".Order");
      var relationEndPointID = RelationEndPointID.Create (OrderItem.GetObject (DomainObjectIDs.OrderItem1).ID, definition);
      var endPoint =
          (IObjectEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (relationEndPointID);
      new ObjectEndPointSetOneOneCommand (endPoint, Order.NewObject (), mi => { });
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "New related object for EndPoint "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket' is the same as its old value - use a ObjectEndPointSetSameCommand "
        + "instead.\r\nParameter name: newRelatedObject")]
    public void Initialization_Same ()
    {
      var endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (_endPointID, _oldRelatedObject.ID);
      new ObjectEndPointSetOneOneCommand (endPoint, _oldRelatedObject, mi => { });
    }

    [Test]
    public virtual void Begin ()
    {
      DomainObject domainObject = ((IObjectEndPoint) _endPoint).GetDomainObject ();
      var eventReceiver = new DomainObjectEventReceiver (domainObject);

      _command.Begin();

      Assert.IsTrue (eventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsFalse (eventReceiver.HasRelationChangedEventBeenCalled);
    }

    [Test]
    public void Perform_InvokesPerformRelationChange ()
    {
      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (_oldRelatedObject.ID));
      _command.Perform();
      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (_newRelatedObject.ID));
    }

    [Test]
    public void Perform_TouchesEndPoint ()
    {
      Assert.That (_endPoint.HasBeenTouched, Is.False);
      _command.Perform();
      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public virtual void End ()
    {
      DomainObject domainObject = ((IObjectEndPoint) _endPoint).GetDomainObject ();
      var eventReceiver = new DomainObjectEventReceiver (domainObject);

      _command.End();

      Assert.IsFalse (eventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsTrue (eventReceiver.HasRelationChangedEventBeenCalled);
    }

    [Test]
    public virtual void NotifyClientTransactionOfBegin ()
    {
      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      _command.NotifyClientTransactionOfBegin();

      listenerMock.AssertWasCalled(mock => mock.RelationChanging (
          ClientTransactionMock, 
          _endPoint.GetDomainObject (), 
          _endPoint.Definition, 
          _oldRelatedObject, 
          _newRelatedObject));
    }

    [Test]
    public virtual void NotifyClientTransactionOfEnd ()
    {
      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      _command.NotifyClientTransactionOfEnd ();

      listenerMock.AssertWasCalled (mock => mock.RelationChanged (
          ClientTransactionMock, 
          _endPoint.GetDomainObject (), 
          _endPoint.Definition));
    }

    [Test]
    public void ExpandToAllRelatedObjects_SetDifferent_BidirectionalOneOne ()
    {
      // order.OrderTicket = newOrderTicket;

      var bidirectionalModification = _command.ExpandToAllRelatedObjects ();

      var steps = GetAllCommands (bidirectionalModification);
      Assert.That (steps.Count, Is.EqualTo (4));

      // order.OrderTicket = newOrderTicket;
      Assert.That (steps[0], Is.SameAs (_command));

      // oldOrderTicket.Order = null;

      var orderOfOldOrderTicketEndPointID = RelationEndPointID.Create (_oldRelatedObject, ot => ot.Order);
      var orderOfOldOrderTicketEndPoint =
          ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (orderOfOldOrderTicketEndPointID);

      Assert.That (steps[1], Is.InstanceOfType (typeof (ObjectEndPointSetCommand)));
      Assert.That (steps[1].ModifiedEndPoint, Is.SameAs (orderOfOldOrderTicketEndPoint));
      Assert.That (steps[1].OldRelatedObject, Is.SameAs (_domainObject));
      Assert.That (steps[1].NewRelatedObject, Is.Null);

      // newOrderTicket.Order = order;

      var orderOfNewOrderTicketEndPointID = RelationEndPointID.Create (_newRelatedObject, ot => ot.Order);
      var orderOfNewOrderTicketEndPoint =
          ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (orderOfNewOrderTicketEndPointID);

      Assert.That (steps[2], Is.InstanceOfType (typeof (ObjectEndPointSetCommand)));
      Assert.That (steps[2].ModifiedEndPoint, Is.SameAs (orderOfNewOrderTicketEndPoint));
      Assert.That (steps[2].OldRelatedObject, Is.SameAs (_newRelatedObject.Order));
      Assert.That (steps[2].NewRelatedObject, Is.SameAs (_domainObject));

      // oldOrderOfNewOrderTicket.OrderTicket = null

      var orderTicketOfOldOrderOfNewOrderTicketEndPointID = RelationEndPointID.Create (_newRelatedObject.Order.ID, _endPoint.Definition);
      var orderTicketOfOldOrderOfNewOrderTicketEndPoint =
          ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (orderTicketOfOldOrderOfNewOrderTicketEndPointID);

      Assert.That (steps[3], Is.InstanceOfType (typeof (ObjectEndPointSetCommand)));
      Assert.That (steps[3].ModifiedEndPoint, Is.SameAs (orderTicketOfOldOrderOfNewOrderTicketEndPoint));
      Assert.That (steps[3].OldRelatedObject, Is.SameAs (_newRelatedObject));
      Assert.That (steps[3].NewRelatedObject, Is.SameAs (null));
    }
  }
}
