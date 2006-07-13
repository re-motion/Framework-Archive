using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security
{
  public class NullGlobalAccessTypeCacheProvider : IGlobalAccessTypeCacheProvider
  {
    private NullAccessTypeCache<SecurityContext> _cache = new NullAccessTypeCache<SecurityContext>();

    public NullGlobalAccessTypeCacheProvider ()
    {
    }

    public IAccessTypeCache<SecurityContext> GetAccessTypeCache ()
    {
      return _cache;
    }
  }
}