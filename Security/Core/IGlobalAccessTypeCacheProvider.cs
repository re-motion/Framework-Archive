using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Collections;

namespace Rubicon.Security
{
  public interface IGlobalAccessTypeCacheProvider
  {
    ICache<Tupel<SecurityContext, string>, AccessType[]> GetCache ();
  }
}