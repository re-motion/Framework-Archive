using System;
using System.Collections.Specialized;
using System.Web;
using NUnit.Framework;
using Rubicon.Web.Utilities;
using Rubicon.Web.UnitTests.AspNetFramework;

namespace Rubicon.Web.UnitTests.Utilities
{

[TestFixture]
public class UrlUtilityTest
{
  private HttpContext _currentHttpContext;

  public HttpContext CurrentHttpContext
  {
    get { return _currentHttpContext; }
  }

  [SetUp]
  public virtual void SetUp()
  {
    _currentHttpContext = HttpContextHelper.CreateHttpContext (@"C:\default.html", @"http://localhost/default.html", null);
    _currentHttpContext.Response.ContentEncoding = System.Text.Encoding.UTF8;
    HttpContextHelper.SetCurrent (_currentHttpContext);
  }

  [Test]
  public void FormatQueryString()
  {
    string parameter1 = "Parameter1";
    string parameter2 = "Parameter2";
    string value1 = "Value1";
    string value2 = "Value2";

    NameValueCollection queryString = new NameValueCollection();
    queryString.Add (parameter1, value1);
    queryString.Add (parameter2, value2);

    string expectedQueryString = string.Format (
        "?{0}={1}&{2}={3}", parameter1, HttpUtility.UrlEncode (value1), parameter2, HttpUtility.UrlEncode (value2));

    Assert.AreEqual (expectedQueryString, UrlUtility.FormatQueryString (queryString));
  }

  [Test]
  public void FormatQueryStringNoParameters()
  {
    NameValueCollection queryString = new NameValueCollection();
    Assert.AreEqual ("", UrlUtility.FormatQueryString (queryString));
  }
}

}
