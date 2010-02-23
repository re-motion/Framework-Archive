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
using System.Xml;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering.TabbedMultiView;
using Remotion.Web.UI.Controls.Rendering.TabbedMultiView.StandardMode;
using Remotion.Web.UI.Controls.Rendering.WebTabStrip;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.UI.Controls.Rendering.TabbedMultiView.StandardMode
{
  [TestFixture]
  public class TabbedMultiViewRendererTest : RendererTestBase
  {
    private const string c_cssClass = "SomeCssClass";

    private ITabbedMultiView _control;

    [SetUp]
    public void SetUp ()
    {
      ServiceLocator.SetLocatorProvider (() => new StubServiceLocator());
      Initialize();
      _control = MockRepository.GenerateStub<ITabbedMultiView>();
      _control.Stub (stub => stub.ClientID).Return ("MyTabbedMultiView");
      _control.Stub (stub => stub.TopControl).Return (new PlaceHolder { ID = "MyTabbedMultiView_TopControl" });
      _control.Stub (stub => stub.BottomControl).Return (new PlaceHolder { ID = "MyTabbedMultiView_BottomControl" });

      var tabStrip = MockRepository.GenerateStub<IWebTabStrip>();
      tabStrip.Stub (stub => stub.RenderControl (Html.Writer)).WhenCalled (
          delegate (MethodInvocation obj)
          {
            HtmlTextWriter writer = (HtmlTextWriter) obj.Arguments[0];
            writer.AddAttribute (HtmlTextWriterAttribute.Class, tabStrip.CssClass);
            writer.RenderBeginTag ("tabStrip");
            writer.RenderEndTag ();
          });

      _control.Stub (stub => stub.TabStrip).Return (tabStrip);

      _control.Stub (stub => stub.ActiveViewClientID).Return (_control.ClientID + "_ActiveView");
      _control.Stub (stub => stub.ActiveViewContentClientID).Return (_control.ActiveViewClientID + "_Content");
      _control.Stub (stub => stub.WrapperClientID).Return ("WrapperClientID");
      

      StateBag stateBag = new StateBag ();
      _control.Stub (stub => stub.Attributes).Return (new AttributeCollection (stateBag));
      _control.Stub (stub => stub.TopControlsStyle).Return (new Style (stateBag));
      _control.Stub (stub => stub.BottomControlsStyle).Return (new Style (stateBag));
      _control.Stub (stub => stub.ActiveViewStyle).Return (new WebTabStyle ());
      _control.Stub (stub => stub.ControlStyle).Return (new Style (stateBag));

      var clientScriptStub = MockRepository.GenerateStub<IClientScriptManager> ();

      var pageStub = MockRepository.GenerateStub<IPage> ();
      pageStub.Stub (stub => stub.ClientScript).Return (clientScriptStub);

      _control.Stub (stub => stub.Page).Return (pageStub);
    }

    [Test]
    public void RenderEmptyControl ()
    {
      AssertControl (false, false, false, true);
    }

    [Test]
    public void RenderEmptyControlWithCssClass ()
    {
      _control.CssClass = c_cssClass;
      _control.TopControlsStyle.CssClass = c_cssClass;
      _control.ActiveViewStyle.CssClass = c_cssClass;
      _control.BottomControlsStyle.CssClass = c_cssClass;

      AssertControl (true, false, false, true);
    }

    [Test]
    public void RenderEmptyControlWithCssClassInAttributes ()
    {
      _control.Attributes["class"] = c_cssClass;
      _control.TopControlsStyle.CssClass = c_cssClass;
      _control.ActiveViewStyle.CssClass = c_cssClass;
      _control.BottomControlsStyle.CssClass = c_cssClass;

      AssertControl (true, true, false, true);
    }

    [Test]
    public void RenderEmptyControlInDesignMode ()
    {
      _control.Stub (stub => stub.IsDesignMode).Return (true);

      AssertControl (false, false, true, true);
    }

    [Test]
    public void RenderEmptyControlWithCssClassInDesignMode ()
    {
      _control.Stub (stub => stub.IsDesignMode).Return (true);
      _control.CssClass = c_cssClass;
      _control.TopControlsStyle.CssClass = c_cssClass;
      _control.ActiveViewStyle.CssClass = c_cssClass;
      _control.BottomControlsStyle.CssClass = c_cssClass;

      AssertControl (true, false,true, true);
    }

    [Test]
    public void RenderPopulatedControl ()
    {
      PopulateControl();

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

      AssertControl (true, false, false, false);
    }

    [Test]
    public void RenderPopulatedControlInDesignMode ()
    {
      PopulateControl ();
      _control.Stub (stub => stub.IsDesignMode).Return (true);

      AssertControl (false, false, true, false);
    }

    [Test]
    public void RenderPopulatedControlWithCssClassInDesignMode ()
    {
      PopulateControl();

      _control.Stub (stub => stub.IsDesignMode).Return (true);
      _control.CssClass = c_cssClass;
      _control.TopControlsStyle.CssClass = c_cssClass;
      _control.ActiveViewStyle.CssClass = c_cssClass;
      _control.BottomControlsStyle.CssClass = c_cssClass;

      AssertControl (true, false, true, false);
    }

    private void PopulateControl ()
    {
      _control.TopControl.Controls.Add (new LiteralControl ("TopControls"));
      
      var view1 = new TabView { ID="View1ID", Title = "View1Title" };
      view1.LazyControls.Add (new LiteralControl ("View1Contents"));
      _control.Stub(stub=>stub.GetActiveView()).Return (view1);

      _control.BottomControl.Controls.Add (new LiteralControl ("BottomControls"));
    }

    private void AssertControl (bool withCssClass, bool inAttributes, bool isDesignMode, bool isEmpty)
    {
      var renderer = new TabbedMultiViewRenderer (HttpContext, Html.Writer, _control);
      renderer.Render (Html.Writer);

      var container = GetAssertedContainerElement (withCssClass, inAttributes, isDesignMode, renderer);
      AssertTopControls (container, withCssClass, isEmpty, renderer);
      AssertTabStrip (container, renderer);
      AssertView (container, withCssClass, isDesignMode, renderer);
      AssertBottomControls (container, withCssClass, isEmpty, renderer);
    }

    private XmlNode GetAssertedContainerElement (bool withCssClass, bool inAttributes, bool isDesignMode, TabbedMultiViewRenderer renderer)
    {
      string cssClass = renderer.CssClassBase;
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

      var contentDiv = outerDiv.GetAssertedChildElement ("div", 0);
      contentDiv.AssertAttributeValueEquals ("class", renderer.CssClassWrapper);

      return contentDiv;
    }

    private void AssertBottomControls (XmlNode container, bool withCssClass, bool isEmpty, TabbedMultiViewRenderer renderer)
    {
      string cssClass = renderer.CssClassBottomControls;
      if (withCssClass)
        cssClass = c_cssClass;

      var divBottomControls = container.GetAssertedChildElement ("div", 3);

      divBottomControls.AssertAttributeValueEquals ("id", _control.BottomControl.ClientID);
      divBottomControls.AssertAttributeValueContains ("class", cssClass);
      if( isEmpty )
        divBottomControls.AssertAttributeValueContains ("class", renderer.CssClassEmpty);

      divBottomControls.AssertChildElementCount (1);

      var divContent = divBottomControls.GetAssertedChildElement ("div", 0);
      divContent.AssertAttributeValueEquals ("class", renderer.CssClassContent);

      if (!isEmpty)
        divContent.AssertTextNode ("BottomControls", 0);
    }

    private void AssertView (XmlNode container, bool withCssClass, bool isDesignMode, TabbedMultiViewRenderer renderer)
    {
      string cssClassActiveView = renderer.CssClassActiveView;
      if (withCssClass)
        cssClassActiveView = c_cssClass;

      var divActiveView = container.GetAssertedChildElement ("div", 2);
      divActiveView.AssertAttributeValueEquals ("id", _control.ActiveViewClientID);
      divActiveView.AssertAttributeValueEquals ("class", cssClassActiveView);
      if( isDesignMode )
        divActiveView.AssertStyleAttribute ("border", "solid 1px black");
      divActiveView.AssertChildElementCount (1);

      var divContentBorder = divActiveView.GetAssertedChildElement ("div", 0);
      divContentBorder.AssertAttributeValueEquals ("class", renderer.CssClassContentBorder);

      var divContent = divContentBorder.GetAssertedChildElement ("div", 0);
      divContent.AssertAttributeValueEquals ("class", renderer.CssClassContent);
    }

    private void AssertTabStrip (XmlNode container, TabbedMultiViewRenderer renderer)
    {
      var divTabStrip = container.GetAssertedChildElement ("tabStrip", 1);
      divTabStrip.AssertChildElementCount (0);

      divTabStrip.AssertAttributeValueEquals ("class", renderer.CssClassTabStrip);
    }

    private void AssertTopControls (XmlNode container, bool withCssClass, bool isEmpty, TabbedMultiViewRenderer renderer)
    {
      string cssClass = renderer.CssClassTopControls;
      if (withCssClass)
        cssClass = c_cssClass;

      var divTopControls = container.GetAssertedChildElement ("div", 0);
      divTopControls.AssertAttributeValueEquals ("id", _control.TopControl.ClientID);
      divTopControls.AssertAttributeValueContains ("class", cssClass);
      if (isEmpty)
        divTopControls.AssertAttributeValueContains ("class", renderer.CssClassEmpty);

      divTopControls.AssertChildElementCount (1);

      var divContent = divTopControls.GetAssertedChildElement ("div", 0);
      divContent.AssertAttributeValueEquals ("class", renderer.CssClassContent);

      if (!isEmpty)
        divContent.AssertTextNode ("TopControls", 0);
    }
  }
}
