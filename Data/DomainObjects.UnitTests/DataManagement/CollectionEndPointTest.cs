using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
[TestFixture]
public class CollectionEndPointTest : RelationEndPointBaseTest
{
  // types

  // static members and constants

  // member fields

  private RelationEndPointID _customerEndPointID;
  private DomainObjectCollection _orders;
  private CollectionEndPoint _customerEndPoint;
  private DomainObject _order1;
  private DomainObject _order2;

  // construction and disposing

  public CollectionEndPointTest ()
  {
  }

  // methods and properties

  public override void SetUp ()
  {
    base.SetUp ();

    _customerEndPointID = new RelationEndPointID (DomainObjectIDs.Customer1, "Orders");
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
  [ExpectedException (typeof (DataManagementException), "Internal error: CollectionEndPoint must have an ILinkChangeDelegate registered.")]
  public void RemoveFromOppositeDomainObjects ()
  {
    CollectionEndPoint collectionEndPoint = new CollectionEndPoint (
        ClientTransactionMock, _customerEndPointID, _orders);
    
    collectionEndPoint.OppositeDomainObjects.Remove (_order1.ID);
  }

  [Test]
  [ExpectedException (typeof (DataManagementException), "Internal error: CollectionEndPoint must have an ILinkChangeDelegate registered.")]
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

    _customerEndPoint.BeginRelationChange (CreateObjectEndPoint (_order1, "Customer", _customerEndPoint.ObjectID));
    _customerEndPoint.PerformRelationChange ();
    _customerEndPoint.EndRelationChange ();

    Assert.IsTrue (_customerEndPoint.OriginalOppositeDomainObjects.Count != _customerEndPoint.OppositeDomainObjects.Count);
  }

  [Test]
  public void PerformDelete ()
  {
    Assert.AreEqual (_customerEndPoint.OriginalOppositeDomainObjects.Count, _customerEndPoint.OppositeDomainObjects.Count);

    _customerEndPoint.BeginRelationChange (CreateObjectEndPoint (_order1, "Customer", _customerEndPoint.ObjectID));
    _customerEndPoint.PerformDelete ();
    _customerEndPoint.EndRelationChange ();

    Assert.IsTrue (_customerEndPoint.OriginalOppositeDomainObjects.Count != _customerEndPoint.OppositeDomainObjects.Count);
    Assert.AreEqual (0, _customerEndPoint.OppositeDomainObjects.Count);
  }
}
}
