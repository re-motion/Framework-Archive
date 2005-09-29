using System;
using System.IO;
using System.Web;
using System.Web.SessionState;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Utilities;
using Rubicon.Collections;
using Rubicon.Web.UntTests.AspNetFramework;

namespace Rubicon.Web.UntTests.ExecutionEngine
{

[TestFixture]
public class WxeMethodStepTest: WxeTest
{
  private TestFunction _function;
  private TestFunctionWithInvalidSteps _functionWithInvalidSteps;

  [SetUp]
  public override void SetUp()
  {
    base.SetUp();

    _function = new TestFunction();
    _functionWithInvalidSteps = new TestFunctionWithInvalidSteps();
  }

  [Test]
  [ExpectedException (typeof (WxeException))]
  public void CheckCtorArgumentNotInstanceMember()
  {
    Type functionType = typeof (TestFunctionWithInvalidSteps);
    MethodInfo step1 = functionType.GetMethod ("InvalidStep1", BindingFlags.Static | BindingFlags.NonPublic);
    WxeMethodStep methodStep = new WxeMethodStep (_functionWithInvalidSteps, step1);

    Assert.Fail();
  }

  [Test]
  [ExpectedException (typeof (WxeException))]
  public void CheckCtorArgumentWrongParameterType()
  {
    Type functionType = typeof (TestFunctionWithInvalidSteps);
    MethodInfo step2 = functionType.GetMethod ("InvalidStep2", BindingFlags.Instance | BindingFlags.NonPublic);
    WxeMethodStep methodStep = new WxeMethodStep (_functionWithInvalidSteps, step2);

    Assert.Fail();
  }

  [Test]
  [ExpectedException (typeof (WxeException))]
  public void CheckCtorArgumentTooManyParameters()
  {
    Type functionType = typeof (TestFunctionWithInvalidSteps);
    MethodInfo step3 = functionType.GetMethod ("InvalidStep3", BindingFlags.Instance | BindingFlags.NonPublic);
    WxeMethodStep methodStep = new WxeMethodStep (_functionWithInvalidSteps, step3);

    Assert.Fail();
  }

  [Test]
  [ExpectedException (typeof (WxeException))]
  public void CheckCtorArgumentWrongStepListInstance()
  {
    Type functionType = typeof (TestFunction);
    MethodInfo step1 = functionType.GetMethod ("Step1", BindingFlags.Instance | BindingFlags.NonPublic);
    WxeMethodStep methodStep = new WxeMethodStep (_functionWithInvalidSteps, step1);

    Assert.Fail();
  }

  [Test]
  public void ExecuteMethodStep()
  {
    Type functionType = typeof (TestFunction);
    MethodInfo step1 = functionType.GetMethod ("Step1", BindingFlags.Instance | BindingFlags.NonPublic);
    WxeMethodStep methodStep = new WxeMethodStep (_function, step1);
    
    methodStep.Execute (CurrentWxeContext);
    
    Assert.AreEqual ("1", _function.LastExecutedStepID);
  }

  [Test]
  public void ExecuteMethodStepWithoutContext()
  {
    Type functionType = typeof (TestFunction);
    MethodInfo step2 = functionType.GetMethod ("Step2", BindingFlags.Instance | BindingFlags.NonPublic);
    WxeMethodStep methodStepWithContext = new WxeMethodStep (_function, step2);
    
    methodStepWithContext.Execute (CurrentWxeContext);
    
    Assert.AreEqual ("2", _function.LastExecutedStepID);
    Assert.AreSame (CurrentWxeContext, _function.WxeContextStep2);
  }
}

}
