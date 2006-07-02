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
    public void GetRequiredMethodPermissions_MethodWithoutAttributes ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Save");

      Assert.IsNotNull (requiredAccessTypes);
      Assert.IsEmpty (requiredAccessTypes);
    }

    [Test]
    public void GetRequiredMethodPermissions_MethodWithOneAttribute ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Record");

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessType.Edit, requiredAccessTypes);
    }

    [Test]
    public void GetRequiredMethodPermissions_OverloadedMethodWithOneAttribute ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Delete");

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.AreEqual (GeneralAccessType.Delete, requiredAccessTypes[0]);
    }

    [Test]
    public void GetRequiredMethodPermissions_MethodWithTwoPermissions ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Show");

      Assert.AreEqual (2, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessType.Edit, requiredAccessTypes);
      Assert.Contains (GeneralAccessType.Create, requiredAccessTypes);
    }

    [Test]
    public void GetRequiredMethodPermissions_MethodOfDerivedClass ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (DerivedSecurableObject), "Show");

      Assert.AreEqual (2, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessType.Edit, requiredAccessTypes);
      Assert.Contains (GeneralAccessType.Create, requiredAccessTypes);
    }

    [Test, ExpectedException (typeof (ArgumentException), "The member 'Sve' could not be found.\r\nParameter name: memberName")]
    public void GetRequiredMethodPermissions_NotExistingMethod ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Sve");
    }

    [Test, ExpectedException (typeof (ArgumentException),
     "The member 'Send' has multiple DemandMethodPermissionAttribute defined.\r\nParameter name: memberName")]
    public void GetRequiredMethodPermissions_PermissionsDeclaredOnBaseAndDerivedClass ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (DerivedSecurableObject), "Send");
    }

    [Test, ExpectedException (typeof (ArgumentException),
     "The member 'Load' has multiple DemandMethodPermissionAttribute defined.\r\nParameter name: memberName")]
    public void GetRequiredMethodPermissions_PermissionsDeclaredOnOverloads ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Load");
    }

    [Test, ExpectedException (typeof (ArgumentException),
      "The DemandMethodPermissionAttribute must not be defined on members overriden or redefined in derived classes. "
        + "A member 'Print' exists in class 'Rubicon.Security.UnitTests.SampleDomain.PermissionReflection.DerivedSecurableObject' and its "
        + "base class."
        + "\r\nParameter name: memberName")]
    public void GetRequiredMethodPermissions_VirtualMethod ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (DerivedSecurableObject), "Print");
    }

    [Test]
    public void GetRequiredMethodPermissions_StaticMethodWithoutAttributes ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "CheckPermissions");

      Assert.IsNotNull (requiredAccessTypes);
      Assert.IsEmpty (requiredAccessTypes);
    }

    [Test]
    public void GetRequiredMethodPermissions_StaticMethodWithOneAttribute ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "CreateForSpecialCase");

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.AreEqual (GeneralAccessType.Create, requiredAccessTypes[0]);
    }

    [Test]
    public void GetRequiredMethodPermissions_StaticOverloadedMethodWithOneAttributes ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "IsValid");

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.AreEqual (GeneralAccessType.Read, requiredAccessTypes[0]);
    }

    [Test]
    public void GetRequiredMethodPermissions_StaticMethodOfDerivedClass ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (DerivedSecurableObject), "CreateForSpecialCase");

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.AreEqual (GeneralAccessType.Create, requiredAccessTypes[0]);
    }

    [Test, ExpectedException (typeof (ArgumentException), "The member 'Sve' could not be found.\r\nParameter name: memberName")]
    public void GetRequiredMethodPermissions_NotExistingStaticMethod ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "Sve");
    }

    [Test]
    public void GetRequiredMethodPermissions_FilterMultipleAccessTypes ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Close");

      Assert.AreEqual (2, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessType.Edit, requiredAccessTypes);
      Assert.Contains (GeneralAccessType.Find, requiredAccessTypes);
    }

    [Test]
    public void GetRequiredPropertyReadPermissions_PropertyWithoutAttributes ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredPropertyReadPermissions (typeof (SecurableObject), "IsEnabled");

      Assert.IsNotNull (requiredAccessTypes);
      Assert.IsEmpty (requiredAccessTypes);
    }

    [Test]
    public void GetRequiredPropertyReadPermissions_PropertyWithOneAttribute ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredPropertyReadPermissions (typeof (SecurableObject), "IsVisible");

      Assert.IsNotNull (requiredAccessTypes);
      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessType.Create, requiredAccessTypes);
    }

    [Test]
    public void GetRequiredPropertyWritePermissions_PropertyWithoutAttributes ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredPropertyWritePermissions (typeof (SecurableObject), "IsEnabled");

      Assert.IsNotNull (requiredAccessTypes);
      Assert.IsEmpty (requiredAccessTypes);
    }

    [Test]
    public void GetRequiredPropertyWritePermissions_PropertyWithOneAttribute ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredPropertyWritePermissions (typeof (SecurableObject), "IsVisible");

      Assert.IsNotNull (requiredAccessTypes);
      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessType.Find, requiredAccessTypes);
    }
  }
}
