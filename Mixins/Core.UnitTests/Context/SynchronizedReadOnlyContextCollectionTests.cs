using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using Remotion.Development.UnitTesting;
using Remotion.Mixins.Context;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.Mixins.UnitTests.Context
{
  [TestFixture]
  public class SynchronizedReadOnlyContextCollectionTests
  {
    private SynchronizedReadOnlyContextCollection<string, int> _collection;

    [SetUp]
    public void SetUp ()
    {
      _collection = new SynchronizedReadOnlyContextCollection<string, int> (delegate (int i) { return i.ToString(); }, new int[] { 1, 2, 3 });
    }

    [Test]
    public void NewCollection ()
    {
      Assert.AreEqual (3, _collection.Count);
      ReadOnlyContextCollection<string, int> inner = (ReadOnlyContextCollection<string, int>) PrivateInvoke.GetNonPublicField (_collection, "_internalCollection");
      Assert.That (inner, Is.EqualTo (new int[] { 1, 2, 3 }));
    }

    [Test]
    public void MembersDelegated ()
    {
      MockRepository repository = new MockRepository();
      ReadOnlyContextCollection<string, int> innerMock = repository.CreateMock<ReadOnlyContextCollection<string, int>> (
          (Func<int, string>) delegate { return ""; }, new int[0]);
      PrivateInvoke.SetNonPublicField (_collection, "_internalCollection", innerMock);

      IEnumerator<int> enumerator = new List<int>().GetEnumerator();
      int[] array = new int[0];

      using (repository.Ordered ())
      {
        Expect.Call (innerMock.Count).Return (1);
        Expect.Call (innerMock.Contains (7)).Return (true);
        Expect.Call (innerMock.ContainsKey ("8")).Return (false);
        Expect.Call (innerMock["8"]).Return (1);
        Expect.Call (innerMock.GetEnumerator ()).Return (enumerator);
        innerMock.CopyTo(array, 13);
      }

      repository.ReplayAll();

      Assert.AreEqual (1, _collection.Count);
      Assert.AreEqual (true, _collection.Contains (7));
      Assert.AreEqual (false, _collection.ContainsKey ("8"));
      Assert.AreEqual (1, _collection["8"]);
      Assert.AreEqual (enumerator, _collection.GetEnumerator ());
      _collection.CopyTo (array, 13);

      repository.VerifyAll();
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
      Assert.IsTrue (((ICollection) _collection).IsSynchronized);
    }

    [Test]
    public void SyncRoot ()
    {
      Assert.IsNotNull (((ICollection) _collection).SyncRoot);
    }
  }
}