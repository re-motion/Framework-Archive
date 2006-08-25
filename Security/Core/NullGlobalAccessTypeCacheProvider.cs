using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Collections;

namespace Rubicon.Security
{
  public class NullGlobalAccessTypeCacheProvider : IGlobalAccessTypeCacheProvider
  {
    private NullCache<Tuple<SecurityContext, string>, AccessType[]> _cache = new NullCache<Tuple<SecurityContext, string>, AccessType[]> ();

    public NullGlobalAccessTypeCacheProvider ()
    {
    }

    public ICache<Tuple<SecurityContext, string>, AccessType[]> GetCache ()
    {
      return _cache;
    }
  }
}