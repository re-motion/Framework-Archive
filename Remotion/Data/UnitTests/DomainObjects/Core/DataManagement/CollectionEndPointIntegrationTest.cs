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
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class CollectionEndPointIntegrationTest : RelationEndPointBaseTest
  {
    private RelationEndPointID _customerEndPointID;

    private Order _order1; // belongs to customer1
    private Order _orderWithoutOrderItem; // belongs to customer1
    private Order _order2; // belongs to customer3

    private CollectionEndPoint _customerEndPoint;
    
    public override void SetUp ()
    {
      base.SetUp ();

      _customerEndPointID = new RelationEndPointID (DomainObjectIDs.Customer1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders");
      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _orderWithoutOrderItem = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);
      _order2 = Order.GetObject (DomainObjectIDs.Order2);

      _customerEndPoint = CreateCollectionEndPoint (_customerEndPointID, new[] { _order1, _orderWithoutOrderItem });
    }

    [Test]
    public void AddToOppositeDomainObjects ()
    {
      var newOrder = Order.NewObject ();

      _customerEndPoint.OppositeDomainObjects.Add (newOrder);

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.EqualTo (new[] { _order1, _orderWithoutOrderItem, newOrder }), "changes go down to actual data store");
      Assert.That (newOrder.Customer, Is.SameAs (_customerEndPoint.GetDomainObject()), "bidirectional modification");
    }

    [Test]
    public void RemoveFromOppositeDomainObjects ()
    {
      _customerEndPoint.OppositeDomainObjects.Remove (_order1.ID);

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.EqualTo (new[] { _orderWithoutOrderItem }), "changes go down to actual data store");
      Assert.That (_order1.Customer, Is.Null, "bidirectional modification");
    }

    [Test]
    public void ReplaceOppositeCollection()
    {
      var oldOpposites = _customerEndPoint.OppositeDomainObjects;
      var newOpposites = new OrderCollection (new DomainObjectCollectionData (new[] { _orderWithoutOrderItem }));

      _customerEndPoint.ReplaceOppositeCollection (newOpposites);

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.SameAs (newOpposites));
      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.EqualTo (new[] { _orderWithoutOrderItem }));
      Assert.That (oldOpposites, Is.EqualTo (new[] { _order1, _orderWithoutOrderItem }));

      Assert.That (newOpposites.AssociatedEndPoint, Is.SameAs (_customerEndPoint));
      Assert.That (oldOpposites.AssociatedEndPoint, Is.Null);
    }

    [Test]
    public void ReplaceOppositeCollection_DataStrategy_OfOldEndPoint ()
    {
      var oldOpposites = _customerEndPoint.OppositeDomainObjects;
      var originalDataOfOldOpposites = GetDomainObjectCollectionData (oldOpposites);
      var originalDataStoreOfOldOpposites = originalDataOfOldOpposites.GetUndecoratedDataStore ();

      var newOpposites = new OrderCollection (new DomainObjectCollectionData (new[] { _orderWithoutOrderItem }));
      _customerEndPoint.ReplaceOppositeCollection (newOpposites);

      // oldCollection => argument decorator => event decorator => actual data store

      var argChecker = DomainObjectCollectionDataTestHelper.GetCollectionDataAndCheckType<ArgumentCheckingCollectionDataDecorator> (oldOpposites);
      Assert.That (argChecker.RequiredItemType, Is.SameAs (typeof (Order)));

      var eventRaiser = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<EventRaisingCollectionDataDecorator> (argChecker);
      Assert.That (eventRaiser.EventRaiser, Is.SameAs (oldOpposites));

      var dataStore = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<DomainObjectCollectionData> (eventRaiser);
      Assert.That (dataStore, Is.SameAs (originalDataStoreOfOldOpposites));
    }

    [Test]
    public void ReplaceOppositeCollection_DataStrategy_OfNewEndPoint ()
    {
      var newOpposites = new OrderCollection (new DomainObjectCollectionData (new[] { _orderWithoutOrderItem }));
      var originalDataOfNewOpposites = GetDomainObjectCollectionData (newOpposites);
      var originalDataStoreOfNewOpposites = originalDataOfNewOpposites.GetUndecoratedDataStore ();

      _customerEndPoint.ReplaceOppositeCollection (newOpposites);

      // newCollection => argument checking decorator => end point data => actual data store

      var argChecker = DomainObjectCollectionDataTestHelper.GetCollectionDataAndCheckType<ArgumentCheckingCollectionDataDecorator> (newOpposites);
      Assert.That (argChecker.RequiredItemType, Is.SameAs (typeof (Order)));

      var delegator = DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<EndPointDelegatingCollectionData> (argChecker);
      var dataStore = DomainObjectCollectionDataTestHelper.GetActualDataAndCheckType<DomainObjectCollectionData> (delegator);
      Assert.That (dataStore, Is.SameAs (originalDataStoreOfNewOpposites), "new collection still uses its original data store");
    }

    [Test]
    public void ReplaceOppositeCollection_PerformsAllBidirectionalChanges ()
    {
      var newOpposites = new OrderCollection (new DomainObjectCollectionData (new[] { _orderWithoutOrderItem, _order2}));

      var customer3 = Customer.GetObject (DomainObjectIDs.Customer3);
      
      Assert.That (_order1.Customer, Is.SameAs (_customerEndPoint.GetDomainObject ()));
      Assert.That (_orderWithoutOrderItem.Customer, Is.SameAs (_customerEndPoint.GetDomainObject ()));
      Assert.That (_order2.Customer, Is.SameAs (customer3));
      Assert.That (customer3.Orders, List.Contains (_order2));

      _customerEndPoint.ReplaceOppositeCollection (newOpposites);

      Assert.That (_order1.Customer, Is.Null);
      Assert.That (_orderWithoutOrderItem.Customer, Is.SameAs (_customerEndPoint.GetDomainObject ()));
      Assert.That (_order2.Customer, Is.SameAs (_customerEndPoint.GetDomainObject()));
      Assert.That (customer3.Orders, List.Not.Contains (_order2));
    }

    [Test]
    public void ReplaceOppositeCollection_RaisesNoEventsOnCollections ()
    {
      var oldOpposites = _customerEndPoint.OppositeDomainObjects;
      var oldEventListener = new DomainObjectCollectionEventReceiver (oldOpposites);

      var newOpposites = new OrderCollection (new DomainObjectCollectionData (new[] { _orderWithoutOrderItem }));
      var newEventListener = new DomainObjectCollectionEventReceiver (newOpposites);

      _customerEndPoint.ReplaceOppositeCollection (newOpposites);

      Assert.That (oldEventListener.HasAddedEventBeenCalled, Is.False);
      Assert.That (oldEventListener.HasAddingEventBeenCalled, Is.False);
      Assert.That (oldEventListener.HasRemovedEventBeenCalled, Is.False);
      Assert.That (oldEventListener.HasRemovingEventBeenCalled, Is.False);

      Assert.That (newEventListener.HasAddedEventBeenCalled, Is.False);
      Assert.That (newEventListener.HasAddingEventBeenCalled, Is.False);
      Assert.That (newEventListener.HasRemovedEventBeenCalled, Is.False);
      Assert.That (newEventListener.HasRemovingEventBeenCalled, Is.False);
    }

    [Test]
    public void ReplaceOppositeCollection_HasChanged_OnlyWhenSetToNewCollection ()
    {
      Assert.That (_customerEndPoint.HasChanged, Is.False);

      _customerEndPoint.ReplaceOppositeCollection (_customerEndPoint.OppositeDomainObjects);
      Assert.That (_customerEndPoint.HasChanged, Is.False);

      var newOpposites = new OrderCollection { _orderWithoutOrderItem };
      _customerEndPoint.ReplaceOppositeCollection (newOpposites);
      Assert.That (_customerEndPoint.HasChanged, Is.True);
    }

    [Test]
    public void ReplaceOppositeCollection_HasChanged_EvenWhenSetToEqualCollection ()
    {
      Assert.That (_customerEndPoint.HasChanged, Is.False);

      var newOpposites = new OrderCollection { _order1, _orderWithoutOrderItem };
      Assert.That (newOpposites, Is.EqualTo (_customerEndPoint.OppositeDomainObjects));
      Assert.That (newOpposites, Is.Not.SameAs (_customerEndPoint.OppositeDomainObjects));

      _customerEndPoint.ReplaceOppositeCollection (newOpposites);
      Assert.That (_customerEndPoint.HasChanged, Is.True);
    }

    [Test]
    public void ReplaceOppositeCollection_RemembersOriginalCollection ()
    {
      var oldOpposites = _customerEndPoint.OppositeDomainObjects;
      var oldOriginalOpposites = _customerEndPoint.OriginalOppositeDomainObjectsContents;

      var newOpposites = new OrderCollection { _order2 };
      _customerEndPoint.ReplaceOppositeCollection (newOpposites);

      Assert.That (_customerEndPoint.OriginalOppositeDomainObjectsContents, Is.SameAs (oldOriginalOpposites));
      Assert.That (_customerEndPoint.OriginalOppositeDomainObjectsReference, Is.SameAs (oldOpposites));
    }

    [Test]
    public void ReplaceOppositeCollection_SetsChangeDelegateOfNewCollection ()
    {
      var newOpposites = new OrderCollection { _order2 };
      _customerEndPoint.ReplaceOppositeCollection (newOpposites);

      Assert.That (newOpposites.ChangeDelegate, Is.SameAs (_customerEndPoint));
    }

    [Test]
    public void ReplaceOppositeCollection_ResetsChangeDelegateOfOldCollection ()
    {
      var oldOpposites = _customerEndPoint.OppositeDomainObjects;

      var newOpposites = new OrderCollection { _order2 };
      _customerEndPoint.ReplaceOppositeCollection (newOpposites);

      Assert.That (oldOpposites.ChangeDelegate, Is.Null);
    }

    [Test]
    public void ReplaceOppositeCollection_SetsTouchedFlag ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);

      var newOpposites = new OrderCollection { _order2 };
      _customerEndPoint.ReplaceOppositeCollection (newOpposites);

      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void ReplaceOppositeCollection_SelfSet ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);

      var originalOpposites = _customerEndPoint.OppositeDomainObjects;
      _customerEndPoint.ReplaceOppositeCollection (_customerEndPoint.OppositeDomainObjects);

      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.SameAs (originalOpposites));
      Assert.That (_customerEndPoint.OppositeDomainObjects.AssociatedEndPoint, Is.SameAs (_customerEndPoint));
    }

    [Test]
    public void RollbackAfterReplace_RestoresPreviousReference ()
    {
      var oldOpposites = _customerEndPoint.OppositeDomainObjects;

      var newOpposites = new OrderCollection { _orderWithoutOrderItem };
      _customerEndPoint.ReplaceOppositeCollection (newOpposites); // replace collection

      _customerEndPoint.Rollback ();

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.SameAs (oldOpposites));
      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.EqualTo (new[] { _order1, _orderWithoutOrderItem }));
      Assert.That (_customerEndPoint.OppositeDomainObjects.ChangeDelegate, Is.SameAs (_customerEndPoint));
      Assert.That (newOpposites.ChangeDelegate, Is.Null);
    }

    [Test]
    public void RollbackAfterReplace_RestoresPreviousReference_UndoesModifications_LeavesModificationOnDetached ()
    {
      _customerEndPoint.OppositeDomainObjects.Clear (); // modify collection
      var oldOpposites = _customerEndPoint.OppositeDomainObjects;

      var newOpposites = new OrderCollection { _orderWithoutOrderItem };
      _customerEndPoint.ReplaceOppositeCollection (newOpposites); // replace collection

      newOpposites.Add (_order1);

      _customerEndPoint.Rollback ();

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.SameAs (oldOpposites));
      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.EqualTo (new[] { _order1, _orderWithoutOrderItem }));
      Assert.That (_customerEndPoint.OppositeDomainObjects.ChangeDelegate, Is.SameAs (_customerEndPoint));
      Assert.That (newOpposites.ChangeDelegate, Is.Null);
      Assert.That (newOpposites, Is.EqualTo (new[] { _orderWithoutOrderItem, _order1 })); // does not undo changes on detached collection
    }

    [Test]
    public void CommitAfterReplace_SavesReference ()
    {
      var oldOpposites = _customerEndPoint.OppositeDomainObjects;

      oldOpposites.Clear (); // modify collection

      var newOpposites = new OrderCollection { _orderWithoutOrderItem };
      _customerEndPoint.ReplaceOppositeCollection (newOpposites); // replace collection
      _customerEndPoint.Commit ();

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.SameAs (newOpposites));
      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.EqualTo (new[] { _orderWithoutOrderItem }));
      Assert.That (_customerEndPoint.OppositeDomainObjects.ChangeDelegate, Is.SameAs (_customerEndPoint));
      Assert.That (oldOpposites.ChangeDelegate, Is.Null);

      _customerEndPoint.Rollback ();

      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.SameAs (newOpposites));
      Assert.That (_customerEndPoint.OppositeDomainObjects, Is.EqualTo (new[] { _orderWithoutOrderItem }));
      Assert.That (_customerEndPoint.OppositeDomainObjects.ChangeDelegate, Is.SameAs (_customerEndPoint));
      Assert.That (oldOpposites.ChangeDelegate, Is.Null);
    }

    private IDomainObjectCollectionData GetDomainObjectCollectionData (DomainObjectCollection collection)
    {
      var decorator = DomainObjectCollectionDataTestHelper.GetCollectionDataAndCheckType<ArgumentCheckingCollectionDataDecorator> (collection);
      return DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<IDomainObjectCollectionData> (decorator);
    }
  }
}