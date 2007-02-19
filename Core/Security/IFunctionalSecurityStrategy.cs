using System;
using System.Security.Principal;

namespace Rubicon.Security
{
  //TODO: SD: kapselt security abfrage fuer static access auf businessobject.
  // anmerkugn: nur globales caching. von impl bestimmt

  /// <summary>Encapsulates the security checks for static access to the business object.</summary>
  /// <remarks><note type="implementnotes">Implementations are free to decide whether they provide caching, yet only global caching can be provided.</note></remarks>
  public interface IFunctionalSecurityStrategy
  {
    // wird von securityclient.hasaccess aufgerufen und mischt securitycontext in die abfrage ein.
    //andere parameter von securityclient.hasaccess bereitgestellt.
    //reqaccesstypes werden von secclient berechnet aus obj-instanz und propertyname/methodenname

    /// <summary>Determines whether the requested access is granted.</summary>
    /// <param name="type">the <see cref="Type"/> of the business object.</param>
    /// <param name="securityProvider">The <see cref="ISecurityProvider"/> used to determine the permissions.</param>
    /// <param name="user">The <see cref="IPrincipal"/> on whose behalf the permissions are evaluated.</param>
    /// <param name="requiredAccessTypes">The access rights required for the access to be granted.</param>
    /// <returns><see langword="true"/> if the <paramref name="requiredAccessTypes"/> are granted.</returns>
    /// <remarks>
    /// Typically called via the <see cref="M:Rubicon.Security.SecurityClient.HasAccess"/> method of <see cref="T:Rubicon.Security.SecurityClient"/>
    /// it incorporates the <see cref="SecurityContext"/> in the permission query.
    /// The <paramref name="requiredAccessTypes"/> are determined by the <see cref="T:Rubicon.Security.SecurityClient"/>, 
    /// taking the business object instance and the member name (property or method) into account.
    /// </remarks>
    bool HasAccess (Type type, ISecurityProvider securityProvider, IPrincipal user, params AccessType[] requiredAccessTypes);
  }
}
