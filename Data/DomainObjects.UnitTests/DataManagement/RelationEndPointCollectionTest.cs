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

  private ObjectEndPoint _objectEndPoint;
  private RelationEndPointList _relationEndPointList;

  // construction and disposing

  public RelationEndPointListTest ()
  {
  }

  // methods and properties

  public override void SetUp ()
  {
    base.SetUp ();

    _objectEndPoint = new ObjectEndPoint (Order.GetObject (DomainObjectIDs.Order1), "OrderTicket");
    _relationEndPointList = new RelationEndPointList ();
  }

  [Test]
  public void Initialize ()
  {
    Assert.AreEqual (0, _relationEndPointList.Count);
  }

  [Test]
  public void Add ()
  {
    _relationEndPointList.Add (_objectEndPoint);

    Assert.AreEqual (1, _relationEndPointList.Count);
    Assert.AreSame (_objectEndPoint, _relationEndPointList[0]);
  }

  [Test]
  public void RemoveAt ()
  {
    _relationEndPointList.Add (_objectEndPoint);
    Assert.AreEqual (1, _relationEndPointList.Count);

    _relationEndPointList.RemoveAt (0);
    Assert.AreEqual (0, _relationEndPointList.Count);
  }

  [Test]
  public void Remove ()
  {
    _relationEndPointList.Add (_objectEndPoint);
    Assert.AreEqual (1, _relationEndPointList.Count);

    _relationEndPointList.Remove (_objectEndPoint);
    Assert.AreEqual (0, _relationEndPointList.Count);
  }

  [Test]
  public void Contains ()
  {
    Assert.IsFalse (_relationEndPointList.Contains (_objectEndPoint));

    _relationEndPointList.Add (_objectEndPoint);
    
    Assert.IsTrue (_relationEndPointList.Contains (_objectEndPoint));
  }

  [Test]
  public void Clear ()
  {
    _relationEndPointList.Add (_objectEndPoint);
    Assert.AreEqual (1, _relationEndPointList.Count);

    _relationEndPointList.Clear ();
    Assert.AreEqual (0, _relationEndPointList.Count);
  }

  [Test]
  public void IndexOf ()
  {
    _relationEndPointList.Add (_objectEndPoint);

    Assert.AreEqual (0, _relationEndPointList.IndexOf (_objectEndPoint));
  }

  [Test]
  public void Enumeration ()
  {
    _relationEndPointList.Add (_objectEndPoint);
    
    foreach (RelationEndPoint relationEndPoint in _relationEndPointList)
      Assert.AreSame (_objectEndPoint, relationEndPoint);
  }
}
}
