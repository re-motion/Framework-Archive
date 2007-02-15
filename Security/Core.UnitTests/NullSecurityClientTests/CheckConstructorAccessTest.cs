using System;
using NUnit.Framework;
using Rubicon.Security.UnitTests.SampleDomain;

namespace Rubicon.Security.UnitTests.NullSecurityClientTests
{
  [TestFixture]
  public class CheckConstructorAccessTest
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

      _securityClient.CheckConstructorAccess (typeof (SecurableObject));

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted ()
    {
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _securityClient.CheckConstructorAccess (typeof (SecurableObject));
      }

      _testHelper.VerifyAll ();
    }
  }
}
