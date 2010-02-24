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
using System.Xml;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocReferenceValue.StandardMode;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocReferenceValue.StandardMode
{
  [TestFixture]
  public class BocReferenceValueRendererTest : BocReferenceValueRendererTestBase
  {
    [Test]
    public void RenderNullReferenceValue ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);

      XmlNode containerDiv = GetAssertedContainerSpan(false);
      AssertControl(containerDiv, false, false);
    }

    [Test]
    public void RenderNullReferenceValueWithOptionsMenu ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetUpClientScriptExpectations();
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      
      XmlNode containerDiv = GetAssertedContainerSpan (false);
      AssertControl (containerDiv, false, false);
    }

    [Test]
    public void RenderNullReferenceValueWithEmbeddedOptionsMenu ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      Control.Stub (stub => stub.HasValueEmbeddedInsideOptionsMenu).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);

      XmlNode span = GetAssertedContainerSpan (false);
      AssertControl (span, false, false);
    }

    [Test]
    public void RenderNullReferenceValueWithEmbeddedOptionsMenuAndStyle ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      Control.Stub (stub => stub.HasValueEmbeddedInsideOptionsMenu).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      AddStyle ();

      XmlNode span = GetAssertedContainerSpan (true);
      AssertControl (span, true, false);
    }

    [Test]
    public void RenderNullReferenceValueReadOnly ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      XmlNode span = GetAssertedContainerSpan (false);
      AssertReadOnlyContent (span, false);
    }

    [Test]
    public void RenderNullReferenceValueReadOnlyWithStyle ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);
      AddStyle ();

      XmlNode span = GetAssertedContainerSpan (true);
      AssertReadOnlyContent (span, true);
    }

    [Test]
    public void RenderNullReferenceValueReadOnlyWithOptionsMenu ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      XmlNode span = GetAssertedContainerSpan (false);
      AssertReadOnlyContent (span, false);

      Assert.That (OptionsMenu.Style["width"], Is.Null);
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderNullReferenceValueWithStyle ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      AddStyle ();

      XmlNode span = GetAssertedContainerSpan (true);
      AssertControl (span, true, false);
    }

    [Test]
    public void RenderNullReferenceValueWithOptionsAndStyle ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      AddStyle ();

      XmlNode span = GetAssertedContainerSpan (true);
      AssertControl (span, true, false);

      Assert.That (OptionsMenu.Style["width"], Is.Null);
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderNullReferenceValueWithIcon ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.Property).Return (
          (IBusinessObjectReferenceProperty) ((IBusinessObject) BusinessObject).BusinessObjectClass.GetPropertyDefinition ("ReferenceValue"));
      SetUpGetIconExpectations ();

      XmlNode span = GetAssertedContainerSpan (false);
      AssertControl (span, false, true);
    }

    [Test]
    public void RenderReferenceValue ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetUpClientScriptExpectations ();
      SetValue ();
      XmlNode span = GetAssertedContainerSpan (false);
      AssertControl (span, false, false);
    }

    [Test]
    public void RenderReferenceValueAutoPostback ()
    {
      DropDownList.AutoPostBack = false;
      Control.Stub (stub => stub.Enabled).Return (true);
      SetUpClientScriptExpectations ();
      SetValue ();

      Control.Stub (stub => stub.DropDownListStyle).Return (new DropDownListStyle());
      Control.DropDownListStyle.AutoPostBack = true;

      XmlNode span = GetAssertedContainerSpan (false);
      AssertControl (span, false, false);
      Assert.IsTrue (DropDownList.AutoPostBack);
    }

    private void SetValue ()
    {
      BusinessObject.ReferenceValue = BusinessObject.ReferenceList[0];
      Control.Stub (stub => stub.Value).Return ((IBusinessObjectWithIdentity) BusinessObject.ReferenceValue);
    }

    [Test]
    public void RenderReferenceValueWithOptionsMenu ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetUpClientScriptExpectations ();
      SetValue ();
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);

      XmlNode span = GetAssertedContainerSpan (false);
      AssertControl (span, false, false);

      Assert.That (OptionsMenu.Style["width"], Is.Null);
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderReferenceValueWithEmbeddedOptionsMenu ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetValue ();
      Control.Stub (stub => stub.HasValueEmbeddedInsideOptionsMenu).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);

      XmlNode span = GetAssertedContainerSpan (false);
      AssertControl (span, false, false);
    }

    [Test]
    public void RenderReferenceValueWithEmbeddedOptionsMenuAndStyle ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetValue ();
      Control.Stub (stub => stub.HasValueEmbeddedInsideOptionsMenu).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      AddStyle ();

      XmlNode span = GetAssertedContainerSpan (false);
      AssertControl (span, true, false);
    }

    [Test]
    public void RenderReferenceValueReadOnly ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetValue ();
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      XmlNode span = GetAssertedContainerSpan (false);
      AssertReadOnlyContent (span, false);
    }

    [Test]
    public void RenderReferenceValueReadOnlyWithStyle ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetValue ();
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);
      AddStyle ();

      XmlNode span = GetAssertedContainerSpan (true);
      AssertReadOnlyContent (span, true);
    }

    [Test]
    public void RenderReferenceValueReadOnlyWithOptionsMenu ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetValue ();
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      XmlNode span = GetAssertedContainerSpan (false);
      AssertReadOnlyContent (span, false);

      Assert.That (OptionsMenu.Style["width"], Is.Null);
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderReferenceValueWithStyle ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetValue ();
      AddStyle ();

      XmlNode span = GetAssertedContainerSpan (true);
      AssertControl (span, true, false);
    }

    [Test]
    public void RenderReferenceValueWithOptionsAndStyle ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetValue ();
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      AddStyle ();

      XmlNode span = GetAssertedContainerSpan (true);
      AssertControl (span, true, false);

      Assert.That (OptionsMenu.Style["width"], Is.Null);
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderReferenceValueWithIcon ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetValue ();
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.Property).Return (
          (IBusinessObjectReferenceProperty) ((IBusinessObject) BusinessObject).BusinessObjectClass.GetPropertyDefinition ("ReferenceValue"));
      SetUpGetIconExpectations ();

      XmlNode span = GetAssertedContainerSpan (false);
      AssertControl (span, false, true);
    }

    [Test]
    public void RenderOptions ()
    {
      var renderer = new BocReferenceValueRenderer (HttpContext, Control, () => new StubDropDownList ());

      Html.Writer.AddAttribute (HtmlTextWriterAttribute.Class, "bocReferenceValueContent");
      Html.Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      renderer.RenderOptionsMenuTitle (Html.Writer);
      Html.Writer.RenderEndTag ();

      var document = Html.GetResultDocument ();
      AssertControl (document, false, false);
    }

    [Test]
    public void RenderOptionsReadOnly ()
    {
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      var renderer = new BocReferenceValueRenderer (HttpContext, Control, () => new StubDropDownList ());
      Html.Writer.AddAttribute (HtmlTextWriterAttribute.Class, "bocReferenceValueContent");
      Html.Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      renderer.RenderOptionsMenuTitle (Html.Writer);
      Html.Writer.RenderEndTag ();


      var document = Html.GetResultDocument ();
      AssertReadOnlyContent (document, false);
    }

    private void AssertReadOnlyContent (XmlNode parent, bool withStyle)
    {
      var span = parent.GetAssertedChildElement ("span", 0);
      span.AssertAttributeValueEquals ("class", "bocReferenceValueContent");
      span.AssertChildElementCount (Control.HasOptionsMenu ? 2 : 1);

      var commandSpan = span.GetAssertedChildElement ("span", 0);
      commandSpan.AssertAttributeValueEquals ("class", "bocReferenceValueCommand");
      commandSpan.AssertChildElementCount (1);

      var innerSpan = commandSpan.GetAssertedChildElement ("span", 0);
      innerSpan.AssertAttributeValueEquals ("id", Control.LabelClientID);
      innerSpan.AssertChildElementCount (0);
      innerSpan.AssertTextNode ("MyText", 0);

      if(withStyle)
      {
        span.AssertStyleAttribute ("width", "100%");
      }

      if (Control.HasOptionsMenu)
      {
        var wrapperSpan = span.GetAssertedChildElement ("span", 1);
        wrapperSpan.AssertAttributeValueEquals ("class", "bocReferenceValueOptionsMenu");
        wrapperSpan.AssertChildElementCount (0);
        wrapperSpan.AssertTextNode ("DropDownMenu", 0);
      }
    }

    private void AssertControl (XmlNode containerDiv, bool withStyle, bool withIcon)
    {
      var contentDiv = containerDiv.GetAssertedChildElement ("span", 0);
      contentDiv.AssertAttributeValueEquals ("class", "bocReferenceValueContent");

      if (withStyle)
      {
        contentDiv.AssertStyleAttribute ("width", "100%");

        if( !Control.IsReadOnly )
          contentDiv.AssertStyleAttribute ("height", "100%");
      }

      if (withIcon)
        AssertIcon (contentDiv, true);

      var contentSpan = contentDiv.GetAssertedChildElement ("span", withIcon ? 1 : 0);
      contentSpan.AssertAttributeValueEquals ("class", "content");
      contentSpan.AssertTextNode ("DropDownList", 0);

      if (Control.HasOptionsMenu)
      {
        var optionsMenuDiv = contentDiv.GetAssertedChildElement ("span", 1);
        optionsMenuDiv.AssertAttributeValueEquals ("class", "bocReferenceValueOptionsMenu");
        optionsMenuDiv.AssertTextNode ("DropDownMenu", 0);
      }
    }

    private XmlNode GetAssertedContainerSpan (bool withStyle)
    {
      var renderer = new BocReferenceValueRenderer (HttpContext, Control, () => DropDownList);
      renderer.Render (Html.Writer);

      var document = Html.GetResultDocument();
      var containerDiv = document.GetAssertedChildElement ("span", 0);

      containerDiv.AssertAttributeValueEquals ("id", "MyReferenceValue");
      containerDiv.AssertAttributeValueContains ("class", "bocReferenceValue");
      if( Control.IsReadOnly )
        containerDiv.AssertAttributeValueContains ("class", "readOnly");
      if (!Control.Enabled)
        containerDiv.AssertAttributeValueContains ("class", "disabled");

      // containerDiv.AssertChildElementCount (1);

      if (withStyle)
      {
        containerDiv.AssertStyleAttribute ("width", Width.ToString());
        containerDiv.AssertStyleAttribute ("height", Height.ToString ());
      }

      return containerDiv;
    }
  }
}
