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
using Remotion.Web.ExecutionEngine.WxePageStepExecutionStates;
using Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.ExecuteWithPermaUrlStates;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.WxePageStepExecutionStates.ExecuteWithPermaUrlStates
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
      Assert.That (_executionState.IsExecuting, Is.False);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void ExecuteSubFunction ()
    {
      _executionState.ExecuteSubFunction (WxeContext);
    }

    [Test]
    public void PostProcessSubFunction ()
    {
      WxeContext.PostBackCollection = null;
      WxeContext.SetIsPostBack (false);
      WxeContext.SetIsReturningPostBack (false);
      PrivateInvoke.SetNonPublicField (FunctionState, "_postBackID", 100);

      ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (null));

      MockRepository.ReplayAll();

      _executionState.PostProcessSubFunction (WxeContext);

      MockRepository.VerifyAll();

      Assert.That (WxeContext.ReturningFunction, Is.SameAs (SubFunction));
      Assert.That (WxeContext.PostBackCollection, Is.SameAs (PostBackCollection));
      Assert.That (WxeContext.PostBackCollection[WxePageInfo<WxePage>.PostBackSequenceNumberID], Is.EqualTo ("100"));
      Assert.That (WxeContext.IsPostBack, Is.True);
      Assert.That (WxeContext.IsReturningPostBack, Is.True);
    }
  }
}