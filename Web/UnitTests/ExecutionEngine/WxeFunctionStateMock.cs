using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Web;
using System.Reflection;
using Rubicon.Utilities;
using Rubicon.Web.Configuration;
using Rubicon.Development.UnitTesting;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{

[Serializable]
public class WxeFunctionStateMock: WxeFunctionState
{
  public WxeFunctionStateMock (WxeFunction function, int lifetime, string queryString, bool enableCleanUp)
    : base (function, lifetime, queryString, enableCleanUp)
  {
  }

  public WxeFunctionStateMock (WxeFunction function, string functionToken, string queryString, bool enableCleanUp)
    : base (function, functionToken, queryString, enableCleanUp)
  {
  }
  
  public WxeFunctionStateMock (WxeFunction function, string queryString, bool enableCleanUp)
    : base (function, queryString, enableCleanUp)
  {
  }

  public WxeFunctionStateMock (WxeFunction function, string functionToken, int lifetime, string queryString, bool enableCleanUp)
    : base (function, functionToken, lifetime, queryString, enableCleanUp)
  {
  }

  public new WxeFunction Function
  {
    get { return base.Function; }
    set {PrivateInvoke.SetNonPublicField (this, "_function", value); }
  }

  public new void Abort()
  {
    base.Abort();
  }
}

}
