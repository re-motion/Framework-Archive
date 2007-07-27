using System;
using System.Collections.Specialized;
using System.Web;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UnitTests.AspNetFramework;
using Rubicon.Web.UnitTests.ExecutionEngine;

namespace Rubicon.Data.DomainObjects.UnitTests.Web
{
  public class WxeFunctionBaseTest : StandardMappingTest
  {
    private WxeContextMock _context;

    public override void SetUp ()
    {
      _context = new WxeContextMock (WxeContextTest.CreateHttpContext());

      base.SetUp ();
    }

    public WxeContextMock Context
    {
      get { return _context; }
    }
  }
}