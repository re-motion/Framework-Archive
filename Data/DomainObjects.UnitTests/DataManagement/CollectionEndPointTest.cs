using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
  [TestFixture]
  public class CollectionEndPointTest : DatabaseTest
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

      _orders = new DomainObjectCollection ();
      _orders.Add (_order1);
      _orders.Add (_order2);

      _customerEndPoint = new CollectionEndPoint (_customerEndPointID, _orders);
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
      CollectionEndPoint endPoint = new CollectionEndPoint (null, _orders);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void InitializeWithNullObjectIDCollection ()
    {
      CollectionEndPoint endPoint = new CollectionEndPoint (_customerEndPointID, null);
    }

    [Test]
    [ExpectedException (typeof (DataManagementException), "Internal error: CollectionEndPoint must have an ILinkChangeDelegate registered.")]
    public void RemoveFromOppositeDomainObjects ()
    {
      _customerEndPoint.OppositeDomainObjects.Remove (_order1.ID);
    }

    [Test]
    [ExpectedException (typeof (DataManagementException), "Internal error: CollectionEndPoint must have an ILinkChangeDelegate registered.")]
    public void AddToOppositeDomainObjects ()
    {
      Order newOrder = Order.GetObject (DomainObjectIDs.Order2);
      _customerEndPoint.OppositeDomainObjects.Add (newOrder);
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
  }
}
