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
using System.Web.UI;
using Remotion.Web.Infrastructure;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocTextValueBase.QuirksMode.Factories
{
  /// <summary>
  /// Responsible for creating quirks mode renderers for <see cref="IBocTextValue"/> and <see cref="IBocMultilineTextValue"/> controls.
  /// </summary>
  public class BocTextValueRendererFactory : IBocTextValueRendererFactory, IBocMultilineTextValueRendererFactory
  {
    IBocTextValueRenderer IBocTextValueRendererFactory.CreateRenderer (IHttpContext context, HtmlTextWriter writer, IBocTextValue control)
    {
      return new StandardMode.BocTextValueRenderer (context, writer, control);
    }

    public IBocTextValuePreRenderer CreatePreRenderer (IHttpContext context, IBocTextValue control)
    {
      return new BocTextValuePreRenderer (context, control);
    }

    IBocMultilineTextValueRenderer IBocMultilineTextValueRendererFactory.CreateRenderer (
        IHttpContext context, HtmlTextWriter writer, IBocMultilineTextValue control)
    {
      return new StandardMode.BocMultilineTextValueRenderer (context, writer, control);
    }

    public IBocMultilineTextValuePreRenderer CreatePreRenderer (IHttpContext context, IBocMultilineTextValue control)
    {
      return new BocMultilineTextValuePreRenderer (context, control);
    }
  }
}
