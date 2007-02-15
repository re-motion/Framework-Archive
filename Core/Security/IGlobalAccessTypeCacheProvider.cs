using System;
using Rubicon.Collections;

namespace Rubicon.Security
{
  //TODO: SD: cache der als key seccontext und den usernamen (string) verwendet. 
  // diese beiden werden bei jeder abfrage an isecprovider.getaccess uebergeben
  //cache ausraeumen ist nicht definiert und bleibt bei implementierer
  public interface IGlobalAccessTypeCacheProvider : INullableObject
  {
    ICache<Tuple<SecurityContext, string>, AccessType[]> GetCache ();
  }
}