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
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.ExecuteWithPermaUrlStates
{
  public class ExecutingSubFunctionState : ExecuteWithPermaUrlStateBase
  {
    private readonly string _resumeUrl;

    public ExecutingSubFunctionState (IWxePageStepExecutionStateContext executionStateContext, WxeFunction subFunction, string resumeUrl)
        : base (executionStateContext, subFunction)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("resumeUrl", resumeUrl);

      _resumeUrl = resumeUrl;
    }

    public override void RedirectToSubFunction (WxeContext context)
    {
      throw new InvalidOperationException();
    }

    public override void ExecuteSubFunction (WxeContext context)
    {
      SubFunction.Execute (context);
      ExecutionStateContext.SetExecutionState (new ReturningFromSubFunctionState (ExecutionStateContext, SubFunction, _resumeUrl));
    }

    public override void ReturnFromSubFunction (WxeContext context)
    {
      throw new InvalidOperationException();
    }

    public override void PostProcessSubFunction (WxeContext context)
    {
      throw new InvalidOperationException();
    }

    public override void Cleanup (WxeContext context)
    {
      throw new InvalidOperationException();
    }

    public string ResumeUrl
    {
      get { return _resumeUrl; }
    }
  }
}