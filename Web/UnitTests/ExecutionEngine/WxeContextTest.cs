using System;
using System.IO;
using System.Web;
using System.Web.SessionState;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.ExecutionEngine.UrlMapping;
using Rubicon.Web.UnitTests.AspNetFramework;
using Rubicon.Web.UnitTests.Configuration;
using Rubicon.Web.Utilities;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{

[TestFixture]
public class WxeContextTest
{
  private HttpContext _currentHttpContext;
  private WxeContextMock _currentWxeContext;
  private Type _functionType;
  private string _functionTypeName;
  private string _resource;

  [SetUp]
  public virtual void SetUp()
  {   
    _functionType = typeof (TestFunction);
    _functionTypeName = _functionType.FullName + "," + _functionType.Assembly.GetName().Name;
    _resource = "~/Test.wxe";

    UrlMappingConfiguration.Current.Mappings.Add (new UrlMappingEntry (_functionType, _resource));

    _currentHttpContext = HttpContextHelper.CreateHttpContext ("GET", "Other.wxe", null);
    _currentHttpContext.Response.ContentEncoding = System.Text.Encoding.UTF8;
    NameValueCollection queryString = new NameValueCollection();
    queryString.Add (WxeHandler.Parameters.ReturnUrl, "/Root.wxe");
    HttpContextHelper.SetQueryString (_currentHttpContext, queryString);
    HttpContextHelper.SetCurrent (_currentHttpContext);

    _currentWxeContext = new WxeContextMock (_currentHttpContext, UrlUtility.FormatQueryString (queryString));
    PrivateInvoke.InvokeNonPublicStaticMethod (typeof (WxeContext), "SetCurrent", _currentWxeContext);

    WebConfigurationMock.Current = new Rubicon.Web.Configuration.WebConfiguration();
    WebConfigurationMock.Current.ExecutionEngine.MaximumUrlLength = 100;

  }

  [TearDown]
  public virtual void TearDown()
  { 
    WebConfigurationMock.Current = null;
    Rubicon.Web.ExecutionEngine.UrlMapping.UrlMappingConfiguration.SetCurrent (null);
  }

	[Test]
  public void GetStaticPermanentUrlWithDefaultWxeHandlerWithoutMappingForFunctionType()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetExecutionEngineWithDefaultWxeHandler();
    Rubicon.Web.ExecutionEngine.UrlMapping.UrlMappingConfiguration.SetCurrent (null);

    string wxeHandler = Rubicon.Web.Configuration.WebConfiguration.Current.ExecutionEngine.DefaultWxeHandler;
    string expectedUrl = UrlUtility.GetAbsoluteUrl (_currentHttpContext, wxeHandler);
    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add (WxeHandler.Parameters.WxeFunctionType, _functionTypeName);
    expectedUrl += UrlUtility.FormatQueryString (expectedQueryString);

    string permanentUrl = WxeContext.GetPermanentUrl (_currentHttpContext, _functionType, new NameValueCollection());
    Assert.IsNotNull (permanentUrl);
    Assert.AreEqual (expectedUrl, permanentUrl);
  }

	[Test]
  public void GetStaticPermanentUrlWithDefaultWxeHandlerForMappedFunctionType()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetExecutionEngineWithDefaultWxeHandler();

    string wxeHandler = Rubicon.Web.Configuration.WebConfiguration.Current.ExecutionEngine.DefaultWxeHandler;
    string expectedUrl = UrlUtility.GetAbsoluteUrl (_currentHttpContext, _resource);
    string permanentUrl = WxeContext.GetPermanentUrl (_currentHttpContext, _functionType, new NameValueCollection());
    Assert.IsNotNull (permanentUrl);
    Assert.AreEqual (expectedUrl, permanentUrl);
  }

	[Test]
  public void GetStaticPermanentUrlWithEmptyQueryString()
  {
    string expectedUrl = UrlUtility.GetAbsoluteUrl (_currentHttpContext, _resource);
    string permanentUrl = WxeContext.GetPermanentUrl (_currentHttpContext, _functionType, new NameValueCollection());
    Assert.IsNotNull (permanentUrl);
    Assert.AreEqual (expectedUrl, permanentUrl);
  }

  [Test]
  public void GetStaticPermanentUrlWithQueryString()
  {
    string parameterName = "Param";
    string parameterValue = "Hello World!";

    NameValueCollection queryString = new NameValueCollection();
    queryString.Add (parameterName, parameterValue);

    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add (queryString);
    string expectedUrl = UrlUtility.GetAbsoluteUrl (_currentHttpContext, _resource);
    expectedUrl += UrlUtility.FormatQueryString (expectedQueryString);

    string permanentUrl = WxeContext.GetPermanentUrl (_currentHttpContext, _functionType, queryString);

    Assert.IsNotNull (permanentUrl);
    Assert.AreEqual (expectedUrl, permanentUrl);
  }

  [Test]
  [ExpectedException (typeof (WxeException))]
  public void GetStaticPermanentUrlWithQueryStringExceedingMaxLength()
  {
    string parameterName = "Param";
    string parameterValue = "123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 ";

    NameValueCollection queryString = new NameValueCollection();
    queryString.Add (parameterName, parameterValue);

    WxeContext.GetPermanentUrl (_currentHttpContext,_functionType, queryString);
    Assert.Fail();
  }

	[Test]
  [ExpectedException (typeof (WxeException))]
  public void GetStaticPermanentUrlWithoutWxeHandler()
  {
    WebConfigurationMock.Current = null;
    Rubicon.Web.ExecutionEngine.UrlMapping.UrlMappingConfiguration.SetCurrent (null);
    WxeContext.GetPermanentUrl (_currentHttpContext, _functionType, new NameValueCollection());
    Assert.Fail();
  }

	[Test]
  public void GetPermanentUrlWithEmptyQueryString()
  {
    string expectedUrl = UrlUtility.GetAbsoluteUrl (_currentHttpContext, _resource);
    string permanentUrl = _currentWxeContext.GetPermanentUrl (_functionType, new NameValueCollection(), false);
    Assert.IsNotNull (permanentUrl);
    Assert.AreEqual (expectedUrl, permanentUrl);
  }

  [Test]
  public void GetPermanentUrlWithQueryString()
  {
    string parameterName = "Param";
    string parameterValue = "Hello World!";

    NameValueCollection queryString = new NameValueCollection();
    queryString.Add (parameterName, parameterValue);

    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add (queryString);
    string expectedUrl = UrlUtility.GetAbsoluteUrl (_currentHttpContext, _resource);
    expectedUrl += UrlUtility.FormatQueryString (expectedQueryString);

    string permanentUrl = _currentWxeContext.GetPermanentUrl (_functionType, queryString, false);

    Assert.IsNotNull (permanentUrl);
    Assert.AreEqual (expectedUrl, permanentUrl);
  }

  [Test]
  [ExpectedException (typeof (WxeException))]
  public void GetPermanentUrlWithQueryStringExceedingMaxLength()
  {
    string parameterName = "Param";
    string parameterValue = "123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 ";

    NameValueCollection queryString = new NameValueCollection();
    queryString.Add (parameterName, parameterValue);

    _currentWxeContext.GetPermanentUrl (_functionType, queryString, false);
    Assert.Fail();
  }

  [Test]
  public void GetPermanentUrlWithQueryStringAndParentPermanentUrl()
  {
    string parameterName = "Param";
    string parameterValue = "Hello World!";

    NameValueCollection queryString = new NameValueCollection();
    queryString.Add (parameterName, parameterValue);

    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add (queryString);
    
    string parentUrl = _currentHttpContext.Request.Url.AbsolutePath;
    parentUrl += UrlUtility.FormatQueryString (_currentHttpContext.Request.QueryString);
    expectedQueryString.Add (WxeHandler.Parameters.ReturnUrl, parentUrl);
    
    string expectedUrl = UrlUtility.GetAbsoluteUrl (_currentHttpContext, _resource);
    expectedUrl += UrlUtility.FormatQueryString (expectedQueryString);

    string permanentUrl = _currentWxeContext.GetPermanentUrl (_functionType, queryString, true);

    Assert.IsNotNull (permanentUrl);
    Assert.AreEqual (expectedUrl, permanentUrl);
  }

  [Test]
  public void GetPermanentUrlWithParentPermanentUrlAndRemoveBothReturnUrls()
  {
    string parameterName = "Param";
    string parameterValue = "123456789 123456789 123456789 123456789 123456789 123456789 ";

    NameValueCollection queryString = new NameValueCollection();
    queryString.Add (parameterName, parameterValue);

    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add (queryString);
        
    string expectedUrl = UrlUtility.GetAbsoluteUrl (_currentHttpContext, _resource);
    expectedUrl += UrlUtility.FormatQueryString (expectedQueryString);

    string permanentUrl = _currentWxeContext.GetPermanentUrl (_functionType, queryString, false);

    Assert.IsNotNull (permanentUrl);
    Assert.AreEqual (expectedUrl, permanentUrl);
  }

  [Test]
  public void GetPermanentUrlWithParentPermanentUrlAndRemoveInnermostReturnUrl()
  {
    string parameterName = "Param";
    string parameterValue = "123456789 123456789 123456789 123456789 ";

    NameValueCollection queryString = new NameValueCollection();
    queryString.Add (parameterName, parameterValue);

    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add (queryString);
    
    string parentUrl = _currentHttpContext.Request.Url.AbsolutePath;
    parentUrl += UrlUtility.FormatQueryString (_currentHttpContext.Request.QueryString);
    parentUrl = PageUtility.DeleteUrlParameter (parentUrl, WxeHandler.Parameters.ReturnUrl);
    expectedQueryString.Add (WxeHandler.Parameters.ReturnUrl, parentUrl);
    
    string expectedUrl = UrlUtility.GetAbsoluteUrl (_currentHttpContext, _resource);
    expectedUrl += UrlUtility.FormatQueryString (expectedQueryString);

    string permanentUrl = _currentWxeContext.GetPermanentUrl (_functionType, queryString, true);

    Assert.IsNotNull (permanentUrl);
    Assert.AreEqual (expectedUrl, permanentUrl);
  }

  [Test]
  [ExpectedException (typeof (ArgumentException))]
  public void GetPermanentUrlWithExistingReturnUrl()
  {
    string parameterName = "Param";
    string parameterValue = "Hello World!";

    NameValueCollection queryString = new NameValueCollection();
    queryString.Add (parameterName, parameterValue);
    queryString.Add (WxeHandler.Parameters.ReturnUrl, "");
    
    _currentWxeContext.GetPermanentUrl (_functionType, queryString, true);
    Assert.Fail();
  }
}

}
