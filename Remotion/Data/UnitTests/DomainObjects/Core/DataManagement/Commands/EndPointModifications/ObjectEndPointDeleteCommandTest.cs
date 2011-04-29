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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class ObjectEndPointDeleteCommandTest : ClientTransactionBaseTest
  {
    private ObjectEndPoint _endPoint;
    private RelationEndPointID _endPointID;
    private DomainObject _domainObject;

    private bool _oppositeObjectSetterCalled;
    private DomainObject _oppositeObjectSetterObject;
    private Action<DomainObject> _oppositeObjectSetter;

    private ObjectEndPointDeleteCommand _command;

    public override void SetUp ()
    {
      base.SetUp();

      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      _endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (_endPointID, DomainObjectIDs.OrderTicket1);
      _domainObject = _endPoint.GetDomainObject();
      
      _oppositeObjectSetterCalled = false;
      _oppositeObjectSetter = domainObject =>
      {
        _oppositeObjectSetterCalled = true;
        _oppositeObjectSetterObject = domainObject;
      };
      
      _command = new ObjectEndPointDeleteCommand (_endPoint, _oppositeObjectSetter);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_command.ModifiedEndPoint, Is.SameAs (_endPoint));
      Assert.That (_command.OldRelatedObject, Is.Null);
      Assert.That (_command.NewRelatedObject, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Modified end point is null, a NullEndPointModificationCommand is needed.\r\n"
                                                                      + "Parameter name: modifiedEndPoint")]
    public void Initialization_FromNullEndPoint ()
    {
      var endPoint = new NullObjectEndPoint (ClientTransactionMock, _endPointID.Definition);
      new ObjectEndPointDeleteCommand (endPoint, id => { });
    }

    [Test]
    public void NotifyClientTransactionOfBegin()
    {
      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);
      
      _command.NotifyClientTransactionOfBegin();
    }

    [Test]
    public void NotifyClientTransactionOfEnd ()
    {
      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);

      _command.NotifyClientTransactionOfEnd ();
    }

    [Test]
    public void Begin ()
    {
      bool relationChangingCalled = false;
      bool relationChangedCalled = false;

      _domainObject.RelationChanging += (sender, args) => relationChangingCalled = true;
      _domainObject.RelationChanged += (sender, args) => relationChangedCalled = true;

      _command.Begin ();

      Assert.That (relationChangingCalled, Is.False); // object does not get a notification
      Assert.That (relationChangedCalled, Is.False); // operation was not finished
    }

    [Test]
    public void End ()
    {
      bool relationChangingCalled = false;
      bool relationChangedCalled = false;

      _domainObject.RelationChanging += (sender, args) => relationChangingCalled = true;
      _domainObject.RelationChanged += (sender, args) => relationChangedCalled = true;

      _command.End ();

      Assert.That (relationChangingCalled, Is.False); // object does not get a notification
      Assert.That (relationChangedCalled, Is.False); // operation was not finished
    }

    [Test]
    public void Perform ()
    {
      bool relationChangingCalled = false;
      bool relationChangedCalled = false;

      _domainObject.RelationChanging += (sender, args) => relationChangingCalled = true;
      _domainObject.RelationChanged += (sender, args) => relationChangedCalled = true;

      Assert.That (_oppositeObjectSetterCalled, Is.False);
      Assert.That (_endPoint.HasBeenTouched, Is.False);

      _command.Perform ();

      Assert.That (relationChangingCalled, Is.False); // operation was not started
      Assert.That (relationChangedCalled, Is.False); // operation was not finished

      Assert.That (_oppositeObjectSetterCalled, Is.True);
      Assert.That (_oppositeObjectSetterObject, Is.Null);
      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      var bidirectionalModification = _command.ExpandToAllRelatedObjects ();

      var steps = bidirectionalModification.GetNestedCommands();
      Assert.That (steps.Count, Is.EqualTo (1));
      Assert.That (steps[0], Is.SameAs (_command));

    }
  }
}
