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

namespace Remotion.Web.ExecutionEngine.WxePageStepExecutionStates
{
  public interface IExecutionState
  {
    bool IsExecuting { get; }
    void ExecuteSubFunction (WxeContext context);
    void PostProcessSubFunction (WxeContext context);    
  }

  public class WxePageStepExecutionState : IExecutionState
  {
    public bool IsExecuting
    {
      get { return false; }
    }

    public void ExecuteSubFunction (WxeContext context)
    {
      throw new NotSupportedException();
    }

    public void PostProcessSubFunction (WxeContext context)
    {
    }
  }
}