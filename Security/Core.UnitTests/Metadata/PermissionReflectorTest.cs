using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

using Rubicon.Security.UnitTests.SampleDomain.PermissionReflection;
using Rubicon.Security.Metadata;

namespace Rubicon.Security.UnitTests.Metadata
{
  [TestFixture]
  public class PermissionReflectorTest
  {
    private IPermissionProvider _permissionReflector;

    [SetUp]
    public void SetUp ()
    {
      _permissionReflector = new PermissionReflector ();
    }

    [Test]
    public void GetPermissionsForMethodWithoutAttributes ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Save");

      Assert.IsNotNull (requiredAccessTypes);
      Assert.IsEmpty (requiredAccessTypes);
    }

    [Test]
    public void GetPermissionsForMethodWithOneAttribute ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Record");

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessType.Edit, requiredAccessTypes);
    }

    [Test]
    public void GetPermissionsForOverloadedMethodOneAttribute ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Delete");

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.AreEqual (GeneralAccessType.Delete, requiredAccessTypes[0]);
    }

    [Test]
    public void GetPermissionsForMethodWithTwoPermissions ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Show");

      Assert.AreEqual (2, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessType.Edit, requiredAccessTypes);
      Assert.Contains (GeneralAccessType.Create, requiredAccessTypes);
    }

    [Test]
    public void GetPermissionsForMethodOfDerivedClass ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (DerivedSecurableObject), "Show");

      Assert.AreEqual (2, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessType.Edit, requiredAccessTypes);
      Assert.Contains (GeneralAccessType.Create, requiredAccessTypes);
    }

    [Test, ExpectedException (typeof (ArgumentException), "The method 'Sve' could not be found.\r\nParameter name: methodName")]
    public void GetExceptionForNotExistingMethod ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Sve");
    }

    [Test, ExpectedException (typeof (ArgumentException),
        "The method 'Send' has multiple RequiredMethodPermissionAttributes defined.\r\nParameter name: methodName")]
    public void GetExceptionForPermissionsDeclaredOnBaseAndDerivedClass ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (DerivedSecurableObject), "Send");
    }

    [Test, ExpectedException (typeof (ArgumentException),
        "The method 'Load' has multiple RequiredMethodPermissionAttributes defined.\r\nParameter name: methodName")]
    public void GetExceptionForPermissionsDeclaredOnOverloads ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Load");
    }

    [Test, ExpectedException (typeof (ArgumentException),
        "The RequiredMethodPermissionAttributes must not be defined on methods overriden or redefined in derived classes. "
        + "A method 'Print' exists in class 'Rubicon.Security.UnitTests.SampleDomain.PermissionReflection.DerivedSecurableObject' and its "
        + "base class."
        + "\r\nParameter name: methodName")]
    public void GetExceptionForVirtualMethod ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (DerivedSecurableObject), "Print");
    }

    [Test]
    public void GetPermissionsForStaticMethodWithoutAttributes ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "CheckPermissions");

      Assert.IsNotNull (requiredAccessTypes);
      Assert.IsEmpty (requiredAccessTypes);
    }

    [Test]
    public void GetPermissionsForStaticMethodOneAttribute ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "CreateForSpecialCase");

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.AreEqual (GeneralAccessType.Create, requiredAccessTypes[0]);
    }

    [Test]
    public void GetPermissionsForStaticOverloadedMethodOneAttribute ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "IsValid");

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.AreEqual (GeneralAccessType.Read, requiredAccessTypes[0]);
    }

    [Test]
    public void GetPermissionsForStaticMethodOfDerivedClass ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (DerivedSecurableObject), "CreateForSpecialCase");

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.AreEqual (GeneralAccessType.Create, requiredAccessTypes[0]);
    }

    [Test, ExpectedException (typeof (ArgumentException), "The method 'Sve' could not be found.\r\nParameter name: methodName")]
    public void GetExceptionForNotExistingStaticMethod ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "Sve");
    }

    [Test]
    public void FilterMultipleAccessTypes ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Close");

      Assert.AreEqual (2, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessType.Edit, requiredAccessTypes);
      Assert.Contains (GeneralAccessType.Find, requiredAccessTypes);
    }
  }
}
