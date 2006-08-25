using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Collections;

namespace Rubicon.Security.UnitTests.Configuration
{
  public class GlobalAccessTypeCacheProviderMock : IGlobalAccessTypeCacheProvider
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public GlobalAccessTypeCacheProviderMock ()
    {
    }

    // methods and properties

    public ICache<Tuple<SecurityContext, string>, AccessType[]> GetCache ()
    {
      throw new NotImplementedException ();
    }
  }
}