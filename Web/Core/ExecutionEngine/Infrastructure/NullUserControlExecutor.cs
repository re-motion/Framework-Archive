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

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  public class NullUserControlExecutor : IUserControlExecutor
  {
    public static readonly IUserControlExecutor Null = new NullUserControlExecutor();

    private NullUserControlExecutor ()
    {
      
    }
    public bool IsNull
    {
      get { return true; }
    }

    public void Execute (WxeContext context)
    {
      //NOP
    }

    public void Return (WxeContext context)
    {
      //NOP
    }

    public WxeFunction SubFunction
    {
      get { return null; }
    }

    public string UserControlState
    {
      get { return null; }
    }

    public string UserControlID
    {
      get { return null; }
    }

    public bool IsReturningInnerFunction
    {
      get { return false; }
    }
  }
}