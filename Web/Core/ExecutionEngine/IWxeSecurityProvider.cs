using System;
using Rubicon.Security;

namespace Rubicon.Web.ExecutionEngine
{
  public interface IWxeSecurityProvider : ISecurityProvider
  {
    void CheckAccess (WxeFunction function);
    bool HasAccess (Type type);
  }
}
