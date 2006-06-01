using System;
using Rubicon.Security;

namespace Rubicon.Web.ExecutionEngine
{
  public interface IWxeSecurityProvider : ISecurityProvider
  {
    bool HasAccess (WxeFunction function);
    bool HasStatelessAccess (Type functionType);
    void CheckAccess (WxeFunction function);
  }
}
