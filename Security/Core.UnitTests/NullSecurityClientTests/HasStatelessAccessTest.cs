using System;
using NUnit.Framework;
using Rubicon.Security.UnitTests.SampleDomain;

namespace Rubicon.Security.UnitTests.NullSecurityClientTests
{
  [TestFixture]
  public class HasStatelessAccessTest
  {
    private NullSecurityClientTestHelper _testHelper;
    private SecurityClient _securityClient;

    [SetUp]
    public void SetUp()
    {
      _testHelper = NullSecurityClientTestHelper.CreateForStatelessSecurity();
      _securityClient = _testHelper.CreateSecurityClient();
    }

    [Test]
    public void Test_AccessGranted()
    {
      _testHelper.ReplayAll();

      bool hasAccess = _securityClient.HasStatelessAccess (typeof (SecurableObject), AccessType.Get (TestAccessTypes.First));

      _testHelper.VerifyAll();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted()
    {
      _testHelper.ReplayAll();

      bool hasAccess;
      using (new SecurityFreeSection())
      {
        hasAccess = _securityClient.HasStatelessAccess (typeof (SecurableObject), AccessType.Get (TestAccessTypes.First));
      }

      _testHelper.VerifyAll();
      Assert.IsTrue (hasAccess);
    }
  }
}