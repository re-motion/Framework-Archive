using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Development.UnitTesting;
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
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty", TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");

      Assert.IsTrue (hasAccess);
      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_AccessDenied ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty", TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (TestAccessTypes.First, false);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");

      _testHelper.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty", TestAccessTypes.First);
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
      _testHelper.ExpectObjectSecurityStrategyHasAccess (GeneralAccessTypes.Edit, true);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void Test_AccessDenied_WithDefaultAccessType ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty");
      _testHelper.ExpectObjectSecurityStrategyHasAccess (GeneralAccessTypes.Edit, false);
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
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty", TestAccessTypes.First);
      _testHelper.ReplayAll ();

      _securityClient.HasPropertyWriteAccess (new SecurableObject (null), "InstanceProperty");

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), "IPermissionProvider.GetRequiredPropertyWritePermissions evaluated and returned null.")]
    public void Test_WithPermissionProviderReturnedNull_ShouldThrowInvalidOperationException ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty", (Enum[]) null);
      _testHelper.ReplayAll ();

      _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), "IPermissionProvider.GetRequiredPropertyWritePermissions evaluated and returned null.")]
    public void Test_WithPermissionProviderReturnedNullAndWithinSecurityFreeSection_ShouldThrowInvalidOperationException ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty", (Enum[]) null);
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _securityClient.HasPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");
      }

      _testHelper.VerifyAll ();
    }
  }
}
