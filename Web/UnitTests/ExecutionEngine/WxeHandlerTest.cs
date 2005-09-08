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
public class WxeHandlerTest
{
  private HttpContext _httpContext;
  private WxeContextMock _wxeContext;
  private WxeHandlerMock _wxeHandler;

  protected const string c_functionTokenForFunctionStateWithEnabledCleanUp = "00000000-Enabled-CleanUp";
  protected const string c_functionTokenForFunctionStateWithDisabledCleanUp = "00000000-Disabled-CleanUp";
  protected const string c_functionTokenForFunctionStateWithMissingFunction = "00000000-Missing-Function";
  protected const string c_functionTokenForMissingFunctionState = "00000000-Missing-FunctionState";
  protected const string c_functionTokenForAbortedFunctionState = "00000000-Aborted";
  protected const string c_functionTokenForExpiredFunctionState = "00000000-Expired";
  protected const string c_functionTokenForNewFunctionState = "00000000-New";
  protected const string c_functionTokenForFunctionStateWithChildFunction = "00000000-Has-ChildFunction";

  private TestWxeFunctionState _functionStateWithEnabledCleanUp;
  private TestWxeFunctionState _functionStateWithDisabledCleanUp;
  private TestWxeFunctionState _functionStateWithMissingFunction;
  private TestWxeFunctionState _functionStateAborted;
  private TestWxeFunctionState _functionStateExpired;
  private TestWxeFunctionState _functionStateWithChildFunction;

  private Type _functionType;
  private string _functionTypeName;
  private string _invalidFunctionTypeName;

  private string _returnUrl = "newReturnUrl.html";

  [SetUp]
  public virtual void SetUp()
  {
    _httpContext = HttpContextHelper.CreateHttpContext (@"C:\default.html", @"http://localhost/default.html", null);
    HttpContextHelper.SetCurrent (_httpContext);

    _wxeContext = new WxeContextMock (_httpContext);
    PrivateInvoke.InvokeNonPublicStaticMethod (typeof (WxeContext), "SetCurrent", _wxeContext);

    _wxeHandler = new WxeHandlerMock();

    WxeFunctionStateCollection.Instance = new WxeFunctionStateCollection();

    _functionStateWithEnabledCleanUp = 
        new TestWxeFunctionState (new TestFunction(), c_functionTokenForFunctionStateWithEnabledCleanUp, 10, true);
    WxeFunctionStateCollection.Instance.Add (_functionStateWithEnabledCleanUp);

    _functionStateWithDisabledCleanUp = 
        new TestWxeFunctionState (new TestFunction(), c_functionTokenForFunctionStateWithDisabledCleanUp, 10, false);
    WxeFunctionStateCollection.Instance.Add (_functionStateWithDisabledCleanUp);

    _functionStateWithMissingFunction = 
        new TestWxeFunctionState (new TestFunction(), c_functionTokenForFunctionStateWithMissingFunction, 10, false);
    _functionStateWithMissingFunction.Function = null;
    WxeFunctionStateCollection.Instance.Add (_functionStateWithMissingFunction);

    _functionStateAborted = 
        new TestWxeFunctionState (new TestFunction(), c_functionTokenForAbortedFunctionState, 10, true);
    WxeFunctionStateCollection.Instance.Add (_functionStateAborted);
    _functionStateAborted.Abort();

    _functionStateExpired = 
        new TestWxeFunctionState (new TestFunction(), c_functionTokenForExpiredFunctionState, 0, true);
    WxeFunctionStateCollection.Instance.Add (_functionStateExpired);

    TestFunction rootFunction = new TestFunction();
    TestFunction childFunction = new TestFunction();
    childFunction.ParentStep = rootFunction;
    _functionStateWithChildFunction = 
        new TestWxeFunctionState (childFunction, c_functionTokenForFunctionStateWithChildFunction, 10, true);
    WxeFunctionStateCollection.Instance.Add (_functionStateWithChildFunction);

    _functionType = typeof (TestFunction);
    _functionTypeName = _functionType.AssemblyQualifiedName;
    _invalidFunctionTypeName = "Rubicon.Web.Test::ExecutionEngine.InvalidFunction";

    Thread.Sleep (20);
  }

