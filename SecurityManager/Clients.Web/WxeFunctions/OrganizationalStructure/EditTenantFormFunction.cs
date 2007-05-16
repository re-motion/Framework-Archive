using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Rubicon.SecurityManager.Clients.Web.Classes;
using Rubicon.Data.DomainObjects;
using Rubicon.Web.ExecutionEngine;
using Rubicon.SecurityManager.Clients.Web.UI.OrganizationalStructure;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Data.DomainObjects.Web.ExecutionEngine;
using Rubicon.SecurityManager.Configuration;

namespace Rubicon.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure
{
  public class EditTenantFormFunction : FormFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public EditTenantFormFunction ()
    {
    }

    protected EditTenantFormFunction (params object[] args)
      : base (args)
    {
    }

    public EditTenantFormFunction (ObjectID organizationalStructureObjectID)
      : base (organizationalStructureObjectID)
    {
    }

    // methods and properties
    public Tenant Tenant
    {
      get { return (Tenant) CurrentObject; }
      set { CurrentObject = value; }
    }

    private void Step1 ()
    {
      // TODO check CurrentTransaction
      if (CurrentObject == null)
      {
        Tenant = SecurityManagerConfiguration.Current.OrganizationalStructureFactory.CreateTenant (CurrentTransaction);
      }
    }

    WxeResourcePageStep Step2 = new WxeResourcePageStep (typeof (EditTenantForm), "UI/OrganizationalStructure/EditTenantForm.aspx");
  }
}
