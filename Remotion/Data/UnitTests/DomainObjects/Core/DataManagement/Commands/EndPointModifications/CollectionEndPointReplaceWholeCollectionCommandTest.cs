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
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using System.Collections.Generic;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class CollectionEndPointReplaceWholeCollectionCommandTest : CollectionEndPointModificationCommandTestBase
  {
    private DomainObjectCollection _newCollection;

    private MockRepository _mockRepository;
    private IAssociatableDomainObjectCollection _oldTransformerMock;
    private IAssociatableDomainObjectCollection _newTransformerMock;

    private CollectionEndPointReplaceWholeCollectionCommand _command;

    private Order _order1;
    private Order _orderWithoutOrderItem;
    private Order _order2;

    public override void SetUp ()
    {
      base.SetUp();

      _newCollection = new OrderCollection();

      _mockRepository = new MockRepository ();
      _oldTransformerMock = _mockRepository.StrictMock<IAssociatableDomainObjectCollection> ();
      _newTransformerMock = _mockRepository.StrictMock<IAssociatableDomainObjectCollection> ();

      _command = new CollectionEndPointReplaceWholeCollectionCommand (
          CollectionEndPoint, 
          _newCollection,
          _oldTransformerMock,
          _newTransformerMock);

      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _orderWithoutOrderItem = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);
      _order2 = Order.GetObject (DomainObjectIDs.Order2);
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
      Assert.That (_command.NewOppositeCollection, Is.SameAs (_newCollection));
      Assert.That (_command.OldOppositeCollectionTransformer, Is.SameAs (_oldTransformerMock));
      Assert.That (_command.NewOppositeCollectionTransformer, Is.SameAs (_newTransformerMock));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Modified end point is null, a NullEndPointModificationCommand is needed.\r\n"
                                                                      + "Parameter name: modifiedEndPoint")]
    public void Initialization_FromNullEndPoint ()
    {
      var endPoint = new NullCollectionEndPoint (ClientTransactionMock, RelationEndPointID.Definition);
      new CollectionEndPointReplaceWholeCollectionCommand (endPoint, _newCollection, _oldTransformerMock, _newTransformerMock);
    }

    [Test]
    public void Begin ()
    {
      CollectionEndPoint.OppositeDomainObjects.Add (_order1); // will stay
      CollectionEndPoint.OppositeDomainObjects.Add (_orderWithoutOrderItem); // will be removed

      _newCollection.Add (_order1); // same as existing
      _newCollection.Add (_order2); // new

      var relationChangingEventArgs = new List<RelationChangingEventArgs> ();
      bool relationChangedCalled = false;

      CollectionEventReceiver.Reset();

      DomainObject.RelationChanging += (sender, args) => relationChangingEventArgs.Add (args);
      DomainObject.RelationChanged += (sender, args) => relationChangedCalled = true;

      _command.Begin ();

      Assert.That (CollectionEventReceiver.RemovingDomainObjects, Is.Empty);
      Assert.That (CollectionEventReceiver.AddingDomainObject, Is.Null);

      Assert.That (relationChangingEventArgs.Count, Is.EqualTo (2)); // operation was started
      Assert.That (relationChangedCalled, Is.False); // operation was not finished

      Assert.That (relationChangingEventArgs[0].RelationEndPointDefinition.PropertyName, Is.EqualTo (CollectionEndPoint.PropertyName));
      Assert.That (relationChangingEventArgs[0].OldRelatedObject, Is.SameAs (_orderWithoutOrderItem));
      Assert.That (relationChangingEventArgs[0].NewRelatedObject, Is.Null);

      Assert.That (relationChangingEventArgs[1].RelationEndPointDefinition.PropertyName, Is.EqualTo (CollectionEndPoint.PropertyName));
      Assert.That (relationChangingEventArgs[1].OldRelatedObject, Is.Null);
      Assert.That (relationChangingEventArgs[1].NewRelatedObject, Is.SameAs (_order2));
    }

    [Test]
    public void End ()
    {
      CollectionEndPoint.OppositeDomainObjects.Add (_order1); // will stay
      CollectionEndPoint.OppositeDomainObjects.Add (_orderWithoutOrderItem); // will be removed

      _newCollection.Add (_order1); // same as existing
      _newCollection.Add (_order2); // new

      var relationChangedEventArgs = new List<RelationChangedEventArgs> ();
      bool relationChangingCalled = false;

      CollectionEventReceiver.Reset ();

      DomainObject.RelationChanging += (sender, args) => relationChangingCalled = true;
      DomainObject.RelationChanged += (sender, args) => relationChangedEventArgs.Add (args);

      _command.End ();

      Assert.That (CollectionEventReceiver.RemovedDomainObjects, Is.Empty);
      Assert.That (CollectionEventReceiver.AddedDomainObject, Is.Null);

      Assert.That (relationChangedEventArgs.Count, Is.EqualTo (2)); // operation was started
      Assert.That (relationChangingCalled, Is.False); // operation was not finished

      Assert.That (relationChangedEventArgs[0].RelationEndPointDefinition.PropertyName, Is.EqualTo (CollectionEndPoint.PropertyName));

      Assert.That (relationChangedEventArgs[1].RelationEndPointDefinition.PropertyName, Is.EqualTo (CollectionEndPoint.PropertyName));
    }

    [Test]
    public void NotifyClientTransactionOfBegin ()
    {
      CollectionEndPoint.OppositeDomainObjects.Add (_order1); // will stay
      CollectionEndPoint.OppositeDomainObjects.Add (_orderWithoutOrderItem); // will be removed
      
      _newCollection.Add (_order1); // same as existing
      _newCollection.Add (_order2); // new
      
      var listenerMock = _mockRepository.StrictMock<IClientTransactionListener> ();
      listenerMock.Expect (mock => mock.RelationChanging (
          ClientTransactionMock, 
          DomainObject, 
          CollectionEndPoint.Definition, 
          _orderWithoutOrderItem, 
          null));
      listenerMock.Expect (mock => mock.RelationChanging (
          ClientTransactionMock, 
          DomainObject, 
          CollectionEndPoint.Definition, 
          null, 
          _order2));
      listenerMock.Replay ();

      ClientTransactionMock.AddListener (listenerMock);

      _command.NotifyClientTransactionOfBegin ();

      listenerMock.VerifyAllExpectations ();
    }

    [Test]
    public void NotifyClientTransactionOfEnd ()
    {
      CollectionEndPoint.OppositeDomainObjects.Add (_order1); // will stay
      CollectionEndPoint.OppositeDomainObjects.Add (_orderWithoutOrderItem); // will be removed

      _newCollection.Add (_order1); // same as existing
      _newCollection.Add (_order2); // new

      var listenerMock = _mockRepository.StrictMock<IClientTransactionListener> ();
      listenerMock.Expect (mock => mock.RelationChanged (
          ClientTransactionMock, 
          DomainObject, 
          CollectionEndPoint.Definition));
      listenerMock.Expect (mock => mock.RelationChanged (
          ClientTransactionMock, 
          DomainObject, 
          CollectionEndPoint.Definition));

      listenerMock.Replay ();

      ClientTransactionMock.AddListener (listenerMock);

      _command.NotifyClientTransactionOfEnd();

      listenerMock.VerifyAllExpectations ();
    }

    [Test]
    public void Perform ()
    {
      _newCollection.Add (_order1);

      bool relationChangingCalled = false;
      bool relationChangedCalled = false;

      DomainObject.RelationChanging += (sender, args) => relationChangingCalled = true;
      DomainObject.RelationChanged += (sender, args) => relationChangedCalled = true;

      using (_mockRepository.Ordered ())
      {
        // Transform old collection to stand-alone
        _oldTransformerMock.Stub (mock => mock.IsAssociatedWith (CollectionEndPoint)).Return (true);
        _oldTransformerMock.Expect (mock => mock.TransformToStandAlone ());

        // Transform new collection to associated
        _newTransformerMock
            .Expect (mock => mock.TransformToAssociated (CollectionEndPoint))
            .WhenCalled (mi =>
            {
              Assert.That (CollectionEndPoint.OppositeDomainObjects != _newCollection); // transformations occur before SetOppositeCollection
              TransformToAssociated (_newCollection);
            });
      }
      _mockRepository.ReplayAll ();
      
      _command.Perform ();

      _mockRepository.VerifyAll ();

      Assert.That (relationChangingCalled, Is.False); // operation was not started
      Assert.That (relationChangedCalled, Is.False); // operation was not finished

      Assert.That (CollectionEndPoint.OppositeDomainObjects, Is.SameAs (_newCollection));
      Assert.That (CollectionEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void Perform_DoesNotTransformOldCollectionToStandAlone_WhenOldCollectionAssociatedWithOtherEndPoint ()
    {
      _newCollection.Add (_order1);

      var delegatingCollectionData = new EndPointDelegatingCollectionData (
          MockRepository.GenerateStub<ICollectionEndPoint> (), MockRepository.GenerateStub<IDomainObjectCollectionData>());
      var collectionOfDifferentEndPoint = new DomainObjectCollection (delegatingCollectionData);

      Assert.That (collectionOfDifferentEndPoint.IsAssociatedWith (null), Is.False);
      Assert.That (collectionOfDifferentEndPoint.IsAssociatedWith (CollectionEndPoint), Is.False);

      _oldTransformerMock.Stub (mock => mock.IsAssociatedWith (CollectionEndPoint)).Return (false);
      // _oldTransformerMock.TransformToStandAlone is not called because collectionOfDifferentEndPoint belongs to a different end point

      _newTransformerMock
          .Expect (mock => mock.TransformToAssociated (CollectionEndPoint))
          .WhenCalled (mi => TransformToAssociated (_newCollection));

      _mockRepository.ReplayAll ();

      _command.Perform ();

      _oldTransformerMock.AssertWasNotCalled (mock => mock.TransformToStandAlone ());
      _newTransformerMock.VerifyAllExpectations ();
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      CollectionEndPoint.OppositeDomainObjects.Add (_order1);
      CollectionEndPoint.OppositeDomainObjects.Add (_orderWithoutOrderItem);

      Assert.That (_order1.Customer, Is.SameAs (CollectionEndPoint.GetDomainObject ()));
      Assert.That (_orderWithoutOrderItem.Customer, Is.SameAs (CollectionEndPoint.GetDomainObject ()));

      var customer3 = Customer.GetObject (DomainObjectIDs.Customer3);
      Assert.That (_order2.Customer, Is.SameAs (customer3));

      _newCollection.Add (_order1);
      _newCollection.Add (_order2);
      
      var bidirectionalModification = _command.ExpandToAllRelatedObjects ();

      // DomainObject.Orders = _newCollection

      var steps = GetAllCommands (bidirectionalModification);
      Assert.That (steps.Count, Is.EqualTo (4));

      // orderWithoutOrderItem.Customer = null;
      // order2.Customer.Orders.Remove (order2);
      // order2.Customer = DomainObject;
      // DomainObject.Orders = _newCollection

      // orderWithoutOrderItem.Customer = null;
      Assert.That (steps[0], Is.InstanceOfType (typeof (ObjectEndPointSetCommand)));
      Assert.That (steps[0].ModifiedEndPoint.ID.Definition.PropertyName, Is.EqualTo (typeof (Order).FullName + ".Customer"));
      Assert.That (steps[0].ModifiedEndPoint.ID.ObjectID, Is.EqualTo (_orderWithoutOrderItem.ID));
      Assert.That (steps[0].OldRelatedObject, Is.SameAs (DomainObject));
      Assert.That (steps[0].NewRelatedObject, Is.Null);

      // order2.Customer.Orders.Remove (order2);
      Assert.That (steps[1], Is.InstanceOfType (typeof (CollectionEndPointRemoveCommand)));
      Assert.That (steps[1].ModifiedEndPoint.ID.Definition.PropertyName, Is.EqualTo (typeof (Customer).FullName + ".Orders"));
      Assert.That (steps[1].ModifiedEndPoint.ID.ObjectID, Is.EqualTo (customer3.ID));
      Assert.That (steps[1].OldRelatedObject, Is.SameAs (_order2));
      Assert.That (steps[1].NewRelatedObject, Is.Null);

      // order2.Customer = DomainObject
      Assert.That (steps[2], Is.InstanceOfType (typeof (ObjectEndPointSetCommand)));
      Assert.That (steps[2].ModifiedEndPoint.ID.Definition.PropertyName, Is.EqualTo (typeof (Order).FullName + ".Customer"));
      Assert.That (steps[2].ModifiedEndPoint.ID.ObjectID, Is.EqualTo (_order2.ID));
      Assert.That (steps[2].OldRelatedObject, Is.SameAs (customer3));
      Assert.That (steps[2].NewRelatedObject, Is.SameAs (DomainObject));

      // DomainObject.Orders = _newCollection
      Assert.That (steps[3], Is.SameAs (_command));
    }

    private void TransformToAssociated (DomainObjectCollection collection)
    {
      var transformer = (IAssociatableDomainObjectCollection) collection;
      transformer.TransformToAssociated (CollectionEndPoint);
    }
  }
}
