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
  public class WxeDemandMethodPermissionAttributeHelperTestForGetSecurableObject
  {
    // types

    // static members

    // member fields
    private Mockery _mocks;
    private WxeDemandTargetMethodPermissionAttribute _attribute;
    private TestFunctionWithThisObjectAsSecondParameter _functionWithThisObjectAsSecondParamter;

    // construction and disposing

    public WxeDemandMethodPermissionAttributeHelperTestForGetSecurableObject ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _mocks = new Mockery ();
      _attribute = new WxeDemandTargetMethodPermissionAttribute ("Read");

      object someObject = new object ();
      SecurableObject thisObject = new SecurableObject (null);
      _functionWithThisObjectAsSecondParamter = new TestFunctionWithThisObjectAsSecondParameter (someObject, thisObject);
      _functionWithThisObjectAsSecondParamter.SomeObject = someObject; // Required because in this test the WxeFunction has not started executing.
      _functionWithThisObjectAsSecondParamter.ThisObject = thisObject; // Required because in this test the WxeFunction has not started executing.
    }

    [Test]
    public void TestWithValidParameterName ()
    {
      _attribute.ParameterName = "ThisObject";

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          _functionWithThisObjectAsSecondParamter.GetType (),
          _attribute);

      Assert.AreSame (_functionWithThisObjectAsSecondParamter.ThisObject, helper.GetSecurableObject (_functionWithThisObjectAsSecondParamter));
    }

    [Test]
    public void TestWithDefaultParameter ()
    {
      SecurableObject thisObject = new SecurableObject (null);
      TestFunctionWithThisObject function = new TestFunctionWithThisObject (thisObject, null);
      function.ThisObject = thisObject; // Required because in this test the WxeFunction has not started executing.

      _attribute.ParameterName = null;

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          function.GetType (),
          _attribute);

      Assert.AreSame (function.ThisObject, helper.GetSecurableObject (function));
    }

    [Test]
    [ExpectedException (typeof (WxeException), "The parameter 'ThisObject' specified by the WxeDemandTargetMethodPermissionAttribute applied to"
        + " WxeFunction 'Rubicon.Security.Web.UnitTests.ExecutionEngine.TestFunctionWithThisObject' is null or does not implement"
        + " interface 'Rubicon.Security.ISecurableObject'.")]
    public void TestWithParameterNull ()
    {
      _attribute.ParameterName = "ThisObject";
      TestFunctionWithThisObject function = new TestFunctionWithThisObject (null, null);

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          function.GetType (),
          _attribute);

      helper.GetSecurableObject (function);
    }

    [Test]
    [ExpectedException (typeof (WxeException), "The parameter 'SomeObject' specified by the WxeDemandTargetMethodPermissionAttribute applied to"
        + " WxeFunction 'Rubicon.Security.Web.UnitTests.ExecutionEngine.TestFunctionWithThisObjectAsSecondParameter' is null or does not implement"
        + " interface 'Rubicon.Security.ISecurableObject'.")]
    public void TestWithParameterNotImplementingISecurableObject ()
    {
      _attribute.ParameterName = "SomeObject";

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          _functionWithThisObjectAsSecondParamter.GetType (),
          _attribute);

      helper.GetSecurableObject (_functionWithThisObjectAsSecondParamter);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void TestWithInvalidFunctionType ()
    {
      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          _attribute);

      helper.GetSecurableObject (new TestFunctionWithoutPermissions ());
    }

    [Test]
    [ExpectedException (typeof (WxeException),
       "WxeFunction 'Rubicon.Security.Web.UnitTests.ExecutionEngine.TestFunctionWithoutParameters' has a WxeDemandTargetMethodPermissionAttribute"
       + " applied, but does not define any parameters to supply the 'this-object'.")]
    public void TestWithFunctionWithoutParameters ()
    {
      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithoutParameters),
          _attribute);

      helper.GetSecurableObject (new TestFunctionWithoutParameters ());
    }

    [Test]
    [ExpectedException (typeof (WxeException), "The parameter 'Invalid' specified by the WxeDemandTargetMethodPermissionAttribute applied to"
        + " WxeFunction 'Rubicon.Security.Web.UnitTests.ExecutionEngine.TestFunctionWithThisObjectAsSecondParameter' is not a valid parameter"
        + " of this function.")]
    public void TestWithInvalidParameterName ()
    {
      _attribute.ParameterName = "Invalid";

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          _functionWithThisObjectAsSecondParamter.GetType (),
          _attribute);

      helper.GetSecurableObject (_functionWithThisObjectAsSecondParamter);
    }
  }
}