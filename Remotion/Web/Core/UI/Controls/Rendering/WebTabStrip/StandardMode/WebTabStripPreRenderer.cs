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
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UI.Controls.Rendering.WebTabStrip.StandardMode
{
  /// <summary>
  /// Responsible for registering the style sheet for <see cref="WebTabStrip"/> controls in standard mode.
  /// <seealso cref="IWebTabStrip"/>
  /// </summary>
  public class WebTabStripPreRenderer : WebTabStripPreRendererBase
  {
    public WebTabStripPreRenderer (IHttpContext context, IWebTabStrip control)
        : base (context, control)
    {
    }

    protected override ResourceTheme ResourceTheme
    {
      get { return Web.ResourceTheme.Standard; }
    }
  }
}