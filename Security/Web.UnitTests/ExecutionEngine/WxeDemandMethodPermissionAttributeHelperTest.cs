using System;
using System.Collections.Generic;
using System.Text;

using NMock2;
using NUnit.Framework;

using Rubicon.Security.Web.UnitTests.Domain;
using Rubicon.Security.Web.ExecutionEngine;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Security.Web.UnitTests.ExecutionEngine
{
  [TestFixture]
  public class WxeDemandMethodPermissionAttributeHelperTest
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public WxeDemandMethodPermissionAttributeHelperTest ()
    {
    }

    // methods and properties

    [Test]
    public void InitializeWithMethodTypeInstance ()
    {
      WxeDemandMethodPermissionAttribute attribute = new WxeDemandMethodPermissionAttribute (MethodType.Instance);
      attribute.SecurableClass = null;
      attribute.MethodName = "Show";

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      Assert.AreSame (typeof (TestFunctionWithThisObject), helper.FunctionType);
      Assert.AreEqual (MethodType.Instance, helper.MethodType);
      Assert.IsNull (helper.SecurableClass);
      Assert.AreEqual ("Show", helper.MethodName);
    }

    [Test]
    [ExpectedException (typeof (WxeException), 
        "The WxeDemandMethodPermissionAttribute applied to WxeFunction 'Rubicon.Security.Web.UnitTests.ExecutionEngine.TestFunctionWithThisObject'"
        + " does not specify the method to get the required permissions from.")]
    public void InitializeWithMethodTypeInstanceAndNoMethodName ()
    {
      WxeDemandMethodPermissionAttribute attribute = new WxeDemandMethodPermissionAttribute (MethodType.Instance);
      attribute.SecurableClass = null;
      attribute.MethodName = null;

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);
    }


    [Test]
    public void InitializeWithMethodTypeStatic ()
    {
      WxeDemandMethodPermissionAttribute attribute = new WxeDemandMethodPermissionAttribute (MethodType.Static);
      attribute.SecurableClass = typeof (SecurableObject);
      attribute.MethodName = "Search";

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      Assert.AreSame (typeof (TestFunctionWithThisObject), helper.FunctionType);
      Assert.AreEqual (MethodType.Static, helper.MethodType);
      Assert.AreSame (typeof (SecurableObject), helper.SecurableClass);
      Assert.AreEqual ("Search", helper.MethodName);
    }

    [Test]
    [ExpectedException (typeof (WxeException),
        "The WxeDemandMethodPermissionAttribute applied to WxeFunction 'Rubicon.Security.Web.UnitTests.ExecutionEngine.TestFunctionWithThisObject'"
        + " does not specify the method to get the required permissions from.")]
    public void InitializeWithMethodTypeStaticAndNoMethodName ()
    {
      WxeDemandMethodPermissionAttribute attribute = new WxeDemandMethodPermissionAttribute (MethodType.Static);
      attribute.SecurableClass = typeof (SecurableObject);
      attribute.MethodName = null;

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);
    }

    [Test]
    [ExpectedException (typeof (WxeException),
        "The WxeDemandMethodPermissionAttribute applied to WxeFunction 'Rubicon.Security.Web.UnitTests.ExecutionEngine.TestFunctionWithThisObject'"
        + " does not specify a type implementing interface 'Rubicon.Security.ISecurableObject'.")]
    public void InitializeWithMethodTypeStaticAndNoSecurableClass ()
    {
      WxeDemandMethodPermissionAttribute attribute = new WxeDemandMethodPermissionAttribute (MethodType.Static);
      attribute.SecurableClass = null;
      attribute.MethodName = "Search";

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);
    }

    [Test]
    public void InitializeWithMethodTypeConstructor ()
    {
      WxeDemandMethodPermissionAttribute attribute = new WxeDemandMethodPermissionAttribute (MethodType.Constructor);
      attribute.SecurableClass = typeof (SecurableObject);
      attribute.MethodName = null;

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      Assert.AreSame (typeof (TestFunctionWithThisObject), helper.FunctionType);
      Assert.AreEqual (MethodType.Constructor, helper.MethodType);
      Assert.AreSame (typeof (SecurableObject), helper.SecurableClass);
      Assert.IsNull (helper.MethodName);
    }

    [Test]
    [ExpectedException (typeof (WxeException),
        "The WxeDemandMethodPermissionAttribute applied to WxeFunction 'Rubicon.Security.Web.UnitTests.ExecutionEngine.TestFunctionWithThisObject'"
        + " does not specify a type implementing interface 'Rubicon.Security.ISecurableObject'.")]
    public void InitializeWithMethodTypeConstructorAndNoSecurableClass ()
    {
      WxeDemandMethodPermissionAttribute attribute = new WxeDemandMethodPermissionAttribute (MethodType.Constructor);
      attribute.SecurableClass = null;
      attribute.MethodName = null;

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);
    }
  }
}