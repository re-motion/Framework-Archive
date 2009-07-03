// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Specialized;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls.Rendering.WebTabStrip;
using Remotion.Web.UI.Controls.Rendering.WebTabStrip.QuirksMode;

namespace Remotion.Web.UI.Controls.Rendering.TabbedMenu.QuirksMode
{
  /// <summary>
  /// Responsible for rendering a <see cref="MenuTab"/> in quirks mode.
  /// <seealso cref="IMenuTab"/>
  /// </summary>
  public class MenuTabRenderer : WebTabRenderer, IMenuTabRenderer
  {
    public MenuTabRenderer (IHttpContext context, HtmlTextWriter writer, IWebTabStrip control, IMenuTab tab)
        : base(context, writer, control, tab)
    {
    }

    protected override void RenderBeginTagForCommand (bool isEnabled, WebTabStyle style)
    {
      ArgumentUtility.CheckNotNull ("style", style);

      var menuTab = ((IMenuTab) Tab).GetActiveTab ();
      RenderingCommand = GetRenderingCommand(isEnabled, menuTab);

      if (RenderingCommand != null)
      {
        NameValueCollection additionalUrlParameters = menuTab.GetUrlParameters();
        RenderingCommand.RenderBegin (
            Writer, Tab.GetPostBackClientEvent (), new string[0], string.Empty, null, additionalUrlParameters, false, style);
      }
      else
      {
        style.AddAttributesToRender (Writer);
        Writer.RenderBeginTag (HtmlTextWriterTag.A);
      }
    }

    protected Command RenderingCommand { get; set; }

    protected override void RenderEndTagForCommand ()
    {
      if (RenderingCommand != null)
        RenderingCommand.RenderEnd (Writer);
      else
        Writer.RenderEndTag ();
    }

    private Command GetRenderingCommand (bool isEnabled, IMenuTab activeTab)
    {
      if (isEnabled && activeTab.EvaluateEnabled ())
        return activeTab.Command;

      return null;
    }
  }
}