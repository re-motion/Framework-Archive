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
using System.Xml;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Remotion.Web.UI.Controls;
using Remotion.Web.UnitTests.UI.Controls.Rendering.WebTabStrip;

namespace Remotion.Web.UnitTests.UI.Controls.Rendering.TabbedMultiView.QuirksMode
{
  [TestFixture]
  public class TabbedMultiViewRendererTest : RendererTestBase
  {
    private const string c_cssClass = "SomeCssClass";

    private TabbedMultiViewMock _control;

    [SetUp]
    public void SetUp ()
    {
      ServiceLocator.SetLocatorProvider (() => new StubServiceLocator());
      Initialize();
      _control = new TabbedMultiViewMock();
    }

    [Test]
    public void RenderEmptyControl ()
    {
      _control.RenderControl (Html.Writer);

      AssertControl (false, false, false, true);
    }

    [Test]
    public void RenderEmptyControlWithCssClass ()
    {
      _control.CssClass = c_cssClass;
      _control.TopControlsStyle.CssClass = c_cssClass;
      _control.ActiveViewStyle.CssClass = c_cssClass;
      _control.BottomControlsStyle.CssClass = c_cssClass;

      _control.RenderControl (Html.Writer);

      AssertControl (true, false, false, true);
    }

    [Test]
    public void RenderEmptyControlWithCssClassInAttributes ()
    {
      _control.Attributes["class"] = c_cssClass;
      _control.TopControlsStyle.CssClass = c_cssClass;
      _control.ActiveViewStyle.CssClass = c_cssClass;
      _control.BottomControlsStyle.CssClass = c_cssClass;

      _control.RenderControl (Html.Writer);

      AssertControl (true, true, false, true);
    }

    [Test]
    public void RenderEmptyControlInDesignMode ()
    {
      _control.SetDesignMode (true);

      _control.RenderControl (Html.Writer);

      AssertControl (false, false, true, true);
    }

    [Test]
    public void RenderEmptyControlWithCssClassInDesignMode ()
    {
      _control.SetDesignMode (true);
      _control.CssClass = c_cssClass;
      _control.TopControlsStyle.CssClass = c_cssClass;
      _control.ActiveViewStyle.CssClass = c_cssClass;
      _control.BottomControlsStyle.CssClass = c_cssClass;

      _control.RenderControl (Html.Writer);

      AssertControl (true, false,true, true);
    }

    [Test]
    public void RenderPopulatedControl ()
    {
      PopulateControl();

      _control.RenderControl (Html.Writer);

      AssertControl (false, false, false, false);
    }

    [Test]
    public void RenderPopulatedControlWithCssClass ()
    {
      PopulateControl();

      _control.CssClass = c_cssClass;
      _control.TopControlsStyle.CssClass = c_cssClass;
      _control.ActiveViewStyle.CssClass = c_cssClass;
      _control.BottomControlsStyle.CssClass = c_cssClass;

      _control.RenderControl (Html.Writer);

      AssertControl (true, false, false, false);
    }

    [Test]
    public void RenderPopulatedControlInDesignMode ()
    {
      PopulateControl ();
      _control.SetDesignMode (true);

      _control.RenderControl (Html.Writer);

      AssertControl (false, false, true, false);
    }

    [Test]
    public void RenderPopulatedControlWithCssClassInDesignMode ()
    {
      PopulateControl();

      _control.SetDesignMode (true);
      _control.CssClass = c_cssClass;
      _control.TopControlsStyle.CssClass = c_cssClass;
      _control.ActiveViewStyle.CssClass = c_cssClass;
      _control.BottomControlsStyle.CssClass = c_cssClass;

      _control.RenderControl (Html.Writer);

      AssertControl (true, false, true, false);
    }

    private void PopulateControl ()
    {
      _control.TopControls.Add (new LiteralControl ("TopControls"));
      var view1 = new TabView { ID="View1ID", Title = "View1Title" };
      view1.LazyControls.Add (new LiteralControl ("View1Contents"));

      var view2 = new TabView { ID = "View2ID", Title = "View2Title" };
      view2.LazyControls.Add (new LiteralControl ("View2Contents"));
      
      _control.Views.Add (view1);
      _control.Views.Add (view2);
      _control.BottomControls.Add (new LiteralControl ("BottomControls"));
    }

    private void AssertControl (bool withCssClass, bool inAttributes, bool isDesignMode, bool isEmpty)
    {
      var table = GetAssertedTableElement (withCssClass, inAttributes, isDesignMode);
      AssertTopRow (table, withCssClass, isEmpty);
      AssertTabStripRow (table);
      AssertViewRow (table, withCssClass, isDesignMode);
      AssertBottomRow (table, withCssClass, isEmpty);
    }

