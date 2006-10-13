using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Collections;

namespace Rubicon.Core.UnitTests.Collections
{
  [TestFixture]
  public class InterlockedCacheTest
  {
    [Test]
    public void TestCreateAndTryGet ()
    {
      InterlockedCache<string,string> cache = new InterlockedCache<string,string> ();
      string value = cache.GetOrCreateValue ("key", delegate() {
        return "value"; });

      Assert.AreEqual ("value", value);

      bool hasValue = cache.TryGetValue ("key", out value);

      Assert.IsTrue (hasValue);
      Assert.AreEqual ("value", value);
    }

    [Test]
    public void TestCreateTwice()
    {
      InterlockedCache<string,string> cache = new InterlockedCache<string,string> ();
      string value = cache.GetOrCreateValue ("key", delegate() {
        return "value 1"; });

      Assert.AreEqual ("value 1", value);

      value = cache.GetOrCreateValue ("key", delegate() {
        return "value 2"; });

      Assert.AreEqual ("value 1", value);
    }

    [Test]
    public void TestAddAndTryGet()
    {
      InterlockedCache<string,string> cache = new InterlockedCache<string,string> ();
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
      InterlockedCache<string,string> cache = new InterlockedCache<string,string> ();
      cache.Add ("key", "value 1");
      cache.Add ("key", "value 2");
    }
  }
}
