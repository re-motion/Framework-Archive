using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
[TestFixture]
public class RelationEndPointMapTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  private RelationEndPointMap _map;
  private Order _newOrder;

  // construction and disposing

  public RelationEndPointMapTest ()
  {
  }

  // methods and properties

  public override void SetUp()
  {
    base.SetUp ();

    _map = ClientTransactionMock.DataManager.RelationEndPointMap;
    _newOrder = new Order ();
  }

  [Test]
  public void DeleteNew ()
  {
    Assert.IsTrue (_map.Count > 0);

    _map.PerformDelete (_newOrder);

    Assert.AreEqual (0, _map.Count);
  }
}
}
