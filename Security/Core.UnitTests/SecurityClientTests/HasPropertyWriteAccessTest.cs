using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;
using NUnit.Framework;
using Rhino.Mocks;

using Rubicon.Security.Metadata;
using Rubicon.Security.UnitTests.SampleDomain;

namespace Rubicon.Security.UnitTests.SecurityClientTests
{
  [TestFixture]
  public class HasPropertyWriteAccessTest
  {
    private SecurityClientTestHelper _testHelper;
    private SecurityClient _securityClient;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = SecurityClientTestHelper.CreateForStatefulSecurity ();
      _securityClient = _testHelper.CreateSecurityClient ();
    }

    [Test]
    public void Test_AccessGranted ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty", TestAccessType.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (TestAccessType.First, true);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");

      Assert.IsTrue (hasAccess);
      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_AccessDenied ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty", TestAccessType.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (TestAccessType.First, false);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");

      _testHelper.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void Test_AccessGranted_WithinSecurityFreeSection ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty", TestAccessType.First);
      _testHelper.ReplayAll ();

      bool hasAccess;
      using (new SecurityFreeSection ())
      {
        hasAccess = _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");
      }

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void Test_AccessGranted_WithDefaultAccessType ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty");
      _testHelper.ExpectObjectSecurityStrategyHasAccess (GeneralAccessType.Edit, true);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void Test_AccessDenied_WithDefaultAccessType ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty");
      _testHelper.ExpectObjectSecurityStrategyHasAccess (GeneralAccessType.Edit, false);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");

      _testHelper.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void Test_AccessGranted_WithDefaultAccessTypeAndWithinSecurityFreeSection ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty");
      _testHelper.ReplayAll ();

      bool hasAccess;
      using (new SecurityFreeSection ())
      {
        hasAccess = _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");
      }

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), "The securableObject did not return an IObjectSecurityStrategy.")]
    public void Test_WithSecurityStrategyIsNull ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty", TestAccessType.First);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasPropertyWriteAccess (new SecurableObject (null), "InstanceProperty");
    }
  }
}
