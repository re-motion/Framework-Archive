using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Rubicon.Security.Web.UnitTests.Domain;
using Rubicon.Security.Web.UI;

namespace Rubicon.Security.Web.UnitTests.UI
{
  [TestFixture]
  public class DemandTargetMethodPermissionAttributeTest
  {
    [Test]
    public void Initialize_WithMethodName ()
    {
      DemandTargetMethodPermissionAttribute attribute = new DemandTargetMethodPermissionAttribute ("Show");

      Assert.AreEqual (PermissionSource.SecurableObject, attribute.PermissionSource);
      Assert.AreEqual ("Show", attribute.MethodName);
      Assert.IsNull (attribute.SecurableClass);
    }

    [Test]
    public void Initialize_WithMethodNameAndSecurableClass ()
    {
      DemandTargetMethodPermissionAttribute attribute = new DemandTargetMethodPermissionAttribute ("Show", typeof (SecurableObject));

      Assert.AreEqual (PermissionSource.SecurableObject, attribute.PermissionSource);
      Assert.AreEqual ("Show", attribute.MethodName);
      Assert.AreSame ( typeof (SecurableObject), attribute.SecurableClass);
    }

    [Test]
    public void Initialize_WithMethodNameEnum ()
    {
      DemandTargetMethodPermissionAttribute attribute = new DemandTargetMethodPermissionAttribute (SecurableObject.Method.Show);

      Assert.AreEqual (PermissionSource.SecurableObject, attribute.PermissionSource);
      Assert.AreEqual ("Show", attribute.MethodName);
      Assert.AreSame (typeof (SecurableObject), attribute.SecurableClass);
    }

    [Test]
    public void Initialize_WithMethodNameEnumAndSecurableClass ()
    {
      DemandTargetMethodPermissionAttribute attribute = new DemandTargetMethodPermissionAttribute (SecurableObject.Method.Show, typeof (DerivedSecurableObject));

      Assert.AreEqual (PermissionSource.SecurableObject, attribute.PermissionSource);
      Assert.AreEqual ("Show", attribute.MethodName);
      Assert.AreSame (typeof (DerivedSecurableObject), attribute.SecurableClass);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), 
        "Enumerated type 'Rubicon.Security.Web.UnitTests.Domain.MethodNameEnum' is not declared as a nested type.\r\nParameter name: methodNameEnum")]
    public void Initialize_WithMethodNameEnumNotNestedType ()
    {
      new DemandTargetMethodPermissionAttribute (MethodNameEnum.Show);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        "The declaring type of enumerated type 'Rubicon.Security.Web.UnitTests.Domain.SimpleType+MethodNameEnum' does not implement interface"
        + " 'Rubicon.Security.ISecurableObject'.\r\nParameter name: methodNameEnum")]
    public void Initialize_WithMethodNameEnumNotHavingValidDeclaringType ()
    {
      new DemandTargetMethodPermissionAttribute (SimpleType.MethodNameEnum.Show);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        "Type 'Rubicon.Security.Web.UnitTests.Domain.OtherSecurableObject' cannot be assigned to the declaring type of enumerated type"
        + " 'Rubicon.Security.Web.UnitTests.Domain.SecurableObject+Method'.\r\nParameter name: securableClass")]
    public void TestWithParameterNotOfNotMatchingType ()
    {
      new DemandTargetMethodPermissionAttribute (SecurableObject.Method.Show, typeof (OtherSecurableObject));
    }
  }
}