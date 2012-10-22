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
using System.Collections.Generic;
using System.Web.UI;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering the navigation block of a <see cref="BocList"/>.
  /// </summary>
  /// <remarks>This class should not be instantiated directly. It is meant to be used by a <see cref="Web.UI.Controls.BocListImplementation.Rendering.BocListRenderer"/>.</remarks>
  public class BocListNavigationBlockQuirksModeRenderer : IBocListNavigationBlockRenderer
  {
    /// <summary> A list of control specific resources. </summary>
    /// <remarks> 
    ///   Resources will be accessed using 
    ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
    ///   See the documentation of <b>GetString</b> for further details.
    /// </remarks>
    [ResourceIdentifiers]
    [MultiLingualResources ("Remotion.ObjectBinding.Web.Legacy.Globalization.BocListNavigationBlockQuirksModeRenderer")]
    public enum ResourceIdentifier
    {
      PageInfo,
      GoToFirstAlternateText,
      GoToLastAlternateText,
      GoToNextAlternateText,
      GoToPreviousAlternateText,
    }

    private const string c_whiteSpace = "&nbsp;";
    private const string c_goToFirstIcon = "MoveFirst.gif";
    private const string c_goToLastIcon = "MoveLast.gif";
    private const string c_goToPreviousIcon = "MovePrevious.gif";
    private const string c_goToNextIcon = "MoveNext.gif";
    private const string c_goToFirstInactiveIcon = "MoveFirstInactive.gif";
    private const string c_goToLastInactiveIcon = "MoveLastInactive.gif";
    private const string c_goToPreviousInactiveIcon = "MovePreviousInactive.gif";
    private const string c_goToNextInactiveIcon = "MoveNextInactive.gif";

    private static readonly IDictionary<GoToOption, string> s_activeIcons = new Dictionary<GoToOption, string>
                                                                            {
                                                                                { GoToOption.First, c_goToFirstIcon },
                                                                                { GoToOption.Previous, c_goToPreviousIcon },
                                                                                { GoToOption.Next, c_goToNextIcon },
                                                                                { GoToOption.Last, c_goToLastIcon }
                                                                            };

    private static readonly IDictionary<GoToOption, string> s_inactiveIcons = new Dictionary<GoToOption, string>
                                                                              {
                                                                                  { GoToOption.First, c_goToFirstInactiveIcon },
                                                                                  { GoToOption.Previous, c_goToPreviousInactiveIcon },
                                                                                  { GoToOption.Next, c_goToNextInactiveIcon },
                                                                                  { GoToOption.Last, c_goToLastInactiveIcon }
                                                                              };

    private static readonly IDictionary<GoToOption, ResourceIdentifier> s_alternateTexts =
        new Dictionary
            <GoToOption, ResourceIdentifier>
        {
            { GoToOption.First, ResourceIdentifier.GoToFirstAlternateText },
            { GoToOption.Previous, ResourceIdentifier.GoToPreviousAlternateText },
            { GoToOption.Next, ResourceIdentifier.GoToNextAlternateText },
            { GoToOption.Last, ResourceIdentifier.GoToLastAlternateText }
        };

    /// <summary> The possible directions for paging through the List. </summary>
    private enum GoToOption
    {
      /// <summary> Don't page. </summary>
      Undefined,
      /// <summary> Move to first page. </summary>
      First,
      /// <summary> Move to last page. </summary>
      Last,
      /// <summary> Move to previous page. </summary>
      Previous,
      /// <summary> Move to next page. </summary>
      Next
    }

    private readonly BocListQuirksModeCssClassDefinition _cssClasses;

    /// <summary>
    /// Contructs a renderer bound to a <see cref="BocList"/> to render and an <see cref="HtmlTextWriter"/> to render to.
    /// </summary>
    /// <remarks>
    /// This class should not be instantiated directly by clients. Instead, a <see cref="BocListQuirksModeRenderer"/> should use a
    /// factory to obtain an instance of this class.
    /// </remarks>
    public BocListNavigationBlockQuirksModeRenderer (BocListQuirksModeCssClassDefinition cssClasses)
    {
      ArgumentUtility.CheckNotNull ("cssClasses", cssClasses);

      _cssClasses = cssClasses;
    }

    public BocListQuirksModeCssClassDefinition CssClasses
    {
      get { return _cssClasses; }
    }
    
    /// <summary> 
    /// Renders the navigation bar consisting of the move buttons and the page information. 
    /// </summary>
    public void Render (BocListRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      bool isFirstPage = renderingContext.Control.CurrentPage == 0;
      bool isLastPage = renderingContext.Control.CurrentPage + 1 >= renderingContext.Control.PageCount;

      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.Navigator);
      renderingContext.Writer.AddStyleAttribute ("position", "relative");
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      //  Page info
      string pageInfo = GetResourceManager (renderingContext).GetString (ResourceIdentifier.PageInfo);

      string navigationText = string.Format (pageInfo, renderingContext.Control.CurrentPage + 1, renderingContext.Control.PageCount);
      // Do not HTML encode.
      renderingContext.Writer.Write (navigationText);

      if (renderingContext.Control.HasClientScript)
      {
        renderingContext.Writer.Write (c_whiteSpace + c_whiteSpace + c_whiteSpace);

        RenderNavigationIcon (renderingContext, isFirstPage, GoToOption.First);
        RenderNavigationIcon (renderingContext, isFirstPage, GoToOption.Previous);
        RenderNavigationIcon (renderingContext, isLastPage, GoToOption.Next);
        RenderNavigationIcon (renderingContext, isLastPage, GoToOption.Last);
      }
      renderingContext.Writer.RenderEndTag();
    }

    /// <summary>Renders the appropriate icon for the given <paramref name="command"/>, depending on <paramref name="isInactive"/>.</summary>
    private void RenderNavigationIcon (BocListRenderingContext renderingContext, bool isInactive, GoToOption command)
    {
      if (isInactive || renderingContext.Control.EditModeController.IsRowEditModeActive)
      {
        string imageUrl = GetResolvedImageUrl(renderingContext, s_inactiveIcons[command]);
        new IconInfo (imageUrl).Render (renderingContext.Writer, renderingContext.Control);
      }
      else
      {
        var navigateCommandID = renderingContext.Control.ClientID + "_Navigation_" + command;
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Id, navigateCommandID);

        string argument = BocList.GoToCommandPrefix + command;
        string postBackEvent = renderingContext.Control.Page.ClientScript.GetPostBackEventReference (renderingContext.Control, argument);
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, postBackEvent);

        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");

        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.A);

        string imageUrl = GetResolvedImageUrl (renderingContext, s_activeIcons[command]);
        var icon = new IconInfo (imageUrl);
        icon.AlternateText = GetResourceManager (renderingContext).GetString (s_alternateTexts[command]);
        icon.Render (renderingContext.Writer, renderingContext.Control);

        renderingContext.Writer.RenderEndTag();
      }

      renderingContext.Writer.Write (c_whiteSpace + c_whiteSpace + c_whiteSpace);
    }

    protected virtual IResourceManager GetResourceManager (BocListRenderingContext renderingContext)
    {
      return new ResourceManagerSet (
          MultiLingualResources.GetResourceManager (typeof (ResourceIdentifier), true),
          renderingContext.Control.GetResourceManager());
    }

    private string GetResolvedImageUrl (BocListRenderingContext renderingContext, string imageUrl)
    {
      imageUrl = ResourceUrlResolver.GetResourceUrl (renderingContext.Control, renderingContext.HttpContext, typeof (BocListNavigationBlockQuirksModeRenderer), ResourceType.Image, imageUrl);
      return imageUrl;
    }
  }
}