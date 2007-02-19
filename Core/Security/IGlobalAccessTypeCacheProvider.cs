using System;
using Rubicon.Collections;

namespace Rubicon.Security
{
  //TODO: SD: cache der als key seccontext und den usernamen (string) verwendet. 
  // diese beiden werden bei jeder abfrage an isecprovider.getaccess uebergeben
  //cache ausraeumen ist nicht definiert und bleibt bei implementierer

  /// <summary>
  /// Cache for the <see cref="AccessType"/> array, using the <see cref="SecurityContext"/> and the user name (<see cref="string"/>) as key.
  /// These 2 are used as parameters for each call to the <see cref="ISecurityProvider.GetAccess"/> method of <see cref="ISecurityProvider"/>.
  /// </summary>
  /// <remarks><note type="implementnotes">Implementations are free to decide whether they provide clearing the cache.</note></remarks>
  public interface IGlobalAccessTypeCacheProvider : INullableObject
  {
    /// <summary>
    /// Get the <see cref="ICache{T, S}"/> for the <see cref="SecurityContext"/> and user name (<see cref="string"/>) key pair.
    /// </summary>
    /// <returns>The <see cref="ICache{T, S}"/> in use.</returns>
    ICache<Tuple<SecurityContext, string>, AccessType[]> GetCache ();
  }
}