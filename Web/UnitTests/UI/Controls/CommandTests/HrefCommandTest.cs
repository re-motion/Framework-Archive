using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using NUnit.Framework;

using Rubicon.Security;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UnitTests.AspNetFramework;
using Rubicon.Web.UnitTests.Configuration;
using Rubicon.Web.Utilities;

namespace Rubicon.Web.UnitTests.UI.Controls.CommandTests
{
  [TestFixture]
  public class HrefCommandTest : CommandTest
  {
    private CommandTestHelper _testHelper;

    [SetUp]
    public virtual void SetUp ()
    {
      _testHelper = new CommandTestHelper ();
      HttpContextHelper.SetCurrent (_testHelper.HttpContext);
    }

    [Test]
    public void HasAccess_WithoutSeucrityProvider ()
    {
      Command command = _testHelper.CreateHrefCommand ();
      _testHelper.ReplayAll ();

      bool hasAccess = command.HasAccess ();

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void Render_WithIsActiveTrue ()
    {
      Command command = _testHelper.CreateHrefCommandAsPartialMock ();
      string[] parameters = new string[] { "Value1", "Value2" };

      NameValueCollection additionalUrlParameters = new NameValueCollection ();
      additionalUrlParameters.Add ("Parameter3", "Value3");

      string expectedHref = command.HrefCommand.FormatHref (parameters);
      expectedHref = UrlUtility.AddParameter (expectedHref, additionalUrlParameters.GetKey (0), additionalUrlParameters.Get (0));
      expectedHref = UrlUtility.GetAbsoluteUrl (_testHelper.HttpContext, expectedHref);
      string expectedOnClick = _testHelper.OnClick;

      _testHelper.ExpectOnceOnHasAccess (command, true);
      _testHelper.ReplayAll ();

      command.RenderBegin (_testHelper.HtmlWriter, _testHelper.PostBackEvent, parameters, _testHelper.OnClick, additionalUrlParameters, true, new Style ());

      _testHelper.VerifyAll ();

      Assert.IsNotNull (_testHelper.HtmlWriter.Tag, "Missing Tag");
      Assert.AreEqual (HtmlTextWriterTag.A, _testHelper.HtmlWriter.Tag, "Wrong Tag");

      Assert.IsNotNull (_testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Href], "Missing Href");
      Assert.AreEqual (expectedHref, _testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Href], "Wrong Href");

      Assert.IsNotNull (_testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Onclick], "Missing OnClick");
      Assert.AreEqual (expectedOnClick, _testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Onclick], "Wrong OnClick");

      Assert.IsNotNull (_testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Title], "Missing Title");
      Assert.AreEqual (_testHelper.ToolTip, _testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Title], "Wrong Title");

      Assert.IsNotNull (_testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Target], "Missing Target");
      Assert.AreEqual (_testHelper.Target, _testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Target], "Wrong Target");
    }

    [Test]
    public void Render_WithIsActiveFalse ()
    {
      Command command = _testHelper.CreateHrefCommandAsPartialMock ();
      string[] parameters = new string[] { "Value1", "Value2" };
      NameValueCollection additionalUrlParameters = new NameValueCollection ();
      additionalUrlParameters.Add ("Parameter3", "Value3");
      _testHelper.ExpectOnceOnHasAccess (command, false);
      _testHelper.ReplayAll ();

      command.RenderBegin (_testHelper.HtmlWriter, _testHelper.PostBackEvent, parameters, _testHelper.OnClick, additionalUrlParameters, true, new Style ());

      _testHelper.VerifyAll ();
      Assert.IsNotNull (_testHelper.HtmlWriter.Tag, "Missing Tag");
      Assert.AreEqual (HtmlTextWriterTag.A, _testHelper.HtmlWriter.Tag, "Wrong Tag");
      Assert.AreEqual (0, _testHelper.HtmlWriter.Attributes.Count, "Has Attributes");
    }
  }
}