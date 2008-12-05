// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Threading;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.Execute
{
  /// <summary>
  /// The <see cref="ReturningFromSubFunctionState"/> is responsible for redirecting the user's client back to main-function URL after the sub-function
  /// has completed execution and a perma-URL was used. Executing this state will transition the <see cref="IExecutionStateContext"/> into the 
  /// <see cref="PostProcessingSubFunctionState"/>.
  /// </summary>
  [Serializable]
  public class ReturningFromSubFunctionState : ExecutionStateBase<RedirectingToSubFunctionStateParameters>
  {
    public ReturningFromSubFunctionState (IExecutionStateContext executionStateContext, RedirectingToSubFunctionStateParameters parameters)
        : base (executionStateContext, parameters)
    {
    }

    public override void ExecuteSubFunction (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      try
      {
        context.HttpContext.Response.Redirect (Parameters.ResumeUrl);
        throw new InvalidOperationException (string.Format("Redirect to '{0}' failed.", Parameters.ResumeUrl));
      }
      catch (ThreadAbortException)
      {
        ExecutionStateContext.SetExecutionState (new PostProcessingSubFunctionState (ExecutionStateContext, Parameters));
        throw;
      }
    }
  }
}
