using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Security.Principal;
using Rubicon.Security;
using Rubicon.Web.UI;
using Rubicon.Security.Web.UI;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Security.Web.ExecutionEngine;

namespace Rubicon.SecurityManager.Clients.Web.Test
{
  public class Global : System.Web.HttpApplication
  {

    protected void Application_Start (object sender, EventArgs e)
    {
      SecurityAdapterRegistry.Instance.SetAdapter<IObjectSecurityAdapter> (new ObjectSecurityAdapter ());
      SecurityAdapterRegistry.Instance.SetAdapter<IWebSecurityAdapter> (new WebSecurityAdapter ());
      SecurityAdapterRegistry.Instance.SetAdapter<IWxeSecurityAdapter> (new WxeSecurityAdapter ());
    }

    protected void Application_End (object sender, EventArgs e)
    {

    }

    protected void Application_PostAcquireRequestState (object sender, EventArgs e)
    {
      if (Context.Handler is IRequiresSessionState || Context.Handler is IReadOnlySessionState)
      {
        IPrincipal user = (IPrincipal) Session["CurrentUser"];
        if (user == null)
          HttpContext.Current.User = new GenericPrincipal (new GenericIdentity (string.Empty), new string[0]);
        else
          HttpContext.Current.User = user;
      }
    }


    public void SetUser (IPrincipal user)
    {
      HttpContext.Current.User = user;
      Session["CurrentUser"] = user;
    }
  }
}