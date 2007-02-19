using System;
using System.Security.Principal;

namespace Rubicon.Security
{
  //TODO: SD: kapselt security abfrage fuer das businessobject. ueblicherweise kennt die objsecstrat ihr businessobject (wenn auch indirekt) 
  // und greift bei der abfrage auf seinen zustand zurueck.
  // anmerkung: caching nicht definiert. von impl bestimmt

  /// <summary>Encapsulates the security checks for the business object.</summary>
  /// <remarks>
  /// Typically the <see cref="IObjectSecurityStrategy"/> knows its business object (possibly indirectly) 
  /// and inspects the object state to determine whether access is granted.
  /// <note type="implementnotes">Implementations are free to decide whether they provide caching.</note>
  /// </remarks>
  public interface IObjectSecurityStrategy
  {
    // wird von securityclient.hasaccess aufgerufen und mischt securitycontext in die abfrage ein.
    //andere parameter von securityclient.hasaccess bereitgestellt.
    //reqaccesstypes werden von secclient berechnet aus obj-instanz und propertyname/methodenname

    /// <summary>Determines whether the requested access is granted.</summary>
    /// <param name="securityProvider">The <see cref="ISecurityProvider"/> used to determine the permissions.</param>
    /// <param name="user">The <see cref="IPrincipal"/> on whose behalf the permissions are evaluated.</param>
    /// <param name="requiredAccessTypes">The access rights required for the access to be granted.</param>
    /// <returns><see langword="true"/> if the <paramref name="requiredAccessTypes"/> are granted.</returns>
    /// <remarks>
    /// Typically called via method <see cref="M:Rubicon.Security.SecurityClient.HasAccess"/> of 
    /// <see cref="T:Rubicon.Security.SecurityClient"/>
    /// it incorporates <see cref="SecurityContext"/> in the permission query.
    /// The <paramref name="requiredAccessTypes"/> are determined by the SecurityClient, 
    /// taking the business object instance and the member name (property or method) into account.
    /// </remarks>
    bool HasAccess (ISecurityProvider securityProvider, IPrincipal user, params AccessType[] requiredAccessTypes);
  }
}
