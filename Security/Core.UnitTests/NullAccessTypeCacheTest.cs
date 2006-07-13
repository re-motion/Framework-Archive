using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace Rubicon.Security.UnitTests
{
  [TestFixture]
  public class NullAccessTypeCacheTest
  {
    private IAccessTypeCache<string> _cache;

    [SetUp]
    public void SetUp ()
    {
      _cache = new NullAccessTypeCache<string> ();
    }

    [Test]
    public void Get ()
    {
      Assert.IsNull (_cache.Get ("anyKey"));
    }

    [Test]
    public void Add ()
    {
      _cache.Add ("anyKey", new AccessType[0]);
      // Succeed
    }

    [Test]
    public void Clear ()
    {
      _cache.Clear ();
      // Succeed
    }

  }
}