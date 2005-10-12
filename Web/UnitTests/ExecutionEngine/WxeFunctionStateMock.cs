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
  public WxeFunctionStateMock (WxeFunction function, string queryString, bool hasMappedUrl, bool enableCleanUp)
    : base (function, queryString, hasMappedUrl, enableCleanUp)
  {
  }
  
  public WxeFunctionStateMock (WxeFunction function, int lifetime, string queryString, bool hasMappedUrl, bool enableCleanUp)
    : base (function, lifetime, queryString, hasMappedUrl, enableCleanUp)
  {
  }
  
  public WxeFunctionStateMock (
      WxeFunction function, int lifetime, string queryString, bool enableCleanUp, bool hasMappedUrl, string functionToken)
    : base (function, lifetime, queryString, hasMappedUrl, enableCleanUp)
  {
    FunctionToken = functionToken;
  }

  public new WxeFunction Function
  {
    get { return base.Function; }
    set {PrivateInvoke.SetNonPublicField (this, "_function", value); }
  }

  public new string FunctionToken
  {
    get { return base.FunctionToken; }
    set {PrivateInvoke.SetNonPublicField (this, "_functionToken", value); }
  }

  public new void Abort()
  {
    base.Abort();
  }
}

}
