using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Security.Principal;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security.UnitTests.Configuration
{

  public class UserProviderMock : ProviderBase, IUserProvider
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public UserProviderMock ()
    {
    }

    // methods and properties

    public IPrincipal GetUser ()
    {
      throw new NotImplementedException ();
    }
  }
}