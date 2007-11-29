using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
  [TestFixture]
  public class RelationEndPointMapTest : ClientTransactionBaseTest
  {
    private RelationEndPointMap _map;

    public override void SetUp ()
    {
      base.SetUp ();

      _map = ClientTransactionMock.DataManager.RelationEndPointMap;
    }

    [Test]
    public void DeleteNew ()
    {
      Order newOrder = Order.NewObject ();
      Assert.IsTrue (_map.Count > 0);

      _map.PerformDelete (newOrder, _map.GetOppositeEndPointModificationsForDelete (newOrder));
      Assert.AreEqual (0, _map.Count);
    }

    [Test]
    public void CommitForDeletedObject ()
    {
      Computer computer = Computer.GetObject (DomainObjectIDs.Computer4);
      Assert.IsTrue (_map.Count > 0);

      computer.Delete ();

      DomainObjectCollection deletedDomainObjects = new DomainObjectCollection ();
      deletedDomainObjects.Add (computer);

      _map.Commit (deletedDomainObjects);

      Assert.AreEqual (0, _map.Count);
    }

    [Test]
    public void GetRelatedObject ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      RelationEndPointID endPointID = new RelationEndPointID (order.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");
      DomainObject orderTicket = _map.GetRelatedObject (endPointID, false);

      Assert.IsNotNull (orderTicket);
      Assert.AreEqual (DomainObjectIDs.OrderTicket1, orderTicket.ID);
      Assert.AreSame (OrderTicket.GetObject (DomainObjectIDs.OrderTicket1), orderTicket);
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void GetRelatedObjectIncludeDeletedFalse ()
    {
      Location location = Location.GetObject (DomainObjectIDs.Location1);

      location.Client.Delete ();

      RelationEndPointID endPointID = new RelationEndPointID (location.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Location.Client");
      _map.GetRelatedObject (endPointID, false);
    }

    [Test]
    public void GetRelatedObjectIncludeDeletedTrue ()
    {
      Location location = Location.GetObject (DomainObjectIDs.Location1);

      location.Client.Delete ();

      RelationEndPointID endPointID = new RelationEndPointID (location.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Location.Client");
      DomainObject client = _map.GetRelatedObject (endPointID, true);
      Assert.IsNotNull (client);
      Assert.AreEqual (DomainObjectIDs.Client1, client.ID);
      Assert.AreEqual (StateType.Deleted, client.State);
    }

    [Test]
    public void GetRelatedObjectWithDiscarded ()
    {
      Location location = Location.GetObject (DomainObjectIDs.Location1);
      Client newClient = Client.NewObject ();
      location.Client = newClient;
      location.Client.Delete ();

      RelationEndPointID endPointID = new RelationEndPointID (location.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Location.Client");
      DomainObject client = _map.GetRelatedObject (endPointID, true);
      Assert.IsNotNull (client);
      Assert.AreSame (newClient, client);
      Assert.IsTrue (client.IsDiscarded);
    }

    [Test]
    public void GetOriginalRelatedObjectsWithLazyLoad ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      RelationEndPointID endPointID = new RelationEndPointID (order.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems");
      DomainObjectCollection originalOrderItems = _map.GetOriginalRelatedObjects (endPointID);
      DomainObjectCollection orderItems = _map.GetRelatedObjects (endPointID);

      Assert.IsFalse (ReferenceEquals (originalOrderItems, orderItems));
    }

    [Test]
    public void GetOriginalRelatedObjectWithLazyLoad ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      RelationEndPointID endPointID = new RelationEndPointID (order.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");
      DomainObject originalOrderTicket = _map.GetOriginalRelatedObject (endPointID);
      DomainObject orderTicket = _map.GetRelatedObject (endPointID, false);

      Assert.IsTrue (ReferenceEquals (originalOrderTicket, orderTicket));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "GetRelatedObject can only be called for end points with a cardinality of 'One'.\r\nParameter name: endPointID")]
    public void GetRelatedObjectWithEndPointIDOfWrongCardinality ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      _map.GetRelatedObject (new RelationEndPointID (order.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"), false);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "GetOriginalRelatedObject can only be called for end points with a cardinality of 'One'.\r\nParameter name: endPointID")]
    public void GetOriginalRelatedObjectWithEndPointIDOfWrongCardinality ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      _map.GetOriginalRelatedObject (new RelationEndPointID (order.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "SetRelatedObject can only be called for end points with a cardinality of 'One'.\r\nParameter name: endPointID")]
    public void SetRelatedObjectWithEndPointIDOfWrongCardinality ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      _map.SetRelatedObject (new RelationEndPointID (order.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"), OrderItem.NewObject ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "GetRelatedObjects can only be called for end points with a cardinality of 'Many'.\r\nParameter name: endPointID")]
    public void GetRelatedObjectsWithEndPointIDOfWrongCardinality ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      _map.GetRelatedObjects (new RelationEndPointID (order.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "GetOriginalRelatedObjects can only be called for end points with a cardinality of 'Many'.\r\nParameter name: endPointID")]
    public void GetOriginalRelatedObjectsWithEndPointIDOfWrongCardinality ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      _map.GetOriginalRelatedObjects (new RelationEndPointID (order.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException),
      ExpectedMessage = "Domain object '.*' cannot be used in the current transaction as it was loaded or created in another transaction.",
        MatchType = MessageMatch.Regex)]
    public void SetRelatedObjectWithOtherClientTransaction ()
    {
      Order order1 = (Order) ClientTransactionMock.GetObject (DomainObjectIDs.Order1);

      OrderTicket orderTicket2;
      using (ClientTransaction.NewTransaction().EnterDiscardingScope())
      {
        orderTicket2 = OrderTicket.GetObject (DomainObjectIDs.OrderTicket2);
      }
      _map.SetRelatedObject (new RelationEndPointID (order1.ID, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"), orderTicket2);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException),
       ExpectedMessage = "Domain object '.*' cannot be used in the current transaction as it was loaded or created in another transaction.",
       MatchType = MessageMatch.Regex)]
    public void PerformCollectionAddWithOtherClientTransaction ()
    {
      Order order1 = (Order) ClientTransactionMock.GetObject (DomainObjectIDs.Order1);

      OrderItem orderItem3;
      using (ClientTransaction.NewTransaction().EnterDiscardingScope())
      {
        orderItem3 = OrderItem.GetObject (DomainObjectIDs.OrderItem3);
      }

      order1.OrderItems.Add (orderItem3);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException),
        ExpectedMessage = "Domain object '.*' cannot be used in the current transaction as it was loaded or created in another transaction.",
        MatchType = MessageMatch.Regex)]
    public void PerformCollectionInsertWithOtherClientTransaction ()
    {
      Order order1 = (Order) ClientTransactionMock.GetObject (DomainObjectIDs.Order1);

      ClientTransaction clientTransaction = ClientTransaction.NewTransaction();
      OrderItem orderItem3;

      using (ClientTransaction.NewTransaction().EnterDiscardingScope())
      {
        orderItem3 = OrderItem.GetObject (DomainObjectIDs.OrderItem3);
      }

      order1.OrderItems.Insert (0, orderItem3);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException),
        ExpectedMessage = "Domain object '.*' cannot be used in the current transaction as it was loaded or created in another transaction.",
        MatchType = MessageMatch.Regex)]
    public void PerformCollectionRemoveWithOtherClientTransaction ()
    {
      Order order1 = (Order) ClientTransactionMock.GetObject (DomainObjectIDs.Order1);

      OrderItem orderItem1;
      using (ClientTransaction.NewTransaction ().EnterDiscardingScope ())
      {
        orderItem1 = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      }

      order1.OrderItems.Remove (orderItem1);
    }

    [Test]
    public void PerformCollectionReplaceWithOtherClientTransaction ()
    {
      Order order1 = (Order) ClientTransactionMock.GetObject (DomainObjectIDs.Order1);

      OrderItem orderItem3;
      using (ClientTransaction.NewTransaction().EnterDiscardingScope())
      {
        orderItem3 = OrderItem.GetObject (DomainObjectIDs.OrderItem3);
      }

      int index = order1.OrderItems.IndexOf (DomainObjectIDs.OrderItem1);

      try
      {
        order1.OrderItems[index] = orderItem3;
        Assert.Fail ("This test expects a ClientTransactionsDifferException.");
      }
      catch (ClientTransactionsDifferException ex)
      {
        string expectedMessage = "Domain object 'OrderItem|0d7196a5-8161-4048-820d-b1bbdabe3293|System.Guid' cannot be used in the "
            + "current transaction as it was loaded or created in another transaction.";
        Assert.IsTrue (ex.Message.Contains (expectedMessage));
      }
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException),
        ExpectedMessage = "Domain object '.*' cannot be used in the current transaction as it was loaded or created in another transaction.",
        MatchType = MessageMatch.Regex)]
    public void PerformDeletionWithOtherClientTransaction ()
    {
      Order order1;
      using (ClientTransaction.NewTransaction().EnterDiscardingScope())
      {
        order1 = Order.GetObject (DomainObjectIDs.Order1);
      }

      _map.PerformDelete (order1, new RelationEndPointModificationCollection());
    }

    [Test]
    public void CopyFromEmpty ()
    {
      ClientTransactionMock sourceTransaction = new ClientTransactionMock ();
      ClientTransactionMock destinationTransaction = new ClientTransactionMock ();

      RelationEndPointMap sourceMap = sourceTransaction.DataManager.RelationEndPointMap;
      RelationEndPointMap destinationMap = destinationTransaction.DataManager.RelationEndPointMap;

      destinationMap.CopyFrom (sourceMap);

      Assert.AreEqual (0, destinationMap.Count);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Source cannot be the destination RelationEndPointMap instance.",
        MatchType = MessageMatch.Contains)]
    public void CannotCopyFromSelf ()
    {
      _map.CopyFrom (_map);
    }

    [Test]
    public void CopyFromNotEmpty ()
    {
      ClientTransactionMock sourceTransaction = new ClientTransactionMock ();
      ClientTransactionMock destinationTransaction = new ClientTransactionMock ();

      RelationEndPointMap sourceMap = sourceTransaction.DataManager.RelationEndPointMap;
      RelationEndPointMap destinationMap = destinationTransaction.DataManager.RelationEndPointMap;

      Order newOrder;

      using (sourceTransaction.EnterNonDiscardingScope ())
      {
        newOrder = Order.NewObject ();
      }

      RelationEndPointID orderItemsID = new RelationEndPointID (newOrder.ID, typeof (Order).FullName + ".OrderItems");
      RelationEndPointID officialID = new RelationEndPointID (newOrder.ID, typeof (Order).FullName + ".Official");

      Assert.AreNotEqual (0, sourceMap.Count);
      Assert.IsNotNull (sourceMap[orderItemsID]);
      Assert.AreSame (sourceMap, ((CollectionEndPoint) sourceMap[orderItemsID]).ChangeDelegate);

      Assert.AreEqual (0, destinationMap.Count);

      destinationMap.CopyFrom (sourceMap);

      Assert.AreNotEqual (0, destinationMap.Count);
      Assert.AreEqual (sourceMap.Count, destinationMap.Count);

      Assert.IsNotNull (destinationMap[orderItemsID]);
      Assert.AreNotSame (sourceMap[orderItemsID], destinationMap[orderItemsID]);
      
      Assert.AreSame (destinationTransaction, destinationMap[orderItemsID].ClientTransaction);
      Assert.AreSame (destinationMap, ((CollectionEndPoint)destinationMap[orderItemsID]).ChangeDelegate);

      Assert.IsNotNull (destinationMap[officialID]);
      Assert.AreNotSame (sourceMap[officialID], destinationMap[officialID]);

      Assert.AreSame (destinationTransaction, destinationMap[officialID].ClientTransaction);
    }
  }
}