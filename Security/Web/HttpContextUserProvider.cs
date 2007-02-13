using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Text;
using System.Web;

using Rubicon.Utilities;
using System.Security.Principal;
using System.Threading;

namespace Rubicon.Security.Web
{
  public class HttpContextUserProvider : ProviderBase, IUserProvider
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public HttpContextUserProvider ()
    {
    }

    // methods and properties

    public IPrincipal GetUser ()
    {
      if (HttpContext.Current == null)
        return null;
      else
        return HttpContext.Current.User;
    }
  }
}