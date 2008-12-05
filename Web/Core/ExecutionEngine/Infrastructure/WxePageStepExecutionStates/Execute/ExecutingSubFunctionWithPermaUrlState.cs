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
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.Execute
{
  /// <summary>
  /// The <see cref="RedirectingToSubFunctionState"/> is responsible for executing a sub-function.
  /// Executing this state will transition the <see cref="IExecutionStateContext"/> into the <see cref="ReturningFromSubFunctionState"/>.
  /// </summary>
  [Serializable]
  public class ExecutingSubFunctionWithPermaUrlState : ExecutionStateBase<RedirectingToSubFunctionStateParameters>
  {
    public ExecutingSubFunctionWithPermaUrlState (IExecutionStateContext executionStateContext, RedirectingToSubFunctionStateParameters parameters)
        : base (executionStateContext, parameters)
    {
    }

    public override void ExecuteSubFunction (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      Parameters.SubFunction.Execute (context);
      ExecutionStateContext.SetExecutionState (new ReturningFromSubFunctionState (ExecutionStateContext, Parameters));
    }
  }
}
