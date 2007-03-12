using System;
using System.Collections.Specialized;
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
      _provider = new NullGlobalAccessTypeCacheProvider();
    }

    [Test]
    public void Initialize()
    {
      NameValueCollection config = new NameValueCollection ();
      config.Add ("description", "The Description");

      NullGlobalAccessTypeCacheProvider provider = new NullGlobalAccessTypeCacheProvider ("Provider", config);

      Assert.AreEqual ("Provider", provider.Name);
      Assert.AreEqual ("The Description", provider.Description);
    }
    
    [Test]
    public void GetAccessTypeCache ()
    {
      Assert.IsInstanceOfType (typeof (NullCache<Tuple<SecurityContext, string>, AccessType[]>), _provider.GetCache());
    }

    [Test]
    public void GetIsNull ()
    {
      Assert.IsTrue (_provider.IsNull);
    }
  }
}