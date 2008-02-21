using System;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.Security;
using Rubicon.Security.Web.ExecutionEngine;
using Rubicon.Security.Web.UI;
using Rubicon.SecurityManager.Clients.Web.Classes;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI;
using SecurityManagerUser = Rubicon.SecurityManager.Domain.OrganizationalStructure.User;

namespace Rubicon.SecurityManager.Clients.Web.Test
{
  public class Global : SecurityManagerHttpApplication
  {
    protected void Application_Start (object sender, EventArgs e)
    {
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IObjectSecurityAdapter), new ObjectSecurityAdapter());
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IWebSecurityAdapter), new WebSecurityAdapter());
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), new WxeSecurityAdapter());
      BindableObjectProvider.Current.AddService (typeof (BindableDomainObjectSearchService), new BindableDomainObjectSearchService());
      BindableObjectProvider.Current.AddService (typeof (BindableDomainObjectGetObjectService), new BindableDomainObjectGetObjectService ());
      BindableObjectProvider.Current.AddService (typeof (GroupPropertiesSearchService), new GroupPropertiesSearchService ());
      BindableObjectProvider.Current.AddService (typeof (RolePropertiesSearchService), new RolePropertiesSearchService ());
    }

    protected void Application_End (object sender, EventArgs e)
    {
    }
  }
}