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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.Services;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering <see cref="BocAutoCompleteReferenceValue"/> controls in Standards Mode.
  /// </summary>
  /// <remarks>
  /// <para>During edit mode, the control is displayed using a <see cref="System.Web.UI.WebControls.TextBox"/> and a pop-up element.</para>
  /// <para>During read-only mode, the control's value is displayed using a <see cref="System.Web.UI.WebControls.Label"/>.</para>
  /// </remarks>
  public class BocAutoCompleteReferenceValueRenderer : BocReferenceValueRendererBase<IBocAutoCompleteReferenceValue>, IBocAutoCompleteReferenceValueRenderer
  {
    private readonly Func<TextBox> _textBoxFactory;

    public BocAutoCompleteReferenceValueRenderer (IResourceUrlFactory resourceUrlFactory)
      : this (resourceUrlFactory, () => new TextBox ())
    {
    }

    protected BocAutoCompleteReferenceValueRenderer (IResourceUrlFactory resourceUrlFactory, Func<TextBox> textBoxFactory)
      : base (resourceUrlFactory)
    {
      ArgumentUtility.CheckNotNull ("textBoxFactory", textBoxFactory);
      _textBoxFactory = textBoxFactory;
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      RegisterBrowserCompatibilityScript (htmlHeadAppender);
      RegisterJavaScriptFiles (htmlHeadAppender);
      RegisterStylesheets (htmlHeadAppender);
    }

    public void Render (BocAutoCompleteReferenceValueRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      base.Render (renderingContext);

      RegisterInitializationScript (renderingContext);
    }

    protected override sealed void RegisterJavaScriptFiles (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      base.RegisterJavaScriptFiles (htmlHeadAppender);

      htmlHeadAppender.RegisterJQueryIFrameShimJavaScriptInclude();

      string jqueryAutocompleteScriptKey = typeof (BocAutoCompleteReferenceValueRenderer).FullName + "_JQueryAutoCompleteScript";
      htmlHeadAppender.RegisterJavaScriptInclude (
          jqueryAutocompleteScriptKey,
          ResourceUrlFactory.CreateResourceUrl (
              typeof (BocAutoCompleteReferenceValueRenderer),
              ResourceType.Html,
              "BocAutoCompleteReferenceValue.jquery.js"));

      string scriptKey = typeof (BocAutoCompleteReferenceValueRenderer).FullName + "_Script";
      htmlHeadAppender.RegisterJavaScriptInclude (
          scriptKey,
          ResourceUrlFactory.CreateResourceUrl (typeof (BocAutoCompleteReferenceValueRenderer), ResourceType.Html, "BocAutoCompleteReferenceValue.js"));
    }

    private void RegisterStylesheets (HtmlHeadAppender htmlHeadAppender)
    {
      string styleKey = typeof (BocAutoCompleteReferenceValueRenderer).FullName + "_Style";
      htmlHeadAppender.RegisterStylesheetLink (
          styleKey,
          ResourceUrlFactory.CreateThemedResourceUrl (
              typeof (BocAutoCompleteReferenceValueRenderer),
              ResourceType.Html,
              "BocAutoCompleteReferenceValue.css"),
          HtmlHeadAppender.Priority.Library);

      string jqueryAutocompleteStyleKey = typeof (BocAutoCompleteReferenceValueRenderer).FullName + "_JQueryAutoCompleteStyle";
      htmlHeadAppender.RegisterStylesheetLink (
          jqueryAutocompleteStyleKey,
          ResourceUrlFactory.CreateThemedResourceUrl (
              typeof (BocAutoCompleteReferenceValueRenderer),
              ResourceType.Html,
              "BocAutoCompleteReferenceValue.jquery.css"),
          HtmlHeadAppender.Priority.Library);
    }

    private void RegisterInitializationScript (BocAutoCompleteReferenceValueRenderingContext renderingContext)
    {
      if (renderingContext.Control.IsReadOnly)
        return;

      if (!renderingContext.Control.Enabled)
        return;

      string key = renderingContext.Control.UniqueID + "_InitializationScript";

      var script = new StringBuilder (1000);
      script.Append ("$(document).ready( function() { BocAutoCompleteReferenceValue.Initialize(");
      script.AppendFormat ("$('#{0}'), ", renderingContext.Control.TextBoxClientID);
      script.AppendFormat ("$('#{0}'), ", renderingContext.Control.HiddenFieldClientID);
      script.AppendFormat ("$('#{0}'),", renderingContext.Control.DropDownButtonClientID);

      if (renderingContext.Control.EnableIcon)
        script.AppendFormat ("$('#{0} .{1}'), ", renderingContext.Control.ClientID, CssClassCommand);
      else
        script.Append ("null, ");

      script.AppendFormat ("'{0}', ", renderingContext.Control.ResolveClientUrl (renderingContext.Control.SearchServicePath));

      script.AppendFormat ("{0}, ", renderingContext.Control.CompletionSetCount);
      script.AppendFormat ("{0}, ", renderingContext.Control.DropDownDisplayDelay);
      script.AppendFormat ("{0}, ", renderingContext.Control.DropDownRefreshDelay);
      script.AppendFormat ("{0}, ", renderingContext.Control.SelectionUpdateDelay);

      script.AppendFormat ("'{0}', ", renderingContext.Control.NullValueString);
      AppendBooleanValueToScript (script, renderingContext.Control.TextBoxStyle.AutoPostBack ?? false);
      script.Append (", ");
      script.Append (GetSearchContextAsJson (renderingContext.SearchAvailableObjectWebServiceContext));
      script.Append (", ");
      AppendStringValueOrNullToScript (script, GetIconServicePath (renderingContext));
      script.Append (", ");
      script.Append (GetIconContextAsJson (renderingContext.IconWebServiceContext) ?? "null");
      script.Append (", ");
      script.Append (GetCommandInfoAsJson (renderingContext) ?? "null");

      script.Append ("); } );");

      renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock (
          renderingContext.Control, typeof (IBocAutoCompleteReferenceValue), key, script.ToString());
    }

    private string GetSearchContextAsJson (SearchAvailableObjectWebServiceContext searchContext)
    {
      var jsonBuilder = new StringBuilder (1000);

      jsonBuilder.Append ("{ ");
      jsonBuilder.Append ("businessObjectClass : ");
      AppendStringValueOrNullToScript (jsonBuilder, searchContext.BusinessObjectClass);
      jsonBuilder.Append (", ");
      jsonBuilder.Append ("businessObjectProperty : ");
      AppendStringValueOrNullToScript (jsonBuilder, searchContext.BusinessObjectProperty);
      jsonBuilder.Append (", ");
      jsonBuilder.Append ("businessObject : ");
      AppendStringValueOrNullToScript (jsonBuilder, searchContext.BusinessObjectIdentifier);
      jsonBuilder.Append (", ");
      jsonBuilder.Append ("args : ");
      AppendStringValueOrNullToScript (jsonBuilder, searchContext.Args);
      jsonBuilder.Append (" }");

      return jsonBuilder.ToString();
    }

    protected override sealed void RenderEditModeValueWithSeparateOptionsMenu (BocRenderingContext<IBocAutoCompleteReferenceValue> renderingContext)
    {
      TextBox textBox = GetTextBox (renderingContext);
      RenderEditModeValue (renderingContext, textBox);
    }

    protected override sealed void RenderEditModeValueWithIntegratedOptionsMenu (BocRenderingContext<IBocAutoCompleteReferenceValue> renderingContext)
    {
      TextBox textBox = GetTextBox (renderingContext);
      textBox.Attributes.Add ("onclick", DropDownMenu.OnHeadTitleClickScript);
      RenderEditModeValue (renderingContext, textBox);
    }

    private void RenderEditModeValue (BocRenderingContext<IBocAutoCompleteReferenceValue> renderingContext, TextBox textBox)
    {
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassInput);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      bool autoPostBack = textBox.AutoPostBack;
      textBox.AutoPostBack = false;
      textBox.RenderControl (renderingContext.Writer);
      textBox.AutoPostBack = autoPostBack;
      renderingContext.Writer.RenderEndTag ();

      RenderDropdownButton (renderingContext);

      var hiddenField = GetHiddenField(renderingContext);
      if (autoPostBack)
      {
        PostBackOptions options = new PostBackOptions (textBox, string.Empty);
        if (textBox.CausesValidation)
        {
            options.PerformValidation = true;
            options.ValidationGroup = textBox.ValidationGroup;
        }
        if (renderingContext.Control.Page.Form != null)
            options.AutoPostBack = true;
        var postBackEventReference = renderingContext.Control.Page.ClientScript.GetPostBackEventReference (options, true);
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Onchange, postBackEventReference);
      }
      hiddenField.RenderControl (renderingContext.Writer);
    }

    private void RenderDropdownButton (BocRenderingContext<IBocAutoCompleteReferenceValue> renderingContext)
    {
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Id, renderingContext.Control.DropDownButtonClientID);
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassButton);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      IconInfo.Spacer.Render (renderingContext.Writer, renderingContext.Control);
      renderingContext.Writer.RenderEndTag ();
    }

    private HiddenField GetHiddenField (BocRenderingContext<IBocAutoCompleteReferenceValue> renderingContext)
    {
      return new HiddenField
      {
        ID = renderingContext.Control.HiddenFieldUniqueID,
        Page = renderingContext.Control.Page.WrappedInstance,
        EnableViewState = true,
        Value = renderingContext.Control.BusinessObjectUniqueIdentifier ?? renderingContext.Control.NullValueString        
      };
    }

    private TextBox GetTextBox (BocRenderingContext<IBocAutoCompleteReferenceValue> renderingContext)
    {
      var textBox = _textBoxFactory();
      textBox.ID = renderingContext.Control.TextBoxUniqueID;
      textBox.Text = renderingContext.Control.GetLabelText ();
      textBox.Enabled = renderingContext.Control.Enabled;
      textBox.EnableViewState = false;
      textBox.Page = renderingContext.Control.Page.WrappedInstance;
      textBox.ApplyStyle (renderingContext.Control.CommonStyle);
      renderingContext.Control.TextBoxStyle.ApplyStyle (textBox);   
      return textBox;
    }

    public override string GetCssClassBase(IBocAutoCompleteReferenceValue control)
    {
      return "bocAutoCompleteReferenceValue";
    }

    private string CssClassButton
    {
      get { return "bocAutoCompleteReferenceValueButton"; }
    }

    private string CssClassInput
    {
      get { return "bocAutoCompleteReferenceValueInput"; }
    }
  }
}