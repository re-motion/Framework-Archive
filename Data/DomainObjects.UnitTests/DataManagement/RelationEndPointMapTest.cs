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

  // construction and disposing

  public RelationEndPointMapTest ()
  {
  }

  // methods and properties

  public override void SetUp()
  {
    base.SetUp ();

    _map = ClientTransactionMock.DataManager.RelationEndPointMap;
  }

  [Test]
  public void DeleteNew ()
  {
    Order newOrder = new Order ();
    Assert.IsTrue (_map.Count > 0);

    _map.PerformDelete (newOrder);
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
}
}
