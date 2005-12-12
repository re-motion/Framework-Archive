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
using Rubicon.Web.UnitTests.AspNetFramework;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{

[TestFixture]
public class WxeHandlerTest: WxeTest
{
  private WxeHandlerMock _wxeHandler;

  protected const string c_functionTokenForFunctionStateWithEnabledCleanUp = "00000000-Enabled-CleanUp";
  protected const string c_functionTokenForFunctionStateWithDisabledCleanUp = "00000000-Disabled-CleanUp";
  protected const string c_functionTokenForFunctionStateWithMissingFunction = "00000000-Missing-Function";
  protected const string c_functionTokenForMissingFunctionState = "00000000-Missing-FunctionState";
  protected const string c_functionTokenForAbortedFunctionState = "00000000-Aborted";
  protected const string c_functionTokenForExpiredFunctionState = "00000000-Expired";
  protected const string c_functionTokenForNewFunctionState = "00000000-New";
  protected const string c_functionTokenForFunctionStateWithChildFunction = "00000000-Has-ChildFunction";

  private WxeFunctionStateMock _functionStateWithEnabledCleanUp;
  private WxeFunctionStateMock _functionStateWithDisabledCleanUp;
  private WxeFunctionStateMock _functionStateWithMissingFunction;
  private WxeFunctionStateMock _functionStateAborted;
  private WxeFunctionStateMock _functionStateExpired;
  private WxeFunctionStateMock _functionStateWithChildFunction;

  private Type _functionType;
  private string _functionTypeName;
  private string _invalidFunctionTypeName;

  private string _returnUrl = "newReturnUrl.html";

  [SetUp]
  public override void SetUp()
  {
    base.SetUp();

    _wxeHandler = new WxeHandlerMock();

    WxeFunctionStateCollection.Instance = new WxeFunctionStateCollection();

    _functionStateWithEnabledCleanUp = new WxeFunctionStateMock (
        new TestFunction(), 10, true, c_functionTokenForFunctionStateWithEnabledCleanUp);
    WxeFunctionStateCollection.Instance.Add (_functionStateWithEnabledCleanUp);

    _functionStateWithDisabledCleanUp = new WxeFunctionStateMock (
        new TestFunction(), 10, false,c_functionTokenForFunctionStateWithDisabledCleanUp);
    WxeFunctionStateCollection.Instance.Add (_functionStateWithDisabledCleanUp);

    _functionStateWithMissingFunction = new WxeFunctionStateMock (
        new TestFunction(), 10, false,c_functionTokenForFunctionStateWithMissingFunction);
    _functionStateWithMissingFunction.Function = null;
    WxeFunctionStateCollection.Instance.Add (_functionStateWithMissingFunction);

    _functionStateAborted = new WxeFunctionStateMock (
        new TestFunction(), 10, true,c_functionTokenForAbortedFunctionState);
    WxeFunctionStateCollection.Instance.Add (_functionStateAborted);
    _functionStateAborted.Abort();

    _functionStateExpired = new WxeFunctionStateMock (
        new TestFunction(), 0, true,c_functionTokenForExpiredFunctionState);
    WxeFunctionStateCollection.Instance.Add (_functionStateExpired);

    TestFunction rootFunction = new TestFunction();
    TestFunction childFunction = new TestFunction();
    rootFunction.Add (childFunction);
    _functionStateWithChildFunction = new WxeFunctionStateMock (
        childFunction, 10, true, c_functionTokenForFunctionStateWithChildFunction);
    WxeFunctionStateCollection.Instance.Add (_functionStateWithChildFunction);

    _functionType = typeof (TestFunction);
    _functionTypeName = _functionType.AssemblyQualifiedName;
    _invalidFunctionTypeName = "Rubicon.Web.UnitTests::ExecutionEngine.InvalidFunction";

    Thread.Sleep (20);
    Rubicon.Web.ExecutionEngine.UrlMapping.UrlMappingConfiguration.SetCurrent (null);
  }

  [TearDown]
  public override void TearDown()
  {
    WxeFunctionStateCollection.Instance.Abort (_functionStateWithEnabledCleanUp);
    WxeFunctionStateCollection.Instance.Abort (_functionStateWithDisabledCleanUp);
    WxeFunctionStateCollection.Instance.Abort (_functionStateWithMissingFunction);
    WxeFunctionStateCollection.Instance.Abort (_functionStateAborted);
    WxeFunctionStateCollection.Instance.Abort (_functionStateExpired);
    WxeFunctionStateCollection.Instance.Abort (_functionStateWithChildFunction);
 
    Rubicon.Web.ExecutionEngine.UrlMapping.UrlMappingConfiguration.SetCurrent (null);
    base.TearDown();
  }

  [Test]
  public void CreateNewFunctionStateStateWithFunctionToken()
  {
    WxeFunctionState functionState = _wxeHandler.CreateNewFunctionState (
        CurrentHttpContext, _functionType);

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
  public void CreateNewFunctionStateStateWithoutFunctionToken()
  {
    WxeFunctionState functionState = 
        _wxeHandler.CreateNewFunctionState (CurrentHttpContext, _functionType);

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
  [ExpectedException (typeof (HttpException))]
  public void GetFunctionTypeWithInvalidTypeName()
  {
    _wxeHandler.GetTypeByTypeName (_invalidFunctionTypeName);

    Assert.Fail();
  }

  [Test]
  public void GetFunctionTypeByTypeName()
  {
    Type type = _wxeHandler.GetTypeByTypeName (_functionTypeName);
    Assert.IsNotNull (type);
    Assert.AreEqual (_functionType, type);
  }

  [Test]
  public void GetFunctionTypeByPath()
  {
    Rubicon.Web.ExecutionEngine.UrlMapping.UrlMappingConfiguration.Current.Mappings.Add (
        new Rubicon.Web.ExecutionEngine.UrlMapping.UrlMapping (_functionType, "~/Test.wxe"));

    Type type = _wxeHandler.GetTypeByPath (@"/Test.wxe");

    Assert.IsNotNull (type);
    Assert.AreEqual (_functionType, type);
  }

  [Test]
  [ExpectedException (typeof (HttpException))]
  public void GetFunctionTypeByPathWithoutMapping()
  {
    _wxeHandler.GetTypeByPath (@"/Test1.wxe");
    Assert.Fail();
  }

  [Test]
  public void CreateNewFunctionStateStateWithReturnUrl()
  {
    NameValueCollection queryString = new NameValueCollection();
    queryString.Set (WxeHandler.Parameters.ReturnUrl, _returnUrl);
    HttpContextHelper.SetQueryString (CurrentHttpContext, queryString);

    WxeFunctionState functionState = _wxeHandler.CreateNewFunctionState (CurrentHttpContext, _functionType);

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
    NameValueCollection queryString = new NameValueCollection();
    queryString.Set (TestFunction.Parameter1Name, agrumentValue);
    HttpContextHelper.SetQueryString (CurrentHttpContext, queryString);

    WxeFunctionState functionState = _wxeHandler.CreateNewFunctionState (CurrentHttpContext, _functionType);

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
        _wxeHandler.ResumeExistingFunctionState (CurrentHttpContext, c_functionTokenForFunctionStateWithEnabledCleanUp);
    
    Assert.AreSame (_functionStateWithEnabledCleanUp, functionState);
    Assert.IsTrue (_functionStateWithEnabledCleanUp.LastAccess > timeBeforeRefresh);
    Assert.IsFalse (functionState.IsAborted);
    Assert.IsFalse (functionState.IsExpired);

    WxeFunctionState expiredFunctionState =
        WxeFunctionStateCollection.Instance.GetItem (c_functionTokenForExpiredFunctionState);
    Assert.IsNull (expiredFunctionState);
  }

	[Test]
  [ExpectedException (typeof (HttpException))]
  public void RetrieveMissingFunctionStateWithNoType()
  {
    NameValueCollection form = new NameValueCollection();
    form.Set (WxeHandler.Parameters.WxeFunctionToken, c_functionTokenForMissingFunctionState);
    HttpContextHelper.SetForm (CurrentHttpContext, form);

    _wxeHandler.ResumeExistingFunctionState (CurrentHttpContext, c_functionTokenForMissingFunctionState);
    Assert.Fail();
  }

	[Test]
  public void RetrieveMissingFunctionStateWithTypeFromMapping()
  {
    HttpContext context = HttpContextHelper.CreateHttpContext ("GET", "Test.wxe", null);

    Rubicon.Web.ExecutionEngine.UrlMapping.UrlMappingConfiguration.Current.Mappings.Add (
        new Rubicon.Web.ExecutionEngine.UrlMapping.UrlMapping (_functionType, "~/Test.wxe"));

    WxeFunctionState functionState = 
        _wxeHandler.ResumeExistingFunctionState (context, c_functionTokenForMissingFunctionState);

    Assert.IsNotNull (functionState);
    Assert.AreEqual (_functionType, functionState.Function.GetType());
  }

	[Test]
  [ExpectedException (typeof (HttpException))]
  public void RetrieveMissingFunctionStateWithTypeFromMappingAndGetRequestWithPostBackAction()
  {
    HttpContext context = HttpContextHelper.CreateHttpContext ("GET", "Test.wxe", null);
    NameValueCollection queryString = new NameValueCollection();
    queryString.Add (WxeHandler.Parameters.WxeAction, WxeHandler.Actions.Refresh);
    HttpContextHelper.SetQueryString (context, queryString);

    Rubicon.Web.ExecutionEngine.UrlMapping.UrlMappingConfiguration.Current.Mappings.Add (
        new Rubicon.Web.ExecutionEngine.UrlMapping.UrlMapping (typeof (TestFunction), "~/Test.wxe"));

    _wxeHandler.ResumeExistingFunctionState (context, c_functionTokenForMissingFunctionState);
    Assert.Fail();
  }

	[Test]
  [ExpectedException (typeof (HttpException))]
  public void RetrieveMissingFunctionStateWithTypeFromMappingAndPostRequest()
  {
    HttpContext context = HttpContextHelper.CreateHttpContext ("POST", "Test.wxe", null);
    NameValueCollection form = new NameValueCollection();
    form.Add (WxeHandler.Parameters.WxeFunctionToken, c_functionTokenForMissingFunctionState);
    HttpContextHelper.SetForm (context, form);

    Rubicon.Web.ExecutionEngine.UrlMapping.UrlMappingConfiguration.Current.Mappings.Add (
        new Rubicon.Web.ExecutionEngine.UrlMapping.UrlMapping (typeof (TestFunction), "~/Test.wxe"));

    _wxeHandler.ResumeExistingFunctionState (context, c_functionTokenForMissingFunctionState);
    Assert.Fail();
  }

	[Test]
  public void RetrieveMissingFunctionStateWithTypeFromQueryString()
  {
    HttpContext context = HttpContextHelper.CreateHttpContext ("GET", "Test.wxe", null);
    NameValueCollection queryString = new NameValueCollection();
    queryString.Add (WxeHandler.Parameters.WxeFunctionType, _functionTypeName);
    HttpContextHelper.SetQueryString (context, queryString);

    Rubicon.Web.ExecutionEngine.UrlMapping.UrlMappingConfiguration.SetCurrent (null);

    WxeFunctionState functionState = 
        _wxeHandler.ResumeExistingFunctionState (context, c_functionTokenForMissingFunctionState);
    Assert.IsNotNull (functionState);
    Assert.AreEqual (typeof (TestFunction), functionState.Function.GetType());
  }

	[Test]
  [ExpectedException (typeof (HttpException))]
  public void RetrieveMissingFunctionStateWithTypeFromQueryStringAndGetRequestWithPostBackAction()
  {
    HttpContext context = HttpContextHelper.CreateHttpContext ("GET", "Test.wxe", null);
    NameValueCollection queryString = new NameValueCollection();
    queryString.Add (WxeHandler.Parameters.WxeFunctionType, _functionTypeName);
    queryString.Add (WxeHandler.Parameters.WxeAction, WxeHandler.Actions.Refresh);
    HttpContextHelper.SetQueryString (context, queryString);

    Rubicon.Web.ExecutionEngine.UrlMapping.UrlMappingConfiguration.SetCurrent (null);

    _wxeHandler.ResumeExistingFunctionState (context, c_functionTokenForMissingFunctionState);
    Assert.Fail();
  }

	[Test]
  [ExpectedException (typeof (HttpException))]
  public void RetrieveMissingFunctionStateWithTypeFromQueryStringAndPostRequest()
  {
    HttpContext context = HttpContextHelper.CreateHttpContext (
        "POST", "Test.wxe", null);
    NameValueCollection queryString = new NameValueCollection();
    queryString.Add (WxeHandler.Parameters.WxeFunctionType, _functionTypeName);
    HttpContextHelper.SetQueryString (context, queryString);

    Rubicon.Web.ExecutionEngine.UrlMapping.UrlMappingConfiguration.SetCurrent (null);

    _wxeHandler.ResumeExistingFunctionState (context, c_functionTokenForMissingFunctionState);
    Assert.Fail();
  }

	[Test]
  [ExpectedException (typeof (ApplicationException))]
  public void RetrieveFunctionStateWithMissingFunction()
  {
    _wxeHandler.ResumeExistingFunctionState (CurrentHttpContext, c_functionTokenForFunctionStateWithMissingFunction);
    Assert.Fail();
  }

	[Test]
  [ExpectedException (typeof (HttpException))]
  public void RetrieveExpiredFunctionState()
  {
    NameValueCollection form = new NameValueCollection();
    form.Set (WxeHandler.Parameters.WxeFunctionToken, c_functionTokenForExpiredFunctionState);
    HttpContextHelper.SetForm (CurrentHttpContext, form);

    _wxeHandler.ResumeExistingFunctionState (CurrentHttpContext, c_functionTokenForExpiredFunctionState);
    Assert.Fail();
  }

	[Test]
  [ExpectedException (typeof (InvalidOperationException))]
  public void RetrieveAbortedFunctionState()
  {
    NameValueCollection form = new NameValueCollection();
    form.Set (WxeHandler.Parameters.WxeFunctionToken, c_functionTokenForAbortedFunctionState);
    HttpContextHelper.SetForm (CurrentHttpContext, form);

    _wxeHandler.ResumeExistingFunctionState (CurrentHttpContext, c_functionTokenForAbortedFunctionState);
    Assert.Fail();
  }

	[Test]
  public void RefreshExistingFunctionState()
  {
    NameValueCollection queryString = new NameValueCollection();
    queryString.Set (WxeHandler.Parameters.WxeAction, WxeHandler.Actions.Refresh);
    HttpContextHelper.SetQueryString (CurrentHttpContext, queryString);

    DateTime timeBeforeRefresh = DateTime.Now;
    Thread.Sleep (20);

    WxeFunctionState functionState = 
        _wxeHandler.ResumeExistingFunctionState (CurrentHttpContext, c_functionTokenForFunctionStateWithEnabledCleanUp);

    Assert.IsNull (functionState);
    Assert.IsTrue (_functionStateWithEnabledCleanUp.LastAccess > timeBeforeRefresh);
    Assert.IsFalse (_functionStateWithEnabledCleanUp.IsAborted);
    Assert.IsFalse (_functionStateWithEnabledCleanUp.IsExpired);
  }

	[Test]
  public void RefreshExistingFunctionStateWithMissingFunction()
  {
    NameValueCollection queryString = new NameValueCollection();
    queryString.Set (WxeHandler.Parameters.WxeAction, WxeHandler.Actions.Refresh);
    HttpContextHelper.SetQueryString (CurrentHttpContext, queryString);

    DateTime timeBeforeRefresh = DateTime.Now;
    Thread.Sleep (20);

    WxeFunctionState functionState = 
        _wxeHandler.ResumeExistingFunctionState (CurrentHttpContext, c_functionTokenForFunctionStateWithMissingFunction);

    Assert.IsNull (functionState);
    Assert.IsTrue (_functionStateWithMissingFunction.LastAccess > timeBeforeRefresh);
    Assert.IsFalse (_functionStateWithMissingFunction.IsAborted);
    Assert.IsFalse (_functionStateWithMissingFunction.IsExpired);
  }

	[Test]
  public void AbortExistingFunctionState()
  {
    NameValueCollection queryString = new NameValueCollection();
    queryString.Set (WxeHandler.Parameters.WxeAction, WxeHandler.Actions.Abort);
    HttpContextHelper.SetQueryString (CurrentHttpContext, queryString);

    WxeFunctionState functionState = 
        _wxeHandler.ResumeExistingFunctionState (CurrentHttpContext, c_functionTokenForFunctionStateWithEnabledCleanUp);

    Assert.IsNull (functionState);
    Assert.IsTrue (_functionStateWithEnabledCleanUp.IsAborted);

    WxeFunctionState expiredFunctionState = 
        WxeFunctionStateCollection.Instance.GetItem (c_functionTokenForExpiredFunctionState);
    Assert.IsNull (expiredFunctionState);
  }

	[Test]
  public void AbortExistingFunctionStateMissingFunction()
  {
    NameValueCollection queryString = new NameValueCollection();
    queryString.Set (WxeHandler.Parameters.WxeAction, WxeHandler.Actions.Abort);
    HttpContextHelper.SetQueryString (CurrentHttpContext, queryString);

    WxeFunctionState functionState = 
        _wxeHandler.ResumeExistingFunctionState (CurrentHttpContext, c_functionTokenForFunctionStateWithMissingFunction);

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
    _wxeHandler.ExecuteFunctionState (CurrentHttpContext, _functionStateWithEnabledCleanUp, true);
    TestFunction function = (TestFunction) _functionStateWithEnabledCleanUp.Function;

    WxeContext wxeContext = function.TestStep.WxeContext;
    Assert.AreSame (WxeContext.Current, wxeContext);
    Assert.AreEqual (CurrentHttpContext, wxeContext.HttpContext);
    Assert.AreEqual (_functionStateWithEnabledCleanUp.FunctionToken, wxeContext.FunctionToken);
    Assert.AreEqual (CurrentHttpContext, wxeContext.HttpContext);
    Assert.AreEqual ("4", function.LastExecutedStepID);
  }

  [Test]
  [ExpectedException (typeof (ArgumentException))]
  public void ExecuteAbortedFunctionState()
  {
    _wxeHandler.ExecuteFunctionState (CurrentHttpContext, _functionStateAborted, true);
    Assert.Fail();
  }

  [Test]
  public void ExecuteFunction()
  {
    TestFunction function = (TestFunction) _functionStateWithEnabledCleanUp.Function;
    _wxeHandler.ExecuteFunction (function, CurrentWxeContext, true);
    
    WxeContext wxeContext = function.TestStep.WxeContext;
    Assert.AreSame (WxeContext.Current, wxeContext);

    Type[] catchExceptionTypes = function.GetCatchExceptionTypes();
    Assert.AreEqual (1, catchExceptionTypes.Length);
    Assert.AreSame (typeof (WxeUserCancelException), catchExceptionTypes[0]);

    Assert.AreEqual ("4", function.LastExecutedStepID);
  }

  [Test]
  [ExpectedException (typeof (ArgumentException))]
  public void ExecuteAbortedFunction()
  {
    TestFunction function = (TestFunction) _functionStateAborted.Function;
    _wxeHandler.ExecuteFunction (function, CurrentWxeContext, true);
    Assert.Fail();
  }
}

}
