using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using System.Security.Principal;
using Rubicon.Data.DomainObjects;
using Rubicon.Security.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.Queries;


namespace Rubicon.Security.Data.DomainObjects.UnitTests.SecurityClientTransactionExtensionTests
{
  [TestFixture]
  public class RelationChangingTest : BaseTest
  {
    private SecurityClientTransactionExtensionTestHelper _testHelper;
    private IClientTransactionExtension _extension;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = new SecurityClientTransactionExtensionTestHelper ();
      _extension = new SecurityClientTransactionExtension ();

      _testHelper.SetupSecurityConfiguration ();
    }

    [TearDown]
    public void TearDown ()
    {
      _testHelper.TearDownSecurityConfiguration ();
    }

    [Test]
    public void Test_AccessGranted ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("Parent", TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      _extension.RelationChanging (securableObject, "Parent", null, null);

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("Parent", TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, false);
      _testHelper.ReplayAll ();

      _extension.RelationChanging (securableObject, "Parent", null, null);
    }

    [Test]
    public void Test_AccessGranted_WithinSecurityFreeSection ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _extension.RelationChanging (securableObject, "Parent", null, null);
      }

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithNonSecurableObject ()
    {
      NonSecurableObject nonSecurableObject = _testHelper.CreateNonSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      _extension.RelationChanging (nonSecurableObject, "Parent", null, null);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_OneSide_RecursiveSecurity ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      SecurableObject newObject = _testHelper.CreateSecurableObject ();
      securableObject.OtherParent = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("Parent", TestAccessTypes.First);
      HasAccessDelegate hasAccess = delegate (ISecurityService securityService, IPrincipal user, AccessType[] requiredAccessTypes)
      {
        securableObject.OtherParent = newObject;
        return true;
      };
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, hasAccess);
      _testHelper.ReplayAll ();

      _extension.RelationChanging (securableObject, "Parent", null, null);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_OneSideSetNull_AccessedViaDomainObject ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      SecurableObject newObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      using (_testHelper.Ordered ())
      {
        _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("Parent", TestAccessTypes.First);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
        _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("Children", TestAccessTypes.Second);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (newObject, TestAccessTypes.Second, true);
      }
      _testHelper.ReplayAll ();

      securableObject.Parent = newObject;

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_OneSideSetNewValue_AccessedViaDomainObject ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      SecurableObject oldObject = _testHelper.CreateSecurableObject ();
      securableObject.Parent = oldObject;
      _testHelper.AddExtension (_extension);
      using (_testHelper.Ordered ())
      {
        _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("Parent", TestAccessTypes.First);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
        _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("Children", TestAccessTypes.Second);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (oldObject, TestAccessTypes.Second, true);
      }
      _testHelper.ReplayAll ();

      securableObject.Parent = null;

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_ManySideAdd_AccessedViaDomainObject ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      SecurableObject newObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      using (_testHelper.Ordered())
      {
        _testHelper.ExpectPermissionReflectorGetRequiredPropertyReadPermissions ("Children", TestAccessTypes.First);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
        _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("Parent", TestAccessTypes.Second);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (newObject, TestAccessTypes.Second, true);
        _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("Children", TestAccessTypes.First);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      }
      _testHelper.ReplayAll ();

      securableObject.Children.Add (newObject);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_ManySideRemove_AccessedViaDomainObject ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      SecurableObject oldObject = _testHelper.CreateSecurableObject ();
      securableObject.Children.Add (oldObject);
      _testHelper.AddExtension (_extension);
      using (_testHelper.Ordered ())
      {
        _testHelper.ExpectPermissionReflectorGetRequiredPropertyReadPermissions ("Children", TestAccessTypes.First);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
        _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("Parent", TestAccessTypes.Second);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (oldObject, TestAccessTypes.Second, true);
        _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("Children", TestAccessTypes.First);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      }
      _testHelper.ReplayAll ();

      securableObject.Children.Remove (oldObject);

      _testHelper.VerifyAll ();
    }
  }
}