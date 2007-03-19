using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Collections;

namespace Rubicon.Core.UnitTests.Collections
{
  [TestFixture]
  [Obsolete ("LazyInterlockedCache is marked obsolete because it is still in prototype state.")]
  public class InterlockedCacheTest
  {
    [Test]
    public void TestCreateAndTryGet ()
    {
      LazyInterlockedCache<string,string> cache = new LazyInterlockedCache<string,string> ();
      string value = cache.GetOrCreateValue ("key", delegate(string key) {
        return "value"; });

      Assert.AreEqual ("value", value);

      bool hasValue = cache.TryGetValue ("key", out value);

      Assert.IsTrue (hasValue);
      Assert.AreEqual ("value", value);
    }

    [Test]
    public void TestCreateTwice()
    {
      LazyInterlockedCache<string,string> cache = new LazyInterlockedCache<string,string> ();
      string value = cache.GetOrCreateValue ("key", delegate(string key) {
        return "value 1"; });

      Assert.AreEqual ("value 1", value);

      value = cache.GetOrCreateValue ("key", delegate(string key) {
        return "value 2"; });

      Assert.AreEqual ("value 1", value);
    }

    [Test]
    public void TestAddAndTryGet()
    {
      LazyInterlockedCache<string,string> cache = new LazyInterlockedCache<string,string> ();
      cache.Add ("key", "value");

      string value;
      bool hasValue = cache.TryGetValue ("key", out value);

      Assert.IsTrue (hasValue);
      Assert.AreEqual ("value", value);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    public void TestAddTwice()
    {
      LazyInterlockedCache<string,string> cache = new LazyInterlockedCache<string,string> ();
      cache.Add ("key", "value 1");
      cache.Add ("key", "value 2");
    }
  }
}
