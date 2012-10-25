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
using System.Xml;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Web;
using Remotion.Web.Factories;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  [SetUICulture("en-US")]
  public class BocListNavigationBlockRendererTest : BocListRendererTestBase
  {
    private BocListCssClassDefinition _bocListCssClassDefinition;
    private const string c_pageInfo = "Page {0} of {1}";
    private const string c_tripleBlank = HtmlHelper.WhiteSpace + HtmlHelper.WhiteSpace + HtmlHelper.WhiteSpace;

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      _bocListCssClassDefinition = new BocListCssClassDefinition();

      List.Stub (mock => mock.HasNavigator).Return (true);
    }

    [Test]
    public void RenderOnlyPage ()
    {
      List.Stub (mock => mock.CurrentPageIndex).Return (0);
      List.Stub (mock => mock.PageCount).Return (1);

      var renderer = new BocListNavigationBlockRenderer (new ResourceUrlFactory (new ResourceTheme.ClassicBlue ()), _bocListCssClassDefinition);
      renderer.Render (new BocListRenderingContext(HttpContext, Html.Writer, List, new BocColumnRenderer[0]));

      var document = Html.GetResultDocument();

      var div = Html.GetAssertedChildElement (document, "div", 0);
      Html.AssertAttribute (div, "class", _bocListCssClassDefinition.Navigator);

      Html.AssertTextNode (div, string.Format (c_pageInfo, 1, 1) + c_tripleBlank, 0);

      var firstIcon = Html.GetAssertedChildElement (div, "a", 1);
      AssertInactiveIcon (firstIcon, "First");

      Html.AssertTextNode (div, c_tripleBlank, 2);

      var previousIcon = Html.GetAssertedChildElement (div, "a", 3);
      AssertInactiveIcon (previousIcon, "Previous");

      Html.AssertTextNode (div, c_tripleBlank, 4);

      var nextIcon = Html.GetAssertedChildElement (div, "a", 5);
      AssertInactiveIcon (nextIcon, "Next");

      Html.AssertTextNode (div, c_tripleBlank, 6);

      var lastIcon = Html.GetAssertedChildElement (div, "a", 7);
      AssertInactiveIcon (lastIcon, "Last");

      Html.AssertTextNode (div, c_tripleBlank, 8);
    }

    [Test]
    public void RenderFirstPage ()
    {
      List.Stub (mock => mock.CurrentPageIndex).Return (0);
      List.Stub (mock => mock.PageCount).Return (2);

      var renderer = new BocListNavigationBlockRenderer (new ResourceUrlFactory (new ResourceTheme.ClassicBlue ()), _bocListCssClassDefinition);
      renderer.Render (new BocListRenderingContext(HttpContext, Html.Writer, List, new BocColumnRenderer[0]));

      var document = Html.GetResultDocument();

      var div = Html.GetAssertedChildElement (document, "div", 0);
      Html.AssertAttribute (div, "class", _bocListCssClassDefinition.Navigator);

      Html.AssertTextNode (div, string.Format (c_pageInfo, 1, 2) + c_tripleBlank, 0);

      var firstIcon = Html.GetAssertedChildElement (div, "a", 1);
      AssertInactiveIcon (firstIcon, "First");

      Html.AssertTextNode (div, c_tripleBlank, 2);

      var previousIcon = Html.GetAssertedChildElement (div, "a", 3);
      AssertInactiveIcon (previousIcon, "Previous");

      Html.AssertTextNode (div, c_tripleBlank, 4);

      var nextIcon = Html.GetAssertedChildElement (div, "a", 5);
      AssertActiveIcon (nextIcon, "Next", 1);

      Html.AssertTextNode (div, c_tripleBlank, 6);

      var lastIcon = Html.GetAssertedChildElement (div, "a", 7);
      AssertActiveIcon (lastIcon, "Last", 1);

      Html.AssertTextNode (div, c_tripleBlank, 8);
    }

    [Test]
    public void RenderLastPage ()
    {
      List.Stub (mock => mock.CurrentPageIndex).Return (1);
      List.Stub (mock => mock.PageCount).Return (2);

      var renderer = new BocListNavigationBlockRenderer (new ResourceUrlFactory (new ResourceTheme.ClassicBlue ()), _bocListCssClassDefinition);
      renderer.Render (new BocListRenderingContext(HttpContext, Html.Writer, List, new BocColumnRenderer[0]));

      var document = Html.GetResultDocument();

      var div = Html.GetAssertedChildElement (document, "div", 0);
      Html.AssertAttribute (div, "class", _bocListCssClassDefinition.Navigator);

      Html.AssertTextNode (div, string.Format (c_pageInfo, 2, 2) + c_tripleBlank, 0);

      var firstIcon = Html.GetAssertedChildElement (div, "a", 1);
      AssertActiveIcon (firstIcon, "First", 0);

      Html.AssertTextNode (div, c_tripleBlank, 2);

      var previousIcon = Html.GetAssertedChildElement (div, "a", 3);
      AssertActiveIcon (previousIcon, "Previous", 0);

      Html.AssertTextNode (div, c_tripleBlank, 4);

      var nextIcon = Html.GetAssertedChildElement (div, "a", 5);
      AssertInactiveIcon (nextIcon, "Next");

      Html.AssertTextNode (div, c_tripleBlank, 6);

      var lastIcon = Html.GetAssertedChildElement (div, "a", 7);
      AssertInactiveIcon (lastIcon, "Last");

      Html.AssertTextNode (div, c_tripleBlank, 8);
    }

    [Test]
    public void RenderMiddlePage ()
    {
      List.Stub (mock => mock.CurrentPageIndex).Return (3);
      List.Stub (mock => mock.PageCount).Return (7);

      var renderer = new BocListNavigationBlockRenderer (new ResourceUrlFactory (new ResourceTheme.ClassicBlue ()), _bocListCssClassDefinition);
      renderer.Render (new BocListRenderingContext(HttpContext, Html.Writer, List, new BocColumnRenderer[0]));

      var document = Html.GetResultDocument();

      var div = Html.GetAssertedChildElement (document, "div", 0);
      Html.AssertAttribute (div, "class", _bocListCssClassDefinition.Navigator);

      Html.AssertTextNode (div, string.Format (c_pageInfo, 4, 7) + c_tripleBlank, 0);

      var firstIcon = Html.GetAssertedChildElement (div, "a", 1);
      AssertActiveIcon (firstIcon, "First", 0);

      Html.AssertTextNode (div, c_tripleBlank, 2);

      var previousIcon = Html.GetAssertedChildElement (div, "a", 3);
      AssertActiveIcon (previousIcon, "Previous", 2);

      Html.AssertTextNode (div, c_tripleBlank, 4);

      var nextIcon = Html.GetAssertedChildElement (div, "a", 5);
      AssertActiveIcon (nextIcon, "Next", 4);

      Html.AssertTextNode (div, c_tripleBlank, 6);

      var lastIcon = Html.GetAssertedChildElement (div, "a", 7);
      AssertActiveIcon (lastIcon, "Last", 6);

      Html.AssertTextNode (div, c_tripleBlank, 8);
    }

    private void AssertActiveIcon (XmlNode link, string command, int pageIndex)
    {
      Html.AssertAttribute (link, "id", List.ClientID + "_Navigation_" + command);
      Html.AssertAttribute (link, "onclick", string.Format("$('#CurrentPageControl_UniqueID').val({0}).trigger('change');", pageIndex));
      Html.AssertAttribute (link, "href", "#");

      var icon = Html.GetAssertedChildElement (link, "img", 0);
      Html.AssertAttribute (icon, "src", string.Format ("/Move{0}.gif", command), HtmlHelper.AttributeValueCompareMode.Contains);
    }

    private void AssertInactiveIcon (XmlNode link, string command)
    {
      Html.AssertAttribute (link, "id", List.ClientID + "_Navigation_" + command);
      Html.AssertNoAttribute(link, "onclick");
      Html.AssertNoAttribute (link, "href");

      var icon = Html.GetAssertedChildElement (link, "img", 0);
      Html.AssertAttribute (icon, "src", string.Format ("/Move{0}Inactive.gif", command), HtmlHelper.AttributeValueCompareMode.Contains);
    }
  }
}