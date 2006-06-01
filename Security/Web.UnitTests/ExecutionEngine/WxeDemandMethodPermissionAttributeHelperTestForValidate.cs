using System;
using System.Collections.Generic;
using System.Text;

using NMock2;
using NUnit.Framework;

using Rubicon.Security.Web.ExecutionEngine;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Security.Web.UnitTests.ExecutionEngine
{
  [TestFixture]
  public class WxeDemandMethodPermissionAttributeHelperTestForValidate
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public WxeDemandMethodPermissionAttributeHelperTestForValidate ()
    {
    }

    // methods and properties

    [Test]
    public void TestWithMethodTypeInstance ()
    {
      WxeDemandMethodPermissionAttribute attribute = new WxeDemandMethodPermissionAttribute (MethodType.Instance);
      attribute.SecurableClass = null;
      attribute.MethodName = "Show";

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      helper.Validate ();
    }

    [Test]
    [ExpectedException (typeof (WxeException), 
        "The WxeDemandMethodPermissionAttribute applied to WxeFunction 'Rubicon.Security.Web.UnitTests.ExecutionEngine.TestFunctionWithThisObject'"
        + " does not specify the method to get the required permissions from.")]
    public void TestWithMethodTypeInstanceAndNoMethodName ()
    {
      WxeDemandMethodPermissionAttribute attribute = new WxeDemandMethodPermissionAttribute (MethodType.Instance);
      attribute.SecurableClass = null;
      attribute.MethodName = null;

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      helper.Validate ();
    }


    [Test]
    public void TestWithMethodTypeStatic ()
    {
      WxeDemandMethodPermissionAttribute attribute = new WxeDemandMethodPermissionAttribute (MethodType.Static);
      attribute.SecurableClass = typeof (SecurableObject);
      attribute.MethodName = "Search";

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      helper.Validate ();
    }

    [Test]
    [ExpectedException (typeof (WxeException),
        "The WxeDemandMethodPermissionAttribute applied to WxeFunction 'Rubicon.Security.Web.UnitTests.ExecutionEngine.TestFunctionWithThisObject'"
        + " does not specify the method to get the required permissions from.")]
    public void TestWithMethodTypeStaticAndNoMethodName ()
    {
      WxeDemandMethodPermissionAttribute attribute = new WxeDemandMethodPermissionAttribute (MethodType.Static);
      attribute.SecurableClass = typeof (SecurableObject);
      attribute.MethodName = null;

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      helper.Validate ();
    }

    [Test]
    [ExpectedException (typeof (WxeException),
        "The WxeDemandMethodPermissionAttribute applied to WxeFunction 'Rubicon.Security.Web.UnitTests.ExecutionEngine.TestFunctionWithThisObject'"
        + " does not specify a type implementing interface 'Rubicon.Security.ISecurableObject'.")]
    public void TestWithMethodTypeStaticAndNoSecurableClass ()
    {
      WxeDemandMethodPermissionAttribute attribute = new WxeDemandMethodPermissionAttribute (MethodType.Static);
      attribute.SecurableClass = null;
      attribute.MethodName = "Search";

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      helper.Validate ();
    }

    [Test]
    public void TestWithMethodTypeConstructor ()
    {
      WxeDemandMethodPermissionAttribute attribute = new WxeDemandMethodPermissionAttribute (MethodType.Constructor);
      attribute.SecurableClass = typeof (SecurableObject);
      attribute.MethodName = null;

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      helper.Validate ();
    }

    [Test]
    [ExpectedException (typeof (WxeException),
        "The WxeDemandMethodPermissionAttribute applied to WxeFunction 'Rubicon.Security.Web.UnitTests.ExecutionEngine.TestFunctionWithThisObject'"
        + " does not specify a type implementing interface 'Rubicon.Security.ISecurableObject'.")]
    public void TestWithMethodTypeConstructorAndNoSecurableClass ()
    {
      WxeDemandMethodPermissionAttribute attribute = new WxeDemandMethodPermissionAttribute (MethodType.Constructor);
      attribute.SecurableClass = null;
      attribute.MethodName = null;

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      helper.Validate ();
    }
  }
}