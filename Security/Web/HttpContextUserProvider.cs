using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Security.Principal;
using System.Web;
using Rubicon.Configuration;

namespace Rubicon.Security.Web
{
  public class HttpContextUserProvider : ExtendedProviderBase, IUserProvider
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public HttpContextUserProvider()
        : this ("HttpContext", new NameValueCollection())
    {
    }

    public HttpContextUserProvider (string name, NameValueCollection config)
        : base (name, config)
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