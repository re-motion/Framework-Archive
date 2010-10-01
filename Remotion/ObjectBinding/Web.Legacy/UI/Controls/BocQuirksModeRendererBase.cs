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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;
using Remotion.Web.Legacy.UI.Controls;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Legacy.UI.Controls
{
  /// <summary>
  /// Base class for renderers of <see cref="IBocRenderableControl"/> objects.
  /// </summary>
  /// <typeparam name="TControl">The type of control that can be rendered.</typeparam>
  public abstract class BocQuirksModeRendererBase<TControl> : QuirksModeRendererBase<TControl>
      where TControl: IBocRenderableControl, IBusinessObjectBoundEditableWebControl
  {
    protected BocQuirksModeRendererBase () { }

    protected void RegisterBrowserCompatibilityScript (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterUtilitiesJavaScriptInclude ();
    }

    /// <summary>
    /// Adds class and style attributes found in the <see cref="RenderingContext{TControl}.Control"/> 
    /// to the <paramref name="renderingContext"/> so that they are rendered in the next begin tag.
    /// </summary>
    /// <param name="renderingContext">The <see cref="IRenderingContext"/>.</param>
    /// <param name="overrideWidth">When <see langword="true"/>, the 'width' style attribute is rendered with a value of 'auto'
    /// without changing the contents of the actual style.</param>
    /// <remarks>This automatically adds the CSS classes found in <see cref="CssClassReadOnly"/>
    /// and <see cref="CssClassDisabled"/> if appropriate.</remarks>
    protected void AddAttributesToRender (RenderingContext<TControl> renderingContext, bool overrideWidth)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      Unit backUpWidth;
      string backUpStyleWidth;
      OverrideWidth (renderingContext, overrideWidth, "auto", out backUpWidth, out backUpStyleWidth);

      string backUpCssClass;
      string backUpAttributeCssClass;
      OverrideCssClass (renderingContext, out backUpCssClass, out backUpAttributeCssClass);

      AddStandardAttributesToRender (renderingContext);

      RestoreClass (renderingContext, backUpCssClass, backUpAttributeCssClass);
      RestoreWidth (renderingContext, backUpStyleWidth, backUpWidth);

      AddAdditionalAttributes (renderingContext);
    }

    /// <summary>
    /// Called after all attributes have been added by <see cref="AddAttributesToRender"/>.
    /// Use this to render style attributes without putting them into the control's <see cref="IBocRenderableControl.Style"/> property.
    /// </summary>
    protected virtual void AddAdditionalAttributes (RenderingContext<TControl> renderingContext)
    {
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

    /// <summary> Gets the CSS-Class applied to the <see cref="IBocRenderableControl"/> itself. </summary>
    /// <remarks> 
    ///   <para> Class: <c>bocTextValue</c>. </para>
    ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
    /// </remarks>
    public abstract string CssClassBase { get; }

    /// <summary> Gets the CSS-Class applied to the <see cref="IBocRenderableControl"/> when it is displayed disabled. </summary>
    /// <remarks> 
    ///   <para> Class: <c>disabled</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocTextValue.disabled</c> as a selector.</para>
    /// </remarks>
    public virtual string CssClassDisabled
    {
      get { return "disabled"; }
    }

    private void OverrideWidth (RenderingContext<TControl> renderingContext, bool overrideWidth, string newWidth, out Unit backUpWidth, out string backUpStyleWidth)
    {
      backUpStyleWidth = renderingContext.Control.Style["width"];
      backUpWidth = renderingContext.Control.Width;
      if( !overrideWidth )
        return;

      renderingContext.Control.Style["width"] = newWidth;
      renderingContext.Control.Width = Unit.Empty;
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
        renderingContext.Control.CssClass = CssClassBase + GetAdditionalCssClass (renderingContext.Control.IsReadOnly, !renderingContext.Control.Enabled);
    }

    private void RestoreWidth (RenderingContext<TControl> renderingContext, string backUpStyleWidth, Unit backUpWidth)
    {
      renderingContext.Control.Style["width"] = backUpStyleWidth;
      renderingContext.Control.Width = backUpWidth;
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