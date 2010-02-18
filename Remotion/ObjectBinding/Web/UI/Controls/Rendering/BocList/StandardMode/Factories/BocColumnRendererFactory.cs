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
using Remotion.Utilities;
using System.Web;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.StandardMode.Factories
{
  /// <summary>
  /// Responsible for creating the quirks mode column renderers.
  /// </summary>
  public class BocColumnRendererFactory
      :
          IBocColumnRendererFactory<BocSimpleColumnDefinition>,
          IBocColumnRendererFactory<BocCompoundColumnDefinition>,
          IBocColumnRendererFactory<BocCommandColumnDefinition>,
          IBocColumnRendererFactory<BocCustomColumnDefinition>,
          IBocColumnRendererFactory<BocDropDownMenuColumnDefinition>,
          IBocColumnRendererFactory<BocRowEditModeColumnDefinition>,
          IBocIndexColumnRendererFactory,
          IBocSelectorColumnRendererFactory
  {
    public IBocColumnRenderer<BocSimpleColumnDefinition> CreateRenderer
        (HttpContextBase context, HtmlTextWriter writer, IBocList list, BocSimpleColumnDefinition columnDefinition)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("columnDefinition", columnDefinition);

      return new BocSimpleColumnRenderer (context, writer, list, columnDefinition, CssClassContainer.Instance);
    }

    public IBocColumnRenderer<BocCompoundColumnDefinition> CreateRenderer (
        HttpContextBase context, HtmlTextWriter writer, IBocList list, BocCompoundColumnDefinition columnDefinition)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("columnDefinition", columnDefinition);

      return new BocCompoundColumnRenderer (context, writer, list, columnDefinition, CssClassContainer.Instance);
    }

    public IBocColumnRenderer<BocCommandColumnDefinition> CreateRenderer (
        HttpContextBase context, HtmlTextWriter writer, IBocList list, BocCommandColumnDefinition columnDefinition)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("columnDefinition", columnDefinition);

      return new BocCommandColumnRenderer (context, writer, list, columnDefinition, CssClassContainer.Instance);
    }

    public IBocColumnRenderer<BocCustomColumnDefinition> CreateRenderer (
        HttpContextBase context, HtmlTextWriter writer, IBocList list, BocCustomColumnDefinition columnDefinition)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("columnDefinition", columnDefinition);

      return new BocCustomColumnRenderer (context, writer, list, columnDefinition, CssClassContainer.Instance);
    }

    public IBocColumnRenderer<BocDropDownMenuColumnDefinition> CreateRenderer (
        HttpContextBase context, HtmlTextWriter writer, IBocList list, BocDropDownMenuColumnDefinition columnDefinition)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("columnDefinition", columnDefinition);

      return new BocDropDownMenuColumnRenderer (context, writer, list, columnDefinition, CssClassContainer.Instance);
    }

    public IBocColumnRenderer<BocRowEditModeColumnDefinition> CreateRenderer (
        HttpContextBase context, HtmlTextWriter writer, IBocList list, BocRowEditModeColumnDefinition columnDefinition)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("columnDefinition", columnDefinition);

      return new BocRowEditModeColumnRenderer (context, writer, list, columnDefinition, CssClassContainer.Instance);
    }

    IBocIndexColumnRenderer IBocIndexColumnRendererFactory.CreateRenderer (
        HttpContextBase context, HtmlTextWriter writer, IBocList list)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);

      return new BocIndexColumnRenderer(context, writer, list, CssClassContainer.Instance);
    }

    IBocSelectorColumnRenderer IBocSelectorColumnRendererFactory.CreateRenderer (
        HttpContextBase context, HtmlTextWriter writer, IBocList list)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("list", list);

      return new BocSelectorColumnRenderer(context, writer, list, CssClassContainer.Instance);
    }
  }
}
