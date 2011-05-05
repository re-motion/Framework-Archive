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
// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.

using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class ObjectEndPointSetSameCommandTest : ObjectEndPointSetCommandTestBase
  {
    private Computer _domainObject;
    private Employee _relatedObject;

    private RelationEndPointID _endPointID;
    private ObjectEndPoint _endPoint;

    private ObjectEndPointSetCommand _command;

    public override void SetUp ()
    {
      base.SetUp ();

      _domainObject = Computer.GetObject (DomainObjectIDs.Computer1);
      _relatedObject = Employee.GetObject (DomainObjectIDs.Employee3);

      _endPointID = RelationEndPointID.Create (_domainObject, c => c.Employee);
      _endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (_endPointID, _relatedObject.ID);
      
      _command = new ObjectEndPointSetSameCommand (_endPoint, OppositeObjectSetter);
    }

    [Test]
    public void Initialization ()
    {
      Assert.AreSame (_endPoint, _command.ModifiedEndPoint);
      Assert.AreSame (_relatedObject, _command.OldRelatedObject);
      Assert.AreSame (_relatedObject, _command.NewRelatedObject);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Modified end point is null, a NullEndPointModificationCommand is needed.\r\n"
                                                                      + "Parameter name: modifiedEndPoint")]
    public void Initialization_FromNullEndPoint ()
    {
      var endPoint = new NullObjectEndPoint (ClientTransactionMock, _endPointID.Definition);
      new ObjectEndPointSetSameCommand (endPoint, OppositeObjectSetter);
    }

    [Test]
    public void Begin ()
    {
      var eventReceiver = new DomainObjectEventReceiver (_domainObject);

      _command.Begin ();

      Assert.IsFalse (eventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsFalse (eventReceiver.HasRelationChangedEventBeenCalled);
    }

    [Test]
    public void End ()
    {
      var eventReceiver = new DomainObjectEventReceiver (_domainObject);

      _command.End ();

      Assert.IsFalse (eventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsFalse (eventReceiver.HasRelationChangedEventBeenCalled);
    }

    [Test]
    public void Perform_InvokesPerformRelationChange ()
    {
      Assert.That (OppositeObjectSetterCalled, Is.False);

      _command.Perform ();
      
      Assert.That (OppositeObjectSetterCalled, Is.True);
      Assert.That (OppositeObjectSetterObject, Is.EqualTo (_relatedObject));
    }

    [Test]
    public void Perform_TouchesEndPoint ()
    {
      Assert.That (_endPoint.HasBeenTouched, Is.False);
      _command.Perform ();
      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void NotifyClientTransactionOfBegin ()
    {
      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      _command.NotifyClientTransactionOfBegin ();

      listenerMock.AssertWasNotCalled (mock => mock.RelationChanging (
          Arg<ClientTransaction>.Is.Anything, 
          Arg<DomainObject>.Is.Anything,
          Arg<IRelationEndPointDefinition>.Is.Anything,
          Arg<DomainObject>.Is.Anything,
          Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void NotifyClientTransactionOfEnd ()
    {
      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      _command.NotifyClientTransactionOfBegin ();

      listenerMock.AssertWasNotCalled (mock => mock.RelationChanged (
          Arg<ClientTransaction>.Is.Anything,
          Arg<DomainObject>.Is.Anything,
          Arg<IRelationEndPointDefinition>.Is.Anything));
    }

    [Test]
    public void ExpandToAllRelatedObjects_SetSame_Unidirectional ()
    {
      var client = Client.GetObject (DomainObjectIDs.Client2);
      var unidirectionalEndPointID = RelationEndPointID.Create (client, c => c.ParentClient);
      var unidirectionalEndPoint =
          (IObjectEndPoint) ClientTransactionMock.DataManager.GetRelationEndPointWithLazyLoad (unidirectionalEndPointID);
      Assert.That (unidirectionalEndPoint.Definition.GetOppositeEndPointDefinition().IsAnonymous, Is.True);

      var setSameModification = new ObjectEndPointSetSameCommand (unidirectionalEndPoint, mi => { });

      var bidirectionalModification = setSameModification.ExpandToAllRelatedObjects ();
      Assert.That (bidirectionalModification.GetNestedCommands(), Is.EqualTo (new[] { setSameModification }));
    }

    [Test]
    public void ExpandToAllRelatedObjects_SetSame_Bidirectional ()
    {
      var oppositeEndPointID = RelationEndPointID.Create (_relatedObject, e => e.Computer);
      var oppositeEndPoint = ClientTransactionMock.DataManager.GetRelationEndPointWithLazyLoad (oppositeEndPointID);

      var bidirectionalModification = _command.ExpandToAllRelatedObjects ();

      var steps = bidirectionalModification.GetNestedCommands ();
      Assert.That (steps.Count, Is.EqualTo (2));

      Assert.That (steps[0], Is.SameAs (_command));

      Assert.That (steps[1], Is.InstanceOf (typeof (RelationEndPointTouchCommand)));
      Assert.That (((RelationEndPointTouchCommand) steps[1]).EndPoint, Is.SameAs (oppositeEndPoint));
    }
  }
}
