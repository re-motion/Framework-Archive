using System;
using Rubicon.Security;
using System.Web.UI;

namespace Rubicon.Web.UI
{
  [Obsolete ("Use IWebSecurityAdapter instead. (Version: 1.7.41)", true)]
  public interface IWebSecurityProvider : IWebSecurityAdapter, ISecurityProviderObsolete
  {
  }
  
  public interface IWebSecurityAdapter : ISecurityAdapter
  {
    bool HasAccess (ISecurableObject securableObject, Delegate handler);
    //bool HasStatelessAccess (Type functionType);
    //void CheckAccess (ISecurableObject securableObject, Delegate handler);
  }
}
