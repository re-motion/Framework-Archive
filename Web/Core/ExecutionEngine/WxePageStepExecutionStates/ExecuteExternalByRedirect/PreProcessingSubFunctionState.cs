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

namespace Remotion.Web.ExecutionEngine.WxePageStepExecutionStates.ExecuteExternalByRedirect
{
  /// <summary>
  /// The <see cref="PreProcessingSubFunctionState"/> is responsible for setting up the current <see cref="WxeStep"/> to execute the sub-function.
  /// Executing this state will transition the <see cref="IExecutionStateContext"/> into the <see cref="PreparingRedirectToSubFunctionState"/> .
  /// </summary>
  [Serializable]
  public class PreProcessingSubFunctionState : ExecutionStateBase<PreProcessingSubFunctionStateParameters>
  {
    private readonly WxeReturnOptions _returnOptions;

    public PreProcessingSubFunctionState (
        IExecutionStateContext executionStateContext, PreProcessingSubFunctionStateParameters parameters, WxeReturnOptions returnOptions)
        : base (executionStateContext, parameters)
    {
      ArgumentUtility.CheckNotNull ("returnOptions", returnOptions);
      _returnOptions = returnOptions;
    }

    public override void ExecuteSubFunction (WxeContext context)
    {
      NameValueCollection postBackCollection = BackupPostBackCollection();
      Parameters.Page.SaveAllState ();

      var parameters = new PreparingRedirectToSubFunctionStateParameters (Parameters.SubFunction, postBackCollection, Parameters.PermaUrlOptions);
      ExecutionStateContext.SetExecutionState (new PreparingRedirectToSubFunctionState (ExecutionStateContext, parameters, _returnOptions));
    }

    public WxeReturnOptions ReturnOptions
    {
      get { return _returnOptions; }
    }

    private NameValueCollection BackupPostBackCollection ()
    {
      return Parameters.Page.GetPostBackCollection().Clone();
    }
  }
}