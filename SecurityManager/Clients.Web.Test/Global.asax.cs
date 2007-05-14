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
using Rubicon.SecurityManager.Clients.Web.Classes;
using SecurityManagerUser = Rubicon.SecurityManager.Domain.OrganizationalStructure.User;
using Rubicon.Data.DomainObjects;
using System.Threading;

namespace Rubicon.SecurityManager.Clients.Web.Test
{
  public class Global : SecurityManagerHttpApplication
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
  }
}