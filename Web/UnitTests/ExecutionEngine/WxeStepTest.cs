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
using Rubicon.Web.Test.AspNetFramework;

namespace Rubicon.Web.Test.ExecutionEngine
{

[TestFixture]
public class WxeStepTest
{
  private TestFunction _rootFunction;
  private TestFunction _nestedLevel1Function;
  private TestFunction _nestedLevel2Function;
  private TestStep _rootFunctionStep;
  private TestStep _nestedLevel1FunctionStep;
  private TestStep _nestedLevel2FunctionStep;
  private TestStep _standAloneStep;

  [SetUp]
  public virtual void SetUp()
  {
    _rootFunction = new TestFunction();
    _nestedLevel1Function = new TestFunction();
    _nestedLevel2Function = new TestFunction();
    _rootFunctionStep = new TestStep();
    _nestedLevel1FunctionStep = new TestStep();
    _nestedLevel2FunctionStep = new TestStep();
    _standAloneStep = new TestStep();

    _rootFunction.Add (new TestStep());
    _rootFunction.Add (new TestStep());
    _rootFunction.Add (_nestedLevel1Function);
    _rootFunction.Add (_rootFunctionStep);

    _nestedLevel1Function.Add (new TestStep());
    _nestedLevel1Function.Add (new TestStep());
    _nestedLevel1Function.Add (_nestedLevel2Function);
    _nestedLevel1Function.Add (_nestedLevel1FunctionStep);

    _nestedLevel2Function.Add (new TestStep());
    _nestedLevel2Function.Add (new TestStep());
    _nestedLevel2Function.Add (_nestedLevel2FunctionStep);
  }

  [TearDown]
  public virtual void TearDown()
  {
  }

  [Test]
  public void GetFunctionForStep()
  {
    WxeFunction function = WxeStep.GetFunction (_nestedLevel1FunctionStep);
    Assert.AreSame (_nestedLevel1Function, function);    
  }

  [Test]
  public void GetFunctionForNestedFunction()
  {
    WxeFunction function = WxeStep.GetFunction (_nestedLevel1Function);
    Assert.AreSame (_nestedLevel1Function, function);    
  }

  [Test]
  public void GetParentStepForStep()
  {
    WxeStep parentStep = _nestedLevel2FunctionStep.ParentStep;
    Assert.AreSame (_nestedLevel2Function, parentStep);    
  }

  [Test]
  public void GetParentFunctionForStep()
  {
    WxeFunction parentFunction = _nestedLevel2FunctionStep.ParentFunction;
    Assert.AreSame (_nestedLevel2Function, parentFunction);    
  }

  [Test]
  public void GetParentFunctionForNestedFunction()
  {
    WxeFunction parentFunction = _nestedLevel2Function.ParentFunction;
    Assert.AreSame (_nestedLevel1Function, parentFunction);    
  }

  [Test]
  public void GetParentStepForStandAloneStep()
  {
    WxeStep parentStep = _standAloneStep.ParentStep;
    Assert.IsNull(parentStep);    
  }

  [Test]
  public void GetParentFunctionForStandAloneStep()
  {
    WxeFunction parentFunction = _standAloneStep.ParentFunction;
    Assert.IsNull (parentFunction);    
  }

  [Test]
  public void GetRootFunctionForStep()
  {
    WxeFunction rootFunction = _nestedLevel2FunctionStep.RootFunction;
    Assert.AreSame (_rootFunction, rootFunction);    
  }

  [Test]
  public void GetRootFunctionForStandAloneStep()
  {
    WxeFunction rootFunction = _standAloneStep.RootFunction;
    Assert.IsNull (rootFunction);    
  }

  [Test]
  public void GetRootFunctionForRootFunction()
  {
    WxeFunction rootFunction = _rootFunction.RootFunction;
    Assert.AreSame (_rootFunction, rootFunction);    
  }
}

}
