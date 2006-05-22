using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rubicon.Security.UnitTests.XmlAsserter
{
  public class NodeStackToXPathConverter
  {
    private XmlnsAttributeHandler _xmlnsAttributeHandler;
    private bool _includeNamespaces = false;
    private XmlNamespaceManager _namespaceManager;

    public NodeStackToXPathConverter()
    {
      _xmlnsAttributeHandler = new XmlnsAttributeHandler ();
      _xmlnsAttributeHandler.XmlnsAttributeFound += new XmlnsAttributeEventHandler (XmlnsAttributeHandler_XmlnsAttributeFound);
      _namespaceManager = new XmlNamespaceManager (new NameTable ());
    }

    public bool IncludeNamespaces
    {
      get { return _includeNamespaces; }
      set { _includeNamespaces = value; }
    }

    public bool IgnoreXmlnsAttribute
    {
      get { return _xmlnsAttributeHandler.Filter; }
      set { _xmlnsAttributeHandler.Filter = value; }
    }

    public XmlNamespaceManager NamespaceManager
    {
      get { return _namespaceManager; }
    }

    public string GetXPathExpression (Stack<XmlNode> nodeStack)
    {
      StringBuilder xPathBuilder = new StringBuilder ();

      while (nodeStack.Count > 0)
      {
        XmlNode currentNode = nodeStack.Pop ();
        if (currentNode.NodeType == XmlNodeType.Element)
        {
          _xmlnsAttributeHandler.Handle (currentNode.Attributes);
          AppendNode (xPathBuilder, currentNode);
          if (currentNode.NodeType == XmlNodeType.Element)
            AppendAttributes (xPathBuilder, currentNode.Attributes);
        }
      }

      return xPathBuilder.ToString ();
    }

    private void AppendAttributes (StringBuilder xPathBuilder, XmlAttributeCollection attributes)
    {
      if (attributes.Count == 0)
        return;

      xPathBuilder.Append ("[");
      bool isFirstAttribute = true;
      
      foreach (XmlAttribute attribute in attributes)
      {
        if (!isFirstAttribute)
          xPathBuilder.Append (" and ");

        xPathBuilder.Append (GetAttributeExpression (attribute));
        isFirstAttribute = false;
      }

      xPathBuilder.Append ("]");
    }

    private void AppendNode (StringBuilder xPathBuilder, XmlNode currentNode)
    {
      xPathBuilder.Append ("/");

      if (IncludeNamespaces)
      {
        string prefix = _namespaceManager.LookupPrefix (currentNode.NamespaceURI);
        if (string.IsNullOrEmpty (prefix))
          prefix = "default";

        xPathBuilder.Append (prefix + ":");
      }

      xPathBuilder.Append (currentNode.LocalName);
    }

    private string GetAttributeExpression (XmlAttribute attribute)
    {
      return "@" + attribute.LocalName + "=\"" + attribute.Value + "\"";
    }

    private void XmlnsAttributeHandler_XmlnsAttributeFound (object sender, XmlnsAttributeEventArgs args)
    {
      if (args.IsDefaultNamespace)
        _namespaceManager.AddNamespace ("default", args.NamespaceUri);
      else
        _namespaceManager.AddNamespace (args.Prefix, args.NamespaceUri);
    }
  }
}
