using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
[TestFixture]
public class DataContainerMapTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  private DataContainerMap _map;
  private DataContainer _newOrder;
  private DataContainer _existingOrder;

  // construction and disposing

  public DataContainerMapTest ()
  {
  }

  // methods and properties

  public override void SetUp ()
  {
    base.SetUp ();

    _map = new DataContainerMap (ClientTransactionMock);
    _newOrder = TestDataContainerFactory.CreateNewOrderDataContainer ();
    _existingOrder = TestDataContainerFactory.CreateOrder1DataContainer ();
  }

  [Test]
  public void DeleteNewDataContainer ()
  {
    _map.Register (_newOrder);
    Assert.AreEqual (1, _map.Count);

    _map.PerformDelete (_newOrder);
    Assert.AreEqual (0, _map.Count);
  }

  [Test]
  public void RemoveDeletedDataContainerInCommit ()
  {
    _map.Register (_existingOrder);
    Assert.AreEqual (1, _map.Count);

    Order order = (Order) _existingOrder.DomainObject;
    order.Delete ();
    _map.Commit ();

    Assert.AreEqual (0, _map.Count);
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void AccessDeletedDataContainerAfterCommit ()
  {
    _map.Register (_existingOrder);
    Assert.AreEqual (1, _map.Count);

    Order order = (Order) _existingOrder.DomainObject;
    order.Delete ();
    _map.Commit ();

    ObjectID id = _existingOrder.ID;
  }

  [Test]
  [ExpectedException (typeof (ArgumentException), "Invalid state '1000' provided.\r\nParameter name: state")]
  public void GetByInvalidState ()
  {
    _map.GetByState ((StateType) 1000);
  }

  [Test]
  public void RollbackForDeletedObject ()
  {
    _map.Register (_existingOrder);

    Order order = (Order) _existingOrder.DomainObject;
    order.Delete ();
    Assert.AreEqual (StateType.Deleted, _existingOrder.State);

    _map.Rollback ();

    _existingOrder = _map[_existingOrder.ID];
    Assert.IsNotNull (_existingOrder);
    Assert.AreEqual (StateType.Unchanged, _existingOrder.State);
  }

  [Test]
  [ExpectedException (typeof (ObjectDiscardedException))]
  public void RollbackForNewObject ()
  {
    _map.Register (_newOrder);

    _map.Rollback ();

    ObjectID id = _newOrder.ID;
  }
}
}
