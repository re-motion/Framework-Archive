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
  private WxeHandlerMock _wxeHandler;

  protected const string c_functionTokenForFunctionStateWithEnabledCleanUp = "00000000-Enabled-CleanUp";
  protected const string c_functionTokenForFunctionStateWithDisabledCleanUp = "00000000-Disabled-CleanUp";
  protected const string c_functionTokenForFunctionStateWithMissingFunction = "00000000-Missing-Function";
  protected const string c_functionTokenForMissingFunctionState = "00000000-Missing-FunctionState";
  protected const string c_functionTokenForAbortedFunctionState = "00000000-Aborted";
  protected const string c_functionTokenForExpiredFunctionState = "00000000-Expired";

  private TestWxeFunctionState _functionStateWithEnabledCleanUp;
  private TestWxeFunctionState _functionStateWithDisabledCleanUp;
  private TestWxeFunctionState _functionStateWithMissingFunction;
  private TestWxeFunctionState _functionStateAborted;
  private TestWxeFunctionState _functionStateExpired;

  [SetUp]
  public virtual void SetUp()
  {
    _httpContext = HttpContextHelper.CreateHttpContext (@"C:\default.html", @"http://localhost/default.html", null);
    HttpContextHelper.SetCurrent (_httpContext);

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
  }

	[Test]
  public void RetrieveExistingFunctionState()
  {
    DateTime timeBeforeRefresh = DateTime.Now;
    Thread.Sleep (20);

    WxeFunctionState functionState = 
        _wxeHandler.ResumeExistingFunction (_httpContext, c_functionTokenForFunctionStateWithEnabledCleanUp);
    
    Assert.AreEqual (_functionStateWithEnabledCleanUp, functionState);
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
        _wxeHandler.ResumeExistingFunction (_httpContext, c_functionTokenForMissingFunctionState);
    Assert.Fail();
  }

	[Test]
  [ExpectedException (typeof (ApplicationException))]
  public void RetrieveFunctionStateWithMissingFunction()
  {
    WxeFunctionState functionState = 
        _wxeHandler.ResumeExistingFunction (_httpContext, c_functionTokenForFunctionStateWithMissingFunction);
    Assert.Fail();
  }

	[Test]
  [ExpectedException (typeof (ApplicationException))]
  public void RetrieveExpiredFunctionState()
  {
    WxeFunctionState functionState = 
        _wxeHandler.ResumeExistingFunction (_httpContext, c_functionTokenForExpiredFunctionState);
    Assert.Fail();
  }

	[Test]
  [ExpectedException (typeof (InvalidOperationException))]
  public void RetrieveAbortedFunctionState()
  {
    WxeFunctionState functionState = 
        _wxeHandler.ResumeExistingFunction (_httpContext, c_functionTokenForAbortedFunctionState);
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
        _wxeHandler.ResumeExistingFunction (_httpContext, c_functionTokenForFunctionStateWithEnabledCleanUp);

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
        _wxeHandler.ResumeExistingFunction (_httpContext, c_functionTokenForFunctionStateWithMissingFunction);

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
        _wxeHandler.ResumeExistingFunction (_httpContext, c_functionTokenForFunctionStateWithEnabledCleanUp);

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
        _wxeHandler.ResumeExistingFunction (_httpContext, c_functionTokenForFunctionStateWithMissingFunction);

    Assert.IsNull (functionState);
    Assert.IsTrue (_functionStateWithMissingFunction.IsAborted);
  }
}

}
