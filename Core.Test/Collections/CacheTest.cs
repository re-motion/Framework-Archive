using System;
using NUnit.Framework;
using Rubicon.Collections;

namespace Rubicon.Core.UnitTests.Collections
{
  [TestFixture]
  public class CacheTest
  {
    private ICache<string, object> _cache;

    [SetUp]
    public void SetUp ()
    {
      _cache = new Cache<string, object> ();
    }

    [Test]
    public void TryGet_WithResultNotInCache ()
    {
      object actual;
      Assert.IsFalse (_cache.TryGetValue ("key1", out actual));
      Assert.IsNull (actual);
    }

    [Test]
    public void GetOrCreateValue ()
    {
      object expected = new object ();
      Assert.AreSame (expected, _cache.GetOrCreateValue ("key1", delegate (string key) { return expected; }));
    }

    [Test]
    public void GetOrCreateValue_TryGetValue ()
    {
      object expected = new object ();

      _cache.GetOrCreateValue ("key1", delegate (string key) { return expected; });
      object actual;
      Assert.IsTrue (_cache.TryGetValue ("key1", out actual));
      Assert.AreSame (expected, actual);
    }

    [Test]
    public void Add_GetOrCreateValue ()
    {
      object expected = new object ();

      _cache.Add ("key1", expected);
      Assert.AreSame (expected, _cache.GetOrCreateValue ("key1", delegate (string key) { throw new InvalidOperationException ("The valueFactory must not be invoked."); }));
    }

    [Test]
    public void Add_TryGetValue ()
    {
      object expected = new object();

      _cache.Add ("key1", expected);
      object actual;
      Assert.IsTrue (_cache.TryGetValue ("key1", out actual));
      Assert.AreSame (expected, actual);
    }

    [Test]
    public void Add_TryGetValue_Clear_TryGetValue ()
    {
      object expected = new object ();

      _cache.Add ("key1", expected);
      object actual;
      Assert.IsTrue (_cache.TryGetValue ("key1", out actual));
      Assert.AreSame (expected, actual);
      _cache.Clear ();
      Assert.IsFalse (_cache.TryGetValue ("key1", out actual));
      Assert.IsNull (actual);
    }

    [Test]
    public void Add_Null ()
    {
      _cache.Add ("key1", null);
      object actual;
      Assert.IsTrue (_cache.TryGetValue ("key1", out actual));
      Assert.IsNull (actual);
    }

    [Test]
    public void GetIsNull()
    {
      Assert.IsFalse (_cache.IsNull);
    }
  }
}