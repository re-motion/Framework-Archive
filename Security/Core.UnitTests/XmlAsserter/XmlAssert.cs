using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using NUnit.Framework;

namespace Rubicon.Security.UnitTests.XmlAsserter
{
  public static class XmlAssert
  {
    static public void IsElement (string expectedNamespace, string expectedLocalName, XmlNode actualNode, string message, params object[] args)
    {
      Assert.DoAssert (new XmlElementAsserter (expectedNamespace, expectedLocalName, actualNode, message, args));
    }

    static public void IsElement (string expectedNamespace, string expectedLocalName, XmlNode actualNode, string message)
    {
      IsElement (expectedNamespace, expectedLocalName, actualNode, message, null);
    }

    static public void IsElement (string expectedNamespace, string expectedLocalName, XmlNode actualNode)
    {
      IsElement (expectedNamespace, expectedLocalName, actualNode, string.Empty, null);
    }

    static public void AreDocumentsEqual (XmlDocument expectedDocument, XmlDocument actualDocument, string message, params object[] args)
    {
      Assert.DoAssert (new XmlDocumentEqualAsserter (expectedDocument, actualDocument, message, args));
    }

    static public void AreDocumentsEqual (XmlDocument expectedDocument, XmlDocument actualDocument, string message)
    {
      AreDocumentsEqual (expectedDocument, actualDocument, message, null);
    }

    static public void AreDocumentsEqual (XmlDocument expectedDocument, XmlDocument actualDocument)
    {
      AreDocumentsEqual (expectedDocument, actualDocument, string.Empty, null);
    }

    static public void AreDocumentsEqual (string expectedXml, XmlDocument actualDocument, string message, params object[] args)
    {
      XmlDocument expectedDocument = new XmlDocument ();
      expectedDocument.LoadXml (expectedXml);

      AreDocumentsEqual (expectedDocument, actualDocument, message, args);
    }

    static public void AreDocumentsEqual (string expectedXml, XmlDocument actualDocument, string message)
    {
      AreDocumentsEqual (expectedXml, actualDocument, message, null);
    }

    static public void AreDocumentsEqual (string expectedXml, XmlDocument actualDocument)
    {
      AreDocumentsEqual (expectedXml, actualDocument, string.Empty, null);
    }
  }
}
