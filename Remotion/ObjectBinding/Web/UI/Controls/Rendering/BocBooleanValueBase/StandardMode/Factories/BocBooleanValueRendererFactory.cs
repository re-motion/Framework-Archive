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

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocBooleanValueBase.StandardMode.Factories
{
  /// <summary>
  /// Responsible for creating quirks mode renderers for <see cref="IBocBooleanValue"/> and <see cref="IBocCheckBox"/> controls;
  /// </summary>
  public class BocBooleanValueRendererFactory : IBocBooleanValueRendererFactory, IBocCheckboxRendererFactory
  {
    IBocBooleanValueRenderer IBocBooleanValueRendererFactory.CreateRenderer (IHttpContext context, HtmlTextWriter writer, IBocBooleanValue control)
    {
      return new BocBooleanValueRenderer (context, writer, control);
    }

    public IBocBooleanValuePreRenderer CreatePreRenderer (IHttpContext context, IBocBooleanValue control)
    {
      return new BocBooleanValuePreRenderer (context, control);
    }

    IBocCheckboxRenderer IBocCheckboxRendererFactory.CreateRenderer (IHttpContext context, HtmlTextWriter writer, IBocCheckBox control)
    {
      return new BocCheckboxRenderer (context, writer, control);
    }

    public IBocCheckboxPreRenderer CreatePreRenderer (IHttpContext context, IBocCheckBox control)
    {
      return new BocCheckboxPreRenderer (context, control);
    }
  }
}
