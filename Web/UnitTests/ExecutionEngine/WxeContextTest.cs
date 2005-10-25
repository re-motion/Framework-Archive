using System;
using System.IO;
using System.Web;
using System.Web.SessionState;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Utilities;
using Rubicon.Web.UnitTests.AspNetFramework;
using Rubicon.Web.Utilities;
using Rubicon.Web.UnitTests.Configuration;

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
    _functionTypeName = _functionType.AssemblyQualifiedName;
    _resource = "~/Test.wxe";

    Rubicon.Web.ExecutionEngine.UrlMapping.UrlMappingConfiguration.Current.Mappings.Add (
        new Rubicon.Web.ExecutionEngine.UrlMapping.UrlMapping (_functionType, _resource));

    _currentHttpContext = HttpContextHelper.CreateHttpContext ("GET", "Other.wxe", null);
    NameValueCollection queryString = new NameValueCollection();
    queryString.Add (WxeHandler.Parameters.WxeReturnUrl, "/Root.wxe");
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
    Rubicon.Web.ExecutionEngine.UrlMapping.UrlMappingConfiguration.SetCurrent (null);
  }

	[Test]
  public void GetPermanentUrlWithEmptyQueryString()
  {
    string expectedUrl = UrlUtility.GetAbsoluteUrl (_currentHttpContext, _resource);
    string permanentUrl = _currentWxeContext.GetPermanentUrl (_functionType, new NameValueCollection());
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
  public void GetPermanentUrlWithQueryStringAndRemoveBothLevels()
  {
    string parameterName = "Param";
    string parameterValue = "Hello World!";

    NameValueCollection queryString = new NameValueCollection();
    queryString.Add (parameterName, parameterValue);

    NameValueCollection expectedQueryString = new NameValueCollection();
    expectedQueryString.Add (queryString);
    
    string parentUrl = _currentHttpContext.Request.Url.AbsolutePath;
    parentUrl += UrlUtility.FormatQueryString (_currentHttpContext.Request.QueryString);
    expectedQueryString.Add (WxeHandler.Parameters.WxeReturnUrl, parentUrl);
    
    string expectedUrl = UrlUtility.GetAbsoluteUrl (_currentHttpContext, _resource);
    expectedUrl += UrlUtility.FormatQueryString (expectedQueryString);

    string permanentUrl = _currentWxeContext.GetPermanentUrl (_functionType, queryString, true);

    Assert.IsNotNull (permanentUrl);
    Assert.AreEqual (expectedUrl, permanentUrl);
  }

  [Test]
  public void GetPermanentUrlWithParentPermanentUrlAndRemoveReturnUrl()
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
  [Ignore ("Not implemented.")]
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
    expectedQueryString.Add (WxeHandler.Parameters.WxeReturnUrl, parentUrl);
    
    string expectedUrl = UrlUtility.GetAbsoluteUrl (_currentHttpContext, _resource);
    expectedUrl += UrlUtility.FormatQueryString (expectedQueryString);

    string permanentUrl = _currentWxeContext.GetPermanentUrl (_functionType, queryString, true);

    Assert.IsNotNull (permanentUrl);
    Assert.AreEqual (expectedUrl, permanentUrl);
  }
}

}
