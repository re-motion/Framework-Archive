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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using System.Linq;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class CollectionEndPointIntegrationTest : ClientTransactionBaseTest
  {
    private Order _order1; // belongs to customer1
    private Order _orderWithoutOrderItem; // belongs to customer1
    private Order _order2; // belongs to customer3

    private CollectionEndPoint _customerEndPoint;
    
    public override void SetUp ()
    {
      base.SetUp ();

      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _orderWithoutOrderItem = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);
      _order2 = Order.GetObject (DomainObjectIDs.Order2);

      _customerEndPoint = (CollectionEndPoint) 
          ClientTransactionMock.DataManager.GetRelationEndPointWithLazyLoad (
            RelationEndPointID.Create (DomainObjectIDs.Customer1, typeof (Customer), "Orders"));
    }

    [Test]
    public void AddToCollection ()
    {
      var newOrder = Order.NewObject ();

      _customerEndPoint.Collection.Add (newOrder);

      Assert.That (_customerEndPoint.Collection, Is.EqualTo (new[] { _order1, _orderWithoutOrderItem, newOrder }), "changes go down to actual data store");
      Assert.That (newOrder.Customer, Is.SameAs (_customerEndPoint.GetDomainObject()), "bidirectional modification");
    }

    [Test]
    public void RemoveFromCollection ()
    {
      _customerEndPoint.Collection.Remove (_order1.ID);

      Assert.That (_customerEndPoint.Collection, Is.EqualTo (new[] { _orderWithoutOrderItem }), "changes go down to actual data store");
      Assert.That (_order1.Customer, Is.Null, "bidirectional modification");
    }

    [Test]
    public void SetCollection ()
    {
      var oldOpposites = _customerEndPoint.Collection;
      var newOpposites = new OrderCollection { _orderWithoutOrderItem };

      SetCollectionAndNotify (_customerEndPoint, newOpposites);

      Assert.That (_customerEndPoint.Collection, Is.SameAs (newOpposites));
      Assert.That (_customerEndPoint.Collection, Is.EqualTo (new[] { _orderWithoutOrderItem }));
      Assert.That (oldOpposites, Is.EqualTo (new[] { _order1, _orderWithoutOrderItem }));

      Assert.That (newOpposites.IsAssociatedWith (_customerEndPoint), Is.True);
      Assert.That (oldOpposites.IsAssociatedWith (null), Is.True);
    }

    [Test]
    public void SetCollection_DataStrategy_OfOldOpposites ()
    {
      var oldOpposites = _customerEndPoint.Collection;
      var originalDataOfOldOpposites = GetDomainObjectCollectionData (oldOpposites);
      var originalDataStoreOfOldOpposites = originalDataOfOldOpposites.GetDataStore ();

      var newOpposites = new OrderCollection { _orderWithoutOrderItem };
      SetCollectionAndNotify (_customerEndPoint, newOpposites);

      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy(oldOpposites, typeof (Order));

      // old collection got a new data store...
      var dataStoreOfOldOpposites = 
          DomainObjectCollectionDataTestHelper.GetDataStrategyAndCheckType<IDomainObjectCollectionData> (oldOpposites).GetDataStore();
      Assert.That (dataStoreOfOldOpposites, Is.Not.SameAs (originalDataStoreOfOldOpposites));

      // with the data it had before!
      Assert.That (dataStoreOfOldOpposites.ToArray (), Is.EqualTo (new[] { _order1, _orderWithoutOrderItem }));
    }

    [Test]
    public void SetCollection_DataStrategy_OfNewOpposites ()
    {
      var oldOpposites = _customerEndPoint.Collection;
      var originalDataOfOldOpposites = GetDomainObjectCollectionData (oldOpposites);
      var originalDataStoreOfOldOpposites = originalDataOfOldOpposites.GetDataStore ();
      
      var newOpposites = new OrderCollection { _orderWithoutOrderItem };
      SetCollectionAndNotify (_customerEndPoint, newOpposites);

      DomainObjectCollectionDataTestHelper.CheckAssociatedCollectionStrategy (newOpposites, typeof (Order), _customerEndPoint.ID);

      // end point still holds on to the same old data store...
      Assert.That (GetDomainObjectCollectionData (newOpposites).GetDataStore(), Is.SameAs (originalDataStoreOfOldOpposites));

      // but with the new data!
      Assert.That (originalDataStoreOfOldOpposites.ToArray (), Is.EqualTo (new[] { _orderWithoutOrderItem }));
    }

    [Test]
    public void SetCollection_PerformsAllBidirectionalChanges ()
    {
      var newOpposites = new OrderCollection { _orderWithoutOrderItem, _order2};

      var customer3 = Customer.GetObject (DomainObjectIDs.Customer3);
      
      Assert.That (_order1.Customer, Is.SameAs (_customerEndPoint.GetDomainObject ()));
      Assert.That (_orderWithoutOrderItem.Customer, Is.SameAs (_customerEndPoint.GetDomainObject ()));
      Assert.That (_order2.Customer, Is.SameAs (customer3));
      Assert.That (customer3.Orders, Has.Member(_order2));

      SetCollectionAndNotify (_customerEndPoint, newOpposites);

      Assert.That (_order1.Customer, Is.Null);
      Assert.That (_orderWithoutOrderItem.Customer, Is.SameAs (_customerEndPoint.GetDomainObject ()));
      Assert.That (_order2.Customer, Is.SameAs (_customerEndPoint.GetDomainObject()));
      Assert.That (customer3.Orders, Has.No.Member(_order2));
    }

    [Test]
    public void SetCollection_RaisesNoEventsOnCollections ()
    {
      var oldOpposites = _customerEndPoint.Collection;
      var oldEventListener = new DomainObjectCollectionEventReceiver (oldOpposites);

      var newOpposites = new OrderCollection { _orderWithoutOrderItem };
      var newEventListener = new DomainObjectCollectionEventReceiver (newOpposites);

      SetCollectionAndNotify (_customerEndPoint, newOpposites);

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
    public void SetCollection_SourceCollection_IsReadOnly ()
    {
      DomainObjectCollectionDataTestHelper.MakeCollectionReadOnly (_customerEndPoint.Collection);

      var newOpposites = new OrderCollection { _orderWithoutOrderItem, _order2 };
      var oldOpposites = _customerEndPoint.Collection;
      Assert.That (oldOpposites.IsReadOnly, Is.True);

      SetCollectionAndNotify (_customerEndPoint, newOpposites);

      Assert.That (_customerEndPoint.Collection, Is.SameAs (newOpposites));
      Assert.That (_customerEndPoint.OriginalCollection, Is.SameAs (oldOpposites));
    }

    [Test]
    public void SetCollection_TargetCollection_IsReadOnly ()
    {
      var newOpposites = new OrderCollection { _orderWithoutOrderItem, _order2 }.Clone (true);
      Assert.That (newOpposites.IsReadOnly, Is.True);
      var oldOpposites = _customerEndPoint.Collection;

      SetCollectionAndNotify (_customerEndPoint, newOpposites);

      Assert.That (_customerEndPoint.Collection, Is.SameAs (newOpposites));
      Assert.That (_customerEndPoint.OriginalCollection, Is.SameAs (oldOpposites));
    }

    [Test]
    public void SetCollection_HasChanged_OnlyWhenSetToNewCollection ()
    {
      Assert.That (_customerEndPoint.HasChanged, Is.False);

      SetCollectionAndNotify (_customerEndPoint, _customerEndPoint.Collection);
      Assert.That (_customerEndPoint.HasChanged, Is.False);

      var newOpposites = new OrderCollection { _orderWithoutOrderItem };
      SetCollectionAndNotify (_customerEndPoint, newOpposites);
      Assert.That (_customerEndPoint.HasChanged, Is.True);
    }

    [Test]
    public void SetCollection_HasChanged_EvenWhenSetToEqualCollection ()
    {
      Assert.That (_customerEndPoint.HasChanged, Is.False);

      var newOpposites = new OrderCollection { _order1, _orderWithoutOrderItem };
      Assert.That (newOpposites, Is.EqualTo (_customerEndPoint.Collection));
      Assert.That (newOpposites, Is.Not.SameAs (_customerEndPoint.Collection));

      SetCollectionAndNotify (_customerEndPoint, newOpposites);
      Assert.That (_customerEndPoint.HasChanged, Is.True);
    }

    [Test]
    public void SetCollection_RemembersOriginalCollectionAndData ()
    {
      var oldOpposites = _customerEndPoint.Collection;
      var oldOriginalOpposites = _customerEndPoint.GetCollectionWithOriginalData();

      var newOpposites = new OrderCollection { _order2 };
      SetCollectionAndNotify (_customerEndPoint, newOpposites);

      Assert.That (_customerEndPoint.GetCollectionWithOriginalData(), Is.EqualTo (oldOriginalOpposites));
      Assert.That (_customerEndPoint.OriginalCollection, Is.SameAs (oldOpposites));
    }

    [Test]
    public void SetCollection_SetsTouchedFlag ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);

      var newOpposites = new OrderCollection { _order2 };
      SetCollectionAndNotify (_customerEndPoint, newOpposites);

      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void SetCollection_SelfSet ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);

      var originalOpposites = _customerEndPoint.Collection;
      SetCollectionAndNotify (_customerEndPoint, _customerEndPoint.Collection);

      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
      Assert.That (_customerEndPoint.Collection, Is.SameAs (originalOpposites));
      Assert.That (_customerEndPoint.Collection.IsAssociatedWith (_customerEndPoint), Is.True);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void GetCollectionWithOriginalData_IsReadOnly ()
    {
      _customerEndPoint.GetCollectionWithOriginalData().Remove (_order1.ID);
    }

    [Test]
    public void Rollback_AfterReplace_RestoresPreviousReference ()
    {
      var oldOpposites = _customerEndPoint.Collection;

      var newOpposites = new OrderCollection { _order2 };
      SetCollectionAndNotify (_customerEndPoint, newOpposites); // replace collection

      _customerEndPoint.Rollback ();

      Assert.That (_customerEndPoint.Collection, Is.SameAs (oldOpposites));
      Assert.That (_customerEndPoint.Collection, Is.EqualTo (new[] { _order1, _orderWithoutOrderItem }));
    }

    [Test]
    public void Rollback_AfterReplace_RestoresDelegationChain ()
    {
      var oldCollection = _customerEndPoint.Collection;
      var oldCollectionDataStore = 
          DomainObjectCollectionDataTestHelper.GetDataStrategyAndCheckType<IDomainObjectCollectionData> (oldCollection).GetDataStore ();

      var newCollection = new OrderCollection { _order2 };
      SetCollectionAndNotify (_customerEndPoint, newCollection);

      Assert.That (_customerEndPoint.Collection, Is.SameAs (newCollection));
      Assert.That (newCollection.IsAssociatedWith (_customerEndPoint), Is.True);
      Assert.That (oldCollection.IsAssociatedWith (null), Is.True);

      _customerEndPoint.Rollback ();

      DomainObjectCollectionDataTestHelper.CheckAssociatedCollectionStrategy (oldCollection, typeof (Order), _customerEndPoint.ID);
      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy (newCollection, typeof (Order));

      Assert.That (GetDomainObjectCollectionData (oldCollection).GetDataStore (), Is.SameAs (oldCollectionDataStore));
    }

    [Test]
    public void Rollback_AfterReplace_RestoresPreviousReference_UndoesModifications_LeavesModificationOnDetached ()
    {
      _customerEndPoint.Collection.Clear (); // modify collection
      var oldOpposites = _customerEndPoint.Collection;

      var newOpposites = new OrderCollection { _orderWithoutOrderItem };
      SetCollectionAndNotify (_customerEndPoint, newOpposites); // replace collection

      newOpposites.Add (_order1);

      _customerEndPoint.Rollback ();

      Assert.That (_customerEndPoint.Collection, Is.SameAs (oldOpposites));
      Assert.That (_customerEndPoint.Collection, Is.EqualTo (new[] { _order1, _orderWithoutOrderItem }));
      Assert.That (newOpposites, Is.EqualTo (new[] { _orderWithoutOrderItem, _order1 })); // does not undo changes on detached collection
    }

    [Test]
    public void Rollback_ReadOnly ()
    {
      _customerEndPoint.Collection.Add (_order2);
      DomainObjectCollectionDataTestHelper.MakeCollectionReadOnly(_customerEndPoint.Collection);

      Assert.That (_customerEndPoint.Collection.IsReadOnly, Is.True);

      _customerEndPoint.Rollback();

      Assert.That (_customerEndPoint.Collection, Is.EqualTo (new[] { _order1, _orderWithoutOrderItem }));
      Assert.That (_customerEndPoint.GetCollectionWithOriginalData(), Is.EqualTo (new[] { _order1, _orderWithoutOrderItem }));
      Assert.That (_customerEndPoint.OriginalCollection, Is.SameAs (_customerEndPoint.Collection));
      Assert.That (_customerEndPoint.Collection.IsReadOnly, Is.True);
    }

    [Test]
    public void Commit_AfterReplace_SavesReference ()
    {
      var oldOpposites = _customerEndPoint.Collection;

      oldOpposites.Clear (); // modify collection

      var newOpposites = new OrderCollection { _orderWithoutOrderItem };
      SetCollectionAndNotify (_customerEndPoint, newOpposites); // replace collection
      _customerEndPoint.Commit ();

      Assert.That (_customerEndPoint.Collection, Is.SameAs (newOpposites));
      Assert.That (_customerEndPoint.Collection, Is.EqualTo (new[] { _orderWithoutOrderItem }));

      _customerEndPoint.Rollback ();

      Assert.That (_customerEndPoint.Collection, Is.SameAs (newOpposites));
      Assert.That (_customerEndPoint.Collection, Is.EqualTo (new[] { _orderWithoutOrderItem }));
    }

    [Test]
    public void Commit_AfterReplace_DelegationChain ()
    {
      var oldCollection = _customerEndPoint.Collection;

      var newCollection = new OrderCollection { _order2 };
      SetCollectionAndNotify (_customerEndPoint, newCollection);

      Assert.That (_customerEndPoint.Collection, Is.SameAs (newCollection));
      Assert.That (newCollection.IsAssociatedWith (_customerEndPoint), Is.True);
      Assert.That (oldCollection.IsAssociatedWith (null));

      _customerEndPoint.Commit ();

      DomainObjectCollectionDataTestHelper.CheckStandAloneCollectionStrategy (oldCollection, typeof (Order));
      DomainObjectCollectionDataTestHelper.CheckAssociatedCollectionStrategy (newCollection, typeof (Order), _customerEndPoint.ID);
    }

    [Test]
    public void Commit_ReadOnly ()
    {
      _customerEndPoint.Collection.Add (_order2);
      DomainObjectCollectionDataTestHelper.MakeCollectionReadOnly(_customerEndPoint.Collection);

      _customerEndPoint.Commit ();

      Assert.That (_customerEndPoint.Collection, Is.EqualTo (new[] { _order1, _orderWithoutOrderItem, _order2 }));
      Assert.That (_customerEndPoint.GetCollectionWithOriginalData(), Is.EqualTo (new[] { _order1, _orderWithoutOrderItem, _order2 }));
      Assert.That (_customerEndPoint.OriginalCollection, Is.SameAs (_customerEndPoint.Collection));
      Assert.That (_customerEndPoint.Collection.IsReadOnly, Is.True);
    }

    [Test]
    public void ChangesToDataState_CauseTransactionListenerNotifications ()
    {
      var listener = ClientTransactionTestHelper.CreateAndAddListenerMock (_customerEndPoint.ClientTransaction);

      _customerEndPoint.Collection.Add (_order2);

      listener.AssertWasCalled (mock => mock.VirtualRelationEndPointStateUpdated (_customerEndPoint.ClientTransaction, _customerEndPoint.ID, null));
    }

    [Test]
    public void HasBeenTouchedAddAndRemove_LeavingSameElements ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      _customerEndPoint.Collection.Add (Order.NewObject ());
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
      _customerEndPoint.Collection.RemoveAt (_customerEndPoint.Collection.Count - 1);
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasBeenTouchedInsert ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      _customerEndPoint.Collection.Insert (0, Order.NewObject ());
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasBeenTouchedRemove ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      _customerEndPoint.Collection.Remove (_customerEndPoint.Collection[0]);
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasBeenTouchedRemoveNonExisting ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      _customerEndPoint.Collection.Remove (Order.NewObject ());
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasBeenTouchedClear ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      _customerEndPoint.Collection.Clear ();
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasBeenTouchedClearEmpty ()
    {
      _customerEndPoint.Collection.Clear ();
      _customerEndPoint.Commit ();

      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      _customerEndPoint.Collection.Clear ();
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasBeenTouchedReplace ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      _customerEndPoint.Collection[0] = Order.NewObject ();
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasBeenTouchedReplaceWithSame ()
    {
      Assert.That (_customerEndPoint.HasBeenTouched, Is.False);
      _customerEndPoint.Collection[0] = _customerEndPoint.Collection[0];
      Assert.That (_customerEndPoint.HasBeenTouched, Is.True);
    }

    private IDomainObjectCollectionData GetDomainObjectCollectionData (DomainObjectCollection collection)
    {
      var decorator = DomainObjectCollectionDataTestHelper.GetDataStrategyAndCheckType<ModificationCheckingCollectionDataDecorator> (collection);
      return DomainObjectCollectionDataTestHelper.GetWrappedDataAndCheckType<IDomainObjectCollectionData> (decorator);
    }

    private void SetCollectionAndNotify (CollectionEndPoint collectionEndPoint, DomainObjectCollection newCollection)
    {
      collectionEndPoint.CreateSetCollectionCommand (newCollection).ExpandToAllRelatedObjects ().NotifyAndPerform ();
    }
  }
}