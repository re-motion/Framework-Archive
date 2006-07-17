using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Collections;

namespace Rubicon.Security
{
  public class NullGlobalAccessTypeCacheProvider : IGlobalAccessTypeCacheProvider
  {
    private NullAccessTypeCache<Tupel<SecurityContext, string>> _cache = new NullAccessTypeCache<Tupel<SecurityContext, string>> ();

    public NullGlobalAccessTypeCacheProvider ()
    {
    }

    public IAccessTypeCache<Tupel<SecurityContext, string>> GetAccessTypeCache ()
    {
      return _cache;
    }
  }
}