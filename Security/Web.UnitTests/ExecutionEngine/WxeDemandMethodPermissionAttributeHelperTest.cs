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
      WxeDemandTargetMethodPermissionAttribute attribute = new WxeDemandTargetMethodPermissionAttribute ("Show");

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      Assert.AreSame (typeof (TestFunctionWithThisObject), helper.FunctionType);
      Assert.AreEqual (MethodType.Instance, helper.MethodType);
      Assert.IsNull (helper.SecurableClass);
      Assert.AreEqual ("Show", helper.MethodName);
    }


    [Test]
    public void InitializeWithMethodTypeStatic ()
    {
      WxeDemandTargetStaticMethodPermissionAttribute attribute = new WxeDemandTargetStaticMethodPermissionAttribute ("Search", typeof (SecurableObject));

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      Assert.AreSame (typeof (TestFunctionWithThisObject), helper.FunctionType);
      Assert.AreEqual (MethodType.Static, helper.MethodType);
      Assert.AreSame (typeof (SecurableObject), helper.SecurableClass);
      Assert.AreEqual ("Search", helper.MethodName);
    }

    [Test]
    public void InitializeWithMethodTypeStaticAndMethodEnum ()
    {
      WxeDemandTargetStaticMethodPermissionAttribute attribute = new WxeDemandTargetStaticMethodPermissionAttribute (SecurableObject.Methods.Search);

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      Assert.AreSame (typeof (TestFunctionWithThisObject), helper.FunctionType);
      Assert.AreEqual (MethodType.Static, helper.MethodType);
      Assert.AreSame (typeof (SecurableObject), helper.SecurableClass);
      Assert.AreEqual ("Search", helper.MethodName);
    }

    [Test]
    public void InitializeWithMethodTypeStaticAndMethodEnumFromBaseClass ()
    {
      WxeDemandTargetStaticMethodPermissionAttribute attribute = 
          new WxeDemandTargetStaticMethodPermissionAttribute (SecurableObject.Methods.Search, typeof (DerivedSecurableObject));

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      Assert.AreSame (typeof (TestFunctionWithThisObject), helper.FunctionType);
      Assert.AreEqual (MethodType.Static, helper.MethodType);
      Assert.AreSame (typeof (DerivedSecurableObject), helper.SecurableClass);
      Assert.AreEqual ("Search", helper.MethodName);
    }


    [Test]
    public void InitializeWithMethodTypeConstructor ()
    {
      WxeDemandTargetPermissionAttribute attribute = new WxeDemandCreatePermissionAttribute (typeof (SecurableObject));

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      Assert.AreSame (typeof (TestFunctionWithThisObject), helper.FunctionType);
      Assert.AreEqual (MethodType.Constructor, helper.MethodType);
      Assert.AreSame (typeof (SecurableObject), helper.SecurableClass);
      Assert.IsNull (helper.MethodName);
    }
  }
}