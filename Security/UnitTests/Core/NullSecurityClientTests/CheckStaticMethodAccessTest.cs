using System;
using NUnit.Framework;
using Rubicon.Security.UnitTests.Core.SampleDomain;

namespace Rubicon.Security.UnitTests.Core.NullSecurityClientTests
{
  [TestFixture]
  public class CheckStaticMethodAccessTest
  {
    private NullSecurityClientTestHelper _testHelper;
    private SecurityClient _securityClient;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = NullSecurityClientTestHelper.CreateForStatelessSecurity ();
      _securityClient = _testHelper.CreateSecurityClient ();
    }

    [Test]
    public void Test_AccessGranted ()
    {
      _testHelper.ReplayAll ();

      _securityClient.CheckStaticMethodAccess (typeof (SecurableObject), "StaticMethod");

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted ()
    {
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _securityClient.CheckStaticMethodAccess (typeof (SecurableObject), "StaticMethod");
      }

      _testHelper.VerifyAll ();
    }
  }
}
