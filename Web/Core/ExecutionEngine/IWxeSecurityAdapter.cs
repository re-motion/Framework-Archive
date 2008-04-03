using System;
using Remotion.Security;

namespace Remotion.Web.ExecutionEngine
{
  [Obsolete ("Use IWxeSecurityAdapter instead. (Version: 1.7.41)", true)]
  public interface IWxeSecurityProvider : IWxeSecurityAdapter, ISecurityProviderObsolete
  {
  }

  //TODO: SD: definiert adapter für security layer. registiert in secadapterregistry, 
  //verwendet in wxe um security abfragen zu tun.
  public interface IWxeSecurityAdapter : ISecurityAdapter
  {
    // verwendet wenn function läuft. 
    bool HasAccess (WxeFunction function);
    //verwendet bevor wxefunction initialisiert wurde und nur typ bekannt ist.
    bool HasStatelessAccess (Type functionType);
    // verwendet wenn function läuft. zb um zurgriffe auf urls (= wxefunction) zu schützen.
    void CheckAccess (WxeFunction function);
  }
}
