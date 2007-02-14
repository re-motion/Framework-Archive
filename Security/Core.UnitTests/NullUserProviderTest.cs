using System;
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