  [TearDown]
  public virtual void TearDown()
  {
    WxeFunctionStateCollection.Instance.Abort (_functionStateWithEnabledCleanUp);
    WxeFunctionStateCollection.Instance.Abort (_functionStateWithDisabledCleanUp);
    WxeFunctionStateCollection.Instance.Abort (_functionStateWithMissingFunction);
    WxeFunctionStateCollection.Instance.Abort (_functionStateAborted);
    WxeFunctionStateCollection.Instance.Abort (_functionStateExpired);
    WxeFunctionStateCollection.Instance.Abort (_functionStateWithChildFunction);
  }

  [Test]
  public void CreateNewFunctionStateStateWithFunctionToken()
  {
    WxeFunctionState functionState = 
        _wxeHandler.CreateNewFunctionState (_httpContext, _functionTypeName, c_functionTokenForNewFunctionState);

    Assert.IsNotNull (functionState);
    Assert.AreEqual (c_functionTokenForNewFunctionState, functionState.FunctionToken);
    Assert.IsNotNull (functionState.Function);
    Assert.AreEqual (_functionType, functionState.Function.GetType());
    Assert.AreEqual (TestFunction.ReturnUrlValue, functionState.Function.ReturnUrl);

    WxeFunctionState expiredFunctionState =
        WxeFunctionStateCollection.Instance.GetItem (c_functionTokenForExpiredFunctionState);
    Assert.IsNull (expiredFunctionState);
  }

  [Test]
  public void CreateNewFunctionStateStateWithoutFunctionToken()
  {
    WxeFunctionState functionState = 
        _wxeHandler.CreateNewFunctionState (_httpContext, _functionTypeName, null);

    Assert.IsNotNull (functionState);
    Assert.IsNotNull (functionState.FunctionToken);
    Assert.IsNotNull (functionState.Function);
    Assert.AreEqual (_functionType, functionState.Function.GetType());
    Assert.AreEqual (TestFunction.ReturnUrlValue, functionState.Function.ReturnUrl);

    WxeFunctionState expiredFunctionState =
        WxeFunctionStateCollection.Instance.GetItem (c_functionTokenForExpiredFunctionState);
    Assert.IsNull (expiredFunctionState);
  }

  [Test]
  [ExpectedException (typeof (ArgumentException))]
  public void CreateNewFunctionStateStateWithInvalidFunctionType()
  {
    WxeFunctionState functionState = 
        _wxeHandler.CreateNewFunctionState (_httpContext, _invalidFunctionTypeName, null);

    Assert.Fail();
  }

  [Test]
  public void CreateNewFunctionStateStateWithReturnUrl()
  {
    NameValueCollection parameters = new NameValueCollection();
    parameters.Set (WxeHandler.Parameters.WxeReturnUrl, _returnUrl);
    HttpContextHelper.SetParams (_httpContext, parameters);

    WxeFunctionState functionState = 
        _wxeHandler.CreateNewFunctionState (_httpContext, _functionTypeName, null);

    Assert.IsNotNull (functionState);
    Assert.IsNotNull (functionState.FunctionToken);
    Assert.IsNotNull (functionState.Function);
    Assert.AreEqual (_functionType, functionState.Function.GetType());
    Assert.AreEqual (_returnUrl, functionState.Function.ReturnUrl);
  
    WxeFunctionState expiredFunctionState =
        WxeFunctionStateCollection.Instance.GetItem (c_functionTokenForExpiredFunctionState);
    Assert.IsNull (expiredFunctionState);
  }

  [Test]
  public void CreateNewFunctionStateStateWithArgument()
  {
    string agrumentValue = "True";
    NameValueCollection parameters = new NameValueCollection();
    parameters.Set (TestFunction.Parameter1Name, agrumentValue);
    HttpContextHelper.SetParams (_httpContext, parameters);

    WxeFunctionState functionState = 
        _wxeHandler.CreateNewFunctionState (_httpContext, _functionTypeName, null);

    Assert.IsNotNull (functionState);
    Assert.IsNotNull (functionState.FunctionToken);
    Assert.IsNotNull (functionState.Function);
    Assert.AreEqual (_functionType, functionState.Function.GetType());
    TestFunction testFunction = (TestFunction) functionState.Function;
    Assert.AreEqual (agrumentValue, testFunction.Parameter1);

    WxeFunctionState expiredFunctionState =
        WxeFunctionStateCollection.Instance.GetItem (c_functionTokenForExpiredFunctionState);
    Assert.IsNull (expiredFunctionState);
  }

