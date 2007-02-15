using System;
using Rubicon.Security;

namespace Rubicon.Web.ExecutionEngine
{
  [Obsolete ("Use IWxeSecurityAdapter instead. (Version: 1.7.41)", true)]
  public interface IWxeSecurityProvider : IWxeSecurityAdapter, ISecurityProviderObsolete
  {
  }
 
  public interface IWxeSecurityAdapter : ISecurityAdapter
  {
    bool HasAccess (WxeFunction function);
    bool HasStatelessAccess (Type functionType);
    void CheckAccess (WxeFunction function);
  }
}
