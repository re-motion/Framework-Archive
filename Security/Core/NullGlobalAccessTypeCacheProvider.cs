using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security
{
  public class NullGlobalAccessTypeCacheProvider : IGlobalAccessTypeCacheProvider
  {
    private NullAccessTypeCache<GlobalAccessTypeCacheKey> _cache = new NullAccessTypeCache<GlobalAccessTypeCacheKey> ();

    public NullGlobalAccessTypeCacheProvider ()
    {
    }

    public IAccessTypeCache<GlobalAccessTypeCacheKey> GetAccessTypeCache ()
    {
      return _cache;
    }
  }
}