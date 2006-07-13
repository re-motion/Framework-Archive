using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace Rubicon.Security.UnitTests
{
  [TestFixture]
  public class NullCacheTest
  {
    [Test]
    public void Get ()
    {
      IAccessTypeCache cache = new NullAccessTypeCache ();

      Assert.IsNull (cache.Get ("anyKey"));
    }

    [Test]
    public void Add ()
    {
      IAccessTypeCache cache = new NullAccessTypeCache ();

      cache.Add ("anyKey", new AccessType[0]);
      // Succeed
    }
  }
}