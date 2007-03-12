using System;
using System.Collections.Specialized;
using NUnit.Framework;

namespace Rubicon.Security.UnitTests
{
  [TestFixture]
  public class NullUserProviderTest
  {
    private IUserProvider _provider;

    [SetUp]
    public void SetUp()
    {
      _provider = new NullUserProvider();
    }

    [Test]
    public void Initialize ()
    {
      NameValueCollection config = new NameValueCollection ();
      config.Add ("description", "The Description");

      NullUserProvider provider = new NullUserProvider ("Provider", config);

      Assert.AreEqual ("Provider", provider.Name);
      Assert.AreEqual ("The Description", provider.Description);
    }
    
    [Test]
    public void GetUser()
    {
      Assert.IsInstanceOfType (typeof (NullPrincipal), _provider.GetUser());
    }

    [Test]
    public void GetIsNull()
    {
      Assert.IsTrue (_provider.IsNull);
    }
  }
}