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
  public class WxeDemandMethodPermissionAttributeHelperTestForGetTypeOfSecurableObject
  {
    // types

    // static members

    // member fields
    private Mockery _mocks;
    private WxeDemandMethodPermissionAttribute _attribute;

    // construction and disposing

    public WxeDemandMethodPermissionAttributeHelperTestForGetTypeOfSecurableObject ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _mocks = new Mockery ();
      _attribute = new WxeDemandMethodPermissionAttribute (MethodType.Static);
    }

    [Test]
    public void TestWithValidParameterName ()
    {
      _attribute.ParameterName = "ThisObject";

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObjectAsSecondParameter), 
          _attribute);

      Assert.AreSame (typeof (SecurableObject), helper.GetTypeOfSecurableObject ());
    }

    [Test]
    public void TestWithDefaultParameter ()
    {
      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject), 
          _attribute);

      Assert.AreSame (typeof (SecurableObject), helper.GetTypeOfSecurableObject ());
    }

    [Test]
    [ExpectedException (typeof (WxeException), "The parameter 'SomeObject' specified by the WxeDemandMethodPermissionAttribute applied to"
        + " WxeFunction 'Rubicon.Security.Web.UnitTests.ExecutionEngine.TestFunctionWithThisObjectAsSecondParameter' does not implement interface"
        + " 'Rubicon.Security.ISecurableObject'.")]
    public void TestWithParameterNotImplementingISecurableObject ()
    {
      _attribute.ParameterName = "SomeObject";

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObjectAsSecondParameter), 
          _attribute);

      helper.GetTypeOfSecurableObject ();
    }

    [Test]
    [ExpectedException (typeof (WxeException), "WxeFunction 'Rubicon.Security.Web.UnitTests.ExecutionEngine.TestFunctionWithoutParameters' has"
        + " a WxeDemandMethodPermissionAttribute applied, but does not define any parameters to supply the 'this-object'.")]
    public void TestFromFunctionWithoutParameters ()
    {
      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithoutParameters),
          _attribute);

      helper.GetTypeOfSecurableObject ();
    }

    [Test]
    [ExpectedException (typeof (WxeException), "The parameter 'Invalid' specified by the WxeDemandMethodPermissionAttribute applied to"
        + " WxeFunction 'Rubicon.Security.Web.UnitTests.ExecutionEngine.TestFunctionWithThisObjectAsSecondParameter' is not a valid parameter"
        + " of this function.")]
    public void TestWithInvalidParameterName ()
    {
      _attribute.ParameterName = "Invalid";

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObjectAsSecondParameter),
          _attribute);

      helper.GetTypeOfSecurableObject ();
    }
  }
}