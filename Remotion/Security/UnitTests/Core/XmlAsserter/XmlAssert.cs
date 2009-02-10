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
using System.Xml;
using NUnit.Framework;

#pragma warning disable 612,618 // Asserters are obsolete

namespace Remotion.Security.UnitTests.Core.XmlAsserter
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

    static public void AreDocumentsSimilar (XmlDocument expectedDocument, XmlDocument actualDocument, string message, params object[] args)
    {
      Assert.DoAssert (new XmlDocumentSimilarAsserter (expectedDocument, actualDocument, message, args));
    }

    static public void AreDocumentsSimilar (XmlDocument expectedDocument, XmlDocument actualDocument, string message)
    {
      AreDocumentsSimilar (expectedDocument, actualDocument, message, null);
    }

    static public void AreDocumentsSimilar (XmlDocument expectedDocument, XmlDocument actualDocument)
    {
      AreDocumentsSimilar (expectedDocument, actualDocument, string.Empty, null);
    }

    static public void AreDocumentsSimilar (string expectedXml, XmlDocument actualDocument, string message, params object[] args)
    {
      XmlDocument expectedDocument = new XmlDocument ();
      expectedDocument.LoadXml (expectedXml);

      AreDocumentsSimilar (expectedDocument, actualDocument, message, args);
    }

    static public void AreDocumentsSimilar (string expectedXml, XmlDocument actualDocument, string message)
    {
      AreDocumentsSimilar (expectedXml, actualDocument, message, null);
    }

    static public void AreDocumentsSimilar (string expectedXml, XmlDocument actualDocument)
    {
      AreDocumentsSimilar (expectedXml, actualDocument, string.Empty, null);
    }
  }
}

#pragma warning restore 612,618

