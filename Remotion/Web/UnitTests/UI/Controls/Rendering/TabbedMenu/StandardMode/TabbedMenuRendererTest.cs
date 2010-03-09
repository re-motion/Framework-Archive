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
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using NUnit.Framework;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls.TabbedMenuImplementation;
using Remotion.Web.UI.Controls.TabbedMenuImplementation.Rendering;
using Remotion.Web.UI.Controls.WebTabStripImplementation;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.UI.Controls.Rendering.TabbedMenu.StandardMode
{
  [TestFixture]
  public class TabbedMenuRendererTest : RendererTestBase
  {
    private ITabbedMenu _control;

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      _control = MockRepository.GenerateStub<ITabbedMenu>();
      _control.Stub (stub => stub.ClientID).Return ("MyTabbedMenu");
      _control.Stub (stub => stub.MainMenuTabStrip).Return (MockRepository.GenerateStub<IWebTabStrip>());
      _control.Stub (stub => stub.SubMenuTabStrip).Return (MockRepository.GenerateStub<IWebTabStrip> ());

      StateBag stateBag = new StateBag ();
      _control.Stub (stub => stub.Attributes).Return (new AttributeCollection (stateBag));
      _control.Stub (stub => stub.ControlStyle).Return (new Style (stateBag));
      _control.Stub (stub => stub.StatusStyle).Return (new Style (stateBag));

      _control.SubMenuTabStrip.Stub (stub => stub.ControlStyle).Return (new Style (stateBag));
      _control.SubMenuTabStrip.Stub (stub => stub.Style).Return (_control.SubMenuTabStrip.ControlStyle.GetStyleAttributes(_control));

      IPage pageStub = MockRepository.GenerateStub<IPage>();
      _control.Stub (stub => stub.Page).Return (pageStub);
    }

    [Test]
    public void RenderEmptyMenu ()
    {
      AssertControl (false, false, false);
    }

    [Test]
    public void RenderEmptyMenuInDesignMode ()
    {
      _control.Stub (stub => stub.IsDesignMode).Return (true);
      AssertControl (true, false, false);
    }

    [Test]
    public void RenderEmptyMenuWithStatusText ()
    {
      _control.Stub (stub => stub.StatusText).Return ("Status");
      AssertControl (false, true, false);
    }

    [Test]
    public void RenderEmptyMenuWithStatusTextInDesignMode ()
    {
      _control.Stub (stub => stub.IsDesignMode).Return (true);
      _control.Stub (stub => stub.StatusText).Return ("Status");
      AssertControl (true, true, false);
    }

    [Test]
    public void RenderEmptyMenuWithCssClass ()
    {
      _control.CssClass = "CustomCssClass";
      AssertControl (false, false, true);
    }

    [Test]
    public void RenderEmptyMenuWithBackgroundColor ()
    {
      _control.Stub (stub => stub.SubMenuBackgroundColor).Return (Color.Yellow);
      AssertControl (false, false, false);
    }

    private void AssertControl (bool isDesignMode, bool hasStatusText, bool hasCssClass)
    {
      var renderer = new TabbedMenuRenderer (HttpContext, _control);
      renderer.Render (Html.Writer);
      // _control.RenderControl (Html.Writer);

      var document = Html.GetResultDocument();
      var table = document.GetAssertedChildElement ("table", 0);
      table.AssertAttributeValueEquals ("class", hasCssClass ? "CustomCssClass" : "tabbedMenu");
      if (isDesignMode)
        table.AssertStyleAttribute ("width", "100%");
      table.AssertChildElementCount (2);

      var trMainMenu = table.GetAssertedChildElement ("tr", 0);
      trMainMenu.AssertChildElementCount (1);

      var tdMainMenu = trMainMenu.GetAssertedChildElement ("td", 0);
      tdMainMenu.AssertAttributeValueEquals ("colspan", "2");
      tdMainMenu.AssertAttributeValueEquals ("class", "tabbedMainMenuCell");
      tdMainMenu.AssertChildElementCount (0);

      var trSubMenu = table.GetAssertedChildElement ("tr", 1);
      trSubMenu.AssertChildElementCount (2);

      var tdSubMenu = trSubMenu.GetAssertedChildElement ("td", 0);
      tdSubMenu.AssertAttributeValueEquals ("class", "tabbedSubMenuCell");
      if (!_control.SubMenuBackgroundColor.IsEmpty)
        tdSubMenu.AssertStyleAttribute ("background-color", ColorTranslator.ToHtml (Color.Yellow));
      tdSubMenu.AssertChildElementCount (0);

      var tdMenuStatus = trSubMenu.GetAssertedChildElement ("td", 1);
      tdMenuStatus.AssertAttributeValueEquals ("class", "tabbedMenuStatusCell");
      tdMenuStatus.AssertChildElementCount (0);
      tdMenuStatus.AssertTextNode (hasStatusText ? "Status" : "&nbsp;", 0);
    }
  }
}
