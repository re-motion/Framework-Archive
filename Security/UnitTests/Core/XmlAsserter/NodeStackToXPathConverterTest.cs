// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using System.Xml;
using NUnit.Framework;

namespace Remotion.Security.UnitTests.Core.XmlAsserter
{
  [TestFixture]
  public class NodeStackToXPathConverterTest
  {
    private XmlDocument _document;
    private NodeStackToXPathConverter _converter;

    [SetUp]
    public void SetUp ()
    {
      string xml = @"
          <t:securityMetadata xmlns=""http://www.re-motion.org/Data/Mapping/1.0"" xmlns:t=""http://www.re-motion.org/Security/Metadata/1.0"">
            <t:classes>
              <t:class id=""Class1"" />
              <t:class id=""7823-qwer-124"" name=""Class2"" />
            </t:classes>

            <classes>
              <class id=""Class1"">
                <entity>class1</entity>
              </class>
            </classes>
          </t:securityMetadata>";

      _document = new XmlDocument ();
      _document.LoadXml (xml);

      _converter = new NodeStackToXPathConverter ();
      _converter.IncludeNamespaces = false;
    }

    [Test]
    public void RootNodeExpression ()
    {
      Stack<XmlNode> nodeStack = new Stack<XmlNode> ();
      nodeStack.Push (_document.ChildNodes[0]);

      Assert.AreEqual ("/securityMetadata", _converter.GetXPathExpression (nodeStack));
    }

    [Test]
    public void ChildNodeExpression ()
    {
      Stack<XmlNode> nodeStack = new Stack<XmlNode> ();
      nodeStack.Push (_document.ChildNodes[0].ChildNodes[0]);
      nodeStack.Push (_document.ChildNodes[0]);

      Assert.AreEqual ("/securityMetadata/classes", _converter.GetXPathExpression (nodeStack));
    }

    [Test]
    public void ChildNodeWithAttributeExpression ()
    {
      Stack<XmlNode> nodeStack = new Stack<XmlNode> ();
      nodeStack.Push (_document.ChildNodes[0].ChildNodes[0].ChildNodes[0]);
      nodeStack.Push (_document.ChildNodes[0].ChildNodes[0]);
      nodeStack.Push (_document.ChildNodes[0]);

      Assert.AreEqual ("/securityMetadata/classes/class[@id=\"Class1\"]", _converter.GetXPathExpression (nodeStack));
    }

    [Test]
    public void ChildNodeWithMultipleAttributesExpression ()
    {
      Stack<XmlNode> nodeStack = new Stack<XmlNode> ();
      nodeStack.Push (_document.ChildNodes[0].ChildNodes[0].ChildNodes[1]);
      nodeStack.Push (_document.ChildNodes[0].ChildNodes[0]);
      nodeStack.Push (_document.ChildNodes[0]);

      Assert.AreEqual ("/securityMetadata/classes/class[@id=\"7823-qwer-124\" and @name=\"Class2\"]", _converter.GetXPathExpression (nodeStack));
    }

    [Test]
    public void ChildNodeWithNamespaceExpression ()
    {
      _converter.IncludeNamespaces = true;

      Stack<XmlNode> nodeStack = new Stack<XmlNode> ();
      nodeStack.Push (_document.ChildNodes[0].ChildNodes[0]);
      nodeStack.Push (_document.ChildNodes[0]);

      Assert.AreEqual ("/t:securityMetadata/t:classes", _converter.GetXPathExpression (nodeStack));
    }

    [Test]
    public void ChildNodeWithDefaultNamespaceExpression ()
    {
      _converter.IncludeNamespaces = true;

      Stack<XmlNode> nodeStack = new Stack<XmlNode> ();
      nodeStack.Push (_document.ChildNodes[0].ChildNodes[1].ChildNodes[0].ChildNodes[0].ChildNodes[0]);
      nodeStack.Push (_document.ChildNodes[0].ChildNodes[1].ChildNodes[0].ChildNodes[0]);
      nodeStack.Push (_document.ChildNodes[0].ChildNodes[1].ChildNodes[0]);
      nodeStack.Push (_document.ChildNodes[0].ChildNodes[1]);
      nodeStack.Push (_document.ChildNodes[0]);

      Assert.AreEqual ("/t:securityMetadata/default:classes/default:class[@id=\"Class1\"]/default:entity", _converter.GetXPathExpression (nodeStack));
    }
  }
}
