// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Web;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocAutoCompleteReferenceValue.StandardMode
{
  public class BocAutoCompleteReferenceValuePreRenderer : BocAutoCompleteReferenceValuePreRendererBase
  {
    public BocAutoCompleteReferenceValuePreRenderer (HttpContextBase context, IBocAutoCompleteReferenceValue control)
        : base (context, control)
    {
    }

    public override void PreRender ()
    {
      base.PreRender();
      RegisterAdjustPositionScript();
      RegisterAdjustLayoutScript ();
    }

    private void RegisterAdjustPositionScript ()
    {
      string key = Control.ClientID + "_AdjustPositionScript";
      Control.Page.ClientScript.RegisterStartupScriptBlock (
          Control,
          typeof (BocAutoCompleteReferenceValuePreRendererBase),
          key,
          string.Format ("BocAutoCompleteReferenceValue.AdjustPosition($('#{0}'), {1});",
                         Control.ClientID,
                         Control.EmbedInOptionsMenu ? "true" : "false"));
    }

    private void RegisterAdjustLayoutScript ()
    {
      Control.Page.ClientScript.RegisterStartupScriptBlock (
          Control,
          typeof (BocAutoCompleteReferenceValuePreRenderer),
          Guid.NewGuid ().ToString (),
          string.Format ("BocBrowserCompatibility.AdjustAutoCompleteReferenceValueLayout ($('#{0}'));", Control.ClientID));
    }
  }
}
