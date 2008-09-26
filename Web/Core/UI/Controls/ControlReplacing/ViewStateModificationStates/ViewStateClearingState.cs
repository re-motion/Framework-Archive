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
  /// <summary>
  /// The <see cref="ViewStateClearingState"/> represents the <see cref="ViewStateModificationStateBase.Replacer"/> during a page-lifecycle in which 
  /// the control tree will be replaced with new version, requiring the reset of the view state for this instance as well.
  /// Executing the <see cref="LoadViewState"/> method will transation the <see cref="ViewStateModificationStateBase.Replacer"/> into the 
  /// <see cref="ViewStateCompletedState"/> if the <see cref="ViewStateModificationStateBase.Replacer"/> was initialized along with the rest of the 
  /// page's controls and the <see cref="ViewStateClearingAfterParentLoadedState"/> if the <see cref="ViewStateModificationStateBase.Replacer"/> 
  /// was initialized later in the page-lifecycle, i.e. during the loading-phase.
  /// </summary>
  public class ViewStateClearingState : ViewStateClearingStateBase
  {
    public ViewStateClearingState (ControlReplacer replacer, IInternalControlMemberCaller memberCaller)
        : base (replacer, memberCaller)
    {
    }

    public override void LoadViewState (object savedState)
    {
      if (Replacer.WrappedControl != null)
        ClearChildState ();
      else
        Replacer.ViewStateModificationState = new ViewStateClearingAfterParentLoadedState (Replacer, MemberCaller);
    }
  }
}