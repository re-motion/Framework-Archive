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
using System.Collections.Specialized;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.ExecuteWithPermaUrlStates
{
  public class ReturningFromSubFunctionStateParameters : ExecutionStateParameters
  {
    private readonly string _resumeUrl;

    public ReturningFromSubFunctionStateParameters (WxeFunction subFunction, NameValueCollection postBackCollection, string resumeUrl)
        : base (subFunction, postBackCollection)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("resumeUrl", resumeUrl);

      _resumeUrl = resumeUrl;
    }

    public string ResumeUrl
    {
      get { return _resumeUrl; }
    }
  }
}