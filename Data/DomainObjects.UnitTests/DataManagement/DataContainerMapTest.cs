using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
[TestFixture]
public class DataContainerMapTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  private DataContainerMap _map;
  private DataContainer _order;

  // construction and disposing

  public DataContainerMapTest ()
  {
  }

  // methods and properties

  public override void SetUp ()
  {
    base.SetUp ();

    _map = new DataContainerMap (ClientTransaction.Current);
    _order = TestDataContainerFactory.CreateNewOrderDataContainer ();
  }

  [Test]
  public void DeleteNewDataContainer ()
  {
    _map.Register (_order);
    Assert.AreEqual (1, _map.Count);

    _map.PerformDelete (_order);
    Assert.AreEqual (0, _map.Count);
  }
}
}