	[Test]
  public void RetrieveExistingFunctionState()
  {
    DateTime timeBeforeRefresh = DateTime.Now;
    Thread.Sleep (20);

    WxeFunctionState functionState = 
        _wxeHandler.ResumeExistingFunctionState (_httpContext, c_functionTokenForFunctionStateWithEnabledCleanUp);
    
    Assert.AreSame (_functionStateWithEnabledCleanUp, functionState);
    Assert.IsTrue (_functionStateWithEnabledCleanUp.LastAccess > timeBeforeRefresh);
    Assert.IsFalse (functionState.IsAborted);
    Assert.IsFalse (functionState.IsExpired);

    WxeFunctionState expiredFunctionState =
        WxeFunctionStateCollection.Instance.GetItem (c_functionTokenForExpiredFunctionState);
    Assert.IsNull (expiredFunctionState);
  }

	[Test]
  [ExpectedException (typeof (ApplicationException))]
  public void RetrieveMissingFunctionState()
  {
    WxeFunctionState functionState = 
        _wxeHandler.ResumeExistingFunctionState (_httpContext, c_functionTokenForMissingFunctionState);
    Assert.Fail();
  }

	[Test]
  [ExpectedException (typeof (ApplicationException))]
  public void RetrieveFunctionStateWithMissingFunction()
  {
    WxeFunctionState functionState = 
        _wxeHandler.ResumeExistingFunctionState (_httpContext, c_functionTokenForFunctionStateWithMissingFunction);
    Assert.Fail();
  }

	[Test]
  [ExpectedException (typeof (ApplicationException))]
  public void RetrieveExpiredFunctionState()
  {
    WxeFunctionState functionState = 
        _wxeHandler.ResumeExistingFunctionState (_httpContext, c_functionTokenForExpiredFunctionState);
    Assert.Fail();
  }

	[Test]
  [ExpectedException (typeof (InvalidOperationException))]
  public void RetrieveAbortedFunctionState()
  {
    WxeFunctionState functionState = 
        _wxeHandler.ResumeExistingFunctionState (_httpContext, c_functionTokenForAbortedFunctionState);
    Assert.Fail();
  }

	[Test]
  public void RefreshExistingFunctionState()
  {
    NameValueCollection parameters = new NameValueCollection();
    parameters.Set (WxeHandler.Parameters.WxeAction, WxeHandler.Actions.Refresh);
    HttpContextHelper.SetParams (_httpContext, parameters);

    DateTime timeBeforeRefresh = DateTime.Now;
    Thread.Sleep (20);

    WxeFunctionState functionState = 
        _wxeHandler.ResumeExistingFunctionState (_httpContext, c_functionTokenForFunctionStateWithEnabledCleanUp);

    Assert.IsNull (functionState);
    Assert.IsTrue (_functionStateWithEnabledCleanUp.LastAccess > timeBeforeRefresh);
    Assert.IsFalse (_functionStateWithEnabledCleanUp.IsAborted);
    Assert.IsFalse (_functionStateWithEnabledCleanUp.IsExpired);
  }

	[Test]
  public void RefreshExistingFunctionStateWithMissingFunction()
  {
    NameValueCollection parameters = new NameValueCollection();
    parameters.Set (WxeHandler.Parameters.WxeAction, WxeHandler.Actions.Refresh);
    HttpContextHelper.SetParams (_httpContext, parameters);

    DateTime timeBeforeRefresh = DateTime.Now;
    Thread.Sleep (20);

    WxeFunctionState functionState = 
        _wxeHandler.ResumeExistingFunctionState (_httpContext, c_functionTokenForFunctionStateWithMissingFunction);

    Assert.IsNull (functionState);
    Assert.IsTrue (_functionStateWithMissingFunction.LastAccess > timeBeforeRefresh);
    Assert.IsFalse (_functionStateWithMissingFunction.IsAborted);
    Assert.IsFalse (_functionStateWithMissingFunction.IsExpired);
  }

