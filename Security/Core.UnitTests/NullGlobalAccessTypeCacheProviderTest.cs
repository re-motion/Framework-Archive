using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace Rubicon.Security.UnitTests
{
  [TestFixture]
  public class NullGlobalAccessTypeCacheProviderTest
  {
    private IGlobalAccessTypeCacheProvider _provider;

    [SetUp]
    public void SetUp ()
    {
      _provider = new NullGlobalAccessTypeCacheProvider ();
    }

    [Test]
    public void GetAccessTypeCache ()
    {
      Assert.IsInstanceOfType (typeof (NullAccessTypeCache<SecurityContext>), _provider.GetAccessTypeCache ());
    }
  }
}