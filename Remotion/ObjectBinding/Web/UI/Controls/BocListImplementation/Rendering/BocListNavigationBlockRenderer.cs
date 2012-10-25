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
using System.Globalization;
using System.Text;
using System.Web.UI;
using Remotion.Globalization;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering the navigation block of a <see cref="BocList"/>.
  /// </summary>
  /// <remarks>This class should not be instantiated directly. It is meant to be used by a <see cref="BocListRenderer"/>.</remarks>
  public class BocListNavigationBlockRenderer : RendererBase<BocList>, IBocListNavigationBlockRenderer
  {
    /// <summary> A list of control specific resources. </summary>
    /// <remarks> 
    ///   Resources will be accessed using 
    ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
    ///   See the documentation of <b>GetString</b> for further details.
    /// </remarks>
    [ResourceIdentifiers]
    [MultiLingualResources ("Remotion.ObjectBinding.Web.Globalization.BocListNavigationBlockRenderer")]
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

    private readonly BocListCssClassDefinition _cssClasses;

    /// <summary>
    /// Contructs a renderer bound to a <see cref="BocList"/> to render and an <see cref="HtmlTextWriter"/> to render to.
    /// </summary>
    /// <remarks>
    /// This class should not be instantiated directly by clients. Instead, a <see cref="BocListRenderer"/> should use a
    /// factory to obtain an instance of this class.
    /// </remarks>
    public BocListNavigationBlockRenderer (IResourceUrlFactory resourceUrlFactory, BocListCssClassDefinition cssClasses)
      :base (resourceUrlFactory)
    {
      ArgumentUtility.CheckNotNull ("cssClasses", cssClasses);

      _cssClasses = cssClasses;
    }

    public BocListCssClassDefinition CssClasses
    {
      get { return _cssClasses; }
    }

    /// <summary> 
    /// Renders the navigation bar consisting of the move buttons and the current-page field. 
    /// </summary>
    public void Render (BocListRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.Navigator);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      if (renderingContext.Control.HasClientScript)
      {
        bool isFirstPage = renderingContext.Control.CurrentPageIndex == 0;
        bool isLastPage = renderingContext.Control.CurrentPageIndex + 1 >= renderingContext.Control.PageCount;

        //  Page info
        string pageInfo = GetResourceManager (renderingContext).GetString (ResourceIdentifier.PageInfo);

        string navigationText = string.Format (pageInfo, renderingContext.Control.CurrentPageIndex + 1, renderingContext.Control.PageCount);
        // Do not HTML encode.
        renderingContext.Writer.Write (navigationText);

        renderingContext.Writer.Write (c_whiteSpace + c_whiteSpace + c_whiteSpace);

        RenderNavigationIcon (renderingContext, isFirstPage, GoToOption.First, 0);
        RenderNavigationIcon (renderingContext, isFirstPage, GoToOption.Previous, renderingContext.Control.CurrentPageIndex - 1);
        RenderNavigationIcon (renderingContext, isLastPage, GoToOption.Next, renderingContext.Control.CurrentPageIndex + 1);
        RenderNavigationIcon (renderingContext, isLastPage, GoToOption.Last, renderingContext.Control.PageCount - 1);

        RenderValueField(renderingContext);
      }
      else
      {
        //  Page info
        string pageInfo = GetResourceManager (renderingContext).GetString (ResourceIdentifier.PageInfo);

        string navigationText = string.Format (pageInfo, renderingContext.Control.CurrentPageIndex + 1, renderingContext.Control.PageCount);
        // Do not HTML encode.
        renderingContext.Writer.Write (navigationText);
      }

      renderingContext.Writer.RenderEndTag();
    }

    private void RenderValueField (BocListRenderingContext renderingContext)
    {
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Id, renderingContext.Control.GetCurrentPageControlUniqueID().Replace('$', '_'));
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Name, renderingContext.Control.GetCurrentPageControlUniqueID());
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Type, "hidden");
      renderingContext.Writer.AddAttribute (
          HtmlTextWriterAttribute.Value,
          renderingContext.Control.CurrentPageIndex.ToString (CultureInfo.InvariantCulture));

      var postBackOptions = new PostBackOptions (new Control { ID = renderingContext.Control.GetCurrentPageControlUniqueID() }, "");
      renderingContext.Writer.AddAttribute (
          HtmlTextWriterAttribute.Onchange,
          renderingContext.Control.Page.ClientScript.GetPostBackEventReference (postBackOptions));

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Input);
      renderingContext.Writer.RenderEndTag();
    }

    private void RenderNavigationIcon (BocListRenderingContext renderingContext, bool isInactive, GoToOption command, int pageIndex)
    {
      var navigateCommandID = renderingContext.Control.ClientID + "_Navigation_" + command;

      if (isInactive || renderingContext.Control.EditModeController.IsRowEditModeActive)
      {
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Id, navigateCommandID);
        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.A);

        var imageUrl = GetResolvedImageUrl (s_inactiveIcons[command]);
        new IconInfo (imageUrl.GetUrl()).Render (renderingContext.Writer, renderingContext.Control);

        renderingContext.Writer.RenderEndTag ();
      }
      else
      {
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Id, navigateCommandID);

        var currentPageControlClientID = renderingContext.Control.GetCurrentPageControlUniqueID().Replace ('$', '_');
        var postBackEvent = string.Format ("$('#{0}').val({1}).trigger('change');", currentPageControlClientID, pageIndex);
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, postBackEvent);

        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");

        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.A);

        var imageUrl = GetResolvedImageUrl (s_activeIcons[command]);
        var icon = new IconInfo (imageUrl.GetUrl ());
        icon.AlternateText = GetResourceManager (renderingContext).GetString (s_alternateTexts[command]);
        icon.Render (renderingContext.Writer, renderingContext.Control);

        renderingContext.Writer.RenderEndTag();
      }

      renderingContext.Writer.Write (c_whiteSpace + c_whiteSpace + c_whiteSpace);
    }

    protected virtual IResourceManager GetResourceManager (BocListRenderingContext renderingContext)
    {
      return GetResourceManager (typeof (ResourceIdentifier), renderingContext.Control.GetResourceManager());
    }

    private IResourceUrl GetResolvedImageUrl (string imageUrl)
    {
      return ResourceUrlFactory.CreateThemedResourceUrl(typeof (BocListNavigationBlockRenderer), ResourceType.Image, imageUrl);
    }
  }
}