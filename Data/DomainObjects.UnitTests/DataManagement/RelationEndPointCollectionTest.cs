using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
[TestFixture]
public class RelationEndPointCollectionTest : RelationEndPointBaseTest
{
  // types

  // static members and constants

  // member fields

  private RelationEndPoint _orderTicketEndPoint;
  private RelationEndPointCollection _endPoints;

  // construction and disposing

  public RelationEndPointCollectionTest ()
  {
  }

  // methods and properties

  public override void SetUp ()
  {
    base.SetUp ();

    OrderTicket orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
    _orderTicketEndPoint = CreateObjectEndPoint (orderTicket, "Order", DomainObjectIDs.Order1);
    _endPoints = new RelationEndPointCollection ();
  }

  [Test]
  public void Add ()
  {
    _endPoints.Add (_orderTicketEndPoint);
    Assert.AreEqual (1, _endPoints.Count);
  }

  [Test]
  public void EndPointIDIndexer ()
  {
    _endPoints.Add (_orderTicketEndPoint);
    Assert.AreSame (_orderTicketEndPoint, _endPoints[_orderTicketEndPoint.ID]);  
  }

  [Test]
  public void NumericIndexer ()
  {
    _endPoints.Add (_orderTicketEndPoint);
    Assert.AreSame (_orderTicketEndPoint, _endPoints[0]);  
  }

  [Test]
  public void ContainsTrue ()
  {
    _endPoints.Add (_orderTicketEndPoint);
    Assert.IsTrue (_endPoints.Contains (_orderTicketEndPoint.ID));
  }

  [Test]
  public void ContainsFalse ()
  {
    Assert.IsFalse (_endPoints.Contains (_orderTicketEndPoint.ID));
  }

  [Test]
  public void ContainsEndPoint ()
  {
    _endPoints.Add (_orderTicketEndPoint);

    Assert.IsTrue (_endPoints.Contains (_orderTicketEndPoint));
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void ContainsNullEndPoint ()
  {
    _endPoints.Contains ((RelationEndPoint) null);
  }

  [Test]
  public void CopyConstructor ()
  {
    _endPoints.Add (_orderTicketEndPoint);

    RelationEndPointCollection copiedCollection = new RelationEndPointCollection (_endPoints, false);

    Assert.AreEqual (1, copiedCollection.Count);
    Assert.AreSame (_orderTicketEndPoint, copiedCollection[0]);
  }

  [Test]
  public void RemoveByEndPointID ()
  {
    _endPoints.Add (_orderTicketEndPoint);
    Assert.AreEqual (1, _endPoints.Count);

    _endPoints.Remove (_orderTicketEndPoint.ID);
    Assert.AreEqual (0, _endPoints.Count);
  }

  [Test]
  public void RemoveByRelationEndPoint ()
  {
    _endPoints.Add (_orderTicketEndPoint);
    Assert.AreEqual (1, _endPoints.Count);

    _endPoints.Remove (_orderTicketEndPoint);
    Assert.AreEqual (0, _endPoints.Count);
  }

  [Test]
  public void RemoveByIndex ()
  {
    _endPoints.Add (_orderTicketEndPoint);
    Assert.AreEqual (1, _endPoints.Count);

    _endPoints.Remove (0);
    Assert.AreEqual (0, _endPoints.Count);
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void RemoveNullEndPoint ()
  {
    _endPoints.Remove ((RelationEndPoint) null);
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void RemoveNullEndPointID ()
  {
    _endPoints.Remove ((RelationEndPointID) null);
  }

  [Test]
  public void Clear ()
  {
    _endPoints.Add (_orderTicketEndPoint);
    Assert.AreEqual (1, _endPoints.Count);

    _endPoints.Clear ();
    Assert.AreEqual (0, _endPoints.Count);
  }

  [Test]
  public void Clone ()
  {
    _endPoints.Add (_orderTicketEndPoint);

    ICloneable original = (ICloneable) _endPoints;
    RelationEndPointCollection clone = (RelationEndPointCollection) original.Clone ();
    
    Assert.IsNotNull (clone);
    Assert.IsFalse (object.ReferenceEquals (clone, _endPoints));
    Assert.AreEqual (_endPoints.Count, clone.Count);
    Assert.IsFalse (object.ReferenceEquals (_endPoints[0], clone[0]));
    Assert.AreEqual (_endPoints[0].ID, clone[0].ID);
}
}
}
