using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using NUnit.Framework;

namespace Rubicon.Security.UnitTests.XmlAsserter
{
  [CLSCompliant (false)]
  public class XmlDocumentEqualAsserter : AbstractAsserter
  {
    private XmlDocument _expectedDocument;
    private XmlDocument _actualDocument;

    public XmlDocumentEqualAsserter (XmlDocument expected, XmlDocument actual, string message, params object[] args)
      : base (message, args)
    {
      _expectedDocument = expected;
      _actualDocument = actual;
    }

    public override bool Test ()
    {
      if (_actualDocument == null && _expectedDocument == null)
        return true;

      if (_actualDocument == null || _expectedDocument == null)
        return false;

      return AreChildNodesEqual (_expectedDocument, _actualDocument);
    }

    private bool AreChildNodesEqual (XmlNode expectedParentNode, XmlNode actualParentNode)
    {
      if (expectedParentNode.ChildNodes.Count != actualParentNode.ChildNodes.Count)
      {
        FailureMessage.AddLine ("Child node lengths differ:");
        FailureMessage.AddExpectedLine (expectedParentNode.ChildNodes.Count.ToString ());
        FailureMessage.AddActualLine (actualParentNode.ChildNodes.Count.ToString ());
        SetFailureMessage (expectedParentNode, actualParentNode);

        return false;
      }

      for (int i = 0; i < expectedParentNode.ChildNodes.Count; i++)
      {
        XmlNode expectedNode = expectedParentNode.ChildNodes[i];
        XmlNode actualNode = actualParentNode.ChildNodes[i];

        if (!AreNodesEqual (expectedNode, actualNode))
        {
          FailureMessage.AddLine ("Nodes differ:");
          SetFailureMessage (expectedNode, actualNode);
          return false;
        }

        if (!AreChildNodesEqual (expectedNode, actualNode))
          return false;
      }

      return true;
    }

    private bool AreNodesEqual (XmlNode expected, XmlNode actual)
    {
      return expected.NamespaceURI == actual.NamespaceURI 
          && expected.LocalName == actual.LocalName 
          && AreNodeAttributesEqual (expected.Attributes, actual.Attributes)
          && expected.Value == actual.Value;
    }

    private bool AreNodeAttributesEqual (XmlAttributeCollection expectedAttributes, XmlAttributeCollection actualAttributes)
    {
      if (expectedAttributes == null && actualAttributes == null)
        return true;

      if (expectedAttributes == null || actualAttributes == null)
        return false;

      FilterNamespaceDeclarations (expectedAttributes);
      FilterNamespaceDeclarations (actualAttributes);

      if (expectedAttributes.Count != actualAttributes.Count)
        return false;

      for (int i = 0; i < expectedAttributes.Count; i++)
      {
        if (!AreAttributesEqual (expectedAttributes[i], actualAttributes[i]))
          return false;
      }

      return true;
    }

    private bool AreAttributesEqual (XmlAttribute expected, XmlAttribute actual)
    {
      return expected.NamespaceURI == actual.NamespaceURI
          && expected.LocalName == actual.LocalName
          && expected.Value == actual.Value;
    }

    private bool IsNamespaceDeclaration (XmlAttribute attribute)
    {
      return attribute.LocalName == "xmlns" && attribute.NamespaceURI == "http://www.w3.org/2000/xmlns/";
    }

    private void FilterNamespaceDeclarations (XmlAttributeCollection attributes)
    {
      for (int i = attributes.Count - 1; i >= 0; i--)
      {
        if (IsNamespaceDeclaration (attributes[i]))
          attributes.Remove (attributes[i]);
      }
    }

    private void SetFailureMessage (XmlNode expectedNode, XmlNode actualNode)
    {
      Stack<XmlNode> expectedNodeStack = GetNodeStack (expectedNode);
      Stack<XmlNode> actualNodeStack = GetNodeStack (actualNode);

      while (expectedNodeStack.Count > 0)
        FailureMessage.AddExpectedLine (GetNodeInfo (expectedNodeStack.Pop ()));

      while (actualNodeStack.Count > 0)
        FailureMessage.AddActualLine (GetNodeInfo (actualNodeStack.Pop ()));
    }

    private Stack<XmlNode> GetNodeStack (XmlNode node)
    {
      Stack<XmlNode> nodeStack = new Stack<XmlNode> ();

      XmlNode currentNode = node;
      while (currentNode != null && !(currentNode is XmlDocument))
      {
        nodeStack.Push (currentNode);
        currentNode = currentNode.ParentNode;
      }

      return nodeStack;
    }

    private string GetNodeInfo (XmlNode node)
    {
      return node.NamespaceURI + ":" + node.LocalName + GetAttributeInfo (node.Attributes) + GetNodeValueInfo (node.Value);
    }

    private string GetAttributeInfo (XmlAttributeCollection attributes)
    {
      if (attributes == null || attributes.Count == 0)
        return string.Empty;

      StringBuilder attributeInfoBuilder = new StringBuilder ();

      foreach (XmlAttribute attribute in attributes)
      {
        if (attributeInfoBuilder.Length > 0)
          attributeInfoBuilder.Append (", ");

        attributeInfoBuilder.Append (attribute.NamespaceURI + ":" + attribute.Name + "=\"" + attribute.Value + "\"");
      }

      return "[" + attributeInfoBuilder.ToString () + "]";
    }

    private string GetNodeValueInfo (string nodeValue)
    {
      if (nodeValue == null)
        return string.Empty;

      return " = \"" + nodeValue + "\"";
    }
  }
}
