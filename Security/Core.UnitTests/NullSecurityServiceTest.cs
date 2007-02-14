using System;
using NUnit.Framework;

namespace Rubicon.Security.UnitTests
{
  [TestFixture]
  public class NullSecurityServiceTest
  {
    private ISecurityService _securityService;

    [SetUp]
    public void SetUp ()
    {
      _securityService = new NullSecurityService();
    }

    [Test]
    public void Test_GetAccess_ReturnsEmptyList ()
    {
      AccessType[] accessTypes = _securityService.GetAccess (null, null);
      Assert.IsNotNull (accessTypes);
      Assert.AreEqual (0, accessTypes.Length);
    }

    [Test]
    public void Test_GetRevision_ReturnsZero ()
    {
      Assert.AreEqual (0, _securityService.GetRevision());
    }
  }
}