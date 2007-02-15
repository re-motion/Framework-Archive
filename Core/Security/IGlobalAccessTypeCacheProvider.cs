using System;
using Rubicon.Collections;

namespace Rubicon.Security
{
  public interface IGlobalAccessTypeCacheProvider : INullableObject
  {
    ICache<Tuple<SecurityContext, string>, AccessType[]> GetCache ();
  }
}