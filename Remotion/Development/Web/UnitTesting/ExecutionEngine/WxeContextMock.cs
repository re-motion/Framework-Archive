// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.SessionState;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.ExecutionEngine.TestFunctions;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.Infrastructure;

namespace Remotion.Development.Web.UnitTesting.ExecutionEngine
{
  /// <summary> Provides a <see cref="WxeContext"/> for simualating ASP.NET request life cycles. </summary>
  public class WxeContextMock : WxeContext
  {
    public static HttpContext CreateHttpContext (NameValueCollection queryString)
    {
      HttpContext context = HttpContextHelper.CreateHttpContext ("GET", "Other.wxe", null);
      context.Response.ContentEncoding = Encoding.UTF8;
      HttpContextHelper.SetQueryString (context, queryString);
      HttpContextHelper.SetCurrent (context);
      return context;
    }

    public static HttpContext CreateHttpContext ()
    {
      NameValueCollection queryString = new NameValueCollection();
      queryString.Add (WxeHandler.Parameters.ReturnUrl, "/Root.wxe");

      return CreateHttpContext (queryString);
    }

    public WxeContextMock (HttpContext context)
        : base (
            new HttpContextWrapper (context),
            new WxeFunctionStateManager (new HttpSessionStateWrapper (context.Session)),
            new WxeFunctionState (new TestFunction(), false),
            null)
    {
    }

    public WxeContextMock (HttpContext context, NameValueCollection queryString)
        : base (
            new HttpContextWrapper (context),
            new WxeFunctionStateManager (new HttpSessionStateWrapper (context.Session)),
            new WxeFunctionState (new TestFunction(), false),
            queryString)
    {
    }
  }
}
