using System;
using NUnit.Framework;
using Rubicon.Security.Web.ExecutionEngine;
using Rubicon.Security.UnitTests.Web.Domain;

namespace Rubicon.Security.UnitTests.Web.ExecutionEngine
{
  [TestFixture]
  public class WxeDemandTargetStaticMethodPermissionAttributeTest
  {
    [Test]
    public void Initialize_WithMethodNameAndSecurableClass ()
    {
      WxeDemandTargetStaticMethodPermissionAttribute attribute = new WxeDemandTargetStaticMethodPermissionAttribute ("Search", typeof (SecurableObject));

      Assert.AreEqual (MethodType.Static, attribute.MethodType);
      Assert.AreEqual ("Search", attribute.MethodName);
      Assert.AreSame ( typeof (SecurableObject), attribute.SecurableClass);
    }

    [Test]
    public void Initialize_WithMethodNameEnum ()
    {
      WxeDemandTargetStaticMethodPermissionAttribute attribute = new WxeDemandTargetStaticMethodPermissionAttribute (SecurableObject.Method.Search);

      Assert.AreEqual (MethodType.Static, attribute.MethodType);
      Assert.AreEqual ("Search", attribute.MethodName);
      Assert.AreSame (typeof (SecurableObject), attribute.SecurableClass);
    }

    [Test]
    public void Initialize_WithMethodNameEnumAndSecurableClass ()
    {
      WxeDemandTargetStaticMethodPermissionAttribute attribute = 
          new WxeDemandTargetStaticMethodPermissionAttribute (SecurableObject.Method.Search, typeof (DerivedSecurableObject));

      Assert.AreEqual (MethodType.Static, attribute.MethodType);
      Assert.AreEqual ("Search", attribute.MethodName);
      Assert.AreSame (typeof (DerivedSecurableObject), attribute.SecurableClass);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "Enumerated type 'Rubicon.Security.UnitTests.Web.Domain.MethodNameEnum' is not declared as a nested type.\r\nParameter name: methodNameEnum")]
    public void Initialize_WithMethodNameEnumNotNestedType ()
    {
      new WxeDemandTargetStaticMethodPermissionAttribute (MethodNameEnum.Show);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The declaring type of enumerated type 'Rubicon.Security.UnitTests.Web.Domain.SimpleType+MethodNameEnum' does not implement interface"
        + " 'Rubicon.Security.ISecurableObject'.\r\nParameter name: methodNameEnum")]
    public void Initialize_WithMethodNameEnumNotHavingValidDeclaringType ()
    {
      new WxeDemandTargetStaticMethodPermissionAttribute (SimpleType.MethodNameEnum.Show);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "Enumerated type 'Rubicon.Security.UnitTests.Web.Domain.MethodNameEnum' is not declared as a nested type.\r\nParameter name: methodNameEnum")]
    public void Initialize_WithMethodNameEnumNotNestedTypeAndSecurableClass ()
    {
      new WxeDemandTargetStaticMethodPermissionAttribute (MethodNameEnum.Show, typeof (DerivedSecurableObject));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The declaring type of enumerated type 'Rubicon.Security.UnitTests.Web.Domain.SimpleType+MethodNameEnum' does not implement interface"
        + " 'Rubicon.Security.ISecurableObject'.\r\nParameter name: methodNameEnum")]
    public void Initialize_WithMethodNameEnumNotHavingValidDeclaringTypeAndSecurableClass ()
    {
      new WxeDemandTargetStaticMethodPermissionAttribute (SimpleType.MethodNameEnum.Show, typeof (DerivedSecurableObject));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "Type 'Rubicon.Security.UnitTests.Web.Domain.OtherSecurableObject' cannot be assigned to the declaring type of enumerated type"
        + " 'Rubicon.Security.UnitTests.Web.Domain.SecurableObject+Method'.\r\nParameter name: securableClass")]
    public void TestWithParameterNotOfNotMatchingType ()
    {
      new WxeDemandTargetStaticMethodPermissionAttribute (SecurableObject.Method.Show, typeof (OtherSecurableObject));
    }
  }
}
