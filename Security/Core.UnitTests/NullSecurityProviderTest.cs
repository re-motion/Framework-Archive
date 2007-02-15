using System;
using NUnit.Framework;

namespace Rubicon.Security.UnitTests
{
  [TestFixture]
  public class NullSecurityProviderTest
  {
    private ISecurityProvider _securityProvider;

    [SetUp]
    public void SetUp ()
    {
      _securityProvider = new NullSecurityProvider();
    }

    [Test]
    public void GetAccess_ReturnsEmptyList ()
    {
      AccessType[] accessTypes = _securityProvider.GetAccess (null, null);
      Assert.IsNotNull (accessTypes);
      Assert.AreEqual (0, accessTypes.Length);
    }

    [Test]
    public void GetRevision_ReturnsZero ()
    {
      Assert.AreEqual (0, _securityProvider.GetRevision());
    }

    [Test]
    public void GetIsNull ()
    {
      Assert.IsTrue (_securityProvider.IsNull);
    }
  }
}