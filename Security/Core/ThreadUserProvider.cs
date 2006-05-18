using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using System.Security.Principal;
using System.Threading;

namespace Rubicon.Security
{

  public class ThreadUserProvider : IUserProvider
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
  }
}