/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.ExecuteExternalByRedirect;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.ExecuteExternalByRedirect
{
  [TestFixture]
  public class PostProcessingSubFunctionTest : TestBase
  {
    private IExecutionState _executionState;

    public override void SetUp ()
    {
      base.SetUp();
      _executionState = new PostProcessingSubFunctionState (ExecutionStateContextMock, new ExecutionStateParameters (SubFunction, PostBackCollection));
    }

    [Test]
    public void IsExecuting ()
    {
      Assert.That (_executionState.IsExecuting, Is.True);
    }

    [Test]
    public void ExecuteSubFunction_WithGetRequest ()
    {
      PrivateInvoke.SetNonPublicField (FunctionState, "_postBackID", 100);
      RequestMock.Stub (stub => stub.HttpMethod).Return ("GET").Repeat.Any();

      using (MockRepository.Ordered())
      {
        ExecutionStateContextMock.Expect (mock => mock.SetReturnState (SubFunction, true, PostBackCollection));
        ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (NullExecutionState.Null));
      }

      MockRepository.ReplayAll();

      _executionState.ExecuteSubFunction (WxeContext);

      MockRepository.VerifyAll();

      Assert.That (PostBackCollection[WxePageInfo<WxePage>.PostBackSequenceNumberID], Is.EqualTo ("100"));
    }

    [Test]
    public void ExecuteSubFunction_WithPostRequest ()
    {
      PrivateInvoke.SetNonPublicField (FunctionState, "_postBackID", 100);
      RequestMock.Stub (stub => stub.HttpMethod).Return ("POST").Repeat.Any();

      using (MockRepository.Ordered ())
      {
        ExecutionStateContextMock.Expect (mock => mock.SetReturnState (SubFunction, false, null));
        ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (NullExecutionState.Null));
      }

      MockRepository.ReplayAll();

      _executionState.ExecuteSubFunction (WxeContext);

      MockRepository.VerifyAll();
    }
  }
}