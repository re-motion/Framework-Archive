using System;
using System.Xml;

namespace Rubicon.Security.UnitTests.XmlAsserter
{
  [CLSCompliant (false)]
  public class XmlDocumentEqualAsserter : XmlDocumentBaseAsserter
  {
    private XmlnsAttributeHandler _xmlnsAttributeFilter;

    public XmlDocumentEqualAsserter (XmlDocument expected, XmlDocument actual, string message, params object[] args)
        : base (expected, actual, message, args)
    {
      _xmlnsAttributeFilter = new XmlnsAttributeHandler ();
    }

    protected override bool CompareDocuments (XmlDocument expectedDocument, XmlDocument actualDocument)
    {
      return CompareNodes (expectedDocument, actualDocument);
    }

    protected bool CompareNodes (XmlNode expectedParentNode, XmlNode actualParentNode)
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

        if (!CompareNodes (expectedNode, actualNode))
          return false;
      }

      return true;
    }

    protected virtual bool AreNodesEqual (XmlNode expected, XmlNode actual)
    {
      return expected.NamespaceURI == actual.NamespaceURI
          && expected.LocalName == actual.LocalName
          && AreNodeAttributesEqual (expected.Attributes, actual.Attributes)
          && expected.Value == actual.Value;
    }

    protected virtual bool AreNodeAttributesEqual (XmlAttributeCollection expectedAttributes, XmlAttributeCollection actualAttributes)
    {
      if (expectedAttributes == null && actualAttributes == null)
        return true;

      if (expectedAttributes == null || actualAttributes == null)
        return false;

      _xmlnsAttributeFilter.Handle (expectedAttributes);
      _xmlnsAttributeFilter.Handle (actualAttributes);

      if (expectedAttributes.Count != actualAttributes.Count)
        return false;

      for (int i = 0; i < expectedAttributes.Count; i++)
      {
        if (!AreAttributesEqual (expectedAttributes[i], actualAttributes[i]))
          return false;
      }

      return true;
    }

    protected bool AreAttributesEqual (XmlAttribute expected, XmlAttribute actual)
    {
      return expected.NamespaceURI == actual.NamespaceURI
          && expected.LocalName == actual.LocalName
          && expected.Value == actual.Value;
    }

    protected void SetFailureMessage (XmlNode expectedNode, XmlNode actualNode)
    {
      ShowNodeStack (expectedNode, FailureMessage.AddExpectedLine);
      ShowNodeStack (actualNode, FailureMessage.AddActualLine);
    }
  }
}
