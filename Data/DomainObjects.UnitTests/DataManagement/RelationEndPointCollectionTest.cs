using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Relations;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Relations
{
[TestFixture]
public class RelationEndPointListTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  private RelationEndPoint _endPoint;
  private RelationEndPointList _endPointList;

  // construction and disposing

  public RelationEndPointListTest ()
  {
  }

  // methods and properties

  public override void SetUp ()
  {
    base.SetUp ();

    _endPoint = new RelationEndPoint (Order.GetObject (DomainObjectIDs.Order1), "OrderTicket");
    _endPointList = new RelationEndPointList ();
  }

  [Test]
  public void Initialize ()
  {
    Assert.AreEqual (0, _endPointList.Count);
  }

  [Test]
  public void Add ()
  {
    _endPointList.Add (_endPoint);

    Assert.AreEqual (1, _endPointList.Count);
    Assert.AreSame (_endPoint, _endPointList[0]);
  }

  [Test]
  public void RemoveAt ()
  {
    _endPointList.Add (_endPoint);
    Assert.AreEqual (1, _endPointList.Count);

    _endPointList.RemoveAt (0);
    Assert.AreEqual (0, _endPointList.Count);
  }

  [Test]
  public void Remove ()
  {
    _endPointList.Add (_endPoint);
    Assert.AreEqual (1, _endPointList.Count);

    _endPointList.Remove (_endPoint);
    Assert.AreEqual (0, _endPointList.Count);
  }

  [Test]
  public void Contains ()
  {
    Assert.IsFalse (_endPointList.Contains (_endPoint));

    _endPointList.Add (_endPoint);
    
    Assert.IsTrue (_endPointList.Contains (_endPoint));
  }

  [Test]
  public void Clear ()
  {
    _endPointList.Add (_endPoint);
    Assert.AreEqual (1, _endPointList.Count);

    _endPointList.Clear ();
    Assert.AreEqual (0, _endPointList.Count);
  }

  [Test]
  public void IndexOf ()
  {
    _endPointList.Add (_endPoint);

    Assert.AreEqual (0, _endPointList.IndexOf (_endPoint));
  }

  [Test]
  public void Enumeration ()
  {
    _endPointList.Add (_endPoint);
    
    foreach (RelationEndPoint relationEndPoint in _endPointList)
      Assert.AreSame (_endPoint, relationEndPoint);
  }
}
}