	[Test]
  public void AbortExistingFunctionState()
  {
    NameValueCollection parameters = new NameValueCollection();
    parameters.Set (WxeHandler.Parameters.WxeAction, WxeHandler.Actions.Abort);
    HttpContextHelper.SetParams (_httpContext, parameters);

    WxeFunctionState functionState = 
        _wxeHandler.ResumeExistingFunctionState (_httpContext, c_functionTokenForFunctionStateWithEnabledCleanUp);

    Assert.IsNull (functionState);
    Assert.IsTrue (_functionStateWithEnabledCleanUp.IsAborted);

    WxeFunctionState expiredFunctionState = 
        WxeFunctionStateCollection.Instance.GetItem (c_functionTokenForExpiredFunctionState);
    Assert.IsNull (expiredFunctionState);
  }

	[Test]
  public void AbortExistingFunctionStateMissingFunction()
  {
    NameValueCollection parameters = new NameValueCollection();
    parameters.Set (WxeHandler.Parameters.WxeAction, WxeHandler.Actions.Abort);
    HttpContextHelper.SetParams (_httpContext, parameters);

    WxeFunctionState functionState = 
        _wxeHandler.ResumeExistingFunctionState (_httpContext, c_functionTokenForFunctionStateWithMissingFunction);

    Assert.IsNull (functionState);
    Assert.IsTrue (_functionStateWithMissingFunction.IsAborted);
  }

  [Test]
  public void CleanUpFunctionStateWithEnabledCleanUp()
  {
    _wxeHandler.CleanUpFunctionState (_functionStateWithEnabledCleanUp);
    Assert.IsTrue (_functionStateWithEnabledCleanUp.IsAborted);
  }

  [Test]
  public void CleanUpFunctionStateWithDisabledCleanUp()
  {
    _wxeHandler.CleanUpFunctionState (_functionStateWithDisabledCleanUp);
    Assert.IsFalse (_functionStateWithEnabledCleanUp.IsAborted);
  }

  [Test]
  public void CleanUpFunctionStateWithChildFunction()
  {
    _wxeHandler.CleanUpFunctionState (_functionStateWithChildFunction);
    Assert.IsFalse (_functionStateWithChildFunction.IsAborted);
  }

  [Test]
  public void ExecuteFunctionState()
  {
    _wxeHandler.ExecuteFunctionState (_httpContext, _functionStateWithEnabledCleanUp, true);
    TestFunction function = (TestFunction) _functionStateWithEnabledCleanUp.Function;

    Assert.AreSame (WxeContext.Current,  function.WxeContext);
    Assert.AreEqual (_httpContext,  function.WxeContext.HttpContext);
    Assert.AreEqual (_functionStateWithEnabledCleanUp.FunctionToken, function.WxeContext.FunctionToken);
    Assert.AreEqual (_httpContext, function.WxeContext.HttpContext);
    Assert.AreEqual ("4", function.LastExecutedStepID);
  }

  [Test]
  [ExpectedException (typeof (InvalidOperationException))]
  public void ExecuteAbortedFunctionState()
  {
    _wxeHandler.ExecuteFunctionState (_httpContext, _functionStateAborted, true);
    Assert.Fail();
  }

  [Test]
  public void ExecuteFunction()
  {
    TestFunction function = (TestFunction) _functionStateWithEnabledCleanUp.Function;
    _wxeHandler.ExecuteFunction (function, _wxeContext, true);
    
    Assert.AreSame (_wxeContext, function.WxeContext);

    Type[] catchExceptionTypes = function.GetCatchExceptionTypes();
    Assert.AreEqual (1, catchExceptionTypes.Length);
    Assert.AreSame (typeof (WxeUserCancelException), catchExceptionTypes[0]);

    Assert.AreEqual ("4", function.LastExecutedStepID);
  }

  [Test]
  [ExpectedException (typeof (InvalidOperationException))]
  public void ExecuteAbortedFunction()
  {
    TestFunction function = (TestFunction) _functionStateAborted.Function;
    _wxeHandler.ExecuteFunction (function, _wxeContext, true);
    Assert.Fail();
  }
}

}
