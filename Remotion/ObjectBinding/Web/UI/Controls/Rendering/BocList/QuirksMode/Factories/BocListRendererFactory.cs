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
using Microsoft.Practices.ServiceLocation;
using Remotion.Utilities;
using System.Web;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.QuirksMode.Factories
{
  /// <summary>
  /// Responsible for creating the quirks mode renderers for <see cref="IBocList"/> and its parts except columns - for that,
  /// see <see cref="BocColumnRendererFactory"/>.
  /// </summary>
  public class BocListRendererFactory
      :
          IBocListRendererFactory,
          IBocListMenuBlockRendererFactory,
          IBocListNavigationBlockRendererFactory,
          IBocRowRendererFactory,
          IBocListTableBlockRendererFactory
  {
    IBocRowRenderer IBocRowRendererFactory.CreateRenderer (HttpContextBase context, IBocList list, IServiceLocator serviceLocator)
    {
      return new StandardMode.BocRowRenderer (context, list, CssClassContainer.Instance, serviceLocator);
    }

    IBocListPreRenderer IBocListRendererFactory.CreatePreRenderer (HttpContextBase context, IBocList list)
    {
      return new BocListPreRenderer (context, list, CssClassContainer.Instance);
    }

    IBocListMenuBlockRenderer IBocListMenuBlockRendererFactory.CreateRenderer (HttpContextBase context, IBocList list)
    {
      return new StandardMode.BocListMenuBlockRenderer (context, list, CssClassContainer.Instance);
    }

    IBocListNavigationBlockRenderer IBocListNavigationBlockRendererFactory.CreateRenderer (HttpContextBase context, IBocList list)
    {
      return new StandardMode.BocListNavigationBlockRenderer (context, list, CssClassContainer.Instance);
    }

    public IRenderer CreateRenderer (HttpContextBase context, IBocList list, IServiceLocator serviceLocator)
    {
      return new StandardMode.BocListRenderer (context, list, CssClassContainer.Instance, serviceLocator);
    }

    IBocListTableBlockRenderer IBocListTableBlockRendererFactory.CreateRenderer (
        HttpContextBase context, IBocList list, IServiceLocator serviceLocator)
    {
      return new StandardMode.BocListTableBlockRenderer (context, list, CssClassContainer.Instance, serviceLocator);
    }
  }
}
