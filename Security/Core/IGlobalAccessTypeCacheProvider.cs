using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security
{
  public interface IGlobalAccessTypeCacheProvider
  {
    IAccessTypeCache<GlobalAccessTypeCacheKey> GetAccessTypeCache ();
  }
}