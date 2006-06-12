using System;
using Rubicon.Security;
using System.Web.UI;

namespace Rubicon.Web.UI
{
  public interface IWebSecurityProvider : ISecurityProvider
  {
    bool HasAccess (ISecurableObject securableObject, Delegate handler);
    //bool HasStatelessAccess (Type functionType);
    //void CheckAccess (ISecurableObject securableObject, Delegate handler);
  }
}
