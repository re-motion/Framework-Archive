using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NUnit.Framework;
using Rubicon.NullableValueTypes;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.ExecutionEngine.UrlMapping;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UnitTests.AspNetFramework;
using Rubicon.Web.UnitTests.Configuration;
using Rubicon.Web.Utilities;

namespace Rubicon.Web.UnitTests.UI.Controls
{

[TestFixture]
public class NavigationCommandTest
{
  private HttpContext _currentHttpContext;
  private Type _functionType;
  private string _functionTypeName;
  private NavigationCommand _hrefCommand;
  private NavigationCommand _eventCommand;
  private NavigationCommand _wxeFunctionCommand;
  private NavigationCommand _noneCommand;
  private string _toolTip;
  private string _href;
  private string _wxeFunctionParameter1Value;
  private string _wxeFunctionParameters;
  private string _target;
  private string _postBackEvent;
  private string _onClick;
  private HtmlTextWriterSingleTagMock _writer;

  [SetUp]
  public virtual void SetUp()
  {
    _currentHttpContext = HttpContextHelper.CreateHttpContext ("GET", "default.html", null);
    _currentHttpContext.Response.ContentEncoding = System.Text.Encoding.UTF8;
    HttpContextHelper.SetCurrent (_currentHttpContext);

    _functionType = typeof (ExecutionEngine.TestFunction);
    _functionTypeName = WebTypeUtility.GetQualifiedName (_functionType);
    _wxeFunctionParameter1Value = "Value1";
    _wxeFunctionParameters = "\"Value1\"";

    _toolTip = "This is a Tool Tip.";
    _href = "test.html?Param1={0}&Param2={1}";
    _target = "_blank";
    _postBackEvent = "__doPostBack (\"Target\", \"Args\");";
    _onClick = "return false;";

    _hrefCommand = new NavigationCommand();
    _hrefCommand.Type = CommandType.Href;
    _hrefCommand.ToolTip = _toolTip;
    _hrefCommand.HrefCommand.Href = _href;
    _hrefCommand.HrefCommand.Target = _target;

    _eventCommand = new NavigationCommand();
    _eventCommand.Type = CommandType.Event;
    _eventCommand.ToolTip = _toolTip;

    _wxeFunctionCommand = new NavigationCommand();
    _wxeFunctionCommand.Type = CommandType.WxeFunction;
    _wxeFunctionCommand.ToolTip = _toolTip;
    _wxeFunctionCommand.WxeFunctionCommand.TypeName = _functionTypeName;
    _wxeFunctionCommand.WxeFunctionCommand.Parameters = _wxeFunctionParameters;
    _wxeFunctionCommand.WxeFunctionCommand.Target = _target;

    _noneCommand = new NavigationCommand ();
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
  public void RenderWxeFunctionCommand()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetExecutionEngineWithDefaultWxeHandler();    
    
    NameValueCollection additionalUrlParameters = new NameValueCollection();
    additionalUrlParameters.Add ("Parameter2", "Value2");

    string expectedHref = _wxeFunctionCommand.GetWxeFunctionPermanentUrl (additionalUrlParameters);
    string expectedOnClick = _onClick;

    _wxeFunctionCommand.RenderBegin (
          _writer, _postBackEvent, new string[0], _onClick, additionalUrlParameters, false, new Style());

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
  public void GetWxeFunctionPermanentUrlWithDefaultWxeHandler()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetExecutionEngineWithDefaultWxeHandler();    
    
    string wxeHandler = Rubicon.Web.Configuration.WebConfiguration.Current.ExecutionEngine.DefaultWxeHandler;
    
    string expectedUrl = UrlUtility.GetAbsoluteUrl (_currentHttpContext, wxeHandler);
    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add ("Parameter1", _wxeFunctionParameter1Value);
    expectedQueryString.Add (WxeHandler.Parameters.WxeReturnToSelf, NaBoolean.True.ToString());
    expectedQueryString.Add (WxeHandler.Parameters.WxeFunctionType, _functionTypeName);
    expectedUrl += UrlUtility.FormatQueryString (expectedQueryString);

    NavigationCommand command = new NavigationCommand ();
    command.Type = CommandType.WxeFunction;
    command.WxeFunctionCommand.TypeName = _functionTypeName;
    command.WxeFunctionCommand.Parameters = _wxeFunctionParameters;
    string url = command.GetWxeFunctionPermanentUrl ();

    Assert.IsNotNull (url);
    Assert.AreEqual (expectedUrl, url);
  }

  [Test]
  public void GetWxeFunctionPermanentUrlWithMappedFunctionTypeByTypeName()
  {
    string resource = "~/Test.wxe";
    UrlMappingConfiguration.Current.Mappings.Add (new UrlMappingEntry (_functionType, resource));
    string parameter1 = "Value1";
    
    string expectedUrl = UrlUtility.GetAbsoluteUrl (_currentHttpContext, resource);
    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add ("Parameter1", parameter1);
    expectedQueryString.Add (WxeHandler.Parameters.WxeReturnToSelf, NaBoolean.True.ToString());
    expectedUrl += UrlUtility.FormatQueryString (expectedQueryString);

    NavigationCommand command = new NavigationCommand ();
    command.Type = CommandType.WxeFunction;
    command.WxeFunctionCommand.TypeName = _functionTypeName;
    command.WxeFunctionCommand.Parameters = "\"" + parameter1 + "\"";
    string url = command.GetWxeFunctionPermanentUrl ();

    Assert.IsNotNull (url);
    Assert.AreEqual (expectedUrl, url);
  }

