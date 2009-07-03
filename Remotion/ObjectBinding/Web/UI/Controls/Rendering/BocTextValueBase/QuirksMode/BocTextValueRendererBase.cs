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
using System.Web.UI.WebControls;
using Remotion.Web.Infrastructure;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocTextValueBase.QuirksMode
{
  /// <summary>
  /// Responsible for rendering <see cref="BocTextValue"/> and <see cref="BocMultilineTextValue"/> controls, which is done
  /// by a template method for which deriving classes have to supply the <see cref="GetLabel()"/> method.
  /// <seealso cref="BocTextValueRenderer"/>
  /// <seealso cref="BocMultilineTextValueRenderer"/>
  /// </summary>
  /// <typeparam name="T">The concrete control or corresponding interface that will be rendered.</typeparam>
  public abstract class BocTextValueRendererBase<T> : BocRendererBase<T>
    where T: IBocTextValueBase
  {
    /// <summary> Text displayed when control is displayed in desinger, is read-only, and has no contents. </summary>
    protected const string c_designModeEmptyLabelContents = "##";

    protected const string c_defaultTextBoxWidth = "150pt";
    protected const int c_defaultColumns = 60;

    protected BocTextValueRendererBase (IHttpContext context, HtmlTextWriter writer, T control)
        : base(context, writer, control)
    {
    }

    /// <summary>
    /// Renders a label when <see cref="IBusinessObjectBoundEditableControl.IsReadOnly"/> is <see langword="true"/>,
    /// a textbox in edit mode.
    /// </summary>
    public void Render ()
    {
      AddAttributesToRender (true);
      Writer.RenderBeginTag ("span");

      bool isControlHeightEmpty = Control.Height.IsEmpty && string.IsNullOrEmpty (Control.Style["height"]);

      string controlWidth = Control.Width.IsEmpty ? Control.Style["width"] : Control.Width.ToString();
      bool isControlWidthEmpty = string.IsNullOrEmpty (controlWidth);


      WebControl innerControl = Control.IsReadOnly ? (WebControl) GetLabel() : GetTextBox();

      bool isInnerControlHeightEmpty = innerControl.Height.IsEmpty && string.IsNullOrEmpty (innerControl.Style["height"]);
      bool isInnerControlWidthEmpty = innerControl.Width.IsEmpty && string.IsNullOrEmpty (innerControl.Style["width"]);

      if (!isControlHeightEmpty && isInnerControlHeightEmpty)
        Writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");


      if (isInnerControlWidthEmpty)
      {
        if (isControlWidthEmpty)
        {
          bool needsColumnCount = Control.TextBoxStyle.TextMode != TextBoxMode.MultiLine || Control.TextBoxStyle.Columns == null;
          if (!Control.IsReadOnly && needsColumnCount)
            Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultTextBoxWidth);
        }
        else
          Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, controlWidth);
      }
      innerControl.RenderControl (Writer);

      Writer.RenderEndTag();
    }

    /// <summary>
    /// Creates a <see cref="TextBox"/> control to use for rendering the <see cref="BocTextValueBase"/> control in edit mode.
    /// </summary>
    /// <returns>A <see cref="TextBox"/> control with the all relevant properties set and all appropriate styles applied to it.</returns>
    protected virtual TextBox GetTextBox ()
    {
      TextBox textBox = new TextBox { Text = Control.Text };
      textBox.ID = Control.GetTextBoxClientID ();
      textBox.EnableViewState = false;
      textBox.Enabled = Control.Enabled;
      textBox.ReadOnly = !Control.Enabled;
      textBox.Width = Unit.Empty;
      textBox.Height = Unit.Empty;
      textBox.ApplyStyle (Control.CommonStyle);
      Control.TextBoxStyle.ApplyStyle (textBox);
      if (textBox.TextMode == TextBoxMode.MultiLine && textBox.Columns < 1)
        textBox.Columns = c_defaultColumns;

      return textBox;
    }

    /// <summary>
    /// Creates a <see cref="Label"/> control to use for rendering the <see cref="BocTextValueBase"/> control in read-only mode.
    /// </summary>
    /// <returns>A <see cref="Label"/> control with all relevant properties set and all appropriate styles applied to it.</returns>
    protected abstract Label GetLabel ();

    protected override void AddAdditionalAttributes ()
    {
      Writer.AddStyleAttribute (HtmlTextWriterStyle.Display, "inline-block");
    }
  }
}