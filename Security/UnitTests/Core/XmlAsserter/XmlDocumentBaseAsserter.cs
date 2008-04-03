using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using NUnit.Framework;

namespace Remotion.Security.UnitTests.Core.XmlAsserter
{
  [CLSCompliant (false)]
  public abstract class XmlDocumentBaseAsserter : AbstractAsserter
  {
    protected delegate void MessageListenerDelegate (string messageInfo);

    private XmlDocument _expectedDocument;
    private XmlDocument _actualDocument;

    public XmlDocumentBaseAsserter (XmlDocument expected, XmlDocument actual, string message, params object[] args)
      : base (message, args)
    {
      _expectedDocument = expected;
      _actualDocument = actual;
    }

    public XmlDocument ExpectedDocument
    {
      get { return _expectedDocument; }
    }

    public XmlDocument ActualDocument
    {
      get { return _actualDocument; }
    }

    public override bool Test ()
    {
      if (_actualDocument == null && _expectedDocument == null)
        return true;

      if (_actualDocument == null || _expectedDocument == null)
        return false;

      return CompareDocuments (_expectedDocument, _actualDocument);
    }

    protected abstract bool CompareDocuments (XmlDocument expectedDocument, XmlDocument actualDocument);

    protected void ShowNodeStack (XmlNode node, MessageListenerDelegate messageListener)
    {
      Stack<XmlNode> nodeStack = GetNodeStack (node);

      while (nodeStack.Count > 0)
        messageListener (GetNodeInfo (nodeStack.Pop ()));
    }

    protected Stack<XmlNode> GetNodeStack (XmlNode node)
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