  [Test]
  public void GetWxeFunctionPermanentUrlWithMappedFunctionTypeByMappingID()
  {
    string mappingID = "Test";
    string resource = "~/Test.wxe";
    UrlMappingConfiguration.Current.Mappings.Add (new UrlMappingEntry (mappingID, _functionType, resource));
    string parameter1 = "Value1";
    
    string expectedUrl = UrlUtility.GetAbsoluteUrl (_currentHttpContext, resource);
    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add ("Parameter1", parameter1);
    expectedQueryString.Add (WxeHandler.Parameters.WxeReturnToSelf, NaBoolean.True.ToString());
    expectedUrl += UrlUtility.FormatQueryString (expectedQueryString);

    NavigationCommand command = new NavigationCommand ();
    command.Type = CommandType.WxeFunction;
    command.WxeFunctionCommand.MappingID = mappingID;
    command.WxeFunctionCommand.Parameters = "\"" + parameter1 + "\"";
    string url = command.GetWxeFunctionPermanentUrl ();

    Assert.IsNotNull (url);
    Assert.AreEqual (expectedUrl, url);
  }

  [Test]
  [ExpectedException (typeof (InvalidOperationException))]
  public void GetWxeFunctionPermanentUrlWithMappedFunctionTypeAndInvalidTypeNameMappingIDCombination()
  {
    string mappingID = "Test";
    string resource = "~/Test.wxe";
    Type functionWithNestingType = typeof (ExecutionEngine.TestFunctionWithNesting);
    string functionWithNestingTypeName = WebTypeUtility.GetQualifiedName (functionWithNestingType);
    UrlMappingConfiguration.Current.Mappings.Add (new UrlMappingEntry (mappingID, functionWithNestingType, resource));
    string parameter1 = "Value1";
    
    string expectedUrl = UrlUtility.GetAbsoluteUrl (_currentHttpContext, resource);
    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add ("Parameter1", parameter1);
    expectedQueryString.Add (WxeHandler.Parameters.WxeReturnToSelf, NaBoolean.True.ToString());
    expectedUrl += UrlUtility.FormatQueryString (expectedQueryString);

    NavigationCommand command = new NavigationCommand ();
    command.Type = CommandType.WxeFunction;
    command.WxeFunctionCommand.MappingID = mappingID;
    command.WxeFunctionCommand.TypeName = _functionTypeName;
    command.WxeFunctionCommand.Parameters = "\"" + parameter1 + "\"";
    command.GetWxeFunctionPermanentUrl ();
  }

  [Test]
  public void GetWxeFunctionPermanentUrlWithDefaultWxeHandlerAndAdditionalUrlParameters()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetExecutionEngineWithDefaultWxeHandler();    
    
    string wxeHandler = Rubicon.Web.Configuration.WebConfiguration.Current.ExecutionEngine.DefaultWxeHandler;
    
    NameValueCollection additionalUrlParameters = new NameValueCollection();
    additionalUrlParameters.Add ("Parameter2", "Value2");

    string expectedUrl = UrlUtility.GetAbsoluteUrl (_currentHttpContext, wxeHandler);
    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add ("Parameter1", _wxeFunctionParameter1Value);
    expectedQueryString.Add (WxeHandler.Parameters.WxeReturnToSelf, NaBoolean.True.ToString());
    expectedQueryString.Add (additionalUrlParameters);
    expectedQueryString.Add (WxeHandler.Parameters.WxeFunctionType, _functionTypeName);
    expectedUrl += UrlUtility.FormatQueryString (expectedQueryString);

    NavigationCommand command = new NavigationCommand ();
    command.Type = CommandType.WxeFunction;
    command.WxeFunctionCommand.TypeName = _functionTypeName;
    command.WxeFunctionCommand.Parameters = _wxeFunctionParameters;
    string url = command.GetWxeFunctionPermanentUrl (additionalUrlParameters);

    Assert.IsNotNull (url);
    Assert.AreEqual (expectedUrl, url);
  }

  [Test]
  public void GetWxeFunctionPermanentUrlWithMappedFunctionTypeAndAdditionalUrlParameters()
  {
    string resource = "~/Test.wxe";
    UrlMappingConfiguration.Current.Mappings.Add (new UrlMappingEntry (_functionType, resource));
    string parameter1 = "Value1";

    NameValueCollection additionalUrlParameters = new NameValueCollection();
    additionalUrlParameters.Add ("Parameter2", "Value2");
    
    string expectedUrl = UrlUtility.GetAbsoluteUrl (_currentHttpContext, resource);
    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add ("Parameter1", parameter1);
    expectedQueryString.Add (WxeHandler.Parameters.WxeReturnToSelf, NaBoolean.True.ToString());
    expectedQueryString.Add (additionalUrlParameters);
    expectedUrl += UrlUtility.FormatQueryString (expectedQueryString);

    NavigationCommand command = new NavigationCommand ();
    command.Type = CommandType.WxeFunction;
    command.WxeFunctionCommand.TypeName = _functionTypeName;
    command.WxeFunctionCommand.Parameters = "\"" + parameter1 + "\"";
    string url = command.GetWxeFunctionPermanentUrl (additionalUrlParameters);

    Assert.IsNotNull (url);
    Assert.AreEqual (expectedUrl, url);
  }

  [Test]
  [ExpectedException (typeof (WxeException))]
  public void GetWxeFunctionPermanentUrlWithoutDefaultWxeHandler()
  {
    WebConfigurationMock.Current = null;
    string parameter1 = "Hello World!";
    
    NavigationCommand command = new NavigationCommand ();
    command.Type = CommandType.WxeFunction;
    command.WxeFunctionCommand.TypeName = _functionTypeName;
    command.WxeFunctionCommand.Parameters = "\"" + parameter1 + "\"";
    command.GetWxeFunctionPermanentUrl ();
  }
}

}
