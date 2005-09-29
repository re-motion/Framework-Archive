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
  public WxeFunctionStateMock (WxeFunction function, int lifetime, bool enableCleanUp)
    : base (function, lifetime, enableCleanUp)
  {
  }

  public WxeFunctionStateMock (WxeFunction function, string functionToken, bool enableCleanUp)
    : base (function, functionToken, enableCleanUp)
  {
  }
  
  public WxeFunctionStateMock (WxeFunction function, bool enableCleanUp)
    : base (function, enableCleanUp)
  {
  }

  public WxeFunctionStateMock (WxeFunction function, string functionToken, int lifetime, bool enableCleanUp)
    : base (function, functionToken, lifetime, enableCleanUp)
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
