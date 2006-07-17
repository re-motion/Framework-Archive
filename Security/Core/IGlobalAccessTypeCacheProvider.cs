using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Collections;

namespace Rubicon.Security
{
  public interface IGlobalAccessTypeCacheProvider
  {
    IAccessTypeCache<Tupel<SecurityContext, string>> GetAccessTypeCache ();
  }
}