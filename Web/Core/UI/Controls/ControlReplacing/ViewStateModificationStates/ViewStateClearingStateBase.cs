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
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.ControlReplacing.ViewStateModificationStates
{
  public abstract class ViewStateClearingStateBase : ViewStateModificationStateBase
  {
    protected ViewStateClearingStateBase (ControlReplacer replacer, IInternalControlMemberCaller memberCaller)
        : base (replacer, memberCaller)
    {
    }

    protected void ClearChildState ()
    {
      bool enableViewStateBackup = Replacer.WrappedControl.EnableViewState;
      Replacer.WrappedControl.EnableViewState = false;
      Replacer.WrappedControl.Load += delegate { Replacer.WrappedControl.EnableViewState = enableViewStateBackup; };

      Replacer.ViewStateModificationState = new ViewStateCompletedState (Replacer, MemberCaller);
    }
  }
}