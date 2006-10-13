using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Rubicon.Collections;

namespace Rubicon.Core.UnitTests.Collections
{
  [TestFixture]
  public class NullCacheTest
  {
    private ICache<string, object> _cache;

    [SetUp]
    public void SetUp ()
    {
      _cache = new NullCache<string, object> ();
    }

    [Test]
    public void TryGet ()
    {
      object actual;
      Assert.IsFalse (_cache.TryGetValue ("anyKey", out actual));
    }

    [Test]
    public void Add_TryGet ()
    {
      _cache.Add ("key1", new object());
      object actual;
      Assert.IsFalse (_cache.TryGetValue ("key1", out actual));
      Assert.IsNull (actual);
    }

    [Test]
    public void Clear ()
    {
      _cache.Clear ();
      // Succeed
    }

  }
}