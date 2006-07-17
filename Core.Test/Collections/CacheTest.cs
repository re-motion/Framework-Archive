using System;
using System.Collections.Generic;
using System.Text;

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
      Assert.IsFalse (_cache.TryGet ("key1", out actual));
      Assert.IsNull (actual);
    }

    [Test]
    public void Add_TryGet ()
    {
      object expected = new object();

      _cache.Add ("key1", expected);
      object actual;
      Assert.IsTrue (_cache.TryGet ("key1", out actual));
      Assert.AreSame (expected, actual);
    }

    [Test]
    public void Add_TryGet_Clear_TryGet ()
    {
      object expected = new object ();

      _cache.Add ("key1", expected);
      object actual;
      Assert.IsTrue (_cache.TryGet ("key1", out actual));
      Assert.AreSame (expected, actual);
      _cache.Clear ();
      Assert.IsFalse (_cache.TryGet ("key1", out actual));
      Assert.IsNull (actual);
    }
  }
}