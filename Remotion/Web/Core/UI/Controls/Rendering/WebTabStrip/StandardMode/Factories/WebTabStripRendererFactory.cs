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
using System.Web.UI;
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UI.Controls.Rendering.WebTabStrip.StandardMode.Factories
{
  /// <summary>
  /// Responsible for creating standard mode renderers for <see cref="WebTabStrip"/>.
  /// </summary>
  public class WebTabStripRendererFactory : IWebTabStripRendererFactory, IWebTabRendererFactory
  {
    public IWebTabStripRenderer CreateRenderer (IHttpContext context, HtmlTextWriter writer, IWebTabStrip control, IWebTabRendererFactory tabRendererFactory)
    {
      return new WebTabStripRenderer (context, writer, control, tabRendererFactory);
    }

    public IWebTabStripPreRenderer CreatePreRenderer (IHttpContext context, IWebTabStrip control)
    {
      return new WebTabStripPreRenderer (context, control);
    }

    IWebTabRenderer IWebTabRendererFactory.CreateRenderer (IHttpContext context, HtmlTextWriter writer, IWebTabStrip control)
    {
      return new WebTabRenderer (context, writer, control);
    }
  }
}