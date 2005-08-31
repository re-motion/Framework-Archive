using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Web;
using System.Reflection;
using Rubicon.Utilities;
using Rubicon.Web.Configuration;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Web.ExecutionEngine
{

[Serializable]
public class TestWxeFunctionState: WxeFunctionState
{
  public TestWxeFunctionState (WxeFunction function, int lifetime, bool enableCleanUp)
    : base (function, lifetime, enableCleanUp)
  {
  }

  public TestWxeFunctionState (WxeFunction function, string functionToken, bool enableCleanUp)
    : base (function, functionToken, enableCleanUp)
  {
  }
  
  public TestWxeFunctionState (WxeFunction function, bool enableCleanUp)
    : base (function, enableCleanUp)
  {
  }

  public TestWxeFunctionState (WxeFunction function, string functionToken, int lifetime, bool enableCleanUp)
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
