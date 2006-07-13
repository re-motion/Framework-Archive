using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

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

    public IAccessTypeCache<SecurityContext> GetAccessTypeCache ()
    {
      throw new NotImplementedException ();
    }
  }
}