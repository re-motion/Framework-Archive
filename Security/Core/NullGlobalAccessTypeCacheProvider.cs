using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Collections;

namespace Rubicon.Security
{
  public class NullGlobalAccessTypeCacheProvider : IGlobalAccessTypeCacheProvider
  {
    private NullCache<Tupel<SecurityContext, string>, AccessType[]> _cache = new NullCache<Tupel<SecurityContext, string>, AccessType[]> ();

    public NullGlobalAccessTypeCacheProvider ()
    {
    }

    public ICache<Tupel<SecurityContext, string>, AccessType[]> GetCache ()
    {
      return _cache;
    }
  }
}