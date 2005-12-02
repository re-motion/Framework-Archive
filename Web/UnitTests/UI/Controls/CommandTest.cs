using System;
using System.Collections.Specialized;
using System.Web;
using NUnit.Framework;
using Rubicon.Web.ExecutionEngine;
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
  private string _invalidFunctionTypeName;

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
    _functionTypeName = _functionType.FullName + "," + _functionType.Assembly.GetName().Name;
    _invalidFunctionTypeName = "Rubicon.Web.UnitTests::ExecutionEngine.InvalidFunction";
  }

  [TearDown]
  public virtual void TearDown()
  { 
    WebConfigurationMock.Current = null;
  }

  [Test]
  public void FormatFunctionUrlWithDefaultWxeHandler()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetExecutionEngineWithDefaultWxeHandler();
    string parameter1 = "Hello World!";
    
    
    string wxeHandler = Rubicon.Web.Configuration.WebConfiguration.Current.ExecutionEngine.DefaultWxeHandler;
    string expectedUrl = UrlUtility.GetAbsoluteUrl (_currentHttpContext, wxeHandler);
    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add ("Parameter1", parameter1);
    expectedQueryString.Add (WxeHandler.Parameters.WxeFunctionType, _functionTypeName);
    expectedUrl += UrlUtility.FormatQueryString (expectedQueryString);

    Command command = new Command ();
    command.Type = CommandType.WxeFunction;
    command.WxeFunctionCommand.TypeName = _functionTypeName;
    command.WxeFunctionCommand.Parameters = "\"" + parameter1 + "\"";
    string url = command.FormatWxeFunctionUrl ();

    Assert.IsNotNull (url);
    Assert.AreEqual (expectedUrl, url);
  }
}
}
