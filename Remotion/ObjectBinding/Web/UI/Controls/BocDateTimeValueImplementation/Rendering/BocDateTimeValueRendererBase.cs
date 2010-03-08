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
using System.Web.UI.WebControls;
using System.Web;
using Remotion.Utilities;
using Remotion.Web.UI.Controls.Rendering.DatePickerButton;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Rendering
{
  public abstract class BocDateTimeValueRendererBase : BocRendererBase<IBocDateTimeValue>
  {
    /// <summary> Text displayed when control is displayed in desinger and is read-only has no contents. </summary>
    private const string c_designModeEmptyLabelContents = "##";

    private readonly DateTimeFormatter _formatter = new DateTimeFormatter();

    protected BocDateTimeValueRendererBase (HttpContextBase context, IBocDateTimeValue control)
        : base (context, control)
    {
    }

    public override string CssClassBase
    {
      get { return "bocDateTimeValue"; }
    }

    protected DateTimeFormatter Formatter
    {
      get { return _formatter; }
    }

    protected abstract void RenderEditModeControls (HtmlTextWriter writer);

    protected abstract bool DetermineClientScriptLevel (IDatePickerButton datePickerButton);

    protected string GetDateTextBoxSize (int dateTextBoxWidthPercentage)
    {
      string dateTextBoxSize;
      if (!Control.DateTextBoxStyle.Width.IsEmpty)
        dateTextBoxSize = Control.DateTextBoxStyle.Width.ToString();
      else
        dateTextBoxSize = dateTextBoxWidthPercentage + "%";
      return dateTextBoxSize;
    }

    protected string GetTimeTextBoxSize (int timeTextBoxWidthPercentage)
    {
      string timeTextBoxSize;
      if (!Control.TimeTextBoxStyle.Width.IsEmpty)
        timeTextBoxSize = Control.TimeTextBoxStyle.Width.ToString();
      else
        timeTextBoxSize = timeTextBoxWidthPercentage + "%";
      return timeTextBoxSize;
    }

    protected void Initialize (TextBox textBox, SingleRowTextBoxStyle textBoxStyle, int maxLength)
    {
      textBox.Enabled = Control.Enabled;
      textBox.ReadOnly = !Control.Enabled;
      textBox.Width = Unit.Empty;
      textBox.Height = Unit.Empty;
      textBox.ApplyStyle (Control.CommonStyle);
      Control.DateTimeTextBoxStyle.ApplyStyle (textBox);
      textBoxStyle.ApplyStyle (textBox);

      if (Control.ProvideMaxLength)
        textBox.MaxLength = maxLength;
    }

    /// <summary> Calculates the maximum length for required for entering the date component. </summary>
    /// <returns> The length. </returns>
    protected virtual int GetDateMaxLength ()
    {
      DateTime date = new DateTime (2000, 12, 31);
      string maxDate = date.ToString ("d");
      return maxDate.Length;
    }

    /// <summary> Calculates the maximum length for required for entering the time component. </summary>
    /// <returns> The length. </returns>
    protected virtual int GetTimeMaxLength ()
    {
      DateTime time = new DateTime (1, 1, 1, 23, 30, 30);
      string maxTime = Control.ShowSeconds ? time.ToString ("T") : time.ToString ("t");

      return maxTime.Length;
    }

    protected int GetDateTextBoxWidthPercentage (bool hasDateField, bool hasTimeField)
    {
      int dateTextBoxWidthPercentage = 0;
      if (hasDateField && hasTimeField && Control.ShowSeconds)
        dateTextBoxWidthPercentage = 55;
      else if (hasDateField && hasTimeField)
        dateTextBoxWidthPercentage = 60;
      else if (hasDateField)
        dateTextBoxWidthPercentage = 100;
      return dateTextBoxWidthPercentage;
    }

    protected virtual void RenderReadOnlyValue (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      Label label = new Label();

      if (Control.IsDesignMode && string.IsNullOrEmpty (label.Text))
      {
        label.Text = c_designModeEmptyLabelContents;
        //  Too long, can't resize in designer to less than the content's width
        //  Control.label.Text = "[ " + this.GetType().Name + " \"" + this.ID + "\" ]";
      }
      else
      {
        if (Control.Value.HasValue)
        {
          DateTime dateTime = Control.Value.Value;

          if (Control.ActualValueType == BocDateTimeValueType.DateTime)
            label.Text = Formatter.FormatDateTimeValue (dateTime, Control.ShowSeconds);
          else if (Control.ActualValueType == BocDateTimeValueType.Date)
            label.Text = Formatter.FormatDateValue (dateTime);
          else
            label.Text = dateTime.ToString();
        }
        else
          label.Text = "&nbsp;";
      }

      label.Height = Unit.Empty;
      label.Width = Unit.Empty;
      label.ApplyStyle (Control.CommonStyle);
      label.ApplyStyle (Control.LabelStyle);

      bool isControlHeightEmpty = Control.Height.IsEmpty && string.IsNullOrEmpty (Control.Style["height"]);
      bool isLabelHeightEmpty = label.Height.IsEmpty && string.IsNullOrEmpty (label.Style["height"]);
      if (!isControlHeightEmpty && isLabelHeightEmpty)
        writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

      bool isControlWidthEmpty = Control.Width.IsEmpty && string.IsNullOrEmpty (Control.Style["width"]);
      bool isLabelWidthEmpty = label.Width.IsEmpty && string.IsNullOrEmpty (label.Style["width"]);
      if (!isControlWidthEmpty && isLabelWidthEmpty)
      {
        if (!Control.Width.IsEmpty)
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Control.Width.ToString());
        else
          writer.AddStyleAttribute (HtmlTextWriterStyle.Width, Control.Style["width"]);
      }

      label.RenderControl (writer);
    }
  }
}