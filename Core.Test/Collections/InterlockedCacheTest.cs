using System;
using NUnit.Framework;
using Rubicon.Collections;

namespace Rubicon.Core.UnitTests.Collections
{
  [TestFixture]
  public class InterlockedCacheTest
  {
    private ICache<string, string> _cache;

    [SetUp]
    public void SetUp ()
    {
      _cache = new InterlockedCache<string, string> ();
    }

    [Test]
    public void CreateAndTryGet()
    {
      string value = _cache.GetOrCreateValue ("key", delegate () { return "value"; });

      Assert.AreEqual ("value", value);

      bool hasValue = _cache.TryGetValue ("key", out value);

      Assert.IsTrue (hasValue);
      Assert.AreEqual ("value", value);
    }

    [Test]
    public void CreateTwice()
    {
      string value = _cache.GetOrCreateValue ("key", delegate() { return "value 1"; });

      Assert.AreEqual ("value 1", value);

      value = _cache.GetOrCreateValue ("key", delegate() { return "value 2"; });

      Assert.AreEqual ("value 1", value);
    }

    [Test]
    public void AddAndTryGet()
    {
      _cache.Add ("key", "value");

      string value;
      bool hasValue = _cache.TryGetValue ("key", out value);

      Assert.IsTrue (hasValue);
      Assert.AreEqual ("value", value);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    public void AddTwice()
    {
      _cache.Add ("key", "value 1");
      _cache.Add ("key", "value 2");
    }

    [Test]
    public void GetIsNull()
    {
      Assert.IsFalse (_cache.IsNull);
    }
  }
}