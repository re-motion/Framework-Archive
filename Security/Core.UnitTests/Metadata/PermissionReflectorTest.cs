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
    private IPermissionReflector _permissionReflector;

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
    public void GetPermissionsForVirtualMethod ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Print");

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessType.Find, requiredAccessTypes);
    }

    [Test]
    public void GetPermissionsForMethodWithTwoAttributes ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Show");

      Assert.AreEqual (2, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessType.Edit, requiredAccessTypes);
      Assert.Contains (GeneralAccessType.Create, requiredAccessTypes);
    }

    [Test]
    public void GetPermissionsForNewMethodInDerivedClass ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (DerivedSecurableObject), "Send");

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessType.Read, requiredAccessTypes);
    }

    [Test]
    public void GetPermissionsForVirtualMethodInDerivedClass ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (DerivedSecurableObject), "Print");

      Assert.AreEqual (2, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessType.Find, requiredAccessTypes);
      Assert.Contains (GeneralAccessType.Create, requiredAccessTypes);
    }

    [Test, ExpectedException (typeof (System.Reflection.AmbiguousMatchException))]
    public void GetExceptionForOverloadedMethod ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (DerivedSecurableObject), "Load");
    }

    [Test]
    public void GetPermissionsForOverloadedMethod ()
    {
      Enum[] requiredAccessTypes = 
          _permissionReflector.GetRequiredMethodPermissions (typeof (DerivedSecurableObject), "Load", new Type[] { typeof (string) });

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessType.Create, requiredAccessTypes);
    }

    [Test]
    public void GetPermissionsForOverloadedMethodWithoutParameters ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (DerivedSecurableObject), "Load", new Type[0]);

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessType.Delete, requiredAccessTypes);
    }

    [Test]
    public void FilterDuplicatedMethodPermissions ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (DerivedSecurableObject), "Create");

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessType.Create, requiredAccessTypes);
    }

    [Test, ExpectedException (typeof (ArgumentException),
       "The method 'NotExistingMethod' does not exist on type 'Rubicon.Security.UnitTests.SampleDomain.PermissionReflection.DerivedSecurableObject'."
        + "\r\nParameter name: methodName")]
    public void GetExceptionForNotExistingMethod ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (DerivedSecurableObject), "NotExistingMethod");
    }

    [Test, ExpectedException (typeof (ArgumentException),
       "The method 'NotExistingMethod' does not exist on type 'Rubicon.Security.UnitTests.SampleDomain.PermissionReflection.DerivedSecurableObject'."
        + "\r\nParameter name: methodName")]
    public void GetExceptionForNotExistingOverloadedMethod ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (DerivedSecurableObject), "NotExistingMethod",
          new Type[] { typeof (string) });
    }

    [Test]
    public void GetPermissionsForStaticMethodWithoutAttributes ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "CheckPermissions");

      Assert.IsNotNull (requiredAccessTypes);
      Assert.IsEmpty (requiredAccessTypes);
    }

    [Test]
    public void GetPermissionsForStaticMethodWithOneAttribute ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "CreateForSpecialCase");

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessType.Create, requiredAccessTypes);
    }

    [Test, ExpectedException (typeof (ArgumentException),
       "The static method 'NotExistingMethod' does not exist on type 'Rubicon.Security.UnitTests.SampleDomain.PermissionReflection.SecurableObject'."
        + "\r\nParameter name: methodName")]
    public void GetExceptionForNotExistingStaticMethod ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "NotExistingMethod");
    }

    [Test]
    public void GetPermissionsForStaticMethodInDerivedClass ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (DerivedSecurableObject), "CreateForSpecialCase");

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessType.Create, requiredAccessTypes);
    }

    [Test]
    public void GetPermissionsForOverloadedStaticMethodWithoutAttributes ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "IsValid", new Type[0]);

      Assert.IsNotNull (requiredAccessTypes);
      Assert.IsEmpty (requiredAccessTypes);
    }

    [Test]
    public void GetPermissionsForOverloadedStaticMethodWithOneAttribute ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "IsValid", 
          new Type[] { typeof (SecurableObject) });

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessType.Read, requiredAccessTypes);
    }

    [Test, ExpectedException (typeof (ArgumentException),
       "The static method 'NotExistingMethod' does not exist on type 'Rubicon.Security.UnitTests.SampleDomain.PermissionReflection.SecurableObject'."
        + "\r\nParameter name: methodName")]
    public void GetExceptionForNotExistingOverloadedStaticMethod ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "NotExistingMethod", new Type[0]);
    }

    [Test]
    public void GetPermissionsForOverloadedStaticMethodInDerivedClass ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (DerivedSecurableObject), "IsValid", 
          new Type[] { typeof (SecurableObject) });

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessType.Read, requiredAccessTypes);
    }

    [Test]
    public void GetPermissionsForConstructor ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredConstructorPermissions (typeof (SecurableObjectWithoutConstructorOverloads));

      Assert.AreEqual (0, requiredAccessTypes.Length);
    }

    [Test]
    public void GetPermissionsForConstructorWithAttribute ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredConstructorPermissions (typeof (DerivedSecurableObjectWithoutConstructorOverloads));

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessType.Find, requiredAccessTypes);
    }

    [Test, ExpectedException (typeof (System.Reflection.AmbiguousMatchException))]
    public void GetExceptionForOverloadedConstructor ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredConstructorPermissions (typeof (SecurableObject));
    }

    [Test]
    public void GetPermissionsForOverloadedConstructor ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredConstructorPermissions (typeof (DerivedSecurableObject), new Type[0]);

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessType.Find, requiredAccessTypes);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
       "Type 'Rubicon.Security.UnitTests.SampleDomain.PermissionReflection.SecurableObjectWithPrivateConstructor' does not have a public constructor."
        + "\r\nParameter name: type")]
    public void GetExceptionForPrivateConstructor ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredConstructorPermissions (typeof (SecurableObjectWithPrivateConstructor));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
       "The specified constructor for type 'Rubicon.Security.UnitTests.SampleDomain.PermissionReflection.SecurableObjectWithPrivateOverloadedConstructor' "
        + "is not public.\r\nParameter name: type")]
    public void GetExceptionForPrivateOverloadedConstructor ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredConstructorPermissions (typeof (SecurableObjectWithPrivateOverloadedConstructor),
          new Type[0]);
    }
  }
}
