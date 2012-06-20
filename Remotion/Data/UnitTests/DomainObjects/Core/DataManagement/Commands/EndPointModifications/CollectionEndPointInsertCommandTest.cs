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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class CollectionEndPointInsertCommandTest : CollectionEndPointModificationCommandTestBase
  {
    private Order _insertedRelatedObject;
    private CollectionEndPointInsertCommand _command;

    public override void SetUp ()
    {
      base.SetUp();

      _insertedRelatedObject = Order.GetObject (DomainObjectIDs.Order2);

      _command = new CollectionEndPointInsertCommand (
          CollectionEndPoint, 12, _insertedRelatedObject, CollectionDataMock, EndPointProviderStub, TransactionEventSinkWithMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_command.ModifiedEndPoint, Is.SameAs (CollectionEndPoint));
      Assert.That (_command.OldRelatedObject, Is.Null);
      Assert.That (_command.NewRelatedObject, Is.SameAs (_insertedRelatedObject));
      Assert.That (_command.Index, Is.EqualTo (12));
      Assert.That (_command.ModifiedCollection, Is.SameAs (CollectionEndPoint.Collection));
      Assert.That (_command.ModifiedCollectionData, Is.SameAs (CollectionDataMock));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Modified end point is null, a NullEndPointModificationCommand is needed.\r\n"
                                                                      + "Parameter name: modifiedEndPoint")]
    public void Initialization_FromNullEndPoint ()
    {
      var endPoint = new NullCollectionEndPoint (TestableClientTransaction, RelationEndPointID.Definition);
      new CollectionEndPointInsertCommand (endPoint, 0, _insertedRelatedObject, CollectionDataMock, EndPointProviderStub, TransactionEventSinkWithMock);
    }

    [Test]
    public void NotifyClientTransactionOfBegin ()
    {
      TransactionEventSinkWithMock
          .ExpectMock (
              mock => mock.RelationChanging (TestableClientTransaction, DomainObject, CollectionEndPoint.Definition, null, _insertedRelatedObject))
          .WhenCalled (
              mock => Assert.That (CollectionEventReceiver.AddingDomainObject, Is.SameAs (_insertedRelatedObject))); // collection got event first

      _command.NotifyClientTransactionOfBegin ();

      TransactionEventSinkWithMock.VerifyMock ();
      Assert.That (CollectionEventReceiver.AddedDomainObject, Is.Null); // operation was not finished
    }

    [Test]
    public void NotifyClientTransactionOfEnd ()
    {
      TransactionEventSinkWithMock
          .ExpectMock (
              mock => mock.RelationChanged (TestableClientTransaction, DomainObject, CollectionEndPoint.Definition, null, _insertedRelatedObject))
          .WhenCalled (
              mock => Assert.That (CollectionEventReceiver.AddedDomainObject, Is.Null)); // collection gets event later

      _command.NotifyClientTransactionOfEnd ();

      TransactionEventSinkWithMock.VerifyMock ();
      Assert.That (CollectionEventReceiver.AddedDomainObject, Is.SameAs (_insertedRelatedObject)); // collection got event later
      Assert.That (CollectionEventReceiver.AddingDomainObject, Is.Null); // operation was not started
    }

    [Test]
    public void Perform ()
    {
      CollectionDataMock.BackToRecord ();
      CollectionDataMock.Expect (mock => mock.Insert (12, _insertedRelatedObject));
      CollectionDataMock.Replay ();

      _command.Perform ();

      CollectionDataMock.VerifyAllExpectations ();

      Assert.That (CollectionEventReceiver.AddingDomainObject, Is.Null); // operation was not started
      Assert.That (CollectionEventReceiver.AddedDomainObject, Is.Null); // operation was not finished
      Assert.That (CollectionEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      var insertedEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID (_insertedRelatedObject.ID, "Customer");
      var insertedEndPoint = (IObjectEndPoint) TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (insertedEndPointID);
      Assert.That (insertedEndPoint, Is.Not.Null);
      
      EndPointProviderStub.Stub (stub => stub.GetRelationEndPointWithLazyLoad (insertedEndPoint.ID)).Return (insertedEndPoint);
      
      var oldCustomer = _insertedRelatedObject.Customer;
      var oldRelatedEndPointOfInsertedObject =
          TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading (RelationEndPointID.Resolve (oldCustomer, c => c.Orders));
      EndPointProviderStub
          .Stub (stub => stub.GetRelationEndPointWithLazyLoad (oldRelatedEndPointOfInsertedObject.ID))
          .Return (oldRelatedEndPointOfInsertedObject);

      var bidirectionalModification = _command.ExpandToAllRelatedObjects ();

      // DomainObject.Orders.Insert (_insertedRelatedObject, 12)
      var steps = bidirectionalModification.GetNestedCommands();
      Assert.That (steps.Count, Is.EqualTo (3));

      // _insertedRelatedObject.Customer = DomainObject (previously oldCustomer)
      Assert.That (steps[0], Is.InstanceOf (typeof (RealObjectEndPointRegistrationCommandDecorator)));
      var setCustomerCommand = ((ObjectEndPointSetCommand) ((RealObjectEndPointRegistrationCommandDecorator) steps[0]).DecoratedCommand);
      Assert.That (setCustomerCommand.ModifiedEndPoint, Is.SameAs (insertedEndPoint));
      Assert.That (setCustomerCommand.OldRelatedObject, Is.SameAs (oldCustomer));
      Assert.That (setCustomerCommand.NewRelatedObject, Is.SameAs (DomainObject));

      // DomainObject.Orders.Insert (_insertedRelatedObject, 12)
      Assert.That (steps[1], Is.SameAs (_command));

      // oldCustomer.Orders.Remove (_insertedRelatedObject)
      Assert.That (steps[2], Is.TypeOf<VirtualEndPointStateUpdatedRaisingCommandDecorator> ());
      var oldCustomerOrdersRemoveCommand = ((CollectionEndPointRemoveCommand) ((VirtualEndPointStateUpdatedRaisingCommandDecorator) steps[2]).DecoratedCommand);
      Assert.That (oldCustomerOrdersRemoveCommand.ModifiedEndPoint, Is.SameAs (((StateUpdateRaisingCollectionEndPointDecorator) oldRelatedEndPointOfInsertedObject).InnerEndPoint));
      Assert.That (oldCustomerOrdersRemoveCommand.ModifiedEndPoint.ID.ObjectID, Is.EqualTo (oldCustomer.ID));
      Assert.That (oldCustomerOrdersRemoveCommand.OldRelatedObject, Is.SameAs (_insertedRelatedObject));
    }
  }
}
