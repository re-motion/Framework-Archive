using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Text;

using Rubicon.Utilities;
using System.Security.Principal;
using System.Threading;

namespace Rubicon.Security
{

  public class ThreadUserProvider : ProviderBase, IUserProvider
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public ThreadUserProvider ()
    {
    }

    // methods and properties

    public IPrincipal GetUser ()
    {
      return Thread.CurrentPrincipal;
    }

    bool INullableObject.IsNull
    {
      get { return false; }
    }
  }
}