    private XmlNode GetAssertedTableElement (bool withCssClass, bool inAttributes, bool isDesignMode)
    {
      string cssClass = _control.CssClassBase;
      if (withCssClass)
      {
        cssClass = inAttributes ? _control.Attributes["class"] : _control.CssClass;
      }

      var document = Html.GetResultDocument();
      var outerDiv = document.GetAssertedChildElement ("div", 0);
      
      outerDiv.AssertAttributeValueEquals ("class", cssClass);
      if (isDesignMode)
      {
        outerDiv.AssertStyleAttribute ("width", "100%");
        outerDiv.AssertStyleAttribute ("height", "75%");
      }
      outerDiv.AssertChildElementCount (1);

      var table = outerDiv.GetAssertedChildElement ("table", 0);
      table.AssertAttributeValueEquals ("class", cssClass);
      table.AssertChildElementCount (4);
      return table;
    }

    private void AssertBottomRow (XmlNode table, bool withCssClass, bool isEmpty)
    {
      string cssClass = _control.CssClassBottomControls;
      if (withCssClass)
        cssClass = c_cssClass;

      var trBottom = table.GetAssertedChildElement ("tr", 3);
      trBottom.AssertChildElementCount (1);

      var tdBottom = trBottom.GetAssertedChildElement ("td", 0);
      tdBottom.AssertAttributeValueContains ("class", cssClass);
      if( isEmpty )
        tdBottom.AssertAttributeValueContains ("class", _control.CssClassEmpty);
      tdBottom.AssertChildElementCount (1);

      var divBottomControl = tdBottom.GetAssertedChildElement ("div", 0);
      divBottomControl.AssertAttributeValueEquals ("id", _control.ClientID + "_BottomControl");
      divBottomControl.AssertAttributeValueEquals ("class", cssClass);
      divBottomControl.AssertChildElementCount (1);

      var divBottomContent = divBottomControl.GetAssertedChildElement ("div", 0);
      divBottomContent.AssertAttributeValueEquals ("class", _control.CssClassContent);
      divBottomContent.AssertChildElementCount (0);
    }

    private void AssertViewRow (XmlNode table, bool withCssClass, bool isDesignMode)
    {
      string cssClassActiveView = _control.CssClassActiveView;
      if (withCssClass)
        cssClassActiveView = c_cssClass;

      var trActiveView = table.GetAssertedChildElement ("tr", 2);
      trActiveView.AssertChildElementCount (1);

      var tdActiveView = trActiveView.GetAssertedChildElement ("td", 0);
      tdActiveView.AssertAttributeValueEquals ("class", cssClassActiveView);
      if( isDesignMode )
        tdActiveView.AssertStyleAttribute ("border", "solid 1px black");

      tdActiveView.AssertChildElementCount (1);

      var divActiveView = tdActiveView.GetAssertedChildElement ("div", 0);
      divActiveView.AssertAttributeValueEquals ("id", _control.ClientID + "_ActiveView");
      divActiveView.AssertAttributeValueEquals ("class", cssClassActiveView);
      divActiveView.AssertChildElementCount (1);

      var divBody = divActiveView.GetAssertedChildElement ("div", 0);
      divBody.AssertAttributeValueEquals ("class", _control.CssClassViewBody);
      divBody.AssertChildElementCount (1);

      var divActiveViewContent = divBody.GetAssertedChildElement ("div", 0);
      divActiveViewContent.AssertAttributeValueEquals ("id", _control.ClientID + "_ActiveView_Content");
      divActiveViewContent.AssertAttributeValueEquals ("class", _control.CssClassContent);
      divActiveViewContent.AssertChildElementCount (0);
    }

    private void AssertTabStripRow (XmlNode table)
    {
      string cssClass = _control.CssClassTabStrip;

      var trTabStrip = table.GetAssertedChildElement ("tr", 1);
      trTabStrip.AssertChildElementCount (1);

      var tdTabStrip = trTabStrip.GetAssertedChildElement ("td", 0);
      tdTabStrip.AssertAttributeValueEquals ("class", cssClass);
      tdTabStrip.AssertChildElementCount (0);
    }

    private void AssertTopRow (XmlNode table, bool withCssClass, bool isEmpty)
    {
      string cssClass = _control.CssClassTopControls;
      if (withCssClass)
        cssClass = c_cssClass;

      var trTop = table.GetAssertedChildElement ("tr", 0);
      trTop.AssertChildElementCount (1);

      var tdTop = trTop.GetAssertedChildElement ("td", 0);
      tdTop.AssertAttributeValueContains ("class", cssClass);
      if( isEmpty )
        tdTop.AssertAttributeValueContains ("class", _control.CssClassEmpty);

      tdTop.AssertChildElementCount (1);

      var divTopControl = tdTop.GetAssertedChildElement ("div", 0);
      divTopControl.AssertAttributeValueEquals ("id", _control.ClientID + "_TopControl");
      divTopControl.AssertAttributeValueEquals ("class", cssClass);
      divTopControl.AssertChildElementCount (1);

      var divTopContent = divTopControl.GetAssertedChildElement ("div", 0);
      divTopContent.AssertAttributeValueEquals ("class", _control.CssClassContent);
      divTopContent.AssertChildElementCount (0);
    }
  }
}