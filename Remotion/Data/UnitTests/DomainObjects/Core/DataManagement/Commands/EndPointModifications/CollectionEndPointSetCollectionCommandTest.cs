// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class CollectionEndPointSetCollectionCommandTest : CollectionEndPointModificationCommandTestBase
  {
    private Order _order1;
    private Order _order5;
    private Order _order2;
    private IDomainObjectCollectionData _modifiedCollectionData;
    private DomainObjectCollection _newCollection;

    private MockRepository _mockRepository;
    private ICollectionEndPointCollectionManager _collectionManagerMock;

    private CollectionEndPointSetCollectionCommand _command;

    public override void SetUp ()
    {
      base.SetUp();

      _order1 = DomainObjectIDs.Order1.GetObject<Order> (Transaction);
      _order5 = DomainObjectIDs.Order5.GetObject<Order> (Transaction);
      _order2 = DomainObjectIDs.Order2.GetObject<Order> (Transaction);

      // Collection currently contains _order1, _order5
      _modifiedCollectionData = new DomainObjectCollectionData (new[] { _order1, _order5 });

      // _order1 will stay, _order5 will be removed, _order2 will be added
      _newCollection = new OrderCollection { _order1, _order2 };

      _mockRepository = new MockRepository ();
      _collectionManagerMock = _mockRepository.StrictMock<ICollectionEndPointCollectionManager> ();

      _command = new CollectionEndPointSetCollectionCommand (
          CollectionEndPoint, 
          _newCollection,
          _modifiedCollectionData,
          _collectionManagerMock,
          TransactionEventSinkMock);
    }

    public override void TearDown ()
    {
      _mockRepository.BackToRecordAll (); // For Discard
      base.TearDown ();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_command.ModifiedEndPoint, Is.SameAs (CollectionEndPoint));
      Assert.That (_command.OldRelatedObject, Is.Null);
      Assert.That (_command.NewRelatedObject, Is.Null);
      Assert.That (_command.NewCollection, Is.SameAs (_newCollection));
      Assert.That (_command.CollectionEndPointCollectionManager, Is.SameAs (_collectionManagerMock));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Modified end point is null, a NullEndPointModificationCommand is needed.\r\n"
                                                                      + "Parameter name: modifiedEndPoint")]
    public void Initialization_FromNullEndPoint ()
    {
      var endPoint = new NullCollectionEndPoint (Transaction, RelationEndPointID.Definition);
      Dev.Null = new CollectionEndPointSetCollectionCommand (
          endPoint, _newCollection, CollectionDataMock, _collectionManagerMock, TransactionEventSinkMock);
    }

    [Test]
    public void Begin ()
    {
      using (TransactionEventSinkMock.GetMockRepository ().Ordered ())
      {
        TransactionEventSinkMock.Expect (mock => mock.RaiseRelationChangingEvent (
            DomainObject,
            CollectionEndPoint.Definition,
            _order5,
            null));
        TransactionEventSinkMock.Expect (mock => mock.RaiseRelationChangingEvent (
            DomainObject,
            CollectionEndPoint.Definition,
            null,
            _order2));
      }
      TransactionEventSinkMock.Replay();

      _command.Begin ();

      TransactionEventSinkMock.VerifyAllExpectations();
    }

    [Test]
    public void End ()
    {
      using (TransactionEventSinkMock.GetMockRepository ().Ordered ())
      {
        TransactionEventSinkMock.Expect (mock => mock.RaiseRelationChangedEvent (
            DomainObject,
            CollectionEndPoint.Definition,
            null,
            _order2));
        TransactionEventSinkMock.Expect (mock => mock.RaiseRelationChangedEvent (
            DomainObject,
            CollectionEndPoint.Definition,
            _order5,
            null));
      }

      TransactionEventSinkMock.Replay();

      _command.End();

      TransactionEventSinkMock.VerifyAllExpectations();
    }

    [Test]
    public void Perform ()
    {
      bool relationChangingCalled = false;
      bool relationChangedCalled = false;

      DomainObject.RelationChanging += (sender, args) => relationChangingCalled = true;
      DomainObject.RelationChanged += (sender, args) => relationChangedCalled = true;

      _collectionManagerMock
          .Expect (mock => mock.AssociateCollectionWithEndPoint (_newCollection))
          .Return (new DomainObjectCollectionData (new[] { _order1, _order2 }))
          .WhenCalled (mi => Assert.That (_modifiedCollectionData, Is.EqualTo (new[] { _order1, _order5 })));
      _mockRepository.ReplayAll ();
      
      _command.Perform ();

      _mockRepository.VerifyAll ();

      Assert.That (_modifiedCollectionData, Is.EqualTo (new[] { _order1, _order2 }));
      Assert.That (relationChangingCalled, Is.False); // operation was not started
      Assert.That (relationChangedCalled, Is.False); // operation was not finished

      Assert.That (CollectionEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      Assert.That (_order1.Customer, Is.SameAs (CollectionEndPoint.GetDomainObject ()));
      Assert.That (_order5.Customer, Is.SameAs (CollectionEndPoint.GetDomainObject ()));

      var customer3 = DomainObjectIDs.Customer3.GetObject<Customer> (Transaction);
      Assert.That (_order2.Customer, Is.SameAs (customer3));

      var bidirectionalModification = _command.ExpandToAllRelatedObjects ();

      // DomainObject.Orders = _newCollection

      var steps = bidirectionalModification.GetNestedCommands();
      Assert.That (steps.Count, Is.EqualTo (4));

      // order5.Customer = null;
      // order2.Customer.Orders.Remove (order2);
      // order2.Customer = DomainObject;
      // DomainObject.Orders = _newCollection

      // order5.Customer = null;
      Assert.That (steps[0], Is.InstanceOf (typeof (RealObjectEndPointRegistrationCommandDecorator)));
      var setOrder5CustomerCommand = ((ObjectEndPointSetCommand) ((RealObjectEndPointRegistrationCommandDecorator) steps[0]).DecoratedCommand);
      Assert.That (setOrder5CustomerCommand.ModifiedEndPoint.ID.Definition.PropertyName, Is.EqualTo (typeof (Order).FullName + ".Customer"));
      Assert.That (setOrder5CustomerCommand.ModifiedEndPoint.ID.ObjectID, Is.EqualTo (_order5.ID));
      Assert.That (setOrder5CustomerCommand.OldRelatedObject, Is.SameAs (DomainObject));
      Assert.That (setOrder5CustomerCommand.NewRelatedObject, Is.Null);

      // order2.Customer.Orders.Remove (order2);
      Assert.That (steps[1], Is.TypeOf<VirtualEndPointStateUpdatedRaisingCommandDecorator> ());
      var order2CustomerOrdersRemoveCommand = ((CollectionEndPointRemoveCommand) ((VirtualEndPointStateUpdatedRaisingCommandDecorator) steps[1]).DecoratedCommand);
      Assert.That (order2CustomerOrdersRemoveCommand.ModifiedEndPoint.ID.Definition.PropertyName, Is.EqualTo (typeof (Customer).FullName + ".Orders"));
      Assert.That (order2CustomerOrdersRemoveCommand.ModifiedEndPoint.ID.ObjectID, Is.EqualTo (customer3.ID));
      Assert.That (order2CustomerOrdersRemoveCommand.OldRelatedObject, Is.SameAs (_order2));
      Assert.That (order2CustomerOrdersRemoveCommand.NewRelatedObject, Is.Null);

      // order2.Customer = DomainObject
      Assert.That (steps[2], Is.InstanceOf (typeof (RealObjectEndPointRegistrationCommandDecorator)));
      var setOrder2CustomerCommand = ((ObjectEndPointSetCommand) ((RealObjectEndPointRegistrationCommandDecorator) steps[2]).DecoratedCommand);
      Assert.That (setOrder2CustomerCommand.ModifiedEndPoint.ID.Definition.PropertyName, Is.EqualTo (typeof (Order).FullName + ".Customer"));
      Assert.That (setOrder2CustomerCommand.ModifiedEndPoint.ID.ObjectID, Is.EqualTo (_order2.ID));
      Assert.That (setOrder2CustomerCommand.OldRelatedObject, Is.SameAs (customer3));
      Assert.That (setOrder2CustomerCommand.NewRelatedObject, Is.SameAs (DomainObject));

      // DomainObject.Orders = _newCollection
      Assert.That (steps[3], Is.SameAs (_command));
    }
  }
}
