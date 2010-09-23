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
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Rendering
{
  /// <summary>
  /// Defines the API for rendering a <see cref="BocTextValue"/>.
  /// </summary>
  public interface IBocTextValueRenderer
  {
    void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender, IControl control, HttpContextBase context);
    void Render (BocTextValueRenderingContext renderingContext);
  }

  /// <summary>
  /// Provides a label for rendering a <see cref="BocTextValue"/> control in read-only mode. 
  /// Rendering is done by the parent class.
  /// </summary>
  public class BocTextValueRenderer : BocTextValueRendererBase<IBocTextValue>, IBocTextValueRenderer
  {
    public BocTextValueRenderer (HttpContextBase context, IBocTextValue control, IResourceUrlFactory resourceUrlFactory)
      : base (context, control, resourceUrlFactory)
    {
    }

    public override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      Render (new BocTextValueRenderingContext(Context, writer, Control));
    }

    public void Render (BocTextValueRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      base.Render (renderingContext);
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender, IControl control, HttpContextBase context)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      ((IBocTextValue) control).TextBoxStyle.RegisterJavaScriptInclude (ResourceUrlFactory, htmlHeadAppender);
      
      string styleKey = typeof (BocTextValueRenderer).FullName + "_Style";
      var styleFile = ResourceUrlFactory.CreateThemedResourceUrl (typeof (BocTextValueRenderer), ResourceType.Html, "BocTextValue.css");
      htmlHeadAppender.RegisterStylesheetLink (styleKey, styleFile, HtmlHeadAppender.Priority.Library);
    }

    protected override Label GetLabel (BocTextValueBaseRenderingContext<IBocTextValue> renderingContext)
    {
      Label label = new Label { Text = renderingContext.Control.Text };
      label.ID = renderingContext.Control.GetTextBoxClientID ();
      label.EnableViewState = false;

      string text;
      if (renderingContext.Control.TextBoxStyle.TextMode == TextBoxMode.MultiLine && !StringUtility.IsNullOrEmpty (renderingContext.Control.Text))
      {
        //  Allows for an optional \r
        string temp = renderingContext.Control.Text.Replace ("\r", "");
        string[] lines = temp.Split ('\n');
        for (int i = 0; i < lines.Length; i++)
          lines[i] = HttpUtility.HtmlEncode (lines[i]);
        text = StringUtility.ConcatWithSeparator (lines, "<br />");
      }
      else
        text = HttpUtility.HtmlEncode (renderingContext.Control.Text);

      if (StringUtility.IsNullOrEmpty (text) && renderingContext.Control.IsDesignMode)
      {
        text = c_designModeEmptyLabelContents;
        //  Too long, can't resize in designer to less than the content's width
        //  Label.Text = "[ " + this.GetType().Name + " \"" + this.ID + "\" ]";
      }
      label.Text = text;
      label.Width = Unit.Empty;
      label.Height = Unit.Empty;
      label.ApplyStyle (renderingContext.Control.CommonStyle);
      label.ApplyStyle (renderingContext.Control.LabelStyle);
      return label;
    }

    public override string GetCssClassBase(IBocTextValue control)
    {
      return "bocTextValue";
    }
  }
}