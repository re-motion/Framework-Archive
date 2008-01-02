using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Web.SessionState;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Collections;
using Rubicon.Development.UnitTesting;
using Rubicon.Reflection;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UnitTests.AspNetFramework;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{
  [TestFixture]
  public class WxeFunctionStateManagerTest
  {
    private MockRepository _mockRepository;
    private IHttpSessionState _mockSessionState;
    private HttpSessionState _session;
    private WxeFunctionState _functionState;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();

      _mockSessionState = _mockRepository.CreateMock<IHttpSessionState>();
      _session = TypesafeActivator.CreateInstance<HttpSessionState> (BindingFlags.Instance | BindingFlags.NonPublic).With (_mockSessionState);
      Assert.IsNotNull (_session);

      Expect.Call (_mockSessionState[GetSessionKeyForFunctionStates()]).Return (null);
      _mockSessionState["key"] = null;
      LastCall.Constraints (
          Rhino.Mocks.Constraints.Is.Equal (GetSessionKeyForFunctionStates()),
          Rhino.Mocks.Constraints.Is.TypeOf (typeof (Dictionary<string, WxeFunctionStateManager.WxeFunctionStateMetaData>)));

      _functionState = new WxeFunctionState (new TestFunction(), 1, true);
    }

    [TearDown]
    public void TearDown ()
    {
      HttpContextHelper.SetCurrent (null);
      System.Runtime.Remoting.Messaging.CallContext.SetData (typeof (WxeFunctionStateManager).AssemblyQualifiedName, null);
    }

    [Test]
    public void InitializeFromExistingSession ()
    {
      _mockRepository.BackToRecordAll();
      WxeFunctionStateManager.WxeFunctionStateMetaData functionStateMetaData =
          new WxeFunctionStateManager.WxeFunctionStateMetaData (Guid.NewGuid().ToString(), 1, DateTime.Now);
      Dictionary<string, WxeFunctionStateManager.WxeFunctionStateMetaData> functionStates =
          new Dictionary<string, WxeFunctionStateManager.WxeFunctionStateMetaData> ();
      functionStates.Add (functionStateMetaData.FunctionToken, functionStateMetaData);

      IHttpSessionState mockSessionState = _mockRepository.CreateMock<IHttpSessionState>();
      HttpSessionState session =
          TypesafeActivator.CreateInstance<HttpSessionState> (BindingFlags.Instance | BindingFlags.NonPublic).With (mockSessionState);

      Expect.Call (mockSessionState[GetSessionKeyForFunctionStates()]).Return (functionStates);
      _mockRepository.ReplayAll();

      WxeFunctionStateManager functionStateManager = new WxeFunctionStateManager (session);

      _mockRepository.VerifyAll();
      Assert.AreEqual (DateTime.Now, functionStateManager.GetLastAccess (functionStateMetaData.FunctionToken));
    }

    [Test]
    public void Add ()
    {
      _mockSessionState[GetSessionKeyForFunctionState()] = _functionState;
      _mockRepository.ReplayAll();

      WxeFunctionStateManager functionStateManager = new WxeFunctionStateManager (_session);
      functionStateManager.Add (_functionState);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetItem ()
    {
      Expect.Call (_mockSessionState[GetSessionKeyForFunctionState()]).Return (_functionState);
      _mockRepository.ReplayAll();

      WxeFunctionStateManager functionStateManager = new WxeFunctionStateManager (_session);
      WxeFunctionState actual = functionStateManager.GetItem (_functionState.FunctionToken);

      _mockRepository.VerifyAll();
      Assert.AreSame (_functionState, actual);
    }

    [Test]
    public void Abort ()
    {
      _mockSessionState.Remove (GetSessionKeyForFunctionState());
      _mockRepository.ReplayAll();

      WxeFunctionStateManager functionStateManager = new WxeFunctionStateManager (_session);
      functionStateManager.Abort (_functionState);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Touch ()
    {
      SetupResult.For (_mockSessionState[GetSessionKeyForFunctionState ()]).Return (_functionState);
      _mockSessionState[GetSessionKeyForFunctionState (_functionState.FunctionToken)] = _functionState;
      _mockRepository.ReplayAll ();

      WxeFunctionStateManager functionStateManager = new WxeFunctionStateManager (_session);
      functionStateManager.Add (_functionState);
      DateTime lastAccess = functionStateManager.GetLastAccess (_functionState.FunctionToken);
      Thread.Sleep (1000);
      functionStateManager.Touch (_functionState.FunctionToken);
      Assert.Greater (functionStateManager.GetLastAccess (_functionState.FunctionToken), lastAccess);

      _mockRepository.VerifyAll ();
    }

    [Test]
    [Explicit]
    public void IsExpired_DelaysForOneMinute ()
    {
      WxeFunctionState functionState = new WxeFunctionState (new TestFunction(), 1, true);
      SetupResult.For (_mockSessionState[GetSessionKeyForFunctionState (functionState.FunctionToken)]).Return (functionState);
      _mockSessionState[GetSessionKeyForFunctionState (functionState.FunctionToken)] = functionState;
      _mockRepository.ReplayAll ();

      WxeFunctionStateManager functionStateManager = new WxeFunctionStateManager (_session);
      functionStateManager.Add (functionState);
      Assert.IsFalse (functionStateManager.IsExpired (functionState.FunctionToken));
      Thread.Sleep (61000);
      Assert.IsTrue (functionStateManager.IsExpired (functionState.FunctionToken));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void IsExpired_WithUnknownFunctionToken ()
    {
      _mockRepository.ReplayAll ();

      WxeFunctionStateManager functionStateManager = new WxeFunctionStateManager (_session);
      Assert.IsTrue (functionStateManager.IsExpired (Guid.NewGuid().ToString()));

      _mockRepository.VerifyAll ();
    }

    [Test]
    [Explicit]
    public void CleanupExpired_DelaysForOneMinute ()
    {
      WxeFunctionState functionStateExpired = new WxeFunctionState (new TestFunction (), 1, true);
      SetupResult.For (_mockSessionState[GetSessionKeyForFunctionState (functionStateExpired.FunctionToken)]).Return (functionStateExpired);
      _mockSessionState[GetSessionKeyForFunctionState (functionStateExpired.FunctionToken)] = functionStateExpired;

      WxeFunctionState functionStateNotExpired = new WxeFunctionState (new TestFunction (), 10, true);
      SetupResult.For (_mockSessionState[GetSessionKeyForFunctionState (functionStateNotExpired.FunctionToken)]).Return (functionStateNotExpired);
      _mockSessionState[GetSessionKeyForFunctionState (functionStateNotExpired.FunctionToken)] = functionStateNotExpired;

      _mockSessionState.Remove (GetSessionKeyForFunctionState (functionStateExpired.FunctionToken));

      _mockRepository.ReplayAll ();

      WxeFunctionStateManager functionStateManager = new WxeFunctionStateManager (_session);
      functionStateManager.Add (functionStateExpired);
      functionStateManager.Add (functionStateNotExpired);

      Thread.Sleep (61000);

      Assert.IsTrue (functionStateManager.IsExpired (functionStateExpired.FunctionToken));
      Assert.IsFalse (functionStateManager.IsExpired (functionStateNotExpired.FunctionToken));

      functionStateManager.CleanUpExpired();

      Assert.IsFalse (functionStateManager.IsExpired (functionStateNotExpired.FunctionToken));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void HasSessionAndGetCurrent ()
    {
      HttpContextHelper.SetCurrent (HttpContextHelper.CreateHttpContext ("get", "default.aspx", string.Empty));
      Assert.IsFalse (WxeFunctionStateManager.HasSession);
      Assert.IsNotNull (WxeFunctionStateManager.Current);
      Assert.IsTrue (WxeFunctionStateManager.HasSession);
    }

    [Test]
    public void GetCurrent_SameInstanceTwice ()
    {
      HttpContextHelper.SetCurrent (HttpContextHelper.CreateHttpContext ("get", "default.aspx", string.Empty));
      Assert.AreSame (WxeFunctionStateManager.Current, WxeFunctionStateManager.Current);
    }

    [Test]
    public void HasSessionAndGetCurrentInSeparateThreads ()
    {
      HttpContextHelper.SetCurrent (HttpContextHelper.CreateHttpContext ("get", "default.aspx", string.Empty));
      Assert.IsFalse (WxeFunctionStateManager.HasSession);
      Assert.IsNotNull (WxeFunctionStateManager.Current);
      ThreadRunner.Run (
          delegate
          {
            HttpContextHelper.SetCurrent (HttpContextHelper.CreateHttpContext ("get", "default.aspx", string.Empty));
            Assert.IsFalse (WxeFunctionStateManager.HasSession);
            Assert.IsNotNull (WxeFunctionStateManager.Current);
            Assert.IsTrue (WxeFunctionStateManager.HasSession);
          });
    }


    private string GetSessionKeyForFunctionState ()
    {
      string functionToken = _functionState.FunctionToken;
      return GetSessionKeyForFunctionState(functionToken);
    }

    private string GetSessionKeyForFunctionState (string functionToken)
    {
      return typeof (WxeFunctionStateManager).AssemblyQualifiedName + "|WxeFunctionState|" + functionToken;
    }

    private string GetSessionKeyForFunctionStates ()
    {
      return typeof (WxeFunctionStateManager).AssemblyQualifiedName + "|WxeFunctionStates";
    }
  }
}