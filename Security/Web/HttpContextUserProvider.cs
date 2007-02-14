using System;
using System.Configuration.Provider;
using System.Security.Principal;
using System.Web;

namespace Rubicon.Security.Web
{
  public class HttpContextUserProvider : ProviderBase, IUserProvider
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public HttpContextUserProvider()
    {
    }

    // methods and properties

    public IPrincipal GetUser()
    {
      if (HttpContext.Current == null)
        return null;
      else
        return HttpContext.Current.User;
    }

    bool INullableObject.IsNull
    {
      get { return false; }
    }
  }
}