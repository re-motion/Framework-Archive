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

namespace Rubicon.Web.UnitTests.UI.Controls
{

[TestFixture]
public class CommandTest
{
  private HttpContext _currentHttpContext;
  private Type _functionType;
  private string _functionTypeName;
  private Command _hrefCommand;
  private Command _eventCommand;
  private Command _wxeFunctionCommand;
  private Command _noneCommand;
  private string _toolTip;
  private string _href;
  private string _wxeFunctionParameters;
  private string _target;
  private string _postBackEvent;
  private string _onClick;
  private HtmlTextWriterSingleTagMock _writer;

  public CommandTest()
	{
	}

  [SetUp]
  public virtual void SetUp()
  {
    _currentHttpContext = HttpContextHelper.CreateHttpContext ("GET", "default.html", null);
    _currentHttpContext.Response.ContentEncoding = System.Text.Encoding.UTF8;
    HttpContextHelper.SetCurrent (_currentHttpContext);

    _functionType = typeof (ExecutionEngine.TestFunction);
    _functionTypeName = WebTypeUtility.GetQualifiedName (_functionType);
    _wxeFunctionParameters = "\"Value1\"";

    _toolTip = "This is a Tool Tip.";
    _href = "test.html?Param1={0}&Param2={1}";
    _target = "_blank";
    _postBackEvent = "__doPostBack (\"Target\", \"Args\");";
    _onClick = "return false;";

    _hrefCommand = new Command();
    _hrefCommand.Type = CommandType.Href;
    _hrefCommand.ToolTip = _toolTip;
    _hrefCommand.HrefCommand.Href = _href;
    _hrefCommand.HrefCommand.Target = _target;

    _eventCommand = new Command();
    _eventCommand.Type = CommandType.Event;
    _eventCommand.ToolTip = _toolTip;

    _wxeFunctionCommand = new Command();
    _wxeFunctionCommand.Type = CommandType.WxeFunction;
    _wxeFunctionCommand.ToolTip = _toolTip;
    _wxeFunctionCommand.WxeFunctionCommand.TypeName = _functionTypeName;
    _wxeFunctionCommand.WxeFunctionCommand.Parameters = _wxeFunctionParameters;
    _wxeFunctionCommand.WxeFunctionCommand.Target = _target;

    _noneCommand = new Command ();
    _noneCommand.Type = CommandType.None;

    _writer = new HtmlTextWriterSingleTagMock();
  }

  [TearDown]
  public virtual void TearDown()
  { 
    WebConfigurationMock.Current = null;
    Rubicon.Web.ExecutionEngine.UrlMapping.UrlMappingConfiguration.SetCurrent (null);
  }

  [Test]
  public void RenderHrefCommand()
  {
    string[] parameters = new string[] {"Value1", "Value2"};

    NameValueCollection additionalUrlParameters = new NameValueCollection();
    additionalUrlParameters.Add ("Parameter3", "Value3");

    string expectedHref = _hrefCommand.HrefCommand.FormatHref (parameters);
    expectedHref = UrlUtility.AddParameter (
        expectedHref, additionalUrlParameters.GetKey (0), additionalUrlParameters.Get (0));
    expectedHref = UrlUtility.GetAbsoluteUrl (_currentHttpContext, expectedHref);
    string expectedOnClick = _onClick;
    _hrefCommand.RenderBegin (_writer, _postBackEvent, parameters, _onClick, additionalUrlParameters, new Style());

    Assert.IsNotNull (_writer.Tag, "Missing Tag");
    Assert.AreEqual (HtmlTextWriterTag.A, _writer.Tag, "Wrong Tag");

    Assert.IsNotNull (_writer.Attributes[HtmlTextWriterAttribute.Href], "Missing Href");
    Assert.AreEqual (expectedHref, _writer.Attributes[HtmlTextWriterAttribute.Href], "Wrong Href");

    Assert.IsNotNull (_writer.Attributes[HtmlTextWriterAttribute.Onclick], "Missing OnClick");
    Assert.AreEqual (expectedOnClick, _writer.Attributes[HtmlTextWriterAttribute.Onclick], "Wrong OnClick");

    Assert.IsNotNull (_writer.Attributes[HtmlTextWriterAttribute.Title], "Missing Title");
    Assert.AreEqual (_toolTip, _writer.Attributes[HtmlTextWriterAttribute.Title], "Wrong Title");

    Assert.IsNotNull (_writer.Attributes[HtmlTextWriterAttribute.Target], "Missing Target");
    Assert.AreEqual (_target, _writer.Attributes[HtmlTextWriterAttribute.Target], "Wrong Target");
  }

  [Test]
  public void RenderEventCommand()
  {
    string expectedOnClick = _postBackEvent + _onClick;
    _eventCommand.RenderBegin (_writer, _postBackEvent, new string[0], _onClick);

    Assert.IsNotNull (_writer.Tag, "Missing Tag");
    Assert.AreEqual (HtmlTextWriterTag.A, _writer.Tag, "Wrong Tag");

    Assert.IsNotNull (_writer.Attributes[HtmlTextWriterAttribute.Href], "Missing Href");
    Assert.AreEqual ("#", _writer.Attributes[HtmlTextWriterAttribute.Href], "Wrong Href");

    Assert.IsNotNull (_writer.Attributes[HtmlTextWriterAttribute.Onclick], "Missing OnClick");
    Assert.AreEqual (expectedOnClick, _writer.Attributes[HtmlTextWriterAttribute.Onclick], "Wrong OnClick");

    Assert.IsNotNull (_writer.Attributes[HtmlTextWriterAttribute.Title], "Missing Title");
    Assert.AreEqual (_toolTip, _writer.Attributes[HtmlTextWriterAttribute.Title], "Wrong Title");

    Assert.IsNull (_writer.Attributes[HtmlTextWriterAttribute.Target], "Has Target");
  }

  [Test]
  public void RenderWxeFunctionCommand()
  {
    _wxeFunctionCommand.RenderBegin (_writer, _postBackEvent, new string[0], _onClick);

    string expectedOnClick = _postBackEvent + _onClick;

    Assert.IsNotNull (_writer.Tag, "Missing Tag");
    Assert.AreEqual (HtmlTextWriterTag.A, _writer.Tag, "Wrong Tag");

    Assert.IsNotNull (_writer.Attributes[HtmlTextWriterAttribute.Href], "Missing Href");
    Assert.AreEqual ("#", _writer.Attributes[HtmlTextWriterAttribute.Href], "Wrong Href");

    Assert.IsNotNull (_writer.Attributes[HtmlTextWriterAttribute.Onclick], "Missing OnClick");
    Assert.AreEqual (expectedOnClick, _writer.Attributes[HtmlTextWriterAttribute.Onclick], "Wrong OnClick");

    Assert.IsNotNull (_writer.Attributes[HtmlTextWriterAttribute.Title], "Missing Title");
    Assert.AreEqual (_toolTip, _writer.Attributes[HtmlTextWriterAttribute.Title], "Wrong Title");

    Assert.IsNull (_writer.Attributes[HtmlTextWriterAttribute.Target], "Has Target");
  }

  [Test]
  public void RenderNoneCommand()
  {
    _noneCommand.RenderBegin (_writer, _postBackEvent, new string[0], _onClick);

    Assert.IsNotNull (_writer.Tag, "Missing Tag");
    Assert.AreEqual (HtmlTextWriterTag.A, _writer.Tag, "Wrong Tag");
    Assert.AreEqual (0, _writer.Attributes.Count, "Has Attributes");
  }

}
}
