using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NUnit.Framework;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.ExecutionEngine.UrlMapping;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UnitTests.AspNetFramework;
using Rubicon.Web.UnitTests.Configuration;
using Rubicon.Web.Utilities;

namespace Rubicon.Web.UnitTests.UI.Controls.CommandTests
{

  [TestFixture]
  public class CommandTest
  {
    private CommandTestHelper _testHelper;
    private HttpContext _currentHttpContext;
    private HtmlTextWriterSingleTagMock _writer;

    public CommandTest ()
    {
    }

    [SetUp]
    public virtual void SetUp ()
    {
      _currentHttpContext = HttpContextHelper.CreateHttpContext ("GET", "default.html", null);
      _currentHttpContext.Response.ContentEncoding = System.Text.Encoding.UTF8;
      HttpContextHelper.SetCurrent (_currentHttpContext);

      _writer = new HtmlTextWriterSingleTagMock ();

      _testHelper = new CommandTestHelper ();
    }

    [TearDown]
    public virtual void TearDown ()
    {
      WebConfigurationMock.Current = null;
      Rubicon.Web.ExecutionEngine.UrlMapping.UrlMappingConfiguration.SetCurrent (null);
    }

    [Test]
    public void RenderHrefCommand ()
    {
      Command command = _testHelper.CreateHrefCommand ();

      string[] parameters = new string[] { "Value1", "Value2" };

      NameValueCollection additionalUrlParameters = new NameValueCollection ();
      additionalUrlParameters.Add ("Parameter3", "Value3");

      string expectedHref = command.HrefCommand.FormatHref (parameters);
      expectedHref = UrlUtility.AddParameter (expectedHref, additionalUrlParameters.GetKey (0), additionalUrlParameters.Get (0));
      expectedHref = UrlUtility.GetAbsoluteUrl (_currentHttpContext, expectedHref);
      string expectedOnClick = _testHelper.OnClick;

      command.RenderBegin (_writer, _testHelper.PostBackEvent, parameters, _testHelper.OnClick, additionalUrlParameters, true, new Style ());

      Assert.IsTrue (command.IsActive (), "Not active");

      Assert.IsNotNull (_writer.Tag, "Missing Tag");
      Assert.AreEqual (HtmlTextWriterTag.A, _writer.Tag, "Wrong Tag");

      Assert.IsNotNull (_writer.Attributes[HtmlTextWriterAttribute.Href], "Missing Href");
      Assert.AreEqual (expectedHref, _writer.Attributes[HtmlTextWriterAttribute.Href], "Wrong Href");

      Assert.IsNotNull (_writer.Attributes[HtmlTextWriterAttribute.Onclick], "Missing OnClick");
      Assert.AreEqual (expectedOnClick, _writer.Attributes[HtmlTextWriterAttribute.Onclick], "Wrong OnClick");

      Assert.IsNotNull (_writer.Attributes[HtmlTextWriterAttribute.Title], "Missing Title");
      Assert.AreEqual (_testHelper.ToolTip, _writer.Attributes[HtmlTextWriterAttribute.Title], "Wrong Title");

      Assert.IsNotNull (_writer.Attributes[HtmlTextWriterAttribute.Target], "Missing Target");
      Assert.AreEqual (_testHelper.Target, _writer.Attributes[HtmlTextWriterAttribute.Target], "Wrong Target");
    }

    [Test]
    public void RenderEventCommand ()
    {
      string expectedOnClick = _testHelper.PostBackEvent + _testHelper.OnClick;
      Command command = _testHelper.CreateEventCommand ();
      command.RenderBegin (_writer, _testHelper.PostBackEvent, new string[0], _testHelper.OnClick);

      Assert.IsTrue (command.IsActive (), "Not active");

      Assert.IsNotNull (_writer.Tag, "Missing Tag");
      Assert.AreEqual (HtmlTextWriterTag.A, _writer.Tag, "Wrong Tag");

      Assert.IsNotNull (_writer.Attributes[HtmlTextWriterAttribute.Href], "Missing Href");
      Assert.AreEqual ("#", _writer.Attributes[HtmlTextWriterAttribute.Href], "Wrong Href");

      Assert.IsNotNull (_writer.Attributes[HtmlTextWriterAttribute.Onclick], "Missing OnClick");
      Assert.AreEqual (expectedOnClick, _writer.Attributes[HtmlTextWriterAttribute.Onclick], "Wrong OnClick");

      Assert.IsNotNull (_writer.Attributes[HtmlTextWriterAttribute.Title], "Missing Title");
      Assert.AreEqual (_testHelper.ToolTip, _writer.Attributes[HtmlTextWriterAttribute.Title], "Wrong Title");

      Assert.IsNull (_writer.Attributes[HtmlTextWriterAttribute.Target], "Has Target");
    }

    [Test]
    public void RenderWxeFunctionCommand ()
    {
      Command command = _testHelper.CreateWxeFunctionCommand ();
      command.RenderBegin (_writer, _testHelper.PostBackEvent, new string[0], _testHelper.OnClick);

      string expectedOnClick = _testHelper.PostBackEvent + _testHelper.OnClick;

      Assert.IsTrue (command.IsActive (), "Not active");

      Assert.IsNotNull (_writer.Tag, "Missing Tag");
      Assert.AreEqual (HtmlTextWriterTag.A, _writer.Tag, "Wrong Tag");

      Assert.IsNotNull (_writer.Attributes[HtmlTextWriterAttribute.Href], "Missing Href");
      Assert.AreEqual ("#", _writer.Attributes[HtmlTextWriterAttribute.Href], "Wrong Href");

      Assert.IsNotNull (_writer.Attributes[HtmlTextWriterAttribute.Onclick], "Missing OnClick");
      Assert.AreEqual (expectedOnClick, _writer.Attributes[HtmlTextWriterAttribute.Onclick], "Wrong OnClick");

      Assert.IsNotNull (_writer.Attributes[HtmlTextWriterAttribute.Title], "Missing Title");
      Assert.AreEqual (_testHelper.ToolTip, _writer.Attributes[HtmlTextWriterAttribute.Title], "Wrong Title");

      Assert.IsNull (_writer.Attributes[HtmlTextWriterAttribute.Target], "Has Target");
    }

    [Test]
    public void RenderNoneCommand ()
    {
      Command command = _testHelper.CreateNoneCommand ();
      command.RenderBegin (_writer, _testHelper.PostBackEvent, new string[0], _testHelper.OnClick);

      Assert.IsTrue (command.IsActive (), "Not active");

      Assert.IsNotNull (_writer.Tag, "Missing Tag");
      Assert.AreEqual (HtmlTextWriterTag.A, _writer.Tag, "Wrong Tag");
      Assert.AreEqual (0, _writer.Attributes.Count, "Has Attributes");
    }
  }
}
