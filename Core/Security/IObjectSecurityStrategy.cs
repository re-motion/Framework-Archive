using System;
using System.Security.Principal;

namespace Rubicon.Security
{
  //TODO: SD: kapselt security abfrage fuer das businessobject. ueblicherweise kennt die objsecstrat ihr businessobject (wenn auch indirekt) 
  // und greift bei der abfrage auf seinen zustand zurueck.
  // anmerkung: caching nicht defniert. von impl bestimmt
  /// <summary>
  /// Encapsulates the security checks for the business object.
  /// </summary>
  /// <remarks>Typically the <see cref="IObjectSecurityStrategy"/> knows</remarks>
  public interface IObjectSecurityStrategy
  {
    // wird von securityclient.hasaccess aufgerufen und mischt securitycontext in die abfrage ein.
    //andere parameter von securityclient.hasaccess bereitgestellt.
    //reqaccesstypes werden von secclient berechnet aus obj-instanz und propertyname/methodenname
    bool HasAccess (ISecurityProvider securityProvider, IPrincipal user, params AccessType[] requiredAccessTypes);
  }
}
