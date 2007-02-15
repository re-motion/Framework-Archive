using System;
using System.Security.Principal;

namespace Rubicon.Security
{
  //TODO: SD: kapselt security abfrage fuer static access auf businessobject.
  // anmerkugn: nur globales caching. von impl bestimmt
  public interface IFunctionalSecurityStrategy
  {
    // wird von securityclient.hasaccess aufgerufen und mischt securitycontext in die abfrage ein.
    //andere parameter von securityclient.hasaccess bereitgestellt.
    //reqaccesstypes werden von secclient berechnet aus obj-instanz und propertyname/methodenname
    bool HasAccess (Type type, ISecurityProvider securityProvider, IPrincipal user, params AccessType[] requiredAccessTypes);
  }
}
