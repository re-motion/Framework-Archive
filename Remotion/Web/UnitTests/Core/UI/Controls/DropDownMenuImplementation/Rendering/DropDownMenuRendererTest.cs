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
using System.Xml;
using NUnit.Framework;
using Remotion.Web.Factories;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.DropDownMenuImplementation;
using Remotion.Web.UI.Controls.DropDownMenuImplementation.Rendering;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.UI.Controls.DropDownMenuImplementation.Rendering
{
  [TestFixture]
  public class DropDownMenuRendererTest : RendererTestBase
  {
    private const string c_MenuTitle = "MenuTitle";
    private const string c_Icon_Url = "/Image/icon.gif";
    private const string c_IconAlternateText = "Icon_AlternateText";
    private const string c_Icon_ToolTip = "Icon_ToolTip";

    private static readonly Unit s_iconWidth = Unit.Pixel (16);
    private static readonly Unit s_iconHeight = Unit.Pixel (12);
    private static readonly IconInfo s_titleIcon = new IconInfo (c_Icon_Url, c_IconAlternateText, c_Icon_ToolTip, s_iconWidth, s_iconHeight);

    private IDropDownMenu _control;
    private HttpContextBase _httpContextStub;
    private HtmlHelper _htmlHelper;

    [SetUp]
    public void SetUp ()
    {
      _htmlHelper = new HtmlHelper ();
      _httpContextStub = MockRepository.GenerateStub<HttpContextBase>();

      _control = MockRepository.GenerateStub<IDropDownMenu> ();
      _control.ID = "DropDownMenu1";
      _control.Stub (stub => stub.Enabled).Return (true);
      _control.Stub (stub => stub.UniqueID).Return ("DropDownMenu1");
      _control.Stub (stub => stub.ClientID).Return ("DropDownMenu1");
      _control.Stub (stub => stub.MenuItems).Return (new WebMenuItemCollection (_control));
      _control.Stub (stub => stub.GetBindOpenEventScript (null, null, true)).IgnoreArguments ().Return ("OpenDropDownMenuEventReference");
      _control.Stub (stub => stub.ResolveClientUrl (null)).IgnoreArguments ().Do ((Func<string, string>) (url => url.TrimStart ('~')));

      IPage pageStub = MockRepository.GenerateStub<IPage> ();
      _control.Stub (stub => stub.Page).Return (pageStub);

      StateBag stateBag = new StateBag ();
      _control.Stub (stub => stub.Attributes).Return (new AttributeCollection (stateBag));
      _control.Stub (stub => stub.ControlStyle).Return (new Style (stateBag));

      IClientScriptManager scriptManagerMock = MockRepository.GenerateMock<IClientScriptManager> ();
      _control.Page.Stub (stub => stub.ClientScript).Return (scriptManagerMock);
      
    }

    [Test]
    public void RenderEmptyMenuWithoutTitle ()
    {
      XmlNode containerDiv = GetAssertedContainerSpan();
      AssertTitleSpan(containerDiv, false, false);
    }

    [Test]
    public void RenderEmptyMenuWithTitle ()
    {
      _control.Stub(stub=>stub.TitleText).Return(c_MenuTitle);

      XmlNode containerDiv = GetAssertedContainerSpan ();
      AssertTitleSpan (containerDiv, true, false);
    }

    [Test]
    public void RenderEmptyMenuWithTitleAndIcon ()
    {
      _control.Stub (stub => stub.TitleText).Return (c_MenuTitle);
      _control.Stub (stub => stub.TitleIcon).Return (s_titleIcon);

      XmlNode containerDiv = GetAssertedContainerSpan ();
      AssertTitleSpan (containerDiv, true, true);
    }

    [Test]
    public void RenderPopulatedMenu ()
    {
      PopulateMenu();

      XmlNode containerDiv = GetAssertedContainerSpan ();
      AssertTitleSpan (containerDiv, false, false);
    }

    private void AssertTitleSpan (XmlNode containerDiv, bool withTitle, bool withIcon)
    {
      var titleDiv = containerDiv.GetAssertedChildElement ("span", 0);
      titleDiv.AssertAttributeValueEquals ("class", "DropDownMenuSelect");
      titleDiv.AssertChildElementCount (2);

      AssertTitleAnchor(titleDiv, withTitle, withIcon);
      AssertDropDownButton(titleDiv);
    }

    private void AssertDropDownButton (XmlNode titleDiv)
    {
      var span = titleDiv.GetAssertedChildElement ("span", 1);
      string imageFileName = _control.Enabled ? "DropDownMenuArrow.gif" : "DropDownMenuArrow_disabled.gif";
      string imageFilePath = "/res/Remotion.Web/Themes/ClassicBlue/Image/" + imageFileName;
      string styleValue = string.Format ("url({0})", imageFilePath);

      span.AssertStyleAttribute ("background-image", styleValue);

      var image = span.GetAssertedChildElement ("img", 0);
      image.AssertAttributeValueEquals ("src", IconInfo.Spacer.Url);
      image.AssertAttributeValueEquals ("alt", "");
    }

    private void AssertTitleAnchor (XmlNode titleDiv, bool withTitle, bool withIcon)
    {
      var titleAnchor = titleDiv.GetAssertedChildElement ("a", 0);
      titleAnchor.AssertAttributeValueEquals ("href", "#");
      titleAnchor.AssertAttributeValueEquals ("onclick", "return false;");
      titleAnchor.AssertChildElementCount (withIcon ? 1 : 0);
      if (withTitle)
        titleAnchor.AssertTextNode (c_MenuTitle + HtmlHelper.WhiteSpace, withIcon ? 1 : 0);

      if (withIcon)
      {
        var icon = titleAnchor.GetAssertedChildElement ("img", 0);
        icon.AssertAttributeValueEquals ("src", c_Icon_Url);
        icon.AssertAttributeValueEquals ("alt", c_IconAlternateText);
        icon.AssertAttributeValueEquals ("width", s_iconWidth.ToString ());
        icon.AssertAttributeValueEquals ("height", s_iconHeight.ToString ());
      }
    }

    private XmlNode GetAssertedContainerSpan ()
    {
      var renderer = new DropDownMenuRenderer (_httpContextStub, _control, new ResourceUrlFactory (ResourceTheme.ClassicBlue));
      renderer.Render (_htmlHelper.Writer);
      var document = _htmlHelper.GetResultDocument();
      var containerDiv = document.GetAssertedChildElement ("span", 0);
      containerDiv.AssertAttributeValueEquals ("id", _control.ClientID);
      containerDiv.AssertAttributeValueEquals ("class", "DropDownMenuContainer");
      containerDiv.AssertChildElementCount (1);

      return containerDiv;
    }

    private void PopulateMenu ()
    {
      AddItem (0, "Category1", CommandType.Event, false, true);
      AddItem (1, "Category1", CommandType.Href, false, true);
      AddItem (2, "Category2", CommandType.WxeFunction, false, true);
      AddItem (3, "Category2", CommandType.WxeFunction, true, true);
      AddItem (4, "Category2", CommandType.WxeFunction, false, false);
    }

    private void AddItem (int index, string category, CommandType commandType, bool isDisabled, bool isVisible)
    {
      string id = "item" + index;
      string text = "Item" + index;
      const string height = "16";
      const string width = "16";
      const string iconUrl = "~/Images/Icon.gif";
      const string disabledIconUrl = "~/Images/DisabledIcon.gif";
      const RequiredSelection requiredSelection = RequiredSelection.Any;

      Command command = new Command (commandType);
      if (commandType == CommandType.Href)
      {
        command.HrefCommand.Href = "~/Target.aspx?index={0}&itemID={1}";
        command.HrefCommand.Target = "_blank";
      }

      _control.MenuItems.Add (
          new WebMenuItem (
              id,
              category,
              text,
              new IconInfo (iconUrl, text, text, width, height),
              new IconInfo (disabledIconUrl, text, text, width, height),
              WebMenuItemStyle.IconAndText,
              requiredSelection,
              isDisabled,
              command) { IsVisible = isVisible });

      if (commandType == CommandType.Href)
      {
      }
      else
      {
        if (isVisible && _control.Enabled)
        {
          _control.Page.ClientScript.Expect (
              mock => mock.GetPostBackClientHyperlink (Arg.Is<IControl> (_control), Arg.Text.Like (index.ToString ())))
              .Return ("PostBackHyperLink:" + index);
        }
      }
    }
  }
}