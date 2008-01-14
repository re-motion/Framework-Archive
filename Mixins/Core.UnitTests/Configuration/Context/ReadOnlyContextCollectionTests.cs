using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rubicon.Mixins.Context;
using NUnit.Framework.SyntaxHelpers;
using System.Collections;
using Rubicon.Utilities;

namespace Rubicon.Mixins.UnitTests.Configuration.Context
{
  [TestFixture]
  public class ReadOnlyContextCollectionTests
  {
    private ReadOnlyContextCollection<string, int> _collection;

    [SetUp]
    public void SetUp ()
    {
      _collection = new ReadOnlyContextCollection<string, int> (
          delegate (int i)
          {
            return i.ToString ();
          }, new int[] { 1, 2, 3 });
    }

    [Test]
    public void NewCollection ()
    {
      Assert.AreEqual (3, _collection.Count);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: values[0]")]
    public void NewCollection_NullValue ()
    {
      new ReadOnlyContextCollection<string, string> ( delegate { return ""; }, new string[] { null });
    }

    [Test]
    public void Contains_Key ()
    {
      Assert.IsTrue (_collection.Contains ("1"));
      Assert.IsTrue (_collection.Contains ("2"));
      Assert.IsTrue (_collection.Contains ("3"));
      Assert.IsFalse (_collection.Contains ("4"));
      Assert.IsFalse (_collection.Contains ("�"));
    }

    [Test]
    public void Contains_Value ()
    {
      ReadOnlyContextCollection<string, int> collection = new ReadOnlyContextCollection<string, int> (
          delegate (int i)
          {
            if (i > 2)
              return ">2";
            else
              return i.ToString ();
          }, new int[] { 1, 2, 3 });

      Assert.IsTrue (collection.Contains (1));
      Assert.IsTrue (collection.Contains (2));
      Assert.IsTrue (collection.Contains (3));
      Assert.IsFalse (collection.Contains (4));
    }

    [Test]
    public void GetEnumerator ()
    {
      List<int> values = new List<int> (_collection);
      Assert.That (values, Is.EqualTo (new int[] {1, 2, 3}));
    }

    [Test]
    public void GetEnumerator_NonGeneric ()
    {
      IEnumerable collectionAsEnumerable = _collection;
      List<object> values = new List<object> (EnumerableUtility.Cast<object> (collectionAsEnumerable));
      Assert.That (values, Is.EqualTo (new int[] { 1, 2, 3 }));
    }

    [Test]
    public void CopyTo ()
    {
      int[] values = new int[5];
      _collection.CopyTo (values, 1);
      Assert.That (values, Is.EqualTo (new int[] {0, 1, 2, 3, 0}));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Add ()
    {
      ((ICollection<int>) _collection).Add (0);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Clear ()
    {
      ((ICollection<int>) _collection).Clear ();
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Remove ()
    {
      ((ICollection<int>) _collection).Remove (1);
    }

    [Test]
    public void IsReadOnly ()
    {
      Assert.IsTrue (((ICollection<int>) _collection).IsReadOnly);
    }

    [Test]
    public void CopyTo_NonGeneric ()
    {
      object[] values = new object[5];
      ((ICollection) _collection).CopyTo (values, 1);
      Assert.That (values, Is.EqualTo (new object[] { null, 1, 2, 3, null }));
    }

    [Test]
    public void IsSynchronized ()
    {
      Assert.IsFalse (((ICollection) _collection).IsSynchronized);
    }

    [Test]
    public void SyncRoot ()
    {
      Assert.IsNotNull (((ICollection) _collection).SyncRoot);
    }
  }
}