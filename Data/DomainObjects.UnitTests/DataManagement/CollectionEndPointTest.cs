using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
  [TestFixture]
  public class CollectionEndPointTest : RelationEndPointBaseTest
  {
    private RelationEndPointID _customerEndPointID;
    private DomainObjectCollection _orders;
    private CollectionEndPoint _customerEndPoint;
    private DomainObject _order1;
    private DomainObject _order2;

    public CollectionEndPointTest ()
    {
    }

    public override void SetUp ()
    {
      base.SetUp ();

      _customerEndPointID = new RelationEndPointID (DomainObjectIDs.Customer1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders");
      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _order2 = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);

      _orders = new OrderCollection ();
      _orders.Add (_order1);
      _orders.Add (_order2);

      _customerEndPoint = CreateCollectionEndPoint (_customerEndPointID, _orders);
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreEqual (_customerEndPointID, _customerEndPoint.ID);

      Assert.AreEqual (_orders.Count, _customerEndPoint.OriginalOppositeDomainObjects.Count);
      Assert.IsNotNull (_customerEndPoint.OriginalOppositeDomainObjects[_order1.ID]);
      Assert.IsNotNull (_customerEndPoint.OriginalOppositeDomainObjects[_order2.ID]);

      Assert.AreSame (_orders, _customerEndPoint.OppositeDomainObjects);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void InitializeWithInvalidRelationEndPointID ()
    {
      CollectionEndPoint endPoint = CreateCollectionEndPoint (null, _orders);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void InitializeWithNullObjectIDCollection ()
    {
      CollectionEndPoint endPoint = CreateCollectionEndPoint (_customerEndPointID, null);
    }

    [Test]
    [ExpectedException (typeof (DataManagementException), ExpectedMessage = "Internal error: CollectionEndPoint must have an ILinkChangeDelegate registered.")]
    public void RemoveFromOppositeDomainObjects ()
    {
      CollectionEndPoint collectionEndPoint = new CollectionEndPoint (
          ClientTransactionMock, _customerEndPointID, _orders);

      collectionEndPoint.OppositeDomainObjects.Remove (_order1.ID);
    }

    [Test]
    [ExpectedException (typeof (DataManagementException), ExpectedMessage = "Internal error: CollectionEndPoint must have an ILinkChangeDelegate registered.")]
    public void AddToOppositeDomainObjects ()
    {
      Order newOrder = Order.GetObject (DomainObjectIDs.Order2);

      CollectionEndPoint collectionEndPoint = new CollectionEndPoint (
          ClientTransactionMock, _customerEndPointID, _orders);

      collectionEndPoint.OppositeDomainObjects.Add (newOrder);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void ChangeOriginalOppositeDomainObjects ()
    {
      _customerEndPoint.OriginalOppositeDomainObjects.Remove (_order1.ID);
    }

    [Test]
    public void HasChangedFalse ()
    {
      Assert.IsFalse (_customerEndPoint.HasChanged);
    }

    [Test]
    public void HasChangedTrueEvenIfSameElements ()
    {
      Assert.IsFalse (_customerEndPoint.HasChanged);
      _customerEndPoint.OppositeDomainObjects.Add (Order.NewObject ());
      Assert.IsTrue (_customerEndPoint.HasChanged);
      _customerEndPoint.OppositeDomainObjects.RemoveAt (_customerEndPoint.OppositeDomainObjects.Count - 1);
      Assert.IsTrue (_customerEndPoint.HasChanged);
    }

    [Test]
    public void OriginalOppositeDomainObjectsType ()
    {
      Assert.AreEqual (typeof (OrderCollection), _customerEndPoint.OriginalOppositeDomainObjects.GetType ());
      Assert.IsTrue (_customerEndPoint.OriginalOppositeDomainObjects.IsReadOnly);

      Assert.AreEqual (
          _customerEndPoint.OppositeDomainObjects.RequiredItemType,
          _customerEndPoint.OriginalOppositeDomainObjects.RequiredItemType);
    }

    [Test]
    public void ChangeOppositeDomainObjects ()
    {
      Assert.AreEqual (_customerEndPoint.OriginalOppositeDomainObjects.Count, _customerEndPoint.OppositeDomainObjects.Count);

      _customerEndPoint.BeginRelationChange (CreateObjectEndPoint (_order1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", _customerEndPoint.ObjectID));
      _customerEndPoint.PerformRelationChange ();
      _customerEndPoint.EndRelationChange ();

      Assert.IsTrue (_customerEndPoint.OriginalOppositeDomainObjects.Count != _customerEndPoint.OppositeDomainObjects.Count);
    }

    [Test]
    public void PerformDelete ()
    {
      Assert.AreEqual (_customerEndPoint.OriginalOppositeDomainObjects.Count, _customerEndPoint.OppositeDomainObjects.Count);

      _customerEndPoint.BeginRelationChange (CreateObjectEndPoint (_order1, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", _customerEndPoint.ObjectID));
      _customerEndPoint.PerformDelete ();
      _customerEndPoint.EndRelationChange ();

      Assert.IsTrue (_customerEndPoint.OriginalOppositeDomainObjects.Count != _customerEndPoint.OppositeDomainObjects.Count);
      Assert.AreEqual (0, _customerEndPoint.OppositeDomainObjects.Count);
    }

    private void CheckIfRelationEndPointsAreEqual (CollectionEndPoint expected, CollectionEndPoint actual)
    {
      ArgumentUtility.CheckNotNull ("expected", expected);
      ArgumentUtility.CheckNotNull ("actual", actual);

      Assert.AreNotSame (expected, actual);

      Assert.AreSame (expected.ChangeDelegate, actual.ChangeDelegate);
      Assert.AreSame (expected.ClientTransaction, actual.ClientTransaction);
      Assert.AreSame (expected.Definition, actual.Definition);
      Assert.AreSame (expected.GetDomainObject(), actual.GetDomainObject ());
      Assert.AreEqual (expected.HasChanged, actual.HasChanged);
      Assert.AreEqual (expected.ID, actual.ID);
      Assert.AreEqual (expected.ObjectID, actual.ObjectID);

      Assert.IsNotNull (actual.OppositeDomainObjects);
      Assert.AreNotSame (expected.OppositeDomainObjects, actual.OppositeDomainObjects);

      Assert.AreEqual (expected.OppositeDomainObjects.Count, actual.OppositeDomainObjects.Count);
      for (int i = 0; i < expected.OppositeDomainObjects.Count; ++i)
        Assert.AreSame (expected.OppositeDomainObjects[i], actual.OppositeDomainObjects[i]);

      Assert.IsNotNull (actual.OriginalOppositeDomainObjects);
      Assert.AreNotSame (expected.OriginalOppositeDomainObjects, actual.OriginalOppositeDomainObjects);
      Assert.AreEqual (expected.OriginalOppositeDomainObjects.IsReadOnly, actual.OriginalOppositeDomainObjects.IsReadOnly);

      Assert.AreEqual (expected.OriginalOppositeDomainObjects.Count, actual.OriginalOppositeDomainObjects.Count);
      for (int i = 0; i < expected.OriginalOppositeDomainObjects.Count; ++i)
        Assert.AreSame (expected.OriginalOppositeDomainObjects[i], actual.OriginalOppositeDomainObjects[i]);
    }

    [Test]
    public void CloneUnchanged ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem item1 = order.OrderItems[0];
      OrderItem item2 = order.OrderItems[1];

      RelationEndPointID id = new RelationEndPointID (order.ID, typeof (Order) + ".OrderItems");
      CollectionEndPoint endPoint = (CollectionEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[id];
      Assert.IsNotNull (endPoint.ChangeDelegate);
      Assert.AreSame (ClientTransactionMock, endPoint.ClientTransaction);
      Assert.IsNotNull (endPoint.Definition);
      Assert.AreSame (order, endPoint.GetDomainObject());
      Assert.IsFalse (endPoint.HasChanged);
      Assert.AreEqual (id, endPoint.ID);
      Assert.AreEqual (order.ID, endPoint.ObjectID);
      Assert.IsNotNull (endPoint.OppositeDomainObjects);
      
      Assert.AreEqual (2, endPoint.OppositeDomainObjects.Count);
      Assert.AreSame (item1, endPoint.OppositeDomainObjects[0]);
      Assert.AreSame (item2, endPoint.OppositeDomainObjects[1]);
      
      Assert.AreNotSame (endPoint.OppositeDomainObjects, endPoint.OriginalOppositeDomainObjects);

      Assert.AreEqual (2, endPoint.OriginalOppositeDomainObjects.Count);
      Assert.AreSame (item1, endPoint.OriginalOppositeDomainObjects[0]);
      Assert.AreSame (item2, endPoint.OriginalOppositeDomainObjects[1]);

      CollectionEndPoint clone = (CollectionEndPoint) endPoint.Clone ();

      Assert.IsNotNull (clone);

      CheckIfRelationEndPointsAreEqual (endPoint, clone);
    }

    [Test]
    public void CloneChanged ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      OrderItem originalItem1 = order.OrderItems[0];
      OrderItem originalItem2 = order.OrderItems[1];

      order.OrderItems.Clear ();

      OrderItem item1 = OrderItem.NewObject ();
      OrderItem item2 = OrderItem.NewObject ();
      OrderItem item3 = OrderItem.NewObject ();

      order.OrderItems.Add (item1);
      order.OrderItems.Add (item2);
      order.OrderItems.Add (item3);

      RelationEndPointID id = new RelationEndPointID (order.ID, typeof (Order) + ".OrderItems");
      CollectionEndPoint endPoint = (CollectionEndPoint) ClientTransactionMock.DataManager.RelationEndPointMap[id];
      Assert.IsNotNull (endPoint.ChangeDelegate);
      Assert.AreSame (ClientTransactionMock, endPoint.ClientTransaction);
      Assert.IsNotNull (endPoint.Definition);
      Assert.AreSame (order, endPoint.GetDomainObject ());
      Assert.IsTrue (endPoint.HasChanged);
      Assert.AreEqual (id, endPoint.ID);
      Assert.AreEqual (order.ID, endPoint.ObjectID);
      Assert.IsNotNull (endPoint.OppositeDomainObjects);

      Assert.AreEqual (3, endPoint.OppositeDomainObjects.Count);
      Assert.AreSame (item1, endPoint.OppositeDomainObjects[0]);
      Assert.AreSame (item2, endPoint.OppositeDomainObjects[1]);
      Assert.AreSame (item3, endPoint.OppositeDomainObjects[2]);

      Assert.AreNotSame (endPoint.OppositeDomainObjects, endPoint.OriginalOppositeDomainObjects);

      Assert.AreEqual (2, endPoint.OriginalOppositeDomainObjects.Count);
      Assert.AreSame (originalItem1, endPoint.OriginalOppositeDomainObjects[0]);
      Assert.AreSame (originalItem2, endPoint.OriginalOppositeDomainObjects[1]);

      CollectionEndPoint clone = (CollectionEndPoint) endPoint.Clone ();

      Assert.IsNotNull (clone);

      CheckIfRelationEndPointsAreEqual (endPoint, clone);
    }
  }
}
