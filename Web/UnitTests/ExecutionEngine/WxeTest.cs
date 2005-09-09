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
using Rubicon.Web.Test.AspNetFramework;

namespace Rubicon.Web.Test.ExecutionEngine
{

public class WxeTest
{
  private HttpContext _currentHttpContext;
  private WxeContextMock _currentWxeContext;

  public HttpContext CurrentHttpContext
  {
    get { return _currentHttpContext; }
  }

  public WxeContextMock CurrentWxeContext
  {
    get { return _currentWxeContext; }
  }

  [SetUp]
  public virtual void SetUp()
  {
    _currentHttpContext = HttpContextHelper.CreateHttpContext (@"C:\default.html", @"http://localhost/default.html", null);
    HttpContextHelper.SetCurrent (_currentHttpContext);

    _currentWxeContext = new WxeContextMock (_currentHttpContext);
    PrivateInvoke.InvokeNonPublicStaticMethod (typeof (WxeContext), "SetCurrent", _currentWxeContext);
  }

  [TearDown]
  public virtual void TearDown()
  {
  }
}

}
