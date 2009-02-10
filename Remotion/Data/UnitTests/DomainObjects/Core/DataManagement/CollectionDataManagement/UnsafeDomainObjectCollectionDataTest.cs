// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionDataManagement
{
  [TestFixture]
  public class UnsafeDomainObjectCollectionDataTest : ClientTransactionBaseTest
  {
    private IDomainObjectCollectionData _data;

    private Order _order1;
    private Order _order2;
    private Order _order3;
    private Order _order4;
    private Type _unsafeType;

    public override void SetUp ()
    {
      base.SetUp ();
      
      _unsafeType = typeof (DomainObjectCollectionData).Assembly.GetType (
          typeof (DomainObjectCollectionData).Namespace + ".UnsafeDomainObjectCollectionData", 
          true);
      _data = (IDomainObjectCollectionData) Activator.CreateInstance (_unsafeType, true);

      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _order2 = Order.GetObject (DomainObjectIDs.Order2);
      _order3 = Order.GetObject (DomainObjectIDs.Order3);
      _order4 = Order.GetObject (DomainObjectIDs.Order4);
    }

    [Test]
    public void IsReadOnly ()
    {
      Assert.That (_data.IsReadOnly, Is.False);
    }

    [Test]
    public void Insert ()
    {
      Add (_order1);
      Add (_order2);
      Add (_order3);

      _data.Insert (2, _order4);

      Assert.That (_data.ToArray (), Is.EqualTo (new[] { _order1, _order2, _order4, _order3 }));
    }

    [Test]
    public void Insert_ChangesVersion ()
    {
      Add (_order1);
      Add (_order2);
      Add (_order3);

      Assert_VersionChanged (() => _data.Insert (2, _order4));
    }

    [Test]
    public void ContainsObjectID_False ()
    {
      Assert.That (_data.ContainsObjectID (DomainObjectIDs.Order1), Is.False);
    }

    [Test]
    public void ContainsObjectID_True ()
    {
      Assert.That (_data.ContainsObjectID (DomainObjectIDs.Order1), Is.False);
      Add (_order1);
      Assert.That (_data.ContainsObjectID (DomainObjectIDs.Order1), Is.True);
    }

    [Test]
    public void GetObject_ByID ()
    {
      Add (_order1);
      Add (_order2);

      Assert.That (_data.GetObject (_order1.ID), Is.SameAs (_order1));
    }

    [Test]
    public void GetObject_InvalidID ()
    {
      Assert.That (_data.GetObject (_order1.ID), Is.Null);
    }

    [Test]
    public void GetObject_ByIndex ()
    {
      Add (_order1);
      Add (_order2);

      Assert.That (_data.GetObject (0), Is.SameAs (_order1));
      Assert.That (_data.GetObject (1), Is.SameAs (_order2));
    }

    [Test]
    public void IndexOf ()
    {
      Add (_order1);
      Add (_order2);
      Add (_order3);

      Assert.That (_data.IndexOf (_order1.ID), Is.EqualTo (0));
      Assert.That (_data.IndexOf (_order2.ID), Is.EqualTo (1));
      Assert.That (_data.IndexOf (_order3.ID), Is.EqualTo (2));
      Assert.That (_data.IndexOf (_order4.ID), Is.EqualTo (-1));
    }

    [Test]
    public void Clear ()
    {
      Add (_order1);
      Add (_order2);

      _data.Clear ();

      Assert.That (_data.Count, Is.EqualTo (0));
      Assert.That (_data.ToArray(), Is.Empty);
    }

    [Test]
    public void Clear_AlsoClearsObjectsByID ()
    {
      Add (_order1);
      Add (_order2);

      _data.Clear ();

      Assert.That (_data.ContainsObjectID (_order1.ID), Is.False);
    }

    [Test]
    public void Clear_ChangesVersion ()
    {
      Add (_order1);
      Add (_order2);
      Add (_order3);

      Assert_VersionChanged (() => _data.Clear());
    }

    [Test]
    public void Remove ()
    {
      Add (_order1);
      Add (_order2);
      Add (_order3);

      _data.Remove (_order2.ID);
      Assert.That (_data.ToArray(), Is.EqualTo (new[] { _order1, _order3 }));
    }

    [Test]
    public void Remove_ChangesVersion ()
    {
      Add (_order1);
      Add (_order2);
      Add (_order3);

      Assert_VersionChanged (() => _data.Remove (_order3.ID));
    }

    [Test]
    public void Remove_NonExistingElement ()
    {
      _data.Remove (_order2.ID);
      Assert.That (_data.ToArray (), Is.Empty);
    }

    [Test]
    public void Remove_NonExistingElement_DoesntChangeVersion ()
    {
      Assert_VersionSame (() => _data.Remove (_order2.ID));
    }

    [Test]
    public void Replace ()
    {
      Add (_order1);
      Add (_order2);
      Add (_order3);

      _data.Replace (_order2.ID, _order4);

      Assert.That (_data.ToArray(), Is.EqualTo (new[] { _order1, _order4, _order3 }));
    }

    [Test]
    public void Replace_ChangesVersion ()
    {
      Add (_order1);
      Add (_order2);
      Add (_order3);

      Assert_VersionChanged (() => _data.Replace (_order2.ID, _order4));
    }

    [Test]
    public void Replace_WithSelf ()
    {
      Add (_order1);
      Add (_order2);
      Add (_order3);

      _data.Replace (_order2.ID, _order2);

      Assert.That (_data.ToArray (), Is.EqualTo (new[] { _order1, _order2, _order3 }));
    }

    [Test]
    public void Replace_WithSelf_ChangesVersion ()
    {
      Add (_order1);
      Add (_order2);
      Add (_order3);

      Assert_VersionSame (() => _data.Replace (_order2.ID, _order2));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Collection was modified during enumeration.")]
    public void Enumeration_ChokesOnVersionChanges ()
    {
      Add (_order1);
      Add (_order2);
      Add (_order3);

      var enumerator = _data.GetEnumerator ();
      enumerator.MoveNext();

      Add (_order4);

      enumerator.MoveNext ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Collection was modified during enumeration.")]
    public void Enumeration_ChokesOnVersionChanges_Remove ()
    {
      Add (_order1);
      Add (_order2);

      foreach (var x in _data)
        _data.Remove (x.ID);
    }

    [Test]
    public void Serializable ()
    {
      Add (_order1);
      Add (_order2);
      Add (_order3);

      var result = Serializer.SerializeAndDeserialize (_data);
      Assert.That (result.Count, Is.EqualTo (3));
    }

    private void Add (Order order)
    {
      _data.Insert (_data.Count, order);
    }

    private void Assert_VersionChanged (Action action)
    {
      var property = _unsafeType.GetProperty ("Version");
      var version = (long) property.GetValue (_data, null);
      action ();
      Assert.That ((long) property.GetValue (_data, null), Is.GreaterThan (version));
    }

    private void Assert_VersionSame (Action action)
    {
      var property = _unsafeType.GetProperty ("Version");
      var version = (long) property.GetValue (_data, null);
      action ();
      Assert.That ((long) property.GetValue (_data, null), Is.EqualTo (version));
    }
  }
}