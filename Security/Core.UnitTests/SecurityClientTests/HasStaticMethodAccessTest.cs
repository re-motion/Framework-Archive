using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using NUnit.Framework;

using Rubicon.Security.UnitTests.SampleDomain;
using Rubicon.Security.Metadata;
using Rubicon.Utilities;
using Rubicon.Security.UnitTests.MockConstraints;

namespace Rubicon.Security.UnitTests.SecurityClientTests
{
  [TestFixture]
  public class HasStaticMethodAccessTest
  {
    private SecurityClientTestHelper _testHelper;
    private SecurityClient _securityClient;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = SecurityClientTestHelper.CreateForStatelessSecurity ();
      _securityClient = _testHelper.CreateSecurityClient ();
    }

    [Test]
    public void Test_AccessGranted ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredStaticMethodPermissions ("StaticMethod", TestAccessType.First);
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess (TestAccessType.First, true);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasStaticMethodAccess (typeof (SecurableObject), "StaticMethod");

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void Test_AccessDenied ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredStaticMethodPermissions ("StaticMethod", TestAccessType.First);
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess (TestAccessType.First, false);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasStaticMethodAccess (typeof (SecurableObject), "StaticMethod");

      _testHelper.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void Test_AccessGranted_WithinSecurityFreeSection ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredStaticMethodPermissions ("StaticMethod", TestAccessType.First);
      _testHelper.ReplayAll ();

      bool hasAccess;
      using (new SecurityFreeSection ())
      {
        hasAccess = _securityClient.HasStaticMethodAccess (typeof (SecurableObject), "StaticMethod");
      }

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        "The member 'StaticMethod' does not define required permissions.\r\nParameter name: requiredAccessTypeEnums")]
    public void Test_WithoutRequiredPermissions_ShouldThrowArgumentException ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredStaticMethodPermissions ("StaticMethod");
      _testHelper.ReplayAll ();

      _securityClient.HasStaticMethodAccess (typeof (SecurableObject), "StaticMethod");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        "The member 'StaticMethod' does not define required permissions.\r\nParameter name: requiredAccessTypeEnums")]
    public void Test_WithoutRequiredPermissionsAndWithinSecurityFreeSection_ShouldThrowArgumentException ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredStaticMethodPermissions ("StaticMethod");
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _securityClient.HasStaticMethodAccess (typeof (SecurableObject), "StaticMethod");
      }
    }
  }
}
