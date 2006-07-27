using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

using Rubicon.Security.Metadata;
using Rubicon.Security.UnitTests.SampleDomain;

namespace Rubicon.Security.UnitTests.SecurityClientTests
{
  [TestFixture]
  public class CheckPropertyWriteAccessTest
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

      _securityClient.CheckPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied_ShouldThrowPermissionDeniedException ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty", TestAccessType.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (TestAccessType.First, false);
      _testHelper.ReplayAll ();

      _securityClient.CheckPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty", TestAccessType.First);
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _securityClient.CheckPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");
      }
    }

    [Test]
    public void Test_AccessGranted_WithDefaultAccessType ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty");
      _testHelper.ExpectObjectSecurityStrategyHasAccess (GeneralAccessType.Edit, true);
      _testHelper.ReplayAll ();

      _securityClient.CheckPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied_WithDefaultAccessType_ShouldThrowPermissionDeniedException ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty");
      _testHelper.ExpectObjectSecurityStrategyHasAccess (GeneralAccessType.Edit, false);
      _testHelper.ReplayAll ();

      _securityClient.CheckPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");
    }

    [Test]
    public void Test_AccessGranted_WithDefaultAccessTypeAndWithinSecurityFreeSection ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty");
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _securityClient.CheckPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), "The securableObject did not return an IObjectSecurityStrategy.")]
    public void Test_WithSecurityStrategyIsNull ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty", TestAccessType.First);
      _testHelper.ReplayAll ();

      _securityClient.CheckPropertyWriteAccess (new SecurableObject (null), "InstanceProperty");
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), "IPermissionProvider.GetRequiredPropertyWritePermissions evaluated and returned null.")]
    public void Test_WithPermissionProviderReturnedNull_ShouldThrowInvalidOperationException ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty", (Enum[]) null);
      _testHelper.ReplayAll ();

      _securityClient.CheckPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), "IPermissionProvider.GetRequiredPropertyWritePermissions evaluated and returned null.")]
    public void Test_WithPermissionProviderReturnedNullAndWithinSecurityFreeSection_ShouldThrowInvalidOperationException ()
    {
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("InstanceProperty", (Enum[]) null);
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _securityClient.CheckPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");
      }
    }
  }
}
