using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Rubicon.Collections;

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
      Assert.IsInstanceOfType (typeof (NullCache<Tupel<SecurityContext, string>, AccessType[]>), _provider.GetCache ());
    }
  }
}