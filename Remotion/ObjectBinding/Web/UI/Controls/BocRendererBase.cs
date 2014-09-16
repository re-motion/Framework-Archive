// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary>
  /// Base class for renderers of <see cref="IBocRenderableControl"/> objects.
  /// </summary>
  /// <typeparam name="TControl">The type of control that can be rendered.</typeparam>
  public abstract class BocRendererBase<TControl> : RendererBase<TControl>
      where TControl: IBocRenderableControl, IBusinessObjectBoundEditableWebControl
  {
    protected BocRendererBase (
        IResourceUrlFactory resourceUrlFactory,
        IGlobalizationService globalizationService,
        IRenderingFeatures renderingFeatures)
        : base (resourceUrlFactory, globalizationService, renderingFeatures)
    {
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="IBocRenderableControl"/> itself. </summary>
    /// <remarks> 
    ///   <para> Class: <c>bocTextValue</c>. </para>
    ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
    /// </remarks>
    public abstract string GetCssClassBase (TControl control); 

    protected void RegisterBrowserCompatibilityScript (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterUtilitiesJavaScriptInclude ();
    }

    /// <summary>
    /// Adds class and style attributes found in the <see cref="RenderingContext{TControl}.Control"/> 
    /// to the <paramref name="renderingContext"/> so that they are rendered in the next begin tag.
    /// </summary>
    /// <param name="renderingContext">The <see cref="RenderingContext{TControl}"/>.</param>
    /// <remarks>This automatically adds the CSS classes found in <see cref="CssClassReadOnly"/>
    /// and <see cref="CssClassDisabled"/> if appropriate.</remarks>
    protected void AddAttributesToRender (RenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      string backUpCssClass;
      string backUpAttributeCssClass;
      OverrideCssClass (renderingContext, out backUpCssClass, out backUpAttributeCssClass);

      AddStandardAttributesToRender (renderingContext);

      RestoreClass (renderingContext, backUpCssClass, backUpAttributeCssClass);

      AddAdditionalAttributes (renderingContext);
    }

    /// <summary>
    /// Called after all attributes have been added by <see cref="AddAttributesToRender"/>.
    /// Use this to render style attributes without putting them into the control's <see cref="IBocRenderableControl.Style"/> property.
    /// </summary>
    protected virtual void AddAdditionalAttributes (RenderingContext<TControl> renderingContext)
    {
    }

    protected override void AddDiagnosticMetadataAttributes (RenderingContext<TControl> renderingContext)
    {
      base.AddDiagnosticMetadataAttributes (renderingContext);

      var control = renderingContext.Control;
      if (!string.IsNullOrEmpty (control.DisplayName))
        renderingContext.Writer.AddAttribute (DiagnosticMetadataAttributes.DisplayName, control.DisplayName);

      var isBound = control.Property != null && control.DataSource != null;
      renderingContext.Writer.AddAttribute (DiagnosticMetadataAttributes.IsBound, isBound.ToString().ToLower());
      
      if (isBound)
      {
        renderingContext.Writer.AddAttribute (DiagnosticMetadataAttributes.BoundType, control.DataSource.BusinessObjectClass.Identifier);
        renderingContext.Writer.AddAttribute (DiagnosticMetadataAttributes.BoundProperty, control.Property.Identifier);
      }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="IBocRenderableControl"/> when it is displayed in read-only mode. </summary>
    /// <remarks> 
    ///   <para> Class: <c>readOnly</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocTextValue.readOnly</c> as a selector. </para>
    /// </remarks>
    public virtual string CssClassReadOnly
    {
      get { return "readOnly"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="IBocRenderableControl"/> when it is displayed disabled. </summary>
    /// <remarks> 
    ///   <para> Class: <c>disabled</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocTextValue.disabled</c> as a selector.</para>
    /// </remarks>
    public virtual string CssClassDisabled
    {
      get { return "disabled"; }
    }

    private void OverrideCssClass (RenderingContext<TControl> renderingContext, out string backUpCssClass, out string backUpAttributeCssClass)
    {
      backUpCssClass = renderingContext.Control.CssClass;
      bool hasCssClass = !string.IsNullOrEmpty (backUpCssClass);
      if (hasCssClass)
        renderingContext.Control.CssClass += GetAdditionalCssClass (renderingContext.Control.IsReadOnly, !renderingContext.Control.Enabled);

      backUpAttributeCssClass = renderingContext.Control.Attributes["class"];
      bool hasClassAttribute = !string.IsNullOrEmpty (backUpAttributeCssClass);
      if (hasClassAttribute)
        renderingContext.Control.Attributes["class"] += GetAdditionalCssClass (renderingContext.Control.IsReadOnly, !renderingContext.Control.Enabled);

      if (!hasCssClass && !hasClassAttribute)
        renderingContext.Control.CssClass = GetCssClassBase(renderingContext.Control) + GetAdditionalCssClass (renderingContext.Control.IsReadOnly, !renderingContext.Control.Enabled);
    }

    private void RestoreClass (RenderingContext<TControl> renderingContext, string backUpCssClass, string backUpAttributeCssClass)
    {
      renderingContext.Control.CssClass = backUpCssClass;
      renderingContext.Control.Attributes["class"] = backUpAttributeCssClass;
    }

    private string GetAdditionalCssClass (bool isReadOnly, bool isDisabled)
    {
      string additionalCssClass = string.Empty;
      if (isReadOnly)
        additionalCssClass = " " + CssClassReadOnly;
      else if (isDisabled)
        additionalCssClass = " " + CssClassDisabled;
      return additionalCssClass;
    }
  }
}