using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.SessionState;
using System.Reflection;
using System.Globalization;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;
using Rubicon.Web.Configuration;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{

/// <summary> Provides a <see cref="WxeContext"/> for simualating ASP.NET request life cycles. </summary>
public class WxeContextMock: WxeContext
{
  public WxeContextMock (HttpContext context)
    : base (context, "Undefined")
  {
  }
}

}