/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.Diagnostics.ToText;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  // TODO AE: Dedicated tests for methods Attribute, CreateXmlWriter, Close missing.
  [TestFixture]
  public class HtmlTagWriterTest
  {
    [Test]
    public void WriteTagTest ()
    {
      var stringWriter = new StringWriter();
      using (var htmlWriter = new HtmlTagWriter (stringWriter, false))
      {
        htmlWriter.Tag ("div").Value ("xxx").TagEnd ("div");
      }
      var result = stringWriter.ToString ();
      Assert.That (result, Is.EqualTo ("<div>xxx</div>"));
    }

    [Test]
    public void WritePageHeaderTest ()
    {
      var stringWriter = new StringWriter ();
      using (var htmlWriter = new HtmlTagWriter (stringWriter, false))
      {
        htmlWriter.WritePageHeader("Page Header Test","pageHeaderTest.css");
      }
      var result = stringWriter.ToString ();
      Assert.That (result, Is.EqualTo ("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" \"\"><html><head><title>Page Header Test</title><style>@import \"pageHeaderTest.css\";</style><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" /></head></html>"));
    }


    [Test]
    public void BreakTagTest ()
    {
      var stringWriter = new StringWriter ();
      using (var htmlWriter = new HtmlTagWriter (stringWriter, false))
      {
        htmlWriter.Tags.br();
      }
      var result = stringWriter.ToString ();
      Assert.That (result, Is.EqualTo ("<br />"));
    }


    [Test]
    public void SpecificTagsTest ()
    {
      string[] tagNames = new[] { "html", "head", "title", "style", "body", "table", "th", "tr", "td", "p"  };
      foreach (string tagName in tagNames)
      { 
        AssertTagNameOpenCloseHtml (tagName);
      }
    }



    [Test]
    [ExpectedException (typeof (XmlException), ExpectedMessage = "Wrong closing tag in HTML: Expected abc but was xyz.")]
    public void NonMatchingEndTagTest ()
    {
      using (var htmlWriter = new HtmlTagWriter (TextWriter.Null, false))
      {
        htmlWriter.Tag("abc");
        htmlWriter.TagEnd ("xyz");
      }
    }

    [Test]
    [ExpectedException (typeof (XmlException), ExpectedMessage = "Wrong closing tag in HTML: Expected abc but was xyz.")]
    public void ComplexNonMatchingEndTagTest ()
    {
      using (var htmlWriter = new HtmlTagWriter (TextWriter.Null, false))
      {
        htmlWriter.Tag ("abc");
        WriteHtmlPage (htmlWriter);
        htmlWriter.TagEnd ("xyz");
      }
    }


    [Test]
    public void HtmlPageIntegrationTest ()
    {
      var stringWriter = new StringWriter ();
      using (var htmlWriter = new HtmlTagWriter (stringWriter, false))
      {
        WriteHtmlPage(htmlWriter);
      }
      var result = stringWriter.ToString ();
      Assert.That (result, Is.EqualTo ("<html><head><title>Title: My HTML Page</title></head><body><p id=\"first_paragraph\">Smells like...<br />Victory<table class=\"myTable\"><tr><th>1st column</th></tr><tr><td>some data</td></tr><tr><td>some more data</td></tr></table></p></body></html>"));
    }

    private static void WriteHtmlPage (HtmlTagWriter htmlWriter)
    {
      htmlWriter.Tags.html ();
      htmlWriter.Tags.head ();
      htmlWriter.Tags.title ();
      htmlWriter.Value ("Title: My HTML Page");
      htmlWriter.Tags.titleEnd ();
      htmlWriter.Tags.headEnd ();
      htmlWriter.Tags.body ();
      htmlWriter.Tags.p ();
      htmlWriter.Attribute ("id", "first_paragraph");
      htmlWriter.Value ("Smells like...");
      htmlWriter.Tags.br ();
      htmlWriter.Value ("Victory");
      htmlWriter.Tags.table ().Attribute ("class", "myTable");
      htmlWriter.Tags.tr().Tags.th().Value("1st column").Tags.thEnd().Tags.trEnd();
      htmlWriter.Tags.tr ().Tags.td ().Value ("some data").Tags.tdEnd ().Tags.trEnd ();
      htmlWriter.Tags.tr ().Tags.td ().Value ("some more data").Tags.tdEnd ().Tags.trEnd ();
      htmlWriter.Tags.tableEnd ();
      htmlWriter.Tags.pEnd ();
      htmlWriter.Tags.bodyEnd ();
      htmlWriter.Tags.htmlEnd ();
    }


    private static void AssertTagNameOpenCloseHtml (string tagName)
    {
      var tagNameHtmlResult = GetSpecificTagOpenCloseHtml (tagName);
      //To.ConsoleLine.sb().e(() => tagName).e (() => tagNameHtmlResult).se();
      Assert.That (tagNameHtmlResult, Is.EqualTo ("<" + tagName + ">abc</" + tagName + ">"));
    }

    private static string GetSpecificTagOpenCloseHtml (string tagName)
    {
      var stringWriter = new StringWriter ();
      using (var htmlWriter = new HtmlTagWriter (stringWriter, false))
      {
        HtmlTagWriterTags htmlTagWriterTags = htmlWriter.Tags;
        PrivateInvoke.InvokePublicMethod (htmlTagWriterTags, tagName);
        htmlWriter.Value ("abc");
        PrivateInvoke.InvokePublicMethod (htmlTagWriterTags, tagName + "End");
      }
      return stringWriter.ToString ();
    }

  }
}