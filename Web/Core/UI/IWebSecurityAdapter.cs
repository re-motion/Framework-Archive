using System;
using Remotion.Security;
using System.Web.UI;

namespace Remotion.Web.UI
{
  [Obsolete ("Use IWebSecurityAdapter instead. (Version: 1.7.41)", true)]
  public interface IWebSecurityProvider : IWebSecurityAdapter, ISecurityProviderObsolete
  {
  }

  //TODO: SD: definiert adapter für security layer. registiert in secadapterregistry, 
  //verwendet in web-controls um security abfragen zu tun.
  public interface IWebSecurityAdapter : ISecurityAdapter
  {
    //verwendet fuer buttons etc, secObj = isntanz fur die sec gecheckt wird. handler ist eventhandler von butonclock etc der geschuetz werden soll.
    bool HasAccess (ISecurableObject securableObject, Delegate handler);
    //bool HasStatelessAccess (Type functionType);
    //void CheckAccess (ISecurableObject securableObject, Delegate handler);
  }
}
