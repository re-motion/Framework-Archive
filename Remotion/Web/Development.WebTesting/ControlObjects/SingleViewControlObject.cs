﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for form grids created with <see cref="T:Remotion.Web.UI.Controls.SingleView"/>.
  /// </summary>
  [UsedImplicitly]
  public class SingleViewControlObject : RemotionControlObject, IControlHost
  {
    public SingleViewControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    public ScopeControlObject GetTopControls ()
    {
      var scope = Scope.FindChild ("TopControl");
      return new ScopeControlObject (Context.CloneForControl (scope));
    }

    public ScopeControlObject GetView ()
    {
      var scope = Scope.FindChild ("View");
      return new ScopeControlObject (Context.CloneForControl (scope));
    }

    public ScopeControlObject GetBottomControls ()
    {
      var scope = Scope.FindChild ("BottomControl");
      return new ScopeControlObject (Context.CloneForControl (scope));
    }

    public TControlObject GetControl<TControlObject> (IControlSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("controlSelectionCommand", controlSelectionCommand);

      return Children.GetControl (controlSelectionCommand);
    }
  }
}