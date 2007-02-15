using System;
using System.Security.Principal;

namespace Rubicon.Security
{
  //TODO: SD: kapselt die security abfrage. implementierung kann caching bereitstellen. 
  public interface ISecurityStrategy
  {
    // macht security abfrage, 
    // factory.createseccontext soll erst aufgerufen werden, wenn lokaler cache kein ergebnis liefert.
    bool HasAccess (ISecurityContextFactory factory, ISecurityProvider securityProvider, IPrincipal user, params AccessType[] requiredAccessTypes);
    //loescht cache der zu dieser instanz gehoert. von anwendungsprogger aufgerufen wenn sich seccontext relevante eigenschaften des obj aendern.
    void InvalidateLocalCache ();
  }
}
