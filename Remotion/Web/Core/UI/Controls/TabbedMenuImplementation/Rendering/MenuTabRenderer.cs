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
using System.Collections.Specialized;
using System.Web.UI;
using Remotion.Utilities;
using System.Web;
using Remotion.Web.UI.Controls.WebTabStripImplementation;
using Remotion.Web.UI.Controls.WebTabStripImplementation.Rendering;

namespace Remotion.Web.UI.Controls.TabbedMenuImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering a <see cref="MenuTab"/> in quirks mode.
  /// <seealso cref="IMenuTab"/>
  /// </summary>
  public class MenuTabRenderer : WebTabRenderer, IMenuTabRenderer
  {
    private Command _renderingCommand;

    public MenuTabRenderer ()
    {
    }

    protected override void RenderBeginTagForCommand (WebTabStripRenderingContext renderingContext, IWebTab tab, bool isEnabled, WebTabStyle style)
    {
      ArgumentUtility.CheckNotNull ("style", style);

      var menuTab = ((IMenuTab) tab).GetActiveTab();
      _renderingCommand = GetRenderingCommand (isEnabled, menuTab);

      var additionalUrlParameters = menuTab.GetUrlParameters();
      var backupID = _renderingCommand.ItemID;

      try
      {
        if (!string.IsNullOrEmpty (tab.ItemID) && string.IsNullOrEmpty (_renderingCommand.ItemID))
          _renderingCommand.ItemID = tab.ItemID + "_Command";

        _renderingCommand.RenderBegin (
            renderingContext.Writer, tab.GetPostBackClientEvent(), new string[0], string.Empty, null, additionalUrlParameters, false, style);
      }
      finally
      {
        _renderingCommand.ItemID = backupID;
      }
    }

    protected override void RenderEndTagForCommand (WebTabStripRenderingContext renderingContext)
    {
      if (_renderingCommand != null)
        _renderingCommand.RenderEnd (renderingContext.Writer);
      else
        renderingContext.Writer.RenderEndTag ();
    }

    private Command GetRenderingCommand (bool isEnabled, IMenuTab activeTab)
    {
      if (isEnabled && activeTab.EvaluateEnabled ())
        return activeTab.Command;

      return new Command (CommandType.None) { OwnerControl = activeTab.OwnerControl };
    }
  }